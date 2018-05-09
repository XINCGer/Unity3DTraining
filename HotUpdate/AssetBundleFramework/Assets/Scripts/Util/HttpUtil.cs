using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System;

public class DownLoaderEnum
{
    public string url;
    public string path;
    public Action action;
    public DownLoaderEnum(string URL, string PATH,Action ACTION)
    {
        url = URL;
        path = PATH;
        action = ACTION;
    }
}


public class HttpUtil  {

    static HttpUtil instance;

    public static HttpUtil Instance
    {
        get
        {
            if (instance == null)
                instance = new HttpUtil();
            return instance;
        }
    }


    public bool is_DownLoadOver = false;

    /// <summary>
    /// http下载文件
    /// </summary>
    /// <param name="url">下载文件地址</param>
    /// <param name="path">文件存放地址，包含文件名</param>
    /// <returns></returns>
    public  void HttpDownloader(object down)
    {
        is_DownLoadOver = false;
        if (!Directory.Exists((down as DownLoaderEnum).path))
            Directory.CreateDirectory((down as DownLoaderEnum).path);
        string tempPath = System.IO.Path.GetDirectoryName((down as DownLoaderEnum).path) + @"\temp";
        System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
        string tempFile = tempPath + @"\" + System.IO.Path.GetFileName((down as DownLoaderEnum).path) + ".temp"; //临时文件

        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);    //存在则删除
        }
        try
        {
            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            // 设置参数
            HttpWebRequest request = WebRequest.Create((down as DownLoaderEnum).url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            //Stream stream = new FileStream(tempFile, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, bArr.Length);
            while (size > 0)
            {
                //stream.Write(bArr, 0, size);
                fs.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, bArr.Length);
            }
            //stream.Close();
            fs.Close();
            responseStream.Close();
            string suffixName = (down as DownLoaderEnum).url;
            int su = suffixName.LastIndexOf('/');
            suffixName = (down as DownLoaderEnum).path + suffixName.Substring(su);
            // Debug.LogError(suffixName);
            if (File.Exists(suffixName))
                File.Delete(suffixName);
            System.IO.File.Move(tempFile, suffixName);
            // return true;
            Debug.LogError("下载完成");
            is_DownLoadOver = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("错误==>>" + ex.Message);
            //return false;
        }
    }
}
