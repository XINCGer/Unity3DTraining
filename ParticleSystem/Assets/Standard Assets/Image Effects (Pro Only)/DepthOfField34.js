
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Camera/Depth of Field (3.4)") 

enum Dof34QualitySetting {
	OnlyBackground = 1,
	BackgroundAndForeground = 2,
}

enum DofResolution {
	High = 2,
	Medium = 3,
	Low = 4,	
}

enum DofBlurriness {
	Low = 1,	
	High = 2,
	VeryHigh = 4,
}

enum BokehDestination {
	Background = 0x1,	
	Foreground = 0x2,
	BackgroundAndForeground = 0x3,
}

class DepthOfField34 extends PostEffectsBase {

	static private var SMOOTH_DOWNSAMPLE_PASS : int = 6;
	static private var BOKEH_EXTRA_BLUR : float = 2.0f;
	
	public var quality : Dof34QualitySetting = Dof34QualitySetting.OnlyBackground;
	public var resolution : DofResolution  = DofResolution.Low;
	public var simpleTweakMode : boolean = true;
	
	public var focalPoint : float = 1.0f;
	public var smoothness : float = 0.5f;
	
	public var focalZDistance : float = 0.0f;
	public var focalZStartCurve : float = 1.0f;
	public var focalZEndCurve : float = 1.0f;
	
	private var focalStartCurve : float = 2.0f;
	private var focalEndCurve : float = 2.0f;
	private var focalDistance01 : float = 0.1f;
		
	public var objectFocus : Transform = null;
	public var focalSize : float = 0.0f;
	
	public var bluriness : DofBlurriness = DofBlurriness.High;
	public var maxBlurSpread : float = 1.75f;
	
	public var foregroundBlurExtrude : float = 1.15f;
			
	public var dofBlurShader : Shader;
	private var dofBlurMaterial : Material = null;	
	
	public var dofShader : Shader;
	private var dofMaterial : Material = null;
    
    public var visualize : boolean = false;
    public var bokehDestination : BokehDestination = BokehDestination.Background;
    
    private var widthOverHeight : float = 1.25f;
    private var oneOverBaseSize : float = 1.0f / 512.0f;	
        
    public var bokeh : boolean = false;
    public var bokehSupport : boolean = true;
    public var bokehShader : Shader;
    public var bokehTexture : Texture2D;
    public var bokehScale : float = 2.4f;
    public var bokehIntensity : float = 0.15f;
    public var bokehThreshholdContrast : float = 0.1f;
    public var bokehThreshholdLuminance : float = 0.55f;
    public var bokehDownsample : int = 1;
    private var bokehMaterial : Material;
	
	function CreateMaterials () {		
		dofBlurMaterial = CheckShaderAndCreateMaterial (dofBlurShader, dofBlurMaterial);
		dofMaterial = CheckShaderAndCreateMaterial (dofShader,dofMaterial);  
		bokehSupport = bokehShader.isSupported;     

		if(bokeh && bokehSupport && bokehShader) 
			bokehMaterial = CheckShaderAndCreateMaterial (bokehShader, bokehMaterial);
	}
	
	function CheckResources () : boolean {		
		CheckSupport (true);
	
		dofBlurMaterial = CheckShaderAndCreateMaterial (dofBlurShader, dofBlurMaterial);
		dofMaterial = CheckShaderAndCreateMaterial (dofShader,dofMaterial);  
		bokehSupport = bokehShader.isSupported;  
				
		if(bokeh && bokehSupport && bokehShader) 
			bokehMaterial = CheckShaderAndCreateMaterial (bokehShader, bokehMaterial);
					
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		  
	}

	function OnDisable () {
		Quads.Cleanup ();	
	}

	function OnEnable() {
		GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;		
	}
	
	function FocalDistance01 (worldDist : float) : float {
		return GetComponent.<Camera>().WorldToViewportPoint((worldDist-GetComponent.<Camera>().nearClipPlane) * GetComponent.<Camera>().transform.forward + GetComponent.<Camera>().transform.position).z / (GetComponent.<Camera>().farClipPlane-GetComponent.<Camera>().nearClipPlane);	
	}
	
