//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Panel is responsible for collecting, sorting and updating widgets in addition to generating widgets' geometry.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Panel")]
public class UIPanel : UIRect
{
	/// <summary>
	/// List of active panels.
	/// </summary>

	static public List<UIPanel> list = new List<UIPanel>();

	public enum RenderQueue
	{
		Automatic,
		StartAt,
		Explicit,
	}

	public delegate void OnGeometryUpdated ();

	/// <summary>
	/// Notification triggered when the panel's geometry get rebuilt. It's mainly here for debugging purposes.
	/// </summary>

	public OnGeometryUpdated onGeometryUpdated;

	/// <summary>
	/// Whether this panel will show up in the panel tool (set this to 'false' for dynamically created temporary panels)
	/// </summary>

	public bool showInPanelTool = true;

	/// <summary>
	/// Whether normals and tangents will be generated for all meshes.
	/// </summary>
	
	public bool generateNormals = false;

	/// <summary>
	/// Whether secondary UV coordinates will be generated for all meshes.
	/// </summary>

	public bool generateUV2 = false;

#if !UNITY_4_7
	/// <summary>
	/// Whether generated geometry will cast shadows.
	/// </summary>

	public UIDrawCall.ShadowMode shadowMode = UIDrawCall.ShadowMode.None;
#endif

	/// <summary>
	/// Whether widgets drawn by this panel are static (won't move). This will improve performance.
	/// </summary>

	public bool widgetsAreStatic = false;

	/// <summary>
	/// Whether widgets will be culled while the panel is being dragged.
	/// Having this on improves performance, but turning it off will reduce garbage collection.
	/// </summary>

	public bool cullWhileDragging = true;

	/// <summary>
	/// Optimization flag. Makes the assumption that the panel's geometry
	/// will always be on screen and the bounds don't need to be re-calculated.
	/// </summary>

	public bool alwaysOnScreen = false;

	/// <summary>
	/// By default, non-clipped panels use the camera's bounds, and the panel's position has no effect.
	/// If you want the panel's position to actually be used with anchors, set this field to 'true'.
	/// </summary>

	public bool anchorOffset = false;

	/// <summary>
	/// Whether the soft border will be used as padding.
	/// </summary>

	public bool softBorderPadding = true;

	/// <summary>
	/// By default all panels manage render queues of their draw calls themselves by incrementing them
	/// so that the geometry is drawn in the proper order. You can alter this behaviour.
	/// </summary>

	public RenderQueue renderQueue = RenderQueue.Automatic;

	/// <summary>
	/// Render queue used by the panel. The default value of '3000' is the equivalent of "Transparent".
	/// This property is only used if 'renderQueue' is set to something other than "Automatic".
	/// </summary>

	public int startingRenderQueue = 3000;

	/// <summary>
	/// Sorting layer used by the panel -- used when mixing NGUI with the Unity's 2D system.
	/// Contributed by Benzino07: http://www.tasharen.com/forum/index.php?topic=6956.15
	/// </summary>

	public string sortingLayerName
	{
		get
		{
			return mSortingLayerName;
		}
		set
		{
			if (mSortingLayerName != value)
			{
				mSortingLayerName = value;
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
				UpdateDrawCalls(list.IndexOf(this));
			}
		}
	}

	/// <summary>
	/// List of widgets managed by this panel. Do not attempt to modify this list yourself.
	/// </summary>

	[System.NonSerialized]
	public List<UIWidget> widgets = new List<UIWidget>();

	/// <summary>
	/// List of draw calls created by this panel. Do not attempt to modify this list yourself.
	/// </summary>

	[System.NonSerialized]
	public List<UIDrawCall> drawCalls = new List<UIDrawCall>();

	/// <summary>
	/// Matrix that will transform the specified world coordinates to relative-to-panel coordinates.
	/// </summary>

	[System.NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	/// <summary>
	/// Cached clip range passed to the draw call's shader.
	/// </summary>

	[System.NonSerialized]
	public Vector4 drawCallClipRange = new Vector4(0f, 0f, 1f, 1f);

	public delegate void OnClippingMoved (UIPanel panel);

	/// <summary>
	/// Event callback that's triggered when the panel's clip region gets moved.
	/// </summary>

	public OnClippingMoved onClipMove;

	/// <summary>
	/// There may be cases where you will want to create a custom material per-widget in order to have unique draw calls.
	/// If that's the case, set this delegate and return your newly created material. Note that it's up to you to cache this material for the next call.
	/// </summary>

	public OnCreateMaterial onCreateMaterial;
	public delegate Material OnCreateMaterial (UIWidget widget, Material mat);

	/// <summary>
	/// Event callback that's triggered whenever the panel creates a new draw call.
	/// </summary>

	public UIDrawCall.OnCreateDrawCall onCreateDrawCall;

	// Clip texture feature contributed by the community: http://www.tasharen.com/forum/index.php?topic=9268.0
	[HideInInspector][SerializeField] Texture2D mClipTexture = null;

	// Panel's alpha (affects the alpha of all widgets)
	[HideInInspector][SerializeField] float mAlpha = 1f;

	// Clipping rectangle
	[HideInInspector][SerializeField] UIDrawCall.Clipping mClipping = UIDrawCall.Clipping.None;
	[HideInInspector][SerializeField] Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);
	[HideInInspector][SerializeField] Vector2 mClipSoftness = new Vector2(4f, 4f);
	[HideInInspector][SerializeField] int mDepth = 0;
	[HideInInspector][SerializeField] int mSortingOrder = 0;
	[HideInInspector][SerializeField] string mSortingLayerName = null;

	// Whether a full rebuild of geometry buffers is required
	bool mRebuild = false;
	bool mResized = false;

	[SerializeField] Vector2 mClipOffset = Vector2.zero;

	int mMatrixFrame = -1;
	int mAlphaFrameID = 0;
	int mLayer = -1;

	// Values used for visibility checks
	static float[] mTemp = new float[4];
	Vector2 mMin = Vector2.zero;
	Vector2 mMax = Vector2.zero;
#if !UNITY_5_5_OR_NEWER
	bool mHalfPixelOffset = false;
#endif
	bool mSortWidgets = false;
	bool mUpdateScroll = false;

