// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FXAA II" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma target 3.0
#pragma glsl
#pragma exclude_renderers d3d11_9x

#define FXAA_HLSL_3 1

/*============================================================================
 
                  FXAA v2 CONSOLE by TIMOTHY LOTTES @ NVIDIA                                

============================================================================*/

/*============================================================================
                                 API PORTING
============================================================================*/
#ifndef     FXAA_GLSL_120
    #define FXAA_GLSL_120 0
#endif
#ifndef     FXAA_GLSL_130
    #define FXAA_GLSL_130 0
#endif
#ifndef     FXAA_HLSL_3
    #define FXAA_HLSL_3 0
#endif
#ifndef     FXAA_HLSL_4
    #define FXAA_HLSL_4 0
#endif    
/*--------------------------------------------------------------------------*/
#if FXAA_GLSL_120
    // Requires,
    //  #version 120
    //  #extension GL_EXT_gpu_shader4 : enable
    #define int2 ivec2
    #define float2 vec2
    #define float3 vec3
    #define float4 vec4
    #define FxaaInt2 ivec2
    #define FxaaFloat2 vec2
    #define FxaaSat(a) clamp((a), 0.0, 1.0)
    #define FxaaTex sampler2D
    #define FxaaTexLod0(t, p) texture2DLod(t, p, 0.0)
    #define FxaaTexOff(t, p, o, r) texture2DLodOffset(t, p, 0.0, o)
#endif
/*--------------------------------------------------------------------------*/
#if FXAA_GLSL_130
    // Requires "#version 130" or better
    #define int2 ivec2
    #define float2 vec2
    #define float3 vec3
    #define float4 vec4
    #define FxaaInt2 ivec2
    #define FxaaFloat2 vec2
    #define FxaaSat(a) clamp((a), 0.0, 1.0)
    #define FxaaTex sampler2D
    #define FxaaTexLod0(t, p) textureLod(t, p, 0.0)
    #define FxaaTexOff(t, p, o, r) textureLodOffset(t, p, 0.0, o)
#endif
/*--------------------------------------------------------------------------*/
#if FXAA_HLSL_3
    #define int2 float2
    #define FxaaInt2 float2
    #define FxaaFloat2 float2
    #define FxaaSat(a) saturate((a))
    #define FxaaTex sampler2D
    #define FxaaTexLod0(t, p) tex2Dlod(t, float4(p, 0.0, 0.0))
    #define FxaaTexOff(t, p, o, r) tex2Dlod(t, float4(p + (o * r), 0, 0))
#endif
/*--------------------------------------------------------------------------*/
#if FXAA_HLSL_4
    #define FxaaInt2 int2
    #define FxaaFloat2 float2
    #define FxaaSat(a) saturate((a))
    struct FxaaTex { SamplerState smpl; Texture2D tex; };
    #define FxaaTexLod0(t, p) t.tex.SampleLevel(t.smpl, p, 0.0) 
    #define FxaaTexOff(t, p, o, r) t.tex.SampleLevel(t.smpl, p, 0.0, o)
#endif


/*============================================================================

                                VERTEX SHADER
                                
============================================================================*/
float4 FxaaVertexShader(
float2 pos,                 // Both x and y range {-1.0 to 1.0 across screen}.
float2 rcpFrame) {          // {1.0/frameWidth, 1.0/frameHeight}
/*--------------------------------------------------------------------------*/
    #define FXAA_SUBPIX_SHIFT (1.0/4.0)
/*--------------------------------------------------------------------------*/
    float4 posPos;
    posPos.xy = (pos.xy * 0.5) + 0.5;
    posPos.zw = posPos.xy - (rcpFrame * (0.5 + FXAA_SUBPIX_SHIFT));
    return posPos; }
        
/*============================================================================
 
                                PIXEL SHADER
                                
============================================================================*/
float3 FxaaPixelShader(
float4 posPos,       // Output of FxaaVertexShader interpolated across screen.
FxaaTex tex,         // Input texture.
float2 rcpFrame) {   // Constant {1.0/frameWidth, 1.0/frameHeight}.
/*--------------------------------------------------------------------------*/
    #define FXAA_REDUCE_MIN   (1.0/128.0)
    #define FXAA_REDUCE_MUL   (1.0/8.0)
    #define FXAA_SPAN_MAX     8.0
/*--------------------------------------------------------------------------*/
    float3 rgbNW = FxaaTexLod0(tex, posPos.zw).xyz;
    float3 rgbNE = FxaaTexOff(tex, posPos.zw, FxaaInt2(1,0), rcpFrame.xy).xyz;
    float3 rgbSW = FxaaTexOff(tex, posPos.zw, FxaaInt2(0,1), rcpFrame.xy).xyz;
    float3 rgbSE = FxaaTexOff(tex, posPos.zw, FxaaInt2(1,1), rcpFrame.xy).xyz;
    float3 rgbM  = FxaaTexLod0(tex, posPos.xy).xyz;
/*--------------------------------------------------------------------------*/
    float3 luma = float3(0.299, 0.587, 0.114);
    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM  = dot(rgbM,  luma);
/*--------------------------------------------------------------------------*/
    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
/*--------------------------------------------------------------------------*/
    float2 dir; 
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));
/*--------------------------------------------------------------------------*/
    float dirReduce = max(
        (lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * FXAA_REDUCE_MUL),
        FXAA_REDUCE_MIN);
    float rcpDirMin = 1.0/(min(abs(dir.x), abs(dir.y)) + dirReduce);
    dir = min(FxaaFloat2( FXAA_SPAN_MAX,  FXAA_SPAN_MAX), 
          max(FxaaFloat2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), 
          dir * rcpDirMin)) * rcpFrame.xy;
/*--------------------------------------------------------------------------*/
    float3 rgbA = (1.0/2.0) * (
        FxaaTexLod0(tex, posPos.xy + dir * (1.0/3.0 - 0.5)).xyz +
        FxaaTexLod0(tex, posPos.xy + dir * (2.0/3.0 - 0.5)).xyz);
    float3 rgbB = rgbA * (1.0/2.0) + (1.0/4.0) * (
        FxaaTexLod0(tex, posPos.xy + dir * (0.0/3.0 - 0.5)).xyz +
        FxaaTexLod0(tex, posPos.xy + dir * (3.0/3.0 - 0.5)).xyz);
    float lumaB = dot(rgbB, luma);
    if((lumaB < lumaMin) || (lumaB > lumaMax)) return rgbA;
    return rgbB; }


struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
};

float4 _MainTex_TexelSize;

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = FxaaVertexShader (v.texcoord.xy*2-1, _MainTex_TexelSize.xy);
	return o;
}

sampler2D _MainTex;

float4 frag (v2f i) : COLOR0
{
	return float4(FxaaPixelShader(i.uv, _MainTex, _MainTex_TexelSize.xy).xyz, 0.0f);
}
	
ENDCG
	}
}

Fallback off
}
