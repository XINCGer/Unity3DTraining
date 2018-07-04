Shader "Custom/Rim Lit Bumped Specular" {
	Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
      _Shininess ("Shininess", Range(0.03,1)) = 0.078125
      _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
      _BumpMap ("Normal Map", 2D) = "bump" {}
      _RimColor ("Rim Color", Color) = (0.2,0.2,0.2,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf BlinnPhong
      
      fixed4 _Color, SpecColor;
      half _Shininess;
         
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
      };
      sampler2D _MainTex;
      sampler2D _BumpMap;
      float4 _RimColor;
      float _RimPower;
      void surf (Input IN, inout SurfaceOutput o) {
  		  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);  
          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          o.Gloss = tex.a;
          o.Alpha = tex.a * _Color.a;
          o.Specular = _Shininess;
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      ENDCG
    } 
    Fallback "Specular"
  }