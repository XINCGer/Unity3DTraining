using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XMLReaderStreaming : MonoBehaviour
{

    private string _result;
    public Text text;

    public void Start()
    {
        StartCoroutine(LoadXML());  
    }

    
    /// <summary>
    /// 如前文所述，streamingAssets只能使用www来读取，
    /// 如果不是使用www来读取的同学，就不要问为啥读不到streamingAssets下的内容了。
    /// 这里还可以使用persistenDataPath来保存从streamingassets那里读到内容。
    /// </summary>
    IEnumerator LoadXML()
    {
        string sPath = "file:// "+Application.streamingAssetsPath + "/Test.xml";
        WWW www = new WWW(sPath);
        yield return www;
        _result = www.text;
        text.text = _result;
    }
}
