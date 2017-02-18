using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuItemTool : MonoBehaviour {

    [MenuItem("ClearMemory/ClearAll")]
    static void ClearAll() {
        Debug.Log("清除所有的缓存！");
    }
}
