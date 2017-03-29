//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIButton))]
#else
[CustomEditor(typeof(UIButton), true)]
#endif
public class UIButtonEditor : UIButtonColorEditor
{
	enum Highlight
	{
		DoNothing,
		Press,
	}

	protected override void DrawProperties ()
	{
		SerializedProperty sp = serializedObject.FindProperty("dragHighlight");
		Highlight ht = sp.boolValue ? Highlight.Press : Highlight.DoNothing;
		GUILayout.BeginHorizontal();
		bool highlight = (Highlight)EditorGUILayout.EnumPopup("Drag Over", ht) == Highlight.Press;
		NGUIEditorTools.DrawPadding();
		GUILayout.EndHorizontal();
		if (sp.boolValue != highlight) sp.boolValue = highlight;

		DrawTransition();
		DrawColors();

		UIButton btn = target as UIButton;

		if (btn.tweenTarget != null)
		{
			UISprite sprite = btn.tweenTarget.GetComponent<UISprite>();
			UI2DSprite s2d = btn.tweenTarget.GetComponent<UI2DSprite>();

			if (sprite != null)
			{
				if (NGUIEditorTools.DrawHeader("Sprites", "Sprites", false, true))
				{
					NGUIEditorTools.BeginContents(true);
					EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
					{
						SerializedObject obj = new SerializedObject(sprite);
						obj.Update();
						SerializedProperty atlas = obj.FindProperty("mAtlas");
						NGUIEditorTools.DrawSpriteField("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
						obj.ApplyModifiedProperties();

						NGUIEditorTools.DrawSpriteField("Hover", serializedObject, atlas, serializedObject.FindProperty("hoverSprite"), true);
						NGUIEditorTools.DrawSpriteField("Pressed", serializedObject, atlas, serializedObject.FindProperty("pressedSprite"), true);
						NGUIEditorTools.DrawSpriteField("Disabled", serializedObject, atlas, serializedObject.FindProperty("disabledSprite"), true);
					}
					EditorGUI.EndDisabledGroup();

					NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "pixelSnap");
					NGUIEditorTools.EndContents();
				}
			}
			else if (s2d != null)
			{
				if (NGUIEditorTools.DrawHeader("Sprites", "Sprites", false, true))
				{
					NGUIEditorTools.BeginContents(true);
					EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
					{
						SerializedObject obj = new SerializedObject(s2d);
						obj.Update();
						NGUIEditorTools.DrawProperty("Normal", obj, "mSprite");
						obj.ApplyModifiedProperties();

						NGUIEditorTools.DrawProperty("Hover", serializedObject, "hoverSprite2D");
						NGUIEditorTools.DrawProperty("Pressed", serializedObject, "pressedSprite2D");
						NGUIEditorTools.DrawProperty("Disabled", serializedObject, "disabledSprite2D");
					}
					EditorGUI.EndDisabledGroup();

					NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "pixelSnap");
					NGUIEditorTools.EndContents();
				}
			}
		}

		UIButton button = target as UIButton;
		NGUIEditorTools.DrawEvents("On Click", button, button.onClick, false);
	}
}
