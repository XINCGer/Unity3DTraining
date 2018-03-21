using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class DoAssetbundle : MonoBehaviour {
    

    /// <summary>
    /// 查看所有的Assetbundle名称（设置Assetbundle Name的对象）
    /// </summary>
    [MenuItem("AssetBundle/Get AssetBundle names")]
    static void GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames(); //获取所有设置的AssetBundle
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);
    }

    /// <summary>
    /// 自动打包所有资源（设置了Assetbundle Name的资源）
    /// </summary>
    [MenuItem("AssetBundle/Create All AssetBundles")] //设置编辑器菜单选项
    static void CreateAllAssetBundles()
    {
        //打包资源的路径，打包在对应平台的文件夹下
        string targetPath = Application.dataPath + "/StreamingAssets/AssetsResources/";
        if(!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        //打包资源
        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 将某一文件夹中的资源进行分离打包，即把依赖资源分离出来打包
    /// </summary>
    [MenuItem("AssetBundle/Set Main AssetbundleName")]
    public static void SetMainAssetBundleName()
    {
        string fullPath = Application.dataPath + "/AssetBundle/MainAssets/";    //将Assets/Prefab/文件夹下的所有预设进行打包

        SetAssetBundleName(fullPath, true);
    }

    /// <summary>
    /// 将某一文件夹中的资源进行整体打包，即不分离依赖资源，全部打成一个资源包
    /// </summary>
    [MenuItem("AssetBundle/Set Total Assetbundle Name")]
    public static void SetTotalAssetBundleName()
    {
        string fullPath = Application.dataPath + "/AssetBundle/TotalAssets/";    //将Assets/Prefab/文件夹下的所有预设进行打包

        SetAssetBundleName(fullPath, false);
    }


    /// <summary>
    /// 设置资源的资源包名称
    /// </summary>
    /// <param name="path">资源主路径</param>
    /// <param name="ContainDependences">资源包中是否包含依赖资源的标志位：true表示分离打包，false表示整体打包</param>
    static void SetAssetBundleName(string path, bool ContainDependences = false)
    {
        //ClearAssetBundlesName();    //先清楚之前设置过的AssetBundleName，避免产生不必要的资源也打包

        if (Directory.Exists(path))
        {
            EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 0f);   //显示进程加载条
            DirectoryInfo dir = new DirectoryInfo(path);    //获取目录信息
            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo fileInfo = files[i];
                EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                if (!fileInfo.Name.EndsWith(".meta"))   //判断去除掉扩展名为“.meta”的文件
                {
                    string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..
                    string assetName = fileInfo.FullName.Substring(path.Length);  //预设的Assetbundle名字，带上一级目录名称
                    assetName = assetName.Substring(0, assetName.LastIndexOf('.')); //名称要去除扩展名
                    assetName = assetName.Replace('\\', '/');   //注意此处的斜线一定要改成反斜线，否则不能设置名称
                    AssetImporter importer = AssetImporter.GetAtPath(basePath);
                    if (importer && importer.assetBundleName != assetName)
                    {
                        importer.assetBundleName = assetName;  //设置预设的AssetBundleName名称
                        //importer.SaveAndReimport();
                    }
                    //Debug.Log("主资源的路径：" + basePath);
                    if (ContainDependences)    //把依赖资源分离打包
                    {
                        //获得他们的所有依赖，不过AssetDatabase.GetDependencies返回的依赖是包含对象自己本身的
                        string[] dps = AssetDatabase.GetDependencies(basePath); //获取依赖的相对路径Assets/...
                        Debug.Log(string.Format("There are {0} dependencies!", dps.Length));
                        //遍历设置依赖资源的Assetbundle名称，用哈希Id作为依赖资源的名称
                        for (int j = 0; j < dps.Length; j++)
                        {
                            Debug.Log(dps[j]);
                            //要过滤掉依赖的自己本身和脚本文件，自己本身的名称已设置，而脚本不能打包
                            if (dps[j].Contains(assetName) || dps[j].Contains(".cs"))
                                continue;
                            else
                            {
                                AssetImporter importer2 = AssetImporter.GetAtPath(dps[j]);
                                string dpName = AssetDatabase.AssetPathToGUID(dps[j]);  //获取依赖资源的哈希ID
                                importer2.assetBundleName = "alldependencies/" + dpName;
                                //importer2.SaveAndReimport();
                            }
                        }
                    }
                    
                }
            }

            EditorUtility.ClearProgressBar();   //清除进度条
            //接下来再全部打包
            //BuildAllAssetBundles();
        }
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包 
    /// 因为只要设置了AssetBundleName的，都会进行打包，不论在什么目录下 
    /// </summary> 
    [MenuItem("AssetBundle/Clear All Assetbundle Name")]
    public static void ClearAssetBundlesName()
    {
        string[] oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
    }

    /// <summary>
    /// 官方的打包案例，在此处没有正确赋值，无法使用
    /// </summary>
    [MenuItem("AssetBundle/Build AssetBundles Using Building Map（官方案例，在此无效）")]
    static void BuildMapABs()
    {
        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        string[] enemyAssets = new string[2];   //用来记录此资源包文件名称
        enemyAssets[0] = "Assets/Textures/char_enemy_alienShip.jpg";    //将需要打包的资源名称赋给数组
        enemyAssets[1] = "Assets/Textures/char_enemy_alienShip-damaged.jpg";
        buildMap[0].assetBundleName = "enemybundle";    //打包的资源包名称，开发者可以随便命名
        buildMap[0].assetNames = enemyAssets;   //将资源名称数组赋给AssetBuild

        string[] heroAssets = new string[1];
        heroAssets[0] = "char_hero_beanMan";
        buildMap[1].assetBundleName = "herobundle";
        buildMap[1].assetNames = heroAssets;

        BuildPipeline.BuildAssetBundles("Assets/ABs", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);   //打包资源到Assets下的ABs文件夹
    }

}


//获取打包的平台
public class PlatformPath
{
    /// <summary>
    /// 获取打包的平台，根据平台设置打包的文件夹名称
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            case BuildTarget.WebPlayer:
                return "WebPlayer";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
            default: return null;
        }
    }
}

