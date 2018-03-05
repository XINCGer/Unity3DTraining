
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)

class PostEffectsBase extends MonoBehaviour {	
	protected var supportHDRTextures : boolean = true;
	protected var supportDX11 : boolean = false;
	protected var isSupported : boolean = true;
	
	function CheckShaderAndCreateMaterial (s : Shader, m2Create : Material) : Material {
		if (!s) { 
			Debug.Log("Missing shader in " + this.ToString ());
			enabled = false;
			return null;
		}
			
		if (s.isSupported && m2Create && m2Create.shader == s) 
			return m2Create;
		
		if (!s.isSupported) {
			NotSupported ();
			Debug.Log("The shader " + s.ToString() + " on effect "+this.ToString()+" is not supported on this platform!");
			return null;
		}
		else {
			m2Create = new Material (s);	
			m2Create.hideFlags = HideFlags.DontSave;		
			if (m2Create) 
				return m2Create;
			else return null;
		}
	}

	function CreateMaterial (s : Shader, m2Create : Material) : Material {
		if (!s) { 
			Debug.Log ("Missing shader in " + this.ToString ());
			return null;
		}
			
		if (m2Create && (m2Create.shader == s) && (s.isSupported)) 
			return m2Create;
		
		if (!s.isSupported) {
			return null;
		}
		else {
			m2Create = new Material (s);	
			m2Create.hideFlags = HideFlags.DontSave;		
			if (m2Create) 
				return m2Create;
			else return null;
		}
	}
	
	function OnEnable() {
		isSupported = true;
	}	

	function CheckSupport () : boolean {
		return CheckSupport (false);
	}
	
	function CheckResources () : boolean {
		Debug.LogWarning ("CheckResources () for " + this.ToString() + " should be overwritten.");
		return isSupported;
	}
	
	function Start () {
		 CheckResources ();
	}	
		
	function CheckSupport (needDepth : boolean) : boolean {
		isSupported = true;
		supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
		
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
			NotSupported ();
			return false;
		}		
		
		if(needDepth && !SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.Depth)) {
			NotSupported ();
			return false;
		}
		
		if(needDepth)
			GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;	
		
		return true;
	}

	function CheckSupport (needDepth : boolean, needHdr : boolean) : boolean {
		if(!CheckSupport(needDepth))
			return false;
		
		if(needHdr && !supportHDRTextures) {
			NotSupported ();
			return false;		
		}
		
		return true;
	}	
	
	function Dx11Support() : boolean {
		return supportDX11;
	}

	function ReportAutoDisable () {
		Debug.LogWarning ("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}
			
	// deprecated but needed for old effects to survive upgrading
	function CheckShader (s : Shader) : boolean {
		Debug.Log("The shader " + s.ToString () + " on effect "+ this.ToString () + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");		
		if (!s.isSupported) {
			NotSupported ();
			return false;
		} 
		else {
			return false;
		}
	}
	
	function NotSupported () {
		enabled = false;
		isSupported = false;
		return;
	}
	
	function DrawBorder (dest : RenderTexture, material : Material ) {
		var x1 : float;	
		var x2 : float;
		var y1 : float;
		var y2 : float;		
		
		RenderTexture.active = dest;
        var invertY : boolean = true; // source.texelSize.y < 0.0f;
        // Set up the simple Matrix
        GL.PushMatrix();
        GL.LoadOrtho();		
        
        for (var i : int = 0; i < material.passCount; i++)
        {
            material.SetPass(i);
	        
	        var y1_ : float; var y2_ : float;
	        if (invertY)
	        {
	            y1_ = 1.0; y2_ = 0.0;
	        }
	        else
	        {
	            y1_ = 0.0; y2_ = 1.0;
	        }
	        	        
	        // left	        
	        x1 = 0.0;
	        x2 = 0.0 + 1.0/(dest.width*1.0);
	        y1 = 0.0;
	        y2 = 1.0;
	        GL.Begin(GL.QUADS);
	        
	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);
	
	        // right
	        x1 = 1.0 - 1.0/(dest.width*1.0);
	        x2 = 1.0;
	        y1 = 0.0;
	        y2 = 1.0;

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);	        
	
	        // top
	        x1 = 0.0;
	        x2 = 1.0;
	        y1 = 0.0;
	        y2 = 0.0 + 1.0/(dest.height*1.0);

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);
	        
	        // bottom
	        x1 = 0.0;
	        x2 = 1.0;
	        y1 = 1.0 - 1.0/(dest.height*1.0);
	        y2 = 1.0;

	        GL.TexCoord2(0.0, y1_); GL.Vertex3(x1, y1, 0.1);
	        GL.TexCoord2(1.0, y1_); GL.Vertex3(x2, y1, 0.1);
	        GL.TexCoord2(1.0, y2_); GL.Vertex3(x2, y2, 0.1);
	        GL.TexCoord2(0.0, y2_); GL.Vertex3(x1, y2, 0.1);	
	                	              
	        GL.End();	
        }	
        
        GL.PopMatrix();
	}
}