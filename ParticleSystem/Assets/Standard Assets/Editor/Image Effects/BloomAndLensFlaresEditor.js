
#pragma strict

@CustomEditor (BloomAndLensFlares)
		
class BloomAndLensFlaresEditor extends Editor 
{	
	var tweakMode : SerializedProperty;
	var screenBlendMode : SerializedProperty;
	
	var serObj : SerializedObject;
	
	var hdr  : SerializedProperty;
	var sepBlurSpread : SerializedProperty;
	var useSrcAlphaAsMask : SerializedProperty;

	var bloomIntensity : SerializedProperty;
	var bloomThreshhold : SerializedProperty;
	var bloomBlurIterations : SerializedProperty;
	
	var lensflares : SerializedProperty;
	
	var hollywoodFlareBlurIterations : SerializedProperty;
	
	var lensflareMode : SerializedProperty;
	var hollyStretchWidth : SerializedProperty;
	var lensflareIntensity : SerializedProperty;
	var lensflareThreshhold : SerializedProperty;
	var flareColorA : SerializedProperty;
	var flareColorB : SerializedProperty;
	var flareColorC : SerializedProperty;
	var flareColorD : SerializedProperty;	
	
	var blurWidth : SerializedProperty;
	var lensFlareVignetteMask : SerializedProperty;

	function OnEnable () {
		serObj = new SerializedObject (target);
				
		screenBlendMode = serObj.FindProperty("screenBlendMode");
		hdr = serObj.FindProperty("hdr");
		
		sepBlurSpread = serObj.FindProperty("sepBlurSpread");
		useSrcAlphaAsMask = serObj.FindProperty("useSrcAlphaAsMask");
		
		bloomIntensity = serObj.FindProperty("bloomIntensity");
		bloomThreshhold = serObj.FindProperty("bloomThreshhold");
		bloomBlurIterations = serObj.FindProperty("bloomBlurIterations");
		
		lensflares = serObj.FindProperty("lensflares"); 
		
		lensflareMode = serObj.FindProperty("lensflareMode");
		hollywoodFlareBlurIterations = serObj.FindProperty("hollywoodFlareBlurIterations");
		hollyStretchWidth = serObj.FindProperty("hollyStretchWidth");
		lensflareIntensity = serObj.FindProperty("lensflareIntensity");
		lensflareThreshhold = serObj.FindProperty("lensflareThreshhold");
		flareColorA = serObj.FindProperty("flareColorA");
		flareColorB = serObj.FindProperty("flareColorB");
		flareColorC = serObj.FindProperty("flareColorC");
		flareColorD = serObj.FindProperty("flareColorD");		
		blurWidth = serObj.FindProperty("blurWidth");
		lensFlareVignetteMask = serObj.FindProperty("lensFlareVignetteMask");
		
		tweakMode = serObj.FindProperty("tweakMode");
	}
    		
