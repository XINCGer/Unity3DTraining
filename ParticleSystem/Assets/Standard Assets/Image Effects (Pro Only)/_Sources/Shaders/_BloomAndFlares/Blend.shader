// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Blend" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_ColorBuffer ("Color", 2D) = "" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
	struct v2f_mt {
		float4 pos : POSITION;
		float2 uv[4] : TEXCOORD0;
	};
			
	sampler2D _ColorBuffer;
	sampler2D _MainTex;
	
	half _Intensity;
	half4 _ColorBuffer_TexelSize;
	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] =  v.texcoord.xy;
		o.uv[1] =  v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_ColorBuffer_TexelSize.y < 0) 
			o.uv[1].y = 1-o.uv[1].y;
		#endif	
		
		return o;
	}

	v2f_mt vertMultiTap( appdata_img v ) {
		v2f_mt o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] = v.texcoord.xy + _MainTex_TexelSize.xy * 0.5;
		o.uv[1] = v.texcoord.xy - _MainTex_TexelSize.xy * 0.5;	
		o.uv[2] = v.texcoord.xy - _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		o.uv[3] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		return o;
	}
	
	half4 fragScreen (v2f i) : COLOR {
		half4 toBlend = saturate (tex2D(_MainTex, i.uv[0]) * _Intensity);
		return 1-(1-toBlend)*(1-tex2D(_ColorBuffer, i.uv[1]));
	}

	half4 fragAdd (v2f i) : COLOR {
		return tex2D(_MainTex, i.uv[0].xy) * _Intensity + tex2D(_ColorBuffer, i.uv[1]);
	}

	half4 fragVignetteBlend (v2f i) : COLOR {
		return tex2D(_MainTex, i.uv[0].xy) * tex2D(_ColorBuffer, i.uv[0]);
	}
	
	half4 fragMultiTap (v2f_mt i) : COLOR {
		half4 outColor = tex2D(_MainTex, i.uv[0].xy);
		outColor += tex2D(_MainTex, i.uv[1].xy);
		outColor += tex2D(_MainTex, i.uv[2].xy);
		outColor += tex2D(_MainTex, i.uv[3].xy);
		return outColor * 0.25;
	}

	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  

 // 0: nicer & softer "screen" blend mode	  		  	
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragScreen
      ENDCG
  }

 // 1: simple "add" blend mode
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAdd
      ENDCG
  }
 // 2: used for "stable" downsampling
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertMultiTap
      #pragma fragment fragMultiTap
      ENDCG
  } 
 // 3: vignette blending
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragVignetteBlend
      ENDCG
  } 
}

Fallback off
	
} // shader