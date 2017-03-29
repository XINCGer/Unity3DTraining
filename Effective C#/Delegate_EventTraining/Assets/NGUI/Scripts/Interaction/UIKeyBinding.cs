//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class makes it possible to activate or select something by pressing a key (such as space bar for example).
/// </summary>

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	static List<UIKeyBinding> mList = new List<UIKeyBinding>();

	public enum Action
	{
		PressAndClick,
		Select,
		All,
	}

	public enum Modifier
	{
		Any,
		Shift,
		Control,
		Alt,
		None,
	}

	/// <summary>
	/// Key that will trigger the binding.
	/// </summary>

	public KeyCode keyCode = KeyCode.None;

	/// <summary>
	/// Modifier key that must be active in order for the binding to trigger.
	/// </summary>

	public Modifier modifier = Modifier.Any;

	/// <summary>
	/// Action to take with the specified key.
	/// </summary>

	public Action action = Action.PressAndClick;

	[System.NonSerialized] bool mIgnoreUp = false;
	[System.NonSerialized] bool mIsInput = false;
	[System.NonSerialized] bool mPress = false;

	/// <summary>
	/// Key binding's descriptive caption.
	/// </summary>

	public string captionText
	{
		get
		{
			string s = NGUITools.KeyToCaption(keyCode);
			if (modifier == Modifier.Alt) return "Alt+" + s;
			if (modifier == Modifier.Control) return "Control+" + s;
			if (modifier == Modifier.Shift) return "Shift+" + s;
			return s;
		}
	}

	/// <summary>
	/// Check to see if the specified key happens to be bound to some element.
	/// </summary>

	static public bool IsBound (KeyCode key)
	{
		for (int i = 0, imax = mList.Count; i < imax; ++i)
		{
			UIKeyBinding kb = mList[i];
			if (kb != null && kb.keyCode == key) return true;
		}
		return false;
	}

	protected virtual void OnEnable () { mList.Add(this); }
	protected virtual void OnDisable () { mList.Remove(this); }

	/// <summary>
	/// If we're bound to an input field, subscribe to its Submit notification.
	/// </summary>

	protected virtual void Start ()
	{
		UIInput input = GetComponent<UIInput>();
		mIsInput = (input != null);
		if (input != null) EventDelegate.Add(input.onSubmit, OnSubmit);
	}

	/// <summary>
	/// Ignore the KeyUp message if the input field "ate" it.
	/// </summary>

	protected virtual void OnSubmit () { if (UICamera.currentKey == keyCode && IsModifierActive()) mIgnoreUp = true; }

	/// <summary>
	/// Convenience function that checks whether the required modifier key is active.
	/// </summary>

	protected virtual bool IsModifierActive () { return IsModifierActive(modifier); }

	/// <summary>
	/// Convenience function that checks whether the required modifier key is active.
	/// </summary>

	static public bool IsModifierActive (Modifier modifier)
	{
		if (modifier == Modifier.Any) return true;

		if (modifier == Modifier.Alt)
		{
			if (UICamera.GetKey(KeyCode.LeftAlt) ||
				UICamera.GetKey(KeyCode.RightAlt)) return true;
		}
		else if (modifier == Modifier.Control)
		{
			if (UICamera.GetKey(KeyCode.LeftControl) ||
				UICamera.GetKey(KeyCode.RightControl)) return true;
		}
		else if (modifier == Modifier.Shift)
		{
			if (UICamera.GetKey(KeyCode.LeftShift) ||
				UICamera.GetKey(KeyCode.RightShift)) return true;
		}
		else if (modifier == Modifier.None)
			return
				!UICamera.GetKey(KeyCode.LeftAlt) &&
				!UICamera.GetKey(KeyCode.RightAlt) &&
				!UICamera.GetKey(KeyCode.LeftControl) &&
				!UICamera.GetKey(KeyCode.RightControl) &&
				!UICamera.GetKey(KeyCode.LeftShift) &&
				!UICamera.GetKey(KeyCode.RightShift);
		return false;
	}

	/// <summary>
	/// Process the key binding.
	/// </summary>

	protected virtual void Update ()
	{
		if (UICamera.inputHasFocus) return;
		if (keyCode == KeyCode.None || !IsModifierActive()) return;
#if WINDWARD && UNITY_ANDROID
		// NVIDIA Shield controller has an odd bug where it can open the on-screen keyboard via a KeyCode.Return binding,
		// and then it can never be closed. I am disabling it here until I can track down the cause.
		if (keyCode == KeyCode.Return && PlayerPrefs.GetInt("Start Chat") == 0) return;
#endif

#if UNITY_FLASH
		bool keyDown = Input.GetKeyDown(keyCode);
		bool keyUp = Input.GetKeyUp(keyCode);
#else
		bool keyDown = UICamera.GetKeyDown(keyCode);
		bool keyUp = UICamera.GetKeyUp(keyCode);
#endif

		if (keyDown) mPress = true;

		if (action == Action.PressAndClick || action == Action.All)
		{
			if (keyDown)
			{
				UICamera.currentKey = keyCode;
				OnBindingPress(true);
			}

			if (mPress && keyUp)
			{
				UICamera.currentKey = keyCode;
				OnBindingPress(false);
				OnBindingClick();
			}
		}

		if (action == Action.Select || action == Action.All)
		{
			if (keyUp)
			{
				if (mIsInput)
				{
					if (!mIgnoreUp && !UICamera.inputHasFocus)
					{
						if (mPress) UICamera.selectedObject = gameObject;
					}
					mIgnoreUp = false;
				}
				else if (mPress)
				{
					UICamera.hoveredObject = gameObject;
				}
			}
		}

		if (keyUp) mPress = false;
	}

	protected virtual void OnBindingPress (bool pressed) { UICamera.Notify(gameObject, "OnPress", pressed); }
	protected virtual void OnBindingClick () { UICamera.Notify(gameObject, "OnClick", null); }

	/// <summary>
	/// Convert the key binding to its text format.
	/// </summary>

	public override string ToString () { return GetString(keyCode, modifier); }

	/// <summary>
	/// Convert the key binding to its text format.
	/// </summary>

	static public string GetString (KeyCode keyCode, Modifier modifier)
	{
		return (modifier != Modifier.None) ? modifier + "+" + keyCode : keyCode.ToString();
	}

	/// <summary>
	/// Given the ToString() text, parse it for key and modifier information.
	/// </summary>

	static public bool GetKeyCode (string text, out KeyCode key, out Modifier modifier)
	{
		key = KeyCode.None;
		modifier = Modifier.None;
		if (string.IsNullOrEmpty(text)) return false;

		if (text.Contains("+"))
		{
			string[] parts = text.Split('+');

			try
			{
				modifier = (Modifier)System.Enum.Parse(typeof(Modifier), parts[0]);
				key = (KeyCode)System.Enum.Parse(typeof(KeyCode), parts[1]);
			}
			catch (System.Exception) { return false; }
		}
		else
		{
			modifier = Modifier.None;
			try { key = (KeyCode)System.Enum.Parse(typeof(KeyCode), text); }
			catch (System.Exception) { return false; }
		}
		return true;
	}

	/// <summary>
	/// Get the currently active key modifier, if any.
	/// </summary>

	static public Modifier GetActiveModifier ()
	{
		UIKeyBinding.Modifier mod = UIKeyBinding.Modifier.None;

		if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt)) mod = UIKeyBinding.Modifier.Alt;
		else if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift)) mod = UIKeyBinding.Modifier.Shift;
		else if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl)) mod = UIKeyBinding.Modifier.Control;

		return mod;
	}
}
