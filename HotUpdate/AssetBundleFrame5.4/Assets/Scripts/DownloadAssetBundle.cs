using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;


//下载和加载资源
public class DownloadAssetBundle : MonoBehaviour {

    private string m_assetPath = "";

    public string fileName = "";

    public int version = 0;

    float time;

    void Start()
    {
        m_assetPath = "file://" + Application.dataPath + "/StreamingAssets/" + PlatformManifest.GetMainManifest(Application.platform) + "/";

        time = Time.time;
        StartCoroutine(DownloadAssetBundleManifest((Manifest) =>
        {
            StartCoroutine(LoadDependenceAsset(fileName, Manifest, (fileName2, Manifest2, bundle) =>
            {
                StartCoroutine(LoadAssetByName(fileName2, Manifest2, bundle));
            }));
        }));

    }

    string GetLower(string s)
    {
        return s.ToLower();
    }

    /// <summary>
    /// 获取总的AssetBundleManifest文件
    /// </summary>
    /// <param name="finish">完后加载后调用的委托</param>
    /// <returns></returns>
    public IEnumerator DownloadAssetBundleManifest(Action<AssetBundleManifest> finish = null)
    {
        
        WWW www = WWW.LoadFromCacheOrDownload(m_assetPath + PlatformManifest.GetMainManifest(Application.platform), version);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }else
        {
            AssetBundle bundle = www.assetBundle;   //获取AssetBundle
            
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest"); //获取总的AssetBundleManifest
            bundle.Unload(false);
            Debug.Log(manifest);
            if (finish!=null)
                finish.Invoke(manifest);
        }

    }

    /// <summary>
    /// 获取完总的Manifest后，先加载依赖资源
    /// </summary>
    /// <param name="fileName">需要的资源名称（待扩展名）</param>
    /// <param name="manifest">总的Manifest</param>
    /// <returns></returns>
    public IEnumerator LoadDependenceAsset(string fileName, AssetBundleManifest manifest = null, Action<string, AssetBundleManifest, AssetBundle[]> loadAsset=null)
    {
        if (manifest == null)
            yield return null;

        string[] dps = manifest.GetAllDependencies(GetLower(fileName));   //先加载依赖资源
        Debug.Log("依赖资源的数目："+ dps.Length);
        AssetBundle[] dpsBundle = new AssetBundle[dps.Length];
        for (int i = 0; i < dpsBundle.Length; i++)
        {
            Debug.Log(dps[i]);
            string dUrl = m_assetPath + dps[i];
            using (WWW www = WWW.LoadFromCacheOrDownload(dUrl, manifest.GetAssetBundleHash(dps[i])))   //此处通过依赖资源的哈希码作为依赖资源的版本信息来加载
            {
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                    Debug.LogError(www.error);
                else
                    dpsBundle[i] = www.assetBundle;
            }

        }
        
        if (loadAsset != null)
            loadAsset.Invoke(fileName, manifest, dpsBundle);

    }

    /// <summary>
    /// 根据资源名称加载和创建资源对象，要确保依赖资源已经加载
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="manifest"></param>
    /// <param name="dpsBundles"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetByName(string fileName, AssetBundleManifest manifest = null, AssetBundle[] dpsBundles = null)
    {
        if (manifest == null)
            yield return null;
        using (WWW www = WWW.LoadFromCacheOrDownload(m_assetPath + GetLower(fileName), manifest.GetAssetBundleHash(GetLower(fileName))))
        {
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
                Debug.LogError(www.error);
            else
            {
                AssetBundle bundle = www.assetBundle;
                string file = fileName;
                if(file.LastIndexOf('.')>=0)
                    file = file.Substring(0, file.LastIndexOf('.'));
                if(file.LastIndexOf('/')>=0)
                    file = file.Substring(file.LastIndexOf('/')+1);
                Debug.Log(file);
                //加载对象
                GameObject obj = bundle.LoadAsset<GameObject>(file);
                if (obj != null)
                {
                    Instantiate(obj);
                }

                //加载场景
                //SceneManager.LoadScene(file);

                bundle.Unload(false);
            }

        }

        foreach (AssetBundle item in dpsBundles)
        {
            item.Unload(false);
        }

        Debug.Log("本次加载用时：" + (Time.time - time));


    }

    /// <summary>
    /// 非缓存下载
    /// </summary>
    /// <param name="url">路径</param>
    /// <param name="assetName">资源名</param>
    /// <returns></returns>
    public IEnumerator NoCacheingDownloadAsset(string url, string assetName)
    {
        // 从URl地址下载文件，它会被临时保存在缓存中
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
            AssetBundle bundle = www.assetBundle;
            if (assetName == "")
                Instantiate(bundle.mainAsset);  //加载主要的资源
            else
                Instantiate(bundle.LoadAsset(assetName));//加载资源名为AssetName的资源
                                                         
            bundle.Unload(false);// 记得要从缓存中清除（Unload）

        } // 自动从Web流中释放内存（使用using()会隐式WWW.Dispose()进行释放）

    }

    /// <summary>
    /// 缓存方式下载
    /// </summary>
    /// <param name="url">下载地址</param>
    /// <param name="assetName">资源名</param>
    /// <param name="version">版本号</param>
    /// <returns></returns>
    public IEnumerator CacheingDownloadAsset(string url, string assetName, int version)
    {
        //等待缓存系统完成缓存准备
        while (!Caching.ready)
            yield return null;

        //如果本地缓存中存在相同版本号的Assetbundle资源，就会从本次返回对象
        //否则会根据版本号从BundleURL下载资源并保存在缓存中
        using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
        {
            yield return www;
            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
            AssetBundle bundle = www.assetBundle;
            if (assetName == "")
                Instantiate(bundle.mainAsset);
            else
                Instantiate(bundle.LoadAsset(assetName));
            // 记得要从缓存中清除（Unload）
            bundle.Unload(false);

        } // 自动从Web流中释放内存（使用using()会隐式WWW.Dispose()进行释放）
    }
    
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="url"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    IEnumerator LoadAssetAsync(string url, int version)
    {
        while (!Caching.ready)
            yield return null;
        using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
        {
            yield return www;

            AssetBundle bundle = www.assetBundle;

            //异步加载名为myObject的GameObject对象，返回AssetBundleRequest
            AssetBundleRequest request = bundle.LoadAssetAsync("myObject", typeof(GameObject));
            yield return request;

            //获取加载的对象，并创建出来
            GameObject obj = request.asset as GameObject;
            Instantiate(obj);

            bundle.Unload(false);   //切记释放没用资源
        }
    }


    

    public class PlatformManifest
    {
        public static string GetMainManifest(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.WindowsWebPlayer:
                    return "WebPlayer";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                default:
                    return null;
            }
        }
    }

}
