// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GlobalFog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
}

CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _CameraDepthTexture;
	
	uniform float _GlobalDensity;
	uniform float4 _FogColor;
	uniform float4 _StartDistance;
	uniform float4 _Y;
	uniform float4 _MainTex_TexelSize;
	
	// for fast world space reconstruction
	
	uniform float4x4 _FrustumCornersWS;
	uniform float4 _CameraWS;
	 
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
		float4 interpolatedRay : TEXCOORD2;
	};
	
	v2f vert( appdata_img v )
	{
		v2f o;
		half index = v.vertex.z;
		v.vertex.z = 0.1;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif				
		
		o.interpolatedRay = _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;
		
		return o;
	}
	
	float ComputeFogForYAndDistance (in float3 camDir, in float3 wsPos) 
	{
		float fogInt = saturate(length(camDir) * _StartDistance.x-1.0) * _StartDistance.y;	
		float fogVert = max(0.0, (wsPos.y-_Y.x) * _Y.y);
		fogVert *= fogVert; 
		return  (1-exp(-_GlobalDensity*fogInt)) * exp (-fogVert);
	}
	
	half4 fragAbsoluteYAndDistance (v2f i) : COLOR
	{
		float dpth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv_depth)));
		float4 wsDir = dpth * i.interpolatedRay;
		float4 wsPos = _CameraWS + wsDir;
		return lerp(tex2D(_MainTex, i.uv), _FogColor, ComputeFogForYAndDistance(wsDir.xyz,wsPos.xyz));
	}

	half4 fragRelativeYAndDistance (v2f i) : COLOR
	{
		float dpth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv_depth)));
		float4 wsDir = dpth * i.interpolatedRay;
		return lerp(tex2D(_MainTex, i.uv), _FogColor, ComputeFogForYAndDistance(wsDir.xyz, wsDir.xyz));
	}

	half4 fragAbsoluteY (v2f i) : COLOR
	{
		float dpth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv_depth)));
		float4 wsPos = (_CameraWS + dpth * i.interpolatedRay);
		float fogVert = max(0.0, (wsPos.y-_Y.x) * _Y.y);
		fogVert *= fogVert; 
		fogVert = (exp (-fogVert));
		return lerp(tex2D( _MainTex, i.uv ), _FogColor, fogVert);				
	}

	half4 fragDistance (v2f i) : COLOR
	{
		float dpth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,i.uv_depth)));		
		float4 camDir = ( /*_CameraWS  + */ dpth * i.interpolatedRay);
		float fogInt = saturate(length( camDir ) * _StartDistance.x - 1.0) * _StartDistance.y;	
		return lerp(_FogColor, tex2D(_MainTex, i.uv), exp(-_GlobalDensity*fogInt));				
	}

ENDCG

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment fragAbsoluteYAndDistance
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma exclude_renderers flash
		
		ENDCG
	}

	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment fragAbsoluteY
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma exclude_renderers flash
		
		ENDCG
	}

	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment fragDistance
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma exclude_renderers flash
		
		ENDCG
	}

	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment fragRelativeYAndDistance
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma exclude_renderers flash
		
		ENDCG
	}
}

Fallback off

}