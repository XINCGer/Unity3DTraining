using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 重复弹出的编辑器窗口
/// </summary>
public class RepeateWindow : EditorWindowBase
{

    private static Vector2 minResolution = new Vector2(300, 200);
    private static Rect leftUpRect = new Rect(new Vector2(0, 0), minResolution);

    public static void Popup(Vector3 position)
    {
        // RepeateWindow window = new RepeateWindow();
        RepeateWindow window = GetWindowWithRectPrivate(typeof(RepeateWindow), leftUpRect, true, "重复弹出窗口") as RepeateWindow;
        window.minSize = minResolution;
        //要在设置位置之前，先把窗体注册到管理器中，以便更新窗体的优先级
        EditorWindowMgr.AddRepeateWindow(window);
        //刷新界面偏移量
        int offset = (window.Priority - 10) * 30;
        window.position = new Rect(new Vector2(position.x + offset, position.y + offset), new Vector2(800, 400));
        window.Show();
        //手动聚焦
        window.Focus();
    }

    /// <summary>
    /// 重写EditorWindow父类的创建窗口函数
    /// </summary>
    /// <param name="t"></param>
    /// <param name="rect"></param>
    /// <param name="utility"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    private static EditorWindow GetWindowWithRectPrivate(Type t, Rect rect, bool utility, string title)
    {
        //UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(t);
        EditorWindow editorWindow = null;/*= (array.Length <= 0) ? null : ((EditorWindow)array[0]);*/
        if (!(bool)editorWindow)
        {
            editorWindow = (ScriptableObject.CreateInstance(t) as EditorWindow);
            editorWindow.minSize = new Vector2(rect.width, rect.height);
            editorWindow.maxSize = new Vector2(rect.width, rect.height);
            editorWindow.position = rect;
            if (title != null)
            {
                editorWindow.titleContent = new GUIContent(title);
            }
            if (utility)
            {
                editorWindow.ShowUtility();
            }
            else
            {
                editorWindow.Show();
            }
        }
        else
        {
            editorWindow.Focus();
        }
        return editorWindow;
    }


    private void OnGUI()
    {
        OnEditorGUI();
    }

    private void OnEditorGUI()
    {
        GUILayout.Space(12);
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("我是重复弹出的窗体", GUILayout.Width(200));
        if (GUILayout.Button("创建窗体", GUILayout.Width(100)))
        {
            //重复创建自己
            Popup(this.position.position);
        }
        GUILayout.Space(12);
        if (GUILayout.Button("关闭窗体", GUILayout.Width(100)))
        {
            this.Close();
        }
        GUILayout.EndVertical();
    }

    private void OnDestroy()
    {
        //销毁窗体的时候，从管理器中移除该窗体的缓存，并且重新刷新焦点
        EditorWindowMgr.RemoveRepeateWindow(this);
        EditorWindowMgr.FoucusWindow();
    }

    private void OnFocus()
    {
        EditorWindowMgr.FoucusWindow();
    }
}
