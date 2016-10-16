/*==============================================================================
Copyright (c) 2016 PTC Inc.
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This class encapsulates functionality to detect various surface events
    /// (size, orientation changed) and delegate this to native.
    /// These are used by Unity Extension code and should usually not be called by app code.
    /// </summary>
    class AndroidUnityPlayer : IUnityPlayer
    {
        // The Activity orientation is sometimes not correct when triggered immediately after the orientation change is
        // reported in Unity.
        // querying for the next 20 frames seems to yield the correct orientation eventually across all devices.
        private const int NUM_FRAMES_TO_QUERY_ORIENTATION = 25;
        private const int JAVA_ORIENTATION_CHECK_FRM_INTERVAL = 60;
        private ScreenOrientation mScreenOrientation = ScreenOrientation.Unknown;
        private ScreenOrientation mJavaScreenOrientation = ScreenOrientation.Unknown;
        private int mFramesSinceLastOrientationReset;
        private int mFramesSinceLastJavaOrientationCheck;

    // AndroidJava resources need to be #if'd in order to allow AoT compilation on iOS
    #if UNITY_ANDROID
        private AndroidJavaObject mCurrentActivity;
        private AndroidJavaClass mJavaOrientationUtility;
        private AndroidJavaClass mVuforiaInitializer;
    #endif

        #region PUBLIC_METHODS

        /// <summary>
        /// Loads native plugin libraries on platforms where this is explicitly required.
        /// </summary>
        public void LoadNativeLibraries()
        {
            LoadNativeLibrariesFromJava();
        }

        /// <summary>
        /// Initialized platform specific settings
        /// </summary>
        public void InitializePlatform()
        {
            InitAndroidPlatform();
        }

        /// <summary>
        /// Initializes Vuforia; called from Start
        /// </summary>
        public VuforiaUnity.InitError Start(string licenseKey)
        {
            int errorCode = InitVuforia(licenseKey);
            if (errorCode >= 0)
                InitializeSurface();
            return (VuforiaUnity.InitError)errorCode;
        }

        /// <summary>
        /// Called from Update, checks for various life cycle events that need to be forwarded
        /// to Vuforia, e.g. orientation changes
        /// </summary>
        public void Update()
        {
            if (SurfaceUtilities.HasSurfaceBeenRecreated())
            {
                InitializeSurface();
            }
            else
            {
                // if Unity reports that the orientation has changed, reset the member variable
                // - this will trigger a check in Java for a few frames...
                if (Screen.orientation != mScreenOrientation)
                    ResetUnityScreenOrientation();

                CheckOrientation();
            }

            mFramesSinceLastOrientationReset++;
        }

        /// <summary>
        /// Pauses Vuforia
        /// </summary>
        public void OnPause()
        {
            VuforiaUnity.OnPause();
        }

        /// <summary>
        /// Resumes Vuforia
        /// </summary>
        public void OnResume()
        {
            VuforiaUnity.OnResume();
        }

        /// <summary>
        /// Deinitializes Vuforia
        /// </summary>
        public void OnDestroy()
        {
            VuforiaUnity.Deinit();
        }

        // Java resources need to be explicitly disposed.
        public void Dispose()
        {
    #if UNITY_ANDROID
            mCurrentActivity.Dispose();
            mCurrentActivity = null;

            mJavaOrientationUtility.Dispose();
            mJavaOrientationUtility = null;
    #endif
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS

        private void LoadNativeLibrariesFromJava()
        {
    #if UNITY_ANDROID
            if (mCurrentActivity == null || mVuforiaInitializer == null)
            {
                AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                mCurrentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (mCurrentActivity != null)
                {
                    mVuforiaInitializer = new AndroidJavaClass("com.vuforia.VuforiaUnityPlayer.VuforiaInitializer");
                    mVuforiaInitializer.CallStatic("loadNativeLibraries");
                }
            }
#endif
        }

        private void InitAndroidPlatform()
        {
    #if UNITY_ANDROID
            LoadNativeLibrariesFromJava();
            if (mVuforiaInitializer != null)
                mVuforiaInitializer.CallStatic("initPlatform");
#endif
        }

        private int InitVuforia(string licenseKey)
        {
            int errorcode = -1;
    #if UNITY_ANDROID
            LoadNativeLibrariesFromJava();
            if (mVuforiaInitializer != null)
                errorcode = mVuforiaInitializer.CallStatic<int>("initVuforia", mCurrentActivity, licenseKey);
#endif
            return errorcode;
        }

        private void InitializeSurface()
        {
            SurfaceUtilities.OnSurfaceCreated();
        
    #if UNITY_ANDROID
            AndroidJavaClass javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            mCurrentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (mCurrentActivity != null)
            {
                mJavaOrientationUtility = new AndroidJavaClass("com.vuforia.VuforiaUnityPlayer.OrientationUtility");
            }
    #endif

            ResetUnityScreenOrientation();
            CheckOrientation();
        }

        private void ResetUnityScreenOrientation()
        {
            mScreenOrientation = Screen.orientation;
            mFramesSinceLastOrientationReset = 0;
        }

        private void CheckOrientation()
        {
            // check for the activity orientation for a few frames after it has changed in Unity
            bool getOrientationFromJava = mFramesSinceLastOrientationReset < NUM_FRAMES_TO_QUERY_ORIENTATION;
            if (!getOrientationFromJava)
                getOrientationFromJava = mFramesSinceLastJavaOrientationCheck > JAVA_ORIENTATION_CHECK_FRM_INTERVAL;

            if (getOrientationFromJava)
            {
                // mScreenOrientation remains at the value reported by Unity even when the activity reports a different one
                // otherwise the check for orientation changes will return true every frame.
                int correctScreenOrientation = (int) mScreenOrientation;

#if UNITY_ANDROID
                if (mCurrentActivity != null)
                {
                    // The orientation reported by Unity is not reliable on some devices (e.g. landscape right on the Nexus 10)
                    // We query the correct orientation from the activity to make sure.
                    int activityOrientation = mJavaOrientationUtility.CallStatic<int>("getSurfaceOrientation", mCurrentActivity);
                    if (activityOrientation != 0)
                        correctScreenOrientation = activityOrientation;
                }
    #endif
                ScreenOrientation javaScreenOrientation = (ScreenOrientation) correctScreenOrientation;
                if (javaScreenOrientation != mJavaScreenOrientation)
                {
                    mJavaScreenOrientation = javaScreenOrientation;
                    SurfaceUtilities.SetSurfaceOrientation(mJavaScreenOrientation);
                }
            
                mFramesSinceLastJavaOrientationCheck = 0;
            }
            else
            {
                mFramesSinceLastJavaOrientationCheck++;
            }
        }

        #endregion // PRIVATE_METHODS
    }
}
