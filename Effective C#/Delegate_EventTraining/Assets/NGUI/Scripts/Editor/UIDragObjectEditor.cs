//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIDragObject))]
public class UIDragObjectEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		serializedObject.Update();

		NGUIEditorTools.SetLabelWidth(100f);

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Target", serializedObject, "target");

		EditorGUI.BeginDisabledGroup(sp.objectReferenceValue == null);
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Movement", GUILayout.Width(78f));
			NGUIEditorTools.DrawPaddedProperty("", serializedObject, "scale");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Scroll Wheel", GUILayout.Width(78f));
			NGUIEditorTools.DrawPaddedProperty("", serializedObject, "scrollMomentum");
			GUILayout.EndHorizontal();

			sp = NGUIEditorTools.DrawPaddedProperty("Drag Effect", serializedObject, "dragEffect");

			if (sp.hasMultipleDifferentValues || (UIDragObject.DragEffect)sp.intValue != UIDragObject.DragEffect.None)
			{
				NGUIEditorTools.DrawProperty("  Momentum", serializedObject, "momentumAmount", GUILayout.Width(140f));
			}

			sp = NGUIEditorTools.DrawProperty("Keep Visible", serializedObject, "restrictWithinPanel");

			if (sp.hasMultipleDifferentValues || sp.boolValue)
			{
				NGUIEditorTools.DrawProperty("  Content Rect", serializedObject, "contentRect");
				NGUIEditorTools.DrawProperty("  Panel Region", serializedObject, "panelRegion");
			}
		}
		EditorGUI.EndDisabledGroup();

		serializedObject.ApplyModifiedProperties();
	}
}
