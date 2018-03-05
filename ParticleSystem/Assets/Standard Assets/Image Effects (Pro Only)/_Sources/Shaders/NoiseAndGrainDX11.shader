// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/NoiseAndGrainDX11" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float4 _NoiseTex_TexelSize;
		
		uniform float4 _MainTex_TexelSize;

		uniform float3 _NoisePerChannel;
		uniform float3 _NoiseTilingPerChannel;
		uniform float3 _NoiseAmount;
		uniform float3 _ThreshholdRGB;
		uniform float3 _MidGrey;
		uniform float _DX11NoiseTime;

		// DX11 noise helper functions, credit: rgba/iq

		int ihash(int n)
		{
		    n = (n<<13)^n;
		   	return (n*(n*n*15731+789221)+1376312589) & 2147483647;
		}

		float frand(int n)
		{
			return ihash(n) / 2147483647.0;
		}

		float cellNoise1f(int3 p)
		{
			return frand(p.z*65536 + p.y*256 + p.x);//*2.0-1.0;
		}

		float3 cellNoise3f(int3 p)
		{
			int i = p.z*65536 + p.y*256 + p.x;
			return float3(frand(i), frand(i + 57), frand(i + 113));//*2.0-1.0;
		}		
		
		struct v2f 
		{
			float4 pos : SV_POSITION;
			float2 uv_screen : TEXCOORD0;
			float4 uvRg : TEXCOORD1;
			float2 uvB : TEXCOORD2;
			float2 uvOffsets : TEXCOORD4;
		};
		
		struct appdata_img2 
		{
		    float4 vertex : POSITION;
		    float2 texcoord : TEXCOORD0;
		    float2 texcoord1 : TEXCOORD1;
		};		

		inline float3 Overlay(float3 m, float3 color) {
			float3 check = step(0.5, color.rgb);
			float3 result = check * (float3(1,1,1) - ((float3(1,1,1) - 2*(color.rgb-0.5)) * (1-m.rgb))); 
			result += (1-check) * (2*color.rgb) * m.rgb;
			return result;
		}		
						
		v2f vert (appdata_img2 v)
		{
			v2f o;
			
			o.pos = UnityObjectToClipPos (v.vertex);	
			
		#if UNITY_UV_STARTS_AT_TOP
			o.uv_screen = v.vertex.xyxy;
			if (_MainTex_TexelSize.y < 0)
        		o.uv_screen.y = 1-o.uv_screen.y;
		#else
        		o.uv_screen = v.vertex.xy;
		#endif
			
			// different tiling for 3 channels
			o.uvRg = v.texcoord.xyxy + v.texcoord1.xyxy * _NoiseTilingPerChannel.rrgg * _NoiseTex_TexelSize.xyxy;
			o.uvB = v.texcoord.xy + v.texcoord1.xy * _NoiseTilingPerChannel.bb * _NoiseTex_TexelSize.xy;

			o.uvOffsets = v.texcoord.xy;

			return o; 
		}		

		float4 fragDX11 ( v2f i ) : COLOR
		{	
			float4 color = saturate(tex2D (_MainTex, i.uv_screen.xy));
			
			// black & white intensities
			float2 blackWhiteCurve = Luminance(color.rgb) - _MidGrey.x; // maybe tweak middle grey
			blackWhiteCurve.xy = saturate(blackWhiteCurve.xy * _MidGrey.yz); //float2(1.0/0.8, -1.0/0.2));

			float finalIntensity = _NoiseAmount.x + max(0.0f, dot(_NoiseAmount.zy, blackWhiteCurve.xy));
			
			float3 m = cellNoise3f(float3( (i.uv_screen.xy + i.uvOffsets) * _MainTex_TexelSize.zw, _DX11NoiseTime));
			m = saturate(lerp(float3(0.5,0.5,0.5), m, _NoisePerChannel.rgb * finalIntensity));	
			
			return float4(Overlay(m, color.rgb), color.a);
		}

		float4 fragDX11Monochrome ( v2f i ) : COLOR
		{	
			float4 color = saturate(tex2D (_MainTex, i.uv_screen.xy));
			
			// black & white intensities
			float2 blackWhiteCurve = Luminance(color.rgb) - _MidGrey.x; // maybe tweak middle grey
			blackWhiteCurve.xy = saturate(blackWhiteCurve.xy * _MidGrey.yz); //float2(1.0/0.8, -1.0/0.2));

			float finalIntensity = _NoiseAmount.x + max(0.0f, dot(_NoiseAmount.zy, blackWhiteCurve.xy));
			
			float3 m = cellNoise1f(float3( (i.uv_screen.xy + i.uvOffsets) * _MainTex_TexelSize.zw, _DX11NoiseTime));
			m = saturate(lerp(float3(0.5,0.5,0.5), m, finalIntensity));		
			
			return float4(Overlay(m, color.rgb), color.a);
		} 

		float4 fragDX11Tmp ( v2f i ) : COLOR
		{	
			float4 color = saturate(tex2D (_MainTex, i.uv_screen.xy));
			
			// black & white intensities
			float2 blackWhiteCurve = Luminance(color.rgb) - _MidGrey.x; // maybe tweak middle grey
			blackWhiteCurve.xy = saturate(blackWhiteCurve.xy * _MidGrey.yz); //float2(1.0/0.8, -1.0/0.2));

			float finalIntensity = _NoiseAmount.x + max(0.0f, dot(_NoiseAmount.zy, blackWhiteCurve.xy));
			
			float3 m = cellNoise3f(float3( (i.uv_screen.xy + i.uvOffsets) * _MainTex_TexelSize.zw, _DX11NoiseTime));
			m = saturate(lerp(float3(0.5,0.5,0.5), m, _NoisePerChannel.rgb * finalIntensity));	
			
			return float4(m.rgb, color.a);
		}

		float4 fragDX11MonochromeTmp ( v2f i ) : COLOR
		{	
			float4 color = saturate(tex2D (_MainTex, i.uv_screen.xy));
			
			// black & white intensities
			float2 blackWhiteCurve = Luminance(color.rgb) - _MidGrey.x; // maybe tweak middle grey
			blackWhiteCurve.xy = saturate(blackWhiteCurve.xy * _MidGrey.yz); //float2(1.0/0.8, -1.0/0.2));

			float finalIntensity = _NoiseAmount.x + max(0.0f, dot(_NoiseAmount.zy, blackWhiteCurve.xy));
			
			float3 m = cellNoise1f(float3( (i.uv_screen.xy + i.uvOffsets) * _MainTex_TexelSize.zw, _DX11NoiseTime));
			m = saturate(lerp(float3(0.5,0.5,0.5), m, finalIntensity));	
			
			return float4(m.rgb, color.a);
		}	

		float4 fragOverlayBlend	( v2f i ) : COLOR
		{	
			float4 color = saturate(tex2D (_MainTex, i.uv_screen.xy));
			float4 m = saturate(tex2D (_NoiseTex, i.uv_screen.xy));
			
			return float4(Overlay(m, color.rgb), color.a);
		}	
	
	ENDCG
	
	SubShader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }  

		Pass {
	
		CGPROGRAM
		
		#pragma exclude_renderers gles xbox360 ps3 d3d9
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment fragDX11
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}		

		Pass {
	
		CGPROGRAM
		
		#pragma exclude_renderers gles xbox360 ps3 d3d9
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment fragDX11Monochrome
		#pragma fragmentoption ARB_precision_hint_fastest 

		ENDCG
		 
		}

		Pass {
	
		CGPROGRAM
		
		#pragma exclude_renderers gles xbox360 ps3 d3d9
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment fragDX11Tmp
		#pragma fragmentoption ARB_precision_hint_fastest 
	
		ENDCG
		 
		}		

		Pass {
	
		CGPROGRAM
		
		#pragma exclude_renderers gles xbox360 ps3 d3d9
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment fragDX11MonochromeTmp
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}

		Pass {
	
		CGPROGRAM
		
		#pragma exclude_renderers gles xbox360 ps3 d3d9
		#pragma target 5.0
		#pragma vertex vert
		#pragma fragment fragOverlayBlend
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}					
	}
	FallBack Off
}
