//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIEventTrigger))]
public class UIEventTriggerEditor : Editor
{
	UIEventTrigger mTrigger;

	void OnEnable ()
	{
		mTrigger = target as UIEventTrigger;
		EditorPrefs.SetBool("ET0", EventDelegate.IsValid(mTrigger.onHoverOver));
		EditorPrefs.SetBool("ET1", EventDelegate.IsValid(mTrigger.onHoverOut));
		EditorPrefs.SetBool("ET2", EventDelegate.IsValid(mTrigger.onPress));
		EditorPrefs.SetBool("ET3", EventDelegate.IsValid(mTrigger.onRelease));
		EditorPrefs.SetBool("ET4", EventDelegate.IsValid(mTrigger.onSelect));
		EditorPrefs.SetBool("ET5", EventDelegate.IsValid(mTrigger.onDeselect));
		EditorPrefs.SetBool("ET6", EventDelegate.IsValid(mTrigger.onClick));
		EditorPrefs.SetBool("ET7", EventDelegate.IsValid(mTrigger.onDoubleClick));
		EditorPrefs.SetBool("ETA", EventDelegate.IsValid(mTrigger.onDragStart));
		EditorPrefs.SetBool("ET8", EventDelegate.IsValid(mTrigger.onDragOver));
		EditorPrefs.SetBool("ET9", EventDelegate.IsValid(mTrigger.onDragOut));
		EditorPrefs.SetBool("ETB", EventDelegate.IsValid(mTrigger.onDragEnd));
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(3f);
		NGUIEditorTools.SetLabelWidth(80f);
		bool minimalistic = NGUISettings.minimalisticLook;
		DrawEvents("ET0", "On Hover Over", mTrigger.onHoverOver, minimalistic);
		DrawEvents("ET1", "On Hover Out", mTrigger.onHoverOut, minimalistic);
		DrawEvents("ET2", "On Press", mTrigger.onPress, minimalistic);
		DrawEvents("ET3", "On Release", mTrigger.onRelease, minimalistic);
		DrawEvents("ET4", "On Select", mTrigger.onSelect, minimalistic);
		DrawEvents("ET5", "On Deselect", mTrigger.onDeselect, minimalistic);
		DrawEvents("ET6", "On Click/Tap", mTrigger.onClick, minimalistic);
		DrawEvents("ET7", "On Double-Click/Tap", mTrigger.onDoubleClick, minimalistic);
		DrawEvents("ETA", "On Drag Start", mTrigger.onDragStart, minimalistic);
		DrawEvents("ET8", "On Drag Over", mTrigger.onDragOver, minimalistic);
		DrawEvents("ET9", "On Drag Out", mTrigger.onDragOut, minimalistic);
		DrawEvents("ETB", "On Drag End", mTrigger.onDragEnd, minimalistic);
	}

	void DrawEvents (string key, string text, List<EventDelegate> list, bool minimalistic)
	{
		if (!NGUIEditorTools.DrawHeader(text, key, false, minimalistic)) return;
		NGUIEditorTools.BeginContents();
		EventDelegateEditor.Field(mTrigger, list, null, null, minimalistic);
		NGUIEditorTools.EndContents();
	}
}
