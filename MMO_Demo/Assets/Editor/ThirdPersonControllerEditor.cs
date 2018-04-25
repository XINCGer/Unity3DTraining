using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (ThirdPersonController))]
public class ThirdPersonControllerEditor : PropertyEditor
{
	private SerializedProperty targetProperty;
	private SerializedProperty speedProperty;
	private SerializedProperty walkSpeedDownscaleProperty;
	private SerializedProperty turnSpeedProperty;
	private SerializedProperty mouseTurnSpeedProperty;
	private SerializedProperty jumpSpeedProperty;
	private SerializedProperty groundLayersProperty;
	private SerializedProperty groundedCheckOffsetProperty;
	private SerializedProperty showGizmosProperty;
	private SerializedProperty requireLockProperty;
	private SerializedProperty controlLockProperty;
	
	
	private const float rotationSpeedHandleScale = 20.0f;
		// Scales the visualization of the rotation speed handles. Reduce if you're dealing with larger rotation speeds.
	
	
	protected override void Initialize ()
	{
		targetProperty = 				serializedObject.FindProperty ("target");
		speedProperty = 				serializedObject.FindProperty ("speed");
		walkSpeedDownscaleProperty =	serializedObject.FindProperty ("walkSpeedDownscale");
		turnSpeedProperty = 			serializedObject.FindProperty ("turnSpeed");
		mouseTurnSpeedProperty = 		serializedObject.FindProperty ("mouseTurnSpeed");
		jumpSpeedProperty = 			serializedObject.FindProperty ("jumpSpeed");
		groundLayersProperty = 			serializedObject.FindProperty ("groundLayers");
		groundedCheckOffsetProperty = 	serializedObject.FindProperty ("groundedCheckOffset");
		showGizmosProperty = 			serializedObject.FindProperty ("showGizmos");
		requireLockProperty = 			serializedObject.FindProperty ("requireLock");
		controlLockProperty = 			serializedObject.FindProperty ("controlLock");
	}
	
	
	public override void OnInspectorGUI ()
	{
		BeginEdit ();
			BeginSection ("Target character");
				PropertyField ("Rigidbody", targetProperty);
			EndSection ();
			
			BeginSection ("Speed");
				PropertyField ("Movement", speedProperty);
				PropertyField ("Walk downscale", walkSpeedDownscaleProperty);
				PropertyField ("Turn", turnSpeedProperty);
				PropertyField ("Mouse turn", mouseTurnSpeedProperty);
				PropertyField ("Jump", jumpSpeedProperty);
			EndSection ();
			
			BeginSection ("Grounding check");
				PropertyField ("Layers", groundLayersProperty);
				Comment ("This should include anything that the character can land on. Make sure that any part of the character is not in any of these layers.");
				PropertyField ("Offset", groundedCheckOffsetProperty);
			EndSection ();
			
			BeginSection ("Mouse control");
				PropertyField ("Require lock", requireLockProperty);
				PropertyField ("Control lock", controlLockProperty);
			EndSection ();
			
			PropertyField ("Show gizmos", showGizmosProperty);
			
			EndSection ();
			
			WideComment ("This component uses more input than is included in the default input setup:\n\n - An extra axis named \"Sidestep\" - a straight copy of the \"Horizontal\" input axis - mapped to Q (negative) and E (positive).\n\n - An extra button named \"ToggleWalk\" - same setup as the \"Jump\" button, by default mapped to \"+\" (positive).");
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
			return Color.red;
		}
	}

	
	protected override void DoSceneGUI ()
	{
		BeginEdit ();
			speedProperty.floatValue = Handles.RadiusHandle (TargetTransform.rotation, TargetTransform.position, speedProperty.floatValue);
				// Do a wire sphere handle for modifying the speed as a radius
			
			float visualizedRotationAngle = turnSpeedProperty.floatValue * rotationSpeedHandleScale;
				// Scaling up the angle used in visualization of the rotation speed as we're dealing with low values per default
			
			DrawThickWireArc (TargetTransform, visualizedRotationAngle, speedProperty.floatValue, 20, 0.005f);
				// Draw the indication of the rotation speed as an angle segment of the planar circle, indicating speed
			
			float change = AngularSlider (
				TargetTransform,
				visualizedRotationAngle,
				speedProperty.floatValue,
				20.0f * 0.005f * HandleUtility.GetHandleSize (TargetTransform.position)
			) - visualizedRotationAngle;
				// Do the slider handle, allowing us to modify the rotation speed from the scene view
			
			if (visualizedRotationAngle + change < 360.0f)
			// Don't allow dragging over 360 degrees. This check is needed since we're scaling up the visual representation of the angle.
			{
				turnSpeedProperty.floatValue = Mathf.Clamp (turnSpeedProperty.floatValue + change / rotationSpeedHandleScale, 0.0f, 360.0f);
			}
			else
			// Clamp to 360
			{
				turnSpeedProperty.floatValue = 360.0f / rotationSpeedHandleScale;
			}
		EndEdit ();
	}
}
