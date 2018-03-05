
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Other/Antialiasing")

enum AAMode {
	FXAA2 = 0,
	FXAA3Console = 1,		
	FXAA1PresetA = 2,
	FXAA1PresetB = 3,
	NFAA = 4,
	SSAA = 5,
	DLAA = 6,
}

class AntialiasingAsPostEffect extends PostEffectsBase  {
	public var mode : AAMode = AAMode.FXAA3Console;

	public var showGeneratedNormals : boolean = false;
	public var offsetScale : float = 0.2;
	public var blurRadius : float = 18.0;

	public var edgeThresholdMin : float = 0.05f;
	public var edgeThreshold : float = 0.2f;
	public var edgeSharpness : float  = 4.0f;
		
	public var dlaaSharp : boolean = false;

	public var ssaaShader : Shader;
	private var ssaa : Material;
	public var dlaaShader : Shader;
	private var dlaa : Material;
	public var nfaaShader : Shader;
	private var nfaa : Material;	
	public var shaderFXAAPreset2 : Shader;
	private var materialFXAAPreset2 : Material;
	public var shaderFXAAPreset3 : Shader;
	private var materialFXAAPreset3 : Material;
	public var shaderFXAAII : Shader;
	private var materialFXAAII : Material;
	public var shaderFXAAIII : Shader;
	private var materialFXAAIII : Material;
		
	function CurrentAAMaterial () : Material
	{
		var returnValue : Material = null;

		switch(mode) {
			case AAMode.FXAA3Console:
				returnValue = materialFXAAIII;
				break;
			case AAMode.FXAA2:
				returnValue = materialFXAAII;
				break;
			case AAMode.FXAA1PresetA:
				returnValue = materialFXAAPreset2;
				break;
			case AAMode.FXAA1PresetB:
				returnValue = materialFXAAPreset3;
				break;
			case AAMode.NFAA:
				returnValue = nfaa;
				break;
			case AAMode.SSAA:
				returnValue = ssaa;
				break;
			case AAMode.DLAA:
				returnValue = dlaa;
				break;	
			default:
				returnValue = null;
				break;
			}
			
		return returnValue;
	}

	function CheckResources () {
		CheckSupport (false);
		
		materialFXAAPreset2 = CreateMaterial (shaderFXAAPreset2, materialFXAAPreset2);
		materialFXAAPreset3 = CreateMaterial (shaderFXAAPreset3, materialFXAAPreset3);
		materialFXAAII = CreateMaterial (shaderFXAAII, materialFXAAII);
		materialFXAAIII = CreateMaterial (shaderFXAAIII, materialFXAAIII);
		nfaa = CreateMaterial (nfaaShader, nfaa);
		ssaa = CreateMaterial (ssaaShader, ssaa); 
		dlaa = CreateMaterial (dlaaShader, dlaa); 
                
        if(!ssaaShader.isSupported) {
            NotSupported ();
			ReportAutoDisable ();
		}
		
		return isSupported;		            
	}

	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}

 		// .............................................................................
		// FXAA antialiasing modes .....................................................
		
		if (mode == AAMode.FXAA3Console && (materialFXAAIII != null)) {
			materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
			materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
			materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);		
		
            Graphics.Blit (source, destination, materialFXAAIII);
        }        
		else if (mode == AAMode.FXAA1PresetB && (materialFXAAPreset3 != null)) {
            Graphics.Blit (source, destination, materialFXAAPreset3);
        }
        else if(mode == AAMode.FXAA1PresetA && materialFXAAPreset2 != null) {
            source.anisoLevel = 4;
            Graphics.Blit (source, destination, materialFXAAPreset2);
            source.anisoLevel = 0;
        }
        else if(mode == AAMode.FXAA2 && materialFXAAII != null) {
            Graphics.Blit (source, destination, materialFXAAII);
        }
		else if (mode == AAMode.SSAA && ssaa != null) {

		// .............................................................................
		// SSAA antialiasing ...........................................................
			
			Graphics.Blit (source, destination, ssaa);								
		}
		else if (mode == AAMode.DLAA && dlaa != null) {

		// .............................................................................
		// DLAA antialiasing ...........................................................
		
			source.anisoLevel = 0;	
			var interim : RenderTexture = RenderTexture.GetTemporary (source.width, source.height);
			Graphics.Blit (source, interim, dlaa, 0);			
			Graphics.Blit (interim, destination, dlaa, dlaaSharp ? 2 : 1);
			RenderTexture.ReleaseTemporary (interim);					
		}
		else if (mode == AAMode.NFAA && nfaa != null) {

		// .............................................................................
		// nfaa antialiasing ..............................................
			
			source.anisoLevel = 0;	
		
			nfaa.SetFloat("_OffsetScale", offsetScale);
			nfaa.SetFloat("_BlurRadius", blurRadius);
				
			Graphics.Blit (source, destination, nfaa, showGeneratedNormals ? 1 : 0);					
		}
		else {
			// none of the AA is supported, fallback to a simple blit
			Graphics.Blit (source, destination);
		}
	}
}
