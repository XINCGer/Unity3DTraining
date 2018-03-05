// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ColorCorrectionCurvesSimple" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_RgbTex ("_RgbTex (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _RgbTex;
	fixed _Saturation;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	fixed4 frag(v2f i) : COLOR 
	{
		fixed4 color = tex2D(_MainTex, i.uv); 
		
		fixed3 red = tex2D(_RgbTex, half2(color.r, 0.5/4.0)).rgb * fixed3(1,0,0);
		fixed3 green = tex2D(_RgbTex, half2(color.g, 1.5/4.0)).rgb * fixed3(0,1,0);
		fixed3 blue = tex2D(_RgbTex, half2(color.b, 2.5/4.0)).rgb * fixed3(0,0,1);
		
		color = fixed4(red+green+blue, color.a);

		fixed lum = Luminance(color.rgb);
		color.rgb = lerp(fixed3(lum,lum,lum), color.rgb, _Saturation);
		return color;		
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off
	
} // shader