Shader "iPhone/LightmapOnlyShader"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_LightMap ("Lightmap (RGB)", 2D) = "black" { LightmapMode }
	}

	SubShader
	{
		Pass
		{
			Name "BASE"
			Tags {"LightMode" = "Always"}
	
			BindChannels {
				Bind "Vertex", vertex
				Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
				Bind "texcoord", texcoord1 // main uses 1st uv
			}
			SetTexture [_LightMap] {
				constantColor [_Color]
				combine texture * constant
			}
			SetTexture [_MainTex] {
				combine texture * previous
			}
		}
	}
}