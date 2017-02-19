using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemTool : MonoBehaviour {

    [MenuItem("ClearMemory/ClearAll", false, 1)]
    static void ClearAll() {
        Debug.Log("清除所有的缓存！");
    }
    [MenuItem("ClearMemory/ClearOther/ClearShopInfo", false, 12)]
    static void ClearShopInfo() {
        Debug.Log("清除商店的缓存！");
    }
    [MenuItem("ClearMemory/ClearOther/ClearPropInfo", false, 13)]
    static void ClearPropInfo() {
        Debug.Log("清除属性的缓存！");
    }
    [MenuItem("ClearMemory/ClearGameInfo", false, 14)]
    static void ClearGameInfo() {
        Debug.Log("清除游戏的缓存！");
    }
    [MenuItem("GameObject/Test", false, 10)]
    static void Test() {
        Debug.Log("测试！");
    }
}
