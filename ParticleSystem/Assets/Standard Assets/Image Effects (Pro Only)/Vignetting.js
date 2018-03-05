
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Camera/Vignette and Chromatic Aberration")

class Vignetting /* And Chromatic Aberration */ extends PostEffectsBase {

	public enum AberrationMode {
		Simple = 0,
		Advanced = 1,
	}

	public var mode : AberrationMode = AberrationMode.Simple;
	
	public var intensity : float = 0.375f; // intensity == 0 disables pre pass (optimization)
	public var chromaticAberration : float = 0.2f;
	public var axialAberration : float = 0.5f;

	public var blur : float = 0.0f; // blur == 0 disables blur pass (optimization)
	public var blurSpread : float = 0.75f;

	public var luminanceDependency : float = 0.25f;

	public var blurDistance : float = 2.5f;
		
	public var vignetteShader : Shader;
	private var vignetteMaterial : Material;
	
	public var separableBlurShader : Shader;
	private var separableBlurMaterial : Material;	
	
	public var chromAberrationShader : Shader;
	private var chromAberrationMaterial : Material;
	
	function CheckResources () : boolean {	
		CheckSupport (false);	
	
		vignetteMaterial = CheckShaderAndCreateMaterial (vignetteShader, vignetteMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial (separableBlurShader, separableBlurMaterial);
		chromAberrationMaterial = CheckShaderAndCreateMaterial (chromAberrationShader, chromAberrationMaterial);
		
		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;		
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if( CheckResources () == false) {
			Graphics.Blit (source, destination);
			return;
		}	
				
		var doPrepass : boolean = (Mathf.Abs(blur)>0.0f || Mathf.Abs(intensity)>0.0f);

		var widthOverHeight : float = (1.0f * source.width) / (1.0f * source.height);
		var oneOverBaseSize : float = 1.0f / 512.0f;				
		
		var color : RenderTexture = null;
		var halfRezColor : RenderTexture = null;
		var secondHalfRezColor : RenderTexture = null;

		if (doPrepass) {
			color = RenderTexture.GetTemporary (source.width, source.height, 0, source.format);	
		
			if (Mathf.Abs (blur)>0.0f) {
				halfRezColor = RenderTexture.GetTemporary (source.width / 2.0f, source.height / 2.0f, 0, source.format);		
				secondHalfRezColor = RenderTexture.GetTemporary (source.width / 2.0f, source.height / 2.0f, 0, source.format);	

				Graphics.Blit (source, halfRezColor, chromAberrationMaterial, 0);

				for(var i : int = 0; i < 2; i++) {	// maybe make iteration count tweakable
					separableBlurMaterial.SetVector ("offsets", Vector4 (0.0f, blurSpread * oneOverBaseSize, 0.0f, 0.0f));	
					Graphics.Blit (halfRezColor, secondHalfRezColor, separableBlurMaterial); 
					separableBlurMaterial.SetVector ("offsets", Vector4 (blurSpread * oneOverBaseSize / widthOverHeight, 0.0f, 0.0f, 0.0f));	
					Graphics.Blit (secondHalfRezColor, halfRezColor, separableBlurMaterial);	
				}	
			}

			vignetteMaterial.SetFloat ("_Intensity", intensity); 		// intensity for vignette
			vignetteMaterial.SetFloat ("_Blur", blur); 					// blur intensity
			vignetteMaterial.SetTexture ("_VignetteTex", halfRezColor);	// blurred texture

			Graphics.Blit (source, color, vignetteMaterial, 0); 		// prepass blit: darken & blur corners
		}		

		chromAberrationMaterial.SetFloat ("_ChromaticAberration", chromaticAberration);
		chromAberrationMaterial.SetFloat ("_AxialAberration", axialAberration);
		chromAberrationMaterial.SetVector ("_BlurDistance", Vector2 (-blurDistance, blurDistance));
		chromAberrationMaterial.SetFloat ("_Luminance", 1.0f/Mathf.Max(Mathf.Epsilon, luminanceDependency));

		if(doPrepass) color.wrapMode = TextureWrapMode.Clamp;
		else source.wrapMode = TextureWrapMode.Clamp;		
		Graphics.Blit (doPrepass ? color : source, destination, chromAberrationMaterial, mode == AberrationMode.Advanced ? 2 : 1);	
		
		if (color) RenderTexture.ReleaseTemporary (color);
		if (halfRezColor) RenderTexture.ReleaseTemporary (halfRezColor);			
		if (secondHalfRezColor) RenderTexture.ReleaseTemporary (secondHalfRezColor);		
	}

}