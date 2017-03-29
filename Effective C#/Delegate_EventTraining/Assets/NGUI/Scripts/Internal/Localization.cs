//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Localization manager is able to parse localization information from text assets.
/// Using it is simple: text = Localization.Get(key), or just add a UILocalize script to your labels.
/// You can switch the language by using Localization.language = "French", for example.
/// This will attempt to load the file called "French.txt" in the Resources folder,
/// or a column "French" from the Localization.csv file in the Resources folder.
/// If going down the TXT language file route, it's expected that the file is full of key = value pairs, like so:
/// 
/// LABEL1 = Hello
/// LABEL2 = Music
/// Info = Localization Example
/// 
/// In the case of the CSV file, the first column should be the "KEY". Other columns
/// should be your localized text values, such as "French" for the first row:
/// 
/// KEY,English,French
/// LABEL1,Hello,Bonjour
/// LABEL2,Music,Musique
/// Info,"Localization Example","Par exemple la localisation"
/// </summary>

static public class Localization
{
	public delegate byte[] LoadFunction (string path);
	public delegate void OnLocalizeNotification ();

	/// <summary>
	/// Want to have Localization loading be custom instead of just Resources.Load? Set this function.
	/// </summary>

	static public LoadFunction loadFunction;

	/// <summary>
	/// Notification triggered when the localization data gets changed, such as when changing the language.
	/// If you want to make modifications to the localization data after it was loaded, this is the place.
	/// </summary>

	static public OnLocalizeNotification onLocalize;

	/// <summary>
	/// Whether the localization dictionary has been loaded.
	/// </summary>
 
	static public bool localizationHasBeenSet = false;

	// Loaded languages, if any
	static string[] mLanguages = null;

	// Key = Value dictionary (single language)
	static Dictionary<string, string> mOldDictionary = new Dictionary<string, string>();

	// Key = Values dictionary (multiple languages)
	static Dictionary<string, string[]> mDictionary = new Dictionary<string, string[]>();

	// Replacement dictionary forces a specific value instead of the existing entry
	static Dictionary<string, string> mReplacement = new Dictionary<string, string>();

	// Index of the selected language within the multi-language dictionary
	static int mLanguageIndex = -1;

	// Currently selected language
	static string mLanguage;

	/// <summary>
	/// Localization dictionary. Dictionary key is the localization key.
	/// Dictionary value is the list of localized values (columns in the CSV file).
	/// </summary>

