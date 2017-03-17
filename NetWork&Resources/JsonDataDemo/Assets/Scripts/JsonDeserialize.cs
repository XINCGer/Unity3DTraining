using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;

/// <summary>
/// Json解析脚本挂在场景中JsonManager上
/// </summary>
public class JsonDeserialize : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        /*********************** 使用自定义结构类读取Json ******************************/
        //获取Json文件
        //TextAsset jsonData = Resources.Load<TextAsset>("JsonData");
        //JsonObjectModel jsonObject = JsonMapper.ToObject<JsonObjectModel>(jsonData.text);
        //foreach (var info in jsonObject.infoList)
        //{
        //    Debug.Log(info.panelTypeString+" : "+info.path);
        //}
        /*********************** 使用自定义结构类读取Json ******************************/

        /*********************** 使用JsonData读取Json ******************************/
        //TextAsset jsonAsset = Resources.Load<TextAsset>("JsonData");
        //JsonData jsonData = JsonMapper.ToObject(jsonAsset.text);
        //Debug.LogWarning(jsonData.Count);
        //Debug.LogWarning(jsonData.IsArray);
        //Debug.LogWarning(jsonData.IsObject);
        //Debug.LogWarning(jsonData.Keys);
        //Debug.LogWarning(jsonData.ToString());
        //Debug.LogWarning(jsonData.GetJsonType());
        //foreach (JsonData  item in jsonData["infoList"])
        //{
        //    Debug.Log(item["panelTypeString"]);
        //    Debug.Log(item["path"]);
        //}
        /*********************** 使用JsonData读取Json ******************************/

        /*********************** 使用JsonReader ******************************/
        //TextAsset jsonData = Resources.Load<TextAsset>("JsonData");
        //JsonReader reader = new JsonReader(jsonData.text);
        //while (reader.Read())
        //{
        //    string type = reader.Value != null ? reader.Value.GetType().ToString() : "None";
        //    Debug.Log(reader.Token+" "+reader.Value+" "+type);
        //}
        //reader.Close();
        /*********************** 使用JsonReader ******************************/

        /*********************** 使用JsonWriter ******************************/
        //通过JsonWriter类创建一个Json对象
        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteArrayStart();
        writer.Write(1);
        writer.Write(2);
        writer.Write(3);
        writer.WriteObjectStart();
        writer.WritePropertyName("color");
        writer.Write("blue");
        writer.WriteObjectEnd();
        writer.WriteArrayEnd();
        Debug.Log(sb.ToString());
        /*********************** 使用JsonWriter ******************************/
	}



}
