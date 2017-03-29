//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Simple progress bar that fills itself based on the specified value.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
public class UIProgressBar : UIWidgetContainer
{
	public enum FillDirection
	{
		LeftToRight,
		RightToLeft,
		BottomToTop,
		TopToBottom,
	}

	/// <summary>
	/// Current slider. This value is set prior to the callback function being triggered.
	/// </summary>

	static public UIProgressBar current;

	/// <summary>
	/// Delegate triggered when the scroll bar stops being dragged.
	/// Useful for things like centering on the closest valid object, for example.
	/// </summary>

	public OnDragFinished onDragFinished;
	public delegate void OnDragFinished ();

	/// <summary>
	/// Object that acts as a thumb.
	/// </summary>

	public Transform thumb;

	[HideInInspector][SerializeField] protected UIWidget mBG;
	[HideInInspector][SerializeField] protected UIWidget mFG;
	[HideInInspector][SerializeField] protected float mValue = 1f;
	[HideInInspector][SerializeField] protected FillDirection mFill = FillDirection.LeftToRight;

	[System.NonSerialized] protected bool mStarted = false;
	[System.NonSerialized] protected Transform mTrans;
	[System.NonSerialized] protected bool mIsDirty = false;
	[System.NonSerialized] protected Camera mCam;
	[System.NonSerialized] protected float mOffset = 0f;

	/// <summary>
	/// Number of steps the slider should be divided into. For example 5 means possible values of 0, 0.25, 0.5, 0.75, and 1.0.
	/// </summary>

	public int numberOfSteps = 0;

	/// <summary>
	/// Callbacks triggered when the scroll bar's value changes.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

	/// <summary>
	/// Cached for speed.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	/// <summary>
	/// Camera used to draw the scroll bar.
	/// </summary>

	public Camera cachedCamera { get { if (mCam == null) mCam = NGUITools.FindCameraForLayer(gameObject.layer); return mCam; } }

	/// <summary>
	/// Widget used for the foreground.
	/// </summary>

	public UIWidget foregroundWidget { get { return mFG; } set { if (mFG != value) { mFG = value; mIsDirty = true; } } }

	/// <summary>
	/// Widget used for the background.
	/// </summary>

	public UIWidget backgroundWidget { get { return mBG; } set { if (mBG != value) { mBG = value; mIsDirty = true; } } }

	/// <summary>
	/// The scroll bar's direction.
	/// </summary>

	public FillDirection fillDirection
	{
		get
		{
			return mFill;
		}
		set
		{
			if (mFill != value)
			{
				mFill = value;
				if (mStarted) ForceUpdate();
			}
		}
	}

	/// <summary>
	/// Modifiable value for the scroll bar, 0-1 range.
	/// </summary>

	public float value
	{
		get
		{
			if (numberOfSteps > 1) return Mathf.Round(mValue * (numberOfSteps - 1)) / (numberOfSteps - 1);
			return mValue;
		}
		set { Set(value); }
	}

	/// <summary>
	/// Allows to easily change the scroll bar's alpha, affecting both the foreground and the background sprite at once.
	/// </summary>

