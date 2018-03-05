// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Twirt Effect Shader" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform float4 _CenterRadius;
uniform float4x4 _RotationMatrix;

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

v2f vert( appdata_img v )
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = v.texcoord - _CenterRadius.xy;
	return o;
}

float4 frag (v2f i) : COLOR
{
	float2 offset = i.uv;
	float2 distortedOffset = MultiplyUV (_RotationMatrix, offset.xy);
	float2 tmp = offset / _CenterRadius.zw;
	float t = min (1, length(tmp));
	
	offset = lerp (distortedOffset, offset, t);
	offset += _CenterRadius.xy;
	
	return tex2D(_MainTex, offset);
}
ENDCG

	}
}

Fallback off

}
