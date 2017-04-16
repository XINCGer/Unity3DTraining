using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetBunbleReader : MonoBehaviour {

    private string _result;
    public Text text;

    // Use this for initialization
    void Start()
    {
        LoadXML();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void LoadXML()
    {
        AssetBundle AssetBundleCsv = new AssetBundle();
        //读取放入StreamingAssets文件夹中的bundle文件
        string str = Application.streamingAssetsPath + "/" + "TestXML.bundle";
        WWW www = new WWW(str);
        www = WWW.LoadFromCacheOrDownload(str, 0);
        AssetBundleCsv = www.assetBundle;

        string path = "Test";

        TextAsset test = AssetBundleCsv.Load(path, typeof(TextAsset)) as TextAsset;

        _result = test.ToString();
        text.text = _result;
    }
}
