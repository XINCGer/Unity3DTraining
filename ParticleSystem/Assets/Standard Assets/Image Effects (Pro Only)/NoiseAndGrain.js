
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Noise/Noise And Grain (Filmic)")

class NoiseAndGrain extends PostEffectsBase {

	public var intensityMultiplier : float = 0.25f;

	public var generalIntensity : float = 0.5f;
	public var blackIntensity : float = 1.0f;
	public var whiteIntensity : float = 1.0f;
	public var midGrey : float = 0.2f;

	public var dx11Grain : boolean = false;
	public var softness : float = 0.0f;
	public var monochrome : boolean = false;

	public var intensities : Vector3 = Vector3(1.0f, 1.0f, 1.0f);
	public var tiling : Vector3 = Vector3(64.0f, 64.0f, 64.0f);
	public var monochromeTiling : float = 64.0f;
	
	public var filterMode : FilterMode = FilterMode.Bilinear;
			
	public var noiseTexture : Texture2D;

	public var noiseShader : Shader;	
	private var noiseMaterial : Material = null;

	public var dx11NoiseShader : Shader;	
	private var dx11NoiseMaterial : Material = null;

	private static var TILE_AMOUNT : float = 64.0f;
	
	function CheckResources () : boolean {
		CheckSupport (false);
		
		noiseMaterial = CheckShaderAndCreateMaterial (noiseShader, noiseMaterial);

		if(dx11Grain && supportDX11) {
		#if UNITY_EDITOR
			dx11NoiseShader = Shader.Find("Hidden/NoiseAndGrainDX11");
		#endif
			dx11NoiseMaterial = CheckShaderAndCreateMaterial (dx11NoiseShader, dx11NoiseMaterial);			
		}
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
		if(CheckResources()==false || (null==noiseTexture)) {
			Graphics.Blit (source, destination);
			if(null==noiseTexture){	
				Debug.LogWarning("Noise & Grain effect failing as noise texture is not assigned. please assign.", transform);
			}
			return;
		}

   		softness = Mathf.Clamp(softness, 0.0f, 0.99f);

	   	if(dx11Grain && supportDX11) {
	   		// We have a fancy, procedural noise pattern in this version, so no texture needed

			dx11NoiseMaterial.SetFloat("_DX11NoiseTime", Time.frameCount);
			dx11NoiseMaterial.SetTexture ("_NoiseTex", noiseTexture);
			dx11NoiseMaterial.SetVector ("_NoisePerChannel", monochrome ? Vector3.one : intensities);
			dx11NoiseMaterial.SetVector ("_MidGrey", Vector3(midGrey, 1.0f/(1.0-midGrey), -1.0f/midGrey));
			dx11NoiseMaterial.SetVector ("_NoiseAmount", Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);

	   		if(softness > Mathf.Epsilon)
	   		{
	   			var rt : RenderTexture = RenderTexture.GetTemporary(source.width * (1.0f-softness), source.height * (1.0f-softness));
		   		DrawNoiseQuadGrid (source, rt, dx11NoiseMaterial, noiseTexture, monochrome ? 3 : 2);
		   		dx11NoiseMaterial.SetTexture("_NoiseTex", rt);
		   		Graphics.Blit(source, destination, dx11NoiseMaterial, 4);
	   			RenderTexture.ReleaseTemporary(rt);
	   		}
	   		else
	   			DrawNoiseQuadGrid (source, destination, dx11NoiseMaterial, noiseTexture, (monochrome ? 1 : 0));
	   	}
	   	else {
	   		// normal noise (DX9 style)

		   	if (noiseTexture) {
				noiseTexture.wrapMode = TextureWrapMode.Repeat;		   		
		   		noiseTexture.filterMode = filterMode; 
		   	}

			noiseMaterial.SetTexture ("_NoiseTex", noiseTexture);
			noiseMaterial.SetVector ("_NoisePerChannel", monochrome ? Vector3.one : intensities);
			noiseMaterial.SetVector ("_NoiseTilingPerChannel", monochrome ? Vector3.one * monochromeTiling : tiling);
			noiseMaterial.SetVector ("_MidGrey", Vector3(midGrey, 1.0f/(1.0-midGrey), -1.0f/midGrey));
			noiseMaterial.SetVector ("_NoiseAmount", Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);

	   		if(softness > Mathf.Epsilon)
	   		{
	   			var rt2 : RenderTexture = RenderTexture.GetTemporary(source.width * (1.0f-softness), source.height * (1.0f-softness));
		   		DrawNoiseQuadGrid (source, rt2, noiseMaterial, noiseTexture, 2);
		   		noiseMaterial.SetTexture("_NoiseTex", rt2);
		   		Graphics.Blit(source, destination, noiseMaterial, 1);
	   			RenderTexture.ReleaseTemporary(rt2);
	   		}
	   		else	   		
				DrawNoiseQuadGrid (source, destination, noiseMaterial, noiseTexture, 0);
		}
	}
		
