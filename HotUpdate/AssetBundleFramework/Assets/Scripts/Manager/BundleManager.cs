using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net;
using System.Threading;
public class BundleManager : MonoBehaviour
{

    //string path = string.Empty; //存放assetbundle的文件路径
    //[HideInInspector]
    //public AssetBundleManifest manifest = null;  //Manifest文件
    //AssetBundleManifest serverManifest = null;
    //Dictionary<string, AssetBundle> DicBundle = null;

    private void Awake()
    {
        Init();
        GameObject cube = LoadGameObj("prefabs/cube.bytes");
        Instantiate(cube);

    }

    private void Init()//这个不用说了吧
    {
        ThreadPool.SetMinThreads(10, 10);
        BundleData.Instance.local_Path = Application.streamingAssetsPath;
        BundleData.Instance.deposit_DownFilePath = Application.persistentDataPath +"/"+ AssetBundleConfig.downLoadDir;
        BundleData.Instance.DicBundle = new Dictionary<string, AssetBundle>();
        CompareManifestInfo();
    }


    GameObject LoadGameObj(string objname) //要加载的这个ab
    {
        AssetBundle bundle = null;
        if (!BundleData.Instance.DicBundle.ContainsKey(objname)) //如果字典中不包含这个AB,加载
            LoadAssetBundle(objname);

        bundle = BundleData.Instance.DicBundle[objname];  //这里就
                                                            //从AB中拿到需要的东东，这里还不完善，另外，后边括号中，只能填资源最终的名称，在那个文件夹下，是不需要的
        GameObject obj = bundle.LoadAsset<GameObject>(GetLoadName(objname));
        return obj;
    }
    void LoadAssetBundle(string bundleName) //加载AB
    {
        LoadDependency(bundleName); //首先加载他所依赖的资源
        string pathname = Path.Combine(BundleData.Instance.local_Path, bundleName); //合并路径，得到AB所在的路径
        //AssetBundle ab = null;
        if (!BundleData.Instance.DicBundle.ContainsKey(bundleName)) //同样，如果字典中没有这个AB，那么从本地加载
        {
            //这里的加载方法有很多种，官方说的好像是四种，你 . 点一下就出来了
            BundleData.Instance.DicBundle[bundleName] = AssetBundle.LoadFromFile(pathname);
            // = ab;
        }
        //else
        //{
        //    ab = DicBundle[bundleName];
        //}
        //return ab;
    }

    void LoadDependency(string assetBundleName) //加载资源的依赖
    {
        if (BundleData.Instance.local_Manifest == null)  //如果说 manifest文件为空，先加载manifest 
        {        //manifest 本身也是一个AssetBundle
            AssetBundle assetBundle = AssetBundle.LoadFromFile(BundleData.Instance.local_Path + "/StreamingAssets");
            BundleData.Instance.local_Manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//这里是固定写法，你写别的会报错哦
        }
        string[] dependencys = BundleData.Instance.local_Manifest.GetAllDependencies(assetBundleName);//得到需要加载的资源的依赖关系

        foreach (var item in dependencys) //加载所有的依赖
        {
            LoadAssetBundle(item);
        }
    }

    //得到加载资源的名字， prefabs/cube,我们只需要这个cube
    //这里以前没有后缀，所以不会报错，但是现在有了bytes后缀，再去加载，会报错，处理一下
    string GetLoadName(string assetObjName)
    {
        int index = assetObjName.LastIndexOf("/");
        string name = assetObjName.Substring(index + 1);
        int ind = name.LastIndexOf(".");
        name = name.Substring(0, ind);
        return name;
    }

    void CompareManifestInfo() //对比manifest文件
    {
        if (BundleData.Instance.local_Manifest == null)  //如果说 manifest文件为空，先加载manifest 
        {        //manifest 本身也是一个AssetBundle
            AssetBundle assetBundle = AssetBundle.LoadFromFile(BundleData.Instance.local_Path + "/"+ AssetBundleConfig.local_BundleManifestName);
            BundleData.Instance.local_Manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//这里是固定写法，你写别的会报错哦
        }
        if(BundleData.Instance.server_Manifest == null) //服务器上的manifest文件
        {
            DownLoaderEnum downLoaderEnum = new DownLoaderEnum(AssetBundleConfig.httpAddress + AssetBundleConfig.server_BundleManifestName, BundleData.Instance.deposit_DownFilePath,null);
            ThreadPool.QueueUserWorkItem(HttpUtil.Instance.HttpDownloader,downLoaderEnum);
            StartCoroutine(AwaitDownServerBundleManifestOver());
        }
       
    }

    IEnumerator AwaitDownServerBundleManifestOver()
    {
        while(!HttpUtil.Instance.is_DownLoadOver)
        {
            yield return  new WaitForEndOfFrame();
        }
        LoadServerBundleManifest();
    }

    void LoadServerBundleManifest()
    {
        Debug.LogError(BundleData.Instance.deposit_DownFilePath +"/"+ AssetBundleConfig.server_BundleManifestName);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(BundleData.Instance.deposit_DownFilePath +"/"+ AssetBundleConfig.server_BundleManifestName);
        BundleData.Instance.server_Manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//这里是固定写法，你写别的会报错哦
        string[] server_Strs = BundleData.Instance.server_Manifest.GetAllAssetBundles();

        foreach (var item in server_Strs)
        {
            Hash128 has = BundleData.Instance.server_Manifest.GetAssetBundleHash(item);
            Debug.LogError(item + " ==> " + has.GetHashCode());
        }
        string[] local_Strs = BundleData.Instance.local_Manifest.GetAllAssetBundles();
        foreach (var item in local_Strs)
        {
            Hash128 has = BundleData.Instance.local_Manifest.GetAssetBundleHash(item);
            Debug.LogError(item + " ==> " + has.GetHashCode());
        }
    }
}
