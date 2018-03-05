// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/MotionBlurClear" 
{

Properties { }

SubShader {
Pass {
	//ZTest LEqual
	ZTest Always // lame depth test
	ZWrite Off // lame depth test

	CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
	#pragma glsl

	#include "UnityCG.cginc"

	struct vs_input {
		float4 vertex : POSITION;
	};

	struct ps_input {
		float4 pos : SV_POSITION;
		float4 screen : TEXCOORD0;
	};

	sampler2D _CameraDepthTexture;

	ps_input vert (vs_input v)
	{
		ps_input o;
		o.pos = UnityObjectToClipPos (v.vertex);	
		o.screen = ComputeScreenPos(o.pos);
		COMPUTE_EYEDEPTH(o.screen.z);
		return o;
	}

	float4 frag (ps_input i) : COLOR
	{
		// superlame: manual depth test needed as we can't bind depth, FIXME for 4.x
		// alternatively implement SM > 3 version where we write out custom depth

		float d = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen)));
		d = LinearEyeDepth(d);
		
		clip(d - i.screen.z + 1e-2f);
		return float4(0, 0, 0, 0);
	}

	ENDCG

	}
}

Fallback Off
}
