//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY
#define MOBILE
#endif

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIInput))]
#else
[CustomEditor(typeof(UIInput), true)]
#endif
public class UIInputEditor : UIWidgetContainerEditor
{
	public override void OnInspectorGUI ()
	{
		UIInput input = target as UIInput;
		serializedObject.Update();
		GUILayout.Space(3f);
		NGUIEditorTools.SetLabelWidth(110f);
		//NGUIEditorTools.DrawProperty(serializedObject, "m_Script");

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		SerializedProperty label = NGUIEditorTools.DrawProperty(serializedObject, "label");
		EditorGUI.EndDisabledGroup();

		EditorGUI.BeginDisabledGroup(label == null || label.objectReferenceValue == null);
		{
			if (Application.isPlaying) NGUIEditorTools.DrawPaddedProperty("Value", serializedObject, "mValue");
			else NGUIEditorTools.DrawPaddedProperty("Starting Value", serializedObject, "mValue");
			NGUIEditorTools.DrawPaddedProperty(serializedObject, "savedAs");
			NGUIEditorTools.DrawProperty("Active Text Color", serializedObject, "activeTextColor");

			EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
			{
				if (label != null && label.objectReferenceValue != null)
				{
					SerializedObject ob = new SerializedObject(label.objectReferenceValue);
					ob.Update();
					NGUIEditorTools.DrawProperty("Inactive Color", ob, "mColor");
					ob.ApplyModifiedProperties();
				}
				else EditorGUILayout.ColorField("Inactive Color", Color.white);
			}
			EditorGUI.EndDisabledGroup();

			NGUIEditorTools.DrawProperty("Caret Color", serializedObject, "caretColor");
			NGUIEditorTools.DrawProperty("Selection Color", serializedObject, "selectionColor");
			NGUIEditorTools.DrawPaddedProperty(serializedObject, "inputType");
			NGUIEditorTools.DrawPaddedProperty(serializedObject, "validation");
			NGUIEditorTools.DrawPaddedProperty("Mobile Keyboard", serializedObject, "keyboardType");
			NGUIEditorTools.DrawPaddedProperty("  Hide Input", serializedObject, "hideInput");
			NGUIEditorTools.DrawPaddedProperty(serializedObject, "onReturnKey");

			// Deprecated, use UIKeyNavigation instead.
			//NGUIEditorTools.DrawProperty(serializedObject, "selectOnTab");

			SerializedProperty sp = serializedObject.FindProperty("characterLimit");

			GUILayout.BeginHorizontal();

			if (sp.hasMultipleDifferentValues || input.characterLimit > 0)
			{
				EditorGUILayout.PropertyField(sp);
				NGUIEditorTools.DrawPadding();
			}
			else
			{
				EditorGUILayout.PropertyField(sp);
				GUILayout.Label("unlimited");
			}
			GUILayout.EndHorizontal();

			NGUIEditorTools.SetLabelWidth(80f);
			EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
			NGUIEditorTools.DrawEvents("On Submit", input, input.onSubmit);
			NGUIEditorTools.DrawEvents("On Change", input, input.onChange);
			EditorGUI.EndDisabledGroup();
		}
		EditorGUI.EndDisabledGroup();
		serializedObject.ApplyModifiedProperties();
	}
}
