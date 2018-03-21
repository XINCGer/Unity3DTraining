using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 第一次打开程序时将资源加载到外部
/// （扩展:可以与网络资源列表对比，然后下载更新资源）
/// </summary>
public class Driver : MonoBehaviour {

    /// <summary>
    /// UI界面显示的提示信息
    /// </summary>
    private Text ShowMessage;
    /// <summary>
    /// 资源列表名称
    /// </summary>
    private string filelist = "AssetsResources";
    /// <summary>
    /// 资源更新列表，更新完后要删除
    /// </summary>
    private string updateFilelist = "updateFilelist.txt";
    /// <summary>
    /// 比较本地文件和最新文件后，判断是否需要更新的标志位
    /// </summary>
    private bool isNeedUpdate = false;

	// Use this for initialization
	void Start () {

        ShowMessage = transform.FindChild("Info").GetComponent<Text>();
        ShowMessage.text = "正在初始化......";

        CheckBundleList();
        //Debug.Log(BuildPath._Instance.ResourceFolder);

    }

    /// <summary>
    /// 检查资源列表
    /// </summary>
    public void CheckBundleList()
    {
        
        if (IsNeedExportAssetBundle()) //判断是否需要解压资源
        {
            //开始解压资源
            StartCoroutine(ExportFileReources(() =>
            {
                //开始检查资源
                StartCoroutine(CheckResources(() =>
                {
                    //开始更新资源
                    StartCoroutine(UpdateResource(() =>
                    {
                        //开始准备运行初始化
                        StartLoadData();
                    }));

                }));
            }));
        }else
        {
            //开始检查资源
            StartCoroutine(CheckResources(() =>
            {
                //开始更新资源
                StartCoroutine(UpdateResource(() =>
                {
                    //开始准备运行初始化
                    StartLoadData();
                }));

            }));
        }
        
    }

