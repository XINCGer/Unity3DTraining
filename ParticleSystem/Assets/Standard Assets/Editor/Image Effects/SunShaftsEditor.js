
#pragma strict

@CustomEditor (SunShafts)

class SunShaftsEditor extends Editor 
{	
	var serObj : SerializedObject;	
		
	var sunTransform : SerializedProperty;
	var radialBlurIterations : SerializedProperty;
	var sunColor : SerializedProperty;
	var sunShaftBlurRadius : SerializedProperty;
	var sunShaftIntensity : SerializedProperty;
	var useSkyBoxAlpha : SerializedProperty;
	var useDepthTexture : SerializedProperty;
    var resolution : SerializedProperty;
    var screenBlendMode : SerializedProperty;
    var maxRadius : SerializedProperty;

	function OnEnable () {
		serObj = new SerializedObject (target);
		
		screenBlendMode = serObj.FindProperty("screenBlendMode");
		
		sunTransform = serObj.FindProperty("sunTransform");
		sunColor = serObj.FindProperty("sunColor");
		
		sunShaftBlurRadius = serObj.FindProperty("sunShaftBlurRadius");
		radialBlurIterations = serObj.FindProperty("radialBlurIterations");
		
		sunShaftIntensity = serObj.FindProperty("sunShaftIntensity");
		useSkyBoxAlpha = serObj.FindProperty("useSkyBoxAlpha");
        
        resolution =  serObj.FindProperty("resolution");
        
        maxRadius = serObj.FindProperty("maxRadius"); 
		
		useDepthTexture = serObj.FindProperty("useDepthTexture");
	}
    		
    function OnInspectorGUI () {        
    	serObj.Update ();
    	
    	EditorGUILayout.BeginHorizontal();
    	
		var oldVal : boolean = useDepthTexture.boolValue;
		EditorGUILayout.PropertyField (useDepthTexture, new GUIContent ("Rely on Z Buffer?"));
		if((target as SunShafts).GetComponent.<Camera>())
			GUILayout.Label("Current camera mode: "+ (target as SunShafts).GetComponent.<Camera>().depthTextureMode, EditorStyles.miniBoldLabel);
		
    	EditorGUILayout.EndHorizontal();
		
		// depth buffer need
		/*
		var newVal : boolean = useDepthTexture.boolValue;
		if (newVal != oldVal) {
			if(newVal)
				(target as SunShafts).camera.depthTextureMode |= DepthTextureMode.Depth;
			else
				(target as SunShafts).camera.depthTextureMode &= ~DepthTextureMode.Depth;
		}
		*/
		
    	EditorGUILayout.PropertyField (resolution,  new GUIContent("Resolution"));
     	EditorGUILayout.PropertyField (screenBlendMode, new GUIContent("Blend mode"));
       
        EditorGUILayout.Separator ();
    
    	EditorGUILayout.BeginHorizontal();
    
    	EditorGUILayout.PropertyField (sunTransform, new GUIContent("Shafts caster", "Chose a transform that acts as a root point for the produced sun shafts"));
    	if((target as SunShafts).sunTransform && (target as SunShafts).GetComponent.<Camera>()) {
    		if (GUILayout.Button("Center on " + (target as SunShafts).GetComponent.<Camera>().name)) {
    			 if (EditorUtility.DisplayDialog ("Move sun shafts source?", "The SunShafts caster named "+ (target as SunShafts).sunTransform.name +"\n will be centered along "+(target as SunShafts).GetComponent.<Camera>().name+". Are you sure? ", "Please do", "Don't")) {
    				var ray : Ray = (target as SunShafts).GetComponent.<Camera>().ViewportPointToRay(Vector3(0.5,0.5,0));
    				(target as SunShafts).sunTransform.position = ray.origin + ray.direction * 500.0;
    				(target as SunShafts).sunTransform.LookAt ((target as SunShafts).transform);
    			}
    		}
    	}
    	
    	EditorGUILayout.EndHorizontal();
    	
		EditorGUILayout.Separator ();
    	
    	EditorGUILayout.PropertyField (sunColor,  new GUIContent ("Shafts color"));
        maxRadius.floatValue = 1.0f - EditorGUILayout.Slider ("Distance falloff", 1.0f - maxRadius.floatValue, 0.1, 1.0);
    	
    	EditorGUILayout.Separator ();
    	
    	sunShaftBlurRadius.floatValue = EditorGUILayout.Slider ("Blur size", sunShaftBlurRadius.floatValue, 1.0, 10.0);
    	radialBlurIterations.intValue = EditorGUILayout.IntSlider ("Blur iterations", radialBlurIterations.intValue, 1, 3);
    	
    	EditorGUILayout.Separator ();
    	
    	EditorGUILayout.PropertyField (sunShaftIntensity,  new GUIContent("Intensity"));
    	useSkyBoxAlpha.floatValue = EditorGUILayout.Slider ("Use alpha mask", useSkyBoxAlpha.floatValue, 0.0, 1.0);    	
    	
    	serObj.ApplyModifiedProperties();
    }
}
