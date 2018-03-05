Shader "Hidden/Sepiatone Effect" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

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

fixed4 frag (v2f_img i) : COLOR
{	
	fixed4 original = tex2D(_MainTex, i.uv);
	
	// get intensity value (Y part of YIQ color space)
	fixed Y = dot (fixed3(0.299, 0.587, 0.114), original.rgb);

	// Convert to Sepia Tone by adding constant
	fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);
	fixed4 output = sepiaConvert + Y;
	output.a = original.a;
	
	return output;
}
ENDCG

	}
}

Fallback off

}