using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Example script that can be used to show tooltips.
/// </summary>

[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	static protected UITooltip mInstance;

	public Camera uiCamera;
	public UILabel text;
	public GameObject tooltipRoot;
	public UISprite background;
	public float appearSpeed = 10f;
	public bool scalingTransitions = true;

	protected GameObject mTooltip;
	protected Transform mTrans;
	protected float mTarget = 0f;
	protected float mCurrent = 0f;
	protected Vector3 mPos;
	protected Vector3 mSize = Vector3.zero;

	protected UIWidget[] mWidgets;

	/// <summary>
	/// Whether the tooltip is currently visible.
	/// </summary>

	static public bool isVisible { get { return (mInstance != null && mInstance.mTarget == 1f); } }

	void Awake () { mInstance = this; }
	void OnDestroy () { mInstance = null; }

	/// <summary>
	/// Get a list of widgets underneath the tooltip.
	/// </summary>

	protected virtual void Start ()
	{
		mTrans = transform;
		mWidgets = GetComponentsInChildren<UIWidget>();
		mPos = mTrans.localPosition;
		if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		SetAlpha(0f);
	}

	/// <summary>
	/// Update the tooltip's alpha based on the target value.
	/// </summary>

	protected virtual void Update ()
	{
		if (mTooltip != UICamera.tooltipObject)
		{
			mTooltip = null;
			mTarget = 0f;
		}

		if (mCurrent != mTarget)
		{
			mCurrent = Mathf.Lerp(mCurrent, mTarget, RealTime.deltaTime * appearSpeed);
			if (Mathf.Abs(mCurrent - mTarget) < 0.001f) mCurrent = mTarget;
			SetAlpha(mCurrent * mCurrent);

			if (scalingTransitions)
			{
				Vector3 offset = mSize * 0.25f;
				offset.y = -offset.y;

				Vector3 size = Vector3.one * (1.5f - mCurrent * 0.5f);
				Vector3 pos = Vector3.Lerp(mPos - offset, mPos, mCurrent);

				mTrans.localPosition = pos;
				mTrans.localScale = size;
			}
		}
	}

	/// <summary>
	/// Set the alpha of all widgets.
	/// </summary>

	protected virtual void SetAlpha (float val)
	{
		for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
		{
			UIWidget w = mWidgets[i];
			Color c = w.color;
			c.a = val;
			w.color = c;
		}
	}

	/// <summary>
	/// Set the tooltip's text to the specified string.
	/// </summary>

	protected virtual void SetText (string tooltipText)
	{
		if (text != null && !string.IsNullOrEmpty(tooltipText))
		{
			mTarget = 1f;
			mTooltip = UICamera.tooltipObject;
			text.text = tooltipText;

			// Orthographic camera positioning is trivial
			mPos = UICamera.lastEventPosition;

			Transform textTrans = text.transform;
			Vector3 offset = textTrans.localPosition;
			Vector3 textScale = textTrans.localScale;

			// Calculate the dimensions of the printed text
			mSize = text.printedSize;

			// Scale by the transform and adjust by the padding offset
			mSize.x *= textScale.x;
			mSize.y *= textScale.y;

			if (background != null)
			{
				Vector4 border = background.border;
				mSize.x += border.x + border.z + (offset.x - border.x) * 2f;
				mSize.y += border.y + border.w + (-offset.y - border.y) * 2f;

				background.width = Mathf.RoundToInt(mSize.x);
				background.height = Mathf.RoundToInt(mSize.y);
			}

			if (uiCamera != null)
			{
				// Since the screen can be of different than expected size, we want to convert
				// mouse coordinates to view space, then convert that to world position.
				mPos.x = Mathf.Clamp01(mPos.x / Screen.width);
				mPos.y = Mathf.Clamp01(mPos.y / Screen.height);

				// Calculate the ratio of the camera's target orthographic size to current screen size
				float activeSize = uiCamera.orthographicSize / mTrans.parent.lossyScale.y;
				float ratio = (Screen.height * 0.5f) / activeSize;

				// Calculate the maximum on-screen size of the tooltip window
				Vector2 max = new Vector2(ratio * mSize.x / Screen.width, ratio * mSize.y / Screen.height);

				// Limit the tooltip to always be visible
				mPos.x = Mathf.Min(mPos.x, 1f - max.x);
				mPos.y = Mathf.Max(mPos.y, max.y);

				// Update the absolute position and save the local one
				mTrans.position = uiCamera.ViewportToWorldPoint(mPos);
				mPos = mTrans.localPosition;
				mPos.x = Mathf.Round(mPos.x);
				mPos.y = Mathf.Round(mPos.y);
			}
			else
			{
				// Don't let the tooltip leave the screen area
				if (mPos.x + mSize.x > Screen.width) mPos.x = Screen.width - mSize.x;
				if (mPos.y - mSize.y < 0f) mPos.y = mSize.y;

				// Simple calculation that assumes that the camera is of fixed size
				mPos.x -= Screen.width * 0.5f;
				mPos.y -= Screen.height * 0.5f;
			}

			mTrans.localPosition = mPos;

			// Force-update all anchors below the tooltip
			if (tooltipRoot != null) tooltipRoot.BroadcastMessage("UpdateAnchors");
			else text.BroadcastMessage("UpdateAnchors");
		}
		else
		{
			mTooltip = null;
			mTarget = 0f;
		}
	}

	/// <summary>
	/// Show a tooltip with the specified text.
	/// </summary>

	[System.Obsolete("Use UITooltip.Show instead")]
	static public void ShowText (string text) { if (mInstance != null) mInstance.SetText(text); }

	/// <summary>
	/// Show the tooltip.
	/// </summary>

	static public void Show (string text) { if (mInstance != null) mInstance.SetText(text); }
	
	/// <summary>
	/// Hide the tooltip.
	/// </summary>

	static public void Hide () { if (mInstance != null) { mInstance.mTooltip = null; mInstance.mTarget = 0f; } }
}
