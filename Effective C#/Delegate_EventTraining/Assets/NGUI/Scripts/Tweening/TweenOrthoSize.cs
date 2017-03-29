//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the camera's orthographic size.
/// </summary>

[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Tween/Tween Orthographic Size")]
public class TweenOrthoSize : UITweener
{
	public float from = 1f;
	public float to = 1f;

	Camera mCam;

	/// <summary>
	/// Camera that's being tweened.
	/// </summary>

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	public Camera cachedCamera { get { if (mCam == null) mCam = camera; return mCam; } }
#else
	public Camera cachedCamera { get { if (mCam == null) mCam = GetComponent<Camera>(); return mCam; } }
#endif

	[System.Obsolete("Use 'value' instead")]
	public float orthoSize { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get { return cachedCamera.orthographicSize; }
		set { cachedCamera.orthographicSize = value; }
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenOrthoSize Begin (GameObject go, float duration, float to)
	{
		TweenOrthoSize comp = UITweener.Begin<TweenOrthoSize>(go, duration);
		comp.from = comp.value;
		comp.to = to;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}
