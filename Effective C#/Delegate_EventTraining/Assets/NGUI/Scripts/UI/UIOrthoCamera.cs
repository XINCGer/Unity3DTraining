//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Convenience script that resizes the camera's orthographic size to match the screen size.
/// This script can be used to create pixel-perfect UI, however it's usually more convenient
/// to create the UI that stays proportional as the screen scales. If that is what you
/// want, you don't need this script (or at least don't need it to be active).
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/UI/Orthographic Camera")]
public class UIOrthoCamera : MonoBehaviour
{
	Camera mCam;
	Transform mTrans;

	void Start ()
	{
		mCam = GetComponent<Camera>();
		mTrans = transform;
		mCam.orthographic = true;
	}

	void Update ()
	{
		float y0 = mCam.rect.yMin * Screen.height;
		float y1 = mCam.rect.yMax * Screen.height;

		float size = (y1 - y0) * 0.5f * mTrans.lossyScale.y;
		if (!Mathf.Approximately(mCam.orthographicSize, size)) mCam.orthographicSize = size;
	}
}