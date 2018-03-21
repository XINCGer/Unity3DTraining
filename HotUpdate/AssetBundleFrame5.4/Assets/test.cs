using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 查看当前Mono支持的.NET Frame的版本，用于确定
/// </summary>
public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
            MethodInfo info = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);

            if (info != null)
                Debug.Log(info.Invoke(null, null));
        }
	}

}
