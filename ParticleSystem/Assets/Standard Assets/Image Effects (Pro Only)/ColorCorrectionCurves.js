
#pragma strict
@script ExecuteInEditMode
@script AddComponentMenu ("Image Effects/Color Adjustments/Color Correction (Curves, Saturation)")

enum ColorCorrectionMode {
	Simple = 0,
	Advanced = 1	
}

class ColorCorrectionCurves extends PostEffectsBase 
{
	public var redChannel : AnimationCurve;
	public var greenChannel : AnimationCurve;
	public var blueChannel : AnimationCurve;
	
	public var useDepthCorrection : boolean = false;
	
	public var zCurve : AnimationCurve;
	public var depthRedChannel : AnimationCurve;
	public var depthGreenChannel : AnimationCurve;
	public var depthBlueChannel : AnimationCurve;
	
	private var ccMaterial : Material;
	private var ccDepthMaterial : Material;
	private var selectiveCcMaterial : Material;
	
	private var rgbChannelTex : Texture2D;
	private var rgbDepthChannelTex : Texture2D;
	private var zCurveTex : Texture2D;
	
	public var saturation : float = 1.0f;

	public var selectiveCc : boolean = false;
	
	public var selectiveFromColor : Color = Color.white;
	public var selectiveToColor : Color = Color.white;
	
	public var mode : ColorCorrectionMode;
	
	public var updateTextures : boolean = true;		
		
	public var colorCorrectionCurvesShader : Shader = null;
	public var simpleColorCorrectionCurvesShader : Shader = null;
	public var colorCorrectionSelectiveShader : Shader = null;
			
	private var updateTexturesOnStartup : boolean = true;
		
	function Start () {
		super ();
		updateTexturesOnStartup = true;
	}
	
	function Awake () {	}
	
	function CheckResources () : boolean {		
		CheckSupport (mode == ColorCorrectionMode.Advanced);
	
		ccMaterial = CheckShaderAndCreateMaterial (simpleColorCorrectionCurvesShader, ccMaterial);
		ccDepthMaterial = CheckShaderAndCreateMaterial (colorCorrectionCurvesShader, ccDepthMaterial);
		selectiveCcMaterial = CheckShaderAndCreateMaterial (colorCorrectionSelectiveShader, selectiveCcMaterial);
		
		if (!rgbChannelTex)
			 rgbChannelTex = new Texture2D (256, 4, TextureFormat.ARGB32, false, true); 
		if (!rgbDepthChannelTex)
			 rgbDepthChannelTex = new Texture2D (256, 4, TextureFormat.ARGB32, false, true);
		if (!zCurveTex)
			 zCurveTex = new Texture2D (256, 1, TextureFormat.ARGB32, false, true);
			 
		rgbChannelTex.hideFlags = HideFlags.DontSave;
		rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
		zCurveTex.hideFlags = HideFlags.DontSave;
			
		rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
		rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
		zCurveTex.wrapMode = TextureWrapMode.Clamp;	
					
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;		  
	}	
	
	public function UpdateParameters () 
	{			
		if (redChannel && greenChannel && blueChannel) {		
			for (var i : float = 0.0f; i <= 1.0f; i += 1.0f / 255.0f) {
				var rCh : float = Mathf.Clamp (redChannel.Evaluate(i), 0.0f, 1.0f);
				var gCh : float = Mathf.Clamp (greenChannel.Evaluate(i), 0.0f, 1.0f);
				var bCh : float = Mathf.Clamp (blueChannel.Evaluate(i), 0.0f, 1.0f);
				
				rgbChannelTex.SetPixel (Mathf.Floor(i*255.0f), 0, Color(rCh,rCh,rCh) );
				rgbChannelTex.SetPixel (Mathf.Floor(i*255.0f), 1, Color(gCh,gCh,gCh) );
				rgbChannelTex.SetPixel (Mathf.Floor(i*255.0f), 2, Color(bCh,bCh,bCh) );
				
				var zC : float = Mathf.Clamp (zCurve.Evaluate(i), 0.0,1.0);
					
				zCurveTex.SetPixel (Mathf.Floor(i*255.0), 0, Color(zC,zC,zC) );
			
				rCh = Mathf.Clamp (depthRedChannel.Evaluate(i), 0.0f,1.0f);
				gCh = Mathf.Clamp (depthGreenChannel.Evaluate(i), 0.0f,1.0f);
				bCh = Mathf.Clamp (depthBlueChannel.Evaluate(i), 0.0f,1.0f);
				
				rgbDepthChannelTex.SetPixel (Mathf.Floor(i*255.0f), 0, Color(rCh,rCh,rCh) );
				rgbDepthChannelTex.SetPixel (Mathf.Floor(i*255.0f), 1, Color(gCh,gCh,gCh) );
				rgbDepthChannelTex.SetPixel (Mathf.Floor(i*255.0f), 2, Color(bCh,bCh,bCh) );
			}
			
			rgbChannelTex.Apply ();
			rgbDepthChannelTex.Apply ();
			zCurveTex.Apply ();				
		}
	}
	
	function UpdateTextures () {
		UpdateParameters ();			
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
		
		if (updateTexturesOnStartup) {
			UpdateParameters ();
			updateTexturesOnStartup = false;
		}
		
		if (useDepthCorrection)
			GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;			
		
		var renderTarget2Use : RenderTexture = destination;
		
		if (selectiveCc) {
			renderTarget2Use = RenderTexture.GetTemporary (source.width, source.height);
		}
		
		if (useDepthCorrection) {
			ccDepthMaterial.SetTexture ("_RgbTex", rgbChannelTex);
			ccDepthMaterial.SetTexture ("_ZCurve", zCurveTex);
			ccDepthMaterial.SetTexture ("_RgbDepthTex", rgbDepthChannelTex);
			ccDepthMaterial.SetFloat ("_Saturation", saturation);
	
			Graphics.Blit (source, renderTarget2Use, ccDepthMaterial); 	
		} 
		else {
			ccMaterial.SetTexture ("_RgbTex", rgbChannelTex);
			ccMaterial.SetFloat ("_Saturation", saturation);
			
			Graphics.Blit (source, renderTarget2Use, ccMaterial); 			
		}
		
		if (selectiveCc) {
			selectiveCcMaterial.SetColor ("selColor", selectiveFromColor);
			selectiveCcMaterial.SetColor ("targetColor", selectiveToColor);
			Graphics.Blit (renderTarget2Use, destination, selectiveCcMaterial); 	
			
			RenderTexture.ReleaseTemporary (renderTarget2Use);
		}
	}
}