
#pragma strict
@script ExecuteInEditMode
@script AddComponentMenu ("Image Effects/Color Adjustments/Color Correction (3D Lookup Texture)")

public class ColorCorrectionLut extends PostEffectsBase
{
	public var shader : Shader;
	private var material : Material;

	// serialize this instead of having another 2d texture ref'ed
	public var converted3DLut : Texture3D = null;
	public var basedOnTempTex : String = "";

	function CheckResources () : boolean {		
		CheckSupport (false);

		material = CheckShaderAndCreateMaterial (shader, material);

		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;			
	}

	function OnDisable () {
		if (material) {
			DestroyImmediate (material);
			material = null;
		}
	}

	function OnDestroy () {
		if (converted3DLut)
			DestroyImmediate (converted3DLut);
		converted3DLut = null;
	}

	public function SetIdentityLut () {
			var dim : int = 16;
			var newC : Color[] = new Color[dim*dim*dim];
 			var oneOverDim : float = 1.0f / (1.0f * dim - 1.0f);

			for(var i : int = 0; i < dim; i++) {
				for(var j : int = 0; j < dim; j++) {
					for(var k : int = 0; k < dim; k++) {
						newC[i + (j*dim) + (k*dim*dim)] = new Color((i*1.0f)*oneOverDim, (j*1.0f)*oneOverDim, (k*1.0f)*oneOverDim, 1.0f);
					}
				}
			}

			if (converted3DLut)
				DestroyImmediate (converted3DLut);
			converted3DLut = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
			converted3DLut.SetPixels (newC);
			converted3DLut.Apply ();
			basedOnTempTex = "";		
	}

	public function ValidDimensions (tex2d : Texture2D) : boolean {
		if (!tex2d) return false;
		var h : int = tex2d.height;
		if (h != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width))) {
			return false;				
		}
		return true;
	}

	public function Convert (temp2DTex : Texture2D, path : String) {

		// conversion fun: the given 2D texture needs to be of the format
		//  w * h, wheras h is the 'depth' (or 3d dimension 'dim') and w = dim * dim

		if (temp2DTex) {
			var dim : int = temp2DTex.width * temp2DTex.height;
			dim = temp2DTex.height;

			if (!ValidDimensions(temp2DTex)) {
				Debug.LogWarning ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");				
				basedOnTempTex = "";
				return;				
			}

			var c : Color[] = temp2DTex.GetPixels();
			var newC : Color[] = new Color[c.Length];
 
			for(var i : int = 0; i < dim; i++) {
				for(var j : int = 0; j < dim; j++) {
					for(var k : int = 0; k < dim; k++) {
						var j_ : int = dim-j-1;
						newC[i + (j*dim) + (k*dim*dim)] = c[k*dim+i+j_*dim*dim];
					}
				}
			}

			if (converted3DLut)
				DestroyImmediate (converted3DLut);
			converted3DLut = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
			converted3DLut.SetPixels (newC);
			converted3DLut.Apply ();
			basedOnTempTex = path;
		}		
		else {
			// error, something went terribly wrong
			Debug.LogError ("Couldn't color correct with 3D LUT texture. Image Effect will be disabled.");
		}		
	}

	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if(CheckResources () == false) {
			Graphics.Blit (source, destination);
			return;
		}

		if (converted3DLut == null) {
			SetIdentityLut ();
		}
	
		var lutSize : int = converted3DLut.width;
		converted3DLut.wrapMode = TextureWrapMode.Clamp;
		material.SetFloat("_Scale", (lutSize - 1) / (1.0f*lutSize));
		material.SetFloat("_Offset", 1.0f / (2.0f * lutSize));		
		material.SetTexture("_ClutTex", converted3DLut);

		Graphics.Blit (source, destination, material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);			
	}
}
