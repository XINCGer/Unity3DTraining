// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ColorCorrection3DLut" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}		
	}

CGINCLUDE

#include "UnityCG.cginc"

struct v2f {
	float4 pos : POSITION;
	float2 uv  : TEXCOORD0;
};

sampler2D _MainTex;
sampler3D _ClutTex;

float _Scale;
float _Offset;

v2f vert( appdata_img v ) 
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv =  v.texcoord.xy;	
	return o;
} 

float4 frag(v2f i) : COLOR 
{
	float4 c = tex2D(_MainTex, i.uv);
	c.rgb = tex3D(_ClutTex, c.rgb * _Scale + _Offset).rgb;
	return c;
}

float4 fragLinear(v2f i) : COLOR 
{ 
	float4 c = tex2D(_MainTex, i.uv);
	c.rgb= sqrt(c.rgb);
	c.rgb = tex3D(_ClutTex, c.rgb * _Scale + _Offset).rgb;
	c.rgb = c.rgb*c.rgb; 
	return c;
}

ENDCG 

	
Subshader 
{
	Pass 
	{
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
	  #pragma target 3.0
      ENDCG
  	}

	Pass 
	{
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment fragLinear
	  #pragma target 3.0
      ENDCG
  	}
}

Fallback off
}
