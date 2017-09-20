using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CSV 单条数据的基类
/// </summary>
abstract class DataBase
{
    public int id;

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="strData"></param>
    /// <param name="splitStr"></param>
    public abstract void InitWithStr(string strData, char splitStr = ',');

    /// <summary>
    /// 获取Int数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    protected int GetInt(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return int.Parse(str);
        }
        return 0;
    }

    /// <summary>
    /// 获取float数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    protected float GetFloat(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return float.Parse(str);
        }
        return 0.0f;
    }

    /// <summary>
    /// 获取bool数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    protected bool GetBool(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            return bool.Parse(str);
        }
        return false;
    }
}

interface IDataBaseMap
{
    void InitMap(string[] rows);
}


class LanguageDataBase : DataBase
{ 
    private List<string> langList = new List<string>();

    public override void InitWithStr(string strData, char splitStr = ',')
    {
        if (string.IsNullOrEmpty(strData)) return;

        strData = strData.TrimEnd('\r');
        string[] strs = strData.Split(splitStr);
        this.id = this.GetInt(strs[0]);

        for (int i = 1; i < strs.Length; i++)
        {
            if (!string.IsNullOrEmpty(strs[i]))
            {
                langList.Add(strs[i]);
            }
        }
    }

    public string GetContentByIndex(int index)
    {
        if (index < langList.Count)
        {
            return langList[index];
        }
        return string.Empty;
    }
}

class LanguageDataBaseMap : IDataBaseMap
{
    private Dictionary<int,LanguageDataBase> dict = new Dictionary<int, LanguageDataBase>();

    public void InitMap(string[] rows)
    {
        dict.Clear();
        for (int i = 1; i < rows.Length; i++)
        {
            LanguageDataBase dataBase = new LanguageDataBase();
            dataBase.InitWithStr(rows[i]);
            dict.Add(dataBase.id,dataBase);
        }
    }

    public LanguageDataBase GetDataById(int id)
    {
        if (dict.ContainsKey(id)) return dict[id];
        return null;
    }
}