using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 编码和解码
/// </summary>
public class NetEncode
{
    /// <summary>
    /// 将数据编码 长度+内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Encode(byte[] data)
    {
        //整型占4个字节，所以声明一个+4的数组
        byte[] result = new byte[data.Length + 4];
        //使用流将编码二进制
        MemoryStream ms = new MemoryStream();
        BinaryWriter br = new BinaryWriter(ms);
        br.Write(data.Length);
        br.Write(data);
        //将流中的内容复制到数组中
        Buffer.BlockCopy(ms.ToArray(), 0, result, 0, (int)ms.Length);
        br.Close();
        ms.Close();
        return result;
    }

    /// <summary>
    /// 将数据解码
    /// </summary>
    /// <param name="cahce"></param>
    /// <returns></returns>
    public static byte[] Decode(ref List<byte> cahce)
    {
        //首先获取到长度，整型4字节，如果字节数不足4字节，舍弃
        if (cahce.Count < 4)
        {
            return null;
        }
        //读取数据
        MemoryStream ms = new MemoryStream(cahce.ToArray());
        BinaryReader br = new BinaryReader(ms);
        //先读取出包头的长度
        int len = br.ReadInt32();
        //根据长度，判断内容是否传递完毕
        if (len > ms.Length - ms.Position)
        {
            return null;
        }
        //获取数据
        byte[] result = br.ReadBytes(len);
        //清空消息池
        cahce.Clear();
        //将剩余没有处理的消息重新存入消息池中
        cahce.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));
        return result;
    }
}
