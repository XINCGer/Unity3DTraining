Shader "Custom/AlphaMask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaMask ("Mask (A)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		AlphaTest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert noambient

		sampler2D _MainTex;
		sampler2D _AlphaMask;

		struct Input {
			float2 uv_MainTex;
			float2 uv_AlphaMask;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = tex2D (_AlphaMask, IN.uv_AlphaMask).a;
			o.Specular = 0;
			o.Gloss = 0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
