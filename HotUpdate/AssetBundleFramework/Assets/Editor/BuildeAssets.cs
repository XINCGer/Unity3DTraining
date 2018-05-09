using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using System.Security.Cryptography;

public class BuildeAssets
{
    static List<AssetImporter> Bundle_Name = new List<AssetImporter>();

    static void Init()
    {
        Bundle_Name.Clear();
        AssetDatabase.RemoveUnusedAssetBundleNames(); //移除没有用的assetbundlename
    }

    [MenuItem("Tools/BuildeAsset")]
    static void Build()
    {
        Init();
        SetAssetBundleName();//设置选中物体的assetbundle名字
        BuildPipeline.BuildAssetBundles(GetBundleDirectory(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        // SetManifestVersion();
        SetBundleVersionInfo();
        AssetDatabase.Refresh();
    }
    static string GetBundleDirectory()
    {
        string path = Application.streamingAssetsPath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    static void SetAssetBundleName()
    {
        UnityEngine.Object[] objs = Selection.objects;
        string[] path = new string[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            path[i] = AssetDatabase.GetAssetPath(objs[i]);
            SelectionVerifyResult(path[i]);
        }

    }
    static void SelectionVerifyResult(string strpath) //验证选中的东西，是不是包含.
    {
        string[] strs = strpath.Split('.'); //如果包含.的话，分割
        int idex = strs[0].LastIndexOf('/'); //得到这个包含.的选中的文件系统的父级所在的索引
        string parentDir = strs[0].Substring(0, idex);//父级目录
        DirectoryInfo directory = new DirectoryInfo(parentDir);
        FileSystemInfo[] fileSystemInfo = directory.GetFileSystemInfos(); //得到父级目录下的所有文件系统
                                                                          //将unity自动生成的.mete 文件剔除
        foreach (var item in fileSystemInfo)
        {
            if (!item.Name.Contains(".meta"))
            {
                //list.Add(item);
                if ((item as DirectoryInfo) != null) //是文件夹
                {
                    //strpath是从Assets下级目录开始的，需要做一下处理
                    if (item.FullName.Contains(WorkPathName(strpath)))//是刚刚选中的这个文件夹,查询他的子目录
                    {

                        CheckChildFileSystem(item.FullName);
                    }
                }
                else  //是文件
                {
                    Debug.LogError("是文件==>" + item);
                    SetAssetBuildName(item.FullName);
                }
            }
        }

    }

    static string WorkPathName(string strpath)
    {
        string[] strs = strpath.Split('/');

        return strs[strs.Length - 1];
    }

    /// <summary>
    /// 判断是否包含文件夹
    /// </summary>
    /// <param name="suf"></param>
    /// <returns></returns>
    static bool IsContainsDrictory(string suf)  //判断是否包含文件夹
    {
        bool isSuf = false;
        if (suf.Contains("/")) //如果里边还包含文件夹
        {
            isSuf = true;
        }
        else
        {
            isSuf = false;
        }
        return isSuf;
    }

    static void CheckChildFileSystem(string pathname) //检查目录下的子目录
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(pathname);
        FileSystemInfo[] fileSystemInfo = directoryInfo.GetFileSystemInfos();
        foreach (var item in fileSystemInfo)
        {
            if (!item.FullName.Contains(".meta"))
            {
                if ((item as DirectoryInfo) != null)//如果是文件夹
                {
                    CheckChildFileSystem(item.FullName);
                }
                else  //如果是文件
                {
                    SetAssetBuildName(item.FullName);
                }
            }
        }
    }

    static void SetAssetBuildName(string pathname) //设置文件的bundle名字
    {
        //E:\Unity3Dproject\ManniuBlog\Assets\Prefab.s\Cube.prefab

        string strs = Application.dataPath;
        int index = pathname.LastIndexOf(pathname);
        string path = pathname.Substring(strs.Length + 1);
        string name = string.Empty;
        int ind = path.LastIndexOf('.');
        name = path.Substring(0, ind);
        //Debug.LogError(name);
        //这里一定要在Assets开始，全名或者没有都不行
        AssetImporter asset = AssetImporter.GetAtPath("Assets/" + path);
        if (asset != null)
        {
            asset.SetAssetBundleNameAndVariant(name, "bytes");
        }
        else
        {
            Debug.LogError("空");
        }
        //  Bundle_Name.Add(GetBundleDirectory() + "/" + asset.assetBundleName + "." + asset.assetBundleVariant);
        Bundle_Name.Add(asset);
    }

    static void SetBundleVersionInfo()
    {
        AllBundleInfo allBundleInfo = new AllBundleInfo();
        allBundleInfo.BundleInfoList = new List<SingleBundleInfo>();
        foreach (var item in Bundle_Name)
        {
            allBundleInfo.BundleInfoList.Add( ComputeHashAndCRC(item));
        }
 

        allBundleInfo.BundleInfoList.Insert(0, ComputeManifestHashAndCRC(GetBundleDirectory() +"/"+GetBundleDirectory().Split('/')[GetBundleDirectory().Split('/').Length - 1]));
        string json = JsonUtil.Instance.ObjectToJson<AllBundleInfo>(allBundleInfo);//序列化为json
        WriteManifestJsonConfig(json);//写入配置文件
    }

    //static void SetManifestVersion()
    //{
    //    string outPaht = GetBundleDirectory();
    //    int index = outPaht.LastIndexOf('/');
    //    string manifest = outPaht.Substring(index + 1);
    //    string md5 = ComputeMD5(outPaht + "/" + manifest);
    //    ComputeHashAndCRC(outPaht + "/" + manifest);
    //    SingleBundleInfo info = new SingleBundleInfo();
    //    info.bundleName = manifest;
    //  //  info.bundleMD5 = md5;
    //    string json = JsonUtil.Instance.ObjectToJson<SingleBundleInfo>(info);
    //    WriteManifestJsonConfig(json);
    //}

    //static string ComputeMD5(string fileName) //计算文件的MD5，网上直接摘得
    //{
    //    string hashMD5 = string.Empty;
    //    //检查文件是否存在，如果文件存在则进行计算，否则返回空值
    //    if (File.Exists(fileName))
    //    {
    //        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
    //        {
    //            //计算文件的MD5值
    //            MD5 calculator = MD5.Create();
    //            Byte[] buffer = calculator.ComputeHash(fs);
    //            calculator.Clear();
    //            //将字节数组转换成十六进制的字符串形式
    //            StringBuilder stringBuilder = new StringBuilder();
    //            for (int i = 0; i < buffer.Length; i++)
    //            {
    //                stringBuilder.Append(buffer[i].ToString("x2"));
    //            }
    //            hashMD5 = stringBuilder.ToString();
    //        }//关闭文件流
    //    }//结束计算
    //    return hashMD5;
    //}

    ///// <summary>
    /////  计算指定文件的SHA1值
    ///// </summary>
    ///// <param name="fileName">指定文件的完全限定名称</param>
    ///// <returns>返回值的字符串形式</returns>
    //static String ComputeSHA1(String fileName)
    //{
    //    String hashSHA1 = String.Empty;
    //    //检查文件是否存在，如果文件存在则进行计算，否则返回空值
    //    if (File.Exists(fileName))
    //    {
    //        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
    //        {
    //            //计算文件的SHA1值
    //            SHA1 calculator = SHA1.Create();
    //            Byte[] buffer = calculator.ComputeHash(fs);
    //            calculator.Clear();
    //            //将字节数组转换成十六进制的字符串形式
    //            StringBuilder stringBuilder = new StringBuilder();
    //            for (int i = 0; i < buffer.Length; i++)
    //            {
    //                stringBuilder.Append(buffer[i].ToString("x2"));
    //            }
    //            hashSHA1 = stringBuilder.ToString();
    //        }//关闭文件流
    //    }
    //    return hashSHA1;
    //}

    static SingleBundleInfo ComputeHashAndCRC(AssetImporter asset)
    {

        SingleBundleInfo info = new SingleBundleInfo();
        Hash128 hash128 = new Hash128();
        string fileName = GetBundleDirectory() + "/" + asset.assetBundleName + "." + asset.assetBundleVariant;
        if (BuildPipeline.GetHashForAssetBundle(fileName, out hash128))
            info.bundleHash128 = hash128.ToString();
        uint crc = new uint();
        if (BuildPipeline.GetCRCForAssetBundle(fileName, out crc))
            info.bundleCRC = crc;
        info.bundleName = asset.assetBundleName + "." + asset.assetBundleVariant;
        return info;
    }

    static SingleBundleInfo ComputeManifestHashAndCRC(string manifestName)
    {
        SingleBundleInfo info = new SingleBundleInfo();
        Hash128 hash128 = new Hash128();
        if (BuildPipeline.GetHashForAssetBundle(manifestName, out hash128))
            info.bundleHash128 = hash128.ToString();
        uint crc = new uint();
        if (BuildPipeline.GetCRCForAssetBundle(manifestName, out crc))
            info.bundleCRC = crc;
        info.bundleName = GetBundleDirectory().Split('/')[GetBundleDirectory().Split('/').Length-1];
        return info;
    }

    static void WriteManifestJsonConfig(string json)
    {
        string path = GetBundleDirectory() + "/ABconfig.json";
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
       fileStream.BeginWrite(bytes, 0, bytes.Length,new AsyncCallback( delegate (IAsyncResult ar) 
       {
           fileStream.EndWrite(ar);
       }) ,fileStream);
    }

}
