using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 重复弹出的编辑器窗口
/// </summary>
public class RepeateWindow : EditorWindowBase {

    public static void Popup()
    {
        RepeateWindow window = new RepeateWindow();
        window.Show();
    }
}
