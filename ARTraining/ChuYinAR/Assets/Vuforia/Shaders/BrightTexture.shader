Shader "Custom/BrightTexture" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {

    Pass{

        CGPROGRAM

        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        sampler2D _MainTex;

        struct v2f {
            float4  pos : SV_POSITION;
            float2  uv : TEXCOORD0;
        };

        float4 _MainTex_ST;

        v2f vert (appdata_base v)
        {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
            return o;
        }

        
        half4 frag(v2f i) : COLOR
        {
            half4 c = tex2D (_MainTex, i.uv);

            float scale = 0.2f;
            c.rgb = c.rgb * scale + 1.0f - scale;

            return c;
        }

        ENDCG
    }
    }



     
    FallBack "Diffuse"
}
