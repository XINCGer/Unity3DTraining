using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


/// <summary>
/// 模拟客户端操作
/// </summary>
public class NetUserToken
{
    /// <summary>
    /// 用于连接的socket
    /// </summary>
    private Socket socket;
    /// <summary>
    /// 数据缓冲区
    /// </summary>
    public byte[] byteBuff;
    /// <summary>
    /// 每次接受和发送的数据大小
    /// </summary>
    private readonly int size = 1024;
    /// <summary>
    /// 接收数据池
    /// </summary>
    private List<byte> receiveCache;

    private bool isReceiving;
    /// <summary>
    /// 发送数据池
    /// </summary>
    private Queue<byte[]> sendCache;

    private bool isSending;
    /// <summary>
    /// 接收到消息后的回调
    /// </summary>
    private Action<NetModel> receiveCallback;


    public NetUserToken()
    {
        byteBuff = new byte[size];
        receiveCache = new List<byte>();
        sendCache = new Queue<byte[]>();
    }

    /// <summary>
    /// 服务器接收客户端发送的消息
    /// </summary>
    /// <param name="data"></param>
    public void Receive(byte[] data)
    {
        Debug.Log("接收数据");
        //将接收到的数据放入数据池中
        receiveCache.AddRange(data);
        //如果没在读数据
        if (!isReceiving)
        {
            isReceiving = true;

        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    private void ReadData()
    {
        byte[] data = NetEncode.Decode(ref receiveCache);

        //如果数据读取成功
        if (null != data)
        {
            NetModel item = NetSerilizer.DeSerialize(data);
            Debug.Log(item.ID + "," + item.Commit + "," + item.Message);
            if (null != receiveCallback)
            {
                receiveCallback(item);
            }
            //尾递归，继续处理数据
            ReadData();
        }
        else
        {
            isReceiving = false;
        }
    }

    /// <summary>
    /// 服务器发送消息给客户端
    /// </summary>
    private void Send()
    {
        try
        {
            if (sendCache.Count == 0)
            {
                isSending = false;
                return;
            }
            byte[] data = sendCache.Dequeue();
            int count = data.Length / size;
            int len = size;
            for (int i = 0; i < count + 1; i++)
            {
                if (i == count)
                {
                    len = data.Length - i * size;
                }
                socket.Send(data, i * size, len, SocketFlags.None);
            }
            Debug.Log("发送成功");
            Send();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void WriteSendData(byte[] data)
    {
        sendCache.Enqueue(data);
        if (!isSending)
        {
            isSending = true;
            Send();
        }
    }
}
