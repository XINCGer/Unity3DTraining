Shader "NGUI/Examples/Orc Skin"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Diffuse (RGB), Specular (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	// Good quality or above
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf PPL

		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _Color;
		float _Shininess;

		struct Input
		{
			float2 uv_MainTex;
		};

		// Forward lighting
		half4 LightingPPL (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 nNormal = normalize(s.Normal);
			half shininess = s.Gloss * 250.0 + 4.0;

		#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
		#endif

			// Phong shading model
			//half reflectiveFactor = max(0.0, dot(-viewDir, reflect(lightDir, nNormal)));

			// Blinn-Phong shading model
			half reflectiveFactor = max(0.0, dot(nNormal, normalize(lightDir + viewDir)));

			half diffuseFactor = max(0.0, dot(nNormal, lightDir));
			half specularFactor = pow(reflectiveFactor, shininess) * s.Specular;

			half4 c;
			c.rgb = (s.Albedo * diffuseFactor + _SpecColor.rgb * specularFactor) * _LightColor0.rgb;
			c.rgb *= (atten * 2.0);
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 tex 	= tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo 	= tex.rgb;
			o.Alpha 	= _Color.a;
			o.Normal	= UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Specular 	= tex.a;
			o.Gloss 	= _Shininess;
		}
		ENDCG
	}
	
	// Simple quality -- drop the normal map
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf PPL

		sampler2D _MainTex;
		fixed4 _Color;
		float _Shininess;

		struct Input
		{
			float2 uv_MainTex;
		};

		// Forward lighting
		half4 LightingPPL (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 nNormal = normalize(s.Normal);
			half shininess = s.Gloss * 250.0 + 4.0;

		#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
		#endif

			// Phong shading model
			half reflectiveFactor = max(0.0, dot(-viewDir, reflect(lightDir, nNormal)));

			// Blinn-Phong shading model
			//half reflectiveFactor = max(0.0, dot(nNormal, normalize(lightDir + viewDir)));

			half diffuseFactor = max(0.0, dot(nNormal, lightDir));
			half specularFactor = pow(reflectiveFactor, shininess) * s.Specular;

			half4 c;
			c.rgb = (s.Albedo * diffuseFactor + _SpecColor.rgb * specularFactor) * _LightColor0.rgb;
			c.rgb *= (atten * 2.0);
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 tex 	= tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo 	= tex.rgb;
			o.Alpha 	= _Color.a;
			o.Specular 	= tex.a;
			o.Gloss 	= _Shininess;
		}
		ENDCG
	}
	Fallback "Diffuse"
}