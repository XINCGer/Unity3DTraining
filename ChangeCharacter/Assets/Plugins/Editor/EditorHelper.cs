using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Object = UnityEngine.Object;

class EditorHelpers
{
	// 找到目录下指定类型的对象列表
	public static List<T> CollectAll<T>(string path) where T : Object
	{
		List<T> l = new List<T>();
		string[] files = Directory.GetFiles(path);
		
		foreach (string file in files)
		{
			if (file.Contains(".meta")) continue;
			T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
			if (asset == null) throw new Exception("Asset 不属于类型" + typeof(T) + ": " + file);
			l.Add(asset);
		}
		return l;
	}
}

