
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Rendering/Global Fog")

class GlobalFog extends PostEffectsBase {
	
	enum FogMode {
		AbsoluteYAndDistance = 0,
		AbsoluteY = 1,
		Distance = 2,
		RelativeYAndDistance = 3,
	}
	
	public var fogMode : FogMode = FogMode.AbsoluteYAndDistance;
	
	private var CAMERA_NEAR : float = 0.5f;
	private var CAMERA_FAR : float = 50.0f;
	private var CAMERA_FOV : float = 60.0f;	
	private var CAMERA_ASPECT_RATIO : float = 1.333333f;
	
	public var startDistance : float = 200.0f;
	public var globalDensity : float = 1.0f;
	public var heightScale : float = 100.0f;
	public var height : float = 0.0f;

	public var globalFogColor : Color = Color.grey;
	
	public var fogShader : Shader;
	private var fogMaterial : Material = null;	
	
	function CheckResources () : boolean {	
		CheckSupport (true);
	
		fogMaterial = CheckShaderAndCreateMaterial (fogShader, fogMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;				
	}

	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}
			
		CAMERA_NEAR = GetComponent.<Camera>().nearClipPlane;
		CAMERA_FAR = GetComponent.<Camera>().farClipPlane;
		CAMERA_FOV = GetComponent.<Camera>().fieldOfView;
		CAMERA_ASPECT_RATIO = GetComponent.<Camera>().aspect;
	
		var frustumCorners : Matrix4x4 = Matrix4x4.identity;		
		var vec : Vector4;
		var corner : Vector3;
	
		var fovWHalf : float = CAMERA_FOV * 0.5f;
		
		var toRight : Vector3 = GetComponent.<Camera>().transform.right * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * CAMERA_ASPECT_RATIO;
		var toTop : Vector3 = GetComponent.<Camera>().transform.up * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);
	
		var topLeft : Vector3 = (GetComponent.<Camera>().transform.forward * CAMERA_NEAR - toRight + toTop);
		var CAMERA_SCALE : float = topLeft.magnitude * CAMERA_FAR/CAMERA_NEAR;	
			
		topLeft.Normalize();
		topLeft *= CAMERA_SCALE;
	
		var topRight : Vector3 = (GetComponent.<Camera>().transform.forward * CAMERA_NEAR + toRight + toTop);
		topRight.Normalize();
		topRight *= CAMERA_SCALE;
		
		var bottomRight : Vector3 = (GetComponent.<Camera>().transform.forward * CAMERA_NEAR + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= CAMERA_SCALE;
		
		var bottomLeft : Vector3 = (GetComponent.<Camera>().transform.forward * CAMERA_NEAR - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= CAMERA_SCALE;
				
		frustumCorners.SetRow (0, topLeft); 
		frustumCorners.SetRow (1, topRight);		
		frustumCorners.SetRow (2, bottomRight);
		frustumCorners.SetRow (3, bottomLeft);		
								
		fogMaterial.SetMatrix ("_FrustumCornersWS", frustumCorners);
		fogMaterial.SetVector ("_CameraWS", GetComponent.<Camera>().transform.position);
		fogMaterial.SetVector ("_StartDistance", Vector4 (1.0f / startDistance, (CAMERA_SCALE-startDistance)));
		fogMaterial.SetVector ("_Y", Vector4 (height, 1.0f / heightScale));
		
		fogMaterial.SetFloat ("_GlobalDensity", globalDensity * 0.01f);
		fogMaterial.SetColor ("_FogColor", globalFogColor);
		
		CustomGraphicsBlit (source, destination, fogMaterial, fogMode);
	}
	
	static function CustomGraphicsBlit (source : RenderTexture, dest : RenderTexture, fxMaterial : Material, passNr : int) {
		RenderTexture.active = dest;
		       
		fxMaterial.SetTexture ("_MainTex", source);	        
	        	        
		GL.PushMatrix ();
		GL.LoadOrtho ();	
	    	
		fxMaterial.SetPass (passNr);	
		
	    GL.Begin (GL.QUADS);
							
		GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
		GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL
		
		GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
		GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR
		
		GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
		GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR
		
		GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
		GL.Vertex3 (0.0f, 1.0f, 0.0); // TL
		
		GL.End ();
	    GL.PopMatrix ();
	}		
}
