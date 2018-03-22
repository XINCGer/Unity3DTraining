// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Character skin shader
// Includes falloff shadow

#define ENABLE_CAST_SHADOWS

// Material parameters
float4 _Color;
float4 _ShadowColor;
float4 _LightColor0;
float4 _MainTex_ST;

// Textures
sampler2D _MainTex;
sampler2D _FalloffSampler;
sampler2D _RimLightSampler;

// Constants
#define FALLOFF_POWER 1.0

#ifdef ENABLE_CAST_SHADOWS

// Structure from vertex shader to fragment shader
struct v2f
{
	float4 pos    : SV_POSITION;
	LIGHTING_COORDS( 0, 1 )
	float3 normal : TEXCOORD2;
	float2 uv     : TEXCOORD3;
	float3 eyeDir : TEXCOORD4;
	float3 lightDir : TEXCOORD5;
};

#else
	
// Structure from vertex shader to fragment shader
struct v2f
{
	float4 pos    : SV_POSITION;
	float3 normal : TEXCOORD0;
	float2 uv     : TEXCOORD1;
	float3 eyeDir : TEXCOORD2;
	float3 lightDir : TEXCOORD3;
};
	
#endif

// Float types
#define float_t  half
#define float2_t half2
#define float3_t half3
#define float4_t half4

// Vertex shader
v2f vert( appdata_base v )
{
	v2f o;
	o.pos = UnityObjectToClipPos( v.vertex );
	o.uv = TRANSFORM_TEX( v.texcoord.xy, _MainTex );
	o.normal = normalize( mul( unity_ObjectToWorld, float4_t( v.normal, 0 ) ).xyz );
	
	// Eye direction vector
	float4_t worldPos =  mul( unity_ObjectToWorld, v.vertex );
	o.eyeDir = normalize( _WorldSpaceCameraPos - worldPos );

	o.lightDir = WorldSpaceLightDir( v.vertex );

#ifdef ENABLE_CAST_SHADOWS
	TRANSFER_VERTEX_TO_FRAGMENT( o );
#endif

	return o;
}

// Fragment shader
float4 frag( v2f i ) : COLOR
{
	float4_t diffSamplerColor = tex2D( _MainTex, i.uv );

	// Falloff. Convert the angle between the normal and the camera direction into a lookup for the gradient
	float_t normalDotEye = dot( i.normal, i.eyeDir );
	float_t falloffU = clamp( 1 - abs( normalDotEye ), 0.02, 0.98 );
	float4_t falloffSamplerColor = FALLOFF_POWER * tex2D( _FalloffSampler, float2( falloffU, 0.25f ) );
	float3_t combinedColor = lerp( diffSamplerColor.rgb, falloffSamplerColor.rgb * diffSamplerColor.rgb, falloffSamplerColor.a );

	// Rimlight
	float_t rimlightDot = saturate( 0.5 * ( dot( i.normal, i.lightDir ) + 1.0 ) );
	falloffU = saturate( rimlightDot * falloffU );
	//falloffU = saturate( ( rimlightDot * falloffU - 0.5 ) * 32.0 );
	falloffU = tex2D( _RimLightSampler, float2( falloffU, 0.25f ) ).r;
	float3_t lightColor = diffSamplerColor.rgb * 0.5; // * 2.0;
	combinedColor += falloffU * lightColor;

#ifdef ENABLE_CAST_SHADOWS
	// Cast shadows
	float3_t shadowColor = _ShadowColor.rgb * combinedColor;
	float_t attenuation = saturate( 2.0 * LIGHT_ATTENUATION( i ) - 1.0 );
	combinedColor = lerp( shadowColor, combinedColor, attenuation );
#endif

	return float4_t( combinedColor, diffSamplerColor.a ) * _Color * _LightColor0;
}