    function OnInspectorGUI () {        
		serObj.Update();

		GUILayout.Label("HDR " + (hdr.enumValueIndex == 0 ? "auto detected, " : (hdr.enumValueIndex == 1 ? "forced on, " : "disabled, ")) + (useSrcAlphaAsMask.floatValue < 0.1f ? " ignoring alpha channel glow information" : " using alpha channel glow information"), EditorStyles.miniBoldLabel);		
		
		EditorGUILayout.PropertyField (tweakMode, new GUIContent("Tweak mode"));	
    	EditorGUILayout.PropertyField (screenBlendMode, new GUIContent("Blend mode"));    	
		EditorGUILayout.PropertyField (hdr, new GUIContent("HDR"));
		
		// display info text when screen blend mode cannot be used
		var cam : Camera = (target as BloomAndLensFlares).GetComponent.<Camera>();
		if(cam != null) {
			if(screenBlendMode.enumValueIndex==0 && ((cam.hdr && hdr.enumValueIndex==0) || (hdr.enumValueIndex==1))) {
				EditorGUILayout.HelpBox("Screen blend is not supported in HDR. Using 'Add' instead.", MessageType.Info);
			}
		}		
		
		if (1 == tweakMode.intValue) 
    		EditorGUILayout.PropertyField (lensflares, new GUIContent("Cast lens flares"));

    	EditorGUILayout.Separator ();

    	EditorGUILayout.PropertyField (bloomIntensity, new GUIContent("Intensity"));	
    	bloomThreshhold.floatValue = EditorGUILayout.Slider ("Threshhold", bloomThreshhold.floatValue, -0.05, 4.0);
    	bloomBlurIterations.intValue = EditorGUILayout.IntSlider ("Blur iterations", bloomBlurIterations.intValue, 1, 4);
		sepBlurSpread.floatValue = EditorGUILayout.Slider ("Blur spread", sepBlurSpread.floatValue, 0.1, 10.0);
    	
    	if (1 == tweakMode.intValue)
    		useSrcAlphaAsMask.floatValue = EditorGUILayout.Slider (new  GUIContent("Use alpha mask", "Make alpha channel define glowiness"), useSrcAlphaAsMask.floatValue, 0.0, 1.0);
    	else
    		useSrcAlphaAsMask.floatValue = 0.0;
    	    	
    	if (1 == tweakMode.intValue) {
    		EditorGUILayout.Separator ();
	    
	    	if (lensflares.boolValue) {
	    			    		
	    		// further lens flare tweakings
	    		if (0 != tweakMode.intValue)
	    			EditorGUILayout.PropertyField (lensflareMode, new GUIContent("Lens flare mode"));
	    		else
	    			lensflareMode.enumValueIndex = 0;	    		
	    		
	    		EditorGUILayout.PropertyField(lensFlareVignetteMask, new GUIContent("Lens flare mask", "This mask is needed to prevent lens flare artifacts"));	    		
	    		
	    		EditorGUILayout.PropertyField (lensflareIntensity, new GUIContent("Local intensity"));
	    		lensflareThreshhold.floatValue = EditorGUILayout.Slider ("Local threshhold", lensflareThreshhold.floatValue, 0.0, 1.0);
	    			    		
	    		if (lensflareMode.intValue == 0) {
	    			// ghosting	
	    			EditorGUILayout.BeginHorizontal ();
	    			EditorGUILayout.PropertyField (flareColorA, new GUIContent("1st Color"));
	    			EditorGUILayout.PropertyField (flareColorB, new GUIContent("2nd Color"));
	   				EditorGUILayout.EndHorizontal ();
	   				
	     			EditorGUILayout.BeginHorizontal ();
	    			EditorGUILayout.PropertyField (flareColorC, new GUIContent("3rd Color"));
	    			EditorGUILayout.PropertyField (flareColorD, new GUIContent("4th Color"));
	   				EditorGUILayout.EndHorizontal ();	
	    		} 
	    		else if (lensflareMode.intValue == 1) {
	    			// hollywood
	    			EditorGUILayout.PropertyField (hollyStretchWidth, new GUIContent("Stretch width"));
	    			hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider ("Blur iterations", hollywoodFlareBlurIterations.intValue, 1, 4);
	    			    			
	    			EditorGUILayout.PropertyField (flareColorA, new GUIContent("Tint Color"));	
	    		} 
	    		else if (lensflareMode.intValue == 2) {
	    			// both
	    			EditorGUILayout.PropertyField (hollyStretchWidth, new GUIContent("Stretch width"));
	    			hollywoodFlareBlurIterations.intValue = EditorGUILayout.IntSlider ("Blur iterations", hollywoodFlareBlurIterations.intValue, 1, 4);
	    			    			
	    			EditorGUILayout.BeginHorizontal ();
	    			EditorGUILayout.PropertyField (flareColorA, new GUIContent("1st Color"));
	    			EditorGUILayout.PropertyField (flareColorB, new GUIContent("2nd Color"));
	   				EditorGUILayout.EndHorizontal ();
	   				
	     			EditorGUILayout.BeginHorizontal ();
	    			EditorGUILayout.PropertyField (flareColorC, new GUIContent("3rd Color"));
	    			EditorGUILayout.PropertyField (flareColorD, new GUIContent("4th Color"));
	   				EditorGUILayout.EndHorizontal ();  			
				} 		
	    	}
    	} else
    		lensflares.boolValue = false; // disable lens flares in simple tweak mode
    	
    	serObj.ApplyModifiedProperties();
    }
}
