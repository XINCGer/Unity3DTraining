//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for all UI components that should be derived from when creating new widget types.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Widget")]
public class UIWidget : UIRect
{
	public enum Pivot
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Center,
		Right,
		BottomLeft,
		Bottom,
		BottomRight,
	}

	// Cached and saved values
	[HideInInspector][SerializeField] protected Color mColor = Color.white;
	[HideInInspector][SerializeField] protected Pivot mPivot = Pivot.Center;
	[HideInInspector][SerializeField] protected int mWidth = 100;
	[HideInInspector][SerializeField] protected int mHeight = 100;
	[HideInInspector][SerializeField] protected int mDepth = 0;

	[Tooltip("Custom material, if desired")]
	[HideInInspector][SerializeField] protected Material mMat;

	/// <summary>
	/// Notification triggered when the widget's dimensions or position changes.
	/// </summary>

	public OnDimensionsChanged onChange;
	public delegate void OnDimensionsChanged ();

	/// <summary>
	/// Notification triggered after the widget's buffer has been filled.
	/// </summary>

	public OnPostFillCallback onPostFill;
	public delegate void OnPostFillCallback (UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols);

	/// <summary>
	/// Callback triggered when the widget is about to be renderered (OnWillRenderObject).
	/// NOTE: This property is only exposed for the sake of speed to avoid property execution.
	/// In most cases you will want to use UIWidget.onRender instead.
	/// </summary>

	public UIDrawCall.OnRenderCallback mOnRender;

	/// <summary>
	/// Set the callback that will be triggered when the widget is being rendered (OnWillRenderObject).
	/// This is where you would set material properties and shader values.
	/// </summary>

	public UIDrawCall.OnRenderCallback onRender
	{
		get
		{
			return mOnRender;
		}
		set
		{
#if UNITY_FLASH
			if (!(mOnRender == value))
#else
			if (mOnRender != value)
#endif
			{
#if !UNITY_FLASH
				if (drawCall != null && drawCall.onRender != null && mOnRender != null)
					drawCall.onRender -= mOnRender;
#endif
				mOnRender = value;
				if (drawCall != null) drawCall.onRender += value;
			}
		}
	}

	/// <summary>
	/// If set to 'true', the box collider's dimensions will be adjusted to always match the widget whenever it resizes.
	/// </summary>

	public bool autoResizeBoxCollider = false;

	/// <summary>
	/// Hide the widget if it happens to be off-screen.
	/// </summary>

	public bool hideIfOffScreen = false;

	public enum AspectRatioSource
	{
		Free,
		BasedOnWidth,
		BasedOnHeight,
	}

	/// <summary>
	/// Whether the rectangle will attempt to maintain a specific aspect ratio.
	/// </summary>

	public AspectRatioSource keepAspectRatio = AspectRatioSource.Free;

	/// <summary>
	/// If you want the anchored rectangle to keep a specific aspect ratio, set this value.
	/// </summary>

	public float aspectRatio = 1f;

	public delegate bool HitCheck (Vector3 worldPos);

	/// <summary>
	/// Custom hit check function. If set, all hit checks (including events) will call this function,
	/// passing the world position. Return 'true' if it's within the bounds of your choice, 'false' otherwise.
	/// </summary>

	public HitCheck hitCheck;

	/// <summary>
	/// Panel that's managing this widget.
	/// </summary>

	[System.NonSerialized] public UIPanel panel;

	/// <summary>
	/// Widget's generated geometry.
	/// </summary>

	[System.NonSerialized] public UIGeometry geometry = new UIGeometry();

	/// <summary>
	/// If set to 'false', the widget's OnFill function will not be called, letting you define custom geometry at will.
	/// </summary>

	[System.NonSerialized] public bool fillGeometry = true;
	[System.NonSerialized] protected bool mPlayMode = true;
	[System.NonSerialized] protected Vector4 mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
	[System.NonSerialized] Matrix4x4 mLocalToPanel;
	[System.NonSerialized] bool mIsVisibleByAlpha = true;
	[System.NonSerialized] bool mIsVisibleByPanel = true;
	[System.NonSerialized] bool mIsInFront = true;
	[System.NonSerialized] float mLastAlpha = 0f;
	[System.NonSerialized] bool mMoved = false;

	/// <summary>
	/// Internal usage -- draw call that's drawing the widget.
	/// </summary>

	[System.NonSerialized] public UIDrawCall drawCall;
	[System.NonSerialized] protected Vector3[] mCorners = new Vector3[4];

	/// <summary>
	/// Draw region alters how the widget looks without modifying the widget's rectangle.
	/// A region is made up of 4 relative values (0-1 range). The order is Left (X), Bottom (Y), Right (Z) and Top (W).
	/// To have a widget's left edge be 30% from the left side, set X to 0.3. To have the widget's right edge be 30%
	/// from the right hand side, set Z to 0.7.
	/// </summary>

	public Vector4 drawRegion
	{
		get
		{
			return mDrawRegion;
		}
		set
		{
			if (mDrawRegion != value)
			{
				mDrawRegion = value;
				if (autoResizeBoxCollider) ResizeCollider();
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Pivot offset in relative coordinates. Bottom-left is (0, 0). Top-right is (1, 1).
	/// </summary>

	public Vector2 pivotOffset { get { return NGUIMath.GetPivotOffset(pivot); } }

	/// <summary>
	/// Widget's width in pixels.
	/// </summary>

	public int width
	{
		get
		{
			return mWidth;
		}
		set
		{
			int min = minWidth;
			if (value < min) value = min;

			if (mWidth != value && keepAspectRatio != AspectRatioSource.BasedOnHeight)
			{
				if (isAnchoredHorizontally)
				{
					if (leftAnchor.target != null && rightAnchor.target != null)
					{
						if (mPivot == Pivot.BottomLeft || mPivot == Pivot.Left || mPivot == Pivot.TopLeft)
						{
							NGUIMath.AdjustWidget(this, 0f, 0f, value - mWidth, 0f);
						}
						else if (mPivot == Pivot.BottomRight || mPivot == Pivot.Right || mPivot == Pivot.TopRight)
						{
							NGUIMath.AdjustWidget(this, mWidth - value, 0f, 0f, 0f);
						}
						else
						{
							int diff = value - mWidth;
							diff = diff - (diff & 1);
							if (diff != 0) NGUIMath.AdjustWidget(this, -diff * 0.5f, 0f, diff * 0.5f, 0f);
						}
					}
					else if (leftAnchor.target != null)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, value - mWidth, 0f);
					}
					else NGUIMath.AdjustWidget(this, mWidth - value, 0f, 0f, 0f);
				}
				else SetDimensions(value, mHeight);
			}
		}
	}

	/// <summary>
	/// Widget's height in pixels.
	/// </summary>

	public int height
	{
		get
		{
			return mHeight;
		}
		set
		{
			int min = minHeight;
			if (value < min) value = min;

			if (mHeight != value && keepAspectRatio != AspectRatioSource.BasedOnWidth)
			{
				if (isAnchoredVertically)
				{
					if (bottomAnchor.target != null && topAnchor.target != null)
					{
						if (mPivot == Pivot.BottomLeft || mPivot == Pivot.Bottom || mPivot == Pivot.BottomRight)
						{
							NGUIMath.AdjustWidget(this, 0f, 0f, 0f, value - mHeight);
						}
						else if (mPivot == Pivot.TopLeft || mPivot == Pivot.Top || mPivot == Pivot.TopRight)
						{
							NGUIMath.AdjustWidget(this, 0f, mHeight - value, 0f, 0f);
						}
						else
						{
							int diff = value - mHeight;
							diff = diff - (diff & 1);
							if (diff != 0) NGUIMath.AdjustWidget(this, 0f, -diff * 0.5f, 0f, diff * 0.5f);
						}
					}
					else if (bottomAnchor.target != null)
					{
						NGUIMath.AdjustWidget(this, 0f, 0f, 0f, value - mHeight);
					}
					else NGUIMath.AdjustWidget(this, 0f, mHeight - value, 0f, 0f);
				}
				else SetDimensions(mWidth, value);
			}
		}
	}

	/// <summary>
	/// Color used by the widget.
	/// </summary>

	public Color color
	{
		get
		{
			return mColor;
		}
		set
		{
			if (mColor != value)
			{
				bool alphaChange = (mColor.a != value.a);
				mColor = value;
				Invalidate(alphaChange);
			}
		}
	}

	/// <summary>
	/// Widget's alpha -- a convenience method.
	/// </summary>

	public override float alpha
	{
		get
		{
			return mColor.a;
		}
		set
		{
			if (mColor.a != value)
			{
				mColor.a = value;
				Invalidate(true);
			}
		}
	}

	/// <summary>
	/// Whether the widget is currently visible.
	/// </summary>

	public bool isVisible { get { return mIsVisibleByPanel && mIsVisibleByAlpha && mIsInFront && finalAlpha > 0.001f && NGUITools.GetActive(this); } }

	/// <summary>
	/// Whether the widget has vertices to draw.
	/// </summary>

	public bool hasVertices { get { return geometry != null && geometry.hasVertices; } }

	/// <summary>
	/// Change the pivot point and do not attempt to keep the widget in the same place by adjusting its transform.
	/// </summary>

	public Pivot rawPivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				mPivot = value;
				if (autoResizeBoxCollider) ResizeCollider();
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Set or get the value that specifies where the widget's pivot point should be.
	/// </summary>

	public Pivot pivot
	{
		get
		{
			return mPivot;
		}
		set
		{
			if (mPivot != value)
			{
				Vector3 before = worldCorners[0];

				mPivot = value;
				mChanged = true;

				Vector3 after = worldCorners[0];

				Transform t = cachedTransform;
				Vector3 pos = t.position;
				float z = t.localPosition.z;
				pos.x += (before.x - after.x);
				pos.y += (before.y - after.y);
				cachedTransform.position = pos;

				pos = cachedTransform.localPosition;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				pos.z = z;
				cachedTransform.localPosition = pos;
			}
		}
	}

	/// <summary>
	/// Depth controls the rendering order -- lowest to highest.
	/// </summary>

	public int depth
	{
		get
		{
			// Experiment with a transform-based depth, uGUI style
			//if (mDepth == int.MinValue)
			//{
			//    int val = cachedTransform.GetSiblingIndex();
			//    UIWidget pt = parent as UIWidget;
			//    if (pt != null) val += pt.depth;
			//    return val;
			//}
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				if (panel != null) panel.RemoveWidget(this);
				mDepth = value;
				
				if (panel != null)
				{
					panel.AddWidget(this);

					if (!Application.isPlaying)
					{
						panel.SortWidgets();
						panel.RebuildAllDrawCalls();
					}
				}
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
			}
		}
	}

	/// <summary>
	/// Raycast depth order on widgets takes the depth of their panel into consideration.
	/// This functionality is used to determine the "final" depth of the widget for drawing and raycasts.
	/// </summary>

	public int raycastDepth
	{
		get
		{
			if (panel == null) CreatePanel();
			return (panel != null) ? mDepth + panel.depth * 1000 : mDepth;
		}
	}

	/// <summary>
	/// Local space corners of the widget. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] localCorners
	{
		get
		{
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			mCorners[0] = new Vector3(x0, y0);
			mCorners[1] = new Vector3(x0, y1);
			mCorners[2] = new Vector3(x1, y1);
			mCorners[3] = new Vector3(x1, y0);

			return mCorners;
		}
	}

	/// <summary>
	/// Local width and height of the widget in pixels.
	/// </summary>

	public virtual Vector2 localSize
	{
		get
		{
			Vector3[] cr = localCorners;
			return cr[2] - cr[0];
		}
	}

	/// <summary>
	/// Widget's center in local coordinates. Don't forget to transform by the widget's transform.
	/// </summary>

	public Vector3 localCenter
	{
		get
		{
			Vector3[] cr = localCorners;
			return Vector3.Lerp(cr[0], cr[2], 0.5f);
		}
	}

	/// <summary>
	/// World-space corners of the widget. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] worldCorners
	{
		get
		{
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			Transform wt = cachedTransform;

			mCorners[0] = wt.TransformPoint(x0, y0, 0f);
			mCorners[1] = wt.TransformPoint(x0, y1, 0f);
			mCorners[2] = wt.TransformPoint(x1, y1, 0f);
			mCorners[3] = wt.TransformPoint(x1, y0, 0f);

			return mCorners;
		}
	}

	/// <summary>
	/// World-space center of the widget.
	/// </summary>

	public Vector3 worldCenter { get { return cachedTransform.TransformPoint(localCenter); } }

	/// <summary>
	/// Local space region where the actual drawing will take place.
	/// X = left, Y = bottom, Z = right, W = top.
	/// </summary>

	public virtual Vector4 drawingDimensions
	{
		get
		{
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			return new Vector4(
				mDrawRegion.x == 0f ? x0 : Mathf.Lerp(x0, x1, mDrawRegion.x),
				mDrawRegion.y == 0f ? y0 : Mathf.Lerp(y0, y1, mDrawRegion.y),
				mDrawRegion.z == 1f ? x1 : Mathf.Lerp(x0, x1, mDrawRegion.z),
				mDrawRegion.w == 1f ? y1 : Mathf.Lerp(y0, y1, mDrawRegion.w));
		}
	}

	/// <summary>
	/// Custom material associated with the widget, if any.
	/// </summary>

	public virtual Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mMat = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Texture used by the widget.
	/// </summary>

	public virtual Texture mainTexture
	{
		get
		{
			Material mat = material;
			return (mat != null) ? mat.mainTexture : null;
		}
		set
		{
			throw new System.NotImplementedException(GetType() + " has no mainTexture setter");
		}
	}

	/// <summary>
	/// Shader is used to create a dynamic material if the widget has no material to work with.
	/// </summary>

	public virtual Shader shader
	{
		get
		{
			Material mat = material;
			return (mat != null) ? mat.shader : null;
		}
		set
		{
			throw new System.NotImplementedException(GetType() + " has no shader setter");
		}
	}

	/// <summary>
	/// Do not use this, it's obsolete.
	/// </summary>

	[System.Obsolete("There is no relative scale anymore. Widgets now have width and height instead")]
	public Vector2 relativeSize { get { return Vector2.one; } }

	/// <summary>
	/// Convenience function that returns 'true' if the widget has a box collider.
	/// </summary>

	public bool hasBoxCollider
	{
		get
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			BoxCollider box = collider as BoxCollider;
#else
			BoxCollider box = GetComponent<Collider>() as BoxCollider;
#endif
			if (box != null) return true;
			return GetComponent<BoxCollider2D>() != null;
		}
	}

	/// <summary>
	/// Adjust the widget's dimensions without going through the anchor validation logic.
	/// </summary>

	public void SetDimensions (int w, int h)
	{
		if (mWidth != w || mHeight != h)
		{
			mWidth = w;
			mHeight = h;

			if (keepAspectRatio == AspectRatioSource.BasedOnWidth)
				mHeight = Mathf.RoundToInt(mWidth / aspectRatio);
			else if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
				mWidth = Mathf.RoundToInt(mHeight * aspectRatio);
			else if (keepAspectRatio == AspectRatioSource.Free)
				aspectRatio = mWidth / (float)mHeight;

			mMoved = true;
			if (autoResizeBoxCollider) ResizeCollider();
			MarkAsChanged();
		}
	}

	/// <summary>
	/// Get the sides of the rectangle relative to the specified transform.
	/// The order is left, top, right, bottom.
	/// </summary>

	public override Vector3[] GetSides (Transform relativeTo)
	{
		Vector2 offset = pivotOffset;

		float x0 = -offset.x * mWidth;
		float y0 = -offset.y * mHeight;
		float x1 = x0 + mWidth;
		float y1 = y0 + mHeight;
		float cx = (x0 + x1) * 0.5f;
		float cy = (y0 + y1) * 0.5f;

		Transform trans = cachedTransform;
		mCorners[0] = trans.TransformPoint(x0, cy, 0f);
		mCorners[1] = trans.TransformPoint(cx, y1, 0f);
		mCorners[2] = trans.TransformPoint(x1, cy, 0f);
		mCorners[3] = trans.TransformPoint(cx, y0, 0f);

		if (relativeTo != null)
		{
			for (int i = 0; i < 4; ++i)
				mCorners[i] = relativeTo.InverseTransformPoint(mCorners[i]);
		}
		return mCorners;
	}

	[System.NonSerialized] int mAlphaFrameID = -1;

	/// <summary>
	/// Widget's final alpha, after taking the panel's alpha into account.
	/// </summary>

	public override float CalculateFinalAlpha (int frameID)
	{
#if UNITY_EDITOR
		if (mAlphaFrameID != frameID || !Application.isPlaying)
#else
		if (mAlphaFrameID != frameID)
#endif
		{
			mAlphaFrameID = frameID;
			UpdateFinalAlpha(frameID);
		}
		return finalAlpha;
	}

	/// <summary>
	/// Force-calculate the final alpha value.
	/// </summary>

	protected void UpdateFinalAlpha (int frameID)
	{
		if (!mIsVisibleByAlpha || !mIsInFront)
		{
			finalAlpha = 0f;
		}
		else
		{
			UIRect pt = parent;
			finalAlpha = (pt != null) ? pt.CalculateFinalAlpha(frameID) * mColor.a : mColor.a;
		}
	}

	/// <summary>
	/// Update the widget's visibility and final alpha.
	/// </summary>

	public override void Invalidate (bool includeChildren)
	{
		mChanged = true;
		mAlphaFrameID = -1;

		if (panel != null)
		{
			bool vis = (hideIfOffScreen || panel.hasCumulativeClipping) ? panel.IsVisible(this) : true;
			UpdateVisibility(CalculateCumulativeAlpha(Time.frameCount) > 0.001f, vis);
			UpdateFinalAlpha(Time.frameCount);
			if (includeChildren) base.Invalidate(true);
		}
	}

	/// <summary>
	/// Same as final alpha, except it doesn't take own visibility into consideration. Used by panels.
	/// </summary>

	public float CalculateCumulativeAlpha (int frameID)
	{
		UIRect pt = parent;
		return (pt != null) ? pt.CalculateFinalAlpha(frameID) * mColor.a : mColor.a;
	}

	/// <summary>
	/// Set the widget's rectangle. XY is the bottom-left corner.
	/// </summary>

	public override void SetRect (float x, float y, float width, float height)
	{
		Vector2 po = pivotOffset;

		float fx = Mathf.Lerp(x, x + width, po.x);
		float fy = Mathf.Lerp(y, y + height, po.y);

		int finalWidth = Mathf.FloorToInt(width + 0.5f);
		int finalHeight = Mathf.FloorToInt(height + 0.5f);

		if (po.x == 0.5f) finalWidth = ((finalWidth >> 1) << 1);
		if (po.y == 0.5f) finalHeight = ((finalHeight >> 1) << 1);

		Transform t = cachedTransform;
		Vector3 pos = t.localPosition;
		pos.x = Mathf.Floor(fx + 0.5f);
		pos.y = Mathf.Floor(fy + 0.5f);

		if (finalWidth < minWidth) finalWidth = minWidth;
		if (finalHeight < minHeight) finalHeight = minHeight;

		t.localPosition = pos;
		this.width = finalWidth;
		this.height = finalHeight;

		if (isAnchored)
		{
			t = t.parent;

			if (leftAnchor.target) leftAnchor.SetHorizontal(t, x);
			if (rightAnchor.target) rightAnchor.SetHorizontal(t, x + width);
			if (bottomAnchor.target) bottomAnchor.SetVertical(t, y);
			if (topAnchor.target) topAnchor.SetVertical(t, y + height);
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	/// <summary>
	/// Adjust the widget's collider size to match the widget's dimensions.
	/// </summary>

	public void ResizeCollider () { if (NGUITools.GetActive(this)) NGUITools.UpdateWidgetCollider(gameObject); }

	/// <summary>
	/// Static widget comparison function used for depth sorting.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int FullCompareFunc (UIWidget left, UIWidget right)
	{
		int val = UIPanel.CompareFunc(left.panel, right.panel);
		return (val == 0) ? PanelCompareFunc(left, right) : val;
	}

	/// <summary>
	/// Static widget comparison function used for inter-panel depth sorting.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int PanelCompareFunc (UIWidget left, UIWidget right)
	{
		if (left.mDepth < right.mDepth) return -1;
		if (left.mDepth > right.mDepth) return 1;

		Material leftMat = left.material;
		Material rightMat = right.material;

		if (leftMat == rightMat) return 0;
		if (leftMat == null) return 1;
		if (rightMat == null) return -1;

		return (leftMat.GetInstanceID() < rightMat.GetInstanceID()) ? -1 : 1;
	}

	/// <summary>
	/// Calculate the widget's bounds, optionally making them relative to the specified transform.
	/// </summary>

	public Bounds CalculateBounds () { return CalculateBounds(null); }

	/// <summary>
	/// Calculate the widget's bounds, optionally making them relative to the specified transform.
	/// </summary>

	public Bounds CalculateBounds (Transform relativeParent)
	{
		if (relativeParent == null)
		{
			Vector3[] corners = localCorners;
			Bounds b = new Bounds(corners[0], Vector3.zero);
			for (int j = 1; j < 4; ++j) b.Encapsulate(corners[j]);
			return b;
		}
		else
		{
			Matrix4x4 toLocal = relativeParent.worldToLocalMatrix;
			Vector3[] corners = worldCorners;
			Bounds b = new Bounds(toLocal.MultiplyPoint3x4(corners[0]), Vector3.zero);
			for (int j = 1; j < 4; ++j) b.Encapsulate(toLocal.MultiplyPoint3x4(corners[j]));
			return b;
		}
	}

	/// <summary>
	/// Mark the widget as changed so that the geometry can be rebuilt.
	/// </summary>

	public void SetDirty ()
	{
		if (drawCall != null)
		{
			drawCall.isDirty = true;
		}
		else if (isVisible && hasVertices)
		{
			CreatePanel();
		}
	}

	/// <summary>
	/// Remove this widget from the panel.
	/// </summary>

	public void RemoveFromPanel ()
	{
		if (panel != null)
		{
			panel.RemoveWidget(this);
			panel = null;
		}
		drawCall = null;
#if UNITY_EDITOR
		mOldTex = null;
		mOldShader = null;
#endif
	}

#if UNITY_EDITOR
	[System.NonSerialized] Texture mOldTex;
	[System.NonSerialized] Shader mOldShader;

	/// <summary>
	/// This callback is sent inside the editor notifying us that some property has changed.
	/// </summary>

	protected override void OnValidate()
	{
		if (NGUITools.GetActive(this))
		{
			base.OnValidate();

			if (mWidth < minWidth) mWidth = minWidth;
			if (mHeight < minHeight) mHeight = minHeight;
			if (autoResizeBoxCollider) ResizeCollider();

			// If the texture is changing, we need to make sure to rebuild the draw calls
			if (mOldTex != mainTexture || mOldShader != shader)
			{
				mOldTex = mainTexture;
				mOldShader = shader;
			}

			aspectRatio = (keepAspectRatio == AspectRatioSource.Free) ?
				(float)mWidth / mHeight : Mathf.Max(0.01f, aspectRatio);

			if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
			{
				mWidth = Mathf.RoundToInt(mHeight * aspectRatio);
			}
			else if (keepAspectRatio == AspectRatioSource.BasedOnWidth)
			{
				mHeight = Mathf.RoundToInt(mWidth / aspectRatio);
			}

			if (!Application.isPlaying)
			{
				if (panel != null)
				{
					panel.RemoveWidget(this);
					panel = null;
				}
				CreatePanel();
			}
		}
		else
		{
			if (mWidth < minWidth) mWidth = minWidth;
			if (mHeight < minHeight) mHeight = minHeight;
		}
	}
#endif

	/// <summary>
	/// Tell the panel responsible for the widget that something has changed and the buffers need to be rebuilt.
	/// </summary>

	public virtual void MarkAsChanged ()
	{
		if (NGUITools.GetActive(this))
		{
			mChanged = true;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
			// If we're in the editor, update the panel right away so its geometry gets updated.
			if (panel != null && enabled && NGUITools.GetActive(gameObject) && !mPlayMode)
			{
				SetDirty();
				CheckLayer();
#if UNITY_EDITOR
				// Mark the panel as dirty so it gets updated
				if (material != null) NGUITools.SetDirty(panel.gameObject);
#endif
			}
		}
	}

	/// <summary>
	/// Ensure we have a panel referencing this widget.
	/// </summary>

	public UIPanel CreatePanel ()
	{
		if (mStarted && panel == null && enabled && NGUITools.GetActive(gameObject))
		{
			panel = UIPanel.Find(cachedTransform, true, cachedGameObject.layer);

			if (panel != null)
			{
				mParentFound = false;
				panel.AddWidget(this);
				CheckLayer();
				Invalidate(true);
			}
		}
		return panel;
	}

	/// <summary>
	/// Check to ensure that the widget resides on the same layer as its panel.
	/// </summary>

	public void CheckLayer ()
	{
		if (panel != null && panel.gameObject.layer != gameObject.layer)
		{
			Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\n" +
				"If you want to move widgets to a different layer, parent them to a new panel instead.", this);
			gameObject.layer = panel.gameObject.layer;
		}
	}

	/// <summary>
	/// Checks to ensure that the widget is still parented to the right panel.
	/// </summary>

	public override void ParentHasChanged ()
	{
		base.ParentHasChanged();

		if (panel != null)
		{
			UIPanel p = UIPanel.Find(cachedTransform, true, cachedGameObject.layer);

			if (panel != p)
			{
				RemoveFromPanel();
				CreatePanel();
			}
		}
	}

	/// <summary>
	/// Remember whether we're in play mode.
	/// </summary>

	protected override void Awake ()
	{
		base.Awake();
		mPlayMode = Application.isPlaying;
	}

	/// <summary>
	/// Mark the widget and the panel as having been changed.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();
		RemoveFromPanel();
		mMoved = true;
		Update();
	}

	/// <summary>
	/// Facilitates upgrading from NGUI 2.6.5 or earlier versions.
	/// </summary>

	protected virtual void UpgradeFrom265 ()
	{
		Vector3 scale = cachedTransform.localScale;
		mWidth = Mathf.Abs(Mathf.RoundToInt(scale.x));
		mHeight = Mathf.Abs(Mathf.RoundToInt(scale.y));
		NGUITools.UpdateWidgetCollider(gameObject, true);
	}

	/// <summary>
	/// Virtual Start() functionality for widgets.
	/// </summary>

	protected override void OnStart ()
	{
#if UNITY_EDITOR
		if (GetComponent<UIPanel>() != null)
		{
			Debug.LogError("Widgets and panels should not be on the same object! Widget must be a child of the panel.", this);
		}
		else if (!Application.isPlaying && GetComponents<UIWidget>().Length > 1)
		{
			Debug.LogError("You should not place more than one widget on the same object. Weird stuff will happen!", this);
		}
#endif
		CreatePanel();
	}

	/// <summary>
	/// Update the anchored edges and ensure the widget is registered with a panel.
	/// </summary>

	protected override void OnAnchor ()
	{
		float lt, bt, rt, tt;
		Transform trans = cachedTransform;
		Transform parent = trans.parent;
		Vector3 pos = trans.localPosition;
		Vector2 pvt = pivotOffset;

		// Attempt to fast-path if all anchors match
		if (leftAnchor.target == bottomAnchor.target &&
			leftAnchor.target == rightAnchor.target &&
			leftAnchor.target == topAnchor.target)
		{
			Vector3[] sides = leftAnchor.GetSides(parent);

			if (sides != null)
			{
				lt = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + leftAnchor.absolute;
				rt = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + rightAnchor.absolute;
				bt = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + bottomAnchor.absolute;
				tt = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + topAnchor.absolute;
				mIsInFront = true;
			}
			else
			{
				// Anchored to a single transform
				Vector3 lp = GetLocalPos(leftAnchor, parent);
				lt = lp.x + leftAnchor.absolute;
				bt = lp.y + bottomAnchor.absolute;
				rt = lp.x + rightAnchor.absolute;
				tt = lp.y + topAnchor.absolute;
				mIsInFront = (!hideIfOffScreen || lp.z >= 0f);
			}
		}
		else
		{
			mIsInFront = true;

			// Left anchor point
			if (leftAnchor.target)
			{
				Vector3[] sides = leftAnchor.GetSides(parent);

				if (sides != null)
				{
					lt = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + leftAnchor.absolute;
				}
				else
				{
					lt = GetLocalPos(leftAnchor, parent).x + leftAnchor.absolute;
				}
			}
			else lt = pos.x - pvt.x * mWidth;

			// Right anchor point
			if (rightAnchor.target)
			{
				Vector3[] sides = rightAnchor.GetSides(parent);

				if (sides != null)
				{
					rt = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + rightAnchor.absolute;
				}
				else
				{
					rt = GetLocalPos(rightAnchor, parent).x + rightAnchor.absolute;
				}
			}
			else rt = pos.x - pvt.x * mWidth + mWidth;

			// Bottom anchor point
			if (bottomAnchor.target)
			{
				Vector3[] sides = bottomAnchor.GetSides(parent);

				if (sides != null)
				{
					bt = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + bottomAnchor.absolute;
				}
				else
				{
					bt = GetLocalPos(bottomAnchor, parent).y + bottomAnchor.absolute;
				}
			}
			else bt = pos.y - pvt.y * mHeight;

			// Top anchor point
			if (topAnchor.target)
			{
				Vector3[] sides = topAnchor.GetSides(parent);

				if (sides != null)
				{
					tt = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + topAnchor.absolute;
				}
				else
				{
					tt = GetLocalPos(topAnchor, parent).y + topAnchor.absolute;
				}
			}
			else tt = pos.y - pvt.y * mHeight + mHeight;
		}

		// Calculate the new position, width and height
		Vector3 newPos = new Vector3(Mathf.Lerp(lt, rt, pvt.x), Mathf.Lerp(bt, tt, pvt.y), pos.z);
		newPos.x = Mathf.Round(newPos.x);
		newPos.y = Mathf.Round(newPos.y);

		int w = Mathf.FloorToInt(rt - lt + 0.5f);
		int h = Mathf.FloorToInt(tt - bt + 0.5f);

		// Maintain the aspect ratio if requested and possible
		if (keepAspectRatio != AspectRatioSource.Free && aspectRatio != 0f)
		{
			if (keepAspectRatio == AspectRatioSource.BasedOnHeight)
			{
				w = Mathf.RoundToInt(h * aspectRatio);
			}
			else h = Mathf.RoundToInt(w / aspectRatio);
		}

		// Don't let the width and height get too small
		if (w < minWidth) w = minWidth;
		if (h < minHeight) h = minHeight;

		// Update the position if it has changed
		if (Vector3.SqrMagnitude(pos - newPos) > 0.001f)
		{
			cachedTransform.localPosition = newPos;
			if (mIsInFront) mChanged = true;
		}

		// Update the width and height if it has changed
		if (mWidth != w || mHeight != h)
		{
			mWidth = w;
			mHeight = h;
			if (mIsInFront) mChanged = true;
			if (autoResizeBoxCollider) ResizeCollider();
		}
	}

	/// <summary>
	/// Ensure we have a panel to work with.
	/// </summary>

	protected override void OnUpdate ()
	{
		if (panel == null) CreatePanel();
#if UNITY_EDITOR
		else if (!mPlayMode) ParentHasChanged();
#endif
	}

