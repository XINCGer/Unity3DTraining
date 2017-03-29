using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不使用委托和事件的方式
/// </summary>
public class NoDelegate : MonoBehaviour
{
    public enum Language
    {
        Chinese,
        English
    }
    public UIButton button;
    // Use this for initialization
    void Start()
    {
        EventDelegate.Add(this.button.onClick, this.BtnClick);
    }

    private void BtnClick()
    {
        GoodMorning("msxher", Language.Chinese);
    }

    private void GoodMorning(string name, Language language)
    {
        switch (language)
        {
            case Language.Chinese:
                MoringChinese(name);
                break;
            case Language.English:
                MoringEnglish(name);
                break;

        }
    }

    private void MoringEnglish(string name)
    {
        Debug.Log("Goodmoring!" + name);
    }

    private void MoringChinese(string name)
    {
        Debug.Log("早上好！" + name);
    }

}
