/*==============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2015 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.  
==============================================================================*/

using UnityEngine;
using UnityEditor;

namespace Vuforia.EditorClasses
{
    [InitializeOnLoad]
    public static class ExtensionImport
    {
        private static readonly string VUFORIA_ANDROID_SETTINGS = "VUFORIA_ANDROID_SETTINGS";
        private static readonly string VUFORIA_IOS_SETTINGS = "VUFORIA_IOS_SETTINGS";
        private static readonly string VUFORIA_WSA_SETTINGS = "VUFORIA_WSA_SETTINGS";
        
        static ExtensionImport() 
        {
            EditorApplication.update += UpdatePluginSettings;
            EditorApplication.update += UpdatePlayerSettings;
        }

        static void UpdatePluginSettings()
        {
            // Unregister callback (executed only once)
            EditorApplication.update -= UpdatePluginSettings;

            PluginImporter[] importers = PluginImporter.GetAllImporters();
            foreach (var imp in importers)
            {
                string pluginPath = imp.assetPath;
                bool isVuforiaWrapperPlugin = 
                    pluginPath.EndsWith("QCARWrapper.dll") || pluginPath.EndsWith("VuforiaWrapper.dll") ||
                    pluginPath.EndsWith("QCARWrapper.bundle") || pluginPath.EndsWith("VuforiaWrapper.bundle");
                if (isVuforiaWrapperPlugin && imp.GetCompatibleWithAnyPlatform())
                {
                    Debug.Log("Setting platform to 'Editor' for plugin: " + pluginPath);
                    imp.SetCompatibleWithAnyPlatform(false);
                    imp.SetCompatibleWithEditor(true);
                }
            }
        }
        
        static void UpdatePlayerSettings()
        {
            // Unregister callback (executed only once)
            EditorApplication.update -= UpdatePlayerSettings;

            BuildTargetGroup androidBuildTarget = BuildTargetGroup.Android;
            BuildTargetGroup iOSBuildTarget = BuildTargetGroup.iOS;
            BuildTargetGroup wsaBuildTarget = BuildTargetGroup.WSA;

            string androidSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(androidBuildTarget);
            androidSymbols = androidSymbols ?? "";
            if (!androidSymbols.Contains(VUFORIA_ANDROID_SETTINGS)) 
            {
                if (PlayerSettings.Android.targetDevice != AndroidTargetDevice.ARMv7)
                {
                    Debug.Log("Setting Android target device to ARMv7");
                    PlayerSettings.Android.targetDevice = AndroidTargetDevice.ARMv7;
                }

                if (PlayerSettings.Android.androidTVCompatibility)
                {
                    // Disable Android TV compatibility, as this is not compatible with
                    // portrait, portrait-upside-down and landscape-right orientations.
                    Debug.Log("Disabling Android TV compatibility.");
                    PlayerSettings.Android.androidTVCompatibility = false;
                }

#if !UNITY_5_0 // UNITY_5_1 and newer
                Debug.Log("Setting Android Graphics API to OpenGL ES 2.0.");
                PlayerSettings.SetGraphicsAPIs(
                    BuildTarget.Android,
                    new UnityEngine.Rendering.GraphicsDeviceType[]{UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2});
#endif
                // Here we set the scripting define symbols for Android
                // so we can remember that the settings were set once.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                                                                 androidSymbols + ";" + VUFORIA_ANDROID_SETTINGS);
            }

            string iOSSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(iOSBuildTarget);
            iOSSymbols = iOSSymbols ?? "";
            if (!iOSSymbols.Contains(VUFORIA_IOS_SETTINGS))
            {
#if INCLUDE_IL2CPP
                int scriptingBackend = 0;
                if (PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref scriptingBackend, iOSBuildTarget))
                {
                    if (scriptingBackend != (int)ScriptingImplementation.IL2CPP)
                    {
                        Debug.Log("Setting iOS scripting backend to IL2CPP to enable 64bit support.");
                        PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, iOSBuildTarget);
                    }
                }
                else
                {
                    Debug.LogWarning("ScriptinBackend property not available for iOS; perhaps the iOS Build Support component was not installed");
                }
#endif //INCLUDE_IL2CPP

                // Here we set the scripting define symbols for IOS
                // so we can remember that the settings were set once.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(iOSBuildTarget, 
                                                                 iOSSymbols + ";" + VUFORIA_IOS_SETTINGS);
            }

            string wsaSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(wsaBuildTarget);
            wsaSymbols = wsaSymbols ?? "";
            if (!wsaSymbols.Contains(VUFORIA_WSA_SETTINGS))
            {
                // The Windows SDK we want to use is "Universal 10"
                EditorUserBuildSettings.wsaSDK = WSASDK.UWP;

                // We want to use the Webcam (obviously); to acheive this, UWP forces us to also require access 
                // to the microphone (which is not so obvious)
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.WebCam, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true);

                // Vuforia SDK for UWP now also requires InternetClient Access
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);

                // Here we set the scripting define symbols for WSA
                // so we can remember that the settings were set once.
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WSA,
                                                                 wsaSymbols + ";" + VUFORIA_WSA_SETTINGS);
            }
        }
    }
}
