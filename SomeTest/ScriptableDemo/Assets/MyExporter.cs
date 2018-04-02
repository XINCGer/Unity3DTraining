using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MyExporter: MonoBehaviour 
{
	[MenuItem("Assets/MyExporter")]
	static void MyExec()
	{
		MyData md = ScriptableObject.CreateInstance<MyData>();
		md.content = new List<Vector3>();
		md.content.Add(new Vector3(0, 1, 2));
		md.content.Add(new Vector3(2, 3, 4));
		md.content.Add(new Vector3(3, 4, 5));
		string a = "Assets/MyData.asset";
		AssetDatabase.CreateAsset(md, a);
		Object o = AssetDatabase.LoadAssetAtPath(a, typeof(MyData));
		string b = "Assets/MyData.assetbundle";
		BuildPipeline.BuildAssetBundle(o, null, b);
		AssetDatabase.DeleteAsset(a);
	}
}
