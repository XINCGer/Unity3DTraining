//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UISprites.
/// </summary>

[CustomEditor(typeof(UIImageButton))]
public class UIImageButtonInspector : Editor
{
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.HelpBox("Image Button component's functionality is now a part of UIButton. You no longer need UIImageButton.", MessageType.Warning, true);

		if (GUILayout.Button("Auto-Upgrade"))
		{
			UIImageButton img = target as UIImageButton;

			UIButton btn = img.GetComponent<UIButton>();

			if (btn == null)
			{
				btn = img.gameObject.AddComponent<UIButton>();
				if (img.target != null) btn.tweenTarget = img.target.gameObject;
				else btn.tweenTarget = img.gameObject;

				UISprite sp = btn.tweenTarget.GetComponent<UISprite>();
				if (sp != null) sp.spriteName = img.normalSprite;
			}

			btn.hoverSprite = img.hoverSprite;
			btn.pressedSprite = img.pressedSprite;
			btn.disabledSprite = img.disabledSprite;
			btn.pixelSnap = img.pixelSnap;

			NGUITools.DestroyImmediate(img);
		}
	}
}
