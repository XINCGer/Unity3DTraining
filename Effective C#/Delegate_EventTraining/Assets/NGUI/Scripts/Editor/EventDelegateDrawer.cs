//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Entry = PropertyReferenceDrawer.Entry;

/// <summary>
/// Draws a single event delegate. Contributed by Adam Byrd.
/// </summary>

[CustomPropertyDrawer(typeof(EventDelegate))]
public class EventDelegateDrawer : PropertyDrawer
{
	const int lineHeight = 16;

	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
	{
		SerializedProperty targetProp = prop.FindPropertyRelative("mTarget");
		if (targetProp.objectReferenceValue == null) return 2 * lineHeight;
		int lines = 3 * lineHeight;

		SerializedProperty methodProp = prop.FindPropertyRelative("mMethodName");

		EventDelegate del = new EventDelegate();
		del.target = targetProp.objectReferenceValue as MonoBehaviour;
		del.methodName = methodProp.stringValue;

		SerializedProperty paramArrayProp = prop.FindPropertyRelative("mParameters");
		EventDelegate.Parameter[] ps = del.parameters;

		if (ps != null)
		{
			paramArrayProp.arraySize = ps.Length;

			for (int i = 0; i < ps.Length; i++)
			{
				lines += lineHeight;

				SerializedProperty paramProp = paramArrayProp.GetArrayElementAtIndex(i);
				SerializedProperty objProp = paramProp.FindPropertyRelative("obj");
				UnityEngine.Object obj = objProp.objectReferenceValue;

				if (obj != null)
				{
					System.Type type = obj.GetType();
					GameObject selGO = null;
					if (type == typeof(GameObject)) selGO = obj as GameObject;
					else if (type.IsSubclassOf(typeof(Component))) selGO = (obj as Component).gameObject;
					if (selGO != null) lines += lineHeight;
				}
			}
		}
		return lines;
	}

	public override void OnGUI (Rect rect, SerializedProperty prop, GUIContent label)
	{
		Undo.RecordObject(prop.serializedObject.targetObject, "Delegate Selection");

		SerializedProperty targetProp = prop.FindPropertyRelative("mTarget");
		SerializedProperty methodProp = prop.FindPropertyRelative("mMethodName");

		MonoBehaviour target = targetProp.objectReferenceValue as MonoBehaviour;
		string methodName = methodProp.stringValue;

		EditorGUI.indentLevel = prop.depth;
		EditorGUI.LabelField(rect, label);

		Rect lineRect = rect;
		lineRect.yMin = rect.yMin + lineHeight;
		lineRect.yMax = lineRect.yMin + lineHeight;

		EditorGUI.indentLevel = targetProp.depth;
		target = EditorGUI.ObjectField(lineRect, "Notify", target, typeof(MonoBehaviour), true) as MonoBehaviour;
		targetProp.objectReferenceValue = target;

		if (target != null && target.gameObject != null)
		{
			GameObject go = target.gameObject;
			List<Entry> list = EventDelegateEditor.GetMethods(go);

			int index = 0;
			int choice = 0;

			EventDelegate del = new EventDelegate();
			del.target = target;
			del.methodName = methodName;
			string[] names = PropertyReferenceDrawer.GetNames(list, del.ToString(), out index);

			lineRect.yMin += lineHeight;
			lineRect.yMax += lineHeight;
			choice = EditorGUI.Popup(lineRect, "Method", index, names);

			if (choice > 0 && choice != index)
			{
				Entry entry = list[choice - 1];
				target = entry.target as MonoBehaviour;
				methodName = entry.name;
				targetProp.objectReferenceValue = target;
				methodProp.stringValue = methodName;
			}

			SerializedProperty paramArrayProp = prop.FindPropertyRelative("mParameters");
			EventDelegate.Parameter[] ps = del.parameters;

			if (ps != null)
			{
				paramArrayProp.arraySize = ps.Length;
				for (int i = 0; i < ps.Length; i++)
				{
					EventDelegate.Parameter param = ps[i];
					SerializedProperty paramProp = paramArrayProp.GetArrayElementAtIndex(i);
					SerializedProperty objProp = paramProp.FindPropertyRelative("obj");
					SerializedProperty fieldProp = paramProp.FindPropertyRelative("field");

					param.obj = objProp.objectReferenceValue;
					param.field = fieldProp.stringValue;
					Object obj = param.obj;

					lineRect.yMin += lineHeight;
					lineRect.yMax += lineHeight;

					obj = EditorGUI.ObjectField(lineRect, "   Arg " + i, obj, typeof(Object), true);

					objProp.objectReferenceValue = obj;
					del.parameters[i].obj = obj;
					param.obj = obj;

					if (obj == null) continue;

					GameObject selGO = null;
					System.Type type = param.obj.GetType();
					if (type == typeof(GameObject)) selGO = param.obj as GameObject;
					else if (type.IsSubclassOf(typeof(Component))) selGO = (param.obj as Component).gameObject;

					if (selGO != null)
					{
						// Parameters must be exact -- they can't be converted like property bindings
						PropertyReferenceDrawer.filter = param.expectedType;
						PropertyReferenceDrawer.canConvert = false;
						List<PropertyReferenceDrawer.Entry> ents = PropertyReferenceDrawer.GetProperties(selGO, true, false);

						int selection;
						string[] props = EventDelegateEditor.GetNames(ents, NGUITools.GetFuncName(param.obj, param.field), out selection);

						lineRect.yMin += lineHeight;
						lineRect.yMax += lineHeight;
						int newSel = EditorGUI.Popup(lineRect, " ", selection, props);

						if (newSel != selection)
						{
							if (newSel == 0)
							{
								param.obj = selGO;
								param.field = null;

								objProp.objectReferenceValue = selGO;
								fieldProp.stringValue = null;
							}
							else
							{
								param.obj = ents[newSel - 1].target;
								param.field = ents[newSel - 1].name;

								objProp.objectReferenceValue = param.obj;
								fieldProp.stringValue = param.field;
							}
						}
					}
					else if (!string.IsNullOrEmpty(param.field))
						param.field = null;

					PropertyReferenceDrawer.filter = typeof(void);
					PropertyReferenceDrawer.canConvert = true;
				}
			}
		}
	}
}
