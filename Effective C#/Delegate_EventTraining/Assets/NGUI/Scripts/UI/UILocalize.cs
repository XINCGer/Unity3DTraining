//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple script that lets you localize a UIWidget.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/UI/Localize")]
public class UILocalize : MonoBehaviour
{
	/// <summary>
	/// Localization key.
	/// </summary>

	public string key;

	/// <summary>
	/// Manually change the value of whatever the localization component is attached to.
	/// </summary>

	public string value
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				UIWidget w = GetComponent<UIWidget>();
				UILabel lbl = w as UILabel;
				UISprite sp = w as UISprite;

				if (lbl != null)
				{
					// If this is a label used by input, we should localize its default value instead
					UIInput input = NGUITools.FindInParents<UIInput>(lbl.gameObject);
					if (input != null && input.label == lbl) input.defaultText = value;
					else lbl.text = value;
#if UNITY_EDITOR
					if (!Application.isPlaying) NGUITools.SetDirty(lbl);
#endif
				}
				else if (sp != null)
				{
					UIButton btn = NGUITools.FindInParents<UIButton>(sp.gameObject);
					if (btn != null && btn.tweenTarget == sp.gameObject)
						btn.normalSprite = value;

					sp.spriteName = value;
					sp.MakePixelPerfect();
#if UNITY_EDITOR
					if (!Application.isPlaying) NGUITools.SetDirty(sp);
#endif
				}
			}
		}
	}

	bool mStarted = false;

	/// <summary>
	/// Localize the widget on enable, but only if it has been started already.
	/// </summary>

	void OnEnable ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mStarted) OnLocalize();
	}

	/// <summary>
	/// Localize the widget on start.
	/// </summary>

	void Start ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		mStarted = true;
		OnLocalize();
	}

	/// <summary>
	/// This function is called by the Localization manager via a broadcast SendMessage.
	/// </summary>

	void OnLocalize ()
	{
		// If no localization key has been specified, use the label's text as the key
		if (string.IsNullOrEmpty(key))
		{
			UILabel lbl = GetComponent<UILabel>();
			if (lbl != null) key = lbl.text;
		}

		// If we still don't have a key, leave the value as blank
		if (!string.IsNullOrEmpty(key)) value = Localization.Get(key);
	}
}
