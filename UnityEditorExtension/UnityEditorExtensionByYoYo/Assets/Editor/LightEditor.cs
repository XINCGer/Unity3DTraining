using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Light))]
public class LightEditor : Editor {
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();
        if (GUILayout.Button("点击")) {
            Debug.Log("灯光点击响应！");
        }
    }
}
