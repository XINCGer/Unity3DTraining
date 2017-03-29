//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIPopupLists.
/// </summary>

[CustomEditor(typeof(UIPopupList))]
public class UIPopupListInspector : UIWidgetContainerEditor
{
	enum FontType
	{
		Bitmap,
		Dynamic,
	}

	UIPopupList mList;
	FontType mType;

	void OnEnable ()
	{
		SerializedProperty bit = serializedObject.FindProperty("bitmapFont");
		mType = (bit.objectReferenceValue != null) ? FontType.Bitmap : FontType.Dynamic;
		mList = target as UIPopupList;

		if (mList.ambigiousFont == null)
		{
			mList.ambigiousFont = NGUISettings.ambigiousFont;
			mList.fontSize = NGUISettings.fontSize;
			mList.fontStyle = NGUISettings.fontStyle;
			EditorUtility.SetDirty(mList);
		}

		if (mList.atlas == null && mList.background2DSprite == null && mList.highlight2DSprite == null)
		{
			mList.atlas = NGUISettings.atlas;
			mList.backgroundSprite = NGUISettings.selectedSprite;
			mList.highlightSprite = NGUISettings.selectedSprite;
			EditorUtility.SetDirty(mList);
		}
	}

	void RegisterUndo ()
	{
		NGUIEditorTools.RegisterUndo("Popup List Change", mList);
	}

	void OnSelectAtlas (Object obj)
	{
		RegisterUndo();
		mList.atlas = obj as UIAtlas;
		NGUISettings.atlas = mList.atlas;
	}

	void OnBackground (string spriteName)
	{
		RegisterUndo();
		mList.backgroundSprite = spriteName;
		Repaint();
	}

	void OnHighlight (string spriteName)
	{
		RegisterUndo();
		mList.highlightSprite = spriteName;
		Repaint();
	}

	void OnBitmapFont (Object obj)
	{
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("bitmapFont");
		sp.objectReferenceValue = obj;
		serializedObject.ApplyModifiedProperties();
		NGUISettings.ambigiousFont = obj;
	}

	void OnDynamicFont (Object obj)
	{
		serializedObject.Update();
		SerializedProperty sp = serializedObject.FindProperty("trueTypeFont");
		sp.objectReferenceValue = obj;
		serializedObject.ApplyModifiedProperties();
		NGUISettings.ambigiousFont = obj;
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		NGUIEditorTools.SetLabelWidth(80f);

		GUILayout.BeginHorizontal();
		GUILayout.Space(6f);
		GUILayout.Label("Options");
		GUILayout.EndHorizontal();

		string text = "";
		foreach (string s in mList.items) text += s + "\n";

		GUILayout.Space(-14f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(84f);
		string modified = EditorGUILayout.TextArea(text, GUILayout.Height(100f));
		GUILayout.EndHorizontal();

		if (modified != text)
		{
			RegisterUndo();
			string[] split = modified.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
			mList.items.Clear();
			foreach (string s in split) mList.items.Add(s);

			if (string.IsNullOrEmpty(mList.value) || !mList.items.Contains(mList.value))
			{
				mList.value = mList.items.Count > 0 ? mList.items[0] : "";
			}
		}

		NGUIEditorTools.DrawProperty("Position", serializedObject, "position");
		NGUIEditorTools.DrawProperty("Alignment", serializedObject, "alignment");
		NGUIEditorTools.DrawProperty("Open on", serializedObject, "openOn");
		NGUIEditorTools.DrawProperty("On Top", serializedObject, "separatePanel");
		NGUIEditorTools.DrawProperty("Localized", serializedObject, "isLocalized");

		GUI.changed = false;
		var sp = NGUIEditorTools.DrawProperty("Keep Value", serializedObject, "keepValue");

		if (GUI.changed)
		{
			serializedObject.FindProperty("mSelectedItem").stringValue = (sp.boolValue && mList.items.Count > 0) ? mList.items[0] : "";
		}

		EditorGUI.BeginDisabledGroup(!sp.boolValue);
		{
			GUI.changed = false;
			string sel = NGUIEditorTools.DrawList("Initial Value", mList.items.ToArray(), mList.value);
			if (GUI.changed) serializedObject.FindProperty("mSelectedItem").stringValue = sel;
		}
		EditorGUI.EndDisabledGroup();

		DrawAtlas();
		DrawFont();

		NGUIEditorTools.DrawEvents("On Value Change", mList, mList.onChange);

		serializedObject.ApplyModifiedProperties();
	}

	void DrawAtlas()
	{
		if (NGUIEditorTools.DrawHeader("Atlas"))
		{
			NGUIEditorTools.BeginContents();

			SerializedProperty atlasSp = null;

			GUILayout.BeginHorizontal();
			{
				if (NGUIEditorTools.DrawPrefixButton("Atlas"))
					ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
				atlasSp = NGUIEditorTools.DrawProperty("", serializedObject, "atlas");
			}
			GUILayout.EndHorizontal();

			if (atlasSp != null && atlasSp.objectReferenceValue != null)
			{
				NGUIEditorTools.DrawPaddedSpriteField("Background", mList.atlas, mList.backgroundSprite, OnBackground);
				NGUIEditorTools.DrawPaddedSpriteField("Highlight", mList.atlas, mList.highlightSprite, OnHighlight);
			}
			else
			{
				serializedObject.DrawProperty("background2DSprite", "Background");
				serializedObject.DrawProperty("highlight2DSprite", "Highlight");
			}

			EditorGUILayout.Space();

			NGUIEditorTools.DrawProperty("Background", serializedObject, "backgroundColor");
			NGUIEditorTools.DrawProperty("Highlight", serializedObject, "highlightColor");
			NGUIEditorTools.DrawProperty("Overlap", serializedObject, "overlap", GUILayout.Width(110f));
			NGUIEditorTools.DrawProperty("Animated", serializedObject, "isAnimated");
			NGUIEditorTools.EndContents();
		}
	}

	void DrawFont ()
	{
		if (NGUIEditorTools.DrawHeader("Font"))
		{
			NGUIEditorTools.BeginContents();

			SerializedProperty ttf = null;

			GUILayout.BeginHorizontal();
			{
				if (NGUIEditorTools.DrawPrefixButton("Font"))
				{
					if (mType == FontType.Bitmap)
					{
						ComponentSelector.Show<UIFont>(OnBitmapFont);
					}
					else
					{
						ComponentSelector.Show<Font>(OnDynamicFont, new string[] { ".ttf", ".otf"});
					}
				}

#if DYNAMIC_FONT
				GUI.changed = false;
				mType = (FontType)EditorGUILayout.EnumPopup(mType, GUILayout.Width(62f));

				if (GUI.changed)
				{
					GUI.changed = false;

					if (mType == FontType.Bitmap)
					{
						serializedObject.FindProperty("trueTypeFont").objectReferenceValue = null;
					}
					else
					{
						serializedObject.FindProperty("bitmapFont").objectReferenceValue = null;
					}
				}
#else
				mType = FontType.Bitmap;
#endif

				if (mType == FontType.Bitmap)
				{
					NGUIEditorTools.DrawProperty("", serializedObject, "bitmapFont", GUILayout.MinWidth(40f));
				}
				else
				{
					ttf = NGUIEditorTools.DrawProperty("", serializedObject, "trueTypeFont", GUILayout.MinWidth(40f));
				}
			}
			GUILayout.EndHorizontal();

			if (ttf != null && ttf.objectReferenceValue != null)
			{
				GUILayout.BeginHorizontal();
				{
					EditorGUI.BeginDisabledGroup(ttf.hasMultipleDifferentValues);
					NGUIEditorTools.DrawProperty("Font Size", serializedObject, "fontSize", GUILayout.Width(142f));
					NGUIEditorTools.DrawProperty("", serializedObject, "fontStyle", GUILayout.MinWidth(40f));
					NGUIEditorTools.DrawPadding();
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();
			}
			else NGUIEditorTools.DrawProperty("Font Size", serializedObject, "fontSize", GUILayout.Width(142f));

			NGUIEditorTools.DrawProperty("Text Color", serializedObject, "textColor");

			GUILayout.BeginHorizontal();
			NGUIEditorTools.SetLabelWidth(66f);
			EditorGUILayout.PrefixLabel("Padding");
			NGUIEditorTools.SetLabelWidth(14f);
			NGUIEditorTools.DrawProperty("X", serializedObject, "padding.x", GUILayout.MinWidth(30f));
			NGUIEditorTools.DrawProperty("Y", serializedObject, "padding.y", GUILayout.MinWidth(30f));
			NGUIEditorTools.DrawPadding();
			NGUIEditorTools.SetLabelWidth(80f);
			GUILayout.EndHorizontal();

			NGUIEditorTools.EndContents();
		}
	}
}