#if !UNITY_EDITOR
	/// <summary>
	/// Mark the UI as changed when returning from paused state.
	/// </summary>

	void OnApplicationPause (bool paused) { if (!paused) MarkAsChanged(); }
#endif

	/// <summary>
	/// Clear references.
	/// </summary>

	protected override void OnDisable ()
	{
		RemoveFromPanel();
		base.OnDisable();
	}

	/// <summary>
	/// Unregister this widget.
	/// </summary>

	void OnDestroy () { RemoveFromPanel(); }

#if UNITY_EDITOR
	static int mHandles = -1;

	/// <summary>
	/// Whether widgets will show handles with the Move Tool, or just the View Tool.
	/// </summary>

	static public bool showHandlesWithMoveTool
	{
		get
		{
			if (mHandles == -1)
			{
				mHandles = UnityEditor.EditorPrefs.GetInt("NGUI Handles", 1);
			}
			return (mHandles == 1);
		}
		set
		{
			int val = value ? 1 : 0;

			if (mHandles != val)
			{
				mHandles = val;
				UnityEditor.EditorPrefs.SetInt("NGUI Handles", mHandles);
			}
		}
	}

	/// <summary>
	/// Whether the widget should have some form of handles shown.
	/// </summary>

	static public bool showHandles
	{
		get
		{
#if UNITY_4_3 || UNITY_4_5
			if (showHandlesWithMoveTool)
			{
				return UnityEditor.Tools.current == UnityEditor.Tool.Move;
			}
			return UnityEditor.Tools.current == UnityEditor.Tool.View;
#else
			return UnityEditor.Tools.current == UnityEditor.Tool.Rect;
#endif
		}
	}

	/// <summary>
	/// Draw some selectable gizmos.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (isVisible && NGUITools.GetActive(this))
		{
			if (UnityEditor.Selection.activeGameObject == gameObject && showHandles) return;

			Color outline = new Color(1f, 1f, 1f, 0.2f);

			float adjustment = (root != null) ? 0.05f : 0.001f;
			Vector2 offset = pivotOffset;
			Vector3 center = new Vector3(mWidth * (0.5f - offset.x), mHeight * (0.5f - offset.y), -mDepth * adjustment);
			Vector3 size = new Vector3(mWidth, mHeight, 1f);

			// Draw the gizmo
			Gizmos.matrix = cachedTransform.localToWorldMatrix;
			Gizmos.color = (UnityEditor.Selection.activeGameObject == gameObject) ? Color.white : outline;
			Gizmos.DrawWireCube(center, size);

			// Make the widget selectable
			size.z = 0.01f;
			Gizmos.color = Color.clear;
			Gizmos.DrawCube(center, size);
		}
	}
