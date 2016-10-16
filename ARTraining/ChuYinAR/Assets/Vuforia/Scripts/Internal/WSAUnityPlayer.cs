/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This class encapsulates functionality to detect various surface events
    /// (size, orientation changed) and delegate this to native.
    /// These are used by Unity Extension code and should usually not be called by app code.
    /// </summary>
    class WSAUnityPlayer : IUnityPlayer
    {
        private ScreenOrientation mScreenOrientation = ScreenOrientation.Unknown;

        /// <summary>
        /// Loads native plugin libraries on platforms where this is explicitly required.
        /// </summary>
        public void LoadNativeLibraries()
        {
        }

        /// <summary>
        /// Initialized platform specific settings
        /// </summary>
        public void InitializePlatform()
        {
            setPlatFormNative();
        }

        /// <summary>
        /// Initializes Vuforia; called from Start
        /// </summary>
        public VuforiaUnity.InitError Start(string licenseKey)
        {
            int errorCode = initVuforiaWSA(licenseKey);
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
                // if Unity reports that the orientation has changed, set it correctly in native
                ScreenOrientation currentOrientation = GetActualScreenOrientation();

                if (currentOrientation != mScreenOrientation)
                    SetUnityScreenOrientation();
            }

        }

        public void Dispose()
        {
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


        private void InitializeSurface()
        {
            SurfaceUtilities.OnSurfaceCreated();

            SetUnityScreenOrientation();
        }

        private void SetUnityScreenOrientation()
        {
            mScreenOrientation = GetActualScreenOrientation();

            SurfaceUtilities.SetSurfaceOrientation(mScreenOrientation);

            // set the native orientation (only required on iOS and WSA)
            setSurfaceOrientationWSA((int) mScreenOrientation);
        }        
 
        /// <summary>
        /// There is a known Unity issue for Windows 10 UWP apps where the initial orientation is wrongly
        /// reported as AutoRotation instead of the actual orientation.
        /// This method tries to infer the screen orientation from the device orientation if this is the case.
        /// </summary>
        /// <returns></returns>
        private ScreenOrientation GetActualScreenOrientation()
        {
            ScreenOrientation orientation = Screen.orientation;

            if (orientation == ScreenOrientation.AutoRotation)
            {
                DeviceOrientation devOrientation = Input.deviceOrientation;

                switch (devOrientation)
                {
                    case DeviceOrientation.LandscapeLeft:
                        orientation = ScreenOrientation.LandscapeLeft;
                        break;

                    case DeviceOrientation.LandscapeRight:
                        orientation = ScreenOrientation.LandscapeRight;
                        break;

                    case DeviceOrientation.Portrait:
                        orientation = ScreenOrientation.Portrait;
                        break;

                    case DeviceOrientation.PortraitUpsideDown:
                        orientation = ScreenOrientation.PortraitUpsideDown;
                        break;

                    default:
                        // fallback: Landscape Left
                        orientation = ScreenOrientation.LandscapeLeft;
                        break;
                }
            }

            return orientation;
        }

        [DllImport("VuforiaWrapper")]
        private static extern void setPlatFormNative();

        [DllImport("VuforiaWrapper")]
        private static extern int initVuforiaWSA(string licenseKey);

        [DllImport("VuforiaWrapper")]
        private static extern void setSurfaceOrientationWSA(int screenOrientation);
    }
}
