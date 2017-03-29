//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIKeyNavigation))]
#else
[CustomEditor(typeof(UIKeyNavigation), true)]
#endif
public class UIKeyNavigationEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		serializedObject.Update();
		NGUIEditorTools.DrawProperty("Starts Selected", serializedObject, "startsSelected");
		NGUIEditorTools.DrawProperty("Constraint", serializedObject, "constraint");

		if (NGUIEditorTools.DrawHeader("Override"))
		{
			NGUIEditorTools.SetLabelWidth(60f);
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.DrawProperty("Left", serializedObject, "onLeft");
			NGUIEditorTools.DrawProperty("Right", serializedObject, "onRight");
			NGUIEditorTools.DrawProperty("Up", serializedObject, "onUp");
			NGUIEditorTools.DrawProperty("Down", serializedObject, "onDown");
			NGUIEditorTools.DrawProperty("OnClick", serializedObject, "onClick");

			if (serializedObject.isEditingMultipleObjects || (target as UIKeyNavigation).GetComponent<UIInput>() != null)
				NGUIEditorTools.DrawProperty("Tab", serializedObject, "onTab");

			NGUIEditorTools.EndContents();
		}

		serializedObject.ApplyModifiedProperties();
	}
}
