//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// When Drag & Drop event begins in UIDragDropItem, it will re-parent itself to the UIDragDropRoot instead.
/// It's useful when you're dragging something out of a clipped panel: you will want to reparent it before
/// it can be dragged outside.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag and Drop Root")]
public class UIDragDropRoot : MonoBehaviour
{
	static public Transform root;

	void OnEnable () { root = transform; }
	void OnDisable () { if (root == transform) root = null; }
}