	public float alpha
	{
		get
		{
			if (mFG != null) return mFG.alpha;
			if (mBG != null) return mBG.alpha;
			return 1f;
		}
		set
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (mFG != null)
			{
				mFG.alpha = value;
				if (mFG.collider != null) mFG.collider.enabled = mFG.alpha > 0.001f;
				else if (mFG.GetComponent<Collider2D>() != null) mFG.GetComponent<Collider2D>().enabled = mFG.alpha > 0.001f;
			}

			if (mBG != null)
			{
				mBG.alpha = value;
				if (mBG.collider != null) mBG.collider.enabled = mBG.alpha > 0.001f;
				else if (mBG.GetComponent<Collider2D>() != null) mBG.GetComponent<Collider2D>().enabled = mBG.alpha > 0.001f;
			}

			if (thumb != null)
			{
				UIWidget w = thumb.GetComponent<UIWidget>();
				
				if (w != null)
				{
					w.alpha = value;
					if (w.collider != null) w.collider.enabled = w.alpha > 0.001f;
					else if (w.GetComponent<Collider2D>() != null) w.GetComponent<Collider2D>().enabled = w.alpha > 0.001f;
				}
			}
#else
			if (mFG != null)
			{
				mFG.alpha = value;
				if (mFG.GetComponent<Collider>() != null) mFG.GetComponent<Collider>().enabled = mFG.alpha > 0.001f;
				else if (mFG.GetComponent<Collider2D>() != null) mFG.GetComponent<Collider2D>().enabled = mFG.alpha > 0.001f;
			}

			if (mBG != null)
			{
				mBG.alpha = value;
				if (mBG.GetComponent<Collider>() != null) mBG.GetComponent<Collider>().enabled = mBG.alpha > 0.001f;
				else if (mBG.GetComponent<Collider2D>() != null) mBG.GetComponent<Collider2D>().enabled = mBG.alpha > 0.001f;
			}

			if (thumb != null)
			{
				UIWidget w = thumb.GetComponent<UIWidget>();
				
				if (w != null)
				{
					w.alpha = value;
					if (w.GetComponent<Collider>() != null) w.GetComponent<Collider>().enabled = w.alpha > 0.001f;
					else if (w.GetComponent<Collider2D>() != null) w.GetComponent<Collider2D>().enabled = w.alpha > 0.001f;
				}
			}
#endif
		}
	}

	/// <summary>
	/// Whether the progress bar is horizontal in nature. Convenience function.
	/// </summary>

	protected bool isHorizontal { get { return (mFill == FillDirection.LeftToRight || mFill == FillDirection.RightToLeft); } }

	/// <summary>
	/// Whether the progress bar is inverted in its behaviour. Convenience function.
	/// </summary>

	protected bool isInverted { get { return (mFill == FillDirection.RightToLeft || mFill == FillDirection.TopToBottom); } }

	/// <summary>
	/// Set the progress bar's value. If setting the initial value, call Start() first.
	/// </summary>

	public void Set (float val, bool notify = true)
	{
		val = Mathf.Clamp01(val);

		if (mValue != val)
		{
			float before = value;
			mValue = val;

			if (mStarted && before != value)
			{
				if (notify && NGUITools.GetActive(this) && EventDelegate.IsValid(onChange))
				{
					current = this;
					EventDelegate.Execute(onChange);
					current = null;
				}

				ForceUpdate();
			}
#if UNITY_EDITOR
			if (!Application.isPlaying)
				NGUITools.SetDirty(this);
#endif
		}
	}

	/// <summary>
	/// Register the event listeners.
	/// </summary>

	public void Start ()
	{
		if (mStarted) return;
		mStarted = true;
		Upgrade();

		if (Application.isPlaying)
		{
			if (mBG != null) mBG.autoResizeBoxCollider = true;

			OnStart();

			if (current == null && onChange != null)
			{
				current = this;
				EventDelegate.Execute(onChange);
				current = null;
			}
		}
		ForceUpdate();
	}

	/// <summary>
	/// Used to upgrade from legacy functionality.
	/// </summary>

	protected virtual void Upgrade () { }

	/// <summary>
	/// Functionality for derived classes.
	/// </summary>

	protected virtual void OnStart() { }

	/// <summary>
	/// Update the value of the scroll bar if necessary.
	/// </summary>

	protected void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		if (mIsDirty) ForceUpdate();
	}

	/// <summary>
	/// Invalidate the scroll bar.
	/// </summary>

	protected void OnValidate ()
	{
		// For some bizarre reason Unity calls this function on prefabs, even if prefabs
		// are not actually used in the scene, nor selected in inspector. Dafuq?
		if (NGUITools.GetActive(this))
		{
			Upgrade();
			mIsDirty = true;
			float val = Mathf.Clamp01(mValue);
			if (mValue != val) mValue = val;
			if (numberOfSteps < 0) numberOfSteps = 0;
			else if (numberOfSteps > 21) numberOfSteps = 21;
			ForceUpdate();
		}
		else
		{
			float val = Mathf.Clamp01(mValue);
			if (mValue != val) mValue = val;
			if (numberOfSteps < 0) numberOfSteps = 0;
			else if (numberOfSteps > 21) numberOfSteps = 21;
		}
	}

	/// <summary>
	/// Drag the scroll bar by the specified on-screen amount.
	/// </summary>

	protected float ScreenToValue (Vector2 screenPos)
	{
		// Create a plane
		Transform trans = cachedTransform;
		Plane plane = new Plane(trans.rotation * Vector3.back, trans.position);

		// If the ray doesn't hit the plane, do nothing
		float dist;
		Ray ray = cachedCamera.ScreenPointToRay(screenPos);
		if (!plane.Raycast(ray, out dist)) return value;

		// Transform the point from world space to local space
		return LocalToValue(trans.InverseTransformPoint(ray.GetPoint(dist)));
	}

	/// <summary>
	/// Calculate the value of the progress bar given the specified local position.
	/// </summary>

	protected virtual float LocalToValue (Vector2 localPos)
	{
		if (mFG != null)
		{
			Vector3[] corners = mFG.localCorners;
			Vector3 size = (corners[2] - corners[0]);

			if (isHorizontal)
			{
				float diff = (localPos.x - corners[0].x) / size.x;
				return isInverted ? 1f - diff : diff;
			}
			else
			{
				float diff = (localPos.y - corners[0].y) / size.y;
				return isInverted ? 1f - diff : diff;
			}
		}
		return value;
	}

	/// <summary>
	/// Update the value of the scroll bar.
	/// </summary>

	public virtual void ForceUpdate ()
	{
		mIsDirty = false;
		bool turnOff = false;

		if (mFG != null)
		{
			UIBasicSprite sprite = mFG as UIBasicSprite;

			if (isHorizontal)
			{
				if (sprite != null && sprite.type == UIBasicSprite.Type.Filled)
				{
					if (sprite.fillDirection == UIBasicSprite.FillDirection.Horizontal ||
						sprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
					{
						sprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
						sprite.invert = isInverted;
					}
					sprite.fillAmount = value;
				}
				else
				{
					mFG.drawRegion = isInverted ?
						new Vector4(1f - value, 0f, 1f, 1f) :
						new Vector4(0f, 0f, value, 1f);
					mFG.enabled = true;
					turnOff = value < 0.001f;
				}
			}
			else if (sprite != null && sprite.type == UIBasicSprite.Type.Filled)
			{
				if (sprite.fillDirection == UIBasicSprite.FillDirection.Horizontal ||
					sprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
				{
					sprite.fillDirection = UIBasicSprite.FillDirection.Vertical;
					sprite.invert = isInverted;
				}
				sprite.fillAmount = value;
			}
			else
			{
				mFG.drawRegion = isInverted ?
					new Vector4(0f, 1f - value, 1f, 1f) :
					new Vector4(0f, 0f, 1f, value);
				mFG.enabled = true;
				turnOff = value < 0.001f;
			}
		}

		if (thumb != null && (mFG != null || mBG != null))
		{
			Vector3[] corners = (mFG != null) ? mFG.localCorners : mBG.localCorners;

			Vector4 br = (mFG != null) ? mFG.border : mBG.border;
			corners[0].x += br.x;
			corners[1].x += br.x;
			corners[2].x -= br.z;
			corners[3].x -= br.z;

			corners[0].y += br.y;
			corners[1].y -= br.w;
			corners[2].y -= br.w;
			corners[3].y += br.y;

			Transform t = (mFG != null) ? mFG.cachedTransform : mBG.cachedTransform;
			for (int i = 0; i < 4; ++i) corners[i] = t.TransformPoint(corners[i]);

			if (isHorizontal)
			{
				Vector3 v0 = Vector3.Lerp(corners[0], corners[1], 0.5f);
				Vector3 v1 = Vector3.Lerp(corners[2], corners[3], 0.5f);
				SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
			}
			else
			{
				Vector3 v0 = Vector3.Lerp(corners[0], corners[3], 0.5f);
				Vector3 v1 = Vector3.Lerp(corners[1], corners[2], 0.5f);
				SetThumbPosition(Vector3.Lerp(v0, v1, isInverted ? 1f - value : value));
			}
		}

		if (turnOff) mFG.enabled = false;
	}

	/// <summary>
	/// Set the position of the thumb to the specified world coordinates.
	/// </summary>

	protected void SetThumbPosition (Vector3 worldPos)
	{
		Transform t = thumb.parent;

		if (t != null)
		{
			worldPos = t.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;

			if (Vector3.Distance(thumb.localPosition, worldPos) > 0.001f)
				thumb.localPosition = worldPos;
		}
		else if (Vector3.Distance(thumb.position, worldPos) > 0.00001f)
			thumb.position = worldPos;
	}

	/// <summary>
	/// Watch for key events and adjust the value accordingly.
	/// </summary>

	public virtual void OnPan (Vector2 delta)
	{
		if (enabled)
		{
			switch (mFill)
			{
				case FillDirection.LeftToRight:
				{
					float after = Mathf.Clamp01(mValue + delta.x);
					value = after;
					mValue = after;
					break;
				}
				case FillDirection.RightToLeft:
				{
					float after = Mathf.Clamp01(mValue - delta.x);
					value = after;
					mValue = after;
					break;
				}
				case FillDirection.BottomToTop:
				{
					float after = Mathf.Clamp01(mValue + delta.y);
					value = after;
					mValue = after;
					break;
				}
				case FillDirection.TopToBottom:
				{
					float after = Mathf.Clamp01(mValue - delta.y);
					value = after;
					mValue = after;
					break;
				}
			}
		}
	}
}
