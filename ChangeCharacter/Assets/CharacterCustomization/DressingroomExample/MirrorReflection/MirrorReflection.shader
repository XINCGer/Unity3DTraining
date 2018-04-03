Shader "FX/Mirror Reflection" { 
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _ReflectionTex ("Reflection", 2D) = "white" { TexGen ObjectLinear }
}

// two texture cards: full thing
Subshader { 
    Pass {
        SetTexture[_MainTex] { combine texture }
        SetTexture[_ReflectionTex] { matrix [_ProjMatrix] combine texture * previous }
    }
}

// fallback: just main texture
Subshader {
    Pass {
        SetTexture [_MainTex] { combine texture }
    }
}

}
