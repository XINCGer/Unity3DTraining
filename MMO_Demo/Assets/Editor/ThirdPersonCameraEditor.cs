using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (ThirdPersonCamera))]
public class ThirdPersonCameraEditor : PropertyEditor
{
	private const float maxCameraDistanceCap = 100.0f;
	
	
	private SerializedProperty targetProperty;
	private SerializedProperty cameraProperty;
	private SerializedProperty obstacleLayersProperty;
	private SerializedProperty minDistanceProperty;
	private SerializedProperty maxDistanceProperty;
	private SerializedProperty groundLayersProperty;
	private SerializedProperty groundedCheckOffsetProperty;
	private SerializedProperty rotationUpdateSpeedProperty;
	private SerializedProperty lookUpSpeedProperty;
	private SerializedProperty zoomSpeedProperty;
	private SerializedProperty followUpdateSpeedProperty;
	private SerializedProperty distanceUpdateSpeedProperty;
	private SerializedProperty maxForwardAngleProperty;
	private SerializedProperty showGizmosProperty;
	private SerializedProperty requireLockProperty;
	private SerializedProperty controlLockProperty;
	
	
	protected override void Initialize ()
	{
		targetProperty = 				serializedObject.FindProperty ("target");
		cameraProperty = 				serializedObject.FindProperty ("camera");
		obstacleLayersProperty = 		serializedObject.FindProperty ("obstacleLayers");
		minDistanceProperty = 			serializedObject.FindProperty ("minDistance");
		maxDistanceProperty = 			serializedObject.FindProperty ("maxDistance");
		groundLayersProperty = 			serializedObject.FindProperty ("groundLayers");
		groundedCheckOffsetProperty =	serializedObject.FindProperty ("groundedCheckOffset");
		rotationUpdateSpeedProperty = 	serializedObject.FindProperty ("rotationUpdateSpeed");
		lookUpSpeedProperty = 			serializedObject.FindProperty ("lookUpSpeed");
		zoomSpeedProperty = 			serializedObject.FindProperty ("zoomSpeed");
		followUpdateSpeedProperty = 	serializedObject.FindProperty ("followUpdateSpeed");
		distanceUpdateSpeedProperty = 	serializedObject.FindProperty ("distanceUpdateSpeed");
		maxForwardAngleProperty = 		serializedObject.FindProperty ("maxForwardAngle");
		showGizmosProperty = 			serializedObject.FindProperty ("showGizmos");
		requireLockProperty = 			serializedObject.FindProperty ("requireLock");
		controlLockProperty = 			serializedObject.FindProperty ("controlLock");
	}
	
	
	public override void OnInspectorGUI ()
	{
		BeginEdit ();
			BeginSection ("Objects");
				PropertyField ("Viewed collider", targetProperty);
				PropertyField ("Camera", cameraProperty);
			EndSection ();
		
			BeginSection ("View obstruction");
				PropertyField ("Obstacle layers", obstacleLayersProperty);
				Comment ("Make sure that the target collider is not in any of these layers.");
				MinMaxPropertySliderFields ("Camera distance", minDistanceProperty, maxDistanceProperty, 0.0f, maxCameraDistanceCap);
			EndSection ();
		
			BeginSection ("Camera grounding check");
				PropertyField ("Ground layers", groundLayersProperty);
				Comment ("Make sure that the target collider is not in any of these layers.");
				PropertyField ("Offset", groundedCheckOffsetProperty);
			EndSection ();
		
			BeginSection ("Speed");
				PropertyField ("Horizontal rotation", rotationUpdateSpeedProperty);
				PropertyField ("Vertical rotation", lookUpSpeedProperty);
				PropertyField ("Zoom", zoomSpeedProperty);
				PropertyField ("Follow snap", followUpdateSpeedProperty);
				PropertyField ("Distance snap", distanceUpdateSpeedProperty);
			EndSection ();
			
			BeginSection ("Mouse control");
				PropertyField ("Require lock", requireLockProperty);
				PropertyField ("Control lock", controlLockProperty);
			EndSection ();
		
			PropertyField ("Angle clamp", maxForwardAngleProperty);
			PropertyField ("Show gizmos", showGizmosProperty);
		EndEdit ();
	}
	
	
	public override bool RenderSceneHandles
	{
		get
		{
			BeginEdit ();
			return showGizmosProperty.boolValue;
		}
	}
	
	
	public override Color SceneHandlesColor
	{
		get
		{
			return Color.blue;
		}
	}
	
	
	protected override void DoSceneGUI ()
	{
		BeginEdit ();
			float min = minDistanceProperty.floatValue, max = maxDistanceProperty.floatValue;
			MinMaxRadiusHandle (TargetTransform, ref min, ref max, 0.0f, maxCameraDistanceCap);
			minDistanceProperty.floatValue = min;
			maxDistanceProperty.floatValue = max;
				// Do a double wire sphere for modifying the min/max camera distance
			
			Color color = Handles.color;
			Handles.color = new Color (color.r, color.g, color.b, 0.1f);
			
			Handles.DrawSolidArc (
				TargetTransform.position,
				TargetTransform.right,
				TargetTransform.forward * -1.0f,
				maxForwardAngleProperty.floatValue,
				maxDistanceProperty.floatValue
			);
				// Render the camera area transparent
			
			Handles.color = color;
			
			DrawThickWireArc (
				TargetTransform.position,
				TargetTransform.forward * -1.0f,
				TargetTransform.right,
				maxForwardAngleProperty.floatValue,
				maxDistanceProperty.floatValue,
				20,
				0.005f
			);
				// Render the outline of the camera area on the camera arc

			maxForwardAngleProperty.floatValue = Mathf.Clamp (
				AngularSlider (
					TargetTransform.position,
					TargetTransform.forward * -1.0f,
					TargetTransform.up,
					TargetTransform.right,
					maxForwardAngleProperty.floatValue,
					maxDistanceProperty.floatValue,
					Handles.ArrowCap,
					20.0f * 0.005f * HandleUtility.GetHandleSize (TargetTransform.position),
					HandleUtility.GetHandleSize (TargetTransform.position)
				),
				0.0f,
				90.0f
			);
				// Make tha camera angle modifyable via an angular slider after the wire arc
		EndEdit ();
	}
}