	/// <summary>
	/// Helper property that returns the first unused depth value.
	/// </summary>

	static public int nextUnusedDepth
	{
		get
		{
			int highest = int.MinValue;
			for (int i = 0, imax = list.Count; i < imax; ++i)
				highest = Mathf.Max(highest, list[i].depth);
			return (highest == int.MinValue) ? 0 : highest + 1;
		}
	}

	/// <summary>
	/// Whether the rectangle can be anchored.
	/// </summary>

	public override bool canBeAnchored { get { return mClipping != UIDrawCall.Clipping.None; } }

	/// <summary>
	/// Panel's alpha affects everything drawn by the panel.
	/// </summary>

	public override float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mAlpha != val)
			{
				bool wasVisible = mAlpha > 0.001f;
				mAlphaFrameID = -1;
				mResized = true;
				mAlpha = val;
				for (int i = 0, imax = drawCalls.Count; i < imax; ++i)
					drawCalls[i].isDirty = true;
				Invalidate(!wasVisible && mAlpha > 0.001f);
			}
		}
	}

	/// <summary>
	/// Panels can have their own depth value that will change the order with which everything they manage gets drawn.
	/// </summary>

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
				list.Sort(CompareFunc);
			}
		}
	}

	/// <summary>
	/// Sorting order value for the panel's draw calls, to be used with Unity's 2D system.
	/// </summary>

	public int sortingOrder
	{
		get
		{
			return mSortingOrder;
		}
		set
		{
			if (mSortingOrder != value)
			{
				mSortingOrder = value;
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
				UpdateDrawCalls(list.IndexOf(this));
			}
		}
	}

	/// <summary>
	/// Function that can be used to depth-sort panels.
	/// </summary>

	static public int CompareFunc (UIPanel a, UIPanel b)
	{
		if (a != b && a != null && b != null)
		{
			if (a.mDepth < b.mDepth) return -1;
			if (a.mDepth > b.mDepth) return 1;
			return (a.GetInstanceID() < b.GetInstanceID()) ? -1 : 1;
		}
		return 0;
	}

	/// <summary>
	/// Panel's width in pixels.
	/// </summary>

	public float width { get { return GetViewSize().x; } }

	/// <summary>
	/// Panel's height in pixels.
	/// </summary>

	public float height { get { return GetViewSize().y; } }

	/// <summary>
	/// Whether the panel's drawn geometry needs to be offset by a half-pixel.
	/// </summary>

	public bool halfPixelOffset
	{
		get
		{
#if UNITY_5_5_OR_NEWER
			return false;
#else
			return mHalfPixelOffset;
#endif
		}
	}

	/// <summary>
	/// Whether the camera is used to draw UI geometry.
	/// </summary>

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	public bool usedForUI { get { return (anchorCamera != null && mCam.isOrthoGraphic); } }
#else
	public bool usedForUI { get { return (anchorCamera != null && mCam.orthographic); } }
#endif

	/// <summary>
	/// Directx9 pixel offset, used for drawing.
	/// </summary>

	public Vector3 drawCallOffset
	{
		get
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (anchorCamera != null && mCam.isOrthoGraphic)
#else
			if (anchorCamera != null && mCam.orthographic)
#endif
			{
				Vector2 size = GetWindowSize();
				float pixelSize = (root != null) ? root.pixelSizeAdjustment : 1f;
				float mod = (pixelSize / size.y) / mCam.orthographicSize;

#if UNITY_5_5_OR_NEWER
				bool x = false, y = false;
#else
				bool x = mHalfPixelOffset, y = mHalfPixelOffset;
#endif
				if ((Mathf.RoundToInt(size.x) & 1) == 1) x = !x;
				if ((Mathf.RoundToInt(size.y) & 1) == 1) y = !y;

				return new Vector3(x ? -mod : 0f, y ? mod : 0f);
			}
			return Vector3.zero;
		}
	}

	/// <summary>
	/// Clipping method used by all draw calls.
	/// </summary>

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mResized = true;
				mClipping = value;
				mMatrixFrame = -1;
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls(list.IndexOf(this));
#endif
			}
		}
	}

	UIPanel mParentPanel = null;

	/// <summary>
	/// Reference to the parent panel, if any.
	/// </summary>

	public UIPanel parentPanel { get { return mParentPanel; } }

	/// <summary>
	/// Number of times the panel's contents get clipped.
	/// </summary>

	public int clipCount
	{
		get
		{
			int count = 0;
			UIPanel p = this;

			while (p != null)
			{
				if (p.mClipping == UIDrawCall.Clipping.SoftClip || p.mClipping == UIDrawCall.Clipping.TextureMask) ++count;
				p = p.mParentPanel;
			}
			return count;
		}
	}

	/// <summary>
	/// Whether the panel will actually perform clipping of children.
	/// </summary>

	public bool hasClipping { get { return mClipping == UIDrawCall.Clipping.SoftClip || mClipping == UIDrawCall.Clipping.TextureMask; } }

	/// <summary>
	/// Whether the panel will actually perform clipping of children.
	/// </summary>

	public bool hasCumulativeClipping { get { return clipCount != 0; } }

	[System.Obsolete("Use 'hasClipping' or 'hasCumulativeClipping' instead")]
	public bool clipsChildren { get { return hasCumulativeClipping; } }

	/// <summary>
	/// Clipping area offset used to make it possible to move clipped panels (scroll views) efficiently.
	/// Scroll views move by adjusting the clip offset by one value, and the transform position by the inverse.
	/// This makes it possible to not have to rebuild the geometry, greatly improving performance.
	/// </summary>

	public Vector2 clipOffset
	{
		get
		{
			return mClipOffset;
		}
		set
		{
			if (Mathf.Abs(mClipOffset.x - value.x) > 0.001f ||
				Mathf.Abs(mClipOffset.y - value.y) > 0.001f)
			{
				mClipOffset = value;
				InvalidateClipping();

				// Call the event delegate
				if (onClipMove != null) onClipMove(this);
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls(list.IndexOf(this));
#endif
			}
		}
	}

	/// <summary>
	/// Invalidate the panel's clipping, calling child panels in turn.
	/// </summary>

	void InvalidateClipping ()
	{
		mResized = true;
		mMatrixFrame = -1;

		for (int i = 0, imax = list.Count; i < imax; ++i)
		{
			UIPanel p = list[i];
			if (p != this && p.parentPanel == this)
				p.InvalidateClipping();
		}
	}

	/// <summary>
	/// Texture used to clip the region.
	/// </summary>

	public Texture2D clipTexture
	{
		get
		{
			return mClipTexture;
		}
		set
		{
			if (mClipTexture != value)
			{
				mClipTexture = value;
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls(list.IndexOf(this));
#endif
			}
		}
	}

	/// <summary>
	/// Clipping position (XY) and size (ZW).
	/// Note that you should not be modifying this property at run-time to reposition the clipping. Adjust clipOffset instead.
	/// </summary>

	[System.Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	public Vector4 clipRange
	{
		get
		{
			return baseClipRegion;
		}
		set
		{
			baseClipRegion = value;
		}
	}

	/// <summary>
	/// Clipping position (XY) and size (ZW).
	/// Note that you should not be modifying this property at run-time to reposition the clipping. Adjust clipOffset instead.
	/// </summary>

	public Vector4 baseClipRegion
	{
		get
		{
			return mClipRange;
		}
		set
		{
			if (Mathf.Abs(mClipRange.x - value.x) > 0.001f ||
				Mathf.Abs(mClipRange.y - value.y) > 0.001f ||
				Mathf.Abs(mClipRange.z - value.z) > 0.001f ||
				Mathf.Abs(mClipRange.w - value.w) > 0.001f)
			{
				mResized = true;
				mClipRange = value;
				mMatrixFrame = -1;

				UIScrollView sv = GetComponent<UIScrollView>();
				if (sv != null) sv.UpdatePosition();
				if (onClipMove != null) onClipMove(this);
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls(list.IndexOf(this));
#endif
			}
		}
	}

	/// <summary>
	/// Final clipping region after the offset has been taken into consideration. XY = center, ZW = size.
	/// </summary>

	public Vector4 finalClipRegion
	{
		get
		{
			Vector2 size = GetViewSize();

			if (mClipping != UIDrawCall.Clipping.None)
			{
				return new Vector4(mClipRange.x + mClipOffset.x, mClipRange.y + mClipOffset.y, size.x, size.y);
			}
			return new Vector4(0f, 0f, size.x, size.y);
		}
	}

	/// <summary>
	/// Clipping softness is used if the clipped style is set to "Soft".
	/// </summary>

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoftness;
		}
		set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
