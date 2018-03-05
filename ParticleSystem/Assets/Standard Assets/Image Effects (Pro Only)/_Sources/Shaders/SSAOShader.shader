// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SSAO" {
Properties {
	_MainTex ("", 2D) = "" {}
	_RandomTexture ("", 2D) = "" {}
	_SSAO ("", 2D) = "" {}
}
Subshader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

CGINCLUDE
// Common code used by several SSAO passes below
#include "UnityCG.cginc"
#pragma exclude_renderers gles
struct v2f_ao {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	float2 uvr : TEXCOORD1;
};

uniform float2 _NoiseScale;
float4 _CameraDepthNormalsTexture_ST;

v2f_ao vert_ao (appdata_img v)
{
	v2f_ao o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = TRANSFORM_TEX(v.texcoord, _CameraDepthNormalsTexture);
	o.uvr = v.texcoord.xy * _NoiseScale;
	return o;
}

sampler2D _CameraDepthNormalsTexture;
sampler2D _RandomTexture;
float4 _Params; // x=radius, y=minz, z=attenuation power, w=SSAO power

#ifdef UNITY_COMPILER_HLSL

#	define INPUT_SAMPLE_COUNT 8
#	include "frag_ao.cginc"

#	define INPUT_SAMPLE_COUNT 14
#	include "frag_ao.cginc"

#	define INPUT_SAMPLE_COUNT 26
#	include "frag_ao.cginc"

#	define INPUT_SAMPLE_COUNT 34
#	include "frag_ao.cginc"

#else
#	define INPUT_SAMPLE_COUNT
#	include "frag_ao.cginc"
#endif

ENDCG

	// ---- SSAO pass, 8 samples
	Pass {
		
CGPROGRAM
#pragma vertex vert_ao
#pragma fragment frag
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest


half4 frag (v2f_ao i) : COLOR
{
	#define SAMPLE_COUNT 8
	const float3 RAND_SAMPLES[SAMPLE_COUNT] = {
		float3(0.01305719,0.5872321,-0.119337),
		float3(0.3230782,0.02207272,-0.4188725),
		float3(-0.310725,-0.191367,0.05613686),
		float3(-0.4796457,0.09398766,-0.5802653),
		float3(0.1399992,-0.3357702,0.5596789),
		float3(-0.2484578,0.2555322,0.3489439),
		float3(0.1871898,-0.702764,-0.2317479),
		float3(0.8849149,0.2842076,0.368524),
	};
    return frag_ao (i, SAMPLE_COUNT, RAND_SAMPLES);
}
ENDCG

	}

// ---- SSAO pass, 14 samples
	Pass {
		
CGPROGRAM
#pragma vertex vert_ao
#pragma fragment frag
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest


half4 frag (v2f_ao i) : COLOR
{
	#define SAMPLE_COUNT 14
	const float3 RAND_SAMPLES[SAMPLE_COUNT] = {
		float3(0.4010039,0.8899381,-0.01751772),
		float3(0.1617837,0.1338552,-0.3530486),
		float3(-0.2305296,-0.1900085,0.5025396),
		float3(-0.6256684,0.1241661,0.1163932),
		float3(0.3820786,-0.3241398,0.4112825),
		float3(-0.08829653,0.1649759,0.1395879),
		float3(0.1891677,-0.1283755,-0.09873557),
		float3(0.1986142,0.1767239,0.4380491),
		float3(-0.3294966,0.02684341,-0.4021836),
		float3(-0.01956503,-0.3108062,-0.410663),
		float3(-0.3215499,0.6832048,-0.3433446),
		float3(0.7026125,0.1648249,0.02250625),
		float3(0.03704464,-0.939131,0.1358765),
		float3(-0.6984446,-0.6003422,-0.04016943),
	};
    return frag_ao (i, SAMPLE_COUNT, RAND_SAMPLES);
}
ENDCG

	}
	
