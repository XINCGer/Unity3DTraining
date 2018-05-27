using UnityEngine;
using UnityEditor;
public class NodeEditor : EditorWindow
{
    //窗口的矩形
    Rect windowRect = new Rect(10, 10, 100, 100);
    Rect windowRect2 = new Rect(210, 210, 100, 100);
    static Rect MainRect = new Rect(0,0,800,600);
    //窗口的ID
    int windownID = 1;
    int windownID2 = 2;
    private bool isLink = false;

    [MenuItem("Tools/NodeEditor")]
    public static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindowWithRect<NodeEditor>(MainRect, false, "节点编辑器", true);
        editor.Show();
    }

    void OnGUI()
    {
        //绘画窗口
        BeginWindows();
        windowRect = GUI.Window(windownID, windowRect, DrawNodeWindow, "Demo Window");
        windowRect2 = GUI.Window(windownID2, windowRect2, DrawNodeWindow, "Demo Window2");
        EndWindows();
        if (isLink)
        {
            //连接窗口
            DrawNodeCurve(windowRect, windowRect2, Color.green);
        }
    }
    //绘画窗口函数
    void DrawNodeWindow(int id)
    {
        //创建一个GUI Button
        if (GUILayout.Button("Link"))
        {
            isLink = true;
        }
        if (GUILayout.Button("UnLink"))
        {
            isLink = false;
        }
        //设置改窗口可以拖动
        GUI.DragWindow();
    }

    void DrawNodeCurve(Rect start, Rect end, Color color)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 4);
    }
}