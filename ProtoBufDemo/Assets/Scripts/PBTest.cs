using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PBTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        //创建对象
        NetModel item = new NetModel() { ID = 1, Commit = "马三", Message = "Unity" };
        //序列化对象
        byte[] temp = Serialize(item);
        Debug.Log("序列化数组长度：" + temp.Length);
        //反序列化为对象
        NetModel result = DeSerialize(temp);
        Debug.Log(result.ID + "," + result.Commit + "," + result.Message);
    }

    /// <summary>
    /// 将消息序列化为二进制数组
    /// </summary>
    /// <param name="netModel"></param>
    /// <returns></returns>
    private byte[] Serialize(NetModel netModel)
    {
        try
        {
            //将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具序列化方法
                ProtoBuf.Serializer.Serialize<NetModel>(ms, netModel);
                //保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设置为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 把收到的消息反序列化成对象
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    private NetModel DeSerialize(byte[] msg)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //将消息写入流中
                ms.Write(msg, 0, msg.Length);
                //将流的位置归零
                ms.Position = 0;
                //使用工具反序列化对象
                NetModel result = ProtoBuf.Serializer.Deserialize<NetModel>(ms);
                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
