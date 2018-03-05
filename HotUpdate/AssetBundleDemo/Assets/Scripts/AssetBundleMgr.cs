using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// AssetBundle管理器
/// </summary>
public class AssetBundleMgr
{
    private static readonly string streamingAssetPath = Application.streamingAssetsPath;
    private static readonly string persistentDataPath = Application.persistentDataPath;
    private static AssetBundleMgr instance;
    private static AssetBundleLoader assetBundleLoader;

    private AssetBundleMgr()
    {
        if (null == assetBundleLoader)
        {
            GameObject go = new GameObject("AssetBundleLoader");
            GameObject.DontDestroyOnLoad(go);
            assetBundleLoader = go.AddComponent<AssetBundleLoader>();
        }
    }

    public static AssetBundleMgr GetInstance()
    {
        if (null == instance)
        {
            instance = new AssetBundleMgr();
        }
        return instance;
    }

    /// <summary>
    /// 从StreamingAssetPath加载(同步)
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static AssetBundle LoadFromStreamingAssetPath(string bundleName)
    {
        return AssetBundle.LoadFromFile(Path.Combine(streamingAssetPath, bundleName));
    }

    /// <summary>
    /// 从PersistantDataPath加载(同步)
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static AssetBundle LoadFromPersistantDataPath(string bundleName)
    {
        return AssetBundle.LoadFromFile(Path.Combine(persistentDataPath, bundleName));
    }

    /// <summary>
    /// 从StreamingAssetPath加载(异步)
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static void LoadFromStreamingAssetPathAsync(string bundleName, Action<AssetBundle> callback)
    {
        assetBundleLoader.LoadAssetBundleAsync(Path.Combine(streamingAssetPath, bundleName), callback);
    }

    /// <summary>
    /// 从PersistantDataPath加载(异步)
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="callback"></param>
    public static void LoadFromPersistantDataPathAsync(string bundleName, Action<AssetBundle> callback)
    {
        assetBundleLoader.LoadAssetBundleAsync(Path.Combine(persistentDataPath, bundleName), callback);
    }
}
