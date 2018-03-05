// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ColorCorrectionCurves" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}

		_RgbTex ("_RgbTex (RGB)", 2D) = "" {}
		
		_ZCurve ("_ZCurve (RGB)", 2D) = "" {}
		
		_RgbDepthTex ("_RgbDepthTex (RGB)", 2D) = "" {}
	}
	
	// note: also have a look at ColorCorrectionCurvesSimple
	//  for a much simpler color correction without use of 
	//  depth texture lookups
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv  : TEXCOORD0;
		float2 uv2 : TEXCOORD1;
	};
	 
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	
	float4 _CameraDepthTexture_ST;
	uniform float4 _MainTex_TexelSize;
	
	sampler2D _RgbTex;
	sampler2D _ZCurve; 
	sampler2D _RgbDepthTex;

	fixed _Saturation;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		o.uv2 = TRANSFORM_TEX(v.texcoord, _CameraDepthTexture);
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv2.y = 1-o.uv2.y;
		#endif		
		
		return o;
	} 
	
	half4 frag(v2f i) : COLOR 
	{
		half4 color = tex2D(_MainTex, i.uv); 

		half3 ycoords = half3(0.5,1.5,2.5) * 0.25;
		
		half3 red = tex2D(_RgbTex, half2(color.r, ycoords.x)).rgb * half3(1,0,0);
		half3 green = tex2D(_RgbTex, half2(color.g, ycoords.y)).rgb * half3(0,1,0);
		half3 blue = tex2D(_RgbTex, half2(color.b, ycoords.z)).rgb * half3(0,0,1);
		
		half theDepth = UNITY_SAMPLE_DEPTH( tex2D (_CameraDepthTexture, i.uv2) );
		half zval = tex2D(_ZCurve, half2( Linear01Depth (theDepth), 0.5));
		
		half3 depthRed = tex2D(_RgbDepthTex, half2(color.r, ycoords.x)).rgb * half3(1,0,0);
		half3 depthGreen = tex2D(_RgbDepthTex, half2(color.g, ycoords.y)).rgb * half3(0,1,0);
		half3 depthBlue = tex2D(_RgbDepthTex, half2(color.b, ycoords.z)).rgb * half3(0,0,1);
		
		color = half4( lerp(red+green+blue, depthRed+depthBlue+depthGreen, zval), color.a);

		half lum = Luminance(color.rgb);
		color.rgb = lerp(half3(lum,lum,lum), color.rgb, _Saturation);
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