using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    /// <summary>
    /// 异步通过路径加载Bundle
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void LoadAssetBundleAsync(string path, Action<AssetBundle> callback)
    {
        StartCoroutine(LoadAsyncCoroutine(path, callback));
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
}
