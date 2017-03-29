using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MoringDelegate(string name);

public class UseDelegate : MonoBehaviour
{

    public UIButton button;
    // Use this for initialization
    void Start()
    {
        EventDelegate.Add(this.button.onClick, this.BtnClick);
    }

    private void BtnClick()
    {
        GoogMoring("msxher", MoringEnglish);
        GoogMoring("马三小伙儿", MoringChinese);
    }

    private void GoogMoring(string name, MoringDelegate moringLanguage)
    {
        moringLanguage(name);
    }

    private void MoringEnglish(string name)
    {
        Debug.Log("GoodMoring!" + name);
    }

    private void MoringChinese(string name)
    {
        Debug.Log("早上好！" + name);
    }
}
