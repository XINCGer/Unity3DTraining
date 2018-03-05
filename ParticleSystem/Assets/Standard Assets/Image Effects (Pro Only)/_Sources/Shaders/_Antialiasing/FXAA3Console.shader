// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



/*============================================================================

source taken from


                    NVIDIA FXAA 3.11 by TIMOTHY LOTTES

                                        
and adapted and ported to Unity by Unity Technologies

                    
------------------------------------------------------------------------------                       
COPYRIGHT (C) 2010, 2011 NVIDIA CORPORATION. ALL RIGHTS RESERVED.
------------------------------------------------------------------------------                       
TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE LAW, THIS SOFTWARE IS PROVIDED 
*AS IS* AND NVIDIA AND ITS SUPPLIERS DISCLAIM ALL WARRANTIES, EITHER EXPRESS 
OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. IN NO EVENT SHALL NVIDIA 
OR ITS SUPPLIERS BE LIABLE FOR ANY SPECIAL, INCIDENTAL, INDIRECT, OR 
CONSEQUENTIAL DAMAGES WHATSOEVER (INCLUDING, WITHOUT LIMITATION, DAMAGES FOR 
LOSS OF BUSINESS PROFITS, BUSINESS INTERRUPTION, LOSS OF BUSINESS INFORMATION, 
OR ANY OTHER PECUNIARY LOSS) ARISING OUT OF THE USE OF OR INABILITY TO USE 
THIS SOFTWARE, EVEN IF NVIDIA HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH 
DAMAGES.

============================================================================*/


Shader "Hidden/FXAA III (Console)" {
	Properties {
		_MainTex ("-", 2D) = "white" {}
		_EdgeThresholdMin ("Edge threshold min",float) = 0.125
		_EdgeThreshold("Edge Threshold", float) = 0.25
		_EdgeSharpness("Edge sharpness",float) = 4.0
	}
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		#pragma exclude_renderers d3d11_9x
		
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform half _EdgeThresholdMin;
		uniform half _EdgeThreshold;
		uniform half _EdgeSharpness;

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 interpolatorA : TEXCOORD1;
			float4 interpolatorB : TEXCOORD2;
			float4 interpolatorC : TEXCOORD3;
		};
		
		float4 _MainTex_TexelSize;
		
		v2f vert (appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			
			o.uv = v.texcoord.xy;
			
			float4 extents;
			float2 offset = ( _MainTex_TexelSize.xy ) * 0.5f;
			extents.xy = v.texcoord.xy - offset;
			extents.zw = v.texcoord.xy + offset;

			float4 rcpSize;
			rcpSize.xy = -_MainTex_TexelSize.xy * 0.5f;
			rcpSize.zw = _MainTex_TexelSize.xy * 0.5f;			
			
			o.interpolatorA = extents;
			o.interpolatorB = rcpSize;
			o.interpolatorC = rcpSize;
			
			o.interpolatorC.xy *= 4.0;
			o.interpolatorC.zw *= 4.0;
			
			return o;
		}

// hacky support for NaCl
#if defined(SHADER_API_GLES) && defined(SHADER_API_DESKTOP)
		#define FxaaTexTop(t, p) tex2D(t, p) 
#else
		#define FxaaTexTop(t, p) tex2Dlod(t, float4(p, 0.0, 0.0))
#endif

		inline half TexLuminance( float2 uv )
		{
			return Luminance(FxaaTexTop(_MainTex, uv).rgb);
		}

		half3 FxaaPixelShader(float2 pos, float4 extents, float4 rcpSize, float4 rcpSize2)
		{
			half lumaNw = TexLuminance(extents.xy);
			half lumaSw = TexLuminance(extents.xw);
			half lumaNe = TexLuminance(extents.zy);
			half lumaSe = TexLuminance(extents.zw);
			
			half3 centre = FxaaTexTop(_MainTex, pos).rgb;
			half lumaCentre = Luminance(centre);
			
			half lumaMaxNwSw = max( lumaNw , lumaSw );
			lumaNe += 1.0/384.0;
			half lumaMinNwSw = min( lumaNw , lumaSw );
			
			half lumaMaxNeSe = max( lumaNe , lumaSe );
			half lumaMinNeSe = min( lumaNe , lumaSe );
			
			half lumaMax = max( lumaMaxNeSe, lumaMaxNwSw );
			half lumaMin = min( lumaMinNeSe, lumaMinNwSw );
			
			half lumaMaxScaled = lumaMax * _EdgeThreshold;
			
			half lumaMinCentre = min( lumaMin , lumaCentre );
			half lumaMaxScaledClamped = max( _EdgeThresholdMin , lumaMaxScaled );
			half lumaMaxCentre = max( lumaMax , lumaCentre );
			half dirSWMinusNE = lumaSw - lumaNe;
			half lumaMaxCMinusMinC = lumaMaxCentre - lumaMinCentre;
			half dirSEMinusNW = lumaSe - lumaNw;
			
			if(lumaMaxCMinusMinC < lumaMaxScaledClamped)
				return centre;
			
			half2 dir;
			dir.x = dirSWMinusNE + dirSEMinusNW;
			dir.y = dirSWMinusNE - dirSEMinusNW;
			
			dir = normalize(dir);			
			half3 col1 = FxaaTexTop(_MainTex, pos.xy - dir * rcpSize.zw).rgb;
			half3 col2 = FxaaTexTop(_MainTex, pos.xy + dir * rcpSize.zw).rgb;
			
			half dirAbsMinTimesC = min( abs( dir.x ) , abs( dir.y ) ) * _EdgeSharpness;
			dir = clamp(dir.xy/dirAbsMinTimesC, -2.0, 2.0);
			
			half3 col3 = FxaaTexTop(_MainTex, pos.xy - dir * rcpSize2.zw).rgb;
			half3 col4 = FxaaTexTop(_MainTex, pos.xy + dir * rcpSize2.zw).rgb;
			
			half3 rgbyA = col1 + col2;
			half3 rgbyB = ((col3 + col4) * 0.25) + (rgbyA * 0.25);
			
			if((Luminance(rgbyA) < lumaMin) || (Luminance(rgbyB) > lumaMax))
				return rgbyA * 0.5;
			else
				return rgbyB;
		}

		half4 frag (v2f i) : COLOR
		{
			half3 color = FxaaPixelShader(i.uv, i.interpolatorA, i.interpolatorB, i.interpolatorC);
			return half4(color, 1.0);
		}
		
		ENDCG
		}
	} 
	FallBack Off
}

