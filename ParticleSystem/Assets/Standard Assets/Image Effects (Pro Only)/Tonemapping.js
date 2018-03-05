
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Color Adjustments/Tonemapping")

class Tonemapping extends PostEffectsBase {
	
	public enum TonemapperType { 
		SimpleReinhard,
		UserCurve,
		Hable,
		Photographic,
		OptimizedHejiDawson,
		AdaptiveReinhard,	
		AdaptiveReinhardAutoWhite,	
	};
 
	public enum AdaptiveTexSize {
		Square16 = 16,
		Square32 = 32,
		Square64 = 64,
		Square128 = 128,
		Square256 = 256,
		Square512 = 512,
		Square1024 = 1024,
	};
	
	public var type : TonemapperType = TonemapperType.Photographic;
	public var adaptiveTextureSize = AdaptiveTexSize.Square256;
	
	// CURVE parameter
	public var remapCurve : AnimationCurve;
	private var curveTex : Texture2D = null;
	
	// UNCHARTED parameter
	public var exposureAdjustment : float = 1.5f;
	
	// REINHARD parameter
	public var middleGrey : float = 0.4f;
	public var white : float = 2.0f;
	public var adaptionSpeed : float = 1.5f;

    // usual & internal stuff
	public var tonemapper : Shader = null;
	public var validRenderTextureFormat : boolean = true;
	private var tonemapMaterial : Material = null;	
	private var rt : RenderTexture = null;
	private var rtFormat : RenderTextureFormat =  RenderTextureFormat.ARGBHalf;
	
	function CheckResources () : boolean {	
		CheckSupport (false, true);	
	
		tonemapMaterial = CheckShaderAndCreateMaterial(tonemapper, tonemapMaterial);
		if (!curveTex && type == TonemapperType.UserCurve) {
			curveTex = new Texture2D (256, 1, TextureFormat.ARGB32, false, true); 	
			curveTex.filterMode = FilterMode.Bilinear;
			curveTex.wrapMode = TextureWrapMode.Clamp;
			curveTex.hideFlags = HideFlags.DontSave;
		}
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		
	}

	public function UpdateCurve () : float {	
        var range : float = 1.0f;		
		if(remapCurve.keys.length  < 1)
			remapCurve =  new AnimationCurve(Keyframe(0, 0), Keyframe(2, 1));	
		if (remapCurve) {		
			if(remapCurve.length)
				range = remapCurve[remapCurve.length-1].time;			
			for (var i : float = 0.0f; i <= 1.0f; i += 1.0f / 255.0f) {
				var c : float = remapCurve.Evaluate(i * 1.0f * range);
				curveTex.SetPixel (Mathf.Floor(i*255.0f), 0, Color(c,c,c));
			}
			curveTex.Apply ();			
		}
		return 1.0f / range;
	}

	function OnDisable () {
		if (rt) {
			DestroyImmediate (rt);
			rt = null;
		}
		if (tonemapMaterial) {
			DestroyImmediate (tonemapMaterial);
			tonemapMaterial = null;
		}
		if (curveTex) {
			DestroyImmediate (curveTex);
			curveTex = null;
		}
	}
	
	function CreateInternalRenderTexture () : boolean {
		if (rt) {
			return false;
		}
		rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGHalf) ? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;
		rt = new RenderTexture(1,1, 0, rtFormat);
		rt.hideFlags = HideFlags.DontSave;		
		return true;
	}
		
	// a new attribute we introduced in 3.5 indicating that the image filter chain will continue in LDR
	@ImageEffectTransformsToLDR	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
		if (CheckResources() == false) {
			Graphics.Blit (source, destination);
			return;
		}		
		
		#if UNITY_EDITOR
		validRenderTextureFormat = true;
		if (source.format != RenderTextureFormat.ARGBHalf) {
			validRenderTextureFormat = false;
		}
		#endif
						
		// clamp some values to not go out of a valid range
		
		exposureAdjustment = exposureAdjustment < 0.001f ? 0.001f : exposureAdjustment;
		
		// SimpleReinhard tonemappers (local, non adaptive)
		
		if (type == TonemapperType.UserCurve) {
			var rangeScale : float = UpdateCurve ();
			tonemapMaterial.SetFloat("_RangeScale", rangeScale);	
			tonemapMaterial.SetTexture("_Curve", curveTex);		
			Graphics.Blit(source, destination, tonemapMaterial, 4);		
			return;	
		}
		
		if (type == TonemapperType.SimpleReinhard) {
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);	
			Graphics.Blit(source, destination, tonemapMaterial, 6);		
			return;	
		}
		
		if (type == TonemapperType.Hable) {
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
			Graphics.Blit(source, destination, tonemapMaterial, 5);
			return;	
		}
		
		if (type == TonemapperType.Photographic) {
			tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
			Graphics.Blit(source, destination, tonemapMaterial, 8);
			return;
		}

		if (type == TonemapperType.OptimizedHejiDawson) {
			tonemapMaterial.SetFloat("_ExposureAdjustment", 0.5f * exposureAdjustment);
			Graphics.Blit(source, destination, tonemapMaterial, 7);
			return;
		}
		
		// still here? 
		// =>  adaptive tone mapping:
		// builds an average log luminance, tonemaps according to 
		// middle grey and white values (user controlled)

		// AdaptiveReinhardAutoWhite will calculate white value automagically
		
		var freshlyBrewedInternalRt : boolean = CreateInternalRenderTexture (); // this retrieves rtFormat, so should happen before rt allocations
			
		var rtSquared : RenderTexture = RenderTexture.GetTemporary(adaptiveTextureSize, adaptiveTextureSize, 0, rtFormat);
		Graphics.Blit(source, rtSquared);
				
		var downsample : int = Mathf.Log(rtSquared.width * 1.0f, 2);
				
		var div : int = 2;
		var rts : RenderTexture[] = new RenderTexture[downsample];
		for (var i : int = 0; i < downsample; i++) {
			rts[i] = RenderTexture.GetTemporary(rtSquared.width / div, rtSquared.width / div, 0, rtFormat);
			div *= 2;
		}

		var ar : float = (source.width * 1.0f) / (source.height * 1.0f);

		// downsample pyramid
		
		var lumRt = rts[downsample-1];		
		Graphics.Blit(rtSquared, rts[0], tonemapMaterial, 1); 			
		if (type == TonemapperType.AdaptiveReinhardAutoWhite) {
			for(i = 0; i < downsample-1; i++) {
				Graphics.Blit(rts[i], rts[i+1], tonemapMaterial, 9); 
				lumRt = rts[i+1];
			}				
		}
		else if (type == TonemapperType.AdaptiveReinhard) {
			for(i = 0; i < downsample-1; i++) {
				Graphics.Blit(rts[i], rts[i+1]); 
				lumRt = rts[i+1];
			}		
		}
		
		// we have the needed values, let's apply adaptive tonemapping
	
		adaptionSpeed = adaptionSpeed < 0.001f ? 0.001f : adaptionSpeed;	
		tonemapMaterial.SetFloat ("_AdaptionSpeed", adaptionSpeed);

		#if UNITY_EDITOR
			if(Application.isPlaying && !freshlyBrewedInternalRt)
				Graphics.Blit (lumRt, rt, tonemapMaterial, 2); 
			else
				Graphics.Blit (lumRt, rt, tonemapMaterial, 3);
		#else
			Graphics.Blit (lumRt, rt, tonemapMaterial, freshlyBrewedInternalRt ? 3 : 2); 	
		#endif	

		middleGrey = middleGrey < 0.001f ? 0.001f : middleGrey;	
		tonemapMaterial.SetVector ("_HdrParams", Vector4 (middleGrey, middleGrey, middleGrey, white*white));
		tonemapMaterial.SetTexture ("_SmallTex", rt);
		if (type == TonemapperType.AdaptiveReinhard) {
			Graphics.Blit (source, destination, tonemapMaterial, 0); 
		}
		else if (type == TonemapperType.AdaptiveReinhardAutoWhite) {
			Graphics.Blit (source, destination, tonemapMaterial, 10); 		
		}
		else {
			Debug.LogError ("No valid adaptive tonemapper type found!");
			Graphics.Blit (source, destination); // at least we get the TransformToLDR effect
		}
			
		// cleanup for adaptive
			
		for(i = 0; i < downsample; i++) {
			RenderTexture.ReleaseTemporary (rts[i]);
		}
		RenderTexture.ReleaseTemporary (rtSquared);
	}
}