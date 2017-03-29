//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(UIForwardEvents))]
public class UIForwardEventsEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.HelpBox("This is a legacy component. Consider using the Event Trigger instead.", MessageType.Warning);
		base.OnInspectorGUI();
	}
}
