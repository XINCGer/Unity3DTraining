using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtil
{

    static JsonUtil instance;

    public static JsonUtil Instance
    {
        get
        {
            if (instance == null)
                instance = new JsonUtil();
            return instance;
        }
    }

    public void Init()
    {
  
             
    }
    /// <summary>
    /// 将类转换成json字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public string ObjectToJson<T>(T t)
    {
        string json = JsonUtility.ToJson(t);
        return json;
    }
    /// <summary>
    /// json字符串，反序列化成对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public T JsonToObject<T>(string json)
    {
        T t = JsonUtility.FromJson<T>(json);
        return t;
    }
}
