//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UIDragDropItem is a base script for your own Drag & Drop operations.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None,
		Horizontal,
		Vertical,
		PressAndHold,
	}

	/// <summary>
	/// What kind of restriction is applied to the drag & drop logic before dragging is made possible.
	/// </summary>

	public Restriction restriction = Restriction.None;

	/// <summary>
	/// Whether a copy of the item will be dragged instead of the item itself.
	/// </summary>

	public bool cloneOnDrag = false;

	/// <summary>
	/// How long the user has to press on an item before the drag action activates.
	/// </summary>

	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	/// <summary>
	/// Whether this drag & drop item can be interacted with. If not, only tooltips will work.
	/// </summary>

	public bool interactable = true;

#region Common functionality

	[System.NonSerialized] protected Transform mTrans;
	[System.NonSerialized] protected Transform mParent;
	[System.NonSerialized] protected Collider mCollider;
	[System.NonSerialized] protected Collider2D mCollider2D;
	[System.NonSerialized] protected UIButton mButton;
	[System.NonSerialized] protected UIRoot mRoot;
	[System.NonSerialized] protected UIGrid mGrid;
	[System.NonSerialized] protected UITable mTable;
	[System.NonSerialized] protected float mDragStartTime = 0f;
	[System.NonSerialized] protected UIDragScrollView mDragScrollView = null;
	[System.NonSerialized] protected bool mPressed = false;
	[System.NonSerialized] protected bool mDragging = false;
	[System.NonSerialized] protected UICamera.MouseOrTouch mTouch;

	/// <summary>
	/// List of items that are currently being dragged.
	/// </summary>

	static public List<UIDragDropItem> draggedItems = new List<UIDragDropItem>();

	protected virtual void Awake ()
	{
		mTrans = transform;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		mCollider = collider;
		mCollider2D = collider2D;
#else
		mCollider = gameObject.GetComponent<Collider>();
		mCollider2D = gameObject.GetComponent<Collider2D>();
#endif
	}

	protected virtual void OnEnable () { }
	protected virtual void OnDisable () { if (mDragging) StopDragging(UICamera.hoveredObject); }

	/// <summary>
	/// Cache the transform.
	/// </summary>

	protected virtual void Start ()
	{
		mButton = GetComponent<UIButton>();
		mDragScrollView = GetComponent<UIDragScrollView>();
	}

	/// <summary>
	/// Record the time the item was pressed on.
	/// </summary>

	protected virtual void OnPress (bool isPressed)
	{
		if (!interactable || UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3) return;

		if (isPressed)
		{
			if (!mPressed)
			{
				mTouch = UICamera.currentTouch;
				mDragStartTime = RealTime.time + pressAndHoldDelay;
				mPressed = true;
			}
		}
		else if (mPressed && mTouch == UICamera.currentTouch)
		{
			mPressed = false;
			mTouch = null;
		}
	}

	/// <summary>
	/// Start the dragging operation after the item was held for a while.
	/// </summary>

	protected virtual void Update ()
	{
		if (restriction == Restriction.PressAndHold)
		{
			if (mPressed && !mDragging && mDragStartTime < RealTime.time)
				StartDragging();
		}
	}

	/// <summary>
	/// Start the dragging operation.
	/// </summary>

	protected virtual void OnDragStart ()
	{
		if (!interactable) return;
		if (!enabled || mTouch != UICamera.currentTouch) return;

		// If we have a restriction, check to see if its condition has been met first
		if (restriction != Restriction.None)
		{
			if (restriction == Restriction.Horizontal)
			{
				Vector2 delta = mTouch.totalDelta;
				if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
			}
			else if (restriction == Restriction.Vertical)
			{
				Vector2 delta = mTouch.totalDelta;
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) return;
			}
			else if (restriction == Restriction.PressAndHold)
			{
				// Checked in Update instead
				return;
			}
		}
		StartDragging();
	}

	/// <summary>
	/// Start the dragging operation.
	/// </summary>

	public virtual void StartDragging ()
	{
		if (!interactable) return;

		if (!mDragging)
		{
			if (cloneOnDrag)
			{
				mPressed = false;
				GameObject clone = transform.parent.gameObject.AddChild(gameObject);
				clone.transform.localPosition = transform.localPosition;
				clone.transform.localRotation = transform.localRotation;
				clone.transform.localScale = transform.localScale;

				UIButtonColor bc = clone.GetComponent<UIButtonColor>();
				if (bc != null) bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;

				if (mTouch != null && mTouch.pressed == gameObject)
				{
					mTouch.current = clone;
					mTouch.pressed = clone;
					mTouch.dragged = clone;
					mTouch.last = clone;
				}

				UIDragDropItem item = clone.GetComponent<UIDragDropItem>();
				item.mTouch = mTouch;
				item.mPressed = true;
				item.mDragging = true;
				item.Start();
				item.OnClone(gameObject);
				item.OnDragDropStart();

				if (UICamera.currentTouch == null)
					UICamera.currentTouch = mTouch;

				mTouch = null;

				UICamera.Notify(gameObject, "OnPress", false);
				UICamera.Notify(gameObject, "OnHover", false);
			}
			else
			{
				mDragging = true;
				OnDragDropStart();
			}
		}
	}

	/// <summary>
	/// Called on the cloned object when it was duplicated.
	/// </summary>

	protected virtual void OnClone (GameObject original) { }

	/// <summary>
	/// Perform the dragging.
	/// </summary>

	protected virtual void OnDrag (Vector2 delta)
	{
		if (!interactable) return;
		if (!mDragging || !enabled || mTouch != UICamera.currentTouch) return;
		if (mRoot != null) OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
		else OnDragDropMove(delta);
	}

	/// <summary>
	/// Notification sent when the drag event has ended.
	/// </summary>

	protected virtual void OnDragEnd ()
	{
		if (!interactable) return;
		if (!enabled || mTouch != UICamera.currentTouch) return;
		StopDragging(UICamera.hoveredObject);
	}

	/// <summary>
	/// Drop the dragged item.
	/// </summary>

	public void StopDragging (GameObject go)
	{
		if (mDragging)
		{
			mDragging = false;
			OnDragDropRelease(go);
		}
	}

#endregion

	/// <summary>
	/// Perform any logic related to starting the drag & drop operation.
	/// </summary>

	protected virtual void OnDragDropStart ()
	{
		if (!draggedItems.Contains(this))
			draggedItems.Add(this);

		// Automatically disable the scroll view
		if (mDragScrollView != null) mDragScrollView.enabled = false;

		// Disable the collider so that it doesn't intercept events
		if (mButton != null) mButton.isEnabled = false;
		else if (mCollider != null) mCollider.enabled = false;
		else if (mCollider2D != null) mCollider2D.enabled = false;

		mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
		mGrid = NGUITools.FindInParents<UIGrid>(mParent);
		mTable = NGUITools.FindInParents<UITable>(mParent);

		// Re-parent the item
		if (UIDragDropRoot.root != null)
			mTrans.parent = UIDragDropRoot.root;

		Vector3 pos = mTrans.localPosition;
		pos.z = 0f;
		mTrans.localPosition = pos;

		TweenPosition tp = GetComponent<TweenPosition>();
		if (tp != null) tp.enabled = false;

		SpringPosition sp = GetComponent<SpringPosition>();
		if (sp != null) sp.enabled = false;

		// Notify the widgets that the parent has changed
		NGUITools.MarkParentAsChanged(gameObject);

		if (mTable != null) mTable.repositionNow = true;
		if (mGrid != null) mGrid.repositionNow = true;
	}

	/// <summary>
	/// Adjust the dragged object's position.
	/// </summary>

	protected virtual void OnDragDropMove (Vector2 delta)
	{
		mTrans.localPosition += mTrans.InverseTransformDirection((Vector3)delta);
	}

	/// <summary>
	/// Drop the item onto the specified object.
	/// </summary>

	protected virtual void OnDragDropRelease (GameObject surface)
	{
		if (!cloneOnDrag)
		{
			// Clear the reference to the scroll view since it might be in another scroll view now
			var drags = GetComponentsInChildren<UIDragScrollView>();
			foreach (var d in drags) d.scrollView = null;

			// Re-enable the collider
			if (mButton != null) mButton.isEnabled = true;
			else if (mCollider != null) mCollider.enabled = true;
			else if (mCollider2D != null) mCollider2D.enabled = true;

			// Is there a droppable container?
			UIDragDropContainer container = surface ? NGUITools.FindInParents<UIDragDropContainer>(surface) : null;

			if (container != null)
			{
				// Container found -- parent this object to the container
				mTrans.parent = (container.reparentTarget != null) ? container.reparentTarget : container.transform;

				Vector3 pos = mTrans.localPosition;
				pos.z = 0f;
				mTrans.localPosition = pos;
			}
			else
			{
				// No valid container under the mouse -- revert the item's parent
				mTrans.parent = mParent;
			}

			// Update the grid and table references
			mParent = mTrans.parent;
			mGrid = NGUITools.FindInParents<UIGrid>(mParent);
			mTable = NGUITools.FindInParents<UITable>(mParent);

			// Re-enable the drag scroll view script
			if (mDragScrollView != null)
				Invoke("EnableDragScrollView", 0.001f);

			// Notify the widgets that the parent has changed
			NGUITools.MarkParentAsChanged(gameObject);

			if (mTable != null) mTable.repositionNow = true;
			if (mGrid != null) mGrid.repositionNow = true;
		}
		else NGUITools.Destroy(gameObject);

		// We're now done
		OnDragDropEnd();
	}

	/// <summary>
	/// Function called when the object gets reparented after the drop operation finishes.
	/// </summary>

	protected virtual void OnDragDropEnd () { draggedItems.Remove(this); }

	/// <summary>
	/// Re-enable the drag scroll view script at the end of the frame.
	/// Reason: http://www.tasharen.com/forum/index.php?topic=10203.0
	/// </summary>

	protected void EnableDragScrollView ()
	{
		if (mDragScrollView != null)
			mDragScrollView.enabled = true;
	}
}
