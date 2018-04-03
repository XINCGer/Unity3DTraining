using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class CharacterGenerator
{
	static WWW database;
	static Dictionary<string, Dictionary<string, List<CharacterElement>>> sortedElements;
	static List<string> availableCharacters = new List<string>();
	static Dictionary<string, WWW> characterBaseWWWs = new Dictionary<string, WWW>();
	static Dictionary<string, AssetBundleRequest> characterBaseRequests = new Dictionary<string, AssetBundleRequest>();
	string currentCharacter;
	Dictionary<string, CharacterElement> currentConfiguration = new Dictionary<string, CharacterElement>();
	float assetbundlesAlreadyDownloaded;

	public static bool ReadyToUse 
	{
		get
		{
			if (database == null)
				database = new WWW(CharacterElement.AssetbundleBaseURL + "CharacterElementDatabase.assetbundle");
			
			if (sortedElements != null) return true;
			if (!database.isDone) return false;

			//Debug.Log (database.assetBundle.mainAsset);
			CharacterElementHolder ceh = database.assetBundle.mainAsset as CharacterElementHolder;
			if(ceh == null) return false;
			sortedElements = new Dictionary<string, Dictionary<string, List<CharacterElement>>>();
			foreach (CharacterElement element in ceh.content)
			{
				string[] a = element.bundleName.Split('_');
				string character = a[0];
				string category = a[1].Split('-')[0].Replace(".assetbundle", "");
				
				if (!availableCharacters.Contains(character))
					availableCharacters.Add(character);
				
				if (!sortedElements.ContainsKey(character))
					sortedElements.Add(character, new Dictionary<string, List<CharacterElement>>());
				
				if (!sortedElements[character].ContainsKey(category))
					sortedElements[character].Add(category, new List<CharacterElement>());
				
				sortedElements[character][category].Add(element);
			}

			return true;
		}
	}

	public GameObject Generate(GameObject root)
	{
		List<CombineInstance> combineInstances = new List<CombineInstance>();
		List<Material> materials = new List<Material>();
		List<Transform> bones = new List<Transform>();
		Transform[] transforms = root.GetComponentsInChildren<Transform>();
		
		foreach (CharacterElement element in currentConfiguration.Values)
		{
			Debug.Log ("Element的bundleName = " + element.bundleName);
			//肢体部件处理逻辑
			SkinnedMeshRenderer smr = element.GetSkinnedMeshRenderer();
			materials.AddRange(smr.materials);
			for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
			}
			
			foreach (string bone in element.GetBoneNames())
			{
				foreach (Transform transform in transforms)
				{
					if (transform.name != bone) continue;
					bones.Add(transform);
					break;
				}
			}
			
			Object.Destroy(smr.gameObject);
		}
		
		SkinnedMeshRenderer r = root.GetComponent<SkinnedMeshRenderer>();
		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
		r.bones = bones.ToArray();
		r.materials = materials.ToArray();
		
		return root;
	}

	public GameObject Generate()
	{
		GameObject root = (GameObject)Object.Instantiate(characterBaseRequests[currentCharacter].asset);
		root.name = currentCharacter;
		return Generate(root);
	}

	public void ChangeCharacter(bool next)
	{
		string character = null;
		for (int i = 0; i < availableCharacters.Count; i++)
		{
			if (availableCharacters[i] != currentCharacter) continue;
			if (next)
				character = i < availableCharacters.Count - 1 ? availableCharacters[i + 1] : availableCharacters[0];
			else
				character = i > 0 ? availableCharacters[i - 1] : availableCharacters[availableCharacters.Count - 1];
			break;
		}
		PrepareRandomConfig(character);
	}

	void UpdateAssetbundlesAlreadyDownloaded()
	{
		assetbundlesAlreadyDownloaded = CurrentCharacterBase.progress;
		foreach (CharacterElement e in currentConfiguration.Values)
			assetbundlesAlreadyDownloaded += e.WWW.progress;
	}

	public void PrepareRandomConfig(string character)
	{
		currentConfiguration.Clear();
		currentCharacter = character.ToLower();
		foreach (KeyValuePair<string, List<CharacterElement>> category in sortedElements[currentCharacter])
			currentConfiguration.Add(category.Key, category.Value[Random.Range(0, category.Value.Count)]);
		UpdateAssetbundlesAlreadyDownloaded();
	}

	public void ChangeElement(string catagory, bool next)
	{
		List<CharacterElement> available = sortedElements[currentCharacter][catagory];
		CharacterElement element = null;
		for (int i = 0; i < available.Count; i++)
		{
			if (available[i] != currentConfiguration[catagory]) continue;
			if (next)
				element = i < available.Count - 1 ? available[i + 1] : available[0];
			else
				element = i > 0 ? available[i - 1] : available[available.Count - 1];
			break;
		}
		currentConfiguration[catagory] = element;
		UpdateAssetbundlesAlreadyDownloaded();
	}

	public static CharacterGenerator CreateWithRandomConfig(string character)
	{
		CharacterGenerator gen = new CharacterGenerator();
		gen.PrepareRandomConfig(character);
		return gen;
	}

	WWW CurrentCharacterBase
	{
		get
		{
			if (!characterBaseWWWs.ContainsKey(currentCharacter))
				characterBaseWWWs.Add(currentCharacter, new WWW(CharacterElement.AssetbundleBaseURL + currentCharacter + "_characterbase.assetbundle"));
			return characterBaseWWWs[currentCharacter];
		}
	}

	public bool ConfigReady
	{
		get
		{
			if (!CurrentCharacterBase.isDone) return false;
			
			if (!characterBaseRequests.ContainsKey(currentCharacter))
				characterBaseRequests.Add(currentCharacter, CurrentCharacterBase.assetBundle.LoadAssetAsync("characterbase", typeof(GameObject)));
			
			if (!characterBaseRequests[currentCharacter].isDone) return false;
			
			foreach (CharacterElement c in currentConfiguration.Values)
				if (!c.IsLoaded) return false;
			
			return true;
		}
	}

	public float CurrentConfigProgress
	{
		get
		{
			float toDownload = currentConfiguration.Count + 1 - assetbundlesAlreadyDownloaded;
			if (toDownload == 0) return 1;
			float progress = CurrentCharacterBase.progress;
			foreach (CharacterElement e in currentConfiguration.Values)
				progress += e.WWW.progress;
			return (progress - assetbundlesAlreadyDownloaded) / toDownload;
		}
	}

	public string GetConfig()
	{
		string s = currentCharacter;
		foreach (KeyValuePair<string, CharacterElement> category in currentConfiguration)
			s += "|" + category.Key + "|" + category.Value.name;
		return s;
	}

	public void PrepareConfig(string config)
	{
		config = config.ToLower();
		string[] settings = config.Split('|');
		currentCharacter = settings[0];
		currentConfiguration = new Dictionary<string, CharacterElement>();
		for (int i = 1; i < settings.Length; )
		{
			string categoryName = settings[i++];
			string elementName = settings[i++];
			CharacterElement element = null;
			foreach (CharacterElement e in sortedElements[currentCharacter][categoryName])
			{
				if (e.name != elementName) continue;
				element = e;
				break;
			}
			if (element == null) throw new System.Exception("未找到Element: " + elementName);
			currentConfiguration.Add(categoryName, element);
		}
		UpdateAssetbundlesAlreadyDownloaded();
	}

	public static CharacterGenerator CreateWithConfig(string config)
	{
		CharacterGenerator gen = new CharacterGenerator();
		gen.PrepareConfig(config);
		return gen;
	}
}
