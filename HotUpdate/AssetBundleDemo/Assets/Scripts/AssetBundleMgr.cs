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
    public static readonly string streamingAssetPath = Application.streamingAssetsPath;
    public static readonly string persistentDataPath = Application.persistentDataPath;
    public static readonly string wwwStreamingAssetPath =
#if UNITY_ANDROID
        Application.streamingAssetsPath;
#else
        "file://" + Application.streamingAssetsPath;
#endif
    private static AssetBundleMgr instance;
    private static AssetBundleLoader assetBundleLoader;
    private AssetBundleManifest mainManifest;
    private List<AssetBundle> depsBundles;

    private AssetBundleMgr()
    {
        if (null == assetBundleLoader)
        {
            GameObject go = new GameObject("AssetBundleLoader");
            GameObject.DontDestroyOnLoad(go);
            assetBundleLoader = go.AddComponent<AssetBundleLoader>();
            depsBundles = new List<AssetBundle>();
            LoadMainManifest();
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
    /// 通过资源名获取所在bundle的名称
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    string GetBundleName(string assetName)
    {
        if (assetName == "AssetBundleManifest")
        {
            return "StreamingAssets";
        }
        return assetName.ToLower();
    }

    #region AssetBundle加载

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

    /// <summary>
    /// 使用WWW从本地加载bundle(异步)
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="callback"></param>
    /// <param name="version"></param>
    public static void LoadFromWWWLocalAsync(string bundleName, Action<AssetBundle> callback, int version)
    {
        assetBundleLoader.LoadAssetBundleAsync(Path.Combine(wwwStreamingAssetPath, bundleName), callback, BundleLoadType.WWW, version);
    }

    /// <summary>
    /// 使用WWW从网络URL地址加载bundle(异步)
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <param name="version"></param>
    public static void LoadFromWWWUrlAsync(string url, Action<AssetBundle> callback, int version)
    {
        assetBundleLoader.LoadAssetBundleAsync(url, callback, BundleLoadType.WWW, version);
    }

    /// <summary>
    /// 使用WWWCacheOrDownload从网络URL加载bundle(异步)
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    /// <param name="version"></param>
    public static void LoadFromWWWCacheOrDownloadAsync(string url, Action<AssetBundle> callback, int version)
    {
        assetBundleLoader.LoadAssetBundleAsync(url, callback, BundleLoadType.LoadFromCacheOrDownload, version);
    }

    public static void LoadFromWebRequestAsync(string url, Action<AssetBundle> callback, int version)
    {
        assetBundleLoader.LoadAssetBundleAsync(url, callback, BundleLoadType.WebRequest, version);
    }

    #endregion

    #region 资源加载

    /// <summary>
    /// 从StreamingAsset中加载资源(同步)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public T LoadAssetFromStreamingAsset<T>(string assetName) where T : UnityEngine.Object
    {
        GetDependenciesByName(assetName);
        AssetBundle assetBundle = LoadFromStreamingAssetPath(GetBundleName(assetName));
        T obj = assetBundle.LoadAsset<T>(assetName);
        assetBundle.Unload(false);
        //ReleaseAllDependencies();
        return obj;
    }

    /// <summary>
    /// 从PersistantData中加载资源(同步)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public T LoadAssetFromPersistant<T>(string assetName) where T : UnityEngine.Object
    {
        GetDependenciesByName(assetName);
        AssetBundle assetBundle = LoadFromPersistantDataPath(GetBundleName(assetName));
        T obj = assetBundle.LoadAsset<T>(assetName);
        assetBundle.Unload(false);
        //ReleaseAllDependencies();
        return obj;
    }

    public void LoadMainManifest()
    {
        mainManifest = LoadAssetFromStreamingAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    private void GetDependenciesByName(string assetName)
    {
        if ("AssetBundleManifest" == assetName) return;
        string[] deps = mainManifest.GetAllDependencies(GetBundleName(assetName));
        depsBundles.Clear();
        for (int i = 0; i < deps.Length; i++)
        {
            AssetBundle assetBundle = LoadFromStreamingAssetPath(GetBundleName(deps[i]));
            depsBundles.Add(assetBundle);
        }
    }

    public void ReleaseAllDependencies()
    {
        for (int i = 0; i < depsBundles.Count; i++)
        {
            depsBundles[i].Unload(false);
        }
    }

    #endregion
}
