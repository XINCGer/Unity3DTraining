
Shader "Hidden/Dof/Bokeh34" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Source ("Base (RGB)", 2D) = "black" {}
}

SubShader {
	CGINCLUDE

	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _Source;
	
	uniform half4 _ArScale;		
	uniform half _Intensity; 
	uniform half4 _Source_TexelSize;
	
	struct v2f {
		half4 pos : POSITION;
		half2 uv2 : TEXCOORD0;
		half4 source : TEXCOORD1;
	};
	
	#define COC bokeh.a
	
	v2f vert (appdata_full v)
	{
		v2f o;
		
		o.pos = v.vertex; 
				
		o.uv2.xy = v.texcoord.xy;// * 2.0; <- needed when using Triangles.js and not Quads.js
		
		#if UNITY_UV_STARTS_AT_TOP
			float4 bokeh = tex2Dlod (_Source, half4 (v.texcoord1.xy * half2(1,-1) + half2(0,1), 0, 0));
		#else
			float4 bokeh = tex2Dlod (_Source, half4 (v.texcoord1.xy, 0, 0));
		#endif
		
		o.source = bokeh;			

		o.pos.xy += (v.texcoord.xy * 2.0 - 1.0) * _ArScale.xy * COC;// + _ArScale.zw * coc;
		o.source.rgb *= _Intensity;		
								
		return o;
	}
	
	
	half4 frag (v2f i) : COLOR 
	{
		half4 color = tex2D (_MainTex, i.uv2.xy);
		color.rgb *= i.source.rgb;	
		color.a *= Luminance(i.source.rgb*0.25);
		return color;
	}
	
	ENDCG

	Pass {
		Blend OneMinusDstColor One 
		ZTest Always Cull Off ZWrite Off

				Fog { Mode off }

		CGPROGRAM
		
		#pragma glsl
		#pragma target 3.0
		#pragma exclude_renderers d3d11_9x
		
		#pragma vertex vert
		#pragma fragment frag
		
		ENDCG
	}

}

Fallback off

}