using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Tool;

[CustomEditor(typeof(ChangeInspector))]
public class InspectEditor : Editor {
    private bool groundEnable = false;
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
        inspector.Type = (EDirType)EditorGUILayout.EnumPopup("方向", inspector.Type);

        //绘制一个复选框组
        GUILayout.Label("*****以下是附加设置*****");
        groundEnable = EditorGUILayout.BeginToggleGroup("是否开启附加设置", groundEnable);
        inspector.TheValue1 = EditorGUILayout.FloatField("值1", inspector.TheValue1);
        inspector.TheValue2 = EditorGUILayout.FloatField("值2", inspector.TheValue2);
        inspector.IsAdd = EditorGUILayout.Toggle("开启", inspector.IsAdd);
        EditorGUILayout.EndToggleGroup();
    }
}
