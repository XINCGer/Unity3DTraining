//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenOrthoSize))]
public class TweenOrthoSizeEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenOrthoSize tw = target as TweenOrthoSize;
		GUI.changed = false;

		float from = EditorGUILayout.FloatField("From", tw.from);
		float to = EditorGUILayout.FloatField("To", tw.to);

		if (from < 0f) from = 0f;
		if (to < 0f) to = 0f;

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
			NGUITools.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
