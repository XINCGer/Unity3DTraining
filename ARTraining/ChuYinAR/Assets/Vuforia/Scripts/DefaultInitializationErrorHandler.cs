/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that registers for Vuforia initialization errors
    /// </summary>
    public class DefaultInitializationErrorHandler : MonoBehaviour
    {
        #region PRIVATE_MEMBER_VARIABLES

        private string mErrorText = "";
        private bool mErrorOccurred = false;

        private const string WINDOW_TITLE = "Vuforia Initialization Error";

        #endregion // PRIVATE_MEMBER_VARIABLES

        #region UNTIY_MONOBEHAVIOUR_METHODS

        void Awake()
        {
            // Check for an initialization error on start.
            VuforiaAbstractBehaviour vuforiaBehaviour = (VuforiaAbstractBehaviour)FindObjectOfType(typeof(VuforiaAbstractBehaviour));
            if (vuforiaBehaviour)
            {
                vuforiaBehaviour.RegisterVuforiaInitErrorCallback(OnVuforiaInitializationError);
            }
        }

        void OnGUI()
        {
            // On error, create a full screen window.
            if (mErrorOccurred)
                GUI.Window(0, new Rect(0, 0, Screen.width, Screen.height),
                                       DrawWindowContent, WINDOW_TITLE);
        }

        /// <summary>
        /// When this game object is destroyed, it unregisters itself as event handler
        /// </summary>
        void OnDestroy()
        {
            VuforiaAbstractBehaviour vuforiaBehaviour = (VuforiaAbstractBehaviour)FindObjectOfType(typeof(VuforiaAbstractBehaviour));
            if (vuforiaBehaviour)
            {
                vuforiaBehaviour.UnregisterVuforiaInitErrorCallback(OnVuforiaInitializationError);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS

        #region PRIVATE_METHODS

        private void DrawWindowContent(int id)
        {
            // Create text area with a 10 pixel distance from other controls and
            // window border.
            GUI.Label(new Rect(10, 25, Screen.width - 20, Screen.height - 95),
                        mErrorText);

            // Create centered button with 50/50 size and 10 pixel distance from
            // other controls and window border.
            if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height - 60, 150, 50), "Close"))
            {
    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
            }
        }

        private void SetErrorCode(VuforiaUnity.InitError errorCode)
        {
            Debug.LogError("Vuforia initialization failed: " + mErrorText);
            switch (errorCode)
            {
                case VuforiaUnity.InitError.INIT_EXTERNAL_DEVICE_NOT_DETECTED:
                    mErrorText =
                        "Failed to initialize Vuforia because this " +
                        "device is not docked with required external hardware.";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_MISSING_KEY:
                    mErrorText =
                        "Vuforia App key is missing. Please get a valid key, " + 
					    "by logging into your account at developer.vuforia.com " + 
					    "and creating a new project";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_INVALID_KEY:
                    mErrorText =
                        "Invalid Key used. " + 
                        "Please make sure you are using a valid Vuforia App Key";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_TRANSIENT:
                    mErrorText =
                        "Unable to contact server. Please try again later.";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_NO_NETWORK_PERMANENT:
                    mErrorText =
                        "No network available. Please make sure you are connected to the internet.";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_CANCELED_KEY:
                    mErrorText =
                        "This App license key has been cancelled " + 
                        "and may no longer be used. Please get a new license key.";
                    break;
                case VuforiaUnity.InitError.INIT_LICENSE_ERROR_PRODUCT_TYPE_MISMATCH:
                    mErrorText =
                        "Vuforia App key is not valid for this product. Please get a valid key, "+
                        "by logging into your account at developer.vuforia.com and choosing the "+
                        "right product type during project creation";
                    break;
    #if (UNITY_IPHONE || UNITY_IOS)
                case VuforiaUnity.InitError.INIT_NO_CAMERA_ACCESS:
                    mErrorText = 
                        "Camera Access was denied to this App. \n" + 
                        "When running on iOS8 devices, \n" + 
                        "users must explicitly allow the App to access the camera.\n" + 
                        "To restore camera access on your device, go to: \n" + 
                        "Settings > Privacy > Camera > [This App Name] and switch it ON.";
                    break;
    #endif
                case VuforiaUnity.InitError.INIT_DEVICE_NOT_SUPPORTED:
                    mErrorText =
                        "Failed to initialize Vuforia because this device is not " +
                        "supported.";
                    break;
                case VuforiaUnity.InitError.INIT_ERROR:
                    mErrorText = "Failed to initialize Vuforia.";
                    break;
            }
        }

        private void SetErrorOccurred(bool errorOccurred)
        {
            mErrorOccurred = errorOccurred;
        }

        #endregion // PRIVATE_METHODS



        #region Vuforia_lifecycle_events

        public void OnVuforiaInitializationError(VuforiaUnity.InitError initError)
        {
            if (initError != VuforiaUnity.InitError.INIT_SUCCESS)
            {
                SetErrorCode(initError);
                SetErrorOccurred(true);
            }
        }

        #endregion // Vuforia_lifecycle_events
    }
}
