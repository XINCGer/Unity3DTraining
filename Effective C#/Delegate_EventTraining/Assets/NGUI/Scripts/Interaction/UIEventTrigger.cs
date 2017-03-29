//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attaching this script to an object will let you trigger remote functions using NGUI events.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	static public UIEventTrigger current;

	public List<EventDelegate> onHoverOver = new List<EventDelegate>();
	public List<EventDelegate> onHoverOut = new List<EventDelegate>();
	public List<EventDelegate> onPress = new List<EventDelegate>();
	public List<EventDelegate> onRelease = new List<EventDelegate>();
	public List<EventDelegate> onSelect = new List<EventDelegate>();
	public List<EventDelegate> onDeselect = new List<EventDelegate>();
	public List<EventDelegate> onClick = new List<EventDelegate>();
	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();
	public List<EventDelegate> onDragStart = new List<EventDelegate>();
	public List<EventDelegate> onDragEnd = new List<EventDelegate>();
	public List<EventDelegate> onDragOver = new List<EventDelegate>();
	public List<EventDelegate> onDragOut = new List<EventDelegate>();
	public List<EventDelegate> onDrag = new List<EventDelegate>();

	/// <summary>
	/// Whether the collider is enabled and the widget can be interacted with.
	/// </summary>

	public bool isColliderEnabled
	{
		get
		{
			Collider c = GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
	}

	void OnHover (bool isOver)
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		if (isOver) EventDelegate.Execute(onHoverOver);
		else EventDelegate.Execute(onHoverOut);
		current = null;
	}

	void OnPress (bool pressed)
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		if (pressed) EventDelegate.Execute(onPress);
		else EventDelegate.Execute(onRelease);
		current = null;
	}

	void OnSelect (bool selected)
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		if (selected) EventDelegate.Execute(onSelect);
		else EventDelegate.Execute(onDeselect);
		current = null;
	}

	void OnClick ()
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		EventDelegate.Execute(onClick);
		current = null;
	}

	void OnDoubleClick ()
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		EventDelegate.Execute(onDoubleClick);
		current = null;
	}

	void OnDragStart ()
	{
		if (current != null) return;
		current = this;
		EventDelegate.Execute(onDragStart);
		current = null;
	}

	void OnDragEnd ()
	{
		if (current != null) return;
		current = this;
		EventDelegate.Execute(onDragEnd);
		current = null;
	}

	void OnDragOver (GameObject go)
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		EventDelegate.Execute(onDragOver);
		current = null;
	}

	void OnDragOut (GameObject go)
	{
		if (current != null || !isColliderEnabled) return;
		current = this;
		EventDelegate.Execute(onDragOut);
		current = null;
	}

	void OnDrag (Vector2 delta)
	{
		if (current != null) return;
		current = this;
		EventDelegate.Execute(onDrag);
		current = null;
	}
}
