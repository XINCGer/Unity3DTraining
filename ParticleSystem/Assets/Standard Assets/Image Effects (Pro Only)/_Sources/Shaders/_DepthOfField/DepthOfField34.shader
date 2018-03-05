// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Hidden/Dof/DepthOfField34" {
	Properties {
		_MainTex ("Base", 2D) = "" {}
		_TapLowBackground ("TapLowBackground", 2D) = "" {}
		_TapLowForeground ("TapLowForeground", 2D) = "" {}
		_TapMedium ("TapMedium", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv1 : TEXCOORD0;
	};
	
	struct v2fDofApply {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	struct v2fRadius {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv1[4] : TEXCOORD1;
	};
	
	struct v2fDown {
		half4 pos : POSITION;
		half2 uv0 : TEXCOORD0;
		half2 uv[2] : TEXCOORD1;
	};	 
			
	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _TapLowBackground;	
	sampler2D _TapLowForeground;
	sampler2D _TapMedium;
			
	half4 _CurveParams;
	half _ForegroundBlurExtrude;
	uniform half3 _Threshhold;	
	uniform float4 _MainTex_TexelSize;
	uniform float2 _InvRenderTargetSize;
	
	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv1.xy = v.texcoord.xy;
		return o;
	} 

	v2fRadius vertWithRadius( appdata_img v ) {
		v2fRadius o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;

		const half2 blurOffsets[4] = {
			half2(-0.5, +1.5),
			half2(+0.5, -1.5),
			half2(+1.5, +0.5),
			half2(-1.5, -0.5)
		}; 	
				
		o.uv1[0].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[0];
		o.uv1[1].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[1];
		o.uv1[2].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[2];
		o.uv1[3].xy = v.texcoord.xy + 5.0 * _MainTex_TexelSize.xy * blurOffsets[3];
		
		o.uv1[0].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[0];
		o.uv1[1].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[1];
		o.uv1[2].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[2];
		o.uv1[3].zw = v.texcoord.xy + 3.0 * _MainTex_TexelSize.xy * blurOffsets[3];
		
		return o;
	} 
	
	v2fDofApply vertDofApply( appdata_img v ) {
		v2fDofApply o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		return o;
	} 	
		
	v2fDown vertDownsampleWithCocConserve(appdata_img v) {
		v2fDown o;
		o.pos = UnityObjectToClipPos(v.vertex);	
		o.uv0.xy = v.texcoord.xy;
		o.uv[0].xy = v.texcoord.xy + half2(-1.0,-1.0) * _InvRenderTargetSize;
		o.uv[1].xy = v.texcoord.xy + half2(1.0,-1.0) * _InvRenderTargetSize;		
		return o; 
	} 
	
	half4 BokehPrereqs (sampler2D tex, half4 uv1[4], half4 center, half considerCoc) {		
		
		// @NOTE 1:
		// we are checking for 3 things in order to create a bokeh.
		// goal is to get the highest bang for the buck.
		// 1.) contrast/frequency should be very high (otherwise bokeh mostly unvisible)
		// 2.) luminance should be high
		// 3.) no occluder nearby (stored in alpha channel)
		
		// @NOTE 2: about the alpha channel in littleBlur:
		// the alpha channel stores an heuristic on how likely it is 
		// that there is no bokeh occluder nearby.
		// if we didn't' check for that, we'd get very noise bokeh
		// popping because of the sudden contrast changes

		half4 sampleA = tex2D(tex, uv1[0].zw);
		half4 sampleB = tex2D(tex, uv1[1].zw);
		half4 sampleC = tex2D(tex, uv1[2].zw);
		half4 sampleD = tex2D(tex, uv1[3].zw);
		
		half4 littleBlur = 0.125 * (sampleA + sampleB + sampleC + sampleD);
		
		sampleA = tex2D(tex, uv1[0].xy);
		sampleB = tex2D(tex, uv1[1].xy);
		sampleC = tex2D(tex, uv1[2].xy);
		sampleD = tex2D(tex, uv1[3].xy);		

		littleBlur += 0.125 * (sampleA + sampleB + sampleC + sampleD);
				
		littleBlur = lerp (littleBlur, center, saturate(100.0 * considerCoc * abs(littleBlur.a - center.a)));
				
		return littleBlur;
	}	
	
	half4 fragDownsampleWithCocConserve(v2fDown i) : COLOR {
		half2 rowOfs[4];   
		
  		rowOfs[0] = half2(0.0, 0.0);  
  		rowOfs[1] = half2(0.0, _InvRenderTargetSize.y);  
  		rowOfs[2] = half2(0.0, _InvRenderTargetSize.y) * 2.0;  
  		rowOfs[3] = half2(0.0, _InvRenderTargetSize.y) * 3.0; 
  		
  		half4 color = tex2D(_MainTex, i.uv0.xy); 	
			
		half4 sampleA = tex2D(_MainTex, i.uv[0].xy + rowOfs[0]);  
		half4 sampleB = tex2D(_MainTex, i.uv[1].xy + rowOfs[0]);  
		half4 sampleC = tex2D(_MainTex, i.uv[0].xy + rowOfs[2]);  
		half4 sampleD = tex2D(_MainTex, i.uv[1].xy + rowOfs[2]);  
		
		color += sampleA + sampleB + sampleC + sampleD;
		color *= 0.2;
		
		// @NOTE we are doing max on the alpha channel for 2 reasons:
		// 1) foreground blur likes a slightly bigger radius
		// 2) otherwise we get an ugly outline between high blur- and medium blur-areas
		// drawback: we get a little bit of color bleeding  		
		
		color.a = max(max(sampleA.a, sampleB.a), max(sampleC.a, sampleD.a));
  		
		return color;
	}
	
	half4 fragDofApplyBg (v2fDofApply i) : COLOR {		
		half4 tapHigh = tex2D (_MainTex, i.uv.xy);
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			i.uv.xy = i.uv.xy * half2(1,-1)+half2(0,1);
		#endif
		
		half4 tapLow = tex2D (_TapLowBackground, i.uv.xy); // already mixed with medium blur
		tapHigh = lerp (tapHigh, tapLow, tapHigh.a);
		return tapHigh; 
	}	
	
	half4 fragDofApplyBgDebug (v2fDofApply i) : COLOR {		
		half4 tapHigh = tex2D (_MainTex, i.uv.xy); 	
		
		half4 tapLow = tex2D (_TapLowBackground, i.uv.xy);
		
		half4 tapMedium = tex2D (_TapMedium, i.uv.xy);
		tapMedium.rgb = (tapMedium.rgb + half3 (1, 1, 0)) * 0.5;	
		tapLow.rgb = (tapLow.rgb + half3 (0, 1, 0)) * 0.5;
		
		tapLow = lerp (tapMedium, tapLow, saturate (tapLow.a * tapLow.a));		
		tapLow = tapLow * 0.5 + tex2D (_TapLowBackground, i.uv.xy) * 0.5;

		return lerp (tapHigh, tapLow, tapHigh.a);
	}		
	
	half4 fragDofApplyFg (v2fDofApply i) : COLOR {
		half4 fgBlur = tex2D(_TapLowForeground, i.uv.xy);	
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			i.uv.xy = i.uv.xy * half2(1,-1)+half2(0,1);
		#endif
				
		half4 fgColor = tex2D(_MainTex,i.uv.xy);
				
		//fgBlur.a = saturate(fgBlur.a*_ForegroundBlurWeight+saturate(fgColor.a-fgBlur.a));
		//fgBlur.a = max (fgColor.a, (2.0 * fgBlur.a - fgColor.a)) * _ForegroundBlurExtrude;
		fgBlur.a = max(fgColor.a, fgBlur.a * _ForegroundBlurExtrude); //max (fgColor.a, (2.0*fgBlur.a-fgColor.a)) * _ForegroundBlurExtrude;
		
		return lerp (fgColor, fgBlur, saturate(fgBlur.a));
	}	
	
	half4 fragDofApplyFgDebug (v2fDofApply i) : COLOR {
		half4 fgBlur = tex2D(_TapLowForeground, i.uv.xy);		
					
		half4 fgColor = tex2D(_MainTex,i.uv.xy);
		
		fgBlur.a = max(fgColor.a, fgBlur.a * _ForegroundBlurExtrude); //max (fgColor.a, (2.0*fgBlur.a-fgColor.a)) * _ForegroundBlurExtrude;
		
		half4 tapMedium = half4 (1, 1, 0, fgBlur.a);	
		tapMedium.rgb = 0.5 * (tapMedium.rgb + fgColor.rgb);
		
		fgBlur.rgb = 0.5 * (fgBlur.rgb + half3(0,1,0));
		fgBlur.rgb = lerp (tapMedium.rgb, fgBlur.rgb, saturate (fgBlur.a * fgBlur.a));
		
		return lerp ( fgColor, fgBlur, saturate(fgBlur.a));
	}	
		
	half4 fragCocBg (v2f i) : COLOR {
		
		float d = UNITY_SAMPLE_DEPTH ( tex2D (_CameraDepthTexture, i.uv1.xy) );
		d = Linear01Depth (d);
		half coc = 0.0; 
		
		half focalDistance01 = _CurveParams.w + _CurveParams.z;
		
		if (d > focalDistance01) 
			coc = (d - focalDistance01);
	
		coc = saturate (coc * _CurveParams.y);	
		return coc;
	} 
	
	half4 fragCocFg (v2f i) : COLOR {		
		half4 color = tex2D (_MainTex, i.uv1.xy);
		color.a = 0.0;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			i.uv1.xy = i.uv1.xy * half2(1,-1)+half2(0,1);
		#endif

		float d = UNITY_SAMPLE_DEPTH (tex2D (_CameraDepthTexture, i.uv1.xy) );
		d = Linear01Depth (d);	
		
		half focalDistance01 = (_CurveParams.w - _CurveParams.z);	
		
		if (d < focalDistance01) 
			color.a = (focalDistance01 - d);
		
		color.a = saturate (color.a * _CurveParams.x);	
		return color;	
	}	
	
	// not being used atm
	
	half4 fragMask (v2f i) : COLOR {
		return half4(0,0,0,0); 
	}	
	
	// used for simple one one blend
	
	half4 fragAddBokeh (v2f i) : COLOR {	
		half4 from = tex2D( _MainTex, i.uv1.xy );
		return from;
	}
	
	half4 fragAddFgBokeh (v2f i) : COLOR {		
		half4 from = tex2D( _MainTex, i.uv1.xy );
		return from; 
	}
		
	half4 fragDarkenForBokeh(v2fRadius i) : COLOR {		
		half4 fromOriginal = tex2D(_MainTex, i.uv.xy);
		half4 lowRez = BokehPrereqs (_MainTex, i.uv1, fromOriginal, _Threshhold.z);
		half4 outColor = half4(0,0,0, fromOriginal.a);
		half modulate = fromOriginal.a;		
		
		// this code imitates the if-then-else conditions below
		half2 conditionCheck = half2( dot(abs(fromOriginal.rgb-lowRez.rgb), half3(0.3,0.5,0.2)), Luminance(fromOriginal.rgb));
		conditionCheck *= fromOriginal.a;
		conditionCheck = saturate(_Threshhold.xy - conditionCheck);
		outColor = lerp (outColor, fromOriginal, saturate (dot(conditionCheck, half2(1000.0,1000.0))));
		
		/*
		if ( abs(dot(fromOriginal.rgb - lowRez.rgb,  half3 (0.3,0.5,0.2))) * modulate < _Threshhold.x)
			outColor = fromOriginal; // no darkening
		if (Luminance(fromOriginal.rgb) * modulate < _Threshhold.y)
			outColor = fromOriginal; // no darkening
		if (lowRez.a < _Threshhold.z) // need to make foreground not cast false bokeh's
			outColor = fromOriginal; // no darkenin
		*/	
		 
		return outColor;
	}
 
 	half4 fragExtractAndAddToBokeh (v2fRadius i) : COLOR {	
		half4 from = tex2D(_MainTex, i.uv.xy);
		half4 lowRez = BokehPrereqs(_MainTex, i.uv1, from, _Threshhold.z);
		half4 outColor = from;

		// this code imitates the if-then-else conditions below
		half2 conditionCheck = half2( dot(abs(from.rgb-lowRez.rgb), half3(0.3,0.5,0.2)), Luminance(from.rgb));
		conditionCheck *= from.a;
		conditionCheck = saturate(_Threshhold.xy - conditionCheck);
		outColor = lerp (outColor, half4(0,0,0,0), saturate (dot(conditionCheck, half2(1000.0,1000.0))));
		
		/*
		if ( abs(dot(from.rgb - lowRez.rgb,  half3 (0.3,0.5,0.2))) * modulate < _Threshhold.x)
			outColor = half4(0,0,0,0); // don't add
		if (Luminance(from.rgb) * modulate < _Threshhold.y)
			outColor = half4(0,0,0,0); // don't add
		if (lowRez.a < _Threshhold.z) // need to make foreground not cast false bokeh's
			outColor = half4(0,0,0,0); // don't add
		*/
							
		return outColor;
	}
 
	ENDCG
	
Subshader {
 
 // pass 0
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragDofApplyBg
      
      ENDCG
  	}

 // pass 1
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragDofApplyFgDebug

      ENDCG
  	}

 // pass 2

 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragDofApplyBgDebug

      ENDCG
  	}
  	
  	
 
 // pass 3
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCocBg

      ENDCG
  	}  
  	 	
	
 // pass 4

  
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  //Blend One One
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDofApply
      #pragma fragment fragDofApplyFg
      
      ENDCG
  	}  	

 // pass 5
  
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask ARGB
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCocFg

      ENDCG
  	} 

 // pass 6
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertDownsampleWithCocConserve
      #pragma fragment fragDownsampleWithCocConserve

      ENDCG
  	} 

 // pass 7
 // not being used atm
 
 Pass { 
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGBA
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragMask

      ENDCG
  	} 

 // pass 8
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Blend SrcAlpha OneMinusSrcAlpha
	  ColorMask RGB
  	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAddBokeh

      ENDCG
  	} 
  	
 // pass 9
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Blend One One
	  ColorMask RGB
	  Fog { Mode off }       

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertWithRadius
      #pragma fragment fragExtractAndAddToBokeh

      ENDCG
  	} 
  	
 // pass 10
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertWithRadius
      #pragma fragment fragDarkenForBokeh

      ENDCG
  	}   	
  	
 // pass 11
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma exclude_renderers flash
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertWithRadius
      #pragma fragment fragExtractAndAddToBokeh

      ENDCG
  	}   	
  }
  
Fallback off

}