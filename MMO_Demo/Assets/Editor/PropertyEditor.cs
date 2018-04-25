using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class PropertyEditor : Editor
{
	protected SerializedObject serializedObject;
	
	
	private static GUIStyle commentStyle = null;
	private static bool cameraRendered = false;
	
	
	// General //
	
	
	protected abstract void Initialize ();
	
	
	public void BeginEdit ()
	{
		if (serializedObject != null && serializedObject.targetObject == target)
		{
			serializedObject.Update ();
			return;
		}
		
		serializedObject = new SerializedObject (target);

		Initialize ();
	}
	
	
	public void EndEdit ()
	{
		serializedObject.ApplyModifiedProperties ();
	}
	
	
	// Inspector GUI //
	
	
	public static GUIStyle CommentStyle
	{
		get
		{
			if (commentStyle == null)
			{
				commentStyle = new GUIStyle (GUI.skin.GetStyle ("Box"));
				commentStyle.font = EditorStyles.miniFont;
				commentStyle.alignment = TextAnchor.UpperLeft;
			}
			
			return commentStyle;
		}
	}
	
	
	protected void PropertyField (string label, SerializedProperty property, params GUILayoutOption[] options)
	{
		if (string.IsNullOrEmpty (label))
		{
			EditorGUILayout.PropertyField (property, options);
		}
		else
		{
			EditorGUILayout.PropertyField (property, new GUIContent (label), options);
		}
	}
	
	
	protected void PropertyField (SerializedProperty property, params GUILayoutOption[] options)
	{
		PropertyField (null, property, options);
	}
	
	
	protected void FloatPropertyField (SerializedProperty property, params GUILayoutOption[] options)
	{
		float newValue = EditorGUILayout.FloatField (property.floatValue, options);
		if (newValue != property.floatValue)
		{
			property.floatValue = newValue;
		}
	}
	
	
	protected void StringPropertyField (SerializedProperty property, params GUILayoutOption[] options)
	{
		string newValue = EditorGUILayout.TextField (property.stringValue, options);
		if (newValue != property.stringValue)
		{
			property.stringValue = newValue;
		}
	}
	
	
	protected void TexturePropertyField (SerializedProperty property, params GUILayoutOption[] options)
	{
		Object newValue = EditorGUILayout.ObjectField (property.objectReferenceValue, typeof (Texture2D), options);
		if (newValue != property.objectReferenceValue)
		{
			property.objectReferenceValue = newValue;
		}
	}
	
	
	protected void MinMaxPropertySlider (SerializedProperty minProperty, SerializedProperty maxProperty, float minCap, float maxCap, params GUILayoutOption[] options)
	{
		float newMin = minProperty.floatValue, newMax = maxProperty.floatValue;
		EditorGUILayout.MinMaxSlider (ref newMin, ref newMax, minCap, maxCap, options);
		
		if (newMin != minProperty.floatValue || newMax != maxProperty.floatValue)
		{
			minProperty.floatValue = newMin;
			maxProperty.floatValue = newMax;
		}
	}
	
	
	protected void MinMaxPropertySliderFields (string label, SerializedProperty minProperty, SerializedProperty maxProperty, float minCap, float maxCap, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal ();
			GUILayout.Space (5.0f);
			Rect labelRect = GUILayoutUtility.GetRect (new GUIContent (label), EditorStyles.boldLabel);
			GUI.Label (labelRect, label, minProperty.prefabOverride || maxProperty.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label);
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
			GUILayout.Space (15.0f);
			FloatPropertyField (minProperty, GUILayout.Width (40.0f));
			MinMaxPropertySlider (minProperty, maxProperty, minCap, maxCap, options);
			FloatPropertyField (maxProperty, GUILayout.Width (40.0f));
		GUILayout.EndHorizontal ();
	}
	
	
	public static void WideComment (string comment)
	{
		GUILayout.Box (comment, CommentStyle, GUILayout.ExpandWidth (true));
	}
	
	
	public static void Comment (string comment)
	{
		GUILayout.BeginHorizontal ();
			GUILayout.Space (105.0f);
			WideComment (comment);
		GUILayout.EndHorizontal ();
	}
	
	
	public static void Header (string label)
	{
		GUILayout.Label (label, EditorStyles.boldLabel);
	}
	
	
	public static void BeginSection (string label)
	{
		Header (label);
	}
	
	
	public static void EndSection ()
	{
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	}
	
	
	// Scene GUI //
	
	
	public virtual bool RenderSceneHandles
	{
		get
		{
			return true;
		}
	}
	
	
	public virtual Color SceneHandlesColor
	{
		get
		{
			return Color.green;
		}
	}
	
	
	public Transform TargetTransform
	{
		get
		{
			return ((Component)target).transform;
		}
	}
	
	
	protected virtual void DoSceneGUI ()
	// Implement your scene GUI in here for automatic camera, on/off and colour handling
	{
		
	}
	
	
	public void OnPreSceneGUI ()
	{
		cameraRendered = false;
	}
	
	
	public void OnSceneGUI ()
	{
		if (!RenderSceneHandles)
		{
			return;
		}
		
		if (!cameraRendered)
		{
			Handles.DrawCamera (new Rect (0.0f, 0.0f, Screen.width, Screen.height), Camera.current);
			cameraRendered = true;
		}
		
		Handles.color = SceneHandlesColor;
		
		DoSceneGUI ();
	}
	
	
	public static float AngularSlider (Vector3 position, Vector3 forward, Vector3 right, Vector3 up, float angle, float radius, Handles.DrawCapFunction capFunction, float offset = 0.0f, float handleSize = 1.0f)
	// Create an angular slider for the given transform
	{
		Vector3 angleVector = PlanarAngleVector (forward, right, angle) * radius;
		Vector3 directionVector = Vector3.Cross (angleVector, up) * -1;
		Vector3 sliderPosition = position + angleVector + angleVector.normalized * offset;
		Vector3 changeVector = Handles.Slider (sliderPosition, directionVector, handleSize, capFunction, 1.0f) - sliderPosition;
		return angle + (Vector3.Angle (directionVector, changeVector) > 90.0f ? changeVector.magnitude * -1.0f : changeVector.magnitude);
	}
	
	
	public static float AngularSlider (Vector3 position, Vector3 forward, Vector3 right, Vector3 up, float angle, float radius, float offset = 0.0f)
	{
		return AngularSlider (position, forward, right, up, angle, radius, Handles.ArrowCap, offset, HandleUtility.GetHandleSize (position));
	}
	
	
	public static float AngularSlider (Transform transform, float angle, float radius, float offset = 0.0f)
	{
		return AngularSlider (transform.position, transform.forward, transform.right, transform.up, angle, radius, offset);
	}
	
	
	public static void DrawThickWireArc (Vector3 position, Vector3 forward, Vector3 up, float angle, float radius, int thickness, float resolution)
	// Draw a wire arc for a transform with a given thickness and resolution
	{
		for (int i = 0; i < thickness; i++)
		{
			Handles.DrawWireArc (
				position,
				up,
				forward,
				angle,
				radius + resolution * (float)i * HandleUtility.GetHandleSize (position)
			);
		}
	}
	
	
	public static void DrawThickWireArc (Transform transform, float angle, float radius, int thickness, float resolution)
	{
		DrawThickWireArc (transform.position, transform.forward, transform.up, angle, radius, thickness, resolution);
	}
	
	
	public static Vector3 PlanarAngleVector (Vector3 forward, Vector3 right, float angle)
	// Produce a planar directional vector - a set degrees from the forward vector of the given transform
	{
		if (angle < 90.0f)
		{
			return Vector3.Slerp (
				forward,
				right,
				angle / 90.0f
			);
		}
		else if (angle < 180.0f)
		{
			return Vector3.Slerp (
				right,
				forward * -1.0f,
				(angle - 90.0f) / 90.0f
			);
		}
		else if (angle < 270.0f)
		{
			return Vector3.Slerp (
				forward * -1.0f,
				right * -1.0f,
				(angle - 180.0f) / 90.0f
			);
		}
		else
		{
			return Vector3.Slerp (
				right * -1.0f,
				forward,
				(angle - 270.0f) / 90.0f
			);
		}
	}
	
	
	public static void MinMaxRadiusHandle (Transform transform, ref float min, ref float max, float minClamp, float maxClamp)
	// Produce two radius handles around the given transform, one rotated 45 degrees on the up vector, scaling a min and a max
	{
		min = Mathf.Clamp (Handles.RadiusHandle (transform.rotation, transform.position, min), minClamp, max);
		max = Mathf.Clamp (Handles.RadiusHandle (transform.rotation * Quaternion.AngleAxis (45.0f, transform.up), transform.position, max), min, maxClamp);
	}
}
