//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ever wanted to be able to auto-center on an object within a draggable panel?
/// Attach this script to the container that has the objects to center on as its children.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Child")]
public class UICenterOnChild : MonoBehaviour
{
	public delegate void OnCenterCallback (GameObject centeredObject);

	/// <summary>
	/// The strength of the spring.
	/// </summary>

	public float springStrength = 8f;

	/// <summary>
	/// If set to something above zero, it will be possible to move to the next page after dragging past the specified threshold.
	/// </summary>

	public float nextPageThreshold = 0f;

	/// <summary>
	/// Callback to be triggered when the centering operation completes.
	/// </summary>

	public SpringPanel.OnFinished onFinished;

	/// <summary>
	/// Callback triggered whenever the script begins centering on a new child object.
	/// </summary>

	public OnCenterCallback onCenter;

	UIScrollView mScrollView;
	GameObject mCenteredObject;

	/// <summary>
	/// Game object that the draggable panel is currently centered on.
	/// </summary>

	public GameObject centeredObject { get { return mCenteredObject; } }

	void Start () { Recenter(); }
	void OnEnable () { if (mScrollView) { mScrollView.centerOnChild = this; Recenter(); } }
	void OnDisable () { if (mScrollView) mScrollView.centerOnChild = null; }
	void OnDragFinished () { if (enabled) Recenter(); }

	/// <summary>
	/// Ensure that the threshold is always positive.
	/// </summary>

	void OnValidate () { nextPageThreshold = Mathf.Abs(nextPageThreshold); }

	/// <summary>
	/// Recenter the draggable list on the center-most child.
	/// </summary>

	[ContextMenu("Execute")]
	public void Recenter ()
	{
		if (mScrollView == null)
		{
			mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);

			if (mScrollView == null)
			{
				Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
				enabled = false;
				return;
			}
			else
			{
				if (mScrollView)
				{
					mScrollView.centerOnChild = this;
					mScrollView.onDragFinished += OnDragFinished;
				}

				if (mScrollView.horizontalScrollBar != null)
					mScrollView.horizontalScrollBar.onDragFinished += OnDragFinished;

				if (mScrollView.verticalScrollBar != null)
					mScrollView.verticalScrollBar.onDragFinished += OnDragFinished;
			}
		}
		if (mScrollView.panel == null) return;

		Transform trans = transform;
		if (trans.childCount == 0) return;

		// Calculate the panel's center in world coordinates
		Vector3[] corners = mScrollView.panel.worldCorners;
		Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;

		// Offset this value by the momentum
		Vector3 momentum = mScrollView.currentMomentum * mScrollView.momentumAmount;
		Vector3 moveDelta = NGUIMath.SpringDampen(ref momentum, 9f, 2f);
		Vector3 pickingPoint = panelCenter - moveDelta * 0.01f; // Magic number based on what "feels right"

		float min = float.MaxValue;
		Transform closest = null;
		int index = 0;
		int ignoredIndex = 0;

		UIGrid grid = GetComponent<UIGrid>();
		List<Transform> list = null;

		// Determine the closest child
		if (grid != null)
		{
			list = grid.GetChildList();

			for (int i = 0, imax = list.Count, ii = 0; i < imax; ++i)
			{
				Transform t = list[i];
				if (!t.gameObject.activeInHierarchy) continue;
				float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);

				if (sqrDist < min)
				{
					min = sqrDist;
					closest = t;
					index = i;
					ignoredIndex = ii;
				}
				++ii;
			}
		}
		else
		{
			for (int i = 0, imax = trans.childCount, ii = 0; i < imax; ++i)
			{
				Transform t = trans.GetChild(i);
				if (!t.gameObject.activeInHierarchy) continue;
				float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);

				if (sqrDist < min)
				{
					min = sqrDist;
					closest = t;
					index = i;
					ignoredIndex = ii;
				}
				++ii;
			}
		}

		// If we have a touch in progress and the next page threshold set
		if (nextPageThreshold > 0f && UICamera.currentTouch != null)
		{
			// If we're still on the same object
			if (mCenteredObject != null && mCenteredObject.transform == (list != null ? list[index] : trans.GetChild(index)))
			{
				Vector3 totalDelta = UICamera.currentTouch.totalDelta;
				totalDelta = transform.rotation * totalDelta;

				float delta = 0f;

				switch (mScrollView.movement)
				{
					case UIScrollView.Movement.Horizontal:
					{
						delta = totalDelta.x;
						break;
					}
					case UIScrollView.Movement.Vertical:
					{
						delta = totalDelta.y;
						break;
					}
					default:
					{
						delta = totalDelta.magnitude;
						break;
					}
				}

				if (Mathf.Abs(delta) > nextPageThreshold)
				{
					if (delta > nextPageThreshold)
					{
						// Next page
						if (list != null)
						{
							if (ignoredIndex > 0)
							{
								closest = list[ignoredIndex - 1];
							}
							else closest = (GetComponent<UIWrapContent>() == null) ? list[0] : list[list.Count - 1];
						}
						else if (ignoredIndex > 0)
						{
							closest = trans.GetChild(ignoredIndex - 1);
						}
						else closest = (GetComponent<UIWrapContent>() == null) ? trans.GetChild(0) : trans.GetChild(trans.childCount - 1);
					}
					else if (delta < -nextPageThreshold)
					{
						// Previous page
						if (list != null)
						{
							if (ignoredIndex < list.Count - 1)
							{
								closest = list[ignoredIndex + 1];
							}
							else closest = (GetComponent<UIWrapContent>() == null) ? list[list.Count - 1] : list[0];
						}
						else if (ignoredIndex < trans.childCount - 1)
						{
							closest = trans.GetChild(ignoredIndex + 1);
						}
						else closest = (GetComponent<UIWrapContent>() == null) ? trans.GetChild(trans.childCount - 1) : trans.GetChild(0);
					}
				}
			}
		}
		CenterOn(closest, panelCenter);
	}

	/// <summary>
	/// Center the panel on the specified target.
	/// </summary>

	void CenterOn (Transform target, Vector3 panelCenter)
	{
		if (target != null && mScrollView != null && mScrollView.panel != null)
		{
			Transform panelTrans = mScrollView.panel.cachedTransform;
			mCenteredObject = target.gameObject;

			// Figure out the difference between the chosen child and the panel's center in local coordinates
			Vector3 cp = panelTrans.InverseTransformPoint(target.position);
			Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
			Vector3 localOffset = cp - cc;

			// Offset shouldn't occur if blocked
			if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
			if (!mScrollView.canMoveVertically) localOffset.y = 0f;
			localOffset.z = 0f;

			// Spring the panel to this calculated position
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				panelTrans.localPosition = panelTrans.localPosition - localOffset;

				Vector4 co = mScrollView.panel.clipOffset;
				co.x += localOffset.x;
				co.y += localOffset.y;
				mScrollView.panel.clipOffset = co;
			}
			else
#endif
			{
				SpringPanel.Begin(mScrollView.panel.cachedGameObject,
					panelTrans.localPosition - localOffset, springStrength).onFinished = onFinished;
			}
		}
		else mCenteredObject = null;

		// Notify the listener
		if (onCenter != null) onCenter(mCenteredObject);
	}

	/// <summary>
	/// Center the panel on the specified target.
	/// </summary>

	public void CenterOn (Transform target)
	{
		if (mScrollView != null && mScrollView.panel != null)
		{
			Vector3[] corners = mScrollView.panel.worldCorners;
			Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
			CenterOn(target, panelCenter);
		}
	}
}
