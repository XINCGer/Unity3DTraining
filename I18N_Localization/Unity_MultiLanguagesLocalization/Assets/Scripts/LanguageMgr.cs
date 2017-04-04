using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //通过\n分割每行内容
        string[] lines = textAsset.text.Split('\n');
        //获取key,value
        for (int i = 0; i < lines.Length; i++)
        {
            //检测,如果为空串直接跳过
            if (string.IsNullOrEmpty(lines[i]))
            {
                continue;
            }
            //获取key,value
            string[] kv = lines[i].Split(':');
            //保存键值对
            dict.Add(kv[0], kv[1]);
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
