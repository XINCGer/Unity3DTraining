//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This script can be used to anchor an object to the side or corner of the screen, panel, or a widget.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Anchor")]
public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center,
	}

	/// <summary>
	/// Camera used to determine the anchor bounds. Set automatically if none was specified.
	/// </summary>

	public Camera uiCamera = null;

	/// <summary>
	/// Object used to determine the container's bounds. Overwrites the camera-based anchoring if the value was specified.
	/// </summary>

	public GameObject container = null;

	/// <summary>
	/// Side or corner to anchor to.
	/// </summary>

	public Side side = Side.Center;

	/// <summary>
	/// If set to 'true', UIAnchor will execute once, then will be disabled.
	/// Screen size changes will still cause the anchor to update itself, even if it's disabled.
	/// </summary>

	public bool runOnlyOnce = true;

	/// <summary>
	/// Relative offset value, if any. For example "0.25" with 'side' set to Left, means 25% from the left side.
	/// </summary>

	public Vector2 relativeOffset = Vector2.zero;
	
	/// <summary>
	/// Pixel offset value if any. For example "10" in x will move the widget 10 pixels to the right 
	/// while "-10" in x is 10 pixels to the left based on the pixel values set in UIRoot.
	/// </summary>
	
	public Vector2 pixelOffset = Vector2.zero;

	// Deprecated legacy functionality
	[HideInInspector][SerializeField] UIWidget widgetContainer;

	Transform mTrans;
	Animation mAnim;
	Rect mRect = new Rect();
	UIRoot mRoot;
	bool mStarted = false;

	void OnEnable ()
	{
		mTrans = transform;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		mAnim = animation;
#else
		mAnim = GetComponent<Animation>();
#endif
		UICamera.onScreenResize += ScreenSizeChanged;
	}

	void OnDisable () { UICamera.onScreenResize -= ScreenSizeChanged; }

	void ScreenSizeChanged () { if (mStarted && runOnlyOnce) Update(); }

	/// <summary>
	/// Automatically find the camera responsible for drawing the widgets under this object.
	/// </summary>

	void Start ()
	{
		if (container == null && widgetContainer != null)
		{
			container = widgetContainer.gameObject;
			widgetContainer = null;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

		mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
		if (uiCamera == null) uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		
		Update();

		mStarted = true;
	}

	/// <summary>
	/// Anchor the object to the appropriate point.
	/// </summary>

	void Update ()
	{
		if (mAnim != null && mAnim.enabled && mAnim.isPlaying) return;
		if (mTrans == null) return;

		bool useCamera = false;

		UIWidget wc = (container == null) ? null : container.GetComponent<UIWidget>();
		UIPanel pc = (container == null && wc == null) ? null : container.GetComponent<UIPanel>();

		if (wc != null)
		{
			Bounds b = wc.CalculateBounds(container.transform.parent);

			mRect.x = b.min.x;
			mRect.y = b.min.y;

			mRect.width = b.size.x;
			mRect.height = b.size.y;
		}
		else if (pc != null)
		{
			if (pc.clipping == UIDrawCall.Clipping.None)
			{
				// Panel has no clipping -- just use the screen's dimensions
				float ratio = (mRoot != null) ? (float)mRoot.activeHeight / Screen.height * 0.5f : 0.5f;
				mRect.xMin = -Screen.width * ratio;
				mRect.yMin = -Screen.height * ratio;
				mRect.xMax = -mRect.xMin;
				mRect.yMax = -mRect.yMin;
			}
			else
			{
				// Panel has clipping -- use it as the mRect
				Vector4 pos = pc.finalClipRegion;
				mRect.x = pos.x - (pos.z * 0.5f);
				mRect.y = pos.y - (pos.w * 0.5f);
				mRect.width = pos.z;
				mRect.height = pos.w;
			}
		}
		else if (container != null)
		{
			Transform root = container.transform.parent;
			Bounds b = (root != null) ? NGUIMath.CalculateRelativeWidgetBounds(root, container.transform) :
				NGUIMath.CalculateRelativeWidgetBounds(container.transform);

			mRect.x = b.min.x;
			mRect.y = b.min.y;

			mRect.width = b.size.x;
			mRect.height = b.size.y;
		}
		else if (uiCamera != null)
		{
			useCamera = true;
			mRect = uiCamera.pixelRect;
		}
		else return;

		float cx = (mRect.xMin + mRect.xMax) * 0.5f;
		float cy = (mRect.yMin + mRect.yMax) * 0.5f;
		Vector3 v = new Vector3(cx, cy, 0f);

		if (side != Side.Center)
		{
			if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight) v.x = mRect.xMax;
			else if (side == Side.Top || side == Side.Center || side == Side.Bottom) v.x = cx;
			else v.x = mRect.xMin;

			if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft) v.y = mRect.yMax;
			else if (side == Side.Left || side == Side.Center || side == Side.Right) v.y = cy;
			else v.y = mRect.yMin;
		}

		float width = mRect.width;
		float height = mRect.height;

		v.x += pixelOffset.x + relativeOffset.x * width;
		v.y += pixelOffset.y + relativeOffset.y * height;

		if (useCamera)
		{
			if (uiCamera.orthographic)
			{
				v.x = Mathf.Round(v.x);
				v.y = Mathf.Round(v.y);
			}

			v.z = uiCamera.WorldToScreenPoint(mTrans.position).z;
			v = uiCamera.ScreenToWorldPoint(v);
		}
		else
		{
			v.x = Mathf.Round(v.x);
			v.y = Mathf.Round(v.y);

			if (pc != null)
			{
				v = pc.cachedTransform.TransformPoint(v);
			}
			else if (container != null)
			{
				Transform t = container.transform.parent;
				if (t != null) v = t.TransformPoint(v);
			}
			v.z = mTrans.position.z;
		}

		// Wrapped in an 'if' so the scene doesn't get marked as 'edited' every frame
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		if (useCamera && uiCamera.isOrthoGraphic && mTrans.parent != null)
#else
		if (useCamera && uiCamera.orthographic && mTrans.parent != null)
#endif
		{
			v = mTrans.parent.InverseTransformPoint(v);
			v.x = Mathf.RoundToInt(v.x);
			v.y = Mathf.RoundToInt(v.y);
			if (mTrans.localPosition != v) mTrans.localPosition = v;
		}
		else if (mTrans.position != v) mTrans.position = v;
		if (runOnlyOnce && Application.isPlaying) enabled = false;
	}
}
