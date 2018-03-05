// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


// This Ambient Occlusion image effect is based on "Scalable Ambient Obscurance":

/**

\author Morgan McGuire and Michael Mara, NVIDIA and Williams College, http://research.nvidia.com, http://graphics.cs.williams.edu

Open Source under the "BSD" license: http://www.opensource.org/licenses/bsd-license.php

Copyright (c) 2011-2012, NVIDIA
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

Shader "Hidden/ScreenSpaceAmbientObscurance" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	#ifdef SHADER_API_D3D11
		#define NUM_SAMPLES (15)
	#else
		#define NUM_SAMPLES (11)
	#endif

	#define FAR_PLANE_Z (300.0)
	#define NUM_SPIRAL_TURNS (7)
	#define bias (0.01) 

	float _Radius;
	float _Radius2; // _Radius * _Radius;
	float _Intensity;
	float4 _ProjInfo;
	float4x4 _ProjectionInv; // ref only

	sampler2D _CameraDepthTexture;
	sampler2D _Rand;
	sampler2D _AOTex;
	sampler2D _MainTex;

	float4 _MainTex_TexelSize;

	static const float gaussian[5] = { 0.153170, 0.144893, 0.122649, 0.092902, 0.062970 };  // stddev = 2.0

	float2 _Axis;

	/** Increase to make edges crisper. Decrease to reduce temporal flicker. */
	#define EDGE_SHARPNESS     (1.0)

	float _BlurFilterDistance;
	#define SCALE               _BlurFilterDistance

	/** Filter _Radius in pixels. This will be multiplied by SCALE. */
	#define R                   (4)

	struct v2f 
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 uv2 : TEXCOORD1;
	};

	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv2 = v.texcoord.xy;
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv2.y = 1-o.uv2.y;
		#endif				
		return o;
	}

	float3 ReconstructCSPosition(float2 S, float z) 
	{
		float linEyeZ = LinearEyeDepth(z);
		return float3(( ( S.xy * _MainTex_TexelSize.zw) * _ProjInfo.xy + _ProjInfo.zw) * linEyeZ, linEyeZ);
		
		/*
		// for reference
		float4 clipPos = float4(S*2.0-1.0, (z*2-1), 1);
		float4 viewPos;
		viewPos.x = dot((float4)_ProjectionInv[0], clipPos);
		viewPos.y = dot((float4)_ProjectionInv[1], clipPos);
		viewPos.w = dot((float4)_ProjectionInv[3], clipPos);
		viewPos.z = z;
		viewPos = viewPos/viewPos.w;
		return viewPos.xyz;
		*/
	}

	float3 ReconstructCSFaceNormal(float3 C) {
		return normalize(cross(ddy(C), ddx(C)));
	}


	/** Returns a unit vector and a screen-space _Radius for the tap on a unit disk (the caller should scale by the actual disk _Radius) */

	float2 TapLocation(int sampleNumber, float spinAngle, out float ssR){
		// Radius relative to ssR
		float alpha = float(sampleNumber + 0.5) * (1.0 / NUM_SAMPLES);
		float angle = alpha * (NUM_SPIRAL_TURNS * 6.28) + spinAngle;

		ssR = alpha;
		return float2(cos(angle), sin(angle));
	}

	/** Used for packing Z into the GB channels */
	float CSZToKey(float z) {
		return saturate(z * (1.0 / FAR_PLANE_Z));
	}

	/** Used for packing Z into the GB channels */
	void packKey(float key, out float2 p) {
		// Round to the nearest 1/256.0
		float temp = floor(key * 256.0);

		// Integer part
		p.x = temp * (1.0 / 256.0);

		// Fractional part
		p.y = key * 256.0 - temp;
	}

	/** Returns a number on (0, 1) */
	float UnpackKey(float2 p)
	{
		return p.x * (256.0 / 257.0) + p.y * (1.0 / 257.0);
	} 


	/** Read the camera-space position of the point at screen-space pixel ssP */
	float3 GetPosition(float2 ssP) {
		float3 P;

		P.z = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, ssP.xy));

		// Offset to pixel center
		P = ReconstructCSPosition(float2(ssP) /*+ float2(0.5, 0.5)*/, P.z);
		return P;
	}

	/** Read the camera-space position of the point at screen-space pixel ssP + unitOffset * ssR.  Assumes length(unitOffset) == 1 */
	float3 GetOffsetPosition(float2 ssC, float2 unitOffset, float ssR) 
	{
		float2 ssP = saturate(float2(ssR*unitOffset) + ssC);

		float3 P;
		P.z = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, ssP.xy));

		// Offset to pixel center
		P = ReconstructCSPosition(float2(ssP)/* + float2(0.5, 0.5)*/, P.z);

		return P;
	}

	/** Compute the occlusion due to sample with index \a i about the pixel at \a ssC that corresponds
    to camera-space point \a C with unit normal \a n_C, using maximum screen-space sampling _Radius \a ssDiskRadius */
	
	float SampleAO(in float2 ssC, in float3 C, in float3 n_C, in float ssDiskRadius, in int tapIndex, in float randomPatternRotationAngle) 
	{
		// Offset on the unit disk, spun for this pixel
		float ssR;
		float2 unitOffset = TapLocation(tapIndex, randomPatternRotationAngle, ssR);
		ssR *= ssDiskRadius;

		// The occluding point in camera space
		float3 Q = GetOffsetPosition(ssC, unitOffset, ssR);

		float3 v = Q - C; 

		float vv = dot(v, v);
		float vn = dot(v, n_C);

	    const float epsilon = 0.01;
	    float f = max(_Radius2 - vv, 0.0); 
	    return f * f * f * max((vn - bias) / (epsilon + vv), 0.0);
	}

	float4 fragAO(v2f i) : COLOR
	{
		float4 fragment = fixed4(1,1,1,1);

		// Pixel being shaded 
		float2 ssC = i.uv2.xy;// * _MainTex_TexelSize.zw;

		// View space point being shaded
		float3 C = GetPosition(ssC);

		//return abs(float4(C.xyz,0));
		//if(abs(C.z)<0.31)
		//	return 1;
		//return abs(C.z);

		packKey(CSZToKey(C.z), fragment.gb);
		//packKey(CSZToKey(C.z), bilateralKey);

		float randomPatternRotationAngle = 1.0;
		#ifdef SHADER_API_D3D11
			int2 ssCInt = ssC.xy * _MainTex_TexelSize.zw;
			randomPatternRotationAngle = (3 * ssCInt.x ^ ssCInt.y + ssCInt.x * ssCInt.y) * 10;
		#else
			// TODO: make dx9 rand better
			randomPatternRotationAngle = tex2D(_Rand, i.uv*12.0).x * 1000.0;
		#endif

		// Reconstruct normals from positions. These will lead to 1-pixel black lines
		// at depth discontinuities, however the blur will wipe those out so they are not visible
		// in the final image.
		float3 n_C = ReconstructCSFaceNormal(C);

		//return float4((n_C),0);

		// Choose the screen-space sample _Radius
		// proportional to the projected area of the sphere
		float ssDiskRadius = -_Radius / C.z; // -projScale * _Radius / C.z; // <:::::

		float sum = 0.0;
		for (int l = 0; l < NUM_SAMPLES; ++l) {
		     sum += SampleAO(ssC, C, n_C, (ssDiskRadius), l, randomPatternRotationAngle);
		}

        float temp = _Radius2 * _Radius;
        sum /= temp * temp;

		float A = max(0.0, 1.0 - sum * _Intensity * (5.0 / NUM_SAMPLES));
		fragment.ra = float2(A,A);

		return fragment;
	}

	float4 fragUpsample (v2f i) : COLOR
	{
		float4 fragment = fixed4(1,1,1,1);

		// View space point being shaded
		float3 C = GetPosition(i.uv.xy);

		packKey(CSZToKey(C.z), fragment.gb);
		fragment.ra = tex2D(_MainTex, i.uv.xy).ra;

		return fragment;
	}

	float4 fragApply (v2f i) : COLOR
	{
		float4 ao = tex2D(_AOTex, i.uv2.xy);
		return tex2D(_MainTex, i.uv.xy) * ao.rrrr;
	}

	float4 fragApplySoft (v2f i) : COLOR
	{
		float4 color = tex2D(_MainTex, i.uv.xy);

		float ao = tex2D(_AOTex, i.uv2.xy).r;
		ao += tex2D(_AOTex, i.uv2.xy + _MainTex_TexelSize.xy * 0.75).r;
		ao += tex2D(_AOTex, i.uv2.xy - _MainTex_TexelSize.xy * 0.75).r;
		ao += tex2D(_AOTex, i.uv2.xy + _MainTex_TexelSize.xy * float2(-0.75,0.75)).r;
		ao += tex2D(_AOTex, i.uv2.xy - _MainTex_TexelSize.xy * float2(-0.75,0.75)).r;

		return color * float4(ao,ao,ao,5)/5;
	}

	float4 fragBlurBL (v2f i) : COLOR
	{
		float4 fragment = float4(1,1,1,1);

		float2 ssC = i.uv.xy;

		float4 temp = tex2Dlod(_MainTex, float4(i.uv.xy,0,0));

		float2 passthrough2 = temp.gb;
		float key = UnpackKey(passthrough2);

		float sum = temp.r;

		/*
		if (key >= 0.999) { 
			// Sky pixel (if you aren't using depth keying, disable this test)
			fragment.gb = passthrough2; 
			return fragment;
		}
		*/

		// Base weight for depth falloff. Increase this for more blurriness, decrease it for better edge discrimination

		float BASE = gaussian[0] * 0.5; // ole: i decreased
		float totalWeight = BASE;
		sum *= totalWeight; 

		for (int r = -R; r <= R; ++r) {
			// We already handled the zero case above.  This loop should be unrolled and the branch discarded
			if (r != 0) {
				temp = tex2Dlod(_MainTex, float4(ssC + _Axis * _MainTex_TexelSize.xy * (r * SCALE),0,0) );
				float tapKey = UnpackKey(temp.gb);
				float value  = temp.r;

				// spatial domain: offset gaussian tap
				int index = r; if (index<0) index = -index;
				float weight = 0.3 + gaussian[index];

				// range domain (the "bilateral" weight). As depth difference increases, decrease weight.
				weight *= max(0.0, 1.0 - (2000.0 * EDGE_SHARPNESS) * abs(tapKey - key));

				sum += value * weight;
				totalWeight += weight;
			} 
		}

		const float epsilon = 0.0001;
		fragment = sum / (totalWeight + epsilon);	
		
		fragment.gb = passthrough2;

		return fragment;
	}

	ENDCG

SubShader {

	// 0: get ao
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment fragAO
		#pragma target 3.0
		#pragma glsl	
		#pragma exclude_renderers d3d11_9x flash
		
		ENDCG
	}

	// 1: bilateral blur
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment fragBlurBL
		#pragma target 3.0 
		#pragma glsl
		#pragma exclude_renderers d3d11_9x flash
		
		ENDCG
	}

	// 2: apply ao
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment fragApply
		#pragma target 3.0 
		#pragma glsl
		#pragma exclude_renderers d3d11_9x flash
		
		ENDCG
	}

	// 3: apply with a slight box filter
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment fragApplySoft
		#pragma target 3.0 
		#pragma glsl
		#pragma exclude_renderers d3d11_9x flash
		
		ENDCG
	}

	// 4: in case you want to blur in high rez for nicer z borders
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }

		CGPROGRAM

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment fragUpsample
		#pragma target 3.0 
		#pragma glsl
		#pragma exclude_renderers d3d11_9x flash
		
		ENDCG
	}
}

Fallback off

}