//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This script, when attached to a panel turns it into a scroll view.
/// You can then attach UIDragScrollView to colliders within to make it draggable.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Interaction/Scroll View")]
public class UIScrollView : MonoBehaviour
{
	static public BetterList<UIScrollView> list = new BetterList<UIScrollView>();

	public enum Movement
	{
		Horizontal,
		Vertical,
		Unrestricted,
		Custom,
	}

	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring,
	}

	public enum ShowCondition
	{
		Always,
		OnlyIfNeeded,
		WhenDragging,
	}

	public delegate void OnDragNotification ();

	/// <summary>
	/// Type of movement allowed by the scroll view.
	/// </summary>

	public Movement movement = Movement.Horizontal;

	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	/// <summary>
	/// Whether the dragging will be restricted to be within the scroll view's bounds.
	/// </summary>

	public bool restrictWithinPanel = true;

	/// <summary>
	/// Whether the scroll view will execute its constrain within bounds logic on every drag operation.
	/// </summary>

	[Tooltip("Whether the scroll view will execute its constrain within bounds logic on every drag operation")]
	public bool constrainOnDrag = false;

	/// <summary>
	/// Whether dragging will be disabled if the contents fit.
	/// </summary>

	public bool disableDragIfFits = false;

	/// <summary>
	/// Whether the drag operation will be started smoothly, or if if it will be precise (but will have a noticeable "jump").
	/// </summary>

	public bool smoothDragStart = true;

	/// <summary>
	/// Whether to use iOS drag emulation, where the content only drags at half the speed of the touch/mouse movement when the content edge is within the clipping area.
	/// </summary>	
	
	public bool iOSDragEmulation = true;

	/// <summary>
	/// Effect the scroll wheel will have on the momentum.
	/// </summary>

	public float scrollWheelFactor = 0.25f;

	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>

	public float momentumAmount = 35f;

	/// <summary>
	/// Strength of the spring dampening effect.
	/// </summary>

	public float dampenStrength = 9f;

	/// <summary>
	/// Horizontal scrollbar used for visualization.
	/// </summary>

	public UIProgressBar horizontalScrollBar;

	/// <summary>
	/// Vertical scrollbar used for visualization.
	/// </summary>

	public UIProgressBar verticalScrollBar;

	/// <summary>
	/// Condition that must be met for the scroll bars to become visible.
	/// </summary>

	public ShowCondition showScrollBars = ShowCondition.OnlyIfNeeded;

	/// <summary>
	/// Custom movement, if the 'movement' field is set to 'Custom'.
	/// </summary>

	public Vector2 customMovement = new Vector2(1f, 0f);

	/// <summary>
	/// Content's pivot point -- where it originates from by default.
	/// </summary>

	public UIWidget.Pivot contentPivot = UIWidget.Pivot.TopLeft;

	/// <summary>
	/// Event callback to trigger when the drag process begins.
	/// </summary>

	public OnDragNotification onDragStarted;

	/// <summary>
	/// Event callback to trigger when the drag process finished. Can be used for additional effects, such as centering on some object.
	/// </summary>

	public OnDragNotification onDragFinished;

	/// <summary>
	/// Event callback triggered when the scroll view is moving as a result of momentum in between of OnDragFinished and OnStoppedMoving.
	/// </summary>

	public OnDragNotification onMomentumMove;

	/// <summary>
	/// Event callback to trigger when the scroll view's movement ends.
	/// </summary>

	public OnDragNotification onStoppedMoving;

	// Deprecated functionality. Use 'movement' instead.
	[HideInInspector][SerializeField] Vector3 scale = new Vector3(1f, 0f, 0f);

	// Deprecated functionality. Use 'contentPivot' instead.
	[SerializeField][HideInInspector] Vector2 relativePositionOnReset = Vector2.zero;

	protected Transform mTrans;
	protected UIPanel mPanel;
	protected Plane mPlane;
	protected Vector3 mLastPos;
	protected bool mPressed = false;
	protected Vector3 mMomentum = Vector3.zero;
	protected float mScroll = 0f;
	protected Bounds mBounds;
	protected bool mCalculatedBounds = false;
	protected bool mShouldMove = false;
	protected bool mIgnoreCallbacks = false;
	protected int mDragID = -10;
	protected Vector2 mDragStartOffset = Vector2.zero;
	protected bool mDragStarted = false;

	/// <summary>
	/// Panel that's being dragged.
	/// </summary>

	public UIPanel panel { get { return mPanel; } }

	/// <summary>
	/// Whether the scroll view is being dragged.
	/// </summary>

	public bool isDragging { get { return mPressed && mDragStarted; } }

	/// <summary>
	/// Calculate the bounds used by the widgets.
	/// </summary>

	public virtual Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
				mCalculatedBounds = true;
				mTrans = transform;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
			}
			return mBounds;
		}
	}

	/// <summary>
	/// Whether the scroll view can move horizontally.
	/// </summary>

	public bool canMoveHorizontally
	{
		get
		{
			return movement == Movement.Horizontal ||
				movement == Movement.Unrestricted ||
				(movement == Movement.Custom && customMovement.x != 0f);
		}
	}

	/// <summary>
	/// Whether the scroll view can move vertically.
	/// </summary>

	public bool canMoveVertically
	{
		get
		{
			return movement == Movement.Vertical ||
				movement == Movement.Unrestricted ||
				(movement == Movement.Custom && customMovement.y != 0f);
		}
	}

	/// <summary>
	/// Whether the scroll view should be able to move horizontally (contents don't fit).
	/// </summary>

	public virtual bool shouldMoveHorizontally
	{
		get
		{
			float size = bounds.size.x;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip) size += mPanel.clipSoftness.x * 2f;
			return Mathf.RoundToInt(size - mPanel.width) > 0;
		}
	}

	/// <summary>
	/// Whether the scroll view should be able to move vertically (contents don't fit).
	/// </summary>

	public virtual bool shouldMoveVertically
	{
		get
		{
			float size = bounds.size.y;
			if (mPanel.clipping == UIDrawCall.Clipping.SoftClip) size += mPanel.clipSoftness.y * 2f;
			return Mathf.RoundToInt(size - mPanel.height) > 0;
		}
	}

	/// <summary>
	/// Whether the contents of the scroll view should actually be draggable depends on whether they currently fit or not.
	/// </summary>

	protected virtual bool shouldMove
	{
		get
		{
			if (!disableDragIfFits) return true;

			if (mPanel == null) mPanel = GetComponent<UIPanel>();
			Vector4 clip = mPanel.finalClipRegion;
			Bounds b = bounds;

			float hx = (clip.z == 0f) ? Screen.width  : clip.z * 0.5f;
			float hy = (clip.w == 0f) ? Screen.height : clip.w * 0.5f;

			if (canMoveHorizontally)
			{
				if (b.min.x < clip.x - hx) return true;
				if (b.max.x > clip.x + hx) return true;
			}

			if (canMoveVertically)
			{
				if (b.min.y < clip.y - hy) return true;
				if (b.max.y > clip.y + hy) return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Current momentum, exposed just in case it's needed.
	/// </summary>

	public Vector3 currentMomentum
	{
		get
		{
			return mMomentum;
		}
		set
		{
			mMomentum = value;
			mShouldMove = true;
		}
	}

	/// <summary>
	/// Cache the transform and the panel.
	/// </summary>

	void Awake ()
	{
		mTrans = transform;
		mPanel = GetComponent<UIPanel>();

		if (mPanel.clipping == UIDrawCall.Clipping.None)
			mPanel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
		
		// Auto-upgrade
		if (movement != Movement.Custom && scale.sqrMagnitude > 0.001f)
		{
			if (scale.x == 1f && scale.y == 0f)
			{
				movement = Movement.Horizontal;
			}
			else if (scale.x == 0f && scale.y == 1f)
			{
				movement = Movement.Vertical;
			}
			else if (scale.x == 1f && scale.y == 1f)
			{
				movement = Movement.Unrestricted;
			}
			else
			{
				movement = Movement.Custom;
				customMovement.x = scale.x;
				customMovement.y = scale.y;
			}
			scale = Vector3.zero;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

		// Auto-upgrade
		if (contentPivot == UIWidget.Pivot.TopLeft && relativePositionOnReset != Vector2.zero)
		{
			contentPivot = NGUIMath.GetPivot(new Vector2(relativePositionOnReset.x, 1f - relativePositionOnReset.y));
			relativePositionOnReset = Vector2.zero;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	[System.NonSerialized] bool mStarted = false;

	void OnEnable ()
	{
		list.Add(this);
		if (mStarted && Application.isPlaying) CheckScrollbars();
	}

	void Start ()
	{
		mStarted = true;
		if (Application.isPlaying) CheckScrollbars();
	}

	void CheckScrollbars ()
	{
		if (horizontalScrollBar != null)
		{
			EventDelegate.Add(horizontalScrollBar.onChange, OnScrollBar);
			horizontalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			horizontalScrollBar.alpha = ((showScrollBars == ShowCondition.Always) || shouldMoveHorizontally) ? 1f : 0f;
		}

		if (verticalScrollBar != null)
		{
			EventDelegate.Add(verticalScrollBar.onChange, OnScrollBar);
			verticalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			verticalScrollBar.alpha = ((showScrollBars == ShowCondition.Always) || shouldMoveVertically) ? 1f : 0f;
		}
	}

	void OnDisable () { list.Remove(this); mPressed = false; }

	/// <summary>
	/// Restrict the scroll view's contents to be within the scroll view's bounds.
	/// </summary>

	public bool RestrictWithinBounds (bool instant) { return RestrictWithinBounds(instant, true, true); }

	/// <summary>
	/// Restrict the scroll view's contents to be within the scroll view's bounds.
	/// </summary>

	public bool RestrictWithinBounds (bool instant, bool horizontal, bool vertical)
	{
		if (mPanel == null) return false;

		Bounds b = bounds;
		Vector3 constraint = mPanel.CalculateConstrainOffset(b.min, b.max);

		if (!horizontal) constraint.x = 0f;
		if (!vertical) constraint.y = 0f;

		if (constraint.sqrMagnitude > 0.1f)
		{
			if (!instant && dragEffect == DragEffect.MomentumAndSpring)
			{
				// Spring back into place
				Vector3 pos = mTrans.localPosition + constraint;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				SpringPanel.Begin(mPanel.gameObject, pos, 8f);
			}
			else
			{
				// Jump back into place
				MoveRelative(constraint);

				// Clear the momentum in the constrained direction
				if (Mathf.Abs(constraint.x) > 0.01f) mMomentum.x = 0;
				if (Mathf.Abs(constraint.y) > 0.01f) mMomentum.y = 0;
				if (Mathf.Abs(constraint.z) > 0.01f) mMomentum.z = 0;
				mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Disable the spring movement.
	/// </summary>

	public void DisableSpring ()
	{
		SpringPanel sp = GetComponent<SpringPanel>();
		if (sp != null) sp.enabled = false;
	}

	/// <summary>
	/// Update the values of the associated scroll bars.
	/// </summary>

	public void UpdateScrollbars () { UpdateScrollbars(true); }

	/// <summary>
	/// Update the values of the associated scroll bars.
	/// </summary>

	public virtual void UpdateScrollbars (bool recalculateBounds)
	{
		if (mPanel == null) return;

		if (horizontalScrollBar != null || verticalScrollBar != null)
		{
			if (recalculateBounds)
			{
				mCalculatedBounds = false;
				mShouldMove = shouldMove;
			}

			Bounds b = bounds;
			Vector2 bmin = b.min;
			Vector2 bmax = b.max;

			if (horizontalScrollBar != null && bmax.x > bmin.x)
			{
				Vector4 clip = mPanel.finalClipRegion;
				int intViewSize = Mathf.RoundToInt(clip.z);
				if ((intViewSize & 1) != 0) intViewSize -= 1;
				float halfViewSize = intViewSize * 0.5f;
				halfViewSize = Mathf.Round(halfViewSize);

				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
					halfViewSize -= mPanel.clipSoftness.x;

				float contentSize = bmax.x - bmin.x;
				float viewSize = halfViewSize * 2f;
				float contentMin = bmin.x;
				float contentMax = bmax.x;
				float viewMin = clip.x - halfViewSize;
				float viewMax = clip.x + halfViewSize;

				contentMin = viewMin - contentMin;
				contentMax = contentMax - viewMax;

				UpdateScrollbars(horizontalScrollBar, contentMin, contentMax, contentSize, viewSize, false);
			}

			if (verticalScrollBar != null && bmax.y > bmin.y)
			{
				Vector4 clip = mPanel.finalClipRegion;
				int intViewSize = Mathf.RoundToInt(clip.w);
				if ((intViewSize & 1) != 0) intViewSize -= 1;
				float halfViewSize = intViewSize * 0.5f;
				halfViewSize = Mathf.Round(halfViewSize);

				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
					halfViewSize -= mPanel.clipSoftness.y;

				float contentSize = bmax.y - bmin.y;
				float viewSize = halfViewSize * 2f;
				float contentMin = bmin.y;
				float contentMax = bmax.y;
				float viewMin = clip.y - halfViewSize;
				float viewMax = clip.y + halfViewSize;

				contentMin = viewMin - contentMin;
				contentMax = contentMax - viewMax;

				UpdateScrollbars(verticalScrollBar, contentMin, contentMax, contentSize, viewSize, true);
			}
		}
		else if (recalculateBounds)
		{
			mCalculatedBounds = false;
		}
	}

	/// <summary>
	/// Helper function used in UpdateScrollbars(float) function above.
	/// </summary>

	protected void UpdateScrollbars (UIProgressBar slider, float contentMin, float contentMax, float contentSize, float viewSize, bool inverted)
	{
		if (slider == null) return;

		mIgnoreCallbacks = true;
		{
			float contentPadding;

			if (viewSize < contentSize)
			{
				contentMin = Mathf.Clamp01(contentMin / contentSize);
				contentMax = Mathf.Clamp01(contentMax / contentSize);

				contentPadding = contentMin + contentMax;
				slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
					((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);
			}
			else
			{
				contentMin = Mathf.Clamp01(-contentMin / contentSize);
				contentMax = Mathf.Clamp01(-contentMax / contentSize);

				contentPadding = contentMin + contentMax;
				slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
					((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);

				if (contentSize > 0)
				{
					contentMin = Mathf.Clamp01(contentMin / contentSize);
					contentMax = Mathf.Clamp01(contentMax / contentSize);
					contentPadding = contentMin + contentMax;
				}
			}

			UIScrollBar sb = slider as UIScrollBar;
			if (sb != null) sb.barSize = 1f - contentPadding;
		}
		mIgnoreCallbacks = false;
	}

	/// <summary>
	/// Changes the drag amount of the scroll view to the specified 0-1 range values.
	/// (0, 0) is the top-left corner, (1, 1) is the bottom-right.
	/// </summary>

	public virtual void SetDragAmount (float x, float y, bool updateScrollbars)
	{
		if (mPanel == null) mPanel = GetComponent<UIPanel>();

		DisableSpring();

		Bounds b = bounds;
		if (b.min.x == b.max.x || b.min.y == b.max.y) return;

		Vector4 clip = mPanel.finalClipRegion;

		float hx = clip.z * 0.5f;
		float hy = clip.w * 0.5f;
		float left = b.min.x + hx;
		float right = b.max.x - hx;
		float bottom = b.min.y + hy;
		float top = b.max.y - hy;

		if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			left -= mPanel.clipSoftness.x;
			right += mPanel.clipSoftness.x;
			bottom -= mPanel.clipSoftness.y;
			top += mPanel.clipSoftness.y;
		}

		// Calculate the offset based on the scroll value
		float ox = Mathf.Lerp(left, right, x);
		float oy = Mathf.Lerp(top, bottom, y);

		// Update the position
		if (!updateScrollbars)
		{
			Vector3 pos = mTrans.localPosition;
			if (canMoveHorizontally) pos.x += clip.x - ox;
			if (canMoveVertically) pos.y += clip.y - oy;
			mTrans.localPosition = pos;
		}

		if (canMoveHorizontally) clip.x = ox;
		if (canMoveVertically) clip.y = oy;

		// Update the clipping offset
		Vector4 cr = mPanel.baseClipRegion;
		mPanel.clipOffset = new Vector2(clip.x - cr.x, clip.y - cr.y);

		// Update the scrollbars, reflecting this change
		if (updateScrollbars) UpdateScrollbars(mDragID == -10);
	}

	/// <summary>
	/// Manually invalidate the scroll view's bounds so that they update next time.
	/// </summary>

	public void InvalidateBounds () { mCalculatedBounds = false; }

	/// <summary>
	/// Reset the scroll view's position to the top-left corner.
	/// It's recommended to call this function before AND after you re-populate the scroll view's contents (ex: switching window tabs).
	/// Another option is to populate the scroll view's contents, reset its position, then call this function to reposition the clipping.
	/// </summary>

	[ContextMenu("Reset Clipping Position")]
	public void ResetPosition()
	{
		if (NGUITools.GetActive(this))
		{
			// Invalidate the bounds
			mCalculatedBounds = false;
			Vector2 pv = NGUIMath.GetPivotOffset(contentPivot);

			// First move the position back to where it would be if the scroll bars got reset to zero
			SetDragAmount(pv.x, 1f - pv.y, false);

			// Next move the clipping area back and update the scroll bars
			SetDragAmount(pv.x, 1f - pv.y, true);
		}
	}

	/// <summary>
	/// Call this function after you adjust the scroll view's bounds if you want it to maintain the current scrolled position
	/// </summary>

	public void UpdatePosition ()
	{
		if (!mIgnoreCallbacks && (horizontalScrollBar != null || verticalScrollBar != null))
		{
			mIgnoreCallbacks = true;
			mCalculatedBounds = false;
			Vector2 pv = NGUIMath.GetPivotOffset(contentPivot);
			float x = (horizontalScrollBar != null) ? horizontalScrollBar.value : pv.x;
			float y = (verticalScrollBar != null) ? verticalScrollBar.value : 1f - pv.y;
			SetDragAmount(x, y, false);
			UpdateScrollbars(true);
			mIgnoreCallbacks = false;
		}
	}

	/// <summary>
	/// Triggered by the scroll bars when they change.
	/// </summary>

	public void OnScrollBar ()
	{
		if (!mIgnoreCallbacks)
		{
			mIgnoreCallbacks = true;
			float x = (horizontalScrollBar != null) ? horizontalScrollBar.value : 0f;
			float y = (verticalScrollBar != null) ? verticalScrollBar.value : 0f;
			SetDragAmount(x, y, false);
			mIgnoreCallbacks = false;
		}
	}

	/// <summary>
	/// Move the scroll view by the specified local space amount.
	/// </summary>

	public virtual void MoveRelative (Vector3 relative)
	{
		mTrans.localPosition += relative;
		Vector2 co = mPanel.clipOffset;
		co.x -= relative.x;
		co.y -= relative.y;
		mPanel.clipOffset = co;

		// Update the scroll bars
		UpdateScrollbars(false);
	}

	/// <summary>
	/// Move the scroll view by the specified world space amount.
	/// </summary>

	public void MoveAbsolute (Vector3 absolute)
	{
		Vector3 a = mTrans.InverseTransformPoint(absolute);
		Vector3 b = mTrans.InverseTransformPoint(Vector3.zero);
		MoveRelative(a - b);
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	public void Press (bool pressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

		if (smoothDragStart && pressed)
		{
			mDragStarted = false;
			mDragStartOffset = Vector2.zero;
		}

		if (enabled && NGUITools.GetActive(gameObject))
		{
			if (!pressed && mDragID == UICamera.currentTouchID) mDragID = -10;

			mCalculatedBounds = false;
			mShouldMove = shouldMove;
			if (!mShouldMove) return;
			mPressed = pressed;

			if (pressed)
			{
				// Remove all momentum on press
				mMomentum = Vector3.zero;
				mScroll = 0f;

				// Disable the spring movement
				DisableSpring();

				// Remember the hit position
				mLastPos = UICamera.lastWorldPosition;

				// Create the plane to drag along
				mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);

				// Ensure that we're working with whole numbers, keeping everything pixel-perfect
				Vector2 co = mPanel.clipOffset;
				co.x = Mathf.Round(co.x);
				co.y = Mathf.Round(co.y);
				mPanel.clipOffset = co;

				Vector3 v = mTrans.localPosition;
				v.x = Mathf.Round(v.x);
				v.y = Mathf.Round(v.y);
				mTrans.localPosition = v;

				if (!smoothDragStart)
				{
					mDragStarted = true;
					mDragStartOffset = Vector2.zero;
					if (onDragStarted != null) onDragStarted();
				}
			}
			else if (centerOnChild)
			{
				centerOnChild.Recenter();
			}
			else
			{
				if (mDragStarted && restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
					RestrictWithinBounds(dragEffect == DragEffect.None, canMoveHorizontally, canMoveVertically);

				if (mDragStarted && onDragFinished != null) onDragFinished();
				if (!mShouldMove && onStoppedMoving != null)
					onStoppedMoving();
			}
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	public void Drag ()
	{
		if (!mPressed || UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

		if (enabled && NGUITools.GetActive(gameObject) && mShouldMove)
		{
			if (mDragID == -10) mDragID = UICamera.currentTouchID;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

			// Prevents the drag "jump". Contributed by 'mixd' from the Tasharen forums.
			if (smoothDragStart && !mDragStarted)
			{
				mDragStarted = true;
				mDragStartOffset = UICamera.currentTouch.totalDelta;
				if (onDragStarted != null) onDragStarted();
			}

			Ray ray = smoothDragStart ?
				UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - mDragStartOffset) :
				UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);

			float dist = 0f;

			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;

				if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
				{
					offset = mTrans.InverseTransformDirection(offset);

					if (movement == Movement.Horizontal)
					{
						offset.y = 0f;
						offset.z = 0f;
					}
					else if (movement == Movement.Vertical)
					{
						offset.x = 0f;
						offset.z = 0f;
					}
					else if (movement == Movement.Unrestricted)
					{
						offset.z = 0f;
					}
					else
					{
						offset.Scale((Vector3)customMovement);
					}
					offset = mTrans.TransformDirection(offset);
				}

				// Adjust the momentum
				if (dragEffect == DragEffect.None) mMomentum = Vector3.zero;
				else mMomentum = Vector3.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);

				// Move the scroll view
				if (!iOSDragEmulation || dragEffect != DragEffect.MomentumAndSpring)
				{
					MoveAbsolute(offset);
				}
				else
				{
					Vector3 constraint = mPanel.CalculateConstrainOffset(bounds.min, bounds.max);

					if (movement == Movement.Horizontal)
					{
						constraint.y = 0f;
					}
					else if (movement == Movement.Vertical)
					{
						constraint.x = 0f;
					}
					else if (movement == Movement.Custom)
					{
						constraint.x *= customMovement.x;
						constraint.y *= customMovement.y;
					}

					if (constraint.magnitude > 1f)
					{
						MoveAbsolute(offset * 0.5f);
						mMomentum *= 0.5f;
					}
					else MoveAbsolute(offset);
				}

				// We want to constrain the UI to be within bounds
				if (constrainOnDrag && restrictWithinPanel &&
					mPanel.clipping != UIDrawCall.Clipping.None &&
					dragEffect != DragEffect.MomentumAndSpring)
				{
					RestrictWithinBounds(true, canMoveHorizontally, canMoveVertically);
				}
			}
		}
	}

	[HideInInspector]
	public UICenterOnChild centerOnChild = null;

	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	public void Scroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && scrollWheelFactor != 0f)
		{
			DisableSpring();
			mShouldMove |= shouldMove;
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta)) mScroll = 0f;
			mScroll += delta * scrollWheelFactor;
		}
	}

	/// <summary>
	/// Apply the dragging momentum.
	/// </summary>

	void LateUpdate ()
	{
		if (!Application.isPlaying) return;
		float delta = RealTime.deltaTime;

		// Fade the scroll bars if needed
		if (showScrollBars != ShowCondition.Always && (verticalScrollBar || horizontalScrollBar))
		{
			bool vertical = false;
			bool horizontal = false;

			if (showScrollBars != ShowCondition.WhenDragging || mDragID != -10 || mMomentum.magnitude > 0.01f)
			{
				vertical = shouldMoveVertically;
				horizontal = shouldMoveHorizontally;
			}

			if (verticalScrollBar)
			{
				float alpha = verticalScrollBar.alpha;
				alpha += vertical ? delta * 6f : -delta * 3f;
				alpha = Mathf.Clamp01(alpha);
				if (verticalScrollBar.alpha != alpha) verticalScrollBar.alpha = alpha;
			}

			if (horizontalScrollBar)
			{
				float alpha = horizontalScrollBar.alpha;
				alpha += horizontal ? delta * 6f : -delta * 3f;
				alpha = Mathf.Clamp01(alpha);
				if (horizontalScrollBar.alpha != alpha) horizontalScrollBar.alpha = alpha;
			}
		}

		if (!mShouldMove) return;

		// Apply momentum
		if (!mPressed)
		{
			if (mMomentum.magnitude > 0.0001f || Mathf.Abs(mScroll) > 0.0001f)
			{
				if (movement == Movement.Horizontal)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(mScroll * 0.05f, 0f, 0f));
				}
				else if (movement == Movement.Vertical)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(0f, mScroll * 0.05f, 0f));
				}
				else if (movement == Movement.Unrestricted)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(mScroll * 0.05f, mScroll * 0.05f, 0f));
				}
				else
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(
						mScroll * customMovement.x * 0.05f,
						mScroll * customMovement.y * 0.05f, 0f));
				}
				mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

				// Move the scroll view
				Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, dampenStrength, delta);
				MoveAbsolute(offset);

				// Restrict the contents to be within the scroll view's bounds
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
				{
					if (NGUITools.GetActive(centerOnChild))
					{
						if (centerOnChild.nextPageThreshold != 0f)
						{
							mMomentum = Vector3.zero;
							mScroll = 0f;
						}
						else centerOnChild.Recenter();
					}
					else
					{
						RestrictWithinBounds(false, canMoveHorizontally, canMoveVertically);
					}
				}

				if (onMomentumMove != null)
					onMomentumMove();
			}
			else
			{
				mScroll = 0f;
				mMomentum = Vector3.zero;

				SpringPanel sp = GetComponent<SpringPanel>();
				if (sp != null && sp.enabled) return;

				mShouldMove = false;
				if (onStoppedMoving != null)
					onStoppedMoving();
			}
		}
		else
		{
			// Dampen the momentum
			mScroll = 0f;
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
		}
	}

	/// <summary>
	/// Pan the scroll view.
	/// </summary>

	public void OnPan (Vector2 delta)
	{
		if (horizontalScrollBar != null) horizontalScrollBar.OnPan(delta);
		if (verticalScrollBar != null) verticalScrollBar.OnPan(delta);

		if (horizontalScrollBar == null && verticalScrollBar == null)
		{
			if (canMoveHorizontally) Scroll(delta.x);
			else if (canMoveVertically) Scroll(delta.y);
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// Draw a visible orange outline of the bounds.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (mPanel != null)
		{
			if (!Application.isPlaying) mCalculatedBounds = false;
			Bounds b = bounds;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0.4f, 0f);
			Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
		}
	}
#endif
}
