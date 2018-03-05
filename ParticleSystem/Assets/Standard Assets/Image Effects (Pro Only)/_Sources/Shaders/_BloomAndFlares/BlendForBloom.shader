// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlendForBloom" {
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
		float2 uv[5] : TEXCOORD0;
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
		o.uv[4] = v.texcoord.xy;
		o.uv[0] = v.texcoord.xy + _MainTex_TexelSize.xy * 0.5;
		o.uv[1] = v.texcoord.xy - _MainTex_TexelSize.xy * 0.5;	
		o.uv[2] = v.texcoord.xy - _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		o.uv[3] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		return o;
	}
	
	half4 fragScreen (v2f i) : COLOR {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy) * _Intensity;
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return 1-(1-addedbloom)*(1-screencolor);
	}

	half4 fragScreenCheap(v2f i) : COLOR {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy) * _Intensity;
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return 1-(1-addedbloom)*(1-screencolor);
	}

	half4 fragAdd (v2f i) : COLOR {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy);
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return _Intensity * addedbloom + screencolor;
	}

	half4 fragAddCheap (v2f i) : COLOR {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy);
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return _Intensity * addedbloom + screencolor;
	}

	half4 fragVignetteMul (v2f i) : COLOR {
		return tex2D(_MainTex, i.uv[0].xy) * tex2D(_ColorBuffer, i.uv[0]);
	}

	half4 fragVignetteBlend (v2f i) : COLOR {
		return half4(1,1,1, tex2D(_ColorBuffer, i.uv[0]).r);
	}

	half4 fragClear (v2f i) : COLOR {
		return 0;
	}

	half4 fragAddOneOne (v2f i) : COLOR {
		half4 addedColors = tex2D(_MainTex, i.uv[0].xy);
		return addedColors * _Intensity;
	}

	half4 frag1Tap (v2f i) : COLOR {
		return tex2D(_MainTex, i.uv[0].xy);
	}
	
	half4 fragMultiTapMax (v2f_mt i) : COLOR {
		half4 outColor = tex2D(_MainTex, i.uv[4].xy);
		outColor = max(outColor, tex2D(_MainTex, i.uv[0].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[1].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[2].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[3].xy));
		return outColor;
	}

	half4 fragMultiTapBlur (v2f_mt i) : COLOR {
		half4 outColor = 0;
		outColor += tex2D(_MainTex, i.uv[0].xy);
		outColor += tex2D(_MainTex, i.uv[1].xy);
		outColor += tex2D(_MainTex, i.uv[2].xy);
		outColor += tex2D(_MainTex, i.uv[3].xy);
		return outColor/4;
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

 // 1: "add" blend mode 
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAdd
      ENDCG
  }
 // 2: several taps, maxxed
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertMultiTap
      #pragma fragment fragMultiTapMax
      ENDCG
  } 
 // 3: vignette blending
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragVignetteMul
      ENDCG
  } 
  // 4: nicer & softer "screen" blend mode(cheapest)
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragScreenCheap
      ENDCG
 }  
  // 5: "add" blend mode (cheapest)
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAddCheap
      ENDCG
  }     
 // 6: used for "stable" downsampling (blur)
 Pass {    

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertMultiTap
      #pragma fragment fragMultiTapBlur
      ENDCG
  }   
 // 7: vignette blending (blend to dest)
 Pass {    
 	  
 	  Blend Zero SrcAlpha

      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragVignetteBlend
      ENDCG
  }
 // 8: clear
 Pass {    
 	  
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragClear
      ENDCG
  }   
 // 9: fragAddOneOne
 Pass {    

 	  Blend One One
 	  
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAddOneOne
      ENDCG
  }  
 // 10: max blend
 Pass {    

 	  BlendOp Max
 	  Blend One One
 	  
      CGPROGRAM
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag1Tap
      ENDCG
  }
}

Fallback off
	
} // shader