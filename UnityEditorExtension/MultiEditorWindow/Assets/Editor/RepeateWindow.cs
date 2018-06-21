using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeateWindow : EditorWindowBase {

    public static void Popup()
    {
        RepeateWindow window = new RepeateWindow();
        window.Show();
    }
}
