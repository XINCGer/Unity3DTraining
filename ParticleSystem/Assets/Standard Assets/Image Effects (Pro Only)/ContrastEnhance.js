
#pragma strict

@script ExecuteInEditMode
@script RequireComponent(Camera)
@script AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)")

class ContrastEnhance extends PostEffectsBase {
	public var intensity : float = 0.5;
	public var threshhold : float = 0.0;
	
	private var separableBlurMaterial : Material;
	private var contrastCompositeMaterial : Material;
	
	public var blurSpread : float = 1.0;
	
	public var separableBlurShader : Shader = null;
	public var contrastCompositeShader : Shader = null;

	function CheckResources () : boolean {	
		CheckSupport (false);
		
		contrastCompositeMaterial = CheckShaderAndCreateMaterial (contrastCompositeShader, contrastCompositeMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial (separableBlurShader, separableBlurMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
				
		var halfRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 2.0, source.height / 2.0, 0);	
		var quarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4.0, source.height / 4.0, 0);	
		var secondQuarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4.0, source.height / 4.0, 0);	
			
		// ddownsample
		
		Graphics.Blit (source, halfRezColor);
		Graphics.Blit (halfRezColor, quarterRezColor); 
	
		// blur
		
		separableBlurMaterial.SetVector ("offsets", Vector4 (0.0, (blurSpread * 1.0) / quarterRezColor.height, 0.0, 0.0));	
		Graphics.Blit (quarterRezColor, secondQuarterRezColor, separableBlurMaterial);				
		separableBlurMaterial.SetVector ("offsets", Vector4 ((blurSpread * 1.0) / quarterRezColor.width, 0.0, 0.0, 0.0));	
		Graphics.Blit (secondQuarterRezColor, quarterRezColor, separableBlurMaterial); 
	
		// composite
		
		contrastCompositeMaterial.SetTexture ("_MainTexBlurred", quarterRezColor);
		contrastCompositeMaterial.SetFloat ("intensity", intensity);
		contrastCompositeMaterial.SetFloat ("threshhold", threshhold);
		Graphics.Blit (source, destination, contrastCompositeMaterial); 
		
		RenderTexture.ReleaseTemporary (halfRezColor);	
		RenderTexture.ReleaseTemporary (quarterRezColor);	
		RenderTexture.ReleaseTemporary (secondQuarterRezColor);			
	}
}