//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this script to a UITexture to turn it into a color picker.
/// The color picking texture will be generated automatically.
/// </summary>

[RequireComponent(typeof(UITexture))]
public class UIColorPicker : MonoBehaviour
{
	static public UIColorPicker current;

	/// <summary>
	/// Color picker's current value.
	/// </summary>

	public Color value = Color.white;

	/// <summary>
	/// Widget that will be positioned above the current color selection. This value is optional.
	/// </summary>

	public UIWidget selectionWidget;

	/// <summary>
	/// Delegate that will be called when the color picker is being interacted with.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[System.NonSerialized] Transform mTrans;
	[System.NonSerialized] UITexture mUITex;
	[System.NonSerialized] Texture2D mTex;
	[System.NonSerialized] UICamera mCam;
	[System.NonSerialized] Vector2 mPos;
	[System.NonSerialized] int mWidth = 0;
	[System.NonSerialized] int mHeight = 0;

	void Start ()
	{
		mTrans = transform;
		mUITex = GetComponent<UITexture>();
		mCam = UICamera.FindCameraForLayer(gameObject.layer);

		mWidth = mUITex.width;
		mHeight = mUITex.height;
		Color[] cols = new Color[mWidth * mHeight];

		for (int y = 0; y < mHeight; ++y)
		{
			float fy = (y - 1f) / mHeight;

			for (int x = 0; x < mWidth; ++x)
			{
				float fx = (x - 1f) / mWidth;
				int index = x + y * mWidth;
				cols[index] = Sample(fx, fy);
			}
		}

		mTex = new Texture2D(mWidth, mHeight, TextureFormat.RGB24, false);
		mTex.SetPixels(cols);
		mTex.filterMode = FilterMode.Trilinear;
		mTex.wrapMode = TextureWrapMode.Clamp;
		mTex.Apply();
		mUITex.mainTexture = mTex;

		Select(value);
	}

	void OnDestroy ()
	{
		Destroy(mTex);
		mTex = null;
	}

	void OnPress (bool pressed) { if (enabled && pressed && UICamera.currentScheme != UICamera.ControlScheme.Controller) Sample(); }
	void OnDrag (Vector2 delta) { if (enabled) Sample(); }
	void OnPan (Vector2 delta)
	{
		if (enabled)
		{
			mPos.x = Mathf.Clamp01(mPos.x + delta.x);
			mPos.y = Mathf.Clamp01(mPos.y + delta.y);
			Select(mPos);
		}
	}

	/// <summary>
	/// Sample the color under the current event position.
	/// </summary>

	void Sample ()
	{
		Vector3 pos = UICamera.lastEventPosition;
		pos = mCam.cachedCamera.ScreenToWorldPoint(pos);

		pos = mTrans.InverseTransformPoint(pos);
		Vector3[] corners = mUITex.localCorners;
		mPos.x = Mathf.Clamp01((pos.x - corners[0].x) / (corners[2].x - corners[0].x));
		mPos.y = Mathf.Clamp01((pos.y - corners[0].y) / (corners[2].y - corners[0].y));

		if (selectionWidget != null)
		{
			pos.x = Mathf.Lerp(corners[0].x, corners[2].x, mPos.x);
			pos.y = Mathf.Lerp(corners[0].y, corners[2].y, mPos.y);
			pos = mTrans.TransformPoint(pos);
			selectionWidget.transform.OverlayPosition(pos, mCam.cachedCamera);
		}

		value = Sample(mPos.x, mPos.y);
		current = this;
		EventDelegate.Execute(onChange);
		current = null;
	}

	/// <summary>
	/// Select the color under the specified relative coordinate.
	/// </summary>

	public void Select (Vector2 v)
	{
		v.x = Mathf.Clamp01(v.x);
		v.y = Mathf.Clamp01(v.y);
		mPos = v;

		if (selectionWidget != null)
		{
			Vector3[] corners = mUITex.localCorners;
			v.x = Mathf.Lerp(corners[0].x, corners[2].x, mPos.x);
			v.y = Mathf.Lerp(corners[0].y, corners[2].y, mPos.y);
			v = mTrans.TransformPoint(v);
			selectionWidget.transform.OverlayPosition(v, mCam.cachedCamera);
		}

		value = Sample(mPos.x, mPos.y);
		current = this;
		EventDelegate.Execute(onChange);
		current = null;
	}

