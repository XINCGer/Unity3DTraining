using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    public Text title;
    public Text startBtnText;
    public Text continueBtnText;
    public Text exitBtnText;

    // Use this for initialization
    void Start()
    {
        IsNull(title,"welcome");
        IsNull(startBtnText, "start");
        IsNull(continueBtnText, "continue");
        IsNull(exitBtnText, "exit");
    }

    /// <summary>
    /// 判空操作
    /// </summary>
    /// <param name="text"></param>text组件
    /// <param name="key"></param>对应的键值
    public void IsNull(Text text, string key)
    {
        if (null != text)
        {
            string value = LanguageMgr.GetInstance().GetText(key);
            if (!string.IsNullOrEmpty(value))
            {
                text.text = value;
            }
        }
    }
}
