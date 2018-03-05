// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MultipassHollywoodFlares" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_NonBlurredTex ("Base (RGB)", 2D) = "" {}
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	struct v2f_opts {
		half4 pos : POSITION;
		half2 uv[7] : TEXCOORD0;
	};
	
	half4 offsets;
	half4 tintColor;
	
	half stretchWidth;
	half2 _Threshhold;
	
	half4 _MainTex_TexelSize;
	
	sampler2D _MainTex;
	sampler2D _NonBlurredTex;
		
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	}

	v2f_opts vertStretch (appdata_img v) {
		v2f_opts o;
		o.pos = UnityObjectToClipPos(v.vertex);
		half b = stretchWidth;		
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy + b * 2.0 * offsets.xy;
		o.uv[2] = v.texcoord.xy - b * 2.0 * offsets.xy;
		o.uv[3] = v.texcoord.xy + b * 4.0 * offsets.xy;
		o.uv[4] = v.texcoord.xy - b * 4.0 * offsets.xy;
		o.uv[5] = v.texcoord.xy + b * 6.0 * offsets.xy;
		o.uv[6] = v.texcoord.xy - b * 6.0 * offsets.xy;
		return o;
	}
	
	v2f_opts vertVerticalCoords (appdata_img v) {
		v2f_opts o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy + 0.5 * _MainTex_TexelSize.xy * half2(0,1);
		o.uv[2] = v.texcoord.xy - 0.5 * _MainTex_TexelSize.xy * half2(0,1);
		o.uv[3] = v.texcoord.xy + 1.5 * _MainTex_TexelSize.xy * half2(0,1);
		o.uv[4] = v.texcoord.xy - 1.5 * _MainTex_TexelSize.xy * half2(0,1);
		o.uv[5] = v.texcoord.xy + 2.5 * _MainTex_TexelSize.xy * half2(0,1);
		o.uv[6] = v.texcoord.xy - 2.5 * _MainTex_TexelSize.xy * half2(0,1);
		return o;
	}	
		
	// deprecated
	half4 fragPrepare (v2f i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv);
		half4 colorNb = tex2D (_NonBlurredTex, i.uv);
		return color * tintColor * 0.5 + colorNb * normalize (tintColor) * 0.5;
	}


	half4 fragPreAndCut (v2f_opts i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv[0]);
		color += tex2D (_MainTex, i.uv[1]);
		color += tex2D (_MainTex, i.uv[2]);
		color += tex2D (_MainTex, i.uv[3]);
		color += tex2D (_MainTex, i.uv[4]);
		color += tex2D (_MainTex, i.uv[5]);
		color += tex2D (_MainTex, i.uv[6]);
		return max(color / 7.0 - _Threshhold.x, 0.0) * _Threshhold.y * tintColor;
	}

	half4 fragStretch (v2f_opts i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv[0]);
		color = max (color, tex2D (_MainTex, i.uv[1]));
		color = max (color, tex2D (_MainTex, i.uv[2]));
		color = max (color, tex2D (_MainTex, i.uv[3]));
		color = max (color, tex2D (_MainTex, i.uv[4]));
		color = max (color, tex2D (_MainTex, i.uv[5]));
		color = max (color, tex2D (_MainTex, i.uv[6]));
		return color;
	}	
	
	half4 fragPost (v2f_opts i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv[0]);
		color += tex2D (_MainTex, i.uv[1]);
		color += tex2D (_MainTex, i.uv[2]);
		color += tex2D (_MainTex, i.uv[3]);
		color += tex2D (_MainTex, i.uv[4]);
		color += tex2D (_MainTex, i.uv[5]);
		color += tex2D (_MainTex, i.uv[6]);
		return color * 1.0/(7.0 + Luminance(color.rgb) + 0.5); // this also makes it a little noisy
	}

	ENDCG
	
Subshader {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off } 
 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragPrepare
      
      ENDCG
  }

 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertStretch
      #pragma fragment fragStretch
      
      ENDCG
  }

 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertVerticalCoords
      #pragma fragment fragPreAndCut
      
      ENDCG
  } 

 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertVerticalCoords
      #pragma fragment fragPost
      
      ENDCG
  } 
}
	
Fallback off
	
}