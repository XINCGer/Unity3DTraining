using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 打bundle助手
/// </summary>
public class BundleHelper
{
    /// <summary>
    /// 需要打bundle的资源路径
    /// </summary>
    private static readonly List<string> BundlePathList = new List<string>()
    {
        "/Prefabs",
        "/Scenes",
        "/Textures",
    };

    /// <summary>
    /// bundle的输出路径
    /// </summary>
    private static readonly string abOutputPath = Application.streamingAssetsPath;

    /// <summary>
    /// 资源的存放目录
    /// </summary>
    private static readonly string assetDir = Application.dataPath;

    /// <summary>
    /// 给所有的资源打bundle,手动设置bundleName标签
    /// </summary>
    [MenuItem("Assets/Build All_BundlesManual")]
    private static void BuildAllAssetBundlesManual()
    {
        BuildPipeline.BuildAssetBundles(abOutputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 使用脚本给资源自动设置bundle标签
    /// </summary>
    [MenuItem("Assets/SetBundleNameAuto")]
    private static void SetBundleNameAuto()
    {
        ClearAssetBundlesName();
        for (int i = 0; i < BundlePathList.Count; i++)
        {
            SetAssetBundleNameByPath(assetDir + BundlePathList[i]);
        }
    }

    /// <summary>
    /// 自动给指定目录中的资源设置bundlename并打出bundle
    /// </summary>
    [MenuItem("Assets/BuildAssetBundlesAuto")]
    private static void BuildAssetBundlesAuto()
    {
        SetBundleNameAuto();
        BuildAllAssetBundlesManual();
    }

    /// <summary>
    /// 对选中的资源分别打bundle
    /// </summary>
    [MenuItem("Assets/BuildOneBundleFromSelection")]
    private static void BuildOneBundleFromSelection()
    {
        Object[] targets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        for (int i = 0; i < targets.Length; i++)
        {
            if (null == targets[i])
            {
                continue;
            }
            string assetPath = AssetDatabase.GetAssetPath(targets[i]);
            if (null != assetPath)
            {
                //跳过文件夹
                if (!File.Exists(assetPath)) continue;
                //跳过脚本文件和meta文件
                if (assetPath.EndsWith(".cs") || assetPath.EndsWith(".meta")) continue;
                AssetBundleBuild abb = new AssetBundleBuild();
                abb.assetBundleName = Path.GetFileNameWithoutExtension(assetPath);
                //abb.assetBundleVariant = "hd";
                abb.assetNames = new[] { assetPath };
                BuildPipeline.BuildAssetBundles(abOutputPath, new AssetBundleBuild[1] { abb },
                    BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            }
        }
        AssetDatabase.Refresh();
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

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName 
    /// </summary>
    /// <param name="assetPath"></param>
    private static void SetAssetBundleNameByPath(string assetPath)
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源 
        DirectoryInfo dirInfo = new DirectoryInfo(assetPath);
        //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹  
        FileSystemInfo[] fileInfos = dirInfo.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            //如果是文件夹则递归处理 
            if (fileInfos[i] is DirectoryInfo)
            {
                SetAssetBundleNameByPath(fileInfos[i].FullName);
            }
            else if (!fileInfos[i].Name.EndsWith(".meta"))//如果是文件的话，则设置AssetBundleName，并排除掉.meta文件  
            {
                //逐个设置AssetBundleName  
                SetABName(fileInfos[i].FullName);
            }
        }
    }

    /// <summary>
    /// 设置单独的某个AssetBundle的Name
    /// </summary>
    /// <param name="assetPath"></param>
    private static void SetABName(string assetPath)
    {
        string subDir = assetPath.Substring(assetDir.Length);
        string importPath = string.Format("Assets{0}", subDir);
        //上一步的GetFileSystemInfos操作,会把路径都改成\\这种格式的Win路径，因此下面要查找 @"\"
        string tmpName = subDir.Substring(subDir.LastIndexOf(@"\") + 1);
        string bundleName = tmpName.Remove(tmpName.LastIndexOf("."));
        AssetImporter assetImporter = AssetImporter.GetAtPath(importPath);
        assetImporter.assetBundleName = bundleName;

    }
}
