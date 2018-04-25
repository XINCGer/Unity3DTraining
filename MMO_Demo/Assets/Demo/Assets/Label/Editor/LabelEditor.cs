using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (Label))]
public class LabelEditor : PropertyEditor
{
	private const float maxDistanceCap = 100.0f;
	
	private SerializedProperty labelTextProperty;
	private SerializedProperty customSkinProperty;
	private SerializedProperty styleNameProperty;
	private SerializedProperty guiCameraProperty;
	private SerializedProperty fadeDistanceProperty;
	private SerializedProperty hideDistanceProperty;
	private SerializedProperty maxViewAngleProperty;
	
	
	protected override void Initialize ()
	{
		labelTextProperty = serializedObject.FindProperty ("labelText");
		customSkinProperty = serializedObject.FindProperty ("customSkin");
		styleNameProperty = serializedObject.FindProperty ("styleName");
		guiCameraProperty = serializedObject.FindProperty ("guiCamera");
		fadeDistanceProperty = serializedObject.FindProperty ("fadeDistance");
		hideDistanceProperty = serializedObject.FindProperty ("hideDistance");
		maxViewAngleProperty = serializedObject.FindProperty ("maxViewAngle");
	}
	
	
	public override void OnInspectorGUI ()
	{
		BeginEdit ();
			BeginSection ("Contents");
				PropertyField ("Text", labelTextProperty);
			EndSection ();
			
			BeginSection ("View settings");
				PropertyField ("Camera", guiCameraProperty);
				Comment ("The camera displaying the GUI. Used for coordinates and distance checks.");
				MinMaxPropertySliderFields ("Fade and hide distance", fadeDistanceProperty, hideDistanceProperty, 0.0f, maxDistanceCap);
				PropertyField (maxViewAngleProperty);
			EndSection ();
		
			BeginSection ("Rendering");
				PropertyField ("Skin", customSkinProperty);
				Comment ("Leave unassigned to use the built in skin.");
				PropertyField ("Style name", styleNameProperty);
			EndSection ();
		EndEdit ();
	}
}
