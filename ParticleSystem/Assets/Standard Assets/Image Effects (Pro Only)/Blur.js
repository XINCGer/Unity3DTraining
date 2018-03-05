
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Blur/Blur (Optimized)")

class Blur extends PostEffectsBase {

	@Range(0, 2)
	public var downsample : int = 1;

	public enum BlurType {
		StandardGauss = 0,
		SgxGauss = 1,
	}

	@Range(0.0f, 10.0f)
	public var blurSize : float = 3.0f;
	
	@Range(1, 4)
	public var blurIterations : int = 2;

	public var blurType = BlurType.StandardGauss;

	public var blurShader : Shader;
	private var blurMaterial : Material = null;
	
	function CheckResources () : boolean {	
		CheckSupport (false);	
	
		blurMaterial = CheckShaderAndCreateMaterial (blurShader, blurMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}

	function OnDisable() {
		if(blurMaterial)
			DestroyImmediate (blurMaterial);
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources() == false) {
			Graphics.Blit (source, destination);
			return;
		}

		var widthMod : float = 1.0f / (1.0f * (1<<downsample));

		blurMaterial.SetVector ("_Parameter", Vector4 (blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
		source.filterMode = FilterMode.Bilinear;

		var rt : RenderTexture = RenderTexture.GetTemporary (source.width >> downsample, source.height >> downsample, 0, source.format);
		var rt2 : RenderTexture = RenderTexture.GetTemporary (source.width >> downsample, source.height >> downsample, 0, source.format);

		rt.filterMode = FilterMode.Bilinear;
		rt2.filterMode = FilterMode.Bilinear;

		Graphics.Blit (source, rt, blurMaterial, 0);

		var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;
		
		for(var i : int = 0; i < blurIterations; i++) {
			var iterationOffs : float = (i*1.0f);
			blurMaterial.SetVector ("_Parameter", Vector4 (blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

			Graphics.Blit (rt, rt2, blurMaterial, 1 + passOffs);
			Graphics.Blit (rt2, rt, blurMaterial, 2 + passOffs);
		}
		
		Graphics.Blit (rt, destination);

		RenderTexture.ReleaseTemporary (rt);
		RenderTexture.ReleaseTemporary (rt2);
	}	
}