#endif // UNITY_EDITOR

	/// <summary>
	/// Update the widget's visibility state.
	/// </summary>

	public bool UpdateVisibility (bool visibleByAlpha, bool visibleByPanel)
	{
		if (mIsVisibleByAlpha != visibleByAlpha || mIsVisibleByPanel != visibleByPanel)
		{
			mChanged = true;
			mIsVisibleByAlpha = visibleByAlpha;
			mIsVisibleByPanel = visibleByPanel;
			return true;
		}
		return false;
	}

	int mMatrixFrame = -1;
	Vector3 mOldV0;
	Vector3 mOldV1;

	/// <summary>
	/// Check to see if the widget has moved relative to the panel that manages it
	/// </summary>

	public bool UpdateTransform (int frame)
	{
		Transform trans = cachedTransform;
		mPlayMode = Application.isPlaying;

#if UNITY_EDITOR
		if (mMoved || !mPlayMode)
#else
		if (mMoved)
#endif
		{
			mMoved = true;
			mMatrixFrame = -1;
			trans.hasChanged = false;
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			mOldV0 = panel.worldToLocal.MultiplyPoint3x4(trans.TransformPoint(x0, y0, 0f));
			mOldV1 = panel.worldToLocal.MultiplyPoint3x4(trans.TransformPoint(x1, y1, 0f));
		}
		else if (!panel.widgetsAreStatic && trans.hasChanged)
		{
			mMatrixFrame = -1;
			trans.hasChanged = false;
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			Vector3 v0 = panel.worldToLocal.MultiplyPoint3x4(trans.TransformPoint(x0, y0, 0f));
			Vector3 v1 = panel.worldToLocal.MultiplyPoint3x4(trans.TransformPoint(x1, y1, 0f));

			if (Vector3.SqrMagnitude(mOldV0 - v0) > 0.000001f ||
				Vector3.SqrMagnitude(mOldV1 - v1) > 0.000001f)
			{
				mMoved = true;
				mOldV0 = v0;
				mOldV1 = v1;
			}
		}

		// Notify the listeners
		if (mMoved && onChange != null) onChange();
		return mMoved || mChanged;
	}

	/// <summary>
	/// Update the widget and fill its geometry if necessary. Returns whether something was changed.
	/// </summary>

	public bool UpdateGeometry (int frame)
	{
		// Has the alpha changed?
		float finalAlpha = CalculateFinalAlpha(frame);
		if (mIsVisibleByAlpha && mLastAlpha != finalAlpha) mChanged = true;
		mLastAlpha = finalAlpha;

		if (mChanged)
		{
			if (mIsVisibleByAlpha && finalAlpha > 0.001f && shader != null)
			{
				bool hadVertices = geometry.hasVertices;

				if (fillGeometry)
				{
					geometry.Clear();
					OnFill(geometry.verts, geometry.uvs, geometry.cols);
				}

				if (geometry.hasVertices)
				{
					// Want to see what's being filled? Uncomment this line.
					//Debug.Log("Fill " + name + " (" + Time.frameCount + ")");

					if (mMatrixFrame != frame)
					{
						mLocalToPanel = panel.worldToLocal * cachedTransform.localToWorldMatrix;
						mMatrixFrame = frame;
					}
					geometry.ApplyTransform(mLocalToPanel, panel.generateNormals);
					mMoved = false;
					mChanged = false;
					return true;
				}

				mChanged = false;
				return hadVertices;
			}
			else if (geometry.hasVertices)
			{
				if (fillGeometry) geometry.Clear();
				mMoved = false;
				mChanged = false;
				return true;
			}
		}
		else if (mMoved && geometry.hasVertices)
		{
			// Want to see what's being moved? Uncomment this line.
			//Debug.Log("Moving " + name + " (" + Time.frameCount + ")");

			if (mMatrixFrame != frame)
			{
				mLocalToPanel = panel.worldToLocal * cachedTransform.localToWorldMatrix;
				mMatrixFrame = frame;
			}
			geometry.ApplyTransform(mLocalToPanel, panel.generateNormals);
			mMoved = false;
			mChanged = false;
			return true;
		}
		mMoved = false;
		mChanged = false;
		return false;
	}

	/// <summary>
	/// Append the local geometry buffers to the specified ones.
	/// </summary>

	public void WriteToBuffers (List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2)
	{
		geometry.WriteToBuffers(v, u, c, n, t, u2);
	}

	/// <summary>
	/// Make the widget pixel-perfect.
	/// </summary>

	virtual public void MakePixelPerfect ()
	{
		Vector3 pos = cachedTransform.localPosition;
		pos.z = Mathf.Round(pos.z);
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		cachedTransform.localPosition = pos;

		Vector3 ls = cachedTransform.localScale;
		cachedTransform.localScale = new Vector3(Mathf.Sign(ls.x), Mathf.Sign(ls.y), 1f);
	}

	/// <summary>
	/// Minimum allowed width for this widget.
	/// </summary>

	virtual public int minWidth { get { return 2; } }

	/// <summary>
	/// Minimum allowed height for this widget.
	/// </summary>

	virtual public int minHeight { get { return 2; } }

	/// <summary>
	/// Dimensions of the sprite's border, if any.
	/// </summary>

	virtual public Vector4 border { get { return Vector4.zero; } set { } }

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>

	virtual public void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		// Call this in your derived classes:
		//if (onPostFill != null)
		//	onPostFill(this, verts.size, verts, uvs, cols);
	}
}
