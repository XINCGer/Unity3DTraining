//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY || UNITY_WINRT || UNITY_METRO)
#define MOBILE
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Input field makes it possible to enter custom information within the UI.
/// </summary>

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard,
		AutoCorrect,
		Password,
	}

	public enum Validation
	{
		None,
		Integer,
		Float,
		Alphanumeric,
		Username,
		Name,
		Filename,
	}

#if UNITY_EDITOR
	public enum KeyboardType
	{
		Default = (int)TouchScreenKeyboardType.Default,
		ASCIICapable = (int)TouchScreenKeyboardType.ASCIICapable,
		NumbersAndPunctuation = (int)TouchScreenKeyboardType.NumbersAndPunctuation,
		URL = (int)TouchScreenKeyboardType.URL,
		NumberPad = (int)TouchScreenKeyboardType.NumberPad,
		PhonePad = (int)TouchScreenKeyboardType.PhonePad,
		NamePhonePad = (int)TouchScreenKeyboardType.NamePhonePad,
		EmailAddress = (int)TouchScreenKeyboardType.EmailAddress,
	}
#else
	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7,
	}
#endif

	public enum OnReturnKey
	{
		Default,
		Submit,
		NewLine,
	}

	public delegate char OnValidate (string text, int charIndex, char addedChar);

	/// <summary>
	/// Currently active input field. Only valid during callbacks.
	/// </summary>

	static public UIInput current;

	/// <summary>
	/// Currently selected input field, if any.
	/// </summary>

	static public UIInput selection;

	/// <summary>
	/// Text label used to display the input's value.
	/// </summary>

	public UILabel label;

	/// <summary>
	/// Type of data expected by the input field.
	/// </summary>

	public InputType inputType = InputType.Standard;

	/// <summary>
	/// What to do when the Return key is pressed on the keyboard.
	/// </summary>

	public OnReturnKey onReturnKey = OnReturnKey.Default;

	/// <summary>
	/// Keyboard type applies to mobile keyboards that get shown.
	/// </summary>

	public KeyboardType keyboardType = KeyboardType.Default;

	/// <summary>
	/// Whether the input will be hidden on mobile platforms.
	/// </summary>

	public bool hideInput = false;

	/// <summary>
	/// Whether all text will be selected when the input field gains focus.
	/// </summary>

	[System.NonSerialized]
	public bool selectAllTextOnFocus = true;

	/// <summary>
	/// What kind of validation to use with the input field's data.
	/// </summary>

	public Validation validation = Validation.None;

	/// <summary>
	/// Maximum number of characters allowed before input no longer works.
	/// </summary>

	public int characterLimit = 0;

	/// <summary>
	/// Field in player prefs used to automatically save the value.
	/// </summary>

	public string savedAs;

	/// <summary>
	/// Don't use this anymore. Attach UIKeyNavigation instead.
	/// </summary>

	[HideInInspector][SerializeField] GameObject selectOnTab;

	/// <summary>
	/// Color of the label when the input field has focus.
	/// </summary>

	public Color activeTextColor = Color.white;

	/// <summary>
	/// Color used by the caret symbol.
	/// </summary>

	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	/// <summary>
	/// Color used by the selection rectangle.
	/// </summary>

	public Color selectionColor = new Color(1f, 223f / 255f, 141f / 255f, 0.5f);

	/// <summary>
	/// Event delegates triggered when the input field submits its data.
	/// </summary>

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	/// <summary>
	/// Event delegates triggered when the input field's text changes for any reason.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

	/// <summary>
	/// Custom validation callback.
	/// </summary>

	public OnValidate onValidate;

	/// <summary>
	/// Input field's value.
	/// </summary>

	[SerializeField][HideInInspector] protected string mValue;

	[System.NonSerialized] protected string mDefaultText = "";
	[System.NonSerialized] protected Color mDefaultColor = Color.white;
	[System.NonSerialized] protected float mPosition = 0f;
	[System.NonSerialized] protected bool mDoInit = true;
	[System.NonSerialized] protected NGUIText.Alignment mAlignment = NGUIText.Alignment.Left;
	[System.NonSerialized] protected bool mLoadSavedValue = true;

	static protected int mDrawStart = 0;
	static protected string mLastIME = "";

#if MOBILE
	// Unity fails to compile if the touch screen keyboard is used on a non-mobile device
	static protected TouchScreenKeyboard mKeyboard;
	static bool mWaitForKeyboard = false;
#endif
	[System.NonSerialized] protected int mSelectionStart = 0;
	[System.NonSerialized] protected int mSelectionEnd = 0;
	[System.NonSerialized] protected UITexture mHighlight = null;
	[System.NonSerialized] protected UITexture mCaret = null;
	[System.NonSerialized] protected Texture2D mBlankTex = null;
	[System.NonSerialized] protected float mNextBlink = 0f;
	[System.NonSerialized] protected float mLastAlpha = 0f;
	[System.NonSerialized] protected string mCached = "";
	[System.NonSerialized] protected int mSelectMe = -1;
	[System.NonSerialized] protected int mSelectTime = -1;
	[System.NonSerialized] protected bool mStarted = false;

	/// <summary>
	/// Default text used by the input's label.
	/// </summary>

	public string defaultText
	{
		get
		{
			if (mDoInit) Init();
			return mDefaultText;
		}
		set
		{
			if (mDoInit) Init();
			mDefaultText = value;
			UpdateLabel();
		}
	}

	/// <summary>
	/// Text's default color when not selected.
	/// </summary>

	public Color defaultColor
	{
		get
		{
			if (mDoInit) Init();
			return mDefaultColor;
		}
		set
		{
			mDefaultColor = value;
			if (!isSelected) label.color = value;
		}
	}

	/// <summary>
	/// Should the input be hidden?
	/// </summary>

	public bool inputShouldBeHidden
	{
		get
		{
			return hideInput && label != null && !label.multiLine && inputType != InputType.Password;
		}
	}

	[System.Obsolete("Use UIInput.value instead")]
	public string text { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Input field's current text value.
	/// </summary>

	public string value
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return "";
#endif
			if (mDoInit) Init();
			return mValue;
		}
		set { Set(value); }
	}

	/// <summary>
	/// Set the input field's value. If setting the initial value, call Start() first.
	/// </summary>

	public void Set (string value, bool notify = true)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mDoInit) Init();
		if (value == this.value) return;
		mDrawStart = 0;

		// BB10's implementation has a bug in Unity
#if UNITY_4_3
		if (Application.platform == RuntimePlatform.BB10Player)
			value = value.Replace("\\b", "\b");
#elif UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		if (Application.platform == RuntimePlatform.BlackBerryPlayer)
			value = value.Replace("\\b", "\b");
#endif
		// Validate all input
		value = Validate(value);
#if MOBILE
		if (isSelected && mKeyboard != null && mCached != value)
		{
			mKeyboard.text = value;
			mCached = value;
		}
