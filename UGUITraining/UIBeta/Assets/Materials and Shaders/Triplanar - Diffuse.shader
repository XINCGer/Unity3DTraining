#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Triplanar/Diffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TilingMain ("UV Tiling", Float) = 1.0
	}
   
	SubShader
	{
		LOD 200
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		fixed4 _Color;
		float _TilingMain;

		struct Input
		{
			float3 objPos;
			float3 objNormal;
		};

		void vert (inout appdata_full v, out Input o)
		{
			o.objPos = (v.vertex.xyz / v.vertex.w) * (_TilingMain / 1.0);
			o.objNormal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			float3 contribution = normalize(IN.objNormal);
			contribution *= contribution;
			contribution *= contribution;
			contribution *= contribution;
			contribution  = contribution / (contribution.x + contribution.y + contribution.z);

			half4 c = 	tex2D(_MainTex, IN.objPos.yx) * contribution.z +
						tex2D(_MainTex, IN.objPos.xz) * contribution.y +
						tex2D(_MainTex, IN.objPos.yz) * contribution.x;

			c *= _Color;

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "VertexLit"
}
