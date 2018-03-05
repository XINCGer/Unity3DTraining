

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Rendering/Sun Shafts")

enum SunShaftsResolution {
    Low = 0,
    Normal = 1,
	High = 2,
}
	
enum ShaftsScreenBlendMode {
	Screen = 0,
	Add = 1,	
}	
		
class SunShafts extends PostEffectsBase 
{		
	public var resolution : SunShaftsResolution = SunShaftsResolution.Normal;
	public var screenBlendMode : ShaftsScreenBlendMode = ShaftsScreenBlendMode.Screen;
	
	public var sunTransform : Transform;
	public var radialBlurIterations : int = 2;
	public var sunColor : Color = Color.white;
	public var sunShaftBlurRadius : float = 2.5f;
	public var sunShaftIntensity : float = 1.15;
	public var useSkyBoxAlpha : float = 0.75f;
	
	public var maxRadius : float = 0.75f;
	
	public var useDepthTexture : boolean = true;
	
	public var sunShaftsShader : Shader;
	private var sunShaftsMaterial : Material;	
	
	public var simpleClearShader : Shader;
	private var simpleClearMaterial : Material;
		
	function CheckResources () : boolean {	
		CheckSupport (useDepthTexture);
		
		sunShaftsMaterial = CheckShaderAndCreateMaterial (sunShaftsShader, sunShaftsMaterial);
		simpleClearMaterial = CheckShaderAndCreateMaterial (simpleClearShader, simpleClearMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
				
		// we actually need to check this every frame
		if(useDepthTexture)
			GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;	
		
        var divider : float = 4.0;
        if (resolution == SunShaftsResolution.Normal)
            divider = 2.0;
        else if (resolution == SunShaftsResolution.High)
            divider = 1.0;
            
		var v : Vector3 = Vector3.one * 0.5;
		if (sunTransform)
			v = GetComponent.<Camera>().WorldToViewportPoint (sunTransform.position);
		else 
			v = Vector3(0.5, 0.5, 0.0);        
			
		var secondQuarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);	
        var lrDepthBuffer : RenderTexture = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);
		
		// mask out everything except the skybox
		// we have 2 methods, one of which requires depth buffer support, the other one is just comparing images
		
		sunShaftsMaterial.SetVector ("_BlurRadius4", Vector4 (1.0, 1.0, 0.0, 0.0) * sunShaftBlurRadius );
		sunShaftsMaterial.SetVector ("_SunPosition", Vector4 (v.x, v.y, v.z, maxRadius));		
		sunShaftsMaterial.SetFloat ("_NoSkyBoxMask", 1.0f - useSkyBoxAlpha);	
		
		if (!useDepthTexture) {		
			var tmpBuffer : RenderTexture = RenderTexture.GetTemporary (source.width, source.height, 0);					
			RenderTexture.active = tmpBuffer;
			GL.ClearWithSkybox (false, GetComponent.<Camera>());
			
			sunShaftsMaterial.SetTexture ("_Skybox", tmpBuffer);
			Graphics.Blit (source, lrDepthBuffer, sunShaftsMaterial, 3);		
			RenderTexture.ReleaseTemporary (tmpBuffer);				
		}
		else {		
			Graphics.Blit (source, lrDepthBuffer, sunShaftsMaterial, 2);
		}
				
        // paint a small black small border to get rid of clamping problems
		DrawBorder (lrDepthBuffer, simpleClearMaterial);
		        			
		// radial blur:
						
		radialBlurIterations = ClampBlurIterationsToSomethingThatMakesSense (radialBlurIterations);	
				
		var ofs : float = sunShaftBlurRadius * (1.0f / 768.0f);
		
		sunShaftsMaterial.SetVector ("_BlurRadius4", Vector4 (ofs, ofs, 0.0f, 0.0f));			
		sunShaftsMaterial.SetVector ("_SunPosition", Vector4 (v.x, v.y, v.z, maxRadius));				
				
		for (var it2 : int = 0; it2 < radialBlurIterations; it2++ ) {
			// each iteration takes 2 * 6 samples
			// we update _BlurRadius each time to cheaply get a very smooth look
						
			Graphics.Blit (lrDepthBuffer, secondQuarterRezColor, sunShaftsMaterial, 1);
			ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
			sunShaftsMaterial.SetVector ("_BlurRadius4", Vector4 (ofs, ofs, 0.0f, 0.0f) );			
			
			Graphics.Blit (secondQuarterRezColor, lrDepthBuffer, sunShaftsMaterial, 1);		
			ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;			
			sunShaftsMaterial.SetVector ("_BlurRadius4", Vector4 (ofs, ofs, 0.0f, 0.0f) );
		}		
		
		// put together:
		
		if (v.z >= 0.0)
			sunShaftsMaterial.SetVector ("_SunColor", Vector4 (sunColor.r, sunColor.g, sunColor.b, sunColor.a) * sunShaftIntensity);
		else
			sunShaftsMaterial.SetVector ("_SunColor", Vector4.zero); // no backprojection !
		sunShaftsMaterial.SetTexture ("_ColorBuffer", lrDepthBuffer);
		Graphics.Blit (source, destination, sunShaftsMaterial, (screenBlendMode == ShaftsScreenBlendMode.Screen) ? 0 : 4); 	
		
		RenderTexture.ReleaseTemporary (lrDepthBuffer);	
		RenderTexture.ReleaseTemporary (secondQuarterRezColor);	
	}
		
	// helper functions

	private function ClampBlurIterationsToSomethingThatMakesSense (its : int) : int {
		if (its < 1)
			return 1;
		else if (its > 4)
			return 4;		
		else
			return its;	
	}

}