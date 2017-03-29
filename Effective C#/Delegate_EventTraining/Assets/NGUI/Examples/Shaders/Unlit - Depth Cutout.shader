Shader "Unlit/Depth Cutout"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100
	
		Tags
		{
			"Queue" = "Background"
			"IgnoreProjector" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			Blend Off
			ColorMask 0
			ZWrite On
			ZTest Less
			AlphaTest Greater .99
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}