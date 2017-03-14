using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// Json解析脚本挂在场景中JsonManager上
/// </summary>
public class JsonDeserialize : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        //获取Json文件
	    TextAsset jsonData = Resources.Load<TextAsset>("JsonData");
	    JsonObjectModel jsonObject = JsonMapper.ToObject<JsonObjectModel>(jsonData.text);
	    foreach (var info in jsonObject.infoList)
	    {
	        Debug.Log(info.panelTypeString+" : "+info.path);
	    }
	}



}
