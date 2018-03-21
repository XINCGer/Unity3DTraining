using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

namespace AssetBundleDemo
{
    /// <summary>
    /// 资源管理模块
    /// </summary>
    public class SourceManager : MonoBehaviour {

        private static SourceManager instance = null;

        public static SourceManager _Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// T_Shirt程序集
        /// </summary>
        private Assembly assembly = null;

        public Assembly DemoAssembly
        {
            get
            {
                if (assembly == null)
                    assembly = LoadAssembly(BuildPath._Instance.ResourceFolder + "Demo.dll");
                return assembly;
            }
        }

        /// <summary>
        /// 资源列表名称，总的Manifest
        /// </summary>
        private string filelist = "AssetsResources";

        /// <summary>
        /// 保存当前的UI状态
        /// </summary>
        public UIID currentID = UIID.None;


        /// <summary>
        /// 总的Manifest文件
        /// </summary>
        private AssetBundleManifest manifest = null;
        public AssetBundleManifest Manifest
        {
            get
            {
                return manifest;
            }
        }

        /// <summary>
        /// 当前已加载的无依赖的资源包，Key表示本地相对url
        /// </summary>
        public Dictionary<string, AssetBundle> DictSourceBundle = new Dictionary<string, AssetBundle>();

        void Start()
        {
            InitData();
            //添加UI管理器并初始化
            UIManager uiManager = gameObject.AddComponent(getType("UIManager")) as UIManager;
            uiManager.InitUIData();
            //添加音效管理器并初始化
            SoundManager soundManager = gameObject.AddComponent(getType("SoundManager")) as SoundManager;
            soundManager.InitData();

            //先加载总的Manifest文件
            StartCoroutine(GetManifest(BuildPath._Instance.ResourceFolder + filelist, () =>
            {
                StartCoroutine(GetMainAssetBundle(() =>
                {
                    StartCoroutine(UIManager._Instance.CreateUI(UIID.Loading));
                }));
            }));

        }

        // Use this for initialization
        void InitData () {
            instance = this;
            gameObject.AddComponent<AudioListener>();   //添加音频监听器
            //这里可以将模型、特效、场景模型、灯光贴图的信息保存在内存中，方便后期的使用
            //不举例子说明了

        }

        /// <summary>
        /// 将常用的AssetBundle存储到内存中
        /// （例如场景文件、灯光贴图、部分音频等等）
        /// </summary>
        public IEnumerator GetMainAssetBundle(Action beginRun = null)
        {
            //例子：
            //获取音效Audio的AssetBundle
            foreach (AudioInfo info in SoundManager._Instacne.DictAudioInfo.Values)
            {
                //下载音频资源，并释放依赖
                yield return StartCoroutine(DownloadAssetBundle(info.bundleName, true));

                //获取相应的音频信息
                SoundManager._Instacne.LoadAudioClip(info);
            }

            if (beginRun != null)
                beginRun.Invoke();

        }

        /// <summary>
        /// 从保存的容器中获取资源包
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="version">版本号</param>
        /// <returns>返回获取的资源包</returns>
        public AssetBundle getAssetBundle(string bundleName)
        {
            AssetBundle bundle;
            if (DictSourceBundle.TryGetValue(bundleName, out bundle))    //将空的abRef传递进去
                return bundle;
            else
                return null;
        }

        /// <summary>
        /// 下载依赖资源，暂时没用
        /// </summary>
        /// <param name="dependences">依赖资源的bundle名称</param>
        /// <param name="DownloadAssetBundle">最后下载主资源的委托</param>
        /// <returns></returns>
        public IEnumerator DownloadDependBundle(string bundleName, Action<string, AssetBundle[]> DownloadAssetBundle = null)
        {
            if (Manifest == null)
                yield return null;
            else
            {
                string[] dependences = Manifest.GetAllDependencies(bundleName);   //依赖
                AssetBundle[] bundles = null;   //用来保存依赖资源包
                if (dependences!=null && dependences.Length > 0)
                {
                    bundles = new AssetBundle[dependences.Length];    
                    for (int i = 0; i < dependences.Length; i++)
                    {
                        string url = "file://" + BuildPath._Instance.ResourceFolder + dependences[i];
                        using (WWW www = new WWW(url))
                        {
                            yield return www;
                            if (www.error != null)
                            {
                                Debug.Log("依赖资源加载出错：" + dependences[i]);
                            }
                            else
                            {
                                bundles[i] = www.assetBundle;
                            }
                        }
                    }
                }
                
                //依赖资源下载完成后开始下载主资源
                if (DownloadAssetBundle != null)
                {
                    DownloadAssetBundle.Invoke(bundleName, bundles);
                }else
                {
                    for (int i = 0; i < bundles.Length; i++)
                    {
                        bundles[i].Unload(false);
                    }
                }

            }

        }

