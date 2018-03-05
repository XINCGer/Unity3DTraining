
#pragma strict

@CustomEditor (AntialiasingAsPostEffect)

class AntialiasingAsPostEffectEditor extends Editor 
{	
	var serObj : SerializedObject;	
		
	var mode : SerializedProperty;
	
	var showGeneratedNormals : SerializedProperty;
	var offsetScale : SerializedProperty;
	var blurRadius : SerializedProperty;
	var dlaaSharp : SerializedProperty;
	
	var edgeThresholdMin : SerializedProperty;
	var edgeThreshold : SerializedProperty;
	var edgeSharpness : SerializedProperty;	

	function OnEnable () {
		serObj = new SerializedObject (target);
		
		mode = serObj.FindProperty ("mode");
		
		showGeneratedNormals = serObj.FindProperty ("showGeneratedNormals");
		offsetScale = serObj.FindProperty ("offsetScale");
		blurRadius = serObj.FindProperty ("blurRadius");
		dlaaSharp = serObj.FindProperty ("dlaaSharp");
		
		edgeThresholdMin = serObj.FindProperty("edgeThresholdMin");
		edgeThreshold = serObj.FindProperty("edgeThreshold");
		edgeSharpness = serObj.FindProperty("edgeSharpness");
	}
    		
    function OnInspectorGUI () {        
    	serObj.Update ();
    	
		GUILayout.Label("Luminance based fullscreen antialiasing", EditorStyles.miniBoldLabel);
    	
    	EditorGUILayout.PropertyField (mode, new GUIContent ("Technique"));
    	
    	var mat : Material = (target as AntialiasingAsPostEffect).CurrentAAMaterial ();
    	if(null == mat && (target as AntialiasingAsPostEffect).enabled) {
			EditorGUILayout.HelpBox("This AA technique is currently not supported. Choose a different technique or disable the effect and use MSAA instead.", MessageType.Warning);    		
    	}

		if (mode.enumValueIndex == AAMode.NFAA) {
    		EditorGUILayout.PropertyField (offsetScale, new GUIContent ("Edge Detect Ofs"));
    		EditorGUILayout.PropertyField (blurRadius, new GUIContent ("Blur Radius"));
    		EditorGUILayout.PropertyField (showGeneratedNormals, new GUIContent ("Show Normals"));	
		} else if (mode.enumValueIndex == AAMode.DLAA) {
    		EditorGUILayout.PropertyField (dlaaSharp, new GUIContent ("Sharp"));			
		} else if (mode.enumValueIndex == AAMode.FXAA3Console) {
    		EditorGUILayout.PropertyField (edgeThresholdMin, new GUIContent ("Edge Min Threshhold"));
    		EditorGUILayout.PropertyField (edgeThreshold, new GUIContent ("Edge Threshhold"));
    		EditorGUILayout.PropertyField (edgeSharpness, new GUIContent ("Edge Sharpness"));
		}
    	
    	serObj.ApplyModifiedProperties();
    }
}
