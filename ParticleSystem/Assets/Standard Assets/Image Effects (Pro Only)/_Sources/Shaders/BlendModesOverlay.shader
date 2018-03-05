// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlendModesOverlay" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_Overlay ("Color", 2D) = "grey" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv[2] : TEXCOORD0;
	};
			
	sampler2D _Overlay;
	sampler2D _MainTex;
	
	half _Intensity;
	half4 _MainTex_TexelSize;
		
	v2f vert( appdata_img v ) { 
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] =  v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0.0)
			o.uv[0].y = 1.0-o.uv[0].y;
		#endif
		
		o.uv[1] =  v.texcoord.xy;	
		return o;
	}
	
	half4 fragAddSub (v2f i) : COLOR {
		half4 toAdd = tex2D(_Overlay, i.uv[0]) * _Intensity;
		return tex2D(_MainTex, i.uv[1]) + toAdd;
	}

	half4 fragMultiply (v2f i) : COLOR {
		half4 toBlend = tex2D(_Overlay, i.uv[0]) * _Intensity;
		return tex2D(_MainTex, i.uv[1]) * toBlend;
	}	
			
	half4 fragScreen (v2f i) : COLOR {
		half4 toBlend =  (tex2D(_Overlay, i.uv[0]) * _Intensity);
		return 1-(1-toBlend)*(1-(tex2D(_MainTex, i.uv[1])));
	}

	half4 fragOverlay (v2f i) : COLOR {
		half4 m = (tex2D(_Overlay, i.uv[0]));// * 255.0;
		half4 color = (tex2D(_MainTex, i.uv[1]));//* 255.0;

		// overlay blend mode
		//color.rgb = (color.rgb/255.0) * (color.rgb + ((2*m.rgb)/( 255.0 )) * (255.0-color.rgb));
		//color.rgb /= 255.0; 
		 
		/*
if (Target > ½) R = 1 - (1-2x(Target-½)) x (1-Blend)
if (Target <= ½) R = (2xTarget) x Blend		
		*/
		
		float3 check = step(0.5, color.rgb);
		float3 result = 0;
		
			result =  check * (half3(1,1,1) - ( (half3(1,1,1) - 2*(color.rgb-0.5)) *  (1-m.rgb))); 
			result += (1-check) * (2*color.rgb) * m.rgb;
		
		return half4(lerp(color.rgb, result.rgb, (_Intensity)), color.a);
	}
	
	half4 fragAlphaBlend (v2f i) : COLOR {
		half4 toAdd = tex2D(_Overlay, i.uv[0]) ;
		return lerp(tex2D(_MainTex, i.uv[1]), toAdd, toAdd.a);
	}	


	ENDCG 
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  
      ColorMask RGB	  
  		  	
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAddSub
      ENDCG
  }

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragScreen
      ENDCG
  }

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragMultiply
      ENDCG
  }  

 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragOverlay
      ENDCG
  }  
  
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAlphaBlend
      ENDCG
  }   
}

Fallback off
	
} // shader