	/// <summary>
	/// Select the specified color.
	/// </summary>

	public Vector2 Select (Color c)
	{
		if (mUITex == null)
		{
			value = c;
			return mPos;
		}

		float closest = float.MaxValue;

		for (int y = 0; y < mHeight; ++y)
		{
			float fy = (y - 1f) / mHeight;

			for (int x = 0; x < mWidth; ++x)
			{
				float fx = (x - 1f) / mWidth;
				Color sam = Sample(fx, fy);
				Color sc = sam;
				sc.r -= c.r;
				sc.g -= c.g;
				sc.b -= c.b;
				float dot = sc.r * sc.r + sc.g * sc.g + sc.b * sc.b;
				
				if (dot < closest)
				{
					closest = dot;
					mPos.x = fx;
					mPos.y = fy;
				}
			}
		}

		if (selectionWidget != null)
		{
			Vector3[] corners = mUITex.localCorners;
			Vector3 pos;
			pos.x = Mathf.Lerp(corners[0].x, corners[2].x, mPos.x);
			pos.y = Mathf.Lerp(corners[0].y, corners[2].y, mPos.y);
			pos.z = 0f;
			pos = mTrans.TransformPoint(pos);
			selectionWidget.transform.OverlayPosition(pos, mCam.cachedCamera);
		}

		value = c;

		current = this;
		EventDelegate.Execute(onChange);
		current = null;
		return mPos;
	}

	static AnimationCurve mRed;
	static AnimationCurve mGreen;
	static AnimationCurve mBlue;

	/// <summary>
	/// Choose a color, given X and Y in 0-1 range.
	/// </summary>

	static public Color Sample (float x, float y)
	{
		if (mRed == null)
		{
			mRed = new AnimationCurve(
				new Keyframe[]
				{
					new Keyframe(0f, 1f),		// Red
					new Keyframe(1f / 7f, 1f),	// Yellow
					new Keyframe(2f / 7f, 0f),	// Green
					new Keyframe(3f / 7f, 0f),	// Cyan
					new Keyframe(4f / 7f, 0f),	// Blue
					new Keyframe(5f / 7f, 1f),	// Magenta
					new Keyframe(6f / 7f, 1f),	// Red
					new Keyframe(1f, 0.5f),		// Gray
				});

			mGreen = new AnimationCurve(
				new Keyframe[]
				{
					new Keyframe(0f, 0f),		// Red
					new Keyframe(1f / 7f, 1f),	// Yellow
					new Keyframe(2f / 7f, 1f),	// Green
					new Keyframe(3f / 7f, 1f),	// Cyan
					new Keyframe(4f / 7f, 0f),	// Blue
					new Keyframe(5f / 7f, 0f),	// Magenta
					new Keyframe(6f / 7f, 0f),	// Red
					new Keyframe(1f, 0.5f),		// Gray
				});

			mBlue = new AnimationCurve(
				new Keyframe[]
				{
					new Keyframe(0f, 0f),		// Red
					new Keyframe(1f / 7f, 0f),	// Yellow
					new Keyframe(2f / 7f, 0f),	// Green
					new Keyframe(3f / 7f, 1f),	// Cyan
					new Keyframe(4f / 7f, 1f),	// Blue
					new Keyframe(5f / 7f, 1f),	// Magenta
					new Keyframe(6f / 7f, 0f),	// Red
					new Keyframe(1f, 0.5f),		// Gray
				});
		}

		Vector3 v = new Vector3(mRed.Evaluate(x), mGreen.Evaluate(x), mBlue.Evaluate(x));

		if (y < 0.5f)
		{
			y *= 2f;
			v.x *= y;
			v.y *= y;
			v.z *= y;
		}
		else v = Vector3.Lerp(v, Vector3.one, y * 2f - 1f);
		return new Color(v.x, v.y, v.z, 1f);
	}
}
