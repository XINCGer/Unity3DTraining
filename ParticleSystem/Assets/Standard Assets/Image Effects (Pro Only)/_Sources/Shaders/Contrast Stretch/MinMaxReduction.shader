// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Reduces input image (_MainTex) by 2x2.
// Outputs maximum value in R, minimum in G.
Shader "Hidden/Contrast Stretch Reduction" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

Category {
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
	float4 position : POSITION;  
	float2 uv[4]    : TEXCOORD0;
}; 

uniform sampler2D _MainTex;

v2f vert (appdata_img v) {
	v2f o;
	o.position = UnityObjectToClipPos (v.vertex);
	float2 uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
	
	// Compute UVs to sample 2x2 pixel block.
	o.uv[0] = uv + float2(0,0);
	o.uv[1] = uv + float2(0,1);
	o.uv[2] = uv + float2(1,0);
	o.uv[3] = uv + float2(1,1);
	return o;
}

float4 frag (v2f i) : COLOR
{
	// Sample pixel block
	float4 v00 = tex2D(_MainTex, i.uv[0]);
	float2 v01 = tex2D(_MainTex, i.uv[1]).xy;
	float2 v10 = tex2D(_MainTex, i.uv[2]).xy;
	float2 v11 = tex2D(_MainTex, i.uv[3]).xy;
	
	float4 res;
	// output x: maximum of the four values
	res.x = max( max(v00.x,v01.x), max(v10.x,v11.x) );
	// output y: minimum of the four values
	res.y = min( min(v00.y,v01.y), min(v10.y,v11.y) );
	// output zw unchanged from the first pixel
	res.zw = v00.zw;
	
	return res;
}
ENDCG

		}
	}
}

Fallback off

}