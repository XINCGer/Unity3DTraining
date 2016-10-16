/*==============================================================================
Copyright (c) 2015 PTC Inc. All Rights Reserved.

Copyright (c) 2014-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Confidential and Proprietary - Protected under copyright and other laws.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries. 
==============================================================================*/

using UnityEngine;
using System.Collections;

namespace Vuforia
{
    /// <summary>
    /// The VideoBackgroundManager class creates a texture which is used to 
    /// render video background using BTA.
    /// </summary>
    public class VideoBackgroundManager : VideoBackgroundManagerAbstractBehaviour
    {

        private static VideoBackgroundManager mInstance = null;

        /// <summary>
        /// A simple static singleton getter to the VideoBackgroundManager (if present in the scene)
        /// Will return null if no VideoBackgroundManager has been instanciated in the scene.
        /// </summary>
        public static VideoBackgroundManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<VideoBackgroundManager>();

                return mInstance;
            }
        }
    }
}
