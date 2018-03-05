// Outputs luminance (grayscale) of the input image _MainTex

Shader "Hidden/Contrast Stretch Luminance" {
	
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

Category {
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;

float4 frag (v2f_img i) : COLOR
{
	float4 col = tex2D(_MainTex, i.uv);
	col.rgb = Luminance(col.rgb) * (1+col.a*2);
	return col;
}
ENDCG

		}
	}
}

Fallback off

}