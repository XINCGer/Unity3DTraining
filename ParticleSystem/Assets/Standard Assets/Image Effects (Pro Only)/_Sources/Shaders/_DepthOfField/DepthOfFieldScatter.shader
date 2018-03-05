// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Hidden/Dof/DepthOfFieldHdr" {
	Properties {
		_MainTex ("-", 2D) = "black" {}
		_FgOverlap ("-", 2D) = "black" {}
		_LowRez ("-", 2D) = "black" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
	};

	struct v2fRadius {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv1[4] : TEXCOORD1;
	};
	
	struct v2fBlur {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;
		float4 uv45 : TEXCOORD3;
		float4 uv67 : TEXCOORD4; 
		float4 uv89 : TEXCOORD5;
	};	
	
	uniform sampler2D _MainTex;
	uniform sampler2D _CameraDepthTexture;
	uniform sampler2D _FgOverlap;
	uniform sampler2D _LowRez;
	uniform float4 _CurveParams;
	uniform float4 _MainTex_TexelSize;
	uniform float4 _Offsets;

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv1.xy = v.texcoord.xy;
		o.uv.xy = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif			
		
		return o;
	} 

	v2f vertFlip( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv1.xy = v.texcoord.xy;
		o.uv.xy = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		if (_MainTex_TexelSize.y < 0)
			o.uv1.y = 1-o.uv1.y;			
		#endif			
		
		return o;
	} 

	v2fBlur vertBlurPlusMinus (appdata_img v) 
	{
		v2fBlur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * float4(1,1, -1,-1) * _MainTex_TexelSize.xyxy / 6.0;
		o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * float4(2,2, -2,-2) * _MainTex_TexelSize.xyxy / 6.0;
		o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * float4(3,3, -3,-3) * _MainTex_TexelSize.xyxy / 6.0;
		o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * float4(4,4, -4,-4) * _MainTex_TexelSize.xyxy / 6.0;
		o.uv89 =  v.texcoord.xyxy + _Offsets.xyxy * float4(5,5, -5,-5) * _MainTex_TexelSize.xyxy / 6.0;
		return o;  
	}

	#define SCATTER_OVERLAP_SMOOTH (-0.265)

	inline float BokehWeightDisc(float4 sample, float sampleDistance, float4 centerSample)
	{
		return smoothstep(SCATTER_OVERLAP_SMOOTH, 0.0, sample.a - centerSample.a*sampleDistance); 
	}

	inline float2 BokehWeightDisc2(float4 sampleA, float4 sampleB, float2 sampleDistance2, float4 centerSample)
	{
		return smoothstep(float2(SCATTER_OVERLAP_SMOOTH, SCATTER_OVERLAP_SMOOTH), float2(0.0,0.0), float2(sampleA.a, sampleB.a) - centerSample.aa*sampleDistance2);	}		
			
	static const int SmallDiscKernelSamples = 12;		
	static const float2 SmallDiscKernel[SmallDiscKernelSamples] =
	{
		float2(-0.326212,-0.40581),
		float2(-0.840144,-0.07358),
		float2(-0.695914,0.457137),
		float2(-0.203345,0.620716),
		float2(0.96234,-0.194983),
		float2(0.473434,-0.480026),
		float2(0.519456,0.767022),
		float2(0.185461,-0.893124),
		float2(0.507431,0.064425),
		float2(0.89642,0.412458),
		float2(-0.32194,-0.932615),
		float2(-0.791559,-0.59771)
	};

	static const int NumDiscSamples = 28;
	static const float3 DiscKernel[NumDiscSamples] = 
	{
		float3(0.62463,0.54337,0.82790),
		float3(-0.13414,-0.94488,0.95435),
		float3(0.38772,-0.43475,0.58253),
		float3(0.12126,-0.19282,0.22778),
		float3(-0.20388,0.11133,0.23230),
		float3(0.83114,-0.29218,0.88100),
		float3(0.10759,-0.57839,0.58831),
		float3(0.28285,0.79036,0.83945),
		float3(-0.36622,0.39516,0.53876),
		float3(0.75591,0.21916,0.78704),
		float3(-0.52610,0.02386,0.52664),
		float3(-0.88216,-0.24471,0.91547),
		float3(-0.48888,-0.29330,0.57011),
		float3(0.44014,-0.08558,0.44838),
		float3(0.21179,0.51373,0.55567),
		float3(0.05483,0.95701,0.95858),
		float3(-0.59001,-0.70509,0.91938),
		float3(-0.80065,0.24631,0.83768),
		float3(-0.19424,-0.18402,0.26757),
		float3(-0.43667,0.76751,0.88304),
		float3(0.21666,0.11602,0.24577),
		float3(0.15696,-0.85600,0.87027),
		float3(-0.75821,0.58363,0.95682),
		float3(0.99284,-0.02904,0.99327),
		float3(-0.22234,-0.57907,0.62029),
		float3(0.55052,-0.66984,0.86704),
		float3(0.46431,0.28115,0.54280),
		float3(-0.07214,0.60554,0.60982),
	};	

	float4 fragBlurInsaneMQ (v2f i) : COLOR 
	{
		float4 centerTap = tex2D(_MainTex, i.uv1.xy);
		float4 sum = centerTap;
		float4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w;

		float sampleCount = max(centerTap.a * 0.25, _Offsets.z); // <- weighing with 0.25 looks nicer for small high freq spec
		sum *= sampleCount;

		float weights = 0;
		
		for(int l=0; l < NumDiscSamples; l++)
		{
			float2 sampleUV = i.uv1.xy + DiscKernel[l].xy * poissonScale.xy;
			float4 sample0 = tex2D(_MainTex, sampleUV.xy);

			if( sample0.a > 0.0 )  
			{
				weights = BokehWeightDisc(sample0, DiscKernel[l].z, centerTap);
				sum += sample0 * weights; 
				sampleCount += weights; 
			}
		}
		
		float4 returnValue = sum / sampleCount;
		returnValue.a = centerTap.a;

		return returnValue;
	}		

	float4 fragBlurInsaneHQ (v2f i) : COLOR 
	{
		float4 centerTap = tex2D(_MainTex, i.uv1.xy);
		float4 sum = centerTap;
		float4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w;

		float sampleCount = max(centerTap.a * 0.25, _Offsets.z); // <- weighing with 0.25 looks nicer for small high freq spec
		sum *= sampleCount;

		float2 weights = 0;
		
		for(int l=0; l < NumDiscSamples; l++)
		{
			float4 sampleUV = i.uv1.xyxy + DiscKernel[l].xyxy * poissonScale.xyxy / float4(1.2,1.2,DiscKernel[l].zz);

			float4 sample0 = tex2D(_MainTex, sampleUV.xy);
			float4 sample1 = tex2D(_MainTex, sampleUV.zw);	

			if( (sample0.a + sample1.a) > 0.0 )  
			{
				weights = BokehWeightDisc2(sample0, sample1, float2(DiscKernel[l].z/1.2, 1.0), centerTap);
				sum += sample0 * weights.x + sample1 * weights.y; 
				sampleCount += dot(weights, 1); 
			}
		}
		
		float4 returnValue = sum / sampleCount;
		returnValue.a = centerTap.a;

		return returnValue;
	}

	inline float4 BlendLowWithHighHQ(float coc, float4 low, float4 high)
	{
		float blend = smoothstep(0.65,0.85, coc);
		return lerp(low, high, blend);
	}

	inline float4 BlendLowWithHighMQ(float coc, float4 low, float4 high)
	{
		float blend = smoothstep(0.4,0.6, coc);
		return lerp(low, high, blend);
	}

	float4 fragBlurUpsampleCombineHQ (v2f i) : COLOR 
	{	
		float4 bigBlur = tex2D(_LowRez, i.uv1.xy);
		float4 centerTap = tex2D(_MainTex, i.uv1.xy);

		float4 smallBlur = centerTap;
		float4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w ;
					
		float sampleCount = max(centerTap.a * 0.25, 0.1f); // <- weighing with 0.25 looks nicer for small high freq spec
		smallBlur *= sampleCount; 
		
		for(int l=0; l < NumDiscSamples; l++)
		{
			float2 sampleUV = i.uv1.xy + DiscKernel[l].xy * poissonScale.xy;

			float4 sample0 = tex2D(_MainTex, sampleUV);	 
			float weight0 = BokehWeightDisc(sample0, DiscKernel[l].z, centerTap);
			smallBlur += sample0 * weight0; sampleCount += weight0;
		}

		smallBlur /= (sampleCount+1e-5f);		
		smallBlur = BlendLowWithHighHQ(centerTap.a, smallBlur, bigBlur);

		return centerTap.a < 1e-2f ? centerTap : float4(smallBlur.rgb,centerTap.a);
	}

	float4 fragBlurUpsampleCombineMQ (v2f i) : COLOR 
	{			
		float4 bigBlur = tex2D(_LowRez, i.uv1.xy);
		float4 centerTap = tex2D(_MainTex, i.uv1.xy);

		float4 smallBlur = centerTap;
		float4 poissonScale = _MainTex_TexelSize.xyxy * centerTap.a * _Offsets.w ;
					
		float sampleCount = max(centerTap.a * 0.25, 0.1f); // <- weighing with 0.25 looks nicer for small high freq spec
		smallBlur *= sampleCount; 
		
		for(int l=0; l < SmallDiscKernelSamples; l++)
		{
			float2 sampleUV = i.uv1.xy + SmallDiscKernel[l].xy * poissonScale.xy*1.1;

			float4 sample0 = tex2D(_MainTex, sampleUV);	 
			float weight0 = BokehWeightDisc(sample0, length(SmallDiscKernel[l].xy*1.1), centerTap);
			smallBlur += sample0 * weight0; sampleCount += weight0;
		}

		smallBlur /= (sampleCount+1e-5f);
		
		smallBlur = BlendLowWithHighMQ(centerTap.a, smallBlur, bigBlur);

		return centerTap.a < 1e-2f ? centerTap : float4(smallBlur.rgb,centerTap.a);
	}	

	float4 fragBlurUpsampleCheap (v2f i) : COLOR 
	{			
		float4 centerTap = tex2D(_MainTex, i.uv1.xy);
		float4 bigBlur = tex2D(_LowRez, i.uv1.xy);

		float fgCoc = tex2D(_FgOverlap, i.uv1.xy).a;
		float4 smallBlur = lerp(centerTap, bigBlur, saturate( max(centerTap.a,fgCoc)*8.0 ));

		return float4(smallBlur.rgb, centerTap.a);
	}	
									
	float4 fragBlurBox (v2f i) : COLOR 
	{
		const int TAPS = 12;

		float4 centerTap = tex2D(_MainTex, i.uv1.xy);

		// TODO: important ? breaks when HR blur is being used
		//centerTap.a = max(centerTap.a, 0.1f);

		float sampleCount =  centerTap.a;
		float4 sum = centerTap * sampleCount;
		
		float2 lenStep = centerTap.aa * (1.0 / (TAPS-1.0));
		float4 steps = (_Offsets.xyxy * _MainTex_TexelSize.xyxy) * lenStep.xyxy * float4(1,1, -1,-1);
		
		for(int l=1; l<TAPS; l++)
		{
			float4 sampleUV = i.uv1.xyxy + steps * (float)l;
			
			float4 sample0 = tex2D(_MainTex, sampleUV.xy);
			float4 sample1 = tex2D(_MainTex, sampleUV.zw);
	
			float2 maxLen01 = float2(sample0.a, sample1.a);
			float2 r = lenStep.xx * (float)l;
			
			float2 weight01 = smoothstep(float2(-0.4,-0.4),float2(0.0,0.0), maxLen01-r);
			sum += sample0 * weight01.x + sample1 * weight01.y; 

			sampleCount += dot(weight01,1);
		}
		
		float4 returnValue = sum / (1e-5f + sampleCount);

		//returnValue.a = centerTap.a;
		//return centerTap.a;

		return returnValue;
	}		


	float4 fragVisualize (v2f i) : COLOR 
	{
		float4 returnValue = tex2D(_MainTex, i.uv1.xy);	
		returnValue.rgb = lerp(float3(0.0,0.0,0.0), float3(1.0,1.0,1.0), saturate(returnValue.a/_CurveParams.x));
		return returnValue;
	}


	float4 fragBoxDownsample (v2f i) : COLOR 
	{		
		//float4 returnValue = tex2D(_MainTex, i.uv1.xy);			
		float4 returnValue = tex2D(_MainTex, i.uv1.xy + 0.75*_MainTex_TexelSize.xy);
		returnValue += tex2D(_MainTex, i.uv1.xy - 0.75*_MainTex_TexelSize.xy);
		returnValue += tex2D(_MainTex, i.uv1.xy + 0.75*_MainTex_TexelSize.xy * float2(1,-1));
		returnValue += tex2D(_MainTex, i.uv1.xy - 0.75*_MainTex_TexelSize.xy * float2(1,-1));

		return returnValue/4;
	}		

	float4 fragBlurAlphaWeighted (v2fBlur i) : COLOR 
	{
		const float ALPHA_WEIGHT = 2.0f;
		float4 sum = float4 (0,0,0,0);
		float w = 0;
		float weights = 0;
		const float G_WEIGHTS[6] = {1.0, 0.8, 0.675, 0.5, 0.2, 0.075}; 

		float4 sampleA = tex2D(_MainTex, i.uv.xy);

		float4 sampleB = tex2D(_MainTex, i.uv01.xy);
		float4 sampleC = tex2D(_MainTex, i.uv01.zw);
		float4 sampleD = tex2D(_MainTex, i.uv23.xy);
		float4 sampleE = tex2D(_MainTex, i.uv23.zw);
		float4 sampleF = tex2D(_MainTex, i.uv45.xy);
		float4 sampleG = tex2D(_MainTex, i.uv45.zw);
		float4 sampleH = tex2D(_MainTex, i.uv67.xy);
		float4 sampleI = tex2D(_MainTex, i.uv67.zw);
		float4 sampleJ = tex2D(_MainTex, i.uv89.xy);
		float4 sampleK = tex2D(_MainTex, i.uv89.zw);		
								
		w = sampleA.a * G_WEIGHTS[0]; sum += sampleA * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleB.a) * G_WEIGHTS[1]; sum += sampleB * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleC.a) * G_WEIGHTS[1]; sum += sampleC * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleD.a) * G_WEIGHTS[2]; sum += sampleD * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleE.a) * G_WEIGHTS[2]; sum += sampleE * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleF.a) * G_WEIGHTS[3]; sum += sampleF * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleG.a) * G_WEIGHTS[3]; sum += sampleG * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleH.a) * G_WEIGHTS[4]; sum += sampleH * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleI.a) * G_WEIGHTS[4]; sum += sampleI * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleJ.a) * G_WEIGHTS[5]; sum += sampleJ * w; weights += w;
		w = saturate(ALPHA_WEIGHT*sampleK.a) * G_WEIGHTS[5]; sum += sampleK * w; weights += w;

		sum /= weights + 1e-4f;

		sum.a = sampleA.a;
		if(sampleA.a<1e-2f) sum.rgb = sampleA.rgb;

		return sum;
	}	
	
	float4 fragBlurForFgCoc (v2fBlur i) : COLOR 
	{
		float4 sum = float4 (0,0,0,0);
		float w = 0;
		float weights = 0;
		const float G_WEIGHTS[6] = {1.0, 0.8, 0.675, 0.5, 0.2, 0.075}; 

		float4 sampleA = tex2D(_MainTex, i.uv.xy);

		float4 sampleB = tex2D(_MainTex, i.uv01.xy);
		float4 sampleC = tex2D(_MainTex, i.uv01.zw);
		float4 sampleD = tex2D(_MainTex, i.uv23.xy);
		float4 sampleE = tex2D(_MainTex, i.uv23.zw);
		float4 sampleF = tex2D(_MainTex, i.uv45.xy);
		float4 sampleG = tex2D(_MainTex, i.uv45.zw);
		float4 sampleH = tex2D(_MainTex, i.uv67.xy);
		float4 sampleI = tex2D(_MainTex, i.uv67.zw);
		float4 sampleJ = tex2D(_MainTex, i.uv89.xy);
		float4 sampleK = tex2D(_MainTex, i.uv89.zw);		
								
		w = sampleA.a * G_WEIGHTS[0]; sum += sampleA * w; weights += w;								
		w = smoothstep(-0.5,0.0,sampleB.a-sampleA.a) * G_WEIGHTS[1]; sum += sampleB * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleC.a-sampleA.a) * G_WEIGHTS[1]; sum += sampleC * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleD.a-sampleA.a) * G_WEIGHTS[2]; sum += sampleD * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleE.a-sampleA.a) * G_WEIGHTS[2]; sum += sampleE * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleF.a-sampleA.a) * G_WEIGHTS[3]; sum += sampleF * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleG.a-sampleA.a) * G_WEIGHTS[3]; sum += sampleG * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleH.a-sampleA.a) * G_WEIGHTS[4]; sum += sampleH * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleI.a-sampleA.a) * G_WEIGHTS[4]; sum += sampleI * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleJ.a-sampleA.a) * G_WEIGHTS[5]; sum += sampleJ * w; weights += w;
		w = smoothstep(-0.5,0.0,sampleK.a-sampleA.a) * G_WEIGHTS[5]; sum += sampleK * w; weights += w;

		sum /= weights + 1e-4f;

		return sum;
	}	

	float4 fragGaussBlur (v2fBlur i) : COLOR 
	{
		float4 sum = float4 (0,0,0,0);
		float w = 0;
		float weights = 0;
		const float G_WEIGHTS[9] = {1.0, 0.8, 0.65, 0.5, 0.4, 0.2, 0.1, 0.05, 0.025}; 

		float4 sampleA = tex2D(_MainTex, i.uv.xy);

		float4 sampleB = tex2D(_MainTex, i.uv01.xy);
		float4 sampleC = tex2D(_MainTex, i.uv01.zw);
		float4 sampleD = tex2D(_MainTex, i.uv23.xy);
		float4 sampleE = tex2D(_MainTex, i.uv23.zw);
		float4 sampleF = tex2D(_MainTex, i.uv45.xy);
		float4 sampleG = tex2D(_MainTex, i.uv45.zw);
		float4 sampleH = tex2D(_MainTex, i.uv67.xy);
		float4 sampleI = tex2D(_MainTex, i.uv67.zw);
		float4 sampleJ = tex2D(_MainTex, i.uv89.xy);
		float4 sampleK = tex2D(_MainTex, i.uv89.zw);

		w = sampleA.a * G_WEIGHTS[0]; sum += sampleA * w; weights += w;
		w = sampleB.a * G_WEIGHTS[1]; sum += sampleB * w; weights += w;
		w = sampleC.a * G_WEIGHTS[1]; sum += sampleC * w; weights += w;
		w = sampleD.a * G_WEIGHTS[2]; sum += sampleD * w; weights += w;
		w = sampleE.a * G_WEIGHTS[2]; sum += sampleE * w; weights += w;
		w = sampleF.a * G_WEIGHTS[3]; sum += sampleF * w; weights += w;
		w = sampleG.a * G_WEIGHTS[3]; sum += sampleG * w; weights += w;
		w = sampleH.a * G_WEIGHTS[4]; sum += sampleH * w; weights += w;
		w = sampleI.a * G_WEIGHTS[4]; sum += sampleI * w; weights += w;
		w = sampleJ.a * G_WEIGHTS[5]; sum += sampleJ * w; weights += w;
		w = sampleK.a * G_WEIGHTS[5]; sum += sampleK * w; weights += w;

		sum /= weights + 1e-4f;

		return sum;
	}

	float4 frag4TapBlurForLRSpawn (v2f i) : COLOR 
	{
		float4 tap  =  tex2D(_MainTex, i.uv.xy);
		
		float4 tapA =  tex2D(_MainTex, i.uv.xy + 0.75 * _MainTex_TexelSize.xy); 
		float4 tapB =  tex2D(_MainTex, i.uv.xy - 0.75 * _MainTex_TexelSize.xy);
		float4 tapC =  tex2D(_MainTex, i.uv.xy + 0.75 * _MainTex_TexelSize.xy * float2(1,-1));
		float4 tapD =  tex2D(_MainTex, i.uv.xy - 0.75 * _MainTex_TexelSize.xy * float2(1,-1));
		
		float4 weights = saturate(10.0 * float4(tapA.a, tapB.a, tapC.a, tapD.a));
		float sumWeights = dot(weights, 1);

		float4 color = (tapA*weights.x + tapB*weights.y + tapC*weights.z + tapD*weights.w);

		float4 outColor = tap;
		if(tap.a * sumWeights * 8.0 > 1e-5f) outColor.rgb = color.rgb/sumWeights;

		return outColor;
	}

	float4 fragCaptureColorAndSignedCoc (v2f i) : COLOR 
	{	
		float4 color = tex2D (_MainTex, i.uv1.xy);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * abs(d - _CurveParams.w) / (d + 1e-5f); 
		color.a = clamp( max(0.0, color.a - _CurveParams.y), 0.0, _CurveParams.x) * sign(d - _CurveParams.w);
		
		return color;
	} 
	
	float4 fragCaptureCoc (v2f i) : COLOR 
	{	
		float4 color = float4(0,0,0,0); //tex2D (_MainTex, i.uv1.xy);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * abs(d - _CurveParams.w) / (d + 1e-5f); 
		color.a = clamp( max(0.0, color.a - _CurveParams.y), 0.0, _CurveParams.x);
		
		return color;
	} 

	float4 AddFgCoc (v2f i) : COLOR 
	{	
		return tex2D (_MainTex, i.uv1.xy);
	} 

	float4 fragMergeCoc (v2f i) : COLOR 
	{	
		float4 color = tex2D (_FgOverlap, i.uv1.xy); // this is the foreground overlap value
		float fgCoc = color.a;

		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * abs(d - _CurveParams.w) / (d + 1e-5f); 
		color.a = clamp( max(0.0, color.a - _CurveParams.y), 0.0, _CurveParams.x);
		
		return max(color.aaaa, float4(fgCoc,fgCoc,fgCoc,fgCoc));
	} 

	float4 fragCombineCocWithMaskBlur (v2f i) : COLOR 
	{	
		float bgAndFgCoc = tex2D (_MainTex, i.uv1.xy).a;
		float fgOverlapCoc = tex2D (_FgOverlap, i.uv1.xy).a;

		return (bgAndFgCoc < 0.01) * saturate(fgOverlapCoc-bgAndFgCoc);
	} 
	
	float4 fragCaptureForegroundCoc (v2f i) : COLOR 
	{	
		float4 color = float4(0,0,0,0); //tex2D (_MainTex, i.uv1.xy);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * (_CurveParams.w-d) / (d + 1e-5f);
		color.a = clamp(max(0.0, color.a - _CurveParams.y), 0.0, _CurveParams.x);
		
		return color;	
	}	

	float4 fragCaptureForegroundCocMask (v2f i) : COLOR 
	{	
		float4 color = float4(0,0,0,0);
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv1.xy));
		d = Linear01Depth (d);
		color.a = _CurveParams.z * (_CurveParams.w-d) / (d + 1e-5f);
		color.a = clamp(max(0.0, color.a - _CurveParams.y), 0.0, _CurveParams.x);
		
		return color.a > 0;	
	}	
	
	float4 fragBlendInHighRez (v2f i) : COLOR 
	{
		float4 tapHighRez =  tex2D(_MainTex, i.uv.xy);
		return float4(tapHighRez.rgb, 1.0-saturate(tapHighRez.a*5.0));
	}
	
	float4 fragBlendInLowRezParts (v2f i) : COLOR 
	{
		float4 from = tex2D(_MainTex, i.uv1.xy);
		from.a = saturate(from.a * _Offsets.w) / (_CurveParams.x + 1e-5f);
		float square = from.a * from.a;
		from.a = square * square * _CurveParams.x;
		return from;
	}
	
	float4 fragUpsampleWithAlphaMask(v2f i) : COLOR 
	{
		float4 c = tex2D(_MainTex, i.uv1.xy);
		return c;
	}		
	
	float4 fragAlphaMask(v2f i) : COLOR 
	{
		float4 c = tex2D(_MainTex, i.uv1.xy);
		c.a = saturate(c.a*100.0);
		return c;
	}	
		
	ENDCG
	