#if UNITY_EDITOR
				if (!Application.isPlaying) UpdateDrawCalls(list.IndexOf(this));
#endif
			}
		}
	}

	// Temporary variable to avoid GC allocation
	static Vector3[] mCorners = new Vector3[4];

	/// <summary>
	/// Local-space corners of the panel's clipping rectangle. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] localCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector3[] corners = worldCorners;
				Transform wt = cachedTransform;
				for (int i = 0; i < 4; ++i) corners[i] = wt.InverseTransformPoint(corners[i]);
				return corners;
			}
			else
			{
				float x0 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float y0 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x1 = x0 + mClipRange.z;
				float y1 = y0 + mClipRange.w;

				mCorners[0] = new Vector3(x0, y0);
				mCorners[1] = new Vector3(x0, y1);
				mCorners[2] = new Vector3(x1, y1);
				mCorners[3] = new Vector3(x1, y0);
			}
			return mCorners;
		}
	}

	/// <summary>
	/// World-space corners of the panel's clipping rectangle. The order is bottom-left, top-left, top-right, bottom-right.
	/// </summary>

	public override Vector3[] worldCorners
	{
		get
		{
			if (mClipping != UIDrawCall.Clipping.None)
			{
				float x0 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float y0 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x1 = x0 + mClipRange.z;
				float y1 = y0 + mClipRange.w;

				Transform wt = cachedTransform;

				mCorners[0] = wt.TransformPoint(x0, y0, 0f);
				mCorners[1] = wt.TransformPoint(x0, y1, 0f);
				mCorners[2] = wt.TransformPoint(x1, y1, 0f);
				mCorners[3] = wt.TransformPoint(x1, y0, 0f);
			}
			else if (anchorCamera != null)
			{
				Vector3[] corners = mCam.GetWorldCorners(cameraRayDistance);

				//if (anchorOffset && (mCam == null || mCam.transform.parent != cachedTransform))
				//{
				//    Vector3 off = cachedTransform.position;
				//    for (int i = 0; i < 4; ++i)
				//        corners[i] += off;
				//}
				return corners;
			}
			else
			{
				Vector2 size = GetViewSize();

				float x0 = -0.5f * size.x;
				float y0 = -0.5f * size.y;
				float x1 = x0 + size.x;
				float y1 = y0 + size.y;

				mCorners[0] = new Vector3(x0, y0);
				mCorners[1] = new Vector3(x0, y1);
				mCorners[2] = new Vector3(x1, y1);
				mCorners[3] = new Vector3(x1, y0);

				if (anchorOffset && (mCam == null || mCam.transform.parent != cachedTransform))
				{
					Vector3 off = cachedTransform.position;
					for (int i = 0; i < 4; ++i)
						mCorners[i] += off;
				}
			}
			return mCorners;
		}
	}

	/// <summary>
	/// Get the sides of the rectangle relative to the specified transform.
	/// The order is left, top, right, bottom.
	/// </summary>

	public override Vector3[] GetSides (Transform relativeTo)
	{
		if (mClipping != UIDrawCall.Clipping.None)
		{
			float x0 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
			float y0 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
			float x1 = x0 + mClipRange.z;
			float y1 = y0 + mClipRange.w;
			float hx = (x0 + x1) * 0.5f;
			float hy = (y0 + y1) * 0.5f;

			Transform wt = cachedTransform;

			mSides[0] = wt.TransformPoint(x0, hy, 0f);
			mSides[1] = wt.TransformPoint(hx, y1, 0f);
			mSides[2] = wt.TransformPoint(x1, hy, 0f);
			mSides[3] = wt.TransformPoint(hx, y0, 0f);

			if (relativeTo != null)
			{
				for (int i = 0; i < 4; ++i)
					mSides[i] = relativeTo.InverseTransformPoint(mSides[i]);
			}
			return mSides;
		}
		else if (anchorCamera != null && anchorOffset)
		{
			Vector3[] sides = mCam.GetSides(cameraRayDistance);
			Vector3 off = cachedTransform.position;
			for (int i = 0; i < 4; ++i)
				sides[i] += off;

			if (relativeTo != null)
			{
				for (int i = 0; i < 4; ++i)
					sides[i] = relativeTo.InverseTransformPoint(sides[i]);
			}
			return sides;
		}
		return base.GetSides(relativeTo);
	}

	/// <summary>
	/// Invalidating the panel should reset its alpha.
	/// </summary>

	public override void Invalidate (bool includeChildren)
	{
		mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

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
			UIRect pt = parent;
			finalAlpha = (parent != null) ? pt.CalculateFinalAlpha(frameID) * mAlpha : mAlpha;
		}
		return finalAlpha;
	}

	/// <summary>
	/// Set the panel's rectangle.
	/// </summary>

	public override void SetRect (float x, float y, float width, float height)
	{
		int finalWidth = Mathf.FloorToInt(width + 0.5f);
		int finalHeight = Mathf.FloorToInt(height + 0.5f);

		finalWidth = ((finalWidth >> 1) << 1);
		finalHeight = ((finalHeight >> 1) << 1);

		Transform t = cachedTransform;
		Vector3 pos = t.localPosition;
		pos.x = Mathf.Floor(x + 0.5f);
		pos.y = Mathf.Floor(y + 0.5f);

		if (finalWidth < 2) finalWidth = 2;
		if (finalHeight < 2) finalHeight = 2;

		baseClipRegion = new Vector4(pos.x, pos.y, finalWidth, finalHeight);

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
	/// Returns whether the specified rectangle is visible by the panel. The coordinates must be in world space.
	/// </summary>

#if UNITY_FLASH
	public bool IsVisible (Vector3 aa, Vector3 bb, Vector3 cc, Vector3 dd)
#else
	public bool IsVisible (Vector3 a, Vector3 b, Vector3 c, Vector3 d)
#endif
	{
		UpdateTransformMatrix();

		// Transform the specified points from world space to local space
#if UNITY_FLASH
		// http://www.tasharen.com/forum/index.php?topic=11390.0
		Vector3 a = worldToLocal.MultiplyPoint3x4(aa);
		Vector3 b = worldToLocal.MultiplyPoint3x4(bb);
		Vector3 c = worldToLocal.MultiplyPoint3x4(cc);
		Vector3 d = worldToLocal.MultiplyPoint3x4(dd);
#else
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
#endif
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;

		float minX = Mathf.Min(mTemp);
		float maxX = Mathf.Max(mTemp);

		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;

		float minY = Mathf.Min(mTemp);
		float maxY = Mathf.Max(mTemp);

		if (maxX < mMin.x) return false;
		if (maxY < mMin.y) return false;
		if (minX > mMax.x) return false;
		if (minY > mMax.y) return false;

		return true;
	}

	/// <summary>
	/// Returns whether the specified world position is within the panel's bounds determined by the clipping rect.
	/// </summary>

	public bool IsVisible (Vector3 worldPos)
	{
		if (mAlpha < 0.001f) return false;
		if (mClipping == UIDrawCall.Clipping.None ||
			mClipping == UIDrawCall.Clipping.ConstrainButDontClip) return true;

		UpdateTransformMatrix();

		Vector3 pos = worldToLocal.MultiplyPoint3x4(worldPos);
		if (pos.x < mMin.x) return false;
		if (pos.y < mMin.y) return false;
		if (pos.x > mMax.x) return false;
		if (pos.y > mMax.y) return false;
		return true;
	}

	/// <summary>
	/// Returns whether the specified widget is visible by the panel.
	/// </summary>

	public bool IsVisible (UIWidget w)
	{
		UIPanel p = this;
		Vector3[] corners = null;

		while (p != null)
		{
			if ((p.mClipping == UIDrawCall.Clipping.None || p.mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
			{
				p = p.mParentPanel;
				continue;
			}

			if (corners == null) corners = w.worldCorners;
			if (!p.IsVisible(corners[0], corners[1], corners[2], corners[3])) return false;
			p = p.mParentPanel;
		}
		return true;
	}

	/// <summary>
	/// Whether the specified widget is going to be affected by this panel in any way.
	/// </summary>

	public bool Affects (UIWidget w)
	{
		if (w == null) return false;

		UIPanel expected = w.panel;
		if (expected == null) return false;

		UIPanel p = this;

		while (p != null)
		{
			if (p == expected) return true;
			if (!p.hasCumulativeClipping) return false;
			p = p.mParentPanel;
		}
		return false;
	}

	/// <summary>
	/// Causes all draw calls to be re-created on the next update.
	/// </summary>

	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls () { mRebuild = true; }

	/// <summary>
	/// Invalidate the panel's draw calls, forcing them to be rebuilt on the next update.
	/// This call also affects all children.
	/// </summary>

	public void SetDirty ()
	{
		for (int i = 0, imax = drawCalls.Count; i < imax; ++i)
			drawCalls[i].isDirty = true;
		Invalidate(true);
	}

	/// <summary>
	/// Cache components.
	/// </summary>

	protected override void Awake ()
	{
		base.Awake();

#if !UNITY_5_5_OR_NEWER
		mHalfPixelOffset = (Application.platform == RuntimePlatform.WindowsPlayer ||
 #if !UNITY_5_4
			Application.platform == RuntimePlatform.WindowsWebPlayer ||
 #endif
			Application.platform == RuntimePlatform.WindowsEditor) &&
			SystemInfo.graphicsDeviceVersion.Contains("Direct3D") &&
			SystemInfo.graphicsShaderLevel < 40;
#endif
	}

	/// <summary>
	/// Find the parent panel, if we have one.
	/// </summary>

	void FindParent ()
	{
		Transform parent = cachedTransform.parent;
		mParentPanel = (parent != null) ? NGUITools.FindInParents<UIPanel>(parent.gameObject) : null;
	}

	/// <summary>
	/// Find the parent panel, if we have one.
	/// </summary>

	public override void ParentHasChanged ()
	{
		base.ParentHasChanged();
		FindParent();
	}

	/// <summary>
	/// Layer is used to ensure that if it changes, widgets get moved as well.
	/// </summary>

	protected override void OnStart ()
	{
		mLayer = cachedGameObject.layer;
	}

	/// <summary>
	/// Reset the frame IDs.
	/// </summary>

	protected override void OnEnable ()
	{
		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		OnStart();
		base.OnEnable();
		mMatrixFrame = -1;
	}

	/// <summary>
	/// Mark all widgets as having been changed so the draw calls get re-created.
	/// </summary>

	protected override void OnInit ()
	{
		if (list.Contains(this)) return;
		base.OnInit();
		FindParent();

		// Apparently having a rigidbody helps
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		if (rigidbody == null && mParentPanel == null)
#else
		if (GetComponent<Rigidbody>() == null && mParentPanel == null)
#endif
		{
			UICamera uic = (anchorCamera != null) ? mCam.GetComponent<UICamera>() : null;

			if (uic != null)
			{
				if (uic.eventType == UICamera.EventType.UI_3D || uic.eventType == UICamera.EventType.World_3D)
				{
					Rigidbody rb = gameObject.AddComponent<Rigidbody>();
					rb.isKinematic = true;
					rb.useGravity = false;
				}
				// It's unclear if this helps 2D physics or not, so leaving it disabled for now.
				// Note that when enabling this, the 'if (rigidbody == null)' statement above should be adjusted as well.
				//else
				//{
				//    Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
				//    rb.isKinematic = true;
				//}
			}
		}

		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;

		list.Add(this);
		list.Sort(CompareFunc);
	}

	/// <summary>
	/// Destroy all draw calls we've created when this script gets disabled.
	/// </summary>

	protected override void OnDisable ()
	{
		for (int i = 0, imax = drawCalls.Count; i < imax; ++i)
		{
			UIDrawCall dc = drawCalls[i];
			if (dc != null) UIDrawCall.Destroy(dc);
		}
		
		drawCalls.Clear();
		list.Remove(this);

		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		
		if (list.Count == 0)
		{
			UIDrawCall.ReleaseAll();
			mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	/// <summary>
	/// Update the world-to-local transform matrix as well as clipping bounds.
	/// </summary>

	void UpdateTransformMatrix ()
	{
		int fc = Time.frameCount;

		if (mHasMoved || mMatrixFrame != fc)
		{
			mMatrixFrame = fc;
			worldToLocal = cachedTransform.worldToLocalMatrix;

			Vector2 size = GetViewSize() * 0.5f;

			float x = mClipOffset.x + mClipRange.x;
			float y = mClipOffset.y + mClipRange.y;

			mMin.x = x - size.x;
			mMin.y = y - size.y;
			mMax.x = x + size.x;
			mMax.y = y + size.y;
		}
	}

	/// <summary>
	/// Update the edges after the anchors have been updated.
	/// </summary>

	protected override void OnAnchor ()
	{
		// No clipping = no edges to anchor
		if (mClipping == UIDrawCall.Clipping.None) return;

		Transform trans = cachedTransform;
		Transform parent = trans.parent;

		Vector2 size = GetViewSize();
		Vector2 offset = trans.localPosition;

		float lt, bt, rt, tt;

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
			}
			else
			{
				// Anchored to a single transform
				Vector2 lp = GetLocalPos(leftAnchor, parent);
				lt = lp.x + leftAnchor.absolute;
				bt = lp.y + bottomAnchor.absolute;
				rt = lp.x + rightAnchor.absolute;
				tt = lp.y + topAnchor.absolute;
			}
		}
		else
		{
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
			else lt = mClipRange.x - 0.5f * size.x;

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
			else rt = mClipRange.x + 0.5f * size.x;

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
			else bt = mClipRange.y - 0.5f * size.y;

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
			else tt = mClipRange.y + 0.5f * size.y;
		}

		// Take the offset into consideration
		lt -= offset.x + mClipOffset.x;
		rt -= offset.x + mClipOffset.x;
		bt -= offset.y + mClipOffset.y;
		tt -= offset.y + mClipOffset.y;

		// Calculate the new position, width and height
		float newX = Mathf.Lerp(lt, rt, 0.5f);
		float newY = Mathf.Lerp(bt, tt, 0.5f);
		float w = rt - lt;
		float h = tt - bt;

		float minx = Mathf.Max(2f, mClipSoftness.x);
		float miny = Mathf.Max(2f, mClipSoftness.y);

		if (w < minx) w = minx;
		if (h < miny) h = miny;

		// Update the clipping range
		baseClipRegion = new Vector4(newX, newY, w, h);
	}

	static int mUpdateFrame = -1;

	/// <summary>
	/// Update all panels and draw calls.
	/// </summary>

	void LateUpdate ()
	{
#if UNITY_EDITOR
		if (mUpdateFrame != Time.frameCount || !Application.isPlaying)
#else
		if (mUpdateFrame != Time.frameCount)
#endif
		{
			mUpdateFrame = Time.frameCount;

			// Update each panel in order
			for (int i = 0, imax = list.Count; i < imax; ++i)
				list[i].UpdateSelf();

			int rq = 3000;

			// Update all draw calls, making them draw in the right order
			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				UIPanel p = list[i];

				if (p.renderQueue == RenderQueue.Automatic)
				{
					p.startingRenderQueue = rq;
					p.UpdateDrawCalls(i);
					rq += p.drawCalls.Count;
				}
				else if (p.renderQueue == RenderQueue.StartAt)
				{
					p.UpdateDrawCalls(i);
					if (p.drawCalls.Count != 0)
						rq = Mathf.Max(rq, p.startingRenderQueue + p.drawCalls.Count);
				}
				else // Explicit
				{
					p.UpdateDrawCalls(i);
					if (p.drawCalls.Count != 0)
						rq = Mathf.Max(rq, p.startingRenderQueue + 1);
				}
			}
		}
	}

	[System.NonSerialized]
	bool mHasMoved = false;

	/// <summary>
	/// Update the panel, all of its widgets and draw calls.
	/// </summary>

	void UpdateSelf ()
	{
		mHasMoved = cachedTransform.hasChanged;

		UpdateTransformMatrix();
		UpdateLayers();
		UpdateWidgets();

		if (mRebuild)
		{
			mRebuild = false;
			FillAllDrawCalls();
		}
		else
		{
			for (int i = 0; i < drawCalls.Count; )
			{
				UIDrawCall dc = drawCalls[i];

				if (dc.isDirty && !FillDrawCall(dc))
				{
					UIDrawCall.Destroy(dc);
					drawCalls.RemoveAt(i);
					continue;
				}
				++i;
			}
		}

		if (mUpdateScroll)
		{
			mUpdateScroll = false;
			UIScrollView sv = GetComponent<UIScrollView>();
			if (sv != null) sv.UpdateScrollbars();
		}

		if (mHasMoved)
		{
			mHasMoved = false;
			mTrans.hasChanged = false;
		}
	}

	/// <summary>
	/// Immediately sort all child widgets.
	/// </summary>

	public void SortWidgets ()
	{
		mSortWidgets = false;
		widgets.Sort(UIWidget.PanelCompareFunc);
	}

	/// <summary>
	/// Fill the geometry fully, processing all widgets and re-creating all draw calls.
	/// </summary>

	void FillAllDrawCalls ()
	{
		for (int i = 0; i < drawCalls.Count; ++i)
			UIDrawCall.Destroy(drawCalls[i]);
		drawCalls.Clear();

		Material mat = null;
		Texture tex = null;
		Shader sdr = null;
		UIDrawCall dc = null;
		int count = 0;

		if (mSortWidgets) SortWidgets();

		for (int i = 0; i < widgets.Count; ++i)
		{
			UIWidget w = widgets[i];

			if (w.isVisible && w.hasVertices)
			{
				Material mt = w.material;
				
				if (onCreateMaterial != null) mt = onCreateMaterial(w, mt);

				Texture tx = w.mainTexture;
				Shader sd = w.shader;

				if (mat != mt || tex != tx || sdr != sd)
				{
					if (dc != null && dc.verts.Count != 0)
					{
						drawCalls.Add(dc);
						dc.UpdateGeometry(count);
						dc.onRender = mOnRender;
						mOnRender = null;
						count = 0;
						dc = null;
					}

					mat = mt;
					tex = tx;
					sdr = sd;
				}

				if (mat != null || sdr != null || tex != null)
				{
					if (dc == null)
					{
						dc = UIDrawCall.Create(this, mat, tex, sdr);
						dc.depthStart = w.depth;
						dc.depthEnd = dc.depthStart;
						dc.panel = this;
						dc.onCreateDrawCall = onCreateDrawCall;
					}
					else
					{
						int rd = w.depth;
						if (rd < dc.depthStart) dc.depthStart = rd;
						if (rd > dc.depthEnd) dc.depthEnd = rd;
					}

					w.drawCall = dc;

					++count;
					if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans, generateUV2 ? dc.uv2 : null);
					else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null, generateUV2 ? dc.uv2 : null);

					if (w.mOnRender != null)
					{
						if (mOnRender == null) mOnRender = w.mOnRender;
						else mOnRender += w.mOnRender;
					}
				}
			}
			else w.drawCall = null;
		}

		if (dc != null && dc.verts.Count != 0)
		{
			drawCalls.Add(dc);
			dc.UpdateGeometry(count);
			dc.onRender = mOnRender;
			mOnRender = null;
		}
	}

	UIDrawCall.OnRenderCallback mOnRender;

	/// <summary>
	/// Fill the geometry for the specified draw call.
	/// </summary>

	public bool FillDrawCall (UIDrawCall dc)
	{
		if (dc != null)
		{
			dc.isDirty = false;
			int count = 0;

			for (int i = 0; i < widgets.Count; )
			{
				UIWidget w = widgets[i];

				if (w == null)
				{
#if UNITY_EDITOR
					Debug.LogError("This should never happen");
#endif
					widgets.RemoveAt(i);
					continue;
				}

				if (w.drawCall == dc)
				{
					if (w.isVisible && w.hasVertices)
					{
						++count;
						
						if (generateNormals) w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans, generateUV2 ? dc.uv2 : null);
						else w.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null, generateUV2 ? dc.uv2 : null);

						if (w.mOnRender != null)
						{
							if (mOnRender == null) mOnRender = w.mOnRender;
							else mOnRender += w.mOnRender;
						}
					}
					else w.drawCall = null;
				}
				++i;
			}

			if (dc.verts.Count != 0)
			{
				dc.UpdateGeometry(count);
				dc.onRender = mOnRender;
				mOnRender = null;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Update all draw calls associated with the panel.
	/// </summary>

	void UpdateDrawCalls (int sortOrder)
	{
		Transform trans = cachedTransform;
		bool isUI = usedForUI;

		if (clipping != UIDrawCall.Clipping.None)
		{
			drawCallClipRange = finalClipRegion;
			drawCallClipRange.z *= 0.5f;
			drawCallClipRange.w *= 0.5f;
		}
		else drawCallClipRange = Vector4.zero;

		int w = Screen.width;
		int h = Screen.height;

		// Legacy functionality
		if (drawCallClipRange.z == 0f) drawCallClipRange.z = w * 0.5f;
		if (drawCallClipRange.w == 0f) drawCallClipRange.w = h * 0.5f;

		// DirectX 9 half-pixel offset
		if (halfPixelOffset)
		{
			drawCallClipRange.x -= 0.5f;
			drawCallClipRange.y += 0.5f;
		}

		Vector3 pos;

		if (isUI)
		{
			Transform parent = cachedTransform.parent;
			pos = cachedTransform.localPosition;

			if (clipping != UIDrawCall.Clipping.None)
			{
				pos.x = Mathf.RoundToInt(pos.x);
				pos.y = Mathf.RoundToInt(pos.y);
			}

			if (parent != null) pos = parent.TransformPoint(pos);
			pos += drawCallOffset;
		}
		else pos = trans.position;

		Quaternion rot = trans.rotation;
		Vector3 scale = trans.lossyScale;

		for (int i = 0; i < drawCalls.Count; ++i)
		{
			UIDrawCall dc = drawCalls[i];

			Transform t = dc.cachedTransform;
			t.position = pos;
			t.rotation = rot;
			t.localScale = scale;

			dc.renderQueue = (renderQueue == RenderQueue.Explicit) ? startingRenderQueue : startingRenderQueue + i;
			dc.alwaysOnScreen = alwaysOnScreen &&
				(mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip);
			dc.sortingOrder = (mSortingOrder == 0) ? sortOrder : mSortingOrder;
			dc.sortingLayerName = mSortingLayerName;
			dc.clipTexture = mClipTexture;
#if !UNITY_4_7
			dc.shadowMode = shadowMode;
#endif
		}
	}

	/// <summary>
	/// Update the widget layers if the panel's layer has changed.
	/// </summary>

	void UpdateLayers ()
	{
		// Always move widgets to the panel's layer
		if (mLayer != cachedGameObject.layer)
		{
			mLayer = mGo.layer;

			for (int i = 0, imax = widgets.Count; i < imax; ++i)
			{
				UIWidget w = widgets[i];
				if (w && w.parent == this) w.gameObject.layer = mLayer;
			}

			ResetAnchors();

			for (int i = 0; i < drawCalls.Count; ++i)
				drawCalls[i].gameObject.layer = mLayer;
		}
	}

	bool mForced = false;

	/// <summary>
	/// Update all of the widgets belonging to this panel.
	/// </summary>

	void UpdateWidgets()
	{
		bool changed = false;
		bool forceVisible = false;
		bool clipped = hasCumulativeClipping;

		if (!cullWhileDragging)
		{
			for (int i = 0; i < UIScrollView.list.size; ++i)
			{
				UIScrollView sv = UIScrollView.list[i];
				if (sv.panel == this && sv.isDragging) forceVisible = true;
			}
		}

		if (mForced != forceVisible)
		{
			mForced = forceVisible;
			mResized = true;
		}

		// Update all widgets
		int frame = Time.frameCount;
		for (int i = 0, imax = widgets.Count; i < imax; ++i)
		{
			UIWidget w = widgets[i];

			// If the widget is visible, update it
			if (w.panel == this && w.enabled)
			{
#if UNITY_EDITOR
				// When an object is dragged from Project view to Scene view, its Z is...
				// odd, to say the least. Force it if possible.
				if (!Application.isPlaying)
				{
					Transform t = w.cachedTransform;

					if (t.hideFlags != HideFlags.HideInHierarchy)
					{
						t = (t.parent != null && t.parent.hideFlags == HideFlags.HideInHierarchy) ?
							t.parent : null;
					}

					if (t != null)
					{
						for (; ; )
						{
							if (t.parent == null) break;
							if (t.parent.hideFlags == HideFlags.HideInHierarchy) t = t.parent;
							else break;
						}

						if (t != null)
						{
							Vector3 pos = t.localPosition;
							pos.x = Mathf.Round(pos.x);
							pos.y = Mathf.Round(pos.y);
							pos.z = 0f;

							if (Vector3.SqrMagnitude(t.localPosition - pos) > 0.0001f)
								t.localPosition = pos;
						}
					}
				}
#endif
				

				// First update the widget's transform
				if (w.UpdateTransform(frame) || mResized || (mHasMoved && !alwaysOnScreen))
				{
					// Only proceed to checking the widget's visibility if it actually moved
					bool vis = forceVisible || (w.CalculateCumulativeAlpha(frame) > 0.001f);
					w.UpdateVisibility(vis, forceVisible || alwaysOnScreen || ((clipped || w.hideIfOffScreen) ? IsVisible(w) : true));
				}
				
				// Update the widget's geometry if necessary
				if (w.UpdateGeometry(frame))
				{
					changed = true;
					//Debug.Log("Geometry changed: " + w.name + " " + frame, w);

					if (!mRebuild)
					{
						// Find an existing draw call, if possible
						if (w.drawCall != null) w.drawCall.isDirty = true;
						else FindDrawCall(w);
					}
				}
			}
		}

		// Inform the changed event listeners
		if (changed && onGeometryUpdated != null) onGeometryUpdated();
		mResized = false;
	}

	/// <summary>
	/// Insert the specified widget into one of the existing draw calls if possible.
	/// If it's not possible, and a new draw call is required, 'null' is returned
	/// because draw call creation is a delayed operation.
	/// </summary>

	public UIDrawCall FindDrawCall (UIWidget w)
	{
		Material mat = w.material;
		Texture tex = w.mainTexture;
		int depth = w.depth;

		for (int i = 0; i < drawCalls.Count; ++i)
		{
			UIDrawCall dc = drawCalls[i];
			int dcStart = (i == 0) ? int.MinValue : drawCalls[i - 1].depthEnd + 1;
			int dcEnd = (i + 1 == drawCalls.Count) ? int.MaxValue : drawCalls[i + 1].depthStart - 1;

			if (dcStart <= depth && dcEnd >= depth)
			{
				if (dc.baseMaterial == mat && dc.mainTexture == tex)
				{
					if (w.isVisible)
					{
						w.drawCall = dc;
						if (w.hasVertices) dc.isDirty = true;
						return dc;
					}
				}
				else mRebuild = true;
				return null;
			}
		}
		mRebuild = true;
		return null;
	}

	/// <summary>
	/// Make the following widget be managed by the panel.
	/// </summary>

	public void AddWidget (UIWidget w)
	{
		mUpdateScroll = true;

		if (widgets.Count == 0)
		{
			widgets.Add(w);
		}
		else if (mSortWidgets)
		{
			widgets.Add(w);
			SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, widgets[0]) == -1)
		{
			widgets.Insert(0, w);
		}
		else
		{
			for (int i = widgets.Count; i > 0; )
			{
				if (UIWidget.PanelCompareFunc(w, widgets[--i]) == -1) continue;
				widgets.Insert(i+1, w);
				break;
			}
		}
		FindDrawCall(w);
	}

	/// <summary>
	/// Remove the widget from its current draw call, invalidating everything as needed.
	/// </summary>

	public void RemoveWidget (UIWidget w)
	{
		if (widgets.Remove(w) && w.drawCall != null)
		{
			int depth = w.depth;
			if (depth == w.drawCall.depthStart || depth == w.drawCall.depthEnd)
				mRebuild = true;

			w.drawCall.isDirty = true;
			w.drawCall = null;
		}
	}

	/// <summary>
	/// Immediately refresh the panel.
	/// </summary>

	public void Refresh ()
	{
		mRebuild = true;
		mUpdateFrame = -1;
		if (list.Count > 0) list[0].LateUpdate();
	}

	/// <summary>
	/// Calculate the offset needed to be constrained within the panel's bounds.
	/// </summary>

	public virtual Vector3 CalculateConstrainOffset (Vector2 min, Vector2 max)
	{
		Vector4 cr = finalClipRegion;

		float offsetX = cr.z * 0.5f;
		float offsetY = cr.w * 0.5f;

		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(cr.x - offsetX, cr.y - offsetY);
		Vector2 maxArea = new Vector2(cr.x + offsetX, cr.y + offsetY);

		if (softBorderPadding && clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += mClipSoftness.x;
			minArea.y += mClipSoftness.y;
			maxArea.x -= mClipSoftness.x;
			maxArea.y -= mClipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	/// <summary>
	/// Constrain the current target position to be within panel bounds.
	/// </summary>

	public bool ConstrainTargetToBounds (Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 min = targetBounds.min;
		Vector3 max = targetBounds.max;

		float ps = 1f;

		if (mClipping == UIDrawCall.Clipping.None)
		{
			UIRoot rt = root;
			if (rt != null) ps = rt.pixelSizeAdjustment;
		}

		if (ps != 1f)
		{
			min /= ps;
			max /= ps;
		}

		Vector3 offset = CalculateConstrainOffset(min, max) * ps;

		if (offset.sqrMagnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += offset;
				targetBounds.center += offset;
				SpringPosition sp = target.GetComponent<SpringPosition>();
				if (sp != null) sp.enabled = false;
			}
			else
			{
				SpringPosition sp = SpringPosition.Begin(target.gameObject, target.localPosition + offset, 13f);
				sp.ignoreTimeScale = true;
				sp.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Constrain the specified target to be within the panel's bounds.
	/// </summary>

	public bool ConstrainTargetToBounds (Transform target, bool immediate)
	{
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, target);
		return ConstrainTargetToBounds(target, ref bounds, immediate);
	}

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans) { return Find(trans, false, -1); }

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans, bool createIfMissing) { return Find(trans, createIfMissing, -1); }

	/// <summary>
	/// Find the UIPanel responsible for handling the specified transform.
	/// </summary>

	static public UIPanel Find (Transform trans, bool createIfMissing, int layer)
	{
		UIPanel panel = NGUITools.FindInParents<UIPanel>(trans);
		if (panel != null) return panel;
		while (trans.parent != null) trans = trans.parent;
		return createIfMissing ? NGUITools.CreateUI(trans, false, layer) : null;
	}

	/// <summary>
	/// Get the size of the game window in pixels.
	/// </summary>

	public Vector2 GetWindowSize ()
	{
		UIRoot rt = root;
		Vector2 size = NGUITools.screenSize;
		if (rt != null) size *= rt.GetPixelSizeAdjustment(Mathf.RoundToInt(size.y));
		return size;
	}

	/// <summary>
	/// Panel's size -- which is either the clipping rect, or the screen dimensions.
	/// </summary>

	public Vector2 GetViewSize ()
	{
		if (mClipping != UIDrawCall.Clipping.None)
			return new Vector2(mClipRange.z, mClipRange.w);
		
		Vector2 size = NGUITools.screenSize;
		//UIRoot rt = root;
		//if (rt != null) size *= rt.pixelSizeAdjustment;
		return size;
	}

#if UNITY_EDITOR
	/// <summary>
	/// Draw a visible pink outline for the clipped area.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (anchorCamera == null) return;

		bool clip = (mClipping != UIDrawCall.Clipping.None);
		Transform t = clip ? transform : mCam.transform;

		Vector3[] corners = worldCorners;
		for (int i = 0; i < 4; ++i) corners[i] = t.InverseTransformPoint(corners[i]);
		Vector3 pos = Vector3.Lerp(corners[0], corners[2], 0.5f);
		Vector3 size = corners[2] - corners[0];

		GameObject go = UnityEditor.Selection.activeGameObject;
		bool isUsingThisPanel = (go != null) && (NGUITools.FindInParents<UIPanel>(go) == this);
		bool isSelected = (UnityEditor.Selection.activeGameObject == gameObject);
		bool detailedView = (isSelected && isUsingThisPanel);
		bool detailedClipped = detailedView && mClipping == UIDrawCall.Clipping.SoftClip;

		Gizmos.matrix = t.localToWorldMatrix;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		if (isUsingThisPanel && !clip && mCam.isOrthoGraphic)
#else
		if (isUsingThisPanel && !clip && mCam.orthographic)
#endif
		{
			UIRoot rt = root;

			if (rt != null && rt.scalingStyle != UIRoot.Scaling.Flexible)
			{
				float width = rt.manualWidth;
				float height = rt.manualHeight;

				float x0 = -0.5f * width;
				float y0 = -0.5f * height;
				float x1 = x0 + width;
				float y1 = y0 + height;

				corners[0] = new Vector3(x0, y0);
				corners[1] = new Vector3(x0, y1);
				corners[2] = new Vector3(x1, y1);
				corners[3] = new Vector3(x1, y0);

				Vector3 szPos = Vector3.Lerp(corners[0], corners[2], 0.5f);
				Vector3 szSize = corners[2] - corners[0];

				Gizmos.color = new Color(0f, 0.75f, 1f);
				Gizmos.DrawWireCube(szPos, szSize);
			}
		}
		Gizmos.color = (isUsingThisPanel && !detailedClipped) ? new Color(1f, 0f, 0.5f) : new Color(0.5f, 0f, 0.5f);
		Gizmos.DrawWireCube(pos, size);

		if (detailedView)
		{
			if (detailedClipped)
			{
				Gizmos.color = new Color(1f, 0f, 0.5f);
				size.x -= mClipSoftness.x * 2f;
				size.y -= mClipSoftness.y * 2f;
				Gizmos.DrawWireCube(pos, size);
			}
		}
	}
#endif // UNITY_EDITOR
}
