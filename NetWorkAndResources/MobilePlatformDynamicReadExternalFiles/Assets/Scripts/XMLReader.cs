using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class XMLReader : MonoBehaviour
{

    public Text text;
    private string result;

    // Use this for initialization
    void Start()
    {
        LoadXML();
        SetText();
    }
    
    
    private void LoadXML()
    {
        result = Resources.Load<TextAsset>("Test").ToString();
        XmlDocument xmlReader = new XmlDocument();
        xmlReader.LoadXml(result);
    }

    private void SetText()
    {
        text.text = result;

    }
}