    /// <summary>
    /// 开始加载资源信息，检查完资源后开始加载数据
    /// </summary>
    public void StartLoadData()
    {
        Assembly assem = LoadAssembly(BuildPath._Instance.ResourceFolder + "Demo.dll");
        Type t = assem.GetType("AssetBundleDemo.SourceManager");    //获取SourceManager脚本组件

        GameObject GlobalObj = new GameObject("GlobalObj");
        GlobalObj.AddComponent(t);

        Destroy(this.gameObject, 1f);
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
        }else
        {
            byte[] dllData = File.ReadAllBytes(dllPath);
            return Assembly.Load(dllData);
            //return Assembly.LoadFile(dllPath);
        }
    }


    /// <summary>
    /// 比较检查资源，查看资源是否全新
    /// </summary>
    /// <param name="UpdateData">比较完成后执行的委托</param>
    /// <returns></returns>
    IEnumerator CheckResources(Action UpdateData = null)
    {
        ShowMessage.text = "正在检查资源......";

        //这里的暂时检查一下dll文件是否存在
        if (IsNeedExportDll())
        {
            yield return StartCoroutine(ExportDll());   //不存在则导出dll
        }

        string localFilePath = BuildPath._Instance.ResourceFolder + filelist;    //本地外部路径
        string newFilePath = BuildPath._Instance.FileStreamFolder + filelist;   //要对比更新的最新文件路径

        if (!File.Exists(localFilePath))
        {
            Debug.Log("读取资源列表失败，不存在！");
            yield return null;
        }
        else
        {
            //获取本地Manifest文件信息
            ManifestInfo localManifestInfo = new ManifestInfo();
            yield return localManifestInfo.GetManifest("file://" + localFilePath);

            if(localManifestInfo.manifest == null)  //判断是否获取到
            {
                Debug.Log("获取本地Manifest失败！");
            }
            else    
            {
                //获取最新的Manifest文件
                ManifestInfo newManifestInfo = new ManifestInfo();
                yield return newManifestInfo.GetManifest(newFilePath);

                if (newManifestInfo.manifest == null)  //判断是否获取到
                {
                    Debug.Log("获取最新的Manifest失败！");
                }
                else
                {
                    //开始比较本地和最新的Manifest是否一致，从而判断是否需要更新资源
                    int count = newManifestInfo.DictBundleNamesHashID.Count;  //最新资源的总数
                    int index = 0;  //计数器

                    //开始比较资源列表信息：1、比较是否存在资源；2、比较资源的HashID是否一样
                    foreach (string key in newManifestInfo.DictBundleNamesHashID.Keys)
                    {
                        //1、先判断本地资源列表是否存在,若不存在则属于新资源
                        if (!localManifestInfo.DictBundleNamesHashID.ContainsKey(key))
                        {
                            
                            CreateUpdateResourceFileList(key);  //将需要更新的资源信息写入更新列表
                        }
                        else
                        {
                            //2、再判断HashID是否相等,若不想等在则属于新资源
                            string hashCode = newManifestInfo.DictBundleNamesHashID[key];   //获取最新资源的HashID
                            if (localManifestInfo.DictBundleNamesHashID[key] != hashCode)
                            {
                                CreateUpdateResourceFileList(key);  //将需要更新的资源信息写入更新列表
                            }
                        }
                        //显示检查进度
                        index++;
                        ShowMessage.text = "正在检查资源......" + (int)((float)index * 100 / count) + "%";

                    }

                    ShowMessage.text = "资源检查完毕";
                    yield return new WaitForSeconds(0.5f);  //等待0.5s

                    if (UpdateData != null)
                        UpdateData.Invoke();
                }

            }

        }
        
    }

    /// <summary>
    /// 创建和更新资源更新列表（是需要更新的资源列表）
    /// </summary>
    /// <param name="bundleName"></param>
    private void CreateUpdateResourceFileList(string bundleName)
    {
        if (!File.Exists(BuildPath._Instance.ResourceFolder + updateFilelist)) //如果不存在则创建
        {
            isNeedUpdate = true;    //设置更新资源的标志位为true
            using (FileStream fs = File.Create(BuildPath._Instance.ResourceFolder + updateFilelist))
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }
        //以追加的方式打开，进行写入
        using (StreamWriter sw = new StreamWriter(BuildPath._Instance.ResourceFolder + updateFilelist, true))
        {
            sw.WriteLine(bundleName);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }

    /// <summary>
    /// 根据更新列表更新资源
    /// </summary>
    /// <param name="beginRun">更新完资源后准备运行的委托</param>
    /// <returns></returns>
    IEnumerator UpdateResource(Action beginRun = null)
    {
        //如果需要更新资源
        if (isNeedUpdate)
        {
            ShowMessage.text = "正在更新资源......";
            string updateFilePath = BuildPath._Instance.ResourceFolder + updateFilelist;    //更新列表路径
            string updateFileContent = "";
            using (StreamReader sr = File.OpenText(updateFilePath))
            {
                updateFileContent = sr.ReadToEnd(); //获取更新列表
            }
            updateFileContent = updateFileContent.Trim(new char[] { '\n' });    //删除更新列表的最后一个换行符
            string[] files = updateFileContent.Split('\n'); //分离所有资源名称
            Debug.Log(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                yield return StartCoroutine(DownloadResource(files[i].Trim()));    //等待返回下载结果
                ShowMessage.text = "正在更新资源......" + (int)((float)i * 100 / files.Length) + "%" + '\n' + "本次更新不耗费流量";
            }

            yield return StartCoroutine(DownloadResource(filelist));

            ShowMessage.text = "资源更新完成！";
            yield return new WaitForSeconds(0.5f);

            //更新完成后要删除更新列表
            File.Delete(updateFilePath);
        }

        yield return new WaitForEndOfFrame();

        if (beginRun != null)
            beginRun.Invoke();

    }

    /// <summary>
    /// 下载资源
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    IEnumerator DownloadResource(string bundleName)
    {
        string inFilePath = BuildPath._Instance.FileStreamFolder + bundleName;

        using (WWW wwwfile = new WWW(inFilePath))
        {
            yield return wwwfile;
            if (!string.IsNullOrEmpty(wwwfile.error))
            {
                Debug.Log("资源名不存在：" + wwwfile.error);
            }
            else
            {
                string outFilePath = BuildPath._Instance.ResourceFolder + bundleName;
                SaveBytes(outFilePath, wwwfile.bytes);
            }
        }
    }

    /// <summary>
    /// 第一次运行时，将资源列表和资源包全部导出
    /// </summary>
    /// <param name="UpdateReoures">更新资源的委托</param>
    /// <returns></returns>
    IEnumerator ExportFileReources(Action UpdateReoures = null)
    {
        string filePath = BuildPath._Instance.FileStreamFolder + filelist;  //内部路径

        string[] bundleList = null;//资源列表的资源信息

        //下载资源列表到外部路径
        using (WWW wwwManifest = new WWW(filePath))
        {
            yield return wwwManifest;
            if (!string.IsNullOrEmpty(wwwManifest.error))
            {
                Debug.Log("获取AssetBundleManifest出错！");
            }
            else
            {
                AssetBundle manifestBundle = wwwManifest.assetBundle;
                AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");    //获取总的Manifest

                string outPath = BuildPath._Instance.ResourceFolder + filelist; //外部路径
                SaveBytes(outPath, wwwManifest.bytes);  //保存总的Manifest到外部路径
                bundleList = manifest.GetAllAssetBundles();   //获取所有的资源信息

                manifestBundle.Unload(false);   //释放掉
            }
        }

        if (bundleList == null)
        {
            yield return null;
        }
        else
        {
            for (int i = 0; i < bundleList.Length; i++)
            {
                yield return StartCoroutine(DownloadResource(bundleList[i]));    //等待返回下载结果
                ShowMessage.text = "第一次运行，解压资源..." + (int)((float)i * 100 / bundleList.Length) + "%" + '\n' + "解压不耗费流量";

            }

            ShowMessage.text = "解压完成，等待初始化...";
            yield return new WaitForSeconds(1f);    //资源全部导出后等待1s

            if (UpdateReoures != null)
                UpdateReoures.Invoke();
        }

    }

    /// <summary>
    /// 判断是否需要导出资源列表的文本
    /// </summary>
    public bool IsNeedExportAssetBundle()
    {
        string outManifestPath = BuildPath._Instance.ResourceFolder + filelist;
        
        if (File.Exists(outManifestPath))
        {
            return false;
        }else
        {
            return true;
        }
        
    }

    /// <summary>
    /// 导出dll文件到外部路径
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExportDll()
    {
        string inDllPath = BuildPath._Instance.FileStreamFolder + "Demo.dll";
        string outDllPath = BuildPath._Instance.ResourceFolder + "Demo.dll";

        using(WWW www = new WWW(inDllPath))
        {
            yield return www;
            if (www.error != null)
            {
                Debug.Log("工程内部不存在dll文件！");
            }else
            {
                SaveBytes(outDllPath, www.bytes);
            }
        }
            
    }

    /// <summary>
    /// 判断是否需要导出dll文件
    /// </summary>
    /// <returns></returns>
    public bool IsNeedExportDll()
    {
        string outDllPath = BuildPath._Instance.ResourceFolder + "Demo.dll";
        if (File.Exists(outDllPath))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="fileFullName">要写入的文件全路径</param>
    /// <param name="data">字节数据</param>
    public void SaveBytes(string fileFullName, byte[] data)
    {
        if (File.Exists(fileFullName))
            File.Delete(fileFullName);
        else if (!Directory.Exists(GetDirectoryName(fileFullName)))  //判断目录是否存在
            Directory.CreateDirectory(GetDirectoryName(fileFullName));

        using (FileStream fs = new FileStream(fileFullName, FileMode.Create))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(data); //写入
                bw.Flush(); //清空缓冲区
                bw.Close(); //关闭流
            }
            fs.Close();
        }

    }

    /// <summary>
    /// 获取文件的目录
    /// </summary>
    /// <param name="fileFullName">文件全路径</param>
    /// <returns>返回目录全路径</returns>
    public string GetDirectoryName(string fileFullName)
    {
        return fileFullName.Substring(0, fileFullName.LastIndexOf('/'));
    }


}

