using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LanguageMgr : MonoBehaviour
{

    /// <summary>
    /// 语言类型
    /// </summary>
    [SerializeField]
    private SystemLanguage language;

    private static LanguageMgr _instance;

    /// <summary>
    /// 相同的key对应不同语言的value
    /// </summary>
    private Dictionary<string, string> dict = new Dictionary<string, string>();

    private void LoadLanguage()
    {
        //加载文件
        TextAsset textAsset = Resources.Load<TextAsset>(language.ToString());
        if (textAsset == null)
        {
            Debug.LogError("没有该语言的本地化文件！");
            return;
        }
        //解析Json文件内容
        JsonData jsonData = JsonMapper.ToObject(textAsset.text);

        foreach (JsonData data in jsonData)
        {
            dict.Add(data["key"].ToString(),data["value"].ToString());
        }

    }

    void Awake()
    {
        _instance = this;
        LoadLanguage();
    }

    public static LanguageMgr GetInstance()
    {
        return _instance;
    }

    public string GetText(string key)
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        return string.Empty;
    }
}


public class JsonObj
{
    //要与Json中的属性名称一一对应
    public string key;
    public string value;
}