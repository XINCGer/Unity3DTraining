//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIColorPicker))]
public class UIColorPickerEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		NGUIEditorTools.SetLabelWidth(100f);
		UIColorPicker picker = target as UIColorPicker;

		GUILayout.Space(6f);
		GUI.changed = false;

		NGUIEditorTools.DrawProperty(serializedObject, "value");
		NGUIEditorTools.DrawProperty(serializedObject, "selectionWidget");

		GUILayout.Space(6f);
		GUI.changed = false;

		NGUIEditorTools.DrawEvents("On Value Change", picker, picker.onChange);
		serializedObject.ApplyModifiedProperties();
	}
}
