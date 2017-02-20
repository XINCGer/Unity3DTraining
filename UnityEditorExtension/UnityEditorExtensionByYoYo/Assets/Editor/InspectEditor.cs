using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tool;

[CustomEditor(typeof(ChangeInspector))]
public class InspectEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ChangeInspector inspector = (ChangeInspector)target;
        //绘制贴图槽
        inspector.Texture1 = EditorGUILayout.ObjectField("选择贴图", inspector.Texture1, typeof(Texture), true) as Texture;
        inspector.RectValue = EditorGUILayout.RectField("窗口坐标", inspector.RectValue);
        inspector.Remark = EditorGUILayout.TextField("备注", inspector.Remark);
        //绘制滑动条
        inspector.SliderValue = EditorGUILayout.Slider("进度值", inspector.SliderValue, 0, 1f);
        inspector.IsOpen = EditorGUILayout.Toggle("开启", inspector.IsOpen);
        inspector.Type = (EDirType) EditorGUILayout.EnumPopup("方向", inspector.Type);
    }
}