/// <summary>
/// 总的Manifest信息，获取到所有的资源包内容
/// </summary>
public class ManifestInfo
{
    /// <summary>
    /// 总的Manifest
    /// </summary>
    public AssetBundleManifest manifest;    

    /// <summary>
    /// 保存所有AssetBundle的名称和HashID的容器
    /// </summary>
    public Dictionary<string, string> DictBundleNamesHashID = new Dictionary<string, string>();

    public ManifestInfo()   //空的构造函数
    {
        manifest = null;
    }

    public ManifestInfo(AssetBundleManifest manifest)   //构造函数
    {
        this.manifest = manifest;

        GetBundleNamesAndHashID();

    }

    /// <summary>
    /// 获取总的Manifest
    /// </summary>
    /// <param name="manifestPath">总的Manifest路径</param>
    /// <returns></returns>
    public IEnumerator GetManifest(string manifestPath)
    {
        //下载资源列表到外部路径
        using (WWW wwwManifest = new WWW(manifestPath))
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
                GetBundleNamesAndHashID();

            }
        }
    }

    /// <summary>
    /// 获取当前总的Manifest中所有的AssetBundle名称与其HashID的对应集合
    /// </summary>
    public void GetBundleNamesAndHashID()
    {
        string[] assetBundleNames = manifest.GetAllAssetBundles();  //所有的资源包名称

        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            DictBundleNamesHashID.Add(assetBundleNames[i], manifest.GetAssetBundleHash(assetBundleNames[i]).ToString());
        }

    }

}