#endif
		if (mValue != value)
		{
			mValue = value;
			mLoadSavedValue = false;

			if (isSelected)
			{
				if (string.IsNullOrEmpty(value))
				{
					mSelectionStart = 0;
					mSelectionEnd = 0;
				}
				else
				{
					mSelectionStart = value.Length;
					mSelectionEnd = mSelectionStart;
				}
			}
			else if (mStarted) SaveToPlayerPrefs(value);

			UpdateLabel();
			if (notify) ExecuteOnChange();
		}
	}

	[System.Obsolete("Use UIInput.isSelected instead")]
	public bool selected { get { return isSelected; } set { isSelected = value; } }

	/// <summary>
	/// Whether the input is currently selected.
	/// </summary>

	public bool isSelected
	{
		get
		{
			return selection == this;
		}
		set
		{
			if (!value) { if (isSelected) UICamera.selectedObject = null; }
			else UICamera.selectedObject = gameObject;
		}
	}

	/// <summary>
	/// Current position of the cursor.
	/// </summary>

	public int cursorPosition
	{
		get
		{
#if MOBILE
			if (mKeyboard != null && !inputShouldBeHidden) return value.Length;
#endif
			return isSelected ? mSelectionEnd : value.Length;
		}
		set
		{
			if (isSelected)
			{
#if MOBILE
				if (mKeyboard != null && !inputShouldBeHidden) return;
#endif
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	/// <summary>
	/// Index of the character where selection begins.
	/// </summary>

	public int selectionStart
	{
		get
		{
#if MOBILE
			if (mKeyboard != null && !inputShouldBeHidden) return 0;
#endif
			return isSelected ? mSelectionStart : value.Length;
		}
		set
		{
			if (isSelected)
			{
#if MOBILE
				if (mKeyboard != null && !inputShouldBeHidden) return;
#endif
				mSelectionStart = value;
				UpdateLabel();
			}
		}
	}

	/// <summary>
	/// Index of the character where selection ends.
	/// </summary>

	public int selectionEnd
	{
		get
		{
#if MOBILE
			if (mKeyboard != null && !inputShouldBeHidden) return value.Length;
#endif
			return isSelected ? mSelectionEnd : value.Length;
		}
		set
		{
			if (isSelected)
			{
#if MOBILE
				if (mKeyboard != null && !inputShouldBeHidden) return;
#endif
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	/// <summary>
	/// Caret, in case it's needed.
	/// </summary>

	public UITexture caret { get { return mCaret; } }

	/// <summary>
	/// Validate the specified text, returning the validated version.
	/// </summary>

	public string Validate (string val)
	{
		if (string.IsNullOrEmpty(val)) return "";

		StringBuilder sb = new StringBuilder(val.Length);

		for (int i = 0; i < val.Length; ++i)
		{
			char c = val[i];
			if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
			else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);
			if (c != 0) sb.Append(c);
		}

		if (characterLimit > 0 && sb.Length > characterLimit)
			return sb.ToString(0, characterLimit);
		return sb.ToString();
	}

	/// <summary>
	/// Automatically set the value by loading it from player prefs if possible.
	/// </summary>

	public void Start ()
	{
		if (mStarted) return;
		if (selectOnTab != null)
		{
			UIKeyNavigation nav = GetComponent<UIKeyNavigation>();

			if (nav == null)
			{
				nav = gameObject.AddComponent<UIKeyNavigation>();
				nav.onDown = selectOnTab;
			}
			selectOnTab = null;
			NGUITools.SetDirty(this);
		}

		if (mLoadSavedValue && !string.IsNullOrEmpty(savedAs)) LoadValue();
		else value = mValue.Replace("\\n", "\n");
		mStarted = true;
	}

	/// <summary>
	/// Labels used for input shouldn't support rich text.
	/// </summary>

	protected void Init ()
	{
		if (mDoInit && label != null)
		{
			mDoInit = false;
			mDefaultText = label.text;
			mDefaultColor = label.color;
			mEllipsis = label.overflowEllipsis;

			if (label.alignment == NGUIText.Alignment.Justified)
			{
				label.alignment = NGUIText.Alignment.Left;
				Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}

			mAlignment = label.alignment;
			mPosition = label.cachedTransform.localPosition.x;
			UpdateLabel();
		}
	}

	/// <summary>
	/// Save the specified value to player prefs.
	/// </summary>

	protected void SaveToPlayerPrefs (string val)
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			if (string.IsNullOrEmpty(val)) PlayerPrefs.DeleteKey(savedAs);
			else PlayerPrefs.SetString(savedAs, val);
		}
	}

#if !MOBILE
	[System.NonSerialized] UIInputOnGUI mOnGUI;
#endif
	[System.NonSerialized] UICamera mCam;
	/// <summary>
	/// Selection event, sent by the EventSystem.
	/// </summary>

	protected virtual void OnSelect (bool isSelected)
	{
		if (isSelected)
		{
			if (label != null) label.supportEncoding = false;

#if !MOBILE
			if (mOnGUI == null)
				mOnGUI = gameObject.AddComponent<UIInputOnGUI>();
#endif
			OnSelectEvent();
		}
		else
		{
#if !MOBILE
			if (mOnGUI != null)
			{
				Destroy(mOnGUI);
				mOnGUI = null;
			}
#endif
			OnDeselectEvent();
		}
	}

	/// <summary>
	/// Notification of the input field gaining selection.
	/// </summary>

	protected void OnSelectEvent ()
	{
		mSelectTime = Time.frameCount;
		selection = this;
		if (mDoInit) Init();

		if (label != null)
		{
			mEllipsis = label.overflowEllipsis;
			label.overflowEllipsis = false;
		}

		// Unity has issues bringing up the keyboard properly if it's in "hideInput" mode and you happen
		// to select one input in the same Update as de-selecting another.
		if (label != null && NGUITools.GetActive(this)) mSelectMe = Time.frameCount;
	}

	[System.NonSerialized] bool mEllipsis = false;

	/// <summary>
	/// Notification of the input field losing selection.
	/// </summary>

	protected void OnDeselectEvent ()
	{
		if (mDoInit) Init();

		if (label != null) label.overflowEllipsis = mEllipsis;

		if (label != null && NGUITools.GetActive(this))
		{
			mValue = value;
#if MOBILE
			if (mKeyboard != null)
			{
				mWaitForKeyboard = false;
				mKeyboard.active = false;
				mKeyboard = null;
			}
#endif
			if (string.IsNullOrEmpty(mValue))
			{
				label.text = mDefaultText;
				label.color = mDefaultColor;
			}
			else label.text = mValue;

			Input.imeCompositionMode = IMECompositionMode.Auto;
			label.alignment = mAlignment;
		}

		selection = null;
		UpdateLabel();
	}

	/// <summary>
	/// Update the text based on input.
	/// </summary>
	
	protected virtual void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (!isSelected || mSelectTime == Time.frameCount) return;

		if (mDoInit) Init();
#if MOBILE
		// Wait for the keyboard to open. Apparently mKeyboard.active will return 'false' for a while in some cases.
		if (mWaitForKeyboard)
		{
			if (mKeyboard != null && !mKeyboard.active) return;
			mWaitForKeyboard = false;
		}
#endif
		// Unity has issues bringing up the keyboard properly if it's in "hideInput" mode and you happen
		// to select one input in the same Update as de-selecting another.
		if (mSelectMe != -1 && mSelectMe != Time.frameCount)
		{
			mSelectMe = -1;
			mSelectionEnd = string.IsNullOrEmpty(mValue) ? 0 : mValue.Length;
			mDrawStart = 0;
			mSelectionStart = selectAllTextOnFocus ? 0 : mSelectionEnd;
			label.color = activeTextColor;
#if MOBILE
			RuntimePlatform pf = Application.platform;
			if (pf == RuntimePlatform.IPhonePlayer
				|| pf == RuntimePlatform.Android
				|| pf == RuntimePlatform.WP8Player
 #if UNITY_4_3
				|| pf == RuntimePlatform.BB10Player
 #else
				|| pf == RuntimePlatform.BlackBerryPlayer
				|| pf == RuntimePlatform.MetroPlayerARM
				|| pf == RuntimePlatform.MetroPlayerX64
				|| pf == RuntimePlatform.MetroPlayerX86
 #endif
			)
			{
				string val;
				TouchScreenKeyboardType kt;

				if (inputShouldBeHidden)
				{
					TouchScreenKeyboard.hideInput = true;
					kt = (TouchScreenKeyboardType)((int)keyboardType);
					val = "|";
				}
				else if (inputType == InputType.Password)
				{
					TouchScreenKeyboard.hideInput = false;
					kt = (TouchScreenKeyboardType)((int)keyboardType);
					val = mValue;
					mSelectionStart = mSelectionEnd;
				}
				else
				{
					TouchScreenKeyboard.hideInput = false;
					kt = (TouchScreenKeyboardType)((int)keyboardType);
					val = mValue;
					mSelectionStart = mSelectionEnd;
				}

				mWaitForKeyboard = true;
				mKeyboard = (inputType == InputType.Password) ?
					TouchScreenKeyboard.Open(val, kt, false, false, true) :
					TouchScreenKeyboard.Open(val, kt, !inputShouldBeHidden && inputType == InputType.AutoCorrect,
						label.multiLine && !hideInput, false, false, defaultText);
#if UNITY_METRO
				mKeyboard.active = true;
#endif
			}
			else
#endif // MOBILE
			{
				Vector2 pos = (UICamera.current != null && UICamera.current.cachedCamera != null) ?
					UICamera.current.cachedCamera.WorldToScreenPoint(label.worldCorners[0]) :
					label.worldCorners[0];
				pos.y = Screen.height - pos.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = pos;
			}

			UpdateLabel();
			if (string.IsNullOrEmpty(Input.inputString)) return;
		}
#if MOBILE
		if (mKeyboard != null)
		{
			string text = (mKeyboard.done || !mKeyboard.active) ? mCached : mKeyboard.text;
 
			if (inputShouldBeHidden)
			{
				if (text != "|")
				{
					if (!string.IsNullOrEmpty(text))
					{
						Insert(text.Substring(1));
					}
					else if (!mKeyboard.done && mKeyboard.active)
					{
						DoBackspace();
					}
					mKeyboard.text = "|";
				}
			}
			else if (mCached != text)
			{
				mCached = text;
				if (!mKeyboard.done && mKeyboard.active) value = text;
			}

			if (mKeyboard.done || !mKeyboard.active)
			{
				if (!mKeyboard.wasCanceled) Submit();
				mKeyboard = null;
				isSelected = false;
				mCached = "";
			}
		}
		else
#endif // MOBILE
		{
			string ime = Input.compositionString;

			// There seems to be an inconsistency between IME on Windows, and IME on OSX.
			// On Windows, Input.inputString is always empty while IME is active. On the OSX it is not.
			if (string.IsNullOrEmpty(ime) && !string.IsNullOrEmpty(Input.inputString))
			{
				// Process input ignoring non-printable characters as they are not consistent.
				// Windows has them, OSX may not. They get handled inside OnGUI() instead.
				string s = Input.inputString;

				for (int i = 0; i < s.Length; ++i)
				{
					char ch = s[i];
					if (ch < ' ') continue;

					// OSX inserts these characters for arrow keys
					if (ch == '\uF700') continue;
					if (ch == '\uF701') continue;
					if (ch == '\uF702') continue;
					if (ch == '\uF703') continue;
					if (ch == '\uF728') continue;

					Insert(ch.ToString());
				}
			}

			// Append IME composition
			if (mLastIME != ime)
			{
				mSelectionEnd = string.IsNullOrEmpty(ime) ? mSelectionStart : mValue.Length + ime.Length;
				mLastIME = ime;
				UpdateLabel();
				ExecuteOnChange();
			}
		}

		// Blink the caret
		if (mCaret != null && mNextBlink < RealTime.time)
		{
			mNextBlink = RealTime.time + 0.5f;
			mCaret.enabled = !mCaret.enabled;
		}

		// If the label's final alpha changes, we need to update the drawn geometry,
		// or the highlight widgets (which have their geometry set manually) won't update.
		if (isSelected && mLastAlpha != label.finalAlpha)
			UpdateLabel();

		// Cache the camera
		if (mCam == null) mCam = UICamera.FindCameraForLayer(gameObject.layer);

		// Having this in OnGUI causes issues because Input.inputString gets updated *after* OnGUI, apparently...
		if (mCam != null)
		{
			bool newLine = false;

			if (label.multiLine)
			{
				bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				if (onReturnKey == OnReturnKey.Submit) newLine = ctrl;
				else newLine = !ctrl;
			}

			if (UICamera.GetKeyDown(mCam.submitKey0) || (mCam.submitKey0 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (newLine)
				{
					Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					UICamera.currentKey = mCam.submitKey0;
					Submit();
				}
			}

			if (UICamera.GetKeyDown(mCam.submitKey1) || (mCam.submitKey1 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (newLine)
				{
					Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					UICamera.currentKey = mCam.submitKey1;
					Submit();
				}
			}

			if (!mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
				OnKey(KeyCode.Tab);
		}
	}

	static int mIgnoreKey = 0;

	void OnKey (KeyCode key)
	{
		int frame = Time.frameCount;

		if (mIgnoreKey == frame) return;
		
		if (mCam != null && (key == mCam.cancelKey0 || key == mCam.cancelKey1))
		{
			mIgnoreKey = frame;
			isSelected = false;
		}
		else if (key == KeyCode.Tab)
		{
			mIgnoreKey = frame;
			isSelected = false;
			UIKeyNavigation nav = GetComponent<UIKeyNavigation>();
			if (nav != null) nav.OnKey(KeyCode.Tab);
		}
	}

	/// <summary>
	/// Perform a backspace operation.
	/// </summary>

	protected void DoBackspace ()
	{
		if (!string.IsNullOrEmpty(mValue))
		{
			if (mSelectionStart == mSelectionEnd)
			{
				if (mSelectionStart < 1) return;
				--mSelectionEnd;
			}
			Insert("");
		}
	}

#if !MOBILE
	/// <summary>
	/// Handle the specified event.
	/// </summary>

	public virtual bool ProcessEvent (Event ev)
	{
		if (label == null) return false;

		RuntimePlatform rp = Application.platform;

#if UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
		bool isMac = (
			rp == RuntimePlatform.OSXEditor ||
			rp == RuntimePlatform.OSXPlayer ||
			rp == RuntimePlatform.OSXWebPlayer);
#else
		bool isMac = (
			rp == RuntimePlatform.OSXEditor ||
			rp == RuntimePlatform.OSXPlayer);
#endif

		bool ctrl = isMac ?
			((ev.modifiers & EventModifiers.Command) != 0) :
			((ev.modifiers & EventModifiers.Control) != 0);

		// http://www.tasharen.com/forum/index.php?topic=10780.0
		if ((ev.modifiers & EventModifiers.Alt) != 0) ctrl = false;

		bool shift = ((ev.modifiers & EventModifiers.Shift) != 0);

		switch (ev.keyCode)
		{
			case KeyCode.Backspace:
			{
				ev.Use();
				DoBackspace();
				return true;
			}

			case KeyCode.Delete:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					if (mSelectionStart == mSelectionEnd)
					{
						if (mSelectionStart >= mValue.Length) return true;
						++mSelectionEnd;
					}
					Insert("");
				}
				return true;
			}

			case KeyCode.LeftArrow:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = Mathf.Max(mSelectionEnd - 1, 0);
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.RightArrow:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = Mathf.Min(mSelectionEnd + 1, mValue.Length);
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.PageUp:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = 0;
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.PageDown:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = mValue.Length;
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.Home:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					if (label.multiLine)
					{
						mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.Home);
					}
					else mSelectionEnd = 0;

					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.End:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					if (label.multiLine)
					{
						mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.End);
					}
					else mSelectionEnd = mValue.Length;

					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.UpArrow:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.UpArrow);
					if (mSelectionEnd != 0) mSelectionEnd += mDrawStart;
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			case KeyCode.DownArrow:
			{
				ev.Use();

				if (!string.IsNullOrEmpty(mValue))
				{
					mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.DownArrow);
					if (mSelectionEnd != label.processedText.Length) mSelectionEnd += mDrawStart;
					else mSelectionEnd = mValue.Length;
					if (!shift) mSelectionStart = mSelectionEnd;
					UpdateLabel();
				}
				return true;
			}

			// Select all
			case KeyCode.A:
			{
				if (ctrl)
				{
					ev.Use();
					mSelectionStart = 0;
					mSelectionEnd = mValue.Length;
					UpdateLabel();
				}
				return true;
			}

			// Copy
			case KeyCode.C:
			{
				if (ctrl)
				{
					ev.Use();
					NGUITools.clipboard = GetSelection();
				}
				return true;
			}

			// Paste
			case KeyCode.V:
			{
				if (ctrl)
				{
					ev.Use();
					Insert(NGUITools.clipboard);
				}
				return true;
			}

			// Cut
			case KeyCode.X:
			{
				if (ctrl)
				{
					ev.Use();
					NGUITools.clipboard = GetSelection();
					Insert("");
				}
				return true;
			}
		}
		return false;
	}
#endif

	/// <summary>
	/// Insert the specified text string into the current input value, respecting selection and validation.
	/// </summary>

	protected virtual void Insert (string text)
	{
		string left = GetLeftText();
		string right = GetRightText();
		int rl = right.Length;

		StringBuilder sb = new StringBuilder(left.Length + right.Length + text.Length);
		sb.Append(left);

		// Append the new text
		for (int i = 0, imax = text.Length; i < imax; ++i)
		{
			// If we have an input validator, validate the input first
			char c = text[i];

			if (c == '\b')
			{
				DoBackspace();
				continue;
			}

			// Can't go past the character limit
			if (characterLimit > 0 && sb.Length + rl >= characterLimit) break;

			if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
			else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);

			// Append the character if it hasn't been invalidated
			if (c != 0) sb.Append(c);
		}

		// Advance the selection
		mSelectionStart = sb.Length;
		mSelectionEnd = mSelectionStart;

		// Append the text that follows it, ensuring that it's also validated after the inserted value
		for (int i = 0, imax = right.Length; i < imax; ++i)
		{
			char c = right[i];
			if (onValidate != null) c = onValidate(sb.ToString(), sb.Length, c);
			else if (validation != Validation.None) c = Validate(sb.ToString(), sb.Length, c);
			if (c != 0) sb.Append(c);
		}

		mValue = sb.ToString();
		UpdateLabel();
		ExecuteOnChange();
	}

	/// <summary>
	/// Get the text to the left of the selection.
	/// </summary>

	protected string GetLeftText ()
	{
		int min = Mathf.Min(mSelectionStart, mSelectionEnd);
		return (string.IsNullOrEmpty(mValue) || min < 0) ? "" : mValue.Substring(0, min);
	}

	/// <summary>
	/// Get the text to the right of the selection.
	/// </summary>

	protected string GetRightText ()
	{
		int max = Mathf.Max(mSelectionStart, mSelectionEnd);
		return (string.IsNullOrEmpty(mValue) || max >= mValue.Length) ? "" : mValue.Substring(max);
	}

	/// <summary>
	/// Get currently selected text.
	/// </summary>

	protected string GetSelection ()
	{
		if (string.IsNullOrEmpty(mValue) || mSelectionStart == mSelectionEnd)
		{
			return "";
		}
		else
		{
			int min = Mathf.Min(mSelectionStart, mSelectionEnd);
			int max = Mathf.Max(mSelectionStart, mSelectionEnd);
			return mValue.Substring(min, max - min);
		}
	}

	/// <summary>
	/// Helper function that retrieves the index of the character under the mouse.
	/// </summary>

	protected int GetCharUnderMouse ()
	{
		Vector3[] corners = label.worldCorners;
		Ray ray = UICamera.currentRay;
		Plane p = new Plane(corners[0], corners[1], corners[2]);
		float dist;
		return p.Raycast(ray, out dist) ? mDrawStart + label.GetCharacterIndexAtPosition(ray.GetPoint(dist), false) : 0;
	}

	/// <summary>
	/// Move the caret on press.
	/// </summary>

	protected virtual void OnPress (bool isPressed)
	{
		if (isPressed && isSelected && label != null &&
			(UICamera.currentScheme == UICamera.ControlScheme.Mouse ||
			 UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
#if !UNITY_EDITOR && (UNITY_WP8 || UNITY_WP_8_1)
			if (mKeyboard != null) mKeyboard.active = true;
#endif
			selectionEnd = GetCharUnderMouse();
			if (!Input.GetKey(KeyCode.LeftShift) &&
				!Input.GetKey(KeyCode.RightShift)) selectionStart = mSelectionEnd;
		}
	}

	/// <summary>
	/// Drag selection.
	/// </summary>

	protected virtual void OnDrag (Vector2 delta)
	{
		if (label != null &&
			(UICamera.currentScheme == UICamera.ControlScheme.Mouse ||
			 UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			selectionEnd = GetCharUnderMouse();
		}
	}

	/// <summary>
	/// Ensure we've released the dynamically created resources.
	/// </summary>

	void OnDisable () { Cleanup(); }

	/// <summary>
	/// Cleanup.
	/// </summary>

	protected virtual void Cleanup ()
	{
		if (mHighlight) mHighlight.enabled = false;
		if (mCaret) mCaret.enabled = false;

		if (mBlankTex)
		{
			NGUITools.Destroy(mBlankTex);
			mBlankTex = null;
		}
	}

	/// <summary>
	/// Submit the input field's text.
	/// </summary>

	public void Submit ()
	{
		if (NGUITools.GetActive(this))
		{
			mValue = value;

			if (current == null)
			{
				current = this;
				EventDelegate.Execute(onSubmit);
				current = null;
			}
			SaveToPlayerPrefs(mValue);
		}
	}

	/// <summary>
	/// Update the visual text label.
	/// </summary>

	public void UpdateLabel ()
	{
		if (label != null)
		{
			if (mDoInit) Init();
			bool selected = isSelected;
			string fullText = value;
			bool isEmpty = string.IsNullOrEmpty(fullText) && string.IsNullOrEmpty(Input.compositionString);
			label.color = (isEmpty && !selected) ? mDefaultColor : activeTextColor;
			string processed;

			if (isEmpty)
			{
				processed = selected ? "" : mDefaultText;
				label.alignment = mAlignment;
			}
			else
			{
				if (inputType == InputType.Password)
				{
					processed = "";

					string asterisk = "*";

					if (label.bitmapFont != null && label.bitmapFont.bmFont != null &&
						label.bitmapFont.bmFont.GetGlyph('*') == null) asterisk = "x";

					for (int i = 0, imax = fullText.Length; i < imax; ++i) processed += asterisk;
				}
				else processed = fullText;

				// Start with text leading up to the selection
				int selPos = selected ? Mathf.Min(processed.Length, cursorPosition) : 0;
				string left = processed.Substring(0, selPos);

				// Append the composition string and the cursor character
				if (selected) left += Input.compositionString;

				// Append the text from the selection onwards
				processed = left + processed.Substring(selPos, processed.Length - selPos);

				// Clamped content needs to be adjusted further
				if (selected && label.overflowMethod == UILabel.Overflow.ClampContent && label.maxLineCount == 1)
				{
					// Determine what will actually fit into the given line
					int offset = label.CalculateOffsetToFit(processed);

					if (offset == 0)
					{
						mDrawStart = 0;
						label.alignment = mAlignment;
					}
					else if (selPos < mDrawStart)
					{
						mDrawStart = selPos;
						label.alignment = NGUIText.Alignment.Left;
					}
					else if (offset < mDrawStart)
					{
						mDrawStart = offset;
						label.alignment = NGUIText.Alignment.Left;
					}
					else
					{
						offset = label.CalculateOffsetToFit(processed.Substring(0, selPos));

						if (offset > mDrawStart)
						{
							mDrawStart = offset;
							label.alignment = NGUIText.Alignment.Right;
						}
					}

					// If necessary, trim the front
					if (mDrawStart != 0)
						processed = processed.Substring(mDrawStart, processed.Length - mDrawStart);
				}
				else
				{
					mDrawStart = 0;
					label.alignment = mAlignment;
				}
			}

			label.text = processed;
#if MOBILE
			if (selected && (mKeyboard == null || inputShouldBeHidden))
#else
			if (selected)
#endif
			{
				int start = mSelectionStart - mDrawStart;
				int end = mSelectionEnd - mDrawStart;

				// Blank texture used by selection and caret
				if (mBlankTex == null)
				{
					mBlankTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					for (int y = 0; y < 2; ++y)
						for (int x = 0; x < 2; ++x)
							mBlankTex.SetPixel(x, y, Color.white);
					mBlankTex.Apply();
				}

				// Create the selection highlight
				if (start != end)
				{
					if (mHighlight == null)
					{
						mHighlight = NGUITools.AddWidget<UITexture>(label.cachedGameObject);
						mHighlight.name = "Input Highlight";
						mHighlight.mainTexture = mBlankTex;
						mHighlight.fillGeometry = false;
						mHighlight.pivot = label.pivot;
						mHighlight.SetAnchor(label.cachedTransform);
					}
					else
					{
						mHighlight.pivot = label.pivot;
						mHighlight.mainTexture = mBlankTex;
						mHighlight.MarkAsChanged();
						mHighlight.enabled = true;
					}
				}

				// Create the carter
				if (mCaret == null)
				{
					mCaret = NGUITools.AddWidget<UITexture>(label.cachedGameObject);
					mCaret.name = "Input Caret";
					mCaret.mainTexture = mBlankTex;
					mCaret.fillGeometry = false;
					mCaret.pivot = label.pivot;
					mCaret.SetAnchor(label.cachedTransform);
				}
				else
				{
					mCaret.pivot = label.pivot;
					mCaret.mainTexture = mBlankTex;
					mCaret.MarkAsChanged();
					mCaret.enabled = true;
				}

				if (start != end)
				{
					label.PrintOverlay(start, end, mCaret.geometry, mHighlight.geometry, caretColor, selectionColor);
					mHighlight.enabled = mHighlight.geometry.hasVertices;
				}
				else
				{
					label.PrintOverlay(start, end, mCaret.geometry, null, caretColor, selectionColor);
					if (mHighlight != null) mHighlight.enabled = false;
				}

				// Reset the blinking time
				mNextBlink = RealTime.time + 0.5f;
				mLastAlpha = label.finalAlpha;
			}
			else Cleanup();
		}
	}

	/// <summary>
	/// Validate the specified input.
	/// </summary>

	protected char Validate (string text, int pos, char ch)
	{
		// Validation is disabled
		if (validation == Validation.None || !enabled) return ch;

		if (validation == Validation.Integer)
		{
			// Integer number validation
			if (ch >= '0' && ch <= '9') return ch;
			if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
		}
		else if (validation == Validation.Float)
		{
			// Floating-point number
			if (ch >= '0' && ch <= '9') return ch;
			if (ch == '-' && pos == 0 && !text.Contains("-")) return ch;
			if (ch == '.' && !text.Contains(".")) return ch;
		}
		else if (validation == Validation.Alphanumeric)
		{
			// All alphanumeric characters
			if (ch >= 'A' && ch <= 'Z') return ch;
			if (ch >= 'a' && ch <= 'z') return ch;
			if (ch >= '0' && ch <= '9') return ch;
		}
		else if (validation == Validation.Username)
		{
			// Lowercase and numbers
			if (ch >= 'A' && ch <= 'Z') return (char)(ch - 'A' + 'a');
			if (ch >= 'a' && ch <= 'z') return ch;
			if (ch >= '0' && ch <= '9') return ch;
		}
		else if (validation == Validation.Filename)
		{
			if (ch == ':') return (char)0;
			if (ch == '/') return (char)0;
			if (ch == '\\') return (char)0;
			if (ch == '<') return (char)0;
			if (ch == '>') return (char)0;
			if (ch == '|') return (char)0;
			if (ch == '^') return (char)0;
			if (ch == '*') return (char)0;
			if (ch == ';') return (char)0;
			if (ch == '"') return (char)0;
			if (ch == '`') return (char)0;
			if (ch == '\t') return (char)0;
			if (ch == '\n') return (char)0;
			return ch;
		}
		else if (validation == Validation.Name)
		{
			char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
			char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';

			if (ch >= 'a' && ch <= 'z')
			{
				// Space followed by a letter -- make sure it's capitalized
				if (lastChar == ' ') return (char)(ch - 'a' + 'A');
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				// Uppercase letters are only allowed after spaces (and apostrophes)
				if (lastChar != ' ' && lastChar != '\'') return (char)(ch - 'A' + 'a');
				return ch;
			}
			else if (ch == '\'')
			{
				// Don't allow more than one apostrophe
				if (lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains("'")) return ch;
			}
			else if (ch == ' ')
			{
				// Don't allow more than one space in a row
				if (lastChar != ' ' && lastChar != '\'' && nextChar != ' ' && nextChar != '\'') return ch;
			}
		}
		return (char)0;
	}

	/// <summary>
	/// Execute the OnChange callback.
	/// </summary>

	protected void ExecuteOnChange ()
	{
		if (current == null && EventDelegate.IsValid(onChange))
		{
			current = this;
			EventDelegate.Execute(onChange);
			current = null;
		}
	}

	/// <summary>
	/// Convenience function to be used as a callback that will clear the input field's focus.
	/// </summary>

	public void RemoveFocus () { isSelected = false; }

	/// <summary>
	/// Convenience function that can be used as a callback for On Change notification.
	/// </summary>

	public void SaveValue () { SaveToPlayerPrefs(mValue); }

	/// <summary>
	/// Convenience function that can forcefully reset the input field's value to what was saved earlier.
	/// </summary>

	public void LoadValue ()
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			string val = mValue.Replace("\\n", "\n");
			mValue = "";
			value = PlayerPrefs.HasKey(savedAs) ? PlayerPrefs.GetString(savedAs) : val;
		}
	}
}
