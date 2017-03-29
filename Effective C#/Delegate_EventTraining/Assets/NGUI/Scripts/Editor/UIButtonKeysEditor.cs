//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIButtonKeys))]
#else
[CustomEditor(typeof(UIButtonKeys), true)]
#endif
public class UIButtonKeysEditor : UIKeyNavigationEditor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		EditorGUILayout.HelpBox("This component has been replaced by UIKeyNavigation.", MessageType.Warning);

		if (GUILayout.Button("Auto-Upgrade"))
		{
			NGUIEditorTools.ReplaceClass(serializedObject, typeof(UIKeyNavigation));
			Selection.activeGameObject = null;
		}
	}
}