	static public Dictionary<string, string[]> dictionary
	{
		get
		{
			if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "English"));
			return mDictionary;
		}
		set
		{
			localizationHasBeenSet = (value != null);
			mDictionary = value;
		}
	}

	/// <summary>
	/// List of loaded languages. Available if a single Localization.csv file was used.
	/// </summary>

	static public string[] knownLanguages
	{
		get
		{
			if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "English"));
			return mLanguages;
		}
	}

	/// <summary>
	/// Name of the currently active language.
	/// </summary>

	static public string language
	{
		get
		{
			if (string.IsNullOrEmpty(mLanguage))
			{
				mLanguage = PlayerPrefs.GetString("Language", "English");
				LoadAndSelect(mLanguage);
			}
			return mLanguage;
		}
		set
		{
			if (mLanguage != value)
			{
				mLanguage = value;
				LoadAndSelect(value);
			}
		}
	}

	/// <summary>
	/// Load the specified localization dictionary.
	/// </summary>

	static bool LoadDictionary (string value)
	{
		// Try to load the Localization CSV
		byte[] bytes = null;

		if (!localizationHasBeenSet)
		{
			if (loadFunction == null)
			{
				TextAsset asset = Resources.Load<TextAsset>("Localization");
				if (asset != null) bytes = asset.bytes;
			}
			else bytes = loadFunction("Localization");
			localizationHasBeenSet = true;
		}

		// Try to load the localization file
		if (LoadCSV(bytes)) return true;
		if (string.IsNullOrEmpty(value)) value = mLanguage;

		// If this point was reached, the localization file was not present
		if (string.IsNullOrEmpty(value)) return false;

		// Not a referenced asset -- try to load it dynamically
		if (loadFunction == null)
		{
			TextAsset asset = Resources.Load<TextAsset>(value);
			if (asset != null) bytes = asset.bytes;
		}
		else bytes = loadFunction(value);

		if (bytes != null)
		{
			Set(value, bytes);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Load the specified language.
	/// </summary>

	static bool LoadAndSelect (string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			if (mDictionary.Count == 0 && !LoadDictionary(value)) return false;
			if (SelectLanguage(value)) return true;
		}

		// Old style dictionary
		if (mOldDictionary.Count > 0) return true;

		// Either the language is null, or it wasn't found
		mOldDictionary.Clear();
		mDictionary.Clear();
		if (string.IsNullOrEmpty(value)) PlayerPrefs.DeleteKey("Language");
		return false;
	}

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Load (TextAsset asset)
	{
		ByteReader reader = new ByteReader(asset);
		Set(asset.name, reader.ReadDictionary());
	}

	/// <summary>
	/// Set the localization data directly.
	/// </summary>

	static public void Set (string languageName, byte[] bytes)
	{
		ByteReader reader = new ByteReader(bytes);
		Set(languageName, reader.ReadDictionary());
	}

	/// <summary>
	/// Forcefully replace the specified key with another value.
	/// </summary>

	static public void ReplaceKey (string key, string val)
	{
		if (!string.IsNullOrEmpty(val)) mReplacement[key] = val;
		else mReplacement.Remove(key);
	}

	/// <summary>
	/// Clear the replacement values.
	/// </summary>

	static public void ClearReplacements () { mReplacement.Clear(); }

	/// <summary>
	/// Load the specified CSV file.
	/// </summary>

	static public bool LoadCSV (TextAsset asset, bool merge = false) { return LoadCSV(asset.bytes, asset, merge); }

	/// <summary>
	/// Load the specified CSV file.
	/// </summary>

	static public bool LoadCSV (byte[] bytes, bool merge = false) { return LoadCSV(bytes, null, merge); }

	static bool mMerging = false;

	/// <summary>
	/// Whether the specified language is present in the localization.
	/// </summary>

	static bool HasLanguage (string languageName)
	{
		for (int i = 0, imax = mLanguages.Length; i < imax; ++i)
			if (mLanguages[i] == languageName) return true;
		return false;
	}

	/// <summary>
	/// Load the specified CSV file.
	/// </summary>

	static bool LoadCSV (byte[] bytes, TextAsset asset, bool merge = false)
	{
		if (bytes == null) return false;
		ByteReader reader = new ByteReader(bytes);

		// The first line should contain "KEY", followed by languages.
		BetterList<string> header = reader.ReadCSV();

		// There must be at least two columns in a valid CSV file
		if (header.size < 2) return false;
		header.RemoveAt(0);

		string[] languagesToAdd = null;
		if (string.IsNullOrEmpty(mLanguage)) localizationHasBeenSet = false;

		// Clear the dictionary
		if (!localizationHasBeenSet || (!merge && !mMerging) || mLanguages == null || mLanguages.Length == 0)
		{
			mDictionary.Clear();
			mLanguages = new string[header.size];

			if (!localizationHasBeenSet)
			{
				mLanguage = PlayerPrefs.GetString("Language", header[0]);
				localizationHasBeenSet = true;
			}

			for (int i = 0; i < header.size; ++i)
			{
				mLanguages[i] = header[i];
				if (mLanguages[i] == mLanguage)
					mLanguageIndex = i;
			}
		}
		else
		{
			languagesToAdd = new string[header.size];
			for (int i = 0; i < header.size; ++i) languagesToAdd[i] = header[i];

			// Automatically resize the existing languages and add the new language to the mix
			for (int i = 0; i < header.size; ++i)
			{
				if (!HasLanguage(header[i]))
				{
					int newSize = mLanguages.Length + 1;
#if UNITY_FLASH
					string[] temp = new string[newSize];
					for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = mLanguages[b];
					mLanguages = temp;
#else
					System.Array.Resize(ref mLanguages, newSize);
#endif
					mLanguages[newSize - 1] = header[i];

					Dictionary<string, string[]> newDict = new Dictionary<string, string[]>();

					foreach (KeyValuePair<string, string[]> pair in mDictionary)
					{
						string[] arr = pair.Value;
#if UNITY_FLASH
						temp = new string[newSize];
						for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = arr[b];
						arr = temp;
#else
						System.Array.Resize(ref arr, newSize);
#endif
						arr[newSize - 1] = arr[0];
						newDict.Add(pair.Key, arr);
					}
					mDictionary = newDict;
				}
			}
		}

		Dictionary<string, int> languageIndices = new Dictionary<string, int>();
		for (int i = 0; i < mLanguages.Length; ++i)
			languageIndices.Add(mLanguages[i], i);

		// Read the entire CSV file into memory
		for (;;)
		{
			BetterList<string> temp = reader.ReadCSV();
			if (temp == null || temp.size == 0) break;
			if (string.IsNullOrEmpty(temp[0])) continue;
			AddCSV(temp, languagesToAdd, languageIndices);
		}

		if (!mMerging && onLocalize != null)
		{
			mMerging = true;
			OnLocalizeNotification note = onLocalize;
			onLocalize = null;
			note();
			onLocalize = note;
			mMerging = false;
		}
		return true;
	}

	/// <summary>
	/// Helper function that adds a single line from a CSV file to the localization list.
	/// </summary>

	static void AddCSV (BetterList<string> newValues, string[] newLanguages, Dictionary<string, int> languageIndices)
	{
		if (newValues.size < 2) return;
		string key = newValues[0];
		if (string.IsNullOrEmpty(key)) return;
		string[] copy = ExtractStrings(newValues, newLanguages, languageIndices);

		if (mDictionary.ContainsKey(key))
		{
			mDictionary[key] = copy;
			if (newLanguages == null) Debug.LogWarning("Localization key '" + key + "' is already present");
		}
		else
		{
			try
			{
				mDictionary.Add(key, copy);
			}
			catch (System.Exception ex)
			{
				Debug.LogError("Unable to add '" + key + "' to the Localization dictionary.\n" + ex.Message);
			}
		}
	}

	/// <summary>
	/// Used to merge separate localization files into one.
	/// </summary>

	static string[] ExtractStrings (BetterList<string> added, string[] newLanguages, Dictionary<string, int> languageIndices)
	{
		if (newLanguages == null)
		{
			string[] values = new string[mLanguages.Length];
			for (int i = 1, max = Mathf.Min(added.size, values.Length + 1); i < max; ++i)
				values[i - 1] = added[i];
			return values;
		}
		else
		{
			string[] values;
			string s = added[0];

			if (!mDictionary.TryGetValue(s, out values))
				values = new string[mLanguages.Length];

			for (int i = 0, imax = newLanguages.Length; i < imax; ++i)
			{
				string language = newLanguages[i];
				int index = languageIndices[language];
				values[index] = added[i + 1];
			}
			return values;
		}
	}

	/// <summary>
	/// Select the specified language from the previously loaded CSV file.
	/// </summary>

	static bool SelectLanguage (string language)
	{
		mLanguageIndex = -1;

		if (mDictionary.Count == 0) return false;

		for (int i = 0, imax = mLanguages.Length; i < imax; ++i)
		{
			if (mLanguages[i] == language)
			{
				mOldDictionary.Clear();
				mLanguageIndex = i;
				mLanguage = language;
				PlayerPrefs.SetString("Language", mLanguage);
				if (onLocalize != null) onLocalize();
				UIRoot.Broadcast("OnLocalize");
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Load the specified asset and activate the localization.
	/// </summary>

	static public void Set (string languageName, Dictionary<string, string> dictionary)
	{
		mLanguage = languageName;
		PlayerPrefs.SetString("Language", mLanguage);
		mOldDictionary = dictionary;
		localizationHasBeenSet = true;
		mLanguageIndex = -1;
		mLanguages = new string[] { languageName };
		if (onLocalize != null) onLocalize();
		UIRoot.Broadcast("OnLocalize");
	}

	/// <summary>
	/// Change or set the localization value for the specified key.
	/// Note that this method only supports one fallback language, and should
	/// ideally be called from within Localization.onLocalize.
	/// To set the multi-language value just modify Localization.dictionary directly.
	/// </summary>

	static public void Set (string key, string value)
	{
		if (mOldDictionary.ContainsKey(key)) mOldDictionary[key] = value;
		else mOldDictionary.Add(key, value);
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Get (string key)
	{
		if (string.IsNullOrEmpty(key)) return null;

		// Ensure we have a language to work with
		if (!localizationHasBeenSet) LoadDictionary(PlayerPrefs.GetString("Language", "English"));

		if (mLanguages == null)
		{
			Debug.LogError("No localization data present");
			return null;
		}

		string lang = language;

		if (mLanguageIndex == -1)
		{
			for (int i = 0; i < mLanguages.Length; ++i)
			{
				if (mLanguages[i] == lang)
				{
					mLanguageIndex = i;
					break;
				}
			}
		}

		if (mLanguageIndex == -1)
		{
			mLanguageIndex = 0;
			mLanguage = mLanguages[0];
			Debug.LogWarning("Language not found: " + lang);
		}

		string val;
		string[] vals;

		UICamera.ControlScheme scheme = UICamera.currentScheme;


		if (scheme == UICamera.ControlScheme.Touch)
		{
			string altKey = key + " Mobile";
			if (mReplacement.TryGetValue(altKey, out val)) return val;

			if (mLanguageIndex != -1 && mDictionary.TryGetValue(altKey, out vals))
			{
				if (mLanguageIndex < vals.Length)
					return vals[mLanguageIndex];
			}
			if (mOldDictionary.TryGetValue(altKey, out val)) return val;
		}
		else if (scheme == UICamera.ControlScheme.Controller)
		{
			string altKey = key + " Controller";
			if (mReplacement.TryGetValue(altKey, out val)) return val;

			if (mLanguageIndex != -1 && mDictionary.TryGetValue(altKey, out vals))
			{
				if (mLanguageIndex < vals.Length)
					return vals[mLanguageIndex];
			}
			if (mOldDictionary.TryGetValue(altKey, out val)) return val;
		}

		if (mReplacement.TryGetValue(key, out val)) return val;

		if (mLanguageIndex != -1 && mDictionary.TryGetValue(key, out vals))
		{
			if (mLanguageIndex < vals.Length)
			{
				string s = vals[mLanguageIndex];
				if (string.IsNullOrEmpty(s)) s = vals[0];
				return s;
			}
			return vals[0];
		}
		if (mOldDictionary.TryGetValue(key, out val)) return val;

#if UNITY_EDITOR
		Debug.LogWarning("Localization key not found: '" + key + "' for language " + lang);
#endif
		return key;
	}

	/// <summary>
	/// Localize the specified value and format it.
	/// </summary>

	static public string Format (string key, params object[] parameters) { return string.Format(Get(key), parameters); }

	[System.Obsolete("Localization is now always active. You no longer need to check this property.")]
	static public bool isActive { get { return true; } }

	[System.Obsolete("Use Localization.Get instead")]
	static public string Localize (string key) { return Get(key); }

	/// <summary>
	/// Returns whether the specified key is present in the localization dictionary.
	/// </summary>

	static public bool Exists (string key)
	{
		// Ensure we have a language to work with
		if (!localizationHasBeenSet) language = PlayerPrefs.GetString("Language", "English");

#if UNITY_IPHONE || UNITY_ANDROID
		string mobKey = key + " Mobile";
		if (mDictionary.ContainsKey(mobKey)) return true;
		else if (mOldDictionary.ContainsKey(mobKey)) return true;
#endif
		return mDictionary.ContainsKey(key) || mOldDictionary.ContainsKey(key);
	}

	/// <summary>
	/// Add a new entry to the localization dictionary.
	/// </summary>

	static public void Set (string language, string key, string text)
	{
		// Check existing languages first
		string[] kl = knownLanguages;
		
		if (kl == null)
		{
			mLanguages = new string[] { language };
			kl = mLanguages;
		}

		for (int i = 0, imax = kl.Length; i < imax; ++i)
		{
			// Language match
			if (kl[i] == language)
			{
				string[] vals;

				// Get all language values for the desired key
				if (!mDictionary.TryGetValue(key, out vals))
				{
					vals = new string[kl.Length];
					mDictionary[key] = vals;
					vals[0] = text;
				}

				// Assign the value for this language
				vals[i] = text;
				return;
			}
		}

		// Expand the dictionary to include this new language
		int newSize = mLanguages.Length + 1;
#if UNITY_FLASH
		string[] temp = new string[newSize];
		for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = mLanguages[b];
		mLanguages = temp;
#else
		System.Array.Resize(ref mLanguages, newSize);
#endif
		mLanguages[newSize - 1] = language;

		Dictionary<string, string[]> newDict = new Dictionary<string, string[]>();

		foreach (KeyValuePair<string, string[]> pair in mDictionary)
		{
			string[] arr = pair.Value;
#if UNITY_FLASH
			temp = new string[newSize];
			for (int b = 0, bmax = arr.Length; b < bmax; ++b) temp[b] = arr[b];
			arr = temp;
#else
			System.Array.Resize(ref arr, newSize);
#endif
			arr[newSize - 1] = arr[0];
			newDict.Add(pair.Key, arr);
		}
		mDictionary = newDict;

		// Set the new value
		string[] values;

		if (!mDictionary.TryGetValue(key, out values))
		{
			values = new string[kl.Length];
			mDictionary[key] = values;
			values[0] = text;
		}
		values[newSize - 1] = text;
	}
}
