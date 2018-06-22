using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器主界面
/// </summary>
public class MainWindow : EditorWindowBase
{
    private static MainWindow window;
    private static Vector2 minResolution = new Vector2(800, 600);
    private static Rect middleCenterRect = new Rect(200, 100, 400, 400);
    private GUIStyle labelStyle;

    /// <summary>
    /// 对外的访问接口
    /// </summary>
    [MenuItem("Tools/RepeateWindow")]
    public static void Popup()
    {
        window = EditorWindow.GetWindow(typeof(MainWindow), true, "多重窗口编辑器") as MainWindow;
        window.minSize = minResolution;
        window.Init();
        EditorWindowMgr.AddEditorWindow(window);
        window.Show();
    }

    /// <summary>
    /// 在这里可以做一些初始化工作
    /// </summary>
    private void Init()
    {
        Priority = 1;

        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.red;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 14;
        labelStyle.border = new RectOffset(1, 1, 2, 2);
    }

    private void OnGUI()
    {
        ShowEditorGUI();
    }

    /// <summary>
    /// 绘制编辑器界面
    /// </summary>
    private void ShowEditorGUI()
    {
        GUILayout.BeginArea(middleCenterRect);
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("点击下面的按钮创建重复弹出窗口", labelStyle, GUILayout.Width(220));
        if (GUILayout.Button("创建窗口", GUILayout.Width(80)))
        {
            RepeateWindow.Popup(window.position.position);
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void OnDestroy()
    {
        //主界面销毁的时候，附带销毁创建出来的子界面
        EditorWindowMgr.RemoveEditorWindow(window);
        EditorWindowMgr.DestoryAllWindow();
    }

    private void OnFocus()
    {
        //重写OnFocus方法，让EditorWindowMgr去自动排序汇聚焦点
        EditorWindowMgr.FoucusWindow();
    }
}
