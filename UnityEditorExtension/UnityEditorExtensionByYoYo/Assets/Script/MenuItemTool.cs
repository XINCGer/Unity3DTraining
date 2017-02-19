using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemTool : MonoBehaviour {

    [MenuItem("ClearMemory/ClearAll",false,1)]
    static void ClearAll() {
        Debug.Log("清除所有的缓存！");
    }
    [MenuItem("ClearMemory/ClearOther/ClearShopInfo",false,2)]
    static void ClearShopInfo() {
        Debug.Log("清除商店的缓存！");
    }
    [MenuItem("ClearMemory/ClearOther/ClearPropInfo",false,3)]
    static void ClearPropInfo() {
        Debug.Log("清除属性的缓存！");
    }
}
