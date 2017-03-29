Shader "Transparent/Refractive"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_BumpMap ("Normal Map (RGB)", 2D) = "bump" {}
		_Mask ("Specularity (R), Shininess (G), Refraction (B)", 2D) = "black" {}
		_Color ("Color Tint", Color) = (1,1,1,1)
		_Specular ("Specular Color", Color) = (0,0,0,0)
		_Focus ("Focus", Range(-100.0, 100.0)) = -100.0
		_Shininess ("Shininess", Range(0.01, 1.0)) = 0.2
	}

	Category
	{
		Tags
		{
			"Queue" = "Transparent+1"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		SubShader
		{
			LOD 500

			GrabPass
			{
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}

			Cull Off
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater 0

			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma surface surf PPL alpha
			#include "UnityCG.cginc"

			sampler2D _GrabTexture;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _Mask;

			fixed4 _Color;
			fixed4 _Specular;
			half4 _GrabTexture_TexelSize;
			half _Focus;
			half _Shininess;

			struct Input
			{
				float4 position : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float4 color : COLOR;
				float4 proj : TEXCOORD1;
			};

			void vert (inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.position = mul(UNITY_MATRIX_MVP, v.vertex);
				
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
				o.proj.xy = (float2(o.position.x, o.position.y * scale) + o.position.w) * 0.5;
				o.proj.zw = o.position.zw;
			}

			void surf (Input IN, inout SurfaceOutput o)
			{
				half4 tex	= tex2D(_MainTex, IN.uv_MainTex);
				half3 nm	= UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
				half3 mask	= tex2D(_Mask, IN.uv_MainTex);

				float2 offset = nm.xy * _GrabTexture_TexelSize.xy * _Focus;
				IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
				half4 ref = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(IN.proj));
				
				half4 col;
				col.rgb = lerp(IN.color.rgb * tex.rgb, _Color.rgb * ref.rgb, mask.b);
				col.a = IN.color.a * _Color.a * tex.a;

				o.Albedo = col.rgb;
				o.Normal = nm;
				o.Specular = mask.r;
				o.Gloss = _Shininess * mask.g;
				o.Alpha = col.a;
			}

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
				c.rgb = (s.Albedo * diffuseFactor + _Specular.rgb * specularFactor) * _LightColor0.rgb;
				c.rgb *= (atten * 2.0);
				c.a = s.Alpha;
				return c;
			}

			ENDCG
		}
		
		SubShader
		{
			LOD 400

			Cull Off
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater 0

			CGPROGRAM
			#pragma surface surf PPL alpha
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _Mask;

			float4 _Color;
			float4 _Specular;
			float _Shininess;

			struct Input
			{
				float4 position : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float4 color : COLOR;
			};

			void surf (Input IN, inout SurfaceOutput o)
			{
				half4 tex	= tex2D(_MainTex, IN.uv_MainTex);
				half3 nm	= UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
				half3 mask	= tex2D(_Mask, IN.uv_MainTex);

				half4 col;
				col.rgb = IN.color.rgb * tex.rgb;
				col.rgb = lerp(col.rgb, _Color.rgb, mask.b * 0.5);
				col.a = IN.color.a * _Color.a * tex.a;

				o.Albedo = col.rgb;
				o.Normal = nm;
				o.Specular = mask.r;
				o.Gloss = _Shininess * mask.g;
				o.Alpha = col.a;
			}

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
				c.rgb = (s.Albedo * diffuseFactor + _Specular.rgb * specularFactor) * _LightColor0.rgb;
				c.rgb *= (atten * 2.0);
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}
		
		SubShader
		{
			LOD 300

			Cull Off
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			AlphaTest Greater 0

			CGPROGRAM
			#pragma surface surf BlinnPhong alpha
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;

			float4 _Color;

			struct Input
			{
				float4 position : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float4 color : COLOR;
			};

			void surf (Input IN, inout SurfaceOutput o)
			{
				half4 tex	= tex2D(_MainTex, IN.uv_MainTex);
				half3 mask	= tex2D(_Mask, IN.uv_MainTex);

				half4 col;
				col.rgb = IN.color.rgb * tex.rgb;
				col.rgb = lerp(col.rgb, _Color.rgb, mask.b * 0.5);
				col.a = IN.color.a * _Color.a * tex.a;

				o.Albedo = col.rgb;
				o.Alpha = col.a;
			}
			ENDCG
		}
		
		SubShader
		{
			LOD 100
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			
			Pass
			{
				ColorMaterial AmbientAndDiffuse
				
				SetTexture [_MainTex]
				{
					Combine Texture * Primary
				}
			}
		}
	}
	Fallback Off
}
