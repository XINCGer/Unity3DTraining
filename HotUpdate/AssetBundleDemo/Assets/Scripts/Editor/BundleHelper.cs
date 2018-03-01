using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 打bundle助手
/// </summary>
public class BundleHelper
{
    /// <summary>
    /// 给所有的资源打bundle
    /// </summary>
    [MenuItem("Assets/Build All_Bundles")]
    private static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }
}
