using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChangeInspector))]
public class InspectEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ChangeInspector inspector = (ChangeInspector)target;
        //绘制贴图槽
        inspector.texture = EditorGUILayout.ObjectField("选择贴图", inspector.texture, typeof(Texture), true) as Texture;
        inspector.rectValue = EditorGUILayout.RectField("窗口坐标", inspector.rectValue);
        inspector.remark = EditorGUILayout.TextField("备注", inspector.remark);
    }
}