	static function DrawNoiseQuadGrid (source : RenderTexture, dest : RenderTexture, fxMaterial : Material, noise : Texture2D, passNr : int) {
		RenderTexture.active = dest;
		
		var noiseSize : float = (noise.width * 1.0f);
		var subDs : float = (1.0f * source.width) / TILE_AMOUNT;
	       
		fxMaterial.SetTexture ("_MainTex", source);	        
	                
		GL.PushMatrix ();
		GL.LoadOrtho ();	
			
		var aspectCorrection : float = (1.0f * source.width) / (1.0f * source.height);
		var stepSizeX : float = 1.0f / subDs;
		var stepSizeY : float = stepSizeX * aspectCorrection; 
	   	var texTile : float = noiseSize / (noise.width * 1.0f);
	   		    	    	
		fxMaterial.SetPass (passNr);	
		
	    GL.Begin (GL.QUADS);
	    
	   	for (var x1 : float = 0.0; x1 < 1.0; x1 += stepSizeX) {
	   		for (var y1 : float = 0.0; y1 < 1.0; y1 += stepSizeY) { 
	   			
	   			var tcXStart : float = Random.Range (0.0f, 1.0f);
	   			var tcYStart : float = Random.Range (0.0f, 1.0f);

	   			//var v3 : Vector3 = Random.insideUnitSphere; 
	   			//var c : Color = new Color(v3.x, v3.y, v3.z);

	   			tcXStart = Mathf.Floor(tcXStart*noiseSize) / noiseSize;
	   			tcYStart = Mathf.Floor(tcYStart*noiseSize) / noiseSize;
	   			
	   			var texTileMod : float = 1.0f / noiseSize;
							
			    GL.MultiTexCoord2 (0, tcXStart, tcYStart); 
			    GL.MultiTexCoord2 (1, 0.0f, 0.0f);
			    //GL.Color( c );
			    GL.Vertex3 (x1, y1, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart); 
			    GL.MultiTexCoord2 (1, 1.0f, 0.0f); 
			    //GL.Color( c );
			    GL.Vertex3 (x1 + stepSizeX, y1, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart + texTile * texTileMod, tcYStart + texTile * texTileMod); 
			    GL.MultiTexCoord2 (1, 1.0f, 1.0f); 
			    //GL.Color( c );
			    GL.Vertex3 (x1 + stepSizeX, y1 + stepSizeY, 0.1);
			    GL.MultiTexCoord2 (0, tcXStart, tcYStart + texTile * texTileMod); 
			    GL.MultiTexCoord2 (1, 0.0f, 1.0f); 
			    //GL.Color( c );
			    GL.Vertex3 (x1, y1 + stepSizeY, 0.1);
	   		}
	   	}
	    	
		GL.End ();
	    GL.PopMatrix ();
	}
}