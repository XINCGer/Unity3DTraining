using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器窗口基类
/// </summary>
public class EditorWindowBase : EditorWindow
{
    /// <summary>
    /// 界面层级管理，根据界面优先级访问界面焦点
    /// </summary>
    public int Priority { get; set; }

    private void OnFocus()
    {
        //重写OnFocus方法，让EditorWindowMgr去自动排序汇聚焦点
        EditorWindowMgr.FoucusWindow();
    }
}
