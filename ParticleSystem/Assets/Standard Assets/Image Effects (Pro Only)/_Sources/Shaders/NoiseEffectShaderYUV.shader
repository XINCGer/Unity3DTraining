// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Noise Shader YUV" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_GrainTex ("Base (RGB)", 2D) = "gray" {}
	_ScratchTex ("Base (RGB)", 2D) = "gray" {}
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

struct v2f { 
	float4 pos	: POSITION;
	float2 uv	: TEXCOORD0;
	float2 uvg	: TEXCOORD1; // grain
	float2 uvs	: TEXCOORD2; // scratch
}; 

uniform sampler2D _MainTex;
uniform sampler2D _GrainTex;
uniform sampler2D _ScratchTex;

uniform float4 _GrainOffsetScale;
uniform float4 _ScratchOffsetScale;
uniform fixed4 _Intensity; // x=grain, y=scratch

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
	o.uvg = v.texcoord.xy * _GrainOffsetScale.zw + _GrainOffsetScale.xy;
	o.uvs = v.texcoord.xy * _ScratchOffsetScale.zw + _ScratchOffsetScale.xy;
	return o;
}

fixed4 frag (v2f i) : COLOR
{
	fixed4 col = tex2D(_MainTex, i.uv);
	
	// convert to YUV
	fixed3 yuv;
	yuv.x = dot( col.rgb, half3(0.299,0.587,0.114) );
	yuv.y = (col.b-yuv.x)*0.492;
	yuv.z = (col.r-yuv.x)*0.877;
	
	// sample noise texture and do a signed add
	fixed3 grain = tex2D(_GrainTex, i.uvg).rgb * 2 - 1;
	yuv.rgb += grain * _Intensity.x;

	// convert back to rgb
	col.r = yuv.z * 1.140 + yuv.x;
	col.g = yuv.z * (-0.581) + yuv.y * (-0.395) + yuv.x;
	col.b = yuv.y * 2.032 + yuv.x;

	// sample scratch texture and add
	fixed3 scratch = tex2D(_ScratchTex, i.uvs).rgb * 2 - 1;
	col.rgb += scratch * _Intensity.y;

	return col;
}
ENDCG
	}
}

Fallback off

}