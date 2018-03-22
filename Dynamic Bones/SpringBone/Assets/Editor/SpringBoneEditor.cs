using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpringBone))]
public class SpringBoneEditor : Editor {
    public override void OnInspectorGUI() {
        var t = target as SpringBone;
        var so = new SerializedObject(t);
        EditorGUILayout.PropertyField(so.FindProperty("springEnd"));

        EditorGUILayout.HelpBox("If you have don't have other(e.g. Animator) controlling rotation of this gameobject, enable this to fix its rotation. Otherwise don't use it.", MessageType.Info);
        EditorGUILayout.PropertyField(so.FindProperty("useSpecifiedRotation"),new GUIContent("Use custom rotation?"));
        if (t.useSpecifiedRotation) {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(so.FindProperty("customRotation"));
            if (GUILayout.Button("Copy current rotation")) {
                t.customRotation = t.transform.localRotation.eulerAngles;
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("Forces");
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(so.FindProperty("stiffness"));
        EditorGUILayout.PropertyField(so.FindProperty("bounciness"));
        EditorGUILayout.PropertyField(so.FindProperty("dampness"));
        EditorGUI.indentLevel--;
        so.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        var t = target as SpringBone;
        var so = new SerializedObject(t);
        Handles.DrawDottedLine(t.transform.position, t.transform.TransformPoint(t.springEnd),4.0f);
        var currentPos = t.transform.TransformPoint(t.springEnd);
        var size = HandleUtility.GetHandleSize(currentPos) * 0.2f;
        EditorGUI.BeginChangeCheck();
        var movedPos = Handles.FreeMoveHandle(currentPos, Quaternion.identity, size, Vector3.one * 0.5f, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck()) {
            so.FindProperty("springEnd").vector3Value =
                    t.transform.InverseTransformPoint(movedPos);
            so.ApplyModifiedProperties();
        }
    }
}