Subshader 
{
 
 // pass 0
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureCoc
      #pragma exclude_renderers d3d11_9x flash
      
      ENDCG
  	}

 // pass 1
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragGaussBlur
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}

 // pass 2

 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragBlurForFgCoc
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}
  	
  	
 // pass 3
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
	  ColorMask A
	  BlendOp Max, Max
	  Blend One One, One One

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment AddFgCoc
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}  
  	 	

 // pass 4
  
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
	  ColorMask A

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureForegroundCoc
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 5
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurBox
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 6
 
 Pass { 
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment frag4TapBlurForLRSpawn
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 7
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask RGB
	  Blend SrcAlpha OneMinusSrcAlpha
  	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlendInHighRez
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 
  	
 // pass 8
 
 Pass 
 {
	  ZTest Always Cull Off ZWrite Off
	  ColorMask A
	  Fog { Mode off }       

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureForegroundCocMask
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}   
  	

 // pass 9
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurUpsampleCheap
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}   	 	 	  	 	 	  	

 // pass 10
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragCaptureColorAndSignedCoc
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}   

 // pass 11
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurInsaneMQ
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 12
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurUpsampleCombineMQ
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 
  	
  	// pass 13
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }

	  ColorMask A 

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragMergeCoc
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}  
  	
 // pass 14
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }    

	  ColorMask A
	  BlendOp Max, Max
	  Blend One One, One One

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment fragCombineCocWithMaskBlur
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 15
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBoxDownsample
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}   

 // pass 16
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

     	CGPROGRAM

      	#pragma glsl
      	#pragma target 3.0
      	#pragma fragmentoption ARB_precision_hint_fastest
      	#pragma vertex vert
		#pragma fragment fragVisualize
		#pragma exclude_renderers d3d11_9x flash

      	ENDCG
  	}	

 // pass 17
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest 
      #pragma vertex vert
      #pragma fragment fragBlurInsaneHQ
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 

 // pass 18
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragBlurUpsampleCombineHQ
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}    	

  // pass 19

 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl	
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertBlurPlusMinus
      #pragma fragment fragBlurAlphaWeighted
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}    
	
  // pass 20
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragAlphaMask
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}
	
  // pass 21
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  

	  BlendOp Add, Add
	  Blend DstAlpha OneMinusDstAlpha, Zero One

      CGPROGRAM

      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vertFlip
      #pragma fragment fragBlurBox
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	}	
	
  // pass 22
 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }  

	  // destination alpha needs to stay intact as we have layed alpha before
	  BlendOp Add, Add
	  Blend DstAlpha One, Zero One

      CGPROGRAM
      
      #pragma glsl
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma vertex vert
      #pragma fragment fragUpsampleWithAlphaMask
      #pragma exclude_renderers d3d11_9x flash

      ENDCG
  	} 	
}
  
Fallback off

}