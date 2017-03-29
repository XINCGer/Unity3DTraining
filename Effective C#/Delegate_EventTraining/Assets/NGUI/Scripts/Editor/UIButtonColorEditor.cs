//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

#if UNITY_3_5
[CustomEditor(typeof(UIButtonColor))]
#else
[CustomEditor(typeof(UIButtonColor), true)]
#endif
public class UIButtonColorEditor : UIWidgetContainerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(86f);

		serializedObject.Update();
		NGUIEditorTools.DrawProperty("Tween Target", serializedObject, "tweenTarget");
		DrawProperties();
		serializedObject.ApplyModifiedProperties();

		if (target.GetType() == typeof(UIButtonColor))
		{
			GUILayout.Space(3f);

			if (GUILayout.Button("Upgrade to a Button"))
			{
				NGUIEditorTools.ReplaceClass(serializedObject, typeof(UIButton));
				Selection.activeGameObject = null;
			}
		}
	}

	protected virtual void DrawProperties ()
	{
		DrawTransition();
		DrawColors();
	}

	protected void DrawColors ()
	{
		if (serializedObject.FindProperty("tweenTarget").objectReferenceValue == null) return;

		if (NGUIEditorTools.DrawHeader("Colors", "Colors", false, true))
		{
			NGUIEditorTools.BeginContents(true);
			NGUIEditorTools.SetLabelWidth(76f);
			UIButtonColor btn = target as UIButtonColor;

			if (btn.tweenTarget != null)
			{
				UIWidget widget = btn.tweenTarget.GetComponent<UIWidget>();

				if (widget != null)
				{
					EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
					{
						SerializedObject obj = new SerializedObject(widget);
						obj.Update();
						NGUIEditorTools.DrawProperty("Normal", obj, "mColor");
						obj.ApplyModifiedProperties();
					}
					EditorGUI.EndDisabledGroup();
				}
			}

			NGUIEditorTools.DrawProperty("Hover", serializedObject, "hover");
			NGUIEditorTools.DrawProperty("Pressed", serializedObject, "pressed");
			NGUIEditorTools.DrawProperty("Disabled", serializedObject, "disabledColor");
			if (Application.isPlaying) EditorGUILayout.ColorField("Default", (target as UIButtonColor).defaultColor);
			NGUIEditorTools.EndContents();
		}
	}

	protected void DrawTransition ()
	{
		GUILayout.BeginHorizontal();
		NGUIEditorTools.DrawProperty("Transition", serializedObject, "duration", GUILayout.Width(120f));
		GUILayout.Label("seconds");
		GUILayout.EndHorizontal();
		GUILayout.Space(3f);
	}
}
