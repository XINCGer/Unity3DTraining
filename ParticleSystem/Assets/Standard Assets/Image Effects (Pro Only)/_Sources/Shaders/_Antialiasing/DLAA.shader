// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


//
// modified and adapted DLAA code based on Dmitry Andreev's
// Directionally Localized Anti-Aliasing (DLAA)
//
// as seen in "The Force Unleashed 2"
//

Shader "Hidden/DLAA" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

CGINCLUDE
	
	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	#define LD( o, dx, dy ) o = tex2D( _MainTex, texCoord + float2( dx, dy ) * _MainTex_TexelSize.xy );
	
	float GetIntensity( float3 col )
	{
    	return dot( col, float3( 0.33f, 0.33f, 0.33f ) );
	}	
	
	float4 highPassPre( float2 texCoord )
	{
	 	LD(float4 sCenter, 0.0,0.0)
	 	LD(float4 sUpLeft, -1.0,-1.0)
	 	LD(float4 sUpRight, 1.0,-1.0)
	 	LD(float4 sDownLeft, -1.0,1.0)
	 	LD(float4 sDownRight, 1.0,1.0)
	 
		float4 diff		= 4.0f * abs( (sUpLeft + sUpRight + sDownLeft + sDownRight) - 4.0f * sCenter );
		float edgeMask	= GetIntensity(diff.xyz);
	
		return float4(sCenter.rgb, edgeMask);
	}
	
	// Softer (5-pixel wide high-pass)
	/*
	void HighPassEdgeHV (out float4 edge_h, out float4 edge_v, float4 center, float4 w_h, float4 w_v, float2 texCoord) {
        edge_h = abs( w_h - 4.0f * center ) / 4.0f;
        edge_v = abs( w_v - 4.0f * center ) / 4.0f;		
	}

	// Sharper (3-pixel wide high-pass)	
	void EdgeHV (out float4 edge_h, out float4 edge_v, float4 center, float2 texCoord) {
        float4 left, right, top, bottom;

        LD( left,  -1,  0 )
        LD( right,  1,  0 )
        LD( top,    0, -1 )
        LD( bottom, 0,  1 )
        
        edge_h = abs( left + right - 2.0f * center ) / 2.0f;
        edge_v = abs( top + bottom - 2.0f * center ) / 2.0f;		
	} 
	*/
	
	float4 edgeDetectAndBlur(  float2 texCoord )
	{
	float lambda = 3.0f;
	float epsilon = 0.1f;
    
    //
    // Short Edges
    //
    
    float4 center, left_01, right_01, top_01, bottom_01;

    // sample 5x5 cross    
    LD( center,      0,   0 )
    LD( left_01,  -1.5,   0 )
    LD( right_01,  1.5,   0 )
    LD( top_01,      0,-1.5 )
    LD( bottom_01,   0, 1.5 )


    float4 w_h = 2.0f * ( left_01 + right_01 );
    float4 w_v = 2.0f * ( top_01 + bottom_01 );

    
        // Softer (5-pixel wide high-pass)
        float4 edge_h = abs( w_h - 4.0f * center ) / 4.0f;
        float4 edge_v = abs( w_v - 4.0f * center ) / 4.0f;
        

    float4 blurred_h = ( w_h + 2.0f * center ) / 6.0f;
    float4 blurred_v = ( w_v + 2.0f * center ) / 6.0f;

    float edge_h_lum = GetIntensity( edge_h.xyz );
    float edge_v_lum = GetIntensity( edge_v.xyz );
    float blurred_h_lum = GetIntensity( blurred_h.xyz );
    float blurred_v_lum = GetIntensity( blurred_v.xyz );

    float edge_mask_h = saturate( ( lambda * edge_h_lum - epsilon ) / blurred_v_lum );
    float edge_mask_v = saturate( ( lambda * edge_v_lum - epsilon ) / blurred_h_lum );

    float4 clr = center;
    clr = lerp( clr, blurred_h, edge_mask_v );
    clr = lerp( clr, blurred_v, edge_mask_h ); // blurrier version

    //
    // Long Edges
    //
    
    float4 h0, h1, h2, h3, h4, h5, h6, h7;
    float4 v0, v1, v2, v3, v4, v5, v6, v7;

    // sample 16x16 cross (sparse-sample on X360, incremental kernel update on SPUs)
    LD( h0, 1.5, 0 ) LD( h1, 3.5, 0 ) LD( h2, 5.5, 0 ) LD( h3, 7.5, 0 ) LD( h4, -1.5,0 ) LD( h5, -3.5,0 ) LD( h6, -5.5,0 ) LD( h7, -7.5,0 )
    LD( v0, 0, 1.5 ) LD( v1, 0, 3.5 ) LD( v2, 0, 5.5 ) LD( v3, 0, 7.5 ) LD( v4, 0,-1.5 ) LD( v5, 0,-3.5 ) LD( v6, 0,-5.5 ) LD( v7, 0,-7.5 )

    float long_edge_mask_h = ( h0.a + h1.a + h2.a + h3.a + h4.a + h5.a + h6.a + h7.a ) / 8.0f;
    float long_edge_mask_v = ( v0.a + v1.a + v2.a + v3.a + v4.a + v5.a + v6.a + v7.a ) / 8.0f;

    long_edge_mask_h = saturate( long_edge_mask_h * 2.0f - 1.0f );
    long_edge_mask_v = saturate( long_edge_mask_v * 2.0f - 1.0f );

        float4 left, right, top, bottom;

        LD( left,  -1,  0 )
        LD( right,  1,  0 )
        LD( top,    0, -1 )
        LD( bottom, 0,  1 )

    if ( long_edge_mask_h > 0 || long_edge_mask_v > 0 ) // faster but less resistant to noise (TFU2 X360)
    //if ( abs( long_edge_mask_h - long_edge_mask_v ) > 0.2f ) // resistant to noise (TFU2 SPUs)
    {
        float4 long_blurred_h = ( h0 + h1 + h2 + h3 + h4 + h5 + h6 + h7 ) / 8.0f;
        float4 long_blurred_v = ( v0 + v1 + v2 + v3 + v4 + v5 + v6 + v7 ) / 8.0f;

        float lb_h_lum   = GetIntensity( long_blurred_h.xyz );
        float lb_v_lum   = GetIntensity( long_blurred_v.xyz );

        float center_lum = GetIntensity( center.xyz );
        float left_lum   = GetIntensity( left.xyz );
        float right_lum  = GetIntensity( right.xyz );
        float top_lum    = GetIntensity( top.xyz );
        float bottom_lum = GetIntensity( bottom.xyz );

        float4 clr_v = center;
        float4 clr_h = center;

		// we had to hack this because DIV by 0 gives some artefacts on different platforms
        float hx = center_lum == top_lum ? 0.0 : saturate( 0 + ( lb_h_lum - top_lum    ) / ( center_lum - top_lum    ) );
        float hy = center_lum == bottom_lum ? 0.0 : saturate( 1 + ( lb_h_lum - center_lum ) / ( center_lum - bottom_lum ) );
        float vx = center_lum == left_lum ? 0.0 : saturate( 0 + ( lb_v_lum - left_lum   ) / ( center_lum - left_lum   ) );
        float vy = center_lum == right_lum ? 0.0 : saturate( 1 + ( lb_v_lum - center_lum ) / ( center_lum - right_lum  ) );

        float4 vhxy = float4( vx, vy, hx, hy );
        //vhxy = vhxy == float4( 0, 0, 0, 0 ) ? float4( 1, 1, 1, 1 ) : vhxy;

        clr_v = lerp( left  , clr_v, vhxy.x );
        clr_v = lerp( right , clr_v, vhxy.y );
        clr_h = lerp( top   , clr_h, vhxy.z );
        clr_h = lerp( bottom, clr_h, vhxy.w );

        clr = lerp( clr, clr_v, long_edge_mask_v );
        clr = lerp( clr, clr_h, long_edge_mask_h );
    }

    return clr;
  	}	

	float4 edgeDetectAndBlurSharper(float2 texCoord)
	{
	float lambda = 3.0f;
	float epsilon = 0.1f;
    
    //
    // Short Edges
    //
    
    float4 center, left_01, right_01, top_01, bottom_01;

    // sample 5x5 cross    
    LD( center,      0,   0 )
    LD( left_01,  -1.5,   0 )
    LD( right_01,  1.5,   0 )
    LD( top_01,      0,-1.5 )
    LD( bottom_01,   0, 1.5 )


    float4 w_h = 2.0f * ( left_01 + right_01 );
    float4 w_v = 2.0f * ( top_01 + bottom_01 );

        // Sharper (3-pixel wide high-pass)
        float4 left, right, top, bottom;

        LD( left,  -1,  0 )
        LD( right,  1,  0 )
        LD( top,    0, -1 )
        LD( bottom, 0,  1 )
        
        float4 edge_h = abs( left + right - 2.0f * center ) / 2.0f;
        float4 edge_v = abs( top + bottom - 2.0f * center ) / 2.0f;

    float4 blurred_h = ( w_h + 2.0f * center ) / 6.0f;
    float4 blurred_v = ( w_v + 2.0f * center ) / 6.0f;

    float edge_h_lum = GetIntensity( edge_h.xyz );
    float edge_v_lum = GetIntensity( edge_v.xyz );
    float blurred_h_lum = GetIntensity( blurred_h.xyz );
    float blurred_v_lum = GetIntensity( blurred_v.xyz );

    float edge_mask_h = saturate( ( lambda * edge_h_lum - epsilon ) / blurred_v_lum );
    float edge_mask_v = saturate( ( lambda * edge_v_lum - epsilon ) / blurred_h_lum );

    float4 clr = center;
    clr = lerp( clr, blurred_h, edge_mask_v );
    clr = lerp( clr, blurred_v, edge_mask_h * 0.5f ); // TFU2 uses 1.0f instead of 0.5f

    //
    // Long Edges
    //
    
    float4 h0, h1, h2, h3, h4, h5, h6, h7;
    float4 v0, v1, v2, v3, v4, v5, v6, v7;

    // sample 16x16 cross (sparse-sample on X360, incremental kernel update on SPUs)
    LD( h0, 1.5, 0 ) LD( h1, 3.5, 0 ) LD( h2, 5.5, 0 ) LD( h3, 7.5, 0 ) LD( h4, -1.5,0 ) LD( h5, -3.5,0 ) LD( h6, -5.5,0 ) LD( h7, -7.5,0 )
    LD( v0, 0, 1.5 ) LD( v1, 0, 3.5 ) LD( v2, 0, 5.5 ) LD( v3, 0, 7.5 ) LD( v4, 0,-1.5 ) LD( v5, 0,-3.5 ) LD( v6, 0,-5.5 ) LD( v7, 0,-7.5 )

    float long_edge_mask_h = ( h0.a + h1.a + h2.a + h3.a + h4.a + h5.a + h6.a + h7.a ) / 8.0f;
    float long_edge_mask_v = ( v0.a + v1.a + v2.a + v3.a + v4.a + v5.a + v6.a + v7.a ) / 8.0f;

    long_edge_mask_h = saturate( long_edge_mask_h * 2.0f - 1.0f );
    long_edge_mask_v = saturate( long_edge_mask_v * 2.0f - 1.0f );

    //if ( long_edge_mask_h > 0 || long_edge_mask_v > 0 ) // faster but less resistant to noise (TFU2 X360)
    if ( abs( long_edge_mask_h - long_edge_mask_v ) > 0.2f ) // resistant to noise (TFU2 SPUs)
    {
        float4 long_blurred_h = ( h0 + h1 + h2 + h3 + h4 + h5 + h6 + h7 ) / 8.0f;
        float4 long_blurred_v = ( v0 + v1 + v2 + v3 + v4 + v5 + v6 + v7 ) / 8.0f;

        float lb_h_lum   = GetIntensity( long_blurred_h.xyz );
        float lb_v_lum   = GetIntensity( long_blurred_v.xyz );

        float center_lum = GetIntensity( center.xyz );
        float left_lum   = GetIntensity( left.xyz );
        float right_lum  = GetIntensity( right.xyz );
        float top_lum    = GetIntensity( top.xyz );
        float bottom_lum = GetIntensity( bottom.xyz );
 
        float4 clr_v = center;
        float4 clr_h = center;

		// we had to hack this because DIV by 0 gives some artefacts on different platforms
        float hx = center_lum == top_lum ? 0.0 : saturate( 0 + ( lb_h_lum - top_lum    ) / ( center_lum - top_lum    ) );
        float hy = center_lum == bottom_lum ? 0.0 : saturate( 1 + ( lb_h_lum - center_lum ) / ( center_lum - bottom_lum ) );
        float vx = center_lum == left_lum ? 0.0 : saturate( 0 + ( lb_v_lum - left_lum   ) / ( center_lum - left_lum   ) );
        float vy = center_lum == right_lum ? 0.0 : saturate( 1 + ( lb_v_lum - center_lum ) / ( center_lum - right_lum  ) );

        float4 vhxy = float4( vx, vy, hx, hy );
        //vhxy = vhxy == float4( 0, 0, 0, 0 ) ? float4( 1, 1, 1, 1 ) : vhxy;

        clr_v = lerp( left  , clr_v, vhxy.x );
        clr_v = lerp( right , clr_v, vhxy.y );
        clr_h = lerp( top   , clr_h, vhxy.z );
        clr_h = lerp( bottom, clr_h, vhxy.w );

        clr = lerp( clr, clr_v, long_edge_mask_v );
        clr = lerp( clr, clr_h, long_edge_mask_h );
    }

    return clr;
  	}	


	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv.xy = uv;
		
		return o;
	}

	half4 fragFirst (v2f i) : COLOR {		 	 	    
		return highPassPre (i.uv);
	}
	
	half4 fragSecond (v2f i) : COLOR {		 	 	    
	    return edgeDetectAndBlur( i.uv );
	}

	half4 fragThird (v2f i) : COLOR {		 	 	    
	    return edgeDetectAndBlurSharper( i.uv );
	}
			
ENDCG	

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
	
		CGPROGRAM
	
		#pragma vertex vert
		#pragma fragment fragFirst
		//#pragma fragmentoption ARB_precision_hint_fastest 
        #pragma exclude_renderers d3d11_9x
        #pragma glsl
		
		ENDCG
	}
	
	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
	
		CGPROGRAM
	
		#pragma vertex vert
		#pragma fragment fragSecond
		//#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma target 3.0
        #pragma exclude_renderers d3d11_9x
        #pragma glsl
		
		ENDCG
	}

	Pass {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
	
		CGPROGRAM
	
		#pragma vertex vert
		#pragma fragment fragThird
		//#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma target 3.0
        #pragma exclude_renderers d3d11_9x
        #pragma glsl
		
		ENDCG
	}	
}

Fallback off

}