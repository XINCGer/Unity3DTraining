//Copyright (c) 2014 Qualcomm Connected Experiences, Inc.
//All Rights Reserved.
//Confidential and Proprietary - Protected under copyright and other laws.
Shader "Custom/VideoBackground" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue"="geometry-11" "RenderType"="opaque" }
        Pass {
            ZWrite Off
            Cull Off
            Lighting Off
			
			Stencil {
                Ref 250
                Comp Always
                Pass Replace
            }
            
            SetTexture [_MainTex] {
                combine texture 
            }
        }
    } 
    FallBack "Diffuse"
}
