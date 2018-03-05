// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BlurAndFlares" {
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

	struct v2f_blur {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4;
	};
	
	half4 _Offsets;
	half4 _TintColor;
	
	half _StretchWidth;
	half2 _Threshhold;
	half _Saturation;
	
	half4 _MainTex_TexelSize;
	
	sampler2D _MainTex;
	sampler2D _NonBlurredTex;
		
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	}

	v2f_blur vertWithMultiCoords2 (appdata_img v) {
		v2f_blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1) * 2.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1) * 3.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1) * 4.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * half4(1,1, -1,-1) * 5.0;
		return o;  
	}

	v2f_opts vertStretch (appdata_img v) {
		v2f_opts o;
		o.pos = UnityObjectToClipPos(v.vertex);
		half b = _StretchWidth;		
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy + b * 2.0 * _Offsets.xy;
		o.uv[2] = v.texcoord.xy - b * 2.0 * _Offsets.xy;
		o.uv[3] = v.texcoord.xy + b * 4.0 * _Offsets.xy;
		o.uv[4] = v.texcoord.xy - b * 4.0 * _Offsets.xy;
		o.uv[5] = v.texcoord.xy + b * 6.0 * _Offsets.xy;
		o.uv[6] = v.texcoord.xy - b * 6.0 * _Offsets.xy;
		return o;
	}
	
	v2f_opts vertWithMultiCoords (appdata_img v) {
		v2f_opts o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] = v.texcoord.xy;
		o.uv[1] = v.texcoord.xy + 0.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		o.uv[2] = v.texcoord.xy - 0.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		o.uv[3] = v.texcoord.xy + 1.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		o.uv[4] = v.texcoord.xy - 1.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		o.uv[5] = v.texcoord.xy + 2.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		o.uv[6] = v.texcoord.xy - 2.5 * _MainTex_TexelSize.xy * _Offsets.xy;
		return o;
	}	

	half4 fragPostNoBlur (v2f i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv);
		return color * 1.0/(1.0 + Luminance(color.rgb) + 0.5); // this also makes it a little noisy
	}

	half4 fragGaussBlur (v2f_blur i) : COLOR {
		half4 color = half4 (0,0,0,0);
		color += 0.225 * tex2D (_MainTex, i.uv);
		color += 0.150 * tex2D (_MainTex, i.uv01.xy);
		color += 0.150 * tex2D (_MainTex, i.uv01.zw);
		color += 0.110 * tex2D (_MainTex, i.uv23.xy);
		color += 0.110 * tex2D (_MainTex, i.uv23.zw);
		color += 0.075 * tex2D (_MainTex, i.uv45.xy);
		color += 0.075 * tex2D (_MainTex, i.uv45.zw);	
		color += 0.0525 * tex2D (_MainTex, i.uv67.xy);
		color += 0.0525 * tex2D (_MainTex, i.uv67.zw);
		return color;
	} 

	half4 fragPreAndCut (v2f_opts i) : COLOR {
		half4 color = tex2D (_MainTex, i.uv[0]);
		color += tex2D (_MainTex, i.uv[1]);
		color += tex2D (_MainTex, i.uv[2]);
		color += tex2D (_MainTex, i.uv[3]);
		color += tex2D (_MainTex, i.uv[4]);
		color += tex2D (_MainTex, i.uv[5]);
		color += tex2D (_MainTex, i.uv[6]);
		color = max(color / 7.0 - _Threshhold.xxxx, float4(0,0,0,0));
		half lum = Luminance(color.rgb);
		color.rgb = lerp(half3(lum,lum,lum), color.rgb, _Saturation) * _TintColor.rgb;
		return color;
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
      #pragma exclude_renderers flash
      #pragma vertex vert
      #pragma fragment fragPostNoBlur
      
      ENDCG
  }

 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers flash
      #pragma vertex vertStretch
      #pragma fragment fragStretch
      
      ENDCG
  }

 // 2
 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers flash
      #pragma vertex vertWithMultiCoords
      #pragma fragment fragPreAndCut
      
      ENDCG
  } 

 // 3
 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers flash
      #pragma vertex vertWithMultiCoords
      #pragma fragment fragPost
      
      ENDCG
  }
 // 4
 Pass {     

      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers flash
      #pragma vertex vertWithMultiCoords2
      #pragma fragment fragGaussBlur
      
      ENDCG
  } 
}
	
Fallback off
	
}
