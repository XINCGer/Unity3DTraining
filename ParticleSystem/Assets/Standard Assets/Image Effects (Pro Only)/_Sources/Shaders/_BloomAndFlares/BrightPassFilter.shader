// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BrightPassFilterForBloom"
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
	
	half4 threshhold;
	half useSrcAlphaAsMask;
		
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	} 
	
	half4 frag(v2f i) : COLOR 
	{
		half4 color = tex2D(_MainTex, i.uv);
		//color = color * saturate((color-threshhold.x) * 75.0); // didn't go well with HDR and din't make sense
		color = color * lerp(1.0, color.a, useSrcAlphaAsMask);
		color = max(half4(0,0,0,0), color-threshhold.x);
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
		      #pragma fragment frag
		
		      ENDCG
		}
	}
	Fallback off
}