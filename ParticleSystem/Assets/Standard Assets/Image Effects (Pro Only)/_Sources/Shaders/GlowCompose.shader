// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GlowCompose" {

Properties {
	_Color ("Glow Amount", Color) = (1,1,1,1)
	_MainTex ("", 2D) = "white" {}
}

Category {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }
	Blend One One

	Subshader {
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"

				struct v2f {
					float4 pos : POSITION;
					half2 uv : TEXCOORD0;
				};

				float4 _MainTex_TexelSize;
				float4 _BlurOffsets;

				v2f vert (appdata_img v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
					return o;
				}

				sampler2D _MainTex;
				fixed4 _Color;

				fixed4 frag( v2f i ) : COLOR
				{
					return 2.0f * _Color * tex2D( _MainTex, i.uv );
				}
			ENDCG
		}
	}

	SubShader {
		Pass {
			SetTexture [_MainTex] {constantColor [_Color] combine constant * texture DOUBLE}
		}
	}
}

Fallback off

}
