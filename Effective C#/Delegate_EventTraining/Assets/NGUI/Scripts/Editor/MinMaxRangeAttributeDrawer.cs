//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeAttributeDrawer : PropertyDrawer
{
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label) + 16f;
	}

	public override void OnGUI (Rect rect, SerializedProperty property, GUIContent label)
	{
		if (property.type != "Vector2")
		{
			EditorGUI.LabelField(rect, label, "Error: MinMaxRange requires a Vector2");
		}
		else
		{
			MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
			var minProperty = property.FindPropertyRelative("x");
			var maxProperty = property.FindPropertyRelative("y");

			EditorGUI.PrefixLabel(rect, label);
			label.text = "";

			//++EditorGUI.indentLevel;
			var indent = EditorGUI.indentLevel * 14f;

			EditorGUI.PropertyField(new Rect(rect.x, rect.y + 16f, 50f + indent, 16f), minProperty, label);
			label.text = "";
			EditorGUI.PropertyField(new Rect(rect.x + rect.width - 50f - indent, rect.y + 16f, 50f + indent, 16f), maxProperty, label);

			float min = minProperty.floatValue;
			float max = maxProperty.floatValue;

			GUI.changed = false;
			EditorGUI.MinMaxSlider(new Rect(rect.x + 60f, rect.y + 16f, rect.width - 120f, 16f), ref min, ref max, range.minLimit, range.maxLimit);
			//--EditorGUI.indentLevel;

			if (GUI.changed)
			{
				minProperty.floatValue = min;
				maxProperty.floatValue = max;
			}
		}
	}
}
