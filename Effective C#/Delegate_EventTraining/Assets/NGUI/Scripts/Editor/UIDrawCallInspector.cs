//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector class used to view UIDrawCalls.
/// </summary>

[CustomEditor(typeof(UIDrawCall))]
public class UIDrawCallInspector : Editor
{
	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		if (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout)
		{
			UIDrawCall dc = target as UIDrawCall;

			if (dc.manager != null)
			{
				EditorGUILayout.LabelField("Render Queue", dc.renderQueue.ToString());
				EditorGUILayout.LabelField("Owner Panel", NGUITools.GetHierarchy(dc.manager.gameObject));
				EditorGUILayout.LabelField("Triangles", dc.triangles.ToString());
			}
			else if (Event.current.type == EventType.Repaint)
			{
				Debug.LogWarning("Orphaned UIDrawCall detected!\nUse [Selection -> Force Delete] to get rid of it.");
			}
		}
	}
}
