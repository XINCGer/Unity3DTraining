/*==============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// The VuforiaBehaviour class handles tracking and triggers native video
    /// background rendering. The class updates all Trackables in the scene.
    /// </summary>
    public class VuforiaBehaviour : VuforiaAbstractBehaviour
    {
        protected override void Awake()
        {
            IUnityPlayer unityPlayer = new NullUnityPlayer();

            // instantiate the correct UnityPlayer for the current platform
            if (Application.platform == RuntimePlatform.Android)
                unityPlayer = new AndroidUnityPlayer();
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                unityPlayer = new IOSUnityPlayer();
            else if (VuforiaRuntimeUtilities.IsPlayMode())
                unityPlayer = new PlayModeUnityPlayer();
            else if (VuforiaRuntimeUtilities.IsWSARuntime())
            {
                unityPlayer = new WSAUnityPlayer();
            }

            SetUnityPlayerImplementation(unityPlayer);

            gameObject.AddComponent<ComponentFactoryStarterBehaviour>();

            base.Awake();
        }

        private static VuforiaBehaviour mVuforiaBehaviour= null;

        /// <summary>
        /// A simple static singleton getter to the VuforiaBehaviour (if present in the scene)
        /// Will return null if no VuforiaBehaviour has been instanciated in the scene.
        /// </summary>
        public static VuforiaBehaviour Instance
        {
            get
            {
                if (mVuforiaBehaviour == null)
                    mVuforiaBehaviour = FindObjectOfType<VuforiaBehaviour>();

                return mVuforiaBehaviour;
            }
        }
    }
}