// ---- SSAO pass, 26 samples
	Pass {
		
CGPROGRAM
#pragma vertex vert_ao
#pragma fragment frag
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest


half4 frag (v2f_ao i) : COLOR
{
	#define SAMPLE_COUNT 26
	const float3 RAND_SAMPLES[SAMPLE_COUNT] = {
		float3(0.2196607,0.9032637,0.2254677),
		float3(0.05916681,0.2201506,-0.1430302),
		float3(-0.4152246,0.1320857,0.7036734),
		float3(-0.3790807,0.1454145,0.100605),
		float3(0.3149606,-0.1294581,0.7044517),
		float3(-0.1108412,0.2162839,0.1336278),
		float3(0.658012,-0.4395972,-0.2919373),
		float3(0.5377914,0.3112189,0.426864),
		float3(-0.2752537,0.07625949,-0.1273409),
		float3(-0.1915639,-0.4973421,-0.3129629),
		float3(-0.2634767,0.5277923,-0.1107446),
		float3(0.8242752,0.02434147,0.06049098),
		float3(0.06262707,-0.2128643,-0.03671562),
		float3(-0.1795662,-0.3543862,0.07924347),
		float3(0.06039629,0.24629,0.4501176),
		float3(-0.7786345,-0.3814852,-0.2391262),
		float3(0.2792919,0.2487278,-0.05185341),
		float3(0.1841383,0.1696993,-0.8936281),
		float3(-0.3479781,0.4725766,-0.719685),
		float3(-0.1365018,-0.2513416,0.470937),
		float3(0.1280388,-0.563242,0.3419276),
		float3(-0.4800232,-0.1899473,0.2398808),
		float3(0.6389147,0.1191014,-0.5271206),
		float3(0.1932822,-0.3692099,-0.6060588),
		float3(-0.3465451,-0.1654651,-0.6746758),
		float3(0.2448421,-0.1610962,0.1289366),
	};
    return frag_ao (i, SAMPLE_COUNT, RAND_SAMPLES);
}
ENDCG

	}

// ---- Blur pass
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct v2f {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv = TRANSFORM_TEX (v.texcoord, _CameraDepthNormalsTexture);
	return o;
}

sampler2D _SSAO;
float3 _TexelOffsetScale;

inline half CheckSame (half4 n, half4 nn)
{
	// difference in normals
	half2 diff = abs(n.xy - nn.xy);
	half sn = (diff.x + diff.y) < 0.1;
	// difference in depth
	float z = DecodeFloatRG (n.zw);
	float zz = DecodeFloatRG (nn.zw);
	float zdiff = abs(z-zz) * _ProjectionParams.z;
	half sz = zdiff < 0.2;
	return sn * sz;
}


half4 frag( v2f i ) : COLOR
{
	#define NUM_BLUR_SAMPLES 4
	
    float2 o = _TexelOffsetScale.xy;
    
    half sum = tex2D(_SSAO, i.uv).r * (NUM_BLUR_SAMPLES + 1);
    half denom = NUM_BLUR_SAMPLES + 1;
    
    half4 geom = tex2D (_CameraDepthNormalsTexture, i.uv);
    
    for (int s = 0; s < NUM_BLUR_SAMPLES; ++s)
    {
        float2 nuv = i.uv + o * (s+1);
        half4 ngeom = tex2D (_CameraDepthNormalsTexture, nuv.xy);
        half coef = (NUM_BLUR_SAMPLES - s) * CheckSame (geom, ngeom);
        sum += tex2D (_SSAO, nuv.xy).r * coef;
        denom += coef;
    }
    for (int s = 0; s < NUM_BLUR_SAMPLES; ++s)
    {
        float2 nuv = i.uv - o * (s+1);
        half4 ngeom = tex2D (_CameraDepthNormalsTexture, nuv.xy);
        half coef = (NUM_BLUR_SAMPLES - s) * CheckSame (geom, ngeom);
        sum += tex2D (_SSAO, nuv.xy).r * coef;
        denom += coef;
    }
    return sum / denom;
}
ENDCG
	}
	
	// ---- Composite pass
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct v2f {
	float4 pos : POSITION;
	float2 uv[2] : TEXCOORD0;
};

v2f vert (appdata_img v)
{
	v2f o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv[0] = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord);
	o.uv[1] = MultiplyUV (UNITY_MATRIX_TEXTURE1, v.texcoord);
	return o;
}

sampler2D _MainTex;
sampler2D _SSAO;

half4 frag( v2f i ) : COLOR
{
	half4 c = tex2D (_MainTex, i.uv[0]);
	half ao = tex2D (_SSAO, i.uv[1]).r;
	ao = pow (ao, _Params.w);
	c.rgb *= ao;
	return c;
}
ENDCG
	}

}

Fallback off
}
