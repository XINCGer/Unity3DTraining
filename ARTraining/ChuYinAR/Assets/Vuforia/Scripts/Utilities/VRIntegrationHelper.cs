/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved. Confidential and Proprietary - 
Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
===============================================================================*/


using System;
using UnityEngine;

using Vuforia;

public class VRIntegrationHelper : MonoBehaviour
{
    private static Matrix4x4 mLeftCameraMatrixOriginal;
    private static Matrix4x4 mRightCameraMatrixOriginal;

    private static Camera mLeftCamera;
    private static Camera mRightCamera;

    private static HideExcessAreaAbstractBehaviour mLeftExcessAreaBehaviour;
    private static HideExcessAreaAbstractBehaviour mRightExcessAreaBehaviour;

    private static Rect mLeftCameraPixelRect;
    private static Rect mRightCameraPixelRect;

    private static bool mLeftCameraDataAcquired = false;
    private static bool mRightCameraDataAcquired = false;

    public bool IsLeft;
    public Transform TrackableParent;

    void Awake()
    {
        GetComponent<Camera>().fieldOfView = 90f;
    }

    void Start()
    {
        VuforiaBehaviour.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
    }

    void OnVuforiaStarted()
    {
        mLeftCamera = DigitalEyewearBehaviour.Instance.PrimaryCamera;
        mRightCamera = DigitalEyewearBehaviour.Instance.SecondaryCamera;

        mLeftExcessAreaBehaviour = mLeftCamera.GetComponent<HideExcessAreaAbstractBehaviour>();
        mRightExcessAreaBehaviour = mRightCamera.GetComponent<HideExcessAreaAbstractBehaviour>();
    }

    void LateUpdate()
    {
        // to this only once per frame, not for both cameras
        if (IsLeft)
        {
            if (mLeftCameraDataAcquired && mRightCameraDataAcquired)
            {
                // make sure the central anchor point is set to the latest head tracking pose:
                DigitalEyewearBehaviour.Instance.CentralAnchorPoint.localRotation = mLeftCamera.transform.localRotation;
                DigitalEyewearBehaviour.Instance.CentralAnchorPoint.localPosition = mLeftCamera.transform.localPosition;

                // temporarily set the primary and secondary cameras to their offset position and set the pixelrect they will have for rendering
                Vector3 localPosLeftCam = mLeftCamera.transform.localPosition;
                Rect leftCamPixelRect = mLeftCamera.pixelRect;
                Vector3 leftCamOffset = mLeftCamera.transform.right.normalized * mLeftCamera.stereoSeparation * -0.5f;
                mLeftCamera.transform.position = mLeftCamera.transform.position + leftCamOffset;
                mLeftCamera.pixelRect = mLeftCameraPixelRect;

                Vector3 localPosRightCam = mRightCamera.transform.localPosition;
                Rect rightCamPixelRect = mRightCamera.pixelRect;
                Vector3 rightCamOffset = mRightCamera.transform.right.normalized * mRightCamera.stereoSeparation * 0.5f;
                mRightCamera.transform.position = mRightCamera.transform.position + rightCamOffset;
                mRightCamera.pixelRect = mRightCameraPixelRect;

                BackgroundPlaneBehaviour bgPlane = mLeftCamera.GetComponentInChildren<BackgroundPlaneBehaviour>();
                bgPlane.BackgroundOffset = mLeftCamera.transform.position - DigitalEyewearBehaviour.Instance.CentralAnchorPoint.position;

                mLeftExcessAreaBehaviour.PlaneOffset = mLeftCamera.transform.position - DigitalEyewearBehaviour.Instance.CentralAnchorPoint.position;
                mRightExcessAreaBehaviour.PlaneOffset = mRightCamera.transform.position - DigitalEyewearBehaviour.Instance.CentralAnchorPoint.position;

                if (TrackableParent != null)
                    TrackableParent.localPosition = Vector3.zero;

                // update Vuforia explicitly
                VuforiaBehaviour.Instance.UpdateState(false, true);

                if (TrackableParent != null)
                    TrackableParent.position += bgPlane.BackgroundOffset;

                // set the projection matrices for skewing
                VuforiaBehaviour.Instance.ApplyCorrectedProjectionMatrix(mLeftCameraMatrixOriginal, true);
                VuforiaBehaviour.Instance.ApplyCorrectedProjectionMatrix(mRightCameraMatrixOriginal, false);

#if !(UNITY_5_2 || UNITY_5_1 || UNITY_5_0) // UNITY_5_3 and above

                // read back the projection matrices set by Vuforia and set them to the stereo cameras
                // not sure if the matrices would automatically propagate between the left and right, so setting it explicitly twice
                mLeftCamera.SetStereoProjectionMatrices(mLeftCamera.projectionMatrix, mRightCamera.projectionMatrix);
                mRightCamera.SetStereoProjectionMatrices(mLeftCamera.projectionMatrix, mRightCamera.projectionMatrix);

#endif
                // reset the left camera
                mLeftCamera.transform.localPosition = localPosLeftCam;
                mLeftCamera.pixelRect = leftCamPixelRect;

                // reset the position of the right camera
                mRightCamera.transform.localPosition = localPosRightCam;
                mRightCamera.pixelRect = rightCamPixelRect;
            }
        }
    }

    // OnPreRender is called once per camera each frame
    void OnPreRender()
    {
        // on pre render is where projection matrix and pixel rect are set up correctly (for each camera individually)
        // so we use this to acquire this data.
        if (IsLeft && !mLeftCameraDataAcquired)
        {
            // at start matrix can be undefined
            if (!VuforiaRuntimeUtilities.MatrixIsNaN(mLeftCamera.projectionMatrix))
            {
                mLeftCameraMatrixOriginal = mLeftCamera.projectionMatrix;
                mLeftCameraPixelRect = mLeftCamera.pixelRect;
                mLeftCameraDataAcquired = true;
            }
        }
        else if (!mRightCameraDataAcquired)
        {
            // at start matrix can be undefined
            if (!VuforiaRuntimeUtilities.MatrixIsNaN(mRightCamera.projectionMatrix))
            {
                mRightCameraMatrixOriginal = mRightCamera.projectionMatrix;
                mRightCameraPixelRect = mRightCamera.pixelRect;
                mRightCameraDataAcquired = true;
            }
        }
    }
}
