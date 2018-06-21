using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 编辑器窗口管理类
/// </summary>
public class EditorWindowMgr
{
    /// <summary>
    /// 所有打开的编辑器窗口的缓存列表
    /// </summary>
    private static List<EditorWindowBase> windowList = new List<EditorWindowBase>();

    /// <summary>
    /// 重复弹出的窗口的优先级
    /// </summary>
    private static int repeateWindowPriroty = 10;

    /// <summary>
    /// 添加一个重复弹出的编辑器窗口到缓存中
    /// </summary>
    /// <param name="window"></param>
    public static void AddRepeateWindow(EditorWindowBase window)
    {
        repeateWindowPriroty++;
        window.Priority = repeateWindowPriroty;
        AddEditorWindow(window);
    }

    /// <summary>
    /// 从缓存中移除一个重复弹出的编辑器窗口
    /// </summary>
    /// <param name="window"></param>
    public static void RemoveRepeateWindow(EditorWindowBase window)
    {
        repeateWindowPriroty--;
        window.Priority = repeateWindowPriroty;
        RemoveEditorWindow(window);
    }

    /// <summary>
    /// 添加一个编辑器窗口到缓存中
    /// </summary>
    /// <param name="window"></param>
    public static void AddEditorWindow(EditorWindowBase window)
    {
        if (!windowList.Contains(window))
        {
            windowList.Add(window);
            SortWinList();
        }
    }

    /// <summary>
    /// 从缓存中移除一个编辑器窗口
    /// </summary>
    /// <param name="window"></param>
    public static void RemoveEditorWindow(EditorWindowBase window)
    {
        if (windowList.Contains(window))
        {
            windowList.Remove(window);
            SortWinList();
        }
    }

    /// <summary>
    /// 管理器强制刷新Window焦点
    /// </summary>
    public static void FoucusWindow()
    {
        if (windowList.Count > 0)
        {
            windowList[windowList.Count - 1].Focus();
        }
    }

    /// <summary>
    /// 关闭所有界面，并清理WindowList缓存
    /// </summary>
    public static void DestoryAllWindow()
    {
        foreach (EditorWindowBase window in windowList)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        windowList.Clear();
    }

    /// <summary>
    /// 对当前缓存窗口列表中的窗口按优先级升序排序
    /// </summary>
    private static void SortWinList()
    {
        windowList.Sort((x, y) =>
        {
            return x.Priority.CompareTo(y.Priority);
        });
    }
}
