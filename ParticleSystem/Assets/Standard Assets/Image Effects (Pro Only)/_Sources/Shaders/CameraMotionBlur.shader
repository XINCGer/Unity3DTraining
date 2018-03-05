// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 /*
 	CAMERA MOTION BLUR IMAGE EFFECTS

 	Reconstruction Filter:
	Based on "Plausible Motion Blur"
	http://graphics.cs.williams.edu/papers/MotionBlurI3D12/

	CameraMotion:
	Based on Alex Vlacho's technique in
	http://www.valvesoftware.com/publications/2008/GDC2008_PostProcessingInTheOrangeBox.pdf

	SimpleBlur:
	Straightforward sampling along velocities

	ScatterFromGather:
	Combines Reconstruction with depth of field type defocus
 */
 
 Shader "Hidden/CameraMotionBlur" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_NoiseTex ("-", 2D) = "grey" {}
		_VelTex ("-", 2D) = "black" {}
		_NeighbourMaxTex ("-", 2D) = "black" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	// 's' in paper (# of samples for reconstruction)
	#define NUM_SAMPLES (11)
	// # samples for valve style blur
	#define MOTION_SAMPLES (16)
	// 'k' in paper
	float _MaxRadiusOrKInPaper;

	static const int SmallDiscKernelSamples = 12;		
	static const float2 SmallDiscKernel[SmallDiscKernelSamples] =
	{
		float2(-0.326212,-0.40581),
		float2(-0.840144,-0.07358),
		float2(-0.695914,0.457137),
		float2(-0.203345,0.620716),
		float2(0.96234,-0.194983),
		float2(0.473434,-0.480026),
		float2(0.519456,0.767022),
		float2(0.185461,-0.893124),
		float2(0.507431,0.064425),
		float2(0.89642,0.412458),
		float2(-0.32194,-0.932615),
		float2(-0.791559,-0.59771)
	};

	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
	};
				
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _VelTex;
	sampler2D _NeighbourMaxTex;
	sampler2D _NoiseTex;
	sampler2D _TileTexDebug;
	
	float4 _MainTex_TexelSize;
	float4 _CameraDepthTexture_TexelSize;
	float4 _VelTex_TexelSize;
	
	float4x4 _InvViewProj;	// inverse view-projection matrix
	float4x4 _PrevViewProj;	// previous view-projection matrix
	float4x4 _ToPrevViewProjCombined; // combined

	float _Jitter;
	
	float _VelocityScale;
	float _DisplayVelocityScale;

	float _MaxVelocity;
	float _MinVelocity;
	
	float4 _BlurDirectionPacked;
	
	float _SoftZDistance;
	
	v2f vert(appdata_img v) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}
	
	float4 CameraVelocity(v2f i) : COLOR
	{
		float2 depth_uv = i.uv;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			depth_uv.y = 1 - depth_uv.y;	
		#endif

		// read depth
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, depth_uv));
		
		// calculate position from pixel from depth
		float3 clipPos = float3(i.uv.x*2.0-1.0, (i.uv.y)*2.0-1.0, d);

		// only 1 matrix mul:
		float4 prevClipPos = mul(_ToPrevViewProjCombined, float4(clipPos, 1.0));
		prevClipPos.xyz /= prevClipPos.w;

		/*
		float4 ws = mul(_InvViewProj, float4(clipPos, 1.0));
		ws /= ws.w;
		prevClipPos = mul(_PrevViewProj,ws);
		prevClipPos.xyz /= prevClipPos.w;
		*/

		/*
		float2 vel = _VelocityScale *(clipPos.xy - prevClipPos.xy) / 2.f;
		// clamp to maximum velocity (in pixels)
		float maxVel = length(_MainTex_TexelSize.xy*_MaxVelocity);
		if (length(vel) > maxVel) {
			vel = normalize(vel) * maxVel;
		}
		return float4(vel, 0.0, 0.0);
		*/

		float2 vel = _MainTex_TexelSize.zw * _VelocityScale * (clipPos.xy - prevClipPos.xy) / 2.f;
		float vellen = length(vel);
		float maxVel = _MaxVelocity;
		float2 velOut = vel * max(0.5, min(vellen, maxVel)) / (vellen + 1e-2f);
		velOut *= _MainTex_TexelSize.xy;
		return float4(velOut, 0.0, 0.0);
		
	}

	// vector with largest magnitude
	float2 vmax(float2 a, float2 b)
	{
		float ma = dot(a, a);
		float mb = dot(b, b);
		return (ma > mb) ? a : b;
	}

	// find dominant velocity for each tile
	float4 TileMax(v2f i) : COLOR
	{
		float2 uvCorner = i.uv - _MainTex_TexelSize.xy * (_MaxRadiusOrKInPaper * 0.5);
  	  	float2 maxvel = float2(0,0);
  	  	float4 baseUv = float4(uvCorner,0,0);
  	  	float4 uvScale = float4(_MainTex_TexelSize.xy, 0, 0);

  	  	for(int l=0; l<(int)_MaxRadiusOrKInPaper; l++)
  	  	{
  	  		for(int k=0; k<(int)_MaxRadiusOrKInPaper; k++)
  	  		{
				maxvel = vmax(maxvel, tex2Dlod(_MainTex, baseUv + float4(l,k,0,0) * uvScale).xy);  	  		
  	  		}
  	  	}
  	  	return float4(maxvel, 0, 1);
	}

	// find maximum velocity in any adjacent tile
	float4 NeighbourMax(v2f i) : COLOR
	{
		float2 x_ = i.uv;

		// to fetch all neighbours, we need 3x3 point filtered samples

		float2 nx =   tex2D(_MainTex, x_+float2(1.0, 1.0)*_MainTex_TexelSize.xy).xy;
		nx = vmax(nx, tex2D(_MainTex, x_+float2(1.0, 0.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(1.0,-1.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(0.0, 1.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(0.0, 0.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(0.0,-1.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(-1.0, 1.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(-1.0, 0.0)*_MainTex_TexelSize.xy).xy);
		nx = vmax(nx, tex2D(_MainTex, x_+float2(-1.0,-1.0)*_MainTex_TexelSize.xy).xy);

  	  	return float4(nx, 0, 0);		
	}
	 	 	
	float4 Debug(v2f i) : COLOR
	{
		return saturate( float4(tex2D(_MainTex, i.uv).x,abs(tex2D(_MainTex, i.uv).y),-tex2D(_MainTex, i.uv).xy) * _DisplayVelocityScale);
	}

	// classification filters
	float cone(float2 px, float2 py, float2 v)
	{
		return clamp(1.0 - (length(px - py) / length(v)), 0.0, 1.0);
	}

	float cylinder(float2 x, float2 y, float2 v)
	{
		float lv = length(v);
		return 1.0 - smoothstep(0.95*lv, 1.05*lv, length(x - y));
	}

	// is zb closer than za?
	float softDepthCompare(float za, float zb)
	{
		return clamp(1.0 - (za - zb) / _SoftZDistance, 0.0, 1.0);
	}

	float4 SimpleBlur (v2f i) : COLOR
	{
		float2 x = i.uv;
		float2 xf = x;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1 - xf.y;
		#endif

		float2 vx = tex2D(_VelTex, xf).xy;	// vel at x

		float4 sum = float4(0, 0, 0, 0);
		for(int l=0; l<NUM_SAMPLES; l++) {
			float t = l / (float) (NUM_SAMPLES - 1);
			t = t-0.5;
			float2 y = x - vx*t;
			float4 cy = tex2D(_MainTex, y);
			sum += cy;
		}
		sum /= NUM_SAMPLES;		
		return sum;
	}

	float4 ReconstructFilterBlur(v2f i) : COLOR
	{	
		// uv's

		float2 x = i.uv;
		float2 xf = x;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1-xf.y;
		#endif

		float2 x2 = xf;
		
		float2 vn = tex2Dlod(_NeighbourMaxTex, float4(x2,0,0)).xy;	// largest velocity in neighbourhood
		float4 cx = tex2Dlod(_MainTex, float4(x,0,0));				// color at x
		float2 vx = tex2Dlod(_VelTex, float4(xf,0,0)).xy;			// vel at x 

		float zx = UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(x,0,0)));
		zx = -Linear01Depth(zx);

		// random offset [-0.5, 0.5]
		float j = (tex2Dlod(_NoiseTex, float4(i.uv,0,0) * 11.0f).r*2-1) * _Jitter;

		// sample current pixel
		float weight = 0.75; // <= good start weight choice??
		float4 sum = cx * weight;
 
		int centerSample = (int)(NUM_SAMPLES-1)/2;
 
		for(int l=0; l<NUM_SAMPLES; l++) 
		{ 
			float contrib = 1.0f;
		#if SHADER_API_D3D11
			if (l==centerSample) continue;	// skip center sample
		#else
			if (l==centerSample) contrib = 0.0f;	// skip center sample
		#endif

			float t = lerp(-1.0, 1.0, (l + j) / (-1 + _Jitter + (float)NUM_SAMPLES));
			//float t = lerp(-1.0, 1.0, l / (float)(NUM_SAMPLES - 1)); 

			float2 y = x + vn * t;

			float2 yf = y;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
	    		yf.y = 1-yf.y;
			#endif

			// velocity at y 
			float2 vy = tex2Dlod(_VelTex, float4(yf,0,0)).xy;

			float zy = UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(y,0,0) )); 
			zy = -Linear01Depth(zy);
			float f = softDepthCompare(zx, zy);
			float b = softDepthCompare(zy, zx);
			float alphay = b * cone(x, y, vx) + f * cone(y, x, vy) +  cylinder(y, x, vy) * cylinder(x, y, vx) * 2.0;

			float4 cy = tex2Dlod(_MainTex, float4(y,0,0));
			sum += cy * alphay * contrib;
			weight += alphay * contrib;
		}
		sum /= weight;
		return sum;
	}

	float4 ReconstructionDiscBlur (v2f i) : COLOR
	{
		float2 xf = i.uv;
		float2 x = i.uv;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1 - xf.y;
		#endif

		float2 x2 = xf;

		float2 vn 			= tex2Dlod(_NeighbourMaxTex, float4(x2,0,0)).xy;	// largest velocity in neighbourhood
		float4 cx 			= tex2Dlod(_MainTex, float4(x,0,0));				// color at x
		float2 vx 			= tex2Dlod(_VelTex, float4(xf,0,0)).xy;			// vel at x 

		float4 noise 		= tex2Dlod(_NoiseTex, float4(i.uv,0,0)*11.0f)*2-1;
		float zx 			= UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(x,0,0)));

		zx = -Linear01Depth(zx);

		noise *= _MainTex_TexelSize.xyxy * _Jitter;

		//return abs(blurDir.xyxy)*10 + centerTap;

		float weight = 1.0; // <- maybe tweak this: bluriness amount ...
		float4 sum = cx * weight;
		
		float4 jitteredDir = vn.xyxy + noise.xyyz;
#ifdef SHADER_API_D3D11
		jitteredDir = max(abs(jitteredDir.xyxy), _MainTex_TexelSize.xyxy * _MaxVelocity * 0.5) * sign(jitteredDir.xyxy)  * float4(1,1,-1,-1);
#else
		jitteredDir = max(abs(jitteredDir.xyxy), _MainTex_TexelSize.xyxy * _MaxVelocity * 0.15) * sign(jitteredDir.xyxy)  * float4(1,1,-1,-1);
#endif

		for(int l=0; l<SmallDiscKernelSamples; l++)
		{
			float4 y = i.uv.xyxy + jitteredDir.xyxy * SmallDiscKernel[l].xyxy * float4(1,1,-1,-1);

			float4 yf = y;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
	    		yf.yw = 1-yf.yw;
			#endif

			// velocity at y 
			float2 vy = tex2Dlod(_VelTex, float4(yf.xy,0,0)).xy;

			float zy = UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(y.xy,0,0) )); 
			zy = -Linear01Depth(zy);

			float f = softDepthCompare(zx, zy);
			float b = softDepthCompare(zy, zx);
			float alphay = b * cone(x, y.xy, vx) + f * cone(y.xy, x, vy) +  cylinder(y.xy, x, vy) * cylinder(x, y.xy, vx) * 2.0;

			float4 cy = tex2Dlod(_MainTex, float4(y.xy,0,0));
			sum += cy * alphay;
			weight += alphay;

#ifdef SHADER_API_D3D11

			vy = tex2Dlod(_VelTex, float4(yf.zw,0,0)).xy;

			zy = UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(y.zw,0,0) )); 
			zy = -Linear01Depth(zy);

			f = softDepthCompare(zx, zy);
			b = softDepthCompare(zy, zx);
			alphay = b * cone(x, y.zw, vx) + f * cone(y.zw, x, vy) +  cylinder(y.zw, x, vy) * cylinder(x, y.zw, vx) * 2.0;

			cy = tex2Dlod(_MainTex, float4(y.zw,0,0));
			sum += cy * alphay;
			weight += alphay;
			
#endif
		}

		return sum / weight;
	}

	float4 MotionVectorBlur (v2f i) : COLOR
	{
		float2 x = i.uv;

		float2 insideVector = (x*2-1) * float2(1,_MainTex_TexelSize.w/_MainTex_TexelSize.z);
		float2 rollVector = float2(insideVector.y, -insideVector.x);

		float2 blurDir = _BlurDirectionPacked.x * float2(0,1);
		blurDir += _BlurDirectionPacked.y * float2(1,0);
		blurDir += _BlurDirectionPacked.z * rollVector;
		blurDir += _BlurDirectionPacked.w * insideVector;
		blurDir *= _VelocityScale;
 
		// clamp to maximum velocity (in pixels)
		float velMag = length(blurDir);
		if (velMag > _MaxVelocity) {
			blurDir *= (_MaxVelocity / velMag);
			velMag = _MaxVelocity;
		} 

		float4 centerTap = tex2D(_MainTex, x);
		float4 sum = centerTap;

		blurDir *= smoothstep(_MinVelocity * 0.25f, _MinVelocity * 2.5, velMag);

		blurDir *= _MainTex_TexelSize.xy;
		blurDir /= MOTION_SAMPLES;

		for(int i=0; i<MOTION_SAMPLES; i++) {
			float4 tap = tex2D(_MainTex, x+i*blurDir);
			sum += tap;
		}

		return sum/(1+MOTION_SAMPLES);
	}
		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 
	ENDCG
	
Subshader {
 
	// pass 0
	Pass {
		ZTest Always Cull Off ZWrite On Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment CameraVelocity
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x 

		ENDCG
	}

	// pass 1
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment Debug
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x 

		ENDCG
	}

	// pass 2
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment TileMax
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x       

		ENDCG
	}

	// pass 3
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment NeighbourMax
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x       

		ENDCG
	}

	// pass 4
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert 
		#pragma fragment ReconstructFilterBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x       

		ENDCG
	}

	// pass 5
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment SimpleBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x       
		ENDCG
	}

  	// pass 6
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment MotionVectorBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x
		ENDCG
	}

  	// pass 7
	Pass {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment ReconstructionDiscBlur
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl
		#pragma exclude_renderers d3d11_9x
		ENDCG
	}  	
  }

Fallback off
}