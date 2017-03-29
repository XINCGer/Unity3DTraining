//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows dragging of the specified target object by mouse or touch, optionally limiting it to be within the UIPanel's clipped rectangle.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : MonoBehaviour
{
	public enum DragEffect
	{
		None,
		Momentum,
		MomentumAndSpring,
	}

	/// <summary>
	/// Target object that will be dragged.
	/// </summary>

	public Transform target;

	/// <summary>
	/// Panel that will be used for constraining the target.
	/// </summary>

	public UIPanel panelRegion;

	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>

	public Vector3 dragMovement { get { return scale; } set { scale = value; } }

	/// <summary>
	/// Momentum added from the mouse scroll wheel.
	/// </summary>

	public Vector3 scrollMomentum = Vector3.zero;

	/// <summary>
	/// Whether the dragging will be restricted to be within the parent panel's bounds.
	/// </summary>

	public bool restrictWithinPanel = false;

	/// <summary>
	/// Rectangle to be used as the draggable object's bounds. If none specified, all widgets' bounds get added up.
	/// </summary>

	public UIRect contentRect = null;

	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>

	public float momentumAmount = 35f;

	// Obsolete property. Use 'dragMovement' instead.
	[SerializeField] protected Vector3 scale = new Vector3(1f, 1f, 0f);

	// Obsolete property. Use 'scrollMomentum' instead.
	[SerializeField][HideInInspector] float scrollWheelFactor = 0f;

	Plane mPlane;
	Vector3 mTargetPos;
	Vector3 mLastPos;
	Vector3 mMomentum = Vector3.zero;
	Vector3 mScroll = Vector3.zero;
	Bounds mBounds;
	int mTouchID = 0;
	bool mStarted = false;
	bool mPressed = false;

	/// <summary>
	/// Auto-upgrade the legacy data.
	/// </summary>

	void OnEnable ()
	{
		if (scrollWheelFactor != 0f)
		{
			scrollMomentum = scale * scrollWheelFactor;
			scrollWheelFactor = 0f;
		}

		if (contentRect == null && target != null && Application.isPlaying)
		{
			UIWidget w = target.GetComponent<UIWidget>();
			if (w != null) contentRect = w;
		}

		mTargetPos = (target != null) ? target.position : Vector3.zero;
	}

	void OnDisable () { mStarted = false; }

	/// <summary>
	/// Find the panel responsible for this object.
	/// </summary>

	void FindPanel ()
	{
		panelRegion = (target != null) ? UIPanel.Find(target.transform.parent) : null;
		if (panelRegion == null) restrictWithinPanel = false;
	}

	/// <summary>
	/// Recalculate the bounds of the dragged content.
	/// </summary>

	void UpdateBounds ()
	{
		if (contentRect)
		{
			Transform t = panelRegion.cachedTransform;
			Matrix4x4 toLocal = t.worldToLocalMatrix;
			Vector3[] corners = contentRect.worldCorners;
			for (int i = 0; i < 4; ++i) corners[i] = toLocal.MultiplyPoint3x4(corners[i]);
			mBounds = new Bounds(corners[0], Vector3.zero);
			for (int i = 1; i < 4; ++i) mBounds.Encapsulate(corners[i]);
		}
		else
		{
			mBounds = NGUIMath.CalculateRelativeWidgetBounds(panelRegion.cachedTransform, target);
		}
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	void OnPress (bool pressed)
	{
		if (UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3) return;

		// Unity's physics seems to break when timescale is not quite zero. Raycasts start to fail completely.
		float ts = Time.timeScale;
		if (ts < 0.01f && ts != 0f) return;

		if (enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			if (pressed)
			{
				if (!mPressed)
				{
					// Remove all momentum on press
					mTouchID = UICamera.currentTouchID;
					mPressed = true;
					mStarted = false;
					CancelMovement();

					if (restrictWithinPanel && panelRegion == null) FindPanel();
					if (restrictWithinPanel) UpdateBounds();

					// Disable the spring movement
					CancelSpring();

					// Create the plane to drag along
					Transform trans = UICamera.currentCamera.transform;
					mPlane = new Plane((panelRegion != null ? panelRegion.cachedTransform.rotation : trans.rotation) * Vector3.back, UICamera.lastWorldPosition);
				}
			}
			else if (mPressed && mTouchID == UICamera.currentTouchID)
			{
				mPressed = false;

				if (restrictWithinPanel && dragEffect == DragEffect.MomentumAndSpring)
				{
					if (panelRegion.ConstrainTargetToBounds(target, ref mBounds, false))
						CancelMovement();
				}
			}
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (mPressed && mTouchID == UICamera.currentTouchID && enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float dist = 0f;

			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;

				if (!mStarted)
				{
					mStarted = true;
					offset = Vector3.zero;
				}

				if (offset.x != 0f || offset.y != 0f)
				{
					offset = target.InverseTransformDirection(offset);
					offset.Scale(scale);
					offset = target.TransformDirection(offset);
				}

				// Adjust the momentum
				if (dragEffect != DragEffect.None)
					mMomentum = Vector3.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);

				// Adjust the position and bounds
				Vector3 before = target.localPosition;
				Move(offset);

				// We want to constrain the UI to be within bounds
				if (restrictWithinPanel)
				{
					mBounds.center = mBounds.center + (target.localPosition - before);

					// Constrain the UI to the bounds, and if done so, immediately eliminate the momentum
					if (dragEffect != DragEffect.MomentumAndSpring && panelRegion.ConstrainTargetToBounds(target, ref mBounds, true))
						CancelMovement();
				}
			}
		}
	}

	/// <summary>
	/// Move the dragged object by the specified amount.
	/// </summary>

	void Move (Vector3 worldDelta)
	{
		if (panelRegion != null)
		{
			mTargetPos += worldDelta;
			Transform parent = target.parent;
			Rigidbody rb = target.GetComponent<Rigidbody>();

			if (parent != null)
			{
				Vector3 after = parent.worldToLocalMatrix.MultiplyPoint3x4(mTargetPos);
				after.x = Mathf.Round(after.x);
				after.y = Mathf.Round(after.y);

				if (rb != null)
				{
					// With a lot of colliders under the rigidbody, moving the transform causes some crazy overhead.
					// Moving the rigidbody is much cheaper, but it does seem to have a side effect of causing
					// widgets to detect movement relative to the panel, when in fact they should not be moving.
					// This is why it's best to keep the panel as 'static' if at all possible.
					after = parent.localToWorldMatrix.MultiplyPoint3x4(after);
					rb.position = after;
				}
				else target.localPosition = after;
			}
			else if (rb != null)
			{
				rb.position = mTargetPos;
			}
			else target.position = mTargetPos;

			UIScrollView ds = panelRegion.GetComponent<UIScrollView>();
			if (ds != null) ds.UpdateScrollbars(true);
		}
		else target.position += worldDelta;
	}

	/// <summary>
	/// Apply the dragging momentum.
	/// </summary>

	void LateUpdate ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (target == null) return;
		float delta = RealTime.deltaTime;

		mMomentum -= mScroll;
		mScroll = NGUIMath.SpringLerp(mScroll, Vector3.zero, 20f, delta);

		// No momentum? Exit.
		if (mMomentum.magnitude < 0.0001f) return;

		if (!mPressed)
		{
			// Apply the momentum
			if (panelRegion == null) FindPanel();

			Move(NGUIMath.SpringDampen(ref mMomentum, 9f, delta));

			if (restrictWithinPanel && panelRegion != null)
			{
				UpdateBounds();

				if (panelRegion.ConstrainTargetToBounds(target, ref mBounds, dragEffect == DragEffect.None))
				{
					CancelMovement();
				}
				else CancelSpring();
			}

			// Dampen the momentum
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);

			// Cancel all movement (and snap to pixels) at the end
			if (mMomentum.magnitude < 0.0001f) CancelMovement();
		}
		else NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
	}

	/// <summary>
	/// Cancel all movement.
	/// </summary>

	public void CancelMovement ()
	{
		if (target != null)
		{
			Vector3 pos = target.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			target.localPosition = pos;
		}
		mTargetPos = (target != null) ? target.position : Vector3.zero;
		mMomentum = Vector3.zero;
		mScroll = Vector3.zero;
	}

	/// <summary>
	/// Cancel the spring movement.
	/// </summary>

	public void CancelSpring ()
	{
		SpringPosition sp = target.GetComponent<SpringPosition>();
		if (sp != null) sp.enabled = false;
	}

	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	void OnScroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject))
			mScroll -= scrollMomentum * (delta * 0.05f);
	}
}
