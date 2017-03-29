//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIGrid), true)]
public class UIGridEditor : UIWidgetContainerEditor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Arrangement", serializedObject, "arrangement");

		NGUIEditorTools.DrawProperty("  Cell Width", serializedObject, "cellWidth");
		NGUIEditorTools.DrawProperty("  Cell Height", serializedObject, "cellHeight");

		if (sp.intValue < 2)
		{
			bool columns = (sp.hasMultipleDifferentValues || (UIGrid.Arrangement)sp.intValue == UIGrid.Arrangement.Horizontal);

			GUILayout.BeginHorizontal();
			{
				sp = NGUIEditorTools.DrawProperty(columns ? "  Column Limit" : "  Row Limit", serializedObject, "maxPerLine");
				if (sp.intValue < 0) sp.intValue = 0;
				if (sp.intValue == 0) GUILayout.Label("Unlimited");
			}
			GUILayout.EndHorizontal();

			UIGrid.Sorting sort = (UIGrid.Sorting)NGUIEditorTools.DrawProperty("Sorting", serializedObject, "sorting").intValue;

			if (sp.intValue != 0 && (sort == UIGrid.Sorting.Horizontal || sort == UIGrid.Sorting.Vertical))
			{
				EditorGUILayout.HelpBox("Horizontal and Vertical sortinig only works if the number of rows/columns remains at 0.", MessageType.Warning);
			}
		}

		NGUIEditorTools.DrawProperty("Pivot", serializedObject, "pivot");
		NGUIEditorTools.DrawProperty("Smooth Tween", serializedObject, "animateSmoothly");
		NGUIEditorTools.DrawProperty("Hide Inactive", serializedObject, "hideInactive");
		NGUIEditorTools.DrawProperty("Constrain to Panel", serializedObject, "keepWithinPanel");
		serializedObject.ApplyModifiedProperties();
	}
}
