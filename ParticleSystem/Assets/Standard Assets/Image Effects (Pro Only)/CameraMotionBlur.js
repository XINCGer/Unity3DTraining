
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Camera/Camera Motion Blur") 

public class CameraMotionBlur extends PostEffectsBase 
{
	// make sure to match this to MAX_RADIUS in shader ('k' in paper)
	static var MAX_RADIUS : int = 10.0f;

	public enum MotionBlurFilter {
		CameraMotion = 0, 			// global screen blur based on cam motion
		LocalBlur = 1, 				// cheap blur, no dilation or scattering
		Reconstruction = 2, 		// advanced filter (simulates scattering) as in plausible motion blur paper
		ReconstructionDX11 = 3, 	// advanced filter (simulates scattering) as in plausible motion blur paper
		ReconstructionDisc = 4,		// advanced filter using scaled poisson disc sampling
	}

	// settings		
	public var filterType : MotionBlurFilter = MotionBlurFilter.Reconstruction;	
	public var preview : boolean = false;				// show how blur would look like in action ...
	public var previewScale : Vector3 = Vector3.one;	// ... given this movement vector
	
	// params
	public var movementScale : float = 0.0f;
	public var rotationScale : float = 1.0f;
	public var maxVelocity : float = 8.0f;	// maximum velocity in pixels
	public var minVelocity : float = 0.1f;	// minimum velocity in pixels
	public var velocityScale : float = 0.375f;	// global velocity scale
	public var softZDistance : float = 0.005f;	// for z overlap check softness (reconstruction filter only)
	public var velocityDownsample : int = 1;	// low resolution velocity buffer? (optimization)
	public var excludeLayers : LayerMask = 0;
	//public var dynamicLayers : LayerMask = 0;
	private var tmpCam : GameObject = null;

	// resources
	public var shader : Shader;
	public var dx11MotionBlurShader : Shader;
	public var replacementClear : Shader;
	//public var replacementDynamics : Shader;
	private var motionBlurMaterial : Material = null;
	private var dx11MotionBlurMaterial : Material = null;

	public var noiseTexture : Texture2D = null;
	public var jitter : float = 0.05f;

	// (internal) debug
	public var showVelocity : boolean = false;
	public var showVelocityScale : float = 1.0f;	
	
	// camera transforms
	private var currentViewProjMat : Matrix4x4;
	private var prevViewProjMat : Matrix4x4;
	private var prevFrameCount : int;
	private var wasActive : boolean;
	// shortcuts to calculate global blur direction when using 'CameraMotion'
	private var prevFrameForward : Vector3 = Vector3.forward;            	            	            	            	            	            	            	            	            	            	            	            	            	            	            	
	private var prevFrameRight : Vector3 = Vector3.right;            	            	            	            	            	            	            	            	            	            	            	            	            	            	            	
	private var prevFrameUp : Vector3 = Vector3.up;       
	private var prevFramePos : Vector3 = Vector3.zero;     	 

	private function CalculateViewProjection() {
		var viewMat : Matrix4x4 = GetComponent.<Camera>().worldToCameraMatrix;
		var projMat : Matrix4x4 = GL.GetGPUProjectionMatrix (GetComponent.<Camera>().projectionMatrix, true);
		currentViewProjMat = projMat * viewMat;				
	}
	
	function Start () {
		CheckResources ();

		wasActive = gameObject.activeInHierarchy;
		CalculateViewProjection ();
		Remember ();
		wasActive = false; // hack to fake position/rotation update and prevent bad blurs
	}

	function OnEnable () {		
		GetComponent.<Camera>().depthTextureMode |= DepthTextureMode.Depth;	
	}
		
	function OnDisable () {
		if (null != motionBlurMaterial) {
			DestroyImmediate (motionBlurMaterial);
			motionBlurMaterial = null;
		}
		if (null != dx11MotionBlurMaterial) {
			DestroyImmediate (dx11MotionBlurMaterial);
			dx11MotionBlurMaterial = null;
		}		
		if (null != tmpCam) {
			DestroyImmediate (tmpCam);
			tmpCam = null;
		}
	}

	function CheckResources () : boolean {
		CheckSupport (true, true); // depth & hdr needed
		motionBlurMaterial = CheckShaderAndCreateMaterial (shader, motionBlurMaterial);

		if (supportDX11 && filterType == MotionBlurFilter.ReconstructionDX11) {
			dx11MotionBlurMaterial = CheckShaderAndCreateMaterial (dx11MotionBlurShader, dx11MotionBlurMaterial);
		}
	
		if (!isSupported)
			ReportAutoDisable ();

		return isSupported;			
	}		
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {	
		if (false == CheckResources ()) {
			Graphics.Blit (source, destination);
			return;
		}

		if (filterType == MotionBlurFilter.CameraMotion)
			StartFrame ();

		// use if possible new RG format ... fallback to half otherwise
		var rtFormat = SystemInfo.SupportsRenderTextureFormat (RenderTextureFormat.RGHalf) ? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;

		// get temp textures
		var velBuffer : RenderTexture = RenderTexture.GetTemporary (divRoundUp (source.width, velocityDownsample), divRoundUp (source.height, velocityDownsample), 0, rtFormat);
		var tileWidth : int = 1;
		var tileHeight : int = 1;
		maxVelocity = Mathf.Max (2.0f, maxVelocity);

		var _maxVelocity : float = maxVelocity; // calculate 'k'
		// note: 's' is hardcoded in shaders except for DX11 path

		// auto DX11 fallback!
		var fallbackFromDX11 : boolean = false;
		if (filterType == MotionBlurFilter.ReconstructionDX11 && dx11MotionBlurMaterial == null) {
			fallbackFromDX11 = true;
		}		

		if (filterType == MotionBlurFilter.Reconstruction || fallbackFromDX11 || filterType == MotionBlurFilter.ReconstructionDisc) {
			maxVelocity = Mathf.Min (maxVelocity, MAX_RADIUS);
			tileWidth = divRoundUp (velBuffer.width, maxVelocity);
			tileHeight = divRoundUp (velBuffer.height, maxVelocity);
			_maxVelocity = velBuffer.width/tileWidth;
		}
		else {
			tileWidth = divRoundUp (velBuffer.width, maxVelocity);
			tileHeight = divRoundUp (velBuffer.height, maxVelocity);
			_maxVelocity = velBuffer.width/tileWidth;		
		}

		var tileMax : RenderTexture  = RenderTexture.GetTemporary (tileWidth, tileHeight, 0, rtFormat);
		var neighbourMax : RenderTexture  = RenderTexture.GetTemporary (tileWidth, tileHeight, 0, rtFormat);
		velBuffer.filterMode = FilterMode.Point;		
		tileMax.filterMode = FilterMode.Point;
		neighbourMax.filterMode = FilterMode.Point;
		if(noiseTexture) noiseTexture.filterMode = FilterMode.Point;
		source.wrapMode = TextureWrapMode.Clamp;
		velBuffer.wrapMode = TextureWrapMode.Clamp;
		neighbourMax.wrapMode = TextureWrapMode.Clamp;
		tileMax.wrapMode = TextureWrapMode.Clamp;

		// calc correct viewprj matrix
		CalculateViewProjection ();

		// just started up?		
		if (gameObject.activeInHierarchy && !wasActive) {
			Remember ();		
		}
		wasActive = gameObject.activeInHierarchy;
		
		// matrices
		var invViewPrj : Matrix4x4 = Matrix4x4.Inverse (currentViewProjMat);
		motionBlurMaterial.SetMatrix ("_InvViewProj", invViewPrj);
		motionBlurMaterial.SetMatrix ("_PrevViewProj", prevViewProjMat);
		motionBlurMaterial.SetMatrix ("_ToPrevViewProjCombined", prevViewProjMat * invViewPrj);		

		motionBlurMaterial.SetFloat ("_MaxVelocity", _maxVelocity);
		motionBlurMaterial.SetFloat ("_MaxRadiusOrKInPaper", _maxVelocity);
		motionBlurMaterial.SetFloat ("_MinVelocity", minVelocity);
		motionBlurMaterial.SetFloat ("_VelocityScale", velocityScale);
		motionBlurMaterial.SetFloat ("_Jitter", jitter);
		
		// texture samplers
		motionBlurMaterial.SetTexture ("_NoiseTex", noiseTexture);
		motionBlurMaterial.SetTexture ("_VelTex", velBuffer);
		motionBlurMaterial.SetTexture ("_NeighbourMaxTex", neighbourMax);
		motionBlurMaterial.SetTexture ("_TileTexDebug", tileMax);

		if (preview) {
			// generate an artifical 'previous' matrix to simulate blur look
			var viewMat : Matrix4x4 = GetComponent.<Camera>().worldToCameraMatrix;
			var offset : Matrix4x4 = Matrix4x4.identity;
			offset.SetTRS(previewScale * 0.3333f, Quaternion.identity, Vector3.one); // using only translation
			var projMat : Matrix4x4 = GL.GetGPUProjectionMatrix (GetComponent.<Camera>().projectionMatrix, true);
			prevViewProjMat = projMat * offset * viewMat;
			motionBlurMaterial.SetMatrix ("_PrevViewProj", prevViewProjMat);
			motionBlurMaterial.SetMatrix ("_ToPrevViewProjCombined", prevViewProjMat * invViewPrj);			
		}

		if (filterType == MotionBlurFilter.CameraMotion)
		{
			// build blur vector to be used in shader to create a global blur direction
			var blurVector : Vector4 = Vector4.zero;

			var lookUpDown : float = Vector3.Dot (transform.up, Vector3.up);
			var distanceVector : Vector3 = prevFramePos-transform.position;

			var distMag : float = distanceVector.magnitude;

			var farHeur : float = 1.0f;

			// pitch (vertical)
			farHeur = (Vector3.Angle (transform.up, prevFrameUp) / GetComponent.<Camera>().fieldOfView) * (source.width * 0.75f);
			blurVector.x =  rotationScale * farHeur;//Mathf.Clamp01((1.0f-Vector3.Dot(transform.up, prevFrameUp)));

			// yaw #1 (horizontal, faded by pitch)
			farHeur = (Vector3.Angle (transform.forward, prevFrameForward) / GetComponent.<Camera>().fieldOfView) * (source.width * 0.75f);
			blurVector.y = rotationScale * lookUpDown * farHeur;//Mathf.Clamp01((1.0f-Vector3.Dot(transform.forward, prevFrameForward)));

			// yaw #2 (when looking down, faded by 1-pitch)
			farHeur = (Vector3.Angle (transform.forward, prevFrameForward) / GetComponent.<Camera>().fieldOfView) * (source.width * 0.75f);			
			blurVector.z = rotationScale * (1.0f- lookUpDown) * farHeur;//Mathf.Clamp01((1.0f-Vector3.Dot(transform.forward, prevFrameForward)));

			if (distMag > Mathf.Epsilon && movementScale > Mathf.Epsilon) {
				// forward (probably most important)
				blurVector.w = movementScale * (Vector3.Dot (transform.forward, distanceVector) ) * (source.width * 0.5f);
				// jump (maybe scale down further)
				blurVector.x += movementScale * (Vector3.Dot (transform.up, distanceVector) ) * (source.width * 0.5f);
				// strafe (maybe scale down further)
				blurVector.y += movementScale * (Vector3.Dot (transform.right, distanceVector) ) * (source.width * 0.5f);
			}

			if (preview) // crude approximation
				motionBlurMaterial.SetVector ("_BlurDirectionPacked", Vector4 (previewScale.y, previewScale.x, 0.0f, previewScale.z) * 0.5f * GetComponent.<Camera>().fieldOfView);
			else
				motionBlurMaterial.SetVector ("_BlurDirectionPacked", blurVector);
		}
		else {		
			// generate velocity buffer	
			Graphics.Blit (source, velBuffer, motionBlurMaterial, 0);

			// patch up velocity buffer:

			// exclude certain layers (e.g. skinned objects as we cant really support that atm)

			var cam : Camera = null;
			if (excludeLayers.value)// || dynamicLayers.value)
				cam = GetTmpCam ();
	
			if (cam && excludeLayers.value != 0 && replacementClear && replacementClear.isSupported) {
				cam.targetTexture = velBuffer;				
				cam.cullingMask = excludeLayers;
				cam.RenderWithShader (replacementClear, "");
			}
			
			// dynamic layers (e.g. rigid bodies)
			// no worky in 4.0, but let's fix for 4.x
			/*
			if (cam && dynamicLayers.value != 0 && replacementDynamics && replacementDynamics.isSupported) {

				Shader.SetGlobalFloat ("_MaxVelocity", maxVelocity);
				Shader.SetGlobalFloat ("_VelocityScale", velocityScale);
				Shader.SetGlobalVector ("_VelBufferSize", Vector4 (velBuffer.width, velBuffer.height, 0, 0));
				Shader.SetGlobalMatrix ("_PrevViewProj", prevViewProjMat);
				Shader.SetGlobalMatrix ("_ViewProj", currentViewProjMat);

				cam.targetTexture = velBuffer;				
				cam.cullingMask = dynamicLayers;
				cam.RenderWithShader (replacementDynamics, "");
			}
			*/
			
		}

		if (!preview && Time.frameCount != prevFrameCount) {
			// remember current transformation data for next frame
			prevFrameCount = Time.frameCount;
			Remember ();
		}		

		source.filterMode = FilterMode.Bilinear;		

		// debug vel buffer:
		if (showVelocity) {
			// generate tile max and neighbour max		
			//Graphics.Blit (velBuffer, tileMax, motionBlurMaterial, 2);
			//Graphics.Blit (tileMax, neighbourMax, motionBlurMaterial, 3);
			motionBlurMaterial.SetFloat ("_DisplayVelocityScale", showVelocityScale);
			Graphics.Blit (velBuffer, destination, motionBlurMaterial, 1);
		} 
		else {
			if (filterType == MotionBlurFilter.ReconstructionDX11 && !fallbackFromDX11) {
				// need to reset some parameters for dx11 shader
				dx11MotionBlurMaterial.SetFloat ("_MinVelocity", minVelocity);
				dx11MotionBlurMaterial.SetFloat ("_VelocityScale", velocityScale);
				dx11MotionBlurMaterial.SetFloat ("_Jitter", jitter);

				// texture samplers
				dx11MotionBlurMaterial.SetTexture ("_NoiseTex", noiseTexture);
				dx11MotionBlurMaterial.SetTexture ("_VelTex", velBuffer);
				dx11MotionBlurMaterial.SetTexture ("_NeighbourMaxTex", neighbourMax);
				
				dx11MotionBlurMaterial.SetFloat ("_SoftZDistance", Mathf.Max(0.00025f, softZDistance) );				
				dx11MotionBlurMaterial.SetFloat ("_MaxRadiusOrKInPaper", _maxVelocity);
				
				// generate tile max and neighbour max		
				Graphics.Blit (velBuffer, tileMax, dx11MotionBlurMaterial, 0);
				Graphics.Blit (tileMax, neighbourMax, dx11MotionBlurMaterial, 1);
				
				// final blur
				Graphics.Blit (source, destination, dx11MotionBlurMaterial, 2);	
			}
			else if (filterType == MotionBlurFilter.Reconstruction || fallbackFromDX11) {
				// 'reconstructing' properly integrated color
				motionBlurMaterial.SetFloat ("_SoftZDistance", Mathf.Max(0.00025f, softZDistance) );
				
				// generate tile max and neighbour max		
				Graphics.Blit (velBuffer, tileMax, motionBlurMaterial, 2);
				Graphics.Blit (tileMax, neighbourMax, motionBlurMaterial, 3);
				
				// final blur
				Graphics.Blit (source, destination, motionBlurMaterial, 4);
			}
			else if (filterType == MotionBlurFilter.CameraMotion) {
				// orange box style motion blur
				Graphics.Blit (source, destination, motionBlurMaterial, 6);				
			}
			else if (filterType == MotionBlurFilter.ReconstructionDisc) {
				// dof style motion blur defocuing and ellipse around the princical blur direction
				// 'reconstructing' properly integrated color
				motionBlurMaterial.SetFloat ("_SoftZDistance", Mathf.Max(0.00025f, softZDistance) );
				
				// generate tile max and neighbour max		
				Graphics.Blit (velBuffer, tileMax, motionBlurMaterial, 2);
				Graphics.Blit (tileMax, neighbourMax, motionBlurMaterial, 3);
								
				Graphics.Blit (source, destination, motionBlurMaterial, 7);
			}
			else {
				// simple & fast blur (low quality): just blurring along velocity
				Graphics.Blit (source, destination, motionBlurMaterial, 5);
			}
		}
		
		// cleanup
		RenderTexture.ReleaseTemporary (velBuffer);
		RenderTexture.ReleaseTemporary (tileMax);
		RenderTexture.ReleaseTemporary (neighbourMax);
	}

	function Remember () {
		prevViewProjMat = currentViewProjMat;
		prevFrameForward = transform.forward;
		prevFrameRight = transform.right;
		prevFrameUp = transform.up;
		prevFramePos = transform.position;			
	}

	function GetTmpCam () : Camera {
		if (tmpCam == null) {
			var name : String = "_" + GetComponent.<Camera>().name + "_MotionBlurTmpCam";
			var go : GameObject = GameObject.Find (name);
			if (null == go) // couldn't find, recreate
				tmpCam = new GameObject (name, typeof (Camera));
			else
				tmpCam = go;
		}

		tmpCam.hideFlags = HideFlags.DontSave;
		tmpCam.transform.position = GetComponent.<Camera>().transform.position;
		tmpCam.transform.rotation = GetComponent.<Camera>().transform.rotation;
		tmpCam.transform.localScale = GetComponent.<Camera>().transform.localScale;
		tmpCam.GetComponent.<Camera>().CopyFrom (GetComponent.<Camera>());

		tmpCam.GetComponent.<Camera>().enabled = false;
		tmpCam.GetComponent.<Camera>().depthTextureMode = DepthTextureMode.None;
		tmpCam.GetComponent.<Camera>().clearFlags = CameraClearFlags.Nothing;

		return tmpCam.GetComponent.<Camera>();
	}

	function StartFrame () {
		// take only x% of positional changes into account (camera motion)
		// TODO: possibly do the same for rotational part
		prevFramePos = Vector3.Slerp(prevFramePos, transform.position, 0.75f);
	}
			
	function divRoundUp (x : int, d : int) : int {
		return (x + d - 1) / d;
	}
}
