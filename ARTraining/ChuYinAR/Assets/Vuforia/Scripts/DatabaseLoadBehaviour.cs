/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This behaviour allows to automatically load and activate one or more DataSet on startup
    /// </summary>
    public class DatabaseLoadBehaviour : DatabaseLoadAbstractBehaviour
    {
        public override void AddOSSpecificExternalDatasetSearchDirs()
        {
    #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                // Get the external storage directory
                AndroidJavaClass jclassEnvironment = new AndroidJavaClass("android.os.Environment");
                AndroidJavaObject jobjFile = jclassEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
                string externalStorageDirectory = jobjFile.Call<string>("getAbsolutePath");

                // Get the package name
                AndroidJavaObject jobjActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                string packageName = jobjActivity.Call<string>("getPackageName");

                // Add some best practice search directories
                //
                // Assumes just Vufroria datasets extracted to the files directory
                AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/");

                // Assume entire StreamingAssets dir is extracted here and our datasets are in the "Vuforia" directory
                AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/Vuforia/");

                // Assume entire StreamingAssets dir is extracted here and our datasets are in the "QCAR" directory
                AddExternalDatasetSearchDir(externalStorageDirectory + "/Android/data/" + packageName + "/files/QCAR/");
            }
#endif //UNITY_ANDROID
        }
    }
}
