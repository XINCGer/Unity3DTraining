
#pragma strict

@CustomEditor (DepthOfFieldScatter)
class DepthOfFieldScatterEditor extends Editor 
{	
	var serObj : SerializedObject;	
		
  var visualizeFocus : SerializedProperty;
  var focalLength : SerializedProperty;
  var focalSize : SerializedProperty; 
  var aperture : SerializedProperty;
  var focalTransform : SerializedProperty;
  var maxBlurSize : SerializedProperty;
  var highResolution : SerializedProperty;

  var blurType : SerializedProperty;
  var blurSampleCount : SerializedProperty;
  
  var nearBlur : SerializedProperty; 
  var foregroundOverlap : SerializedProperty;

  var dx11BokehThreshhold : SerializedProperty;
  var dx11SpawnHeuristic : SerializedProperty;
  var dx11BokehTexture : SerializedProperty;
  var dx11BokehScale : SerializedProperty;
  var dx11BokehIntensity : SerializedProperty;

	function OnEnable () {
		serObj = new SerializedObject (target);
		
    visualizeFocus = serObj.FindProperty ("visualizeFocus");

    focalLength = serObj.FindProperty ("focalLength");
    focalSize = serObj.FindProperty ("focalSize");
    aperture = serObj.FindProperty ("aperture");
    focalTransform = serObj.FindProperty ("focalTransform");
    maxBlurSize = serObj.FindProperty ("maxBlurSize");
    highResolution = serObj.FindProperty ("highResolution");
    
    blurType = serObj.FindProperty ("blurType");
    blurSampleCount = serObj.FindProperty ("blurSampleCount");

    nearBlur = serObj.FindProperty ("nearBlur");
    foregroundOverlap = serObj.FindProperty ("foregroundOverlap");    

    dx11BokehThreshhold = serObj.FindProperty ("dx11BokehThreshhold"); 
    dx11SpawnHeuristic = serObj.FindProperty ("dx11SpawnHeuristic"); 
    dx11BokehTexture = serObj.FindProperty ("dx11BokehTexture"); 
    dx11BokehScale = serObj.FindProperty ("dx11BokehScale"); 
    dx11BokehIntensity = serObj.FindProperty ("dx11BokehIntensity"); 
	} 
    		
    function OnInspectorGUI () {         
    	serObj.Update ();
    	    	    	
      EditorGUILayout.LabelField("Simulates camera lens defocus", EditorStyles.miniLabel);

    	GUILayout.Label ("Focal Settings");    	
		  EditorGUILayout.PropertyField (visualizeFocus, new GUIContent(" Visualize"));  		
   		EditorGUILayout.PropertyField (focalLength, new GUIContent(" Focal Distance"));
      EditorGUILayout.PropertyField (focalSize, new GUIContent(" Focal Size"));
      EditorGUILayout.PropertyField (focalTransform, new GUIContent(" Focus on Transform"));
   		EditorGUILayout.PropertyField (aperture, new GUIContent(" Aperture"));

      EditorGUILayout.Separator ();

      EditorGUILayout.PropertyField (blurType, new GUIContent("Defocus Type"));      

      if (!(target as DepthOfFieldScatter).Dx11Support() && blurType.enumValueIndex>0) {
        EditorGUILayout.HelpBox("DX11 mode not supported (need shader model 5)", MessageType.Info);      
      }      

      if(blurType.enumValueIndex<1)
        EditorGUILayout.PropertyField (blurSampleCount, new GUIContent(" Sample Count"));

   		EditorGUILayout.PropertyField (maxBlurSize, new GUIContent(" Max Blur Distance"));
      EditorGUILayout.PropertyField (highResolution, new GUIContent(" High Resolution"));
      
      EditorGUILayout.Separator ();

      EditorGUILayout.PropertyField (nearBlur, new GUIContent("Near Blur"));
      EditorGUILayout.PropertyField (foregroundOverlap, new GUIContent("  Overlap Size"));

      EditorGUILayout.Separator ();

      if(blurType.enumValueIndex>0) {
        GUILayout.Label ("DX11 Bokeh Settings"); 		  
        EditorGUILayout.PropertyField (dx11BokehTexture, new GUIContent(" Bokeh Texture"));
        EditorGUILayout.PropertyField (dx11BokehScale, new GUIContent(" Bokeh Scale"));
        EditorGUILayout.PropertyField (dx11BokehIntensity, new GUIContent(" Bokeh Intensity"));
        EditorGUILayout.PropertyField (dx11BokehThreshhold, new GUIContent(" Min Luminance"));
        EditorGUILayout.PropertyField (dx11SpawnHeuristic, new GUIContent(" Spawn Heuristic"));
      }
    	    	
    	serObj.ApplyModifiedProperties();
    }
}
