using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 按钮点击次数计数器
/// </summary>
public class ButtonClickCounter : MonoBehaviour
{
    private int count = 0;
    private string btnName;

    void Start()
    {
        var text = this.transform.Find("Text").GetComponent<Text>();
        btnName = text.text;
    }


    public void Click()
    {
        count++;
        Debug.Log(string.Format("{0}点击了{1}次！", btnName, count));
    }
}
