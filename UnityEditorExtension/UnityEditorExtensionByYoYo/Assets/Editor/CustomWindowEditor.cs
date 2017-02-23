using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomWindowEditor : EditorWindow {
    private bool groundEnable = false;
    private float value1 = 0f;
    private float value2 = 0f;
    [MenuItem("GameObject/ShowWindow")]
    static void ShowWindow() {
        Rect theRect = new Rect(0, 0, 500, 500);
        CustomWindowEditor window = (CustomWindowEditor)EditorWindow.GetWindowWithRect(typeof(CustomWindowEditor), theRect, true, "我的窗口");
        window.Show();
    }

    void OnGUI() {
        if (GUILayout.Button("打开通知", GUILayout.Width(220))) {
            ShowNotification(new GUIContent("通知的内容"));
        }
        if (GUILayout.Button("关闭通知", GUILayout.Width(220))) {
            RemoveNotification();
        }
        if (GUILayout.Button("关闭窗口", GUILayout.Width(220))) {
            Close();
        }
        GUILayout.Label("以下是基础设置");
        groundEnable = EditorGUILayout.BeginToggleGroup("启用", groundEnable);
        value1 = EditorGUILayout.FloatField("值1", value1);
        value2 = EditorGUILayout.FloatField("值2", value2);
        EditorGUILayout.EndToggleGroup();
    }

    //窗体获得焦点的时候被调用
    void OnFocus() {
        Debug.Log("窗口获得了焦点！");
    }

    //窗口失去焦点的时候调用
    void OnLostFocus() {
        Debug.Log("窗口失去了焦点！");
    }

    void OnHierarchyChange()
    {
        Debug.Log("层次面板对象改变！");
    }

    void OnProjectChange()
    {
        Debug.Log("项目面板对象改变！");
    }

    void OnSelectionChange()
    {
        foreach (Transform itemTransform in Selection.transforms)
        {
            Debug.Log("被选中的物体是："+itemTransform.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log("窗口被关闭！");
    }

}
