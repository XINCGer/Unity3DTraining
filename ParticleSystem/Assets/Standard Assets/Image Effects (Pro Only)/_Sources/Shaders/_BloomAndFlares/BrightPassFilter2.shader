// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BrightPassFilter2"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;	
	
	half4 _Threshhold;
		
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 
	
	half4 fragScalarThresh(v2f i) : COLOR 
	{
		half4 color = tex2D(_MainTex, i.uv);
		color.rgb = color.rgb;
		color.rgb = max(half3(0,0,0), color.rgb-_Threshhold.xxx);
		return color;
	}

	half4 fragColorThresh(v2f i) : COLOR 
	{
		half4 color = tex2D(_MainTex, i.uv);
		color.rgb = max(half3(0,0,0), color.rgb-_Threshhold.rgb);
		return color;
	}	

	ENDCG 
	
	Subshader 
	{
		Pass 
 		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragScalarThresh

			ENDCG
		}

		Pass 
 		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }      

			CGPROGRAM

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment fragColorThresh

			ENDCG
		}		
	}
	Fallback off
}