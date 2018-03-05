// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Hidden/PrepareSunShaftsBlur" {
	Properties {
		_MainTex ("Base", 2D) = "" {}
		_Skybox ("Skybox", 2D) = "" {}
	}
	
	CGINCLUDE
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _Skybox; 
	sampler2D _CameraDepthTexture;
	
	uniform half _NoSkyBoxMask;
	uniform half4 _SunPosition; 
		
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}  
	
	half TransformColor (half4 skyboxValue) {
		return max (skyboxValue.a, _NoSkyBoxMask * dot (skyboxValue.rgb, float3 (0.59,0.3,0.11))); 		
	}
	
	half4 frag (v2f i) : COLOR {
		float depthSample = UNITY_SAMPLE_DEPTH( tex2D (_CameraDepthTexture, i.uv.xy) );
		half4 tex = tex2D (_MainTex, i.uv.xy);
		
		depthSample = Linear01Depth (depthSample);
		 
		// consider maximum radius
		half2 vec = _SunPosition.xy - i.uv.xy;
		half dist = saturate (_SunPosition.w - length (vec.xy));		
		
		half4 outColor = 0;
		
		// consider shafts blockers
		if (depthSample > 0.99)
			outColor = TransformColor (tex) * dist;
			
		return outColor;
	}
	
	half4 fragNoDepthNeeded (v2f i) : COLOR {
		float4 sky = (tex2D (_Skybox, i.uv.xy));
		float4 tex = (tex2D (_MainTex, i.uv.xy));
		
		// consider maximum radius
		half2 vec = _SunPosition.xy - i.uv.xy;
		half dist = saturate (_SunPosition.w - length (vec));			
		
		half4 outColor = 0;		
		
		if (Luminance ( abs(sky.rgb - tex.rgb)) < 0.2)
			outColor = TransformColor (sky) * dist;
		
		return outColor;
	}	

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      
      #pragma vertex vert
      #pragma fragment frag
      
      ENDCG
  }
  Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      
      #pragma vertex vert
      #pragma fragment fragNoDepthNeeded
      
      ENDCG
  } 
}

Fallback off
	
} // shader