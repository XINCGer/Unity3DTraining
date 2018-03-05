// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ContrastComposite" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_MainTexBlurred ("Base Blurred (RGB)", 2D) = "" {}
	}
	
	// Shader code pasted into all further CGPROGRAM blocks	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _MainTexBlurred;
	
	float4 _MainTex_TexelSize;
	
	float intensity;
	float threshhold;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv[0].y = 1-o.uv[0].y;
		#endif			
		return o;
	}
	
	half4 frag(v2f i) : COLOR 
	{
		half4 color = tex2D (_MainTex, i.uv[1]);
		half4 blurred = tex2D (_MainTexBlurred, (i.uv[0]));
		
		half4 difff = color - blurred;
		half4 signs = sign (difff);
		
		difff = saturate ( (color-blurred) - threshhold) * signs * 1.0/(1.0-threshhold);
		color += difff * intensity;
		
		return color;
	}

	ENDCG
	
Subshader {
 Pass {
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
	
} // shader