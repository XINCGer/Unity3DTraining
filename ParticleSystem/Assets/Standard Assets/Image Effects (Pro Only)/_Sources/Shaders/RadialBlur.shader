// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/RadialBlur" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE
		
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};
		
	sampler2D _MainTex;
	
	float4 _BlurRadius4;
	float4 _SunPosition;

	float4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy =  v.texcoord.xy;
		
		o.blurVector = (_SunPosition.xy - v.texcoord.xy) * _BlurRadius4.xy;	
		
		return o; 
	}
	
	#define SAMPLES_FLOAT 6.0f
	#define SAMPLES_INT 6
	
	half4 frag(v2f i) : COLOR 
	{
		half4 color = half4(0,0,0,0);
				
		for(int j = 0; j < SAMPLES_INT; j++)   
		{	
			half4 tmpColor = tex2D(_MainTex, i.uv.xy);
			color += tmpColor;
			
			i.uv.xy += i.blurVector; 	
		}
		
		return color / SAMPLES_FLOAT;
	}

	ENDCG
	
Subshader 
{
 Blend One Zero
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag
      
      ENDCG
  } // Pass
} // Subshader

Fallback off

} // shader