// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


/*
	DX11 Depth Of Field
	pretty much just does bokeh texture splatting

	basic algorithm:

	* find bright spots
	* verify high frequency (otherwise dont care)
	* if possitive, replace with black pixel and add to append buffer
	* box blur buffer (thus smearing black pixels)
	* blend bokeh texture sprites via append buffer on top of box blurred buffer
	* composite with frame buffer
*/

Shader "Hidden/Dof/DX11Dof" 
{
	Properties 
	{
		_MainTex ("", 2D) = "white" {}
		_BlurredColor ("", 2D) = "white" {}
		_FgCocMask ("", 2D) = "white" {}
	}

	CGINCLUDE

	#define BOKEH_ZERO_VEC (float4(0,0,0,0))
	#define BOKEH_ONE_VEC (float4(1,1,1,1))

	float4 _BokehParams; // legend: dx11BokehScale, dx11BokehIntensity,dx11BokehThreshhold, internalBlurWidth
	float4 _MainTex_TexelSize;
	float3 _Screen;
	float _SpawnHeuristic;

	sampler2D _CameraDepthTexture;
	sampler2D _BlurredColor;
	sampler2D _MainTex;
	sampler2D _FgCocMask;

	struct appendStruct {
		float3 pos;
		float4 color;
	};

	struct gs_out {
		float4 pos : SV_POSITION; 
		float3 uv : TEXCOORD0;
		float4 color : TEXCOORD1;
		float4 misc : TEXCOORD2;
	};	

	// TODO: activate border clamp tex sampler state instead?
	inline float4 clampBorderColor(float2 uv)
	{
#if 1
		if(uv.x<=0) return BOKEH_ZERO_VEC; if(uv.x>=1) return BOKEH_ZERO_VEC;
		if(uv.y<=0) return BOKEH_ZERO_VEC; if(uv.y>=1) return BOKEH_ZERO_VEC;
#endif	
		return BOKEH_ONE_VEC;
	}

	struct vs_out {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 color : TEXCOORD1;
		float cocOverlap : TEXCOORD2;
	};

	StructuredBuffer<appendStruct> pointBuffer;

	vs_out vertApply (uint id : SV_VertexID)
	{
		vs_out o;
		float2 pos = pointBuffer[id].pos.xy ;
		o.pos = float4(pos * 2.0 - 1.0, 0, 1);
		o.color =  pointBuffer[id].color;
		#if UNITY_UV_STARTS_AT_TOP
			o.pos.y *= -1;
		#endif
		o.cocOverlap = pointBuffer[id].pos.z;

		return o; 
	}

	[maxvertexcount(4)]
	void geom (point vs_out input[1], inout TriangleStream<gs_out> outStream)
	{
		// NEW ENERGY CONSERVATION:

		float2 scale2 = _BokehParams.ww * input[0].color.aa * _BokehParams.xx;
		float4 offs = 0;
		offs.xy = float2(3.0, 3.0) + 2.0f * floor(scale2 + float2(0.5,0.5));

		float2 rs = ((float2(1.0, 1.0) + 2.0f * (scale2 + float2(0.5,0.5))));;
		float2 f2 = offs.xy / rs;

		float energyAdjustment = (_BokehParams.y) / (rs.x*rs.y);
		offs.xy *= _Screen.xy;

		gs_out output;

		output.pos = input[0].pos + offs*float4(-1,1,0,0);
		output.misc = float4(f2,0,0);
		output.uv = float3(0, 1, input[0].cocOverlap); 		
		output.color = input[0].color * energyAdjustment;
		outStream.Append (output);

		output.pos = input[0].pos + offs*float4(1,1,0,0); 
		output.misc = float4(f2,0,0);
		output.uv = float3(1, 1, input[0].cocOverlap); 
		output.color = input[0].color * energyAdjustment;
		outStream.Append (output);

		output.pos = input[0].pos + offs*float4(-1,-1,0,0);
		output.misc = float4(f2,0,0);
		output.uv = float3(0, 0, input[0].cocOverlap); 
		output.color = input[0].color * energyAdjustment;
		outStream.Append (output);

		output.pos = input[0].pos + offs*float4(1,-1,0,0);
		output.misc = float4(f2,0,0);
		output.uv = float3(1, 0, input[0].cocOverlap); 
		output.color = input[0].color * energyAdjustment;
		outStream.Append (output);

		outStream.RestartStrip();
	}	

ENDCG

SubShader 
{

// pass 0: append buffer "collect"

Pass
{
	ZWrite Off ZTest Always Cull Off Fog { Mode Off }

	CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
	#pragma target 5.0

	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv_flip : TEXCOORD0;
		float2 uv : TEXCOORD1;		
	};

	v2f vert (appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv = v.texcoord;
		o.uv_flip = v.texcoord;
		#if UNITY_UV_STARTS_AT_TOP
		if(_MainTex_TexelSize.y<0)		
			o.uv_flip.y = 1.0-o.uv_flip.y;
		if(_MainTex_TexelSize.y<0)		
			o.pos.y *= -1.0;			
		#endif
		return o;
	}

	AppendStructuredBuffer<appendStruct> pointBufferOutput : register(u1);

	float4 frag (v2f i) : COLOR0
	{
		float4 c = tex2D (_MainTex, i.uv_flip);
		float lumc = Luminance (c.rgb);

		float4 cblurred = tex2D (_BlurredColor, i.uv);
		float lumblurred = Luminance (cblurred.rgb);

		float fgCoc = tex2D(_FgCocMask, i.uv).a;

		[branch]
		if (c.a * _BokehParams.w > 1 && cblurred.a > 0.1 && lumc > _BokehParams.z && abs(lumc-lumblurred) > _SpawnHeuristic)
		{
			appendStruct append;
			append.pos = float3(i.uv, fgCoc);
			append.color.rgba = float4(c.rgb * saturate(c.a*4), c.a);
			pointBufferOutput.Append (append);
			return float4(c.rgb * saturate(1-c.a*4), c.a);
		}

		return c;
	}
	ENDCG
}

// pass 1: bokeh splatting (low resolution)

Pass {

	ZWrite Off ZTest Always Cull Off Fog { Mode Off }
	Blend One One, One One
	ColorMask RGBA

	CGPROGRAM

	#pragma target 5.0
	#pragma vertex vertApply
	#pragma geometry geom
	#pragma fragment frag

	#include "UnityCG.cginc"

	fixed4 frag (gs_out i) : COLOR0
	{
		float2 uv = (i.uv.xy) * i.misc.xy + (float2(1,1)-i.misc.xy) * 0.5;	// smooth uv scale
		return float4(i.color.rgb, 1) * float4(tex2D(_MainTex, uv.xy).rgb, i.uv.z) * clampBorderColor (uv);
	}

	ENDCG
}

// pass 2: bokeh splatting (high resolution)

Pass {

	ZWrite Off ZTest Always Cull Off Fog { Mode Off }
	BlendOp Add, Add
	Blend DstAlpha One, Zero One
	ColorMask RGBA

	CGPROGRAM

	#pragma target 5.0
	#pragma vertex vertApply
	#pragma geometry geom
	#pragma fragment frag

	#include "UnityCG.cginc"

	fixed4 frag (gs_out i) : COLOR0
	{
		float2 uv = (i.uv.xy) * i.misc.xy + (float2(1,1)-i.misc.xy) * 0.5;	// smooth uv scale
		return float4(i.color.rgb, 1) * float4(tex2D(_MainTex, uv.xy).rgb, i.uv.z) * clampBorderColor (uv);
	}

	ENDCG
}

}

Fallback Off
}