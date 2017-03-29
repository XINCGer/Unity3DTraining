//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(PropertyBinding))]
public class PropertyBindingEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		PropertyBinding pb = target as PropertyBinding;

		NGUIEditorTools.SetLabelWidth(80f);

		serializedObject.Update();

		if (pb.direction == PropertyBinding.Direction.TargetUpdatesSource && pb.target != null)
			PropertyReferenceDrawer.filter = pb.target.GetPropertyType();

		GUILayout.Space(3f);
		PropertyBinding.Direction dir = (target as PropertyBinding).direction;

		PropertyReferenceDrawer.mustRead = (dir == PropertyBinding.Direction.SourceUpdatesTarget ||
			dir == PropertyBinding.Direction.BiDirectional);
		PropertyReferenceDrawer.mustWrite = (dir == PropertyBinding.Direction.TargetUpdatesSource ||
			dir == PropertyBinding.Direction.BiDirectional);

		NGUIEditorTools.DrawProperty(serializedObject, "source");

		if (pb.direction == PropertyBinding.Direction.SourceUpdatesTarget && pb.source != null)
			PropertyReferenceDrawer.filter = pb.source.GetPropertyType();

		if (pb.source.target != null)
		{
			GUILayout.Space(-18f);

			if (pb.direction == PropertyBinding.Direction.TargetUpdatesSource)
			{
				GUILayout.Label("   \u25B2"); // Up
			}
			else if (pb.direction == PropertyBinding.Direction.SourceUpdatesTarget)
			{
				GUILayout.Label("   \u25BC"); // Down
			}
			else GUILayout.Label("  \u25B2\u25BC");
		}

		GUILayout.Space(1f);

		PropertyReferenceDrawer.mustRead = (dir == PropertyBinding.Direction.TargetUpdatesSource ||
			dir == PropertyBinding.Direction.BiDirectional);
		PropertyReferenceDrawer.mustWrite = (dir == PropertyBinding.Direction.SourceUpdatesTarget ||
			dir == PropertyBinding.Direction.BiDirectional);

		NGUIEditorTools.DrawProperty(serializedObject, "target");

		PropertyReferenceDrawer.mustRead = false;
		PropertyReferenceDrawer.mustWrite = false;
		PropertyReferenceDrawer.filter = typeof(void);

		GUILayout.Space(1f);
		NGUIEditorTools.DrawPaddedProperty(serializedObject, "direction");
		NGUIEditorTools.DrawPaddedProperty(serializedObject, "update");
		GUILayout.BeginHorizontal();
		NGUIEditorTools.DrawProperty(" ", serializedObject, "editMode", GUILayout.Width(100f));
		GUILayout.Label("Update in Edit Mode");
		GUILayout.EndHorizontal();

		if (!serializedObject.isEditingMultipleObjects)
		{
			if (pb.source != null && pb.target != null && pb.source.GetPropertyType() != pb.target.GetPropertyType())
			{
				if (pb.direction == PropertyBinding.Direction.BiDirectional)
				{
					EditorGUILayout.HelpBox("Bi-Directional updates require both Source and Target to reference values of the same type.", MessageType.Error);
				}
				else if (pb.direction == PropertyBinding.Direction.SourceUpdatesTarget)
				{
					if (!PropertyReference.Convert(pb.source.Get(), pb.target.GetPropertyType()))
					{
						EditorGUILayout.HelpBox("Unable to convert " + pb.source.GetPropertyType() + " to " + pb.target.GetPropertyType(), MessageType.Error);
					}
				}
				else if (!PropertyReference.Convert(pb.target.Get(), pb.source.GetPropertyType()))
				{
					EditorGUILayout.HelpBox("Unable to convert " + pb.target.GetPropertyType() + " to " + pb.source.GetPropertyType(), MessageType.Error);
				}
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
}
