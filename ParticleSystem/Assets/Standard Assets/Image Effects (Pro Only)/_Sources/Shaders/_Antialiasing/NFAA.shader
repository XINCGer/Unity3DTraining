// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/NFAA" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BlurTex ("Base (RGB)", 2D) = "white" {}

}

CGINCLUDE

#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform float _OffsetScale;
uniform float _BlurRadius;

struct v2f {
	float4 pos : POSITION;
	float2 uv[8] : TEXCOORD0;
};

	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		
		float2 uv = v.texcoord.xy;
				
		float2 up = float2(0.0, _MainTex_TexelSize.y) * _OffsetScale;
		float2 right = float2(_MainTex_TexelSize.x, 0.0) * _OffsetScale;	
			
		o.uv[0].xy = uv + up;
		o.uv[1].xy = uv - up;
		o.uv[2].xy = uv + right;
		o.uv[3].xy = uv - right;
		o.uv[4].xy = uv - right + up;
		o.uv[5].xy = uv - right -up;
		o.uv[6].xy = uv + right + up;
		o.uv[7].xy = uv + right -up;
		
		return o;
	}

	half4 frag (v2f i) : COLOR
	{	
		// get luminance values
		//  maybe: experiment with different luminance calculations
		float topL = Luminance( tex2D(_MainTex, i.uv[0]).rgb );
		float bottomL = Luminance( tex2D(_MainTex, i.uv[1]).rgb );
		float rightL = Luminance( tex2D(_MainTex, i.uv[2]).rgb );
		float leftL = Luminance( tex2D(_MainTex, i.uv[3]).rgb );
		float leftTopL = Luminance( tex2D(_MainTex, i.uv[4]).rgb );
		float leftBottomL = Luminance( tex2D(_MainTex, i.uv[5]).rgb );
		float rightBottomL = Luminance( tex2D(_MainTex, i.uv[6]).rgb );
		float rightTopL = Luminance( tex2D(_MainTex, i.uv[7]).rgb );
		
		// 2 triangle subtractions
		float sum0 = dot(float3(1,1,1), float3(rightTopL,bottomL,leftTopL));
		float sum1 = dot(float3(1,1,1), float3(leftBottomL,topL,rightBottomL));
		float sum2 = dot(float3(1,1,1), float3(leftTopL,rightL,leftBottomL));
		float sum3 = dot(float3(1,1,1), float3(rightBottomL,leftL,rightTopL));

		// figure out "normal"
		float2 blurDir = half2((sum0-sum1), (sum3-sum2));
		blurDir *= _MainTex_TexelSize.xy * _BlurRadius;

		// reconstruct normal uv
		float2 uv_ = (i.uv[0] + i.uv[1]) * 0.5;
		 
		float4 returnColor = tex2D(_MainTex, uv_);
		returnColor += tex2D(_MainTex, uv_+ blurDir.xy);
		returnColor += tex2D(_MainTex, uv_ - blurDir.xy);
		returnColor += tex2D(_MainTex, uv_ + float2(blurDir.x, -blurDir.y));
		returnColor += tex2D(_MainTex, uv_ - float2(blurDir.x, -blurDir.y));

		return returnColor * 0.2;
	}
	
	half4 fragDebug (v2f i) : COLOR
	{	
		// get luminance values
		//  maybe: experiment with different luminance calculations
		float topL = Luminance( tex2D(_MainTex, i.uv[0]).rgb );
		float bottomL = Luminance( tex2D(_MainTex, i.uv[1]).rgb );
		float rightL = Luminance( tex2D(_MainTex, i.uv[2]).rgb );
		float leftL = Luminance( tex2D(_MainTex, i.uv[3]).rgb );
		float leftTopL = Luminance( tex2D(_MainTex, i.uv[4]).rgb );
		float leftBottomL = Luminance( tex2D(_MainTex, i.uv[5]).rgb );
		float rightBottomL = Luminance( tex2D(_MainTex, i.uv[6]).rgb );
		float rightTopL = Luminance( tex2D(_MainTex, i.uv[7]).rgb );
		
		// 2 triangle subtractions
		float sum0 = dot(float3(1,1,1), float3(rightTopL,bottomL,leftTopL));
		float sum1 = dot(float3(1,1,1), float3(leftBottomL,topL,rightBottomL));
		float sum2 = dot(float3(1,1,1), float3(leftTopL,rightL,leftBottomL));
		float sum3 = dot(float3(1,1,1), float3(rightBottomL,leftL,rightTopL));

		// figure out "normal"
		float2 blurDir = half2((sum0-sum1), (sum3-sum2));
		blurDir *= _MainTex_TexelSize.xy * _BlurRadius;

		// reconstruct normal uv
		float2 uv_ = (i.uv[0] + i.uv[1]) * 0.5;
		 
		float4 returnColor = tex2D(_MainTex, uv_);
		returnColor += tex2D(_MainTex, uv_+ blurDir.xy);
		returnColor += tex2D(_MainTex, uv_ - blurDir.xy);
		returnColor += tex2D(_MainTex, uv_ + float2(blurDir.x, -blurDir.y));
		returnColor += tex2D(_MainTex, uv_ - float2(blurDir.x, -blurDir.y));

		blurDir = half2((sum0-sum1), (sum3-sum2)) * _BlurRadius;
		return half4(normalize( half3(blurDir,1) * 0.5 + 0.5), 1);
		return returnColor * 0.2;
	}	
	
ENDCG

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
	
		CGPROGRAM
	
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma exclude_renderers d3d11_9x
		#pragma glsl
		
		ENDCG
	}
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
	
		CGPROGRAM
	
		#pragma vertex vert
		#pragma fragment fragDebug
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers d3d11_9x
		#pragma glsl
		
		ENDCG
	}
}
/*
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
*/

Fallback off

}