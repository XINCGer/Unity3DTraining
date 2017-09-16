using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{

    [SerializeField]
    private string key;
    // Use this for initialization
    void Start()
    {
        if (!string.IsNullOrEmpty(key))
        {
            string value = LanguageMgr.GetInstance().GetText(key);
            if (!string.IsNullOrEmpty(value))
            {
                GetComponent<Text>().text = value;
            }
        }
    }

}
