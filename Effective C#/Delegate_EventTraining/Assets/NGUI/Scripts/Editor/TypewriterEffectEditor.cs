//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TypewriterEffect))]
public class TypewriterEffectEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		serializedObject.Update();

		NGUIEditorTools.DrawProperty(serializedObject, "charsPerSecond");
		NGUIEditorTools.DrawProperty(serializedObject, "fadeInTime");
		NGUIEditorTools.DrawProperty(serializedObject, "delayOnPeriod");
		NGUIEditorTools.DrawProperty(serializedObject, "delayOnNewLine");
		NGUIEditorTools.DrawProperty(serializedObject, "scrollView");
		NGUIEditorTools.DrawProperty(serializedObject, "keepFullDimensions");

		TypewriterEffect tw = target as TypewriterEffect;
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);

		serializedObject.ApplyModifiedProperties();
	}
}
