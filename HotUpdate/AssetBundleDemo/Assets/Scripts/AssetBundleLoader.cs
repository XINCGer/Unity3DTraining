using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoader : MonoBehaviour
{
    /// <summary>
    /// 异步通过路径加载Bundle
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void LoadAssetBundleAsync(string path, Action<AssetBundle> callback, BundleLoadType type = BundleLoadType.File, int version = 0)
    {
        switch (type)
        {
            case BundleLoadType.File:
                StartCoroutine(LoadAsyncCoroutine(path, callback));
                break;
            case BundleLoadType.WWW:
                StartCoroutine(LoadFromWWWCoroutine(path, callback));
                break;
            case BundleLoadType.LoadFromCacheOrDownload:
                StartCoroutine(LoadFromWWWCacheOrDownload(path, callback, version));
                break;
            case BundleLoadType.WebRequest:
                StartCoroutine(LoadFromWebRequest(path, callback, version));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 从文件加载bundle的携程
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator LoadAsyncCoroutine(string path, Action<AssetBundle> callback)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        callback(request.assetBundle);
    }

    /// <summary>
    /// 使用WWW加载bundle的携程
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private static IEnumerator LoadFromWWWCoroutine(string path, Action<AssetBundle> callback)
    {
        WWW www = new WWW(path);
        yield return www;
        callback(www.assetBundle);
        www.Dispose();
        www = null;
    }

    /// <summary>
    ///  使用WWWCacheOrDownload加载bundle的携程
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static IEnumerator LoadFromWWWCacheOrDownload(string path, Action<AssetBundle> callback, int version)
    {
        while (!Caching.ready)
        {
            yield return null;
        }
        WWW www = WWW.LoadFromCacheOrDownload(path, version);
        yield return www;
        callback(www.assetBundle);
        www.Dispose();
        www = null;
    }

    public static IEnumerator LoadFromWebRequest(string path, Action<AssetBundle> callback, int version)
    {
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(path, (uint)version, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        callback(bundle);
        request.Dispose();
        request = null;
    }
}

/// <summary>
/// BundleLoader的方式
/// </summary>
public enum BundleLoadType : byte
{
    File,
    WWW,
    LoadFromCacheOrDownload,
    WebRequest,
}