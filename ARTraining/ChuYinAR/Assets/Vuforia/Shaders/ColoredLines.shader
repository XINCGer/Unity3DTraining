Shader "Custom/ColoredLines" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    
    SubShader {
        Pass { 
            Lighting Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            Color [_Color]
        }
    } 
}
