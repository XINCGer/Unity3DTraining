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
    /// 资源路径-bundle名字映射
    /// </summary>
    private static readonly Dictionary<string, string> path4bundle = new Dictionary<string, string>()
    {
        {"Assets/Prefabs","Prefabs" },
        {"Assets/Scenes","Scenes" },
        {"Assets/Textures","Textures" },
    };

    /// <summary>
    /// 给所有的资源打bundle
    /// </summary>
    [MenuItem("Assets/Build All_Bundles")]
    private static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 使用脚本给资源自动设置bundle标签
    /// </summary>
    [MenuItem("Assets/SetBundleNameByDir")]
    private static void SetBundleNameByDir()
    {
        using (var enumator = path4bundle.GetEnumerator())
        {
            while (enumator.MoveNext())
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(enumator.Current.Key);
                assetImporter.assetBundleName = enumator.Current.Value;
            }
        }
    }

    /// <summary>
    /// 清除所有的AssetBundleName，由于打包方法会将所有设置过AssetBundleName的资源打包，所以自动打包前需要清理
    /// </summary>
    private static void ClearAssetBundlesName()
    {
        //获取所有的AssetBundle名称
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();

        //强制删除所有的AssetBundle名称
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }
    }
}
