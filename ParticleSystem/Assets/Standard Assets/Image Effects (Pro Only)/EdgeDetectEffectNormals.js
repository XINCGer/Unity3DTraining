
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Edge Detection/Edge Detection")

enum EdgeDetectMode {
	TriangleDepthNormals = 0,
	RobertsCrossDepthNormals = 1,
	SobelDepth = 2,
	SobelDepthThin = 3,
	TriangleLuminance = 4,
}

class EdgeDetectEffectNormals extends PostEffectsBase {	

	public var mode : EdgeDetectMode = EdgeDetectMode.SobelDepthThin;
	public var sensitivityDepth : float = 1.0f;
	public var sensitivityNormals : float = 1.0f;
	public var lumThreshhold : float = 0.2f;
	public var edgeExp : float = 1.0f;
	public var sampleDist : float = 1.0f;
	public var edgesOnly : float = 0.0f;
	public var edgesOnlyBgColor : Color = Color.white;

	public var edgeDetectShader : Shader;
	private var edgeDetectMaterial : Material = null;
	private var oldMode : EdgeDetectMode = EdgeDetectMode.SobelDepthThin;

	function CheckResources () : boolean {	
		CheckSupport (true);
	
		edgeDetectMaterial = CheckShaderAndCreateMaterial (edgeDetectShader,edgeDetectMaterial);
		if (mode != oldMode)
			SetCameraFlag ();

		oldMode = mode;

		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}

	function Start () {
		oldMode	= mode;
	}

	function SetCameraFlag () {
		if (mode>1)
			GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;		
		else
			GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;		
	}

	function OnEnable() {
		SetCameraFlag();
	}
	
	@ImageEffectOpaque
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if (CheckResources () == false) {
			Graphics.Blit (source, destination);
			return;
		}
				
		var sensitivity : Vector2 = Vector2 (sensitivityDepth, sensitivityNormals);		
		edgeDetectMaterial.SetVector ("_Sensitivity", Vector4 (sensitivity.x, sensitivity.y, 1.0, sensitivity.y));		
		edgeDetectMaterial.SetFloat ("_BgFade", edgesOnly);	
		edgeDetectMaterial.SetFloat ("_SampleDistance", sampleDist);		
		edgeDetectMaterial.SetVector ("_BgColor", edgesOnlyBgColor);	
		edgeDetectMaterial.SetFloat ("_Exponent", edgeExp);
		edgeDetectMaterial.SetFloat ("_Threshold", lumThreshhold);
		
		Graphics.Blit (source, destination, edgeDetectMaterial, mode);
	}
}

