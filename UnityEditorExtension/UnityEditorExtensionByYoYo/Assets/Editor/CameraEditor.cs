using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Camera))]
public class CameraEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        GUILayout.Label("******以下是拓展选项******");
        if (GUILayout.Button("点击处理"))
        {
            Debug.Log("检测到点击事件！");
        }
    }
}
