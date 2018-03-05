
#pragma strict

@CustomEditor (Tonemapping)

class TonemappingEditor extends Editor 
{	
	var serObj : SerializedObject;	
	 
	var type : SerializedProperty;;
	
	// CURVE specific parameter
	var remapCurve : SerializedProperty;
	
	var exposureAdjustment : SerializedProperty;
	
	// REINHARD specific parameter
	var middleGrey : SerializedProperty;
	var white : SerializedProperty;
	var adaptionSpeed : SerializedProperty;
	var adaptiveTextureSize : SerializedProperty;

	function OnEnable () {
		serObj = new SerializedObject (target);
		
		type = serObj.FindProperty ("type");
		remapCurve = serObj.FindProperty ("remapCurve");
		exposureAdjustment = serObj.FindProperty ("exposureAdjustment");
		middleGrey = serObj.FindProperty ("middleGrey");
		white = serObj.FindProperty ("white");
		adaptionSpeed = serObj.FindProperty ("adaptionSpeed");
		adaptiveTextureSize = serObj.FindProperty("adaptiveTextureSize");
	}
    		
    function OnInspectorGUI () {        
    	serObj.Update ();
    	
		GUILayout.Label("Mapping HDR to LDR ranges since 1982", EditorStyles.miniLabel);
		
		var cam : Camera = (target as Tonemapping).GetComponent.<Camera>();
		if(cam != null) {
			if(!cam.hdr) {
				EditorGUILayout.HelpBox("The camera is not HDR enabled. This will likely break the Tonemapper.", MessageType.Warning);
			}
			else if(!(target as Tonemapping).validRenderTextureFormat) {
				EditorGUILayout.HelpBox("The input to Tonemapper is not in HDR. Make sure that all effects prior to this are executed in HDR.", MessageType.Warning);			
			}
		}
    	
    	EditorGUILayout.PropertyField (type, new GUIContent ("Technique"));

		if (type.enumValueIndex == Tonemapping.TonemapperType.UserCurve) {  	
    		EditorGUILayout.PropertyField (remapCurve, new GUIContent ("Remap curve", "Specify the mapping of luminances yourself"));
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.SimpleReinhard) {  	
    		EditorGUILayout.PropertyField (exposureAdjustment, new GUIContent ("Exposure", "Exposure adjustment"));			
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.Hable) { 	
    		EditorGUILayout.PropertyField (exposureAdjustment, new GUIContent ("Exposure", "Exposure adjustment"));			
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.Photographic) { 	
    		EditorGUILayout.PropertyField (exposureAdjustment, new GUIContent ("Exposure", "Exposure adjustment"));			
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.OptimizedHejiDawson) { 	
    		EditorGUILayout.PropertyField (exposureAdjustment, new GUIContent ("Exposure", "Exposure adjustment"));			
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.AdaptiveReinhard) {	
    		EditorGUILayout.PropertyField (middleGrey, new GUIContent ("Middle grey", "Middle grey defines the average luminance thus brightening or darkening the entire image."));	
    		EditorGUILayout.PropertyField (white, new GUIContent ("White", "Smallest luminance value that will be mapped to white"));	
    		EditorGUILayout.PropertyField (adaptionSpeed, new GUIContent ("Adaption Speed", "Speed modifier for the automatic adaption"));	
    		EditorGUILayout.PropertyField (adaptiveTextureSize, new GUIContent ("Texture size", "Defines the amount of downsamples needed."));		    				
		} else if (type.enumValueIndex == Tonemapping.TonemapperType.AdaptiveReinhardAutoWhite) {	
    		EditorGUILayout.PropertyField (middleGrey, new GUIContent ("Middle grey", "Middle grey defines the average luminance thus brightening or darkening the entire image."));	
    		EditorGUILayout.PropertyField (adaptionSpeed, new GUIContent ("Adaption Speed", "Speed modifier for the automatic adaption"));	
    		EditorGUILayout.PropertyField (adaptiveTextureSize, new GUIContent ("Texture size", "Defines the amount of downsamples needed."));		    				
		}
		
		GUILayout.Label("All following effects will use LDR color buffers", EditorStyles.miniBoldLabel);
    	
    	serObj.ApplyModifiedProperties();
    }
}
