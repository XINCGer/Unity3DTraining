using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//自定义脚本挂在哪个物体上面，就会对哪个物体进行操作
[CustomEditor(typeof(ChangeObjectInfo))]
public class SceneEditor : Editor {

    private void OnSceneGUI() {

        //得到脚本所挂载的物体
        ChangeObjectInfo changeObjectInfo = (ChangeObjectInfo)target;
        Renderer renderer = changeObjectInfo.GetComponent<Renderer>();
        Handles.Label(changeObjectInfo.transform.position, changeObjectInfo.name + ":" + changeObjectInfo.transform.position.ToString());
        //开始绘制GUI
        Handles.BeginGUI();
        //规定GUI的显示区域
        GUILayout.BeginArea(new Rect(0, 0, 150, 200));
        //绘制文本提示
        GUILayout.Label("选择颜色");

        GUI.color = Color.red;
        if (GUILayout.Button("红色")) {
            Debug.Log("红色");
            renderer.sharedMaterial.color = Color.red;
        }
        GUI.color = Color.green;
        if (GUILayout.Button("绿色")) {
            Debug.Log("绿色");
            renderer.sharedMaterial.color = Color.green;
        }
        GUI.color = Color.blue;
        if (GUILayout.Button("蓝色")) {
            Debug.Log("蓝色");
            renderer.sharedMaterial.color = Color.blue;
        }
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}

