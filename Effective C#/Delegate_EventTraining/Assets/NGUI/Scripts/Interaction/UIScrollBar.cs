//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scroll bar functionality.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Scroll Bar")]
public class UIScrollBar : UISlider
{
	enum Direction
	{
		Horizontal,
		Vertical,
		Upgraded,
	}

	// Size of the scroll bar
	[HideInInspector][SerializeField] protected float mSize = 1f;

	// Deprecated functionality
	[HideInInspector][SerializeField] float mScroll = 0f;
	[HideInInspector][SerializeField] Direction mDir = Direction.Upgraded;

	[System.Obsolete("Use 'value' instead")]
	public float scrollValue { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// The size of the foreground bar in percent (0-1 range).
	/// </summary>

	public float barSize
	{
		get
		{
			return mSize;
		}
		set
		{
			float val = Mathf.Clamp01(value);

			if (mSize != val)
			{
				mSize = val;
				mIsDirty = true;

				if (NGUITools.GetActive(this))
				{
					if (current == null && onChange != null)
					{
						current = this;
						EventDelegate.Execute(onChange);
						current = null;
					}
					ForceUpdate();
#if UNITY_EDITOR
					if (!Application.isPlaying)
						NGUITools.SetDirty(this);
#endif
				}
			}
		}
	}

	/// <summary>
	/// Upgrade from legacy functionality.
	/// </summary>

	protected override void Upgrade ()
	{
		if (mDir != Direction.Upgraded)
		{
			mValue = mScroll;

			if (mDir == Direction.Horizontal)
			{
				mFill = mInverted ? FillDirection.RightToLeft : FillDirection.LeftToRight;
			}
			else
			{
				mFill = mInverted ? FillDirection.BottomToTop : FillDirection.TopToBottom;
			}
			mDir = Direction.Upgraded;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	/// <summary>
	/// Make the scroll bar's foreground react to press events.
	/// </summary>

	protected override void OnStart ()
	{
		base.OnStart();

		if (mFG != null && mFG.gameObject != gameObject)
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			bool hasCollider = (mFG.collider != null) || (mFG.GetComponent<Collider2D>() != null);
#else
			bool hasCollider = (mFG.GetComponent<Collider>() != null) || (mFG.GetComponent<Collider2D>() != null);
#endif
			if (!hasCollider) return;

			UIEventListener fgl = UIEventListener.Get(mFG.gameObject);
			fgl.onPress += OnPressForeground;
			fgl.onDrag += OnDragForeground;
			mFG.autoResizeBoxCollider = true;
		}
	}

	/// <summary>
	/// Move the scroll bar to be centered on the specified position.
	/// </summary>

	protected override float LocalToValue (Vector2 localPos)
	{
		if (mFG != null)
		{
			float halfSize = Mathf.Clamp01(mSize) * 0.5f;
			float val0 = halfSize;
			float val1 = 1f - halfSize;
			Vector3[] corners = mFG.localCorners;

			if (isHorizontal)
			{
				val0 = Mathf.Lerp(corners[0].x, corners[2].x, val0);
				val1 = Mathf.Lerp(corners[0].x, corners[2].x, val1);
				float diff = (val1 - val0);
				if (diff == 0f) return value;

				return isInverted ?
					(val1 - localPos.x) / diff :
					(localPos.x - val0) / diff;
			}
			else
			{
				val0 = Mathf.Lerp(corners[0].y, corners[1].y, val0);
				val1 = Mathf.Lerp(corners[3].y, corners[2].y, val1);
				float diff = (val1 - val0);
				if (diff == 0f) return value;

				return isInverted ?
					(val1 - localPos.y) / diff :
					(localPos.y - val0) / diff;
			}
		}
		return base.LocalToValue(localPos);
	}

	/// <summary>
	/// Update the value of the scroll bar.
	/// </summary>

	public override void ForceUpdate ()
	{
		if (mFG != null)
		{
			mIsDirty = false;

			float halfSize = Mathf.Clamp01(mSize) * 0.5f;
			float pos = Mathf.Lerp(halfSize, 1f - halfSize, value);
			float val0 = pos - halfSize;
			float val1 = pos + halfSize;

			if (isHorizontal)
			{
				mFG.drawRegion = isInverted ?
					new Vector4(1f - val1, 0f, 1f - val0, 1f) :
					new Vector4(val0, 0f, val1, 1f);
			}
			else
			{
				mFG.drawRegion = isInverted ?
					new Vector4(0f, 1f - val1, 1f, 1f - val0) :
					new Vector4(0f, val0, 1f, val1);
			}

			if (thumb != null)
			{
				Vector4 dr = mFG.drawingDimensions;
				Vector3 v = new Vector3(
					Mathf.Lerp(dr.x, dr.z, 0.5f),
					Mathf.Lerp(dr.y, dr.w, 0.5f));
				SetThumbPosition(mFG.cachedTransform.TransformPoint(v));
			}
		}
		else base.ForceUpdate();
	}
}
