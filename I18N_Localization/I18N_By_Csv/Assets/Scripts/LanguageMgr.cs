using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageMgr : MonoBehaviour {

    /// <summary>
    /// 语言类型
    /// </summary>
    [SerializeField]
    private SystemLanguage language;

    private static LanguageMgr _instance;

    /// <summary>
    /// 存储CSV数据的集合
    /// </summary>
    private LanguageDataBaseMap _dataBaseMap;

    private void LoadLanguage()
    {
        //加载文件
        TextAsset textAsset = Resources.Load<TextAsset>(language.ToString());
        if (textAsset == null)
        {
            Debug.LogError("没有该语言的本地化文件！");
            return;
        }

        string[] lines = textAsset.text.Split('\n');
        _dataBaseMap = new LanguageDataBaseMap();
        _dataBaseMap.InitMap(lines);
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

    public string GetText(int key)
    {
        return _dataBaseMap.GetDataById(key).GetContentByIndex(0);
    }
}
