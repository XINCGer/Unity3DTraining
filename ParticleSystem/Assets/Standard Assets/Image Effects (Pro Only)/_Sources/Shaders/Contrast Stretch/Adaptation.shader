// Calculates adaptation to minimum/maximum luminance values,
// based on "currently adapted" and "new values to adapt to"
// textures (both 1x1).

Shader "Hidden/Contrast Stretch Adaptation" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_CurTex ("Base (RGB)", 2D) = "white" {}
}

Category {
	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
				
CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex; // currently adapted to
uniform sampler2D _CurTex; // new value to adapt to
uniform float4 _AdaptParams; // x=adaptLerp, y=limitMinimum, z=limitMaximum

float4 frag (v2f_img i) : COLOR  {
	// value is: max, min
	float2 valAdapted = tex2D(_MainTex, i.uv).xy;
	float2 valCur = tex2D(_CurTex, i.uv).xy;
	
	// Calculate new adapted values: interpolate
	// from valAdapted to valCur by script-supplied amount.
	//
	// Because we store adaptation levels in a simple 8 bit/channel
	// texture, we might not have enough precision - the interpolation
	// amount might be too small to change anything, and we'll never
	// arrive at the needed values.
	//
	// So we make sure the change we do is at least 1/255th of the
	// color range - this way we'll always change the value.
	const float kMinChange = 1.0/255.0;
	float2 delta = (valCur-valAdapted) * _AdaptParams.x;
	delta.x = sign(delta.x) * max( kMinChange, abs(delta.x) );
	delta.y = sign(delta.y) * max( kMinChange, abs(delta.y) );

	float4 valNew;
	valNew.xy = valAdapted + delta;
	
	// Impose user limits on maximum/minimum values
	valNew.x = max( valNew.x, _AdaptParams.z );
	valNew.y = min( valNew.y, _AdaptParams.y );
	
	// Optimization so that our final apply pass is faster:
	// z = max-min (plus a small amount to prevent division by zero)
	valNew.z = valNew.x - valNew.y + 0.01;
	// w = min/(max-min)
	valNew.w = valNew.y / valNew.z;
	
	return valNew;
}
ENDCG

		}
	}
}

Fallback off

}