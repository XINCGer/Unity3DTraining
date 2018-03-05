// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 /*
 	NOTES: see CameraMotionBlur.shader
 */
 
 Shader "Hidden/CameraMotionBlurDX11" {
	Properties {
		_MainTex ("-", 2D) = "" {}
		_NoiseTex ("-", 2D) = "grey" {}
		_VelTex ("-", 2D) = "black" {}
		_NeighbourMaxTex ("-", 2D) = "black" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	// 'k' in paper
	float _MaxRadiusOrKInPaper;
	// 's' in paper
	#define NUM_SAMPLES (19)

	struct v2f {
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
	};
				
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _VelTex;
	sampler2D _NeighbourMaxTex;
	sampler2D _NoiseTex;
	
	float4 _MainTex_TexelSize;
	float4 _CameraDepthTexture_TexelSize;
	float4 _VelTex_TexelSize;
	
	float4x4 _InvViewProj;	// inverse view-projection matrix
	float4x4 _PrevViewProj;	// previous view-projection matrix
	float4x4 _ToPrevViewProjCombined; // combined
	
	float _Jitter;

	float _VelocityScale;
	float _DisplayVelocityScale;

	float _MinVelocity;
		
	float _SoftZDistance;
	
	v2f vert(appdata_img v) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	// returns vector with largest magnitude
	float2 vmax(float2 a, float2 b)
	{
		float ma = dot(a, a);
		float mb = dot(b, b);
		return (ma > mb) ? a : b;
	}

	// find dominant velocity in each tile
	float4 TileMax(v2f i) : COLOR
	{
		float2 tilemax = float2(0.0, 0.0);
		float2 srcPos = i.uv - _MainTex_TexelSize.xy * _MaxRadiusOrKInPaper * 0.5;

		for(int y=0; y<(int)_MaxRadiusOrKInPaper; y++) {
			for(int x=0; x<(int)_MaxRadiusOrKInPaper; x++) {
				float2 v = tex2D(_MainTex, srcPos + float2(x,y) * _MainTex_TexelSize.xy).xy;
				tilemax = vmax(tilemax, v);
		  	}
  	  	}
  	  	return float4(tilemax, 0, 1);
	}

	// find maximum velocity in any adjacent tile
	float4 NeighbourMax(v2f i) : COLOR
	{
		float2 maxvel = float2(0.0, 0.0);
		for(int y=-1; y<=1; y++) {
			for(int x=-1; x<=1; x++) {
				float2 v = tex2D(_MainTex, i.uv + float2(x,y) * _MainTex_TexelSize.xy).xy;
				maxvel = vmax(maxvel, v);
			}
		}
  	  	return float4(maxvel, 0, 1);		
	}	

	float cone(float2 px, float2 py, float2 v)
	{
		return clamp(1.0 - (length(px - py) / length(v)), 0.0, 1.0);
	}

	float cylinder(float2 x, float2 y, float2 v)
	{
		float lv = length(v);
		return 1.0 - smoothstep(0.95*lv, 1.05*lv, length(x - y));
	}

	float softDepthCompare(float za, float zb)
	{
		return clamp(1.0 - (za - zb) / _SoftZDistance, 0.0, 1.0);
	}

	float4 ReconstructFilterBlur(v2f i) : COLOR
	{	
		float2 x = i.uv;
		float2 xf = x;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
    		xf.y = 1-xf.y;
		#endif

		float2 x2 = xf;
		
		float2 vn = tex2D(_NeighbourMaxTex, x2).xy;	// largest velocity in neighbourhood
		float4 cx = tex2D(_MainTex, x);				// color at x

		float zx = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, x));
		zx = -Linear01Depth(zx);					// depth at x
		float2 vx = tex2D(_VelTex, xf).xy;			// vel at x 

		// random offset [-0.5, 0.5]
		float j = (tex2D(_NoiseTex, i.uv * 11.0f ).r*2-1) * _Jitter;

		// sample current pixel
		float weight = 1.0;
		float4 sum = cx * weight;
 
		int centerSample = (int)(NUM_SAMPLES-1) / 2;
 
		// in DX11 county we take more samples and interleave with sampling along vx direction to break up "patternized" look

		for(int l=0; l<NUM_SAMPLES; l++) 
		{
			if (l==centerSample) continue;	// skip center sample

			// Choose evenly placed filter taps along +-vN,
			// but jitter the whole filter to prevent ghosting			

			float t = lerp(-1.0, 1.0, (l + j) / (-1 + _Jitter + (float)NUM_SAMPLES));
			//float t = lerp(-1.0, 1.0, l / (float)(NUM_SAMPLES - 1));

			float2 velInterlaved = lerp(vn, min(vx, normalize(vx) * _MainTex_TexelSize.xy * _MaxRadiusOrKInPaper), l%2==0);
			float2 y = x + velInterlaved * t;			

			float2 yf = y;
			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
	    		yf.y = 1-yf.y;
			#endif

			// velocity at y 
			float2 vy = tex2Dlod(_VelTex, float4(yf,0,0)).xy;

			float zy = UNITY_SAMPLE_DEPTH(tex2Dlod(_CameraDepthTexture, float4(y,0,0))); 
			zy = -Linear01Depth(zy);
			float f = softDepthCompare(zx, zy);
			float b = softDepthCompare(zy, zx);
			float alphay = f * cone(y, x, vy) +	// blurry y in front of any x
			               b * cone(x, y, vx) +	// any y behing blurry x; estimate background
			               cylinder(y, x, vy) * cylinder(x, y, vx) * 2.0;	// simultaneous blurry x and y
			
			float4 cy = tex2Dlod(_MainTex, float4(y,0,0));
			sum += cy * alphay;
			weight += alphay;
		}
		sum /= weight;
		return sum;
	}

		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 
	ENDCG
	
Subshader {

	// pass 0
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment TileMax
		#pragma fragmentoption ARB_precision_hint_fastest

		ENDCG
	}

	// pass 1
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment NeighbourMax
		#pragma fragmentoption ARB_precision_hint_fastest

		ENDCG
	}

	// pass 2
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }      

		CGPROGRAM
		#pragma target 5.0
		#pragma vertex vert 
		#pragma fragment ReconstructFilterBlur
		#pragma fragmentoption ARB_precision_hint_fastest

		ENDCG
	}
  }
  
Fallback off
}