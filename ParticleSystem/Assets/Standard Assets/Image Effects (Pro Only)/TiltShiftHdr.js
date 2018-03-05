
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Camera/Tilt Shift (Lens Blur)")

class TiltShiftHdr extends PostEffectsBase {
	public enum TiltShiftMode 
	{
		TiltShiftMode,
		IrisMode,
	}
	public enum TiltShiftQuality
	{
		Preview,
		Normal,
		High,
	}	

	public var mode : TiltShiftMode = TiltShiftMode.TiltShiftMode;
	public var quality : TiltShiftQuality = TiltShiftQuality.Normal;

	@Range(0.0f, 15.0f)
	public var blurArea : float = 1.0f;

	@Range(0.0f, 25.0f)
	public var maxBlurSize : float = 5.0f;
	
	@Range(0, 1)
	public var downsample : int = 0;

	public var tiltShiftShader : Shader;
	private var tiltShiftMaterial : Material = null;
	
		
	function CheckResources () : boolean {	
		CheckSupport (true);	
	
		tiltShiftMaterial = CheckShaderAndCreateMaterial (tiltShiftShader, tiltShiftMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources() == false) {
			Graphics.Blit (source, destination);
			return;
		}

		tiltShiftMaterial.SetFloat("_BlurSize", maxBlurSize < 0.0f ? 0.0f : maxBlurSize);
		tiltShiftMaterial.SetFloat("_BlurArea", blurArea);
		source.filterMode = FilterMode.Bilinear;
		
		var rt : RenderTexture = destination;
		if (downsample) {
			rt = RenderTexture.GetTemporary (source.width>>downsample, source.height>>downsample, 0, source.format);
			rt.filterMode = FilterMode.Bilinear;		
		}		

		var basePassNr : int = quality; basePassNr *= 2;
		Graphics.Blit (source, rt, tiltShiftMaterial, mode == TiltShiftMode.TiltShiftMode ? basePassNr : basePassNr + 1);

		if (downsample) {
			tiltShiftMaterial.SetTexture ("_Blurred", rt);
			Graphics.Blit (source, destination, tiltShiftMaterial, 6);
		}

		if (rt != destination)
			RenderTexture.ReleaseTemporary (rt);
	}	
}
