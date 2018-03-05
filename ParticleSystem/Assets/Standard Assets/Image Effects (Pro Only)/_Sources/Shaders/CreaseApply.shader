// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Hidden/CreaseApply" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_HrDepthTex ("Base (RGB)", 2D) = "white" {}
	_LrDepthTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _HrDepthTex;
sampler2D _LrDepthTex;

uniform float4 _MainTex_TexelSize;

uniform float intensity;

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv.xy = v.texcoord.xy;
	return o;
}

half4 frag (v2f i) : COLOR
{
	float4 hrDepth = tex2D(_HrDepthTex, i.uv);
	float4 lrDepth = tex2D(_LrDepthTex, i.uv);
	
	hrDepth.a = DecodeFloatRGBA(hrDepth);
	lrDepth.a = DecodeFloatRGBA(lrDepth);
	
	float4 color = tex2D(_MainTex, i.uv);
	
	return color * (1.0-abs(hrDepth.a-lrDepth.a)*intensity);
}

ENDCG


	}
}

Fallback off

}