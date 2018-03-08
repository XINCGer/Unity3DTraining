using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainLoop : MonoBehaviour
{

    private GameObject uiRoot;

    // Use this for initialization
    void Start()
    {
        uiRoot = GameObject.Find("Canvas");
        Button startButton = uiRoot.transform.Find("Button").gameObject.GetComponent<Button>();
        startButton.onClick.AddListener(() =>
        {
            GameObject uiLoginPanl = AssetBundleMgr.GetInstance().LoadAssetFromStreamingAsset<GameObject>("uiLoginPanel");
            GameObject uiLoginObj = GameObject.Instantiate(uiLoginPanl);
            uiLoginObj.transform.SetParent(uiRoot.transform, false);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