	function GetDividerBasedOnQuality () {
		var divider : int = 1;
		if (resolution == DofResolution.Medium)
			divider = 2;
		else if (resolution == DofResolution.Low)
			divider = 2;	
		return divider;	
	}
	
	function GetLowResolutionDividerBasedOnQuality (baseDivider : int) {
        var lowTexDivider : int = baseDivider;
        if (resolution == DofResolution.High)	
        	lowTexDivider *= 2;   
        if (resolution == DofResolution.Low)	
        	lowTexDivider *= 2; 	
        return lowTexDivider;	
	}
	
	private var foregroundTexture : RenderTexture = null; 
	private var mediumRezWorkTexture : RenderTexture = null;
	private var finalDefocus : RenderTexture = null;
	private var lowRezWorkTexture : RenderTexture = null;
	private var bokehSource : RenderTexture = null;
	private var bokehSource2 : RenderTexture = null;
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}	
		
		if (smoothness < 0.1f)
			smoothness = 0.1f;
		
		// update needed focal & rt size parameter
		
		bokeh = bokeh && bokehSupport;
		var bokehBlurAmplifier : float = bokeh ? BOKEH_EXTRA_BLUR : 1.0f;

		var blurForeground : boolean = quality > Dof34QualitySetting.OnlyBackground;	
		var focal01Size : float = focalSize / (GetComponent.<Camera>().farClipPlane - GetComponent.<Camera>().nearClipPlane);;

		if (simpleTweakMode) {		
			focalDistance01 = objectFocus ? (GetComponent.<Camera>().WorldToViewportPoint (objectFocus.position)).z / (GetComponent.<Camera>().farClipPlane) : FocalDistance01 (focalPoint);
			focalStartCurve = focalDistance01 * smoothness;
			focalEndCurve = focalStartCurve;
			blurForeground = blurForeground && (focalPoint > (GetComponent.<Camera>().nearClipPlane + Mathf.Epsilon));
		} 
		else {
			if(objectFocus) {
				var vpPoint = GetComponent.<Camera>().WorldToViewportPoint (objectFocus.position);
				vpPoint.z = (vpPoint.z) / (GetComponent.<Camera>().farClipPlane);
				focalDistance01 = vpPoint.z;			
			} 
			else 
				focalDistance01 = FocalDistance01 (focalZDistance);	
			
			focalStartCurve = focalZStartCurve;
			focalEndCurve = focalZEndCurve;
			blurForeground = blurForeground && (focalPoint > (GetComponent.<Camera>().nearClipPlane + Mathf.Epsilon));				
		}
		
		widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
		oneOverBaseSize = 1.0f / 512.0f;		
        		
		dofMaterial.SetFloat ("_ForegroundBlurExtrude", foregroundBlurExtrude);
		dofMaterial.SetVector ("_CurveParams", Vector4 (simpleTweakMode ? 1.0f / focalStartCurve : focalStartCurve, simpleTweakMode ? 1.0f / focalEndCurve : focalEndCurve, focal01Size * 0.5, focalDistance01));
		dofMaterial.SetVector ("_InvRenderTargetSize", Vector4 (1.0 / (1.0 * source.width), 1.0 / (1.0 * source.height),0.0,0.0));
				
		var divider : int =  GetDividerBasedOnQuality ();
        var lowTexDivider : int = GetLowResolutionDividerBasedOnQuality (divider);
		
        AllocateTextures (blurForeground, source, divider, lowTexDivider);

		// WRITE COC to alpha channel		
		// source is only being bound to detect y texcoord flip
		Graphics.Blit (source, source, dofMaterial, 3); 
				
	    // better DOWNSAMPLE (could actually be weighted for higher quality)
		Downsample (source, mediumRezWorkTexture);	
		
       	// BLUR A LITTLE first, which has two purposes
       	// 1.) reduce jitter, noise, aliasing
       	// 2.) produce the little-blur buffer used in composition later     	     
		Blur (mediumRezWorkTexture, mediumRezWorkTexture, DofBlurriness.Low, 4, maxBlurSpread);			

		if (bokeh && (bokehDestination & BokehDestination.Background)) { 		
           	dofMaterial.SetVector ("_Threshhold", Vector4(bokehThreshholdContrast, bokehThreshholdLuminance, 0.95f, 0.0f));
				
			// add and mark the parts that should end up as bokeh shapes
			Graphics.Blit (mediumRezWorkTexture, bokehSource2, dofMaterial, 11);	
						
			// remove those parts (maybe even a little tittle bittle more) from the regurlarly blurred buffer		
			//Graphics.Blit (mediumRezWorkTexture, lowRezWorkTexture, dofMaterial, 10);
			Graphics.Blit (mediumRezWorkTexture, lowRezWorkTexture);//, dofMaterial, 10);
			
			// maybe you want to reblur the small blur ... but not really needed.
			//Blur (mediumRezWorkTexture, mediumRezWorkTexture, DofBlurriness.Low, 4, maxBlurSpread);						
			
			// bigger BLUR
			Blur (lowRezWorkTexture, lowRezWorkTexture, bluriness, 0, maxBlurSpread * bokehBlurAmplifier);				
		} 
		else  {
			// bigger BLUR
			Downsample (mediumRezWorkTexture, lowRezWorkTexture);			
			Blur (lowRezWorkTexture, lowRezWorkTexture, bluriness, 0, maxBlurSpread);	
		}	
       		
		dofBlurMaterial.SetTexture ("_TapLow", lowRezWorkTexture);
		dofBlurMaterial.SetTexture ("_TapMedium", mediumRezWorkTexture);							
		Graphics.Blit (null, finalDefocus, dofBlurMaterial, 3);	
		
		// we are only adding bokeh now if the background is the only part we have to deal with
		if (bokeh && (bokehDestination & BokehDestination.Background))
			AddBokeh (bokehSource2, bokehSource, finalDefocus);
		
		dofMaterial.SetTexture ("_TapLowBackground", finalDefocus); 
		dofMaterial.SetTexture ("_TapMedium", mediumRezWorkTexture); // needed for debugging/visualization
						
		// FINAL DEFOCUS (background)
		Graphics.Blit (source, blurForeground ? foregroundTexture : destination, dofMaterial, visualize ? 2 : 0); 
		
		// FINAL DEFOCUS (foreground)
		if (blurForeground) {			
			// WRITE COC to alpha channel			
			Graphics.Blit (foregroundTexture, source, dofMaterial, 5); 
			
	    	// DOWNSAMPLE (unweighted)
			Downsample (source, mediumRezWorkTexture);	
			
       		// BLUR A LITTLE first, which has two purposes
       		// 1.) reduce jitter, noise, aliasing
       		// 2.) produce the little-blur buffer used in composition later   
       		BlurFg (mediumRezWorkTexture, mediumRezWorkTexture, DofBlurriness.Low, 2, maxBlurSpread);	
       		
	 		if (bokeh && (bokehDestination & BokehDestination.Foreground)) { 
	           	dofMaterial.SetVector ("_Threshhold", Vector4(bokehThreshholdContrast * 0.5f, bokehThreshholdLuminance, 0.0f, 0.0f));
	 			
				// add and mark the parts that should end up as bokeh shapes
				Graphics.Blit (mediumRezWorkTexture, bokehSource2, dofMaterial, 11);	
				
				// remove the parts (maybe even a little tittle bittle more) that will end up in bokeh space			
				//Graphics.Blit (mediumRezWorkTexture, lowRezWorkTexture, dofMaterial, 10);
				Graphics.Blit (mediumRezWorkTexture, lowRezWorkTexture);//, dofMaterial, 10);
				
				// big BLUR		
				BlurFg (lowRezWorkTexture, lowRezWorkTexture, bluriness, 1, maxBlurSpread * bokehBlurAmplifier);				
			} 
			else  {      		
       			// big BLUR		
				BlurFg (mediumRezWorkTexture, lowRezWorkTexture, bluriness, 1, maxBlurSpread);	
			}
					
			// simple upsample once						
			Graphics.Blit (lowRezWorkTexture, finalDefocus);	

			dofMaterial.SetTexture ("_TapLowForeground", finalDefocus);	
			Graphics.Blit (source, destination, dofMaterial, visualize ? 1 : 4);

			if (bokeh && (bokehDestination & BokehDestination.Foreground))
				AddBokeh (bokehSource2, bokehSource, destination);							
		}					
		
		ReleaseTextures ();
	}
	
	function Blur (from : RenderTexture, to : RenderTexture, iterations : DofBlurriness, blurPass: int, spread : float) {
		var tmp : RenderTexture = RenderTexture.GetTemporary (to.width, to.height);	
		if (iterations > 1) {
			BlurHex (from, to, blurPass, spread, tmp);	
			if (iterations > 2) {
				dofBlurMaterial.SetVector ("offsets", Vector4 (0.0, spread * oneOverBaseSize, 0.0, 0.0));
				Graphics.Blit (to, tmp, dofBlurMaterial, blurPass);
				dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0, 0.0, 0.0));		
				Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 	
			}		
		}
		else {
			dofBlurMaterial.SetVector ("offsets", Vector4 (0.0, spread * oneOverBaseSize, 0.0, 0.0));
			Graphics.Blit (from, tmp, dofBlurMaterial, blurPass);
			dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0, 0.0, 0.0));		
			Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 
		}	
		RenderTexture.ReleaseTemporary (tmp);
	}

	function BlurFg (from : RenderTexture, to : RenderTexture, iterations : DofBlurriness, blurPass: int, spread : float) {
		// we want a nice, big coc, hence we need to tap once from this (higher resolution) texture
		dofBlurMaterial.SetTexture ("_TapHigh", from);
		
		var tmp : RenderTexture = RenderTexture.GetTemporary (to.width, to.height);	
		if (iterations > 1) {
			BlurHex (from, to, blurPass, spread, tmp);	
			if (iterations > 2) {
				dofBlurMaterial.SetVector ("offsets", Vector4 (0.0, spread * oneOverBaseSize, 0.0, 0.0));
				Graphics.Blit (to, tmp, dofBlurMaterial, blurPass);
				dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0, 0.0, 0.0));		
				Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 	
			}		
		}
		else {
			dofBlurMaterial.SetVector ("offsets", Vector4 (0.0, spread * oneOverBaseSize, 0.0, 0.0));
			Graphics.Blit (from, tmp, dofBlurMaterial, blurPass);
			dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0, 0.0, 0.0));		
			Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 
		}	
		RenderTexture.ReleaseTemporary (tmp);
	}

	function BlurHex (from : RenderTexture, to : RenderTexture, blurPass: int, spread : float, tmp : RenderTexture) {		
		dofBlurMaterial.SetVector ("offsets", Vector4 (0.0, spread * oneOverBaseSize, 0.0, 0.0));
		Graphics.Blit (from, tmp, dofBlurMaterial, blurPass);
		dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  0.0, 0.0, 0.0));		
		Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 
		dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  spread * oneOverBaseSize, 0.0, 0.0));		
		Graphics.Blit (to, tmp, dofBlurMaterial, blurPass);	 
		dofBlurMaterial.SetVector ("offsets", Vector4 (spread / widthOverHeight * oneOverBaseSize,  -spread * oneOverBaseSize, 0.0, 0.0));		
		Graphics.Blit (tmp, to, dofBlurMaterial, blurPass);	 
	}
	
	function Downsample (from : RenderTexture, to : RenderTexture) {
		dofMaterial.SetVector ("_InvRenderTargetSize", Vector4 (1.0f / (1.0f * to.width), 1.0f / (1.0f * to.height), 0.0f, 0.0f));
		Graphics.Blit (from, to, dofMaterial, SMOOTH_DOWNSAMPLE_PASS);				
	}

	function AddBokeh (bokehInfo : RenderTexture, tempTex : RenderTexture, finalTarget : RenderTexture) {
		if (bokehMaterial) {
			var meshes : Mesh[] = Quads.GetMeshes (tempTex.width, tempTex.height);	// quads: exchanging more triangles with less overdraw			
			    
			RenderTexture.active = tempTex;
        	GL.Clear (false, true, Color (0.0f, 0.0f, 0.0f, 0.0f));	    
			    
			GL.PushMatrix ();
			GL.LoadIdentity ();			
			
			// point filter mode is important, otherwise we get bokeh shape & size artefacts
			bokehInfo.filterMode = FilterMode.Point;

			var arW : float = (bokehInfo.width * 1.0f) / (bokehInfo.height * 1.0f);			
			var sc : float = 2.0f / (1.0f * bokehInfo.width);
			sc += bokehScale * maxBlurSpread * BOKEH_EXTRA_BLUR * oneOverBaseSize;
			
			bokehMaterial.SetTexture ("_Source", bokehInfo);
			bokehMaterial.SetTexture ("_MainTex", bokehTexture);
			bokehMaterial.SetVector ("_ArScale", Vector4 (sc, sc * arW, 0.5f, 0.5f * arW));
			bokehMaterial.SetFloat ("_Intensity", bokehIntensity);
			bokehMaterial.SetPass (0);	
			
			for (var m : Mesh in meshes)
				if (m) Graphics.DrawMeshNow (m, Matrix4x4.identity);	
	
			GL.PopMatrix ();
				
			Graphics.Blit (tempTex, finalTarget, dofMaterial, 8);    		
			
			// important to set back as we sample from this later on
			bokehInfo.filterMode = FilterMode.Bilinear;
		}	
	}
	
	
	function ReleaseTextures () {
		if (foregroundTexture) RenderTexture.ReleaseTemporary (foregroundTexture);
		if (finalDefocus) RenderTexture.ReleaseTemporary (finalDefocus);
		if (mediumRezWorkTexture) RenderTexture.ReleaseTemporary (mediumRezWorkTexture);
		if (lowRezWorkTexture) RenderTexture.ReleaseTemporary (lowRezWorkTexture);
		if (bokehSource) RenderTexture.ReleaseTemporary (bokehSource);
		if (bokehSource2) RenderTexture.ReleaseTemporary (bokehSource2);
	}
	
	function AllocateTextures (blurForeground : boolean, source : RenderTexture, divider : int, lowTexDivider : int) {
        foregroundTexture = null;
        if (blurForeground)
        	foregroundTexture = RenderTexture.GetTemporary (source.width, source.height, 0); 
		mediumRezWorkTexture = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);         
        finalDefocus = RenderTexture.GetTemporary (source.width / divider, source.height / divider, 0);    
        lowRezWorkTexture  = RenderTexture.GetTemporary (source.width / lowTexDivider, source.height / lowTexDivider, 0);     
		bokehSource = null;
		bokehSource2 = null;
 		if (bokeh) {
        	bokehSource  = RenderTexture.GetTemporary (source.width / (lowTexDivider * bokehDownsample), source.height / (lowTexDivider * bokehDownsample), 0, RenderTextureFormat.ARGBHalf); 
        	bokehSource2  = RenderTexture.GetTemporary (source.width / (lowTexDivider * bokehDownsample), source.height / (lowTexDivider * bokehDownsample), 0,  RenderTextureFormat.ARGBHalf);
        	bokehSource.filterMode = FilterMode.Bilinear;
        	bokehSource2.filterMode = FilterMode.Bilinear;
        	RenderTexture.active = bokehSource2;
        	GL.Clear (false, true, Color(0.0f, 0.0f, 0.0f, 0.0f));   	        	        	
 		}    
        
        // to make sure: always use bilinear filter setting
        
        source.filterMode = FilterMode.Bilinear;
        finalDefocus.filterMode = FilterMode.Bilinear;
        mediumRezWorkTexture.filterMode = FilterMode.Bilinear;    
        lowRezWorkTexture.filterMode = FilterMode.Bilinear;     
        if (foregroundTexture)
        	foregroundTexture.filterMode = FilterMode.Bilinear;   	
	}	
}
