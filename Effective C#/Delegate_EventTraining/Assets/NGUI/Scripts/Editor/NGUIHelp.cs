//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// This script adds the NGUI menu options to the Unity Editor.
/// </summary>

static public class NGUIHelp
{
	/// <summary>
	/// Get the URL pointing to the documentation for the specified component.
	/// </summary>

	static public string GetHelpURL (Type type)
	{
		if (type == typeof(UITexture))		return "http://www.tasharen.com/forum/index.php?topic=6703";
		if (type == typeof(UISprite))		return "http://www.tasharen.com/forum/index.php?topic=6704";
		if (type == typeof(UIPanel))		return "http://www.tasharen.com/forum/index.php?topic=6705";
		if (type == typeof(UILabel))		return "http://www.tasharen.com/forum/index.php?topic=6706";
		if (type == typeof(UIButton))		return "http://www.tasharen.com/forum/index.php?topic=6708";
		if (type == typeof(UIToggle))		return "http://www.tasharen.com/forum/index.php?topic=6709";
		if (type == typeof(UIRoot))			return "http://www.tasharen.com/forum/index.php?topic=6710";
		if (type == typeof(UICamera))		return "http://www.tasharen.com/forum/index.php?topic=6711";
		if (type == typeof(UIAnchor))		return "http://www.tasharen.com/forum/index.php?topic=6712";
		if (type == typeof(UIStretch))		return "http://www.tasharen.com/forum/index.php?topic=6713";
		if (type == typeof(UISlider))		return "http://www.tasharen.com/forum/index.php?topic=6715";
		if (type == typeof(UI2DSprite))		return "http://www.tasharen.com/forum/index.php?topic=6729";
		if (type == typeof(UIScrollBar))	return "http://www.tasharen.com/forum/index.php?topic=6733";
		if (type == typeof(UIProgressBar))	return "http://www.tasharen.com/forum/index.php?topic=6738";
		if (type == typeof(UIPopupList))	return "http://www.tasharen.com/forum/index.php?topic=6751";
		if (type == typeof(UIInput))		return "http://www.tasharen.com/forum/index.php?topic=6752";
		if (type == typeof(UIKeyBinding))	return "http://www.tasharen.com/forum/index.php?topic=6753";
		if (type == typeof(UIGrid))			return "http://www.tasharen.com/forum/index.php?topic=6756";
		if (type == typeof(UITable))		return "http://www.tasharen.com/forum/index.php?topic=6758";
		if (type == typeof(UIKeyNavigation)) return "http://www.tasharen.com/forum/index.php?topic=8747";

		if (type == typeof(PropertyBinding) || type == typeof(PropertyReference))
			return "http://www.tasharen.com/forum/index.php?topic=8808";
		
		if (type == typeof(ActiveAnimation) || type == typeof(UIPlayAnimation))
			return "http://www.tasharen.com/forum/index.php?topic=6762";

		if (type == typeof(UIScrollView) || type == typeof(UIDragScrollView))
			return "http://www.tasharen.com/forum/index.php?topic=6763";

		if (type == typeof(UIWidget) || type.IsSubclassOf(typeof(UIWidget)))
			return "http://www.tasharen.com/forum/index.php?topic=6702";

		if (type == typeof(UIPlayTween) || type.IsSubclassOf(typeof(UITweener)))
			return "http://www.tasharen.com/forum/index.php?topic=6760";

		if (type == typeof(UILocalize) || type == typeof(Localization))
			return "http://www.tasharen.com/forum/index.php?topic=8092.0";

		return null;
	}

	/// <summary>
	/// Show generic help.
	/// </summary>

	static public void Show ()
	{
		Application.OpenURL("http://www.tasharen.com/forum/index.php?topic=6754");
	}

	/// <summary>
	/// Show help for the specific topic.
	/// </summary>

	static public void Show (Type type)
	{
		string url = GetHelpURL(type);
		if (url == null) url = "http://www.tasharen.com/ngui/doc.php?topic=" + type;
		Application.OpenURL(url);
	}

	/// <summary>
	/// Show help for the specific topic.
	/// </summary>

	static public void Show (object obj)
	{
		if (obj is GameObject)
		{
			GameObject go = obj as GameObject;
			UIWidget widget = go.GetComponent<UIWidget>();

			if (widget != null)
			{
				Show(widget.GetType());
				return;
			}
		}
		Show(obj.GetType());
	}
}
