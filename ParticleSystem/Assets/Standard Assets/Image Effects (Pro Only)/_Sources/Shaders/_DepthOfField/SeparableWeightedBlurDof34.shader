// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SeparableWeightedBlurDof34" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_TapMedium ("TapMedium (RGB)", 2D) = "" {}
		_TapLow ("TapLow (RGB)", 2D) = "" {}
		_TapHigh ("TapHigh (RGB)", 2D) = "" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"	
		
	half4 offsets;
	half4 _Threshhold;
	sampler2D _MainTex;		
	sampler2D _TapHigh;
		
	struct v2f {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
	};
	
	struct v2fSingle {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};
	
	//
	// VERT PROGRAMS
	//
	
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 2.0;
		o.uv45 =  v.texcoord.xyxy + offsets.xyxy * half4(1,1, -1,-1) * 3.0;

		return o;  
	}
	
	v2fSingle vertSingleTex (appdata_img v) {
		v2fSingle o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		return o;  
	}	
	
	//
	// FRAG PROGRAMS
	//
		
	// mostly used for foreground, so more gaussian-like
			
	half4 fragBlurUnweighted (v2f i) : COLOR {
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
				
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		
		blurredColor *= 0.2;
		
		blurredColor.a = max(UNITY_SAMPLE_1CHANNEL(_TapHigh, i.uv.xy), blurredColor.a);

		return blurredColor;
	}

	// used for background, so more bone curve-like
		
	half4 fragBlurWeighted (v2f i) : COLOR {
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
 			
		half sum = sampleA.a + dot (half4 (1.25, 1.25, 1.5, 1.5), half4 (sampleB.a,sampleC.a,sampleD.a,sampleE.a));
	
		sampleA.rgb = sampleA.rgb * sampleA.a; 
		sampleB.rgb = sampleB.rgb * sampleB.a * 1.25;
		sampleC.rgb = sampleC.rgb * sampleC.a * 1.25; 
		sampleD.rgb = sampleD.rgb * sampleD.a * 1.5; 
		sampleE.rgb = sampleE.rgb * sampleE.a * 1.5; 
				
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		
		blurredColor /= sum;
		half4 color = blurredColor;	
		
		color.a = sampleA.a;
		
		return color;
	}
	
	half4 fragBlurDark (v2f i) : COLOR {
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
		 			
		half sum = sampleA.a + dot (half4 (0.75, 0.75, 0.5, 0.5), half4 (sampleB.a,sampleC.a,sampleD.a,sampleE.a));
	
		sampleA.rgb = sampleA.rgb * sampleA.a; 
		sampleB.rgb = sampleB.rgb * sampleB.a * 0.75;
		sampleC.rgb = sampleC.rgb * sampleC.a * 0.75; 
		sampleD.rgb = sampleD.rgb * sampleD.a * 0.5; 
		sampleE.rgb = sampleE.rgb * sampleE.a * 0.5; 
				
		blurredColor += sampleA;
		blurredColor += sampleB;
		blurredColor += sampleC; 
		blurredColor += sampleD; 
		blurredColor += sampleE; 
		
		blurredColor /= sum;
		half4 color = blurredColor;	
		
		color.a = sampleA.a;
		
		return color;
	}
		
	// not used atm
	
	half4 fragBlurUnweightedDark (v2f i) : COLOR {
		half4 blurredColor = half4 (0,0,0,0);

		half4 sampleA = tex2D(_MainTex, i.uv.xy);
		half4 sampleB = tex2D(_MainTex, i.uv01.xy);
		half4 sampleC = tex2D(_MainTex, i.uv01.zw);
		half4 sampleD = tex2D(_MainTex, i.uv23.xy);
		half4 sampleE = tex2D(_MainTex, i.uv23.zw);
				
		blurredColor += sampleA;
		blurredColor += sampleB * 0.75;
		blurredColor += sampleC * 0.75; 
		blurredColor += sampleD * 0.5; 
		blurredColor += sampleE * 0.5; 
		
		blurredColor /= 3.5;
		
		blurredColor.a = max(UNITY_SAMPLE_1CHANNEL(_TapHigh, i.uv.xy), blurredColor.a);

		return blurredColor;
	}	
	
	// fragMixMediumAndLowTap
	// happens before applying final coc/blur result to screen,
	// mixes defocus buffers of different resolutions / bluriness
	
	sampler2D _TapMedium;
	sampler2D _TapLow;
	
	half4 fragMixMediumAndLowTap (v2fSingle i) : COLOR 
	{
	 	half4 tapMedium = tex2D (_TapMedium, i.uv.xy);
		half4 tapLow = tex2D (_TapLow, i.uv.xy);
		tapMedium.a *= tapMedium.a;
		
		tapLow.rgb = lerp (tapMedium.rgb, tapLow.rgb, (tapMedium.a * tapMedium.a));
		return tapLow;
	}
	
	ENDCG
	
Subshader {
	ZTest Always Cull Off ZWrite Off
	Fog { Mode off }  
	  	
  Pass {     
      
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurWeighted
      
      ENDCG
  }
  Pass {   
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurUnweighted
      
      ENDCG
  }    
  
  // 2
  
  Pass {    
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurUnweightedDark
      
      ENDCG
  }
  Pass {    
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertSingleTex
      #pragma fragment fragMixMediumAndLowTap
      
      ENDCG
  }   
  
  // 4
  
  Pass {    
      CGPROGRAM
      
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurDark
      
      ENDCG
  }
}

Fallback off
	
} // shader