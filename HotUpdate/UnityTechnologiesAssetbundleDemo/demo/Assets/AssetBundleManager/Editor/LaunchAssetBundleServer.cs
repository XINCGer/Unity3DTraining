using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Net;
using System.Threading;
using UnityEditor.Utils;
using System.Linq;

namespace AssetBundles
{
    internal class LaunchAssetBundleServer : ScriptableSingleton<LaunchAssetBundleServer>
    {
        const string kLocalAssetbundleServerMenu = "Assets/AssetBundles/Local AssetBundle Server";

        [SerializeField]
        int     m_ServerPID = 0;

        [MenuItem(kLocalAssetbundleServerMenu)]
        public static void ToggleLocalAssetBundleServer()
        {
            bool isRunning = IsRunning();
            if (!isRunning)
            {
                Run();
            }
            else
            {
                KillRunningAssetBundleServer();
            }
        }

        [MenuItem(kLocalAssetbundleServerMenu, true)]
        public static bool ToggleLocalAssetBundleServerValidate()
        {
            bool isRunnning = IsRunning();
            Menu.SetChecked(kLocalAssetbundleServerMenu, isRunnning);
            return true;
        }

        static bool IsRunning()
        {
            if (instance.m_ServerPID == 0)
                return false;

            try
            {
                var process = Process.GetProcessById(instance.m_ServerPID);
                if (process == null)
                    return false;

                return !process.HasExited;
            }
            catch
            {
                return false;
            }
        }

        static void KillRunningAssetBundleServer()
        {
            // Kill the last time we ran
            try
            {
                if (instance.m_ServerPID == 0)
                    return;

                var lastProcess = Process.GetProcessById(instance.m_ServerPID);
                lastProcess.Kill();
                instance.m_ServerPID = 0;
            }
            catch
            {
            }
        }

        static void Run()
        {
            string pathToAssetServer = Path.GetFullPath("Assets/AssetBundleManager/Editor/AssetBundleServer.exe");
            string assetBundlesDirectory = Path.Combine(Environment.CurrentDirectory, "AssetBundles");

            KillRunningAssetBundleServer();

            BuildScript.CreateAssetBundleDirectory();
            BuildScript.WriteServerURL();

            string args = assetBundlesDirectory;
            args = string.Format("\"{0}\" {1}", args, Process.GetCurrentProcess().Id);
            ProcessStartInfo startInfo = ExecuteInternalMono.GetProfileStartInfoForMono(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), GetMonoProfileVersion(), pathToAssetServer, args, true);
            startInfo.WorkingDirectory = assetBundlesDirectory;
            startInfo.UseShellExecute = false;
            Process launchProcess = Process.Start(startInfo);
            if (launchProcess == null || launchProcess.HasExited == true || launchProcess.Id == 0)
            {
                //Unable to start process
                UnityEngine.Debug.LogError("Unable Start AssetBundleServer process");
            }
            else
            {
                //We seem to have launched, let's save the PID
                instance.m_ServerPID = launchProcess.Id;
            }
        }

        static string GetMonoProfileVersion()
        {
            string path = Path.Combine(Path.Combine(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "lib"), "mono");

            string[] folders = Directory.GetDirectories(path);
            string[] foldersWithApi = folders.Where(f => f.Contains("-api")).ToArray();
            float profileVersion = 1.0f;

            for (int i = 0; i < foldersWithApi.Length; i++)
            {
                foldersWithApi[i] = foldersWithApi[i].Split(Path.DirectorySeparatorChar).Last();
                foldersWithApi[i] = foldersWithApi[i].Split('-').First();

                if (float.Parse(foldersWithApi[i]) > profileVersion)
                {
                    profileVersion = float.Parse(foldersWithApi[i]);
                }
            }

            return profileVersion.ToString();
        }
    }
}
