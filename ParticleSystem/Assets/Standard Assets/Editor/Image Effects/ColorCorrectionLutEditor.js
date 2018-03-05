

@CustomEditor (ColorCorrectionLut)

class ColorCorrectionLutEditor extends Editor 
{	
	var serObj : SerializedObject;	

	function OnEnable () {
		serObj = new SerializedObject (target);
	}

  private var tempClutTex2D : Texture2D;
    		
  function OnInspectorGUI ()
  {         
  	serObj.Update ();
  	
    EditorGUILayout.LabelField("Converts textures into color lookup volumes (for grading)", EditorStyles.miniLabel);
    
    //EditorGUILayout.LabelField("Change Lookup Texture (LUT):");
    //EditorGUILayout.BeginHorizontal ();
    //var r : Rect = GUILayoutUtility.GetAspectRect(1.0f);

    var r : Rect; var t : Texture2D;

    //EditorGUILayout.Space();
    tempClutTex2D = EditorGUILayout.ObjectField (" Based on", tempClutTex2D, Texture2D, false) as Texture2D;
    if (tempClutTex2D == null) {
      t = AssetDatabase.LoadMainAssetAtPath((target as ColorCorrectionLut).basedOnTempTex) as Texture2D;
      if (t) tempClutTex2D = t;
    }

    var tex : Texture2D = tempClutTex2D;

    if (tex && (target as ColorCorrectionLut).basedOnTempTex != AssetDatabase.GetAssetPath (tex)) 
    {
      EditorGUILayout.Separator();
      if (!(target as ColorCorrectionLut).ValidDimensions (tex))
      {
        EditorGUILayout.HelpBox ("Invalid texture dimensions!\nPick another texture or adjust dimension to e.g. 256x16.", MessageType.Warning);
      }
      else if (GUILayout.Button ("Convert and Apply"))
      {
        var path : String = AssetDatabase.GetAssetPath (tex);
        var textureImporter : TextureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        var doImport : boolean = false;
        if (textureImporter.isReadable == false) {
            doImport = true;
        }
        if (textureImporter.mipmapEnabled == true) {
            doImport = true;
        }
        if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor) {
            doImport = true;          
        }

        if (doImport) 
        {
          textureImporter.isReadable = true;
          textureImporter.mipmapEnabled = false;
          textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
          AssetDatabase.ImportAsset (path, ImportAssetOptions.ForceUpdate);
          //tex = AssetDatabase.LoadMainAssetAtPath(path);  
        }

        (target as ColorCorrectionLut).Convert (tex, path);
      }
    }
    
    if ((target as ColorCorrectionLut).basedOnTempTex != "") {
      EditorGUILayout.HelpBox ("Using " + (target as ColorCorrectionLut).basedOnTempTex, MessageType.Info);
      t = AssetDatabase.LoadMainAssetAtPath((target as ColorCorrectionLut).basedOnTempTex) as Texture2D;
      if (t) {
        r = GUILayoutUtility.GetLastRect();
        r = GUILayoutUtility.GetRect(r.width, 20);
        r.x += r.width * 0.05f/2.0f;
        r.width *= 0.95f;
        GUI.DrawTexture (r, t);
        GUILayoutUtility.GetRect(r.width, 4);
      }
    }

    //EditorGUILayout.EndHorizontal ();    
  	    	
  	serObj.ApplyModifiedProperties();
  }
}