        /// <summary>
        /// 下载AssetBundle资源包，并保存在内存中
        /// </summary>
        /// <param name="bundleName">资源包名</param>
        /// <param name="saveInMemory">是否保存起来的标志位</param>
        /// <param name="loadOver">下一步的委托</param>
        /// <returns></returns>
        public IEnumerator DownloadAssetBundle(string bundleName, bool saveInMemory = false, Action<AssetBundle, AssetBundle[]> loadOver = null)
        {
            if (DictSourceBundle.ContainsKey(bundleName))
                yield return null;
            else
            {
                if (Manifest == null)
                    yield return null;
                else
                {
                    //先下载依赖资源
                    string[] dependences = Manifest.GetAllDependencies(bundleName);   //依赖
                    AssetBundle[] dpBundles = null;   //用来保存依赖资源包
                    if (dependences != null && dependences.Length > 0)
                    {
                        dpBundles = new AssetBundle[dependences.Length];
                        for (int i = 0; i < dependences.Length; i++)
                        {
                            string dpURL = "file://" + BuildPath._Instance.ResourceFolder + dependences[i];
                            using (WWW www = new WWW(dpURL))
                            {
                                yield return www;
                                if (www.error != null)
                                {
                                    Debug.Log("依赖资源加载出错：" + dependences[i]);
                                }
                                else
                                {
                                    dpBundles[i] = www.assetBundle;
                                }
                            }
                        }
                    }

                    //下载主资源
                    string url = "file://" + BuildPath._Instance.ResourceFolder + bundleName;
                    AssetBundle bundle = null;
                    using (WWW www = new WWW(url))
                    {
                        yield return www;
                        if (www.error != null)
                        {
                            Debug.Log("下载AssetBundle出错！地址：" + url);
                        }else
                        {
                            bundle = www.assetBundle;
                        }
                    }
                    if (bundle != null)
                    {
                        if (saveInMemory)   //保存到内存中
                            DictSourceBundle.Add(bundleName, bundle);
                        
                        if (loadOver != null)   //下一步的委托，将加载的Bundle和依赖资源包作为参数
                            loadOver.Invoke(bundle, dpBundles);
                        else
                        {
                            if (!saveInMemory)  //如果没有保存到内存中，则释放
                            {
                                bundle.Unload(false);

                                if (dpBundles != null)    //释放当前Bundles
                                {
                                    Debug.LogWarning("注意：有依赖资源被提前释放！" + bundleName);
                                    for (int i = 0; i < dpBundles.Length; i++)
                                    {
                                        dpBundles[i].Unload(false);
                                    }
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        Debug.Log("主资源加载失败！");
                        if (dpBundles != null)    //释放当前Bundles
                        {
                            Debug.LogWarning("注意：有依赖资源被提前释放！" + bundleName);
                            for (int i = 0; i < dpBundles.Length; i++)
                            {
                                dpBundles[i].Unload(false);
                            }
                        }
                    }

                }


            }


            
        }

        /// <summary>
        /// 下载AssetBundle资源包函数的重载，不保存在内存中，主要用于临时创建的资源
        /// </summary>
        /// <param name="bundleName">资源包名</param>
        /// <param name="loadOver">下一步的委托</param>
        /// <returns></returns>
        public IEnumerator DownloadAssetBundle(string bundleName, Action< AssetBundle,  AssetBundle[]> loadOver = null)
        {
            yield return StartCoroutine(DownloadAssetBundle(bundleName, false, loadOver));
        }


        /// <summary>
        /// 删除容器中保存的资源数据
        /// </summary>
        /// <param name="bundleName">路径地址</param>
        /// <param name="version">版本号</param>
        /// <param name="allObjects">表示是否删除资源包的所有资源</param>
        public void Unload(string bundleName, bool allObjects)
        {
            AssetBundle bundleInfo;
            if (DictSourceBundle.TryGetValue(bundleName, out bundleInfo))
            {
                bundleInfo.Unload(allObjects);
                bundleInfo = null;
                DictSourceBundle.Remove(bundleName);
            }
        }

        /// <summary>
        /// 从程序集中获取脚本组件
        /// </summary>
        /// <param name="scriptName">脚本名称</param>
        /// <returns></returns>
        public Type getType(string scriptName)
        {
            Type t = DemoAssembly.GetType("AssetBundleDemo." + scriptName);
            if (t == null)
            {
                Debug.Log("获取脚本失败！" + scriptName);
                return null;
            }
            else
            {
                return t;
            }
        }


        /// <summary>
        /// 加载dll程序集文件
        /// </summary>
        /// <returns></returns>
        public Assembly LoadAssembly(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                Debug.Log("Demo.dll文件不存在！无法加载程序集！");
                return null;
            }
            else
            {
                //return Assembly.LoadFile(dllPath);    //这种方式加载程序集会将文件加载到内存中，需要释放才能取消对文件的占用
                byte[] dllData = File.ReadAllBytes(dllPath);    //这种是吧dll文件中的程序集信息读取到内存中，不影响文件的使用
                return Assembly.Load(dllData);
                
            }
        }


        /// <summary>
        /// 获取总的Manifest
        /// </summary>
        /// <param name="manifestPath">总的Manifest路径</param>
        /// <param name="loadAssetBundle">委托</param>
        /// <returns></returns>
        public IEnumerator GetManifest(string manifestPath, Action loadAssetBundle = null)
        {
            //下载资源列表到外部路径
            using (WWW wwwManifest = new WWW("file://" + manifestPath))
            {
                yield return wwwManifest;
                if (!string.IsNullOrEmpty(wwwManifest.error))
                {
                    Debug.Log("获取AssetBundleManifest出错！");
                }
                else
                {
                    AssetBundle manifestBundle = wwwManifest.assetBundle;
                    manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");    //获取总的Manifest
                    manifestBundle.Unload(false);

                    if (loadAssetBundle != null)
                        loadAssetBundle.Invoke();

                }
            }
        }

    }

}
