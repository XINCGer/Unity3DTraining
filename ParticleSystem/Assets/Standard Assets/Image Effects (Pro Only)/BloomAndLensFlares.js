
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Bloom and Glow/BloomAndFlares (3.5, Deprecated)")

enum LensflareStyle34 {
	Ghosting = 0,
	Anamorphic = 1,
	Combined = 2,
}

enum TweakMode34 {
	Basic = 0,
	Complex = 1,
}

enum HDRBloomMode {
	Auto = 0,
	On = 1,
	Off = 2,
}

enum BloomScreenBlendMode {
	Screen = 0,
	Add = 1,
}

class BloomAndLensFlares extends PostEffectsBase {
	public var tweakMode : TweakMode34 = 0;
	public var screenBlendMode : BloomScreenBlendMode = BloomScreenBlendMode.Add;

	public var hdr : HDRBloomMode = HDRBloomMode.Auto;
	private var doHdr : boolean = false;
	public var sepBlurSpread : float = 1.5f;
	public var useSrcAlphaAsMask : float = 0.5f;

	public var bloomIntensity : float = 1.0f;
	public var bloomThreshhold : float = 0.5f;
	public var bloomBlurIterations : int = 2;

	public var lensflares : boolean = false;
	public var hollywoodFlareBlurIterations : int = 2;
	public var lensflareMode : LensflareStyle34 = 1;
	public var hollyStretchWidth : float = 3.5f;
	public var lensflareIntensity : float = 1.0f;
	public var lensflareThreshhold : float = 0.3f;
	public var flareColorA : Color = Color (0.4f, 0.4f, 0.8f, 0.75f);
	public var flareColorB : Color = Color (0.4f, 0.8f, 0.8f, 0.75f);
	public var flareColorC : Color = Color (0.8f, 0.4f, 0.8f, 0.75f);
	public var flareColorD : Color = Color (0.8f, 0.4f, 0.0f, 0.75f);
	public var blurWidth : float = 1.0f;
	public var lensFlareVignetteMask : Texture2D;

	public var lensFlareShader : Shader;
	private var lensFlareMaterial : Material;

	public var vignetteShader : Shader;
	private var vignetteMaterial : Material;

	public var separableBlurShader : Shader;
	private var separableBlurMaterial : Material;

	public var addBrightStuffOneOneShader: Shader;
	private var addBrightStuffBlendOneOneMaterial : Material;

	public var screenBlendShader : Shader;
	private var screenBlend : Material;

	public var hollywoodFlaresShader: Shader;
	private var hollywoodFlaresMaterial : Material;

	public var brightPassFilterShader : Shader;
	private var brightPassFilterMaterial : Material;

	function CheckResources () : boolean {
		CheckSupport (false);

		screenBlend = CheckShaderAndCreateMaterial (screenBlendShader, screenBlend);
		lensFlareMaterial = CheckShaderAndCreateMaterial(lensFlareShader,lensFlareMaterial);
		vignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader,vignetteMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader,separableBlurMaterial);
		addBrightStuffBlendOneOneMaterial = CheckShaderAndCreateMaterial(addBrightStuffOneOneShader,addBrightStuffBlendOneOneMaterial);
		hollywoodFlaresMaterial = CheckShaderAndCreateMaterial (hollywoodFlaresShader, hollywoodFlaresMaterial);
		brightPassFilterMaterial = CheckShaderAndCreateMaterial(brightPassFilterShader, brightPassFilterMaterial);

		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {
		if(CheckResources()==false) {
			Graphics.Blit (source, destination);
			return;
		}

		// screen blend is not supported when HDR is enabled (will cap values)

		doHdr = false;
		if(hdr == HDRBloomMode.Auto)
			doHdr = source.format == RenderTextureFormat.ARGBHalf && GetComponent.<Camera>().hdr;
		else {
			doHdr = hdr == HDRBloomMode.On;
		}

		doHdr = doHdr && supportHDRTextures;

		var realBlendMode : BloomScreenBlendMode = screenBlendMode;
		if(doHdr)
			realBlendMode = BloomScreenBlendMode.Add;

		var rtFormat = (doHdr) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
		var halfRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 2, source.height / 2, 0, rtFormat);
		var quarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4, source.height / 4, 0, rtFormat);
		var secondQuarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4, source.height / 4, 0, rtFormat);
		var thirdQuarterRezColor : RenderTexture = RenderTexture.GetTemporary (source.width / 4, source.height / 4, 0, rtFormat);

		var widthOverHeight : float = (1.0f * source.width) / (1.0f * source.height);
		var oneOverBaseSize : float = 1.0f / 512.0f;

		// downsample

		Graphics.Blit (source, halfRezColor, screenBlend, 2); // <- 2 is stable downsample
		Graphics.Blit (halfRezColor, quarterRezColor, screenBlend, 2); // <- 2 is stable downsample

		RenderTexture.ReleaseTemporary (halfRezColor);

		// cut colors (threshholding)

		BrightFilter (bloomThreshhold, useSrcAlphaAsMask, quarterRezColor, secondQuarterRezColor);
		quarterRezColor.DiscardContents();

		// blurring

		if (bloomBlurIterations < 1) bloomBlurIterations = 1;

		for (var iter : int = 0; iter < bloomBlurIterations; iter++ ) {
			var spreadForPass : float = (1.0f + (iter * 0.5f)) * sepBlurSpread;
			separableBlurMaterial.SetVector ("offsets", Vector4 (0.0f, spreadForPass * oneOverBaseSize, 0.0f, 0.0f));

			var src : RenderTexture = iter == 0 ? secondQuarterRezColor : quarterRezColor;
			Graphics.Blit (src, thirdQuarterRezColor, separableBlurMaterial);
			src.DiscardContents();

			separableBlurMaterial.SetVector ("offsets", Vector4 ((spreadForPass / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
			Graphics.Blit (thirdQuarterRezColor, quarterRezColor, separableBlurMaterial);
			thirdQuarterRezColor.DiscardContents();
		}

		// lens flares: ghosting, anamorphic or a combination

		if (lensflares) {

			if (lensflareMode == 0) {

				BrightFilter (lensflareThreshhold, 0.0f, quarterRezColor, thirdQuarterRezColor);
				quarterRezColor.DiscardContents();

				// smooth a little, this needs to be resolution dependent
				/*
				separableBlurMaterial.SetVector ("offsets", Vector4 (0.0f, (2.0f) / (1.0f * quarterRezColor.height), 0.0f, 0.0f));
				Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, separableBlurMaterial);
				separableBlurMaterial.SetVector ("offsets", Vector4 ((2.0f) / (1.0f * quarterRezColor.width), 0.0f, 0.0f, 0.0f));
				Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, separableBlurMaterial);
				*/
				// no ugly edges!

				Vignette (0.975, thirdQuarterRezColor, secondQuarterRezColor);
				thirdQuarterRezColor.DiscardContents();

				BlendFlares (secondQuarterRezColor, quarterRezColor);
				secondQuarterRezColor.DiscardContents();
			}

			// (b) hollywood/anamorphic flares?

			else {

				// thirdQuarter has the brightcut unblurred colors
				// quarterRezColor is the blurred, brightcut buffer that will end up as bloom

				hollywoodFlaresMaterial.SetVector ("_Threshhold", Vector4 (lensflareThreshhold, 1.0f / (1.0f - lensflareThreshhold), 0.0f, 0.0f));
				hollywoodFlaresMaterial.SetVector ("tintColor", Vector4 (flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * flareColorA.a * lensflareIntensity);
				Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, hollywoodFlaresMaterial, 2);
				thirdQuarterRezColor.DiscardContents();

				Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, hollywoodFlaresMaterial, 3);
				secondQuarterRezColor.DiscardContents();

				hollywoodFlaresMaterial.SetVector ("offsets", Vector4 ((sepBlurSpread * 1.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));
				hollywoodFlaresMaterial.SetFloat ("stretchWidth", hollyStretchWidth);
				Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, hollywoodFlaresMaterial, 1);
				thirdQuarterRezColor.DiscardContents();

				hollywoodFlaresMaterial.SetFloat ("stretchWidth", hollyStretchWidth * 2.0f);
				Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, hollywoodFlaresMaterial, 1);
				secondQuarterRezColor.DiscardContents();

				hollywoodFlaresMaterial.SetFloat ("stretchWidth", hollyStretchWidth * 4.0f);
				Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, hollywoodFlaresMaterial, 1);
				thirdQuarterRezColor.DiscardContents();

				if (lensflareMode == 1) {
					for (var itera : int = 0; itera < hollywoodFlareBlurIterations; itera++ ) {
						separableBlurMaterial.SetVector ("offsets", Vector4 ((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));
						Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, separableBlurMaterial);
						secondQuarterRezColor.DiscardContents();

						separableBlurMaterial.SetVector ("offsets", Vector4 ((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));
						Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, separableBlurMaterial);
						thirdQuarterRezColor.DiscardContents();
					}

					AddTo (1.0, secondQuarterRezColor, quarterRezColor);
					secondQuarterRezColor.DiscardContents();
				}
				else {

					// (c) combined

					for (var ix : int = 0; ix < hollywoodFlareBlurIterations; ix++ ) {
						separableBlurMaterial.SetVector ("offsets", Vector4 ((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));
						Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, separableBlurMaterial);
						secondQuarterRezColor.DiscardContents();

						separableBlurMaterial.SetVector ("offsets", Vector4 ((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0, 0.0, 0.0));
						Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, separableBlurMaterial);
						thirdQuarterRezColor.DiscardContents();
					}

					Vignette (1.0, secondQuarterRezColor, thirdQuarterRezColor);
					secondQuarterRezColor.DiscardContents();

					BlendFlares (thirdQuarterRezColor, secondQuarterRezColor);
					thirdQuarterRezColor.DiscardContents();

					AddTo (1.0, secondQuarterRezColor, quarterRezColor);
					secondQuarterRezColor.DiscardContents();
				}
			}
		}

		// screen blend bloom results to color buffer

		screenBlend.SetFloat ("_Intensity", bloomIntensity);
		screenBlend.SetTexture ("_ColorBuffer", source);
		Graphics.Blit (quarterRezColor, destination, screenBlend, realBlendMode);

		RenderTexture.ReleaseTemporary (quarterRezColor);
		RenderTexture.ReleaseTemporary (secondQuarterRezColor);
		RenderTexture.ReleaseTemporary (thirdQuarterRezColor);
	}

	private function AddTo (intensity_ : float, from : RenderTexture, to : RenderTexture) {
		addBrightStuffBlendOneOneMaterial.SetFloat ("_Intensity", intensity_);
		Graphics.Blit (from, to, addBrightStuffBlendOneOneMaterial);
	}

	private function BlendFlares (from : RenderTexture, to : RenderTexture) {
		lensFlareMaterial.SetVector ("colorA", Vector4 (flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * lensflareIntensity);
		lensFlareMaterial.SetVector ("colorB", Vector4 (flareColorB.r, flareColorB.g, flareColorB.b, flareColorB.a) * lensflareIntensity);
		lensFlareMaterial.SetVector ("colorC", Vector4 (flareColorC.r, flareColorC.g, flareColorC.b, flareColorC.a) * lensflareIntensity);
		lensFlareMaterial.SetVector ("colorD", Vector4 (flareColorD.r, flareColorD.g, flareColorD.b, flareColorD.a) * lensflareIntensity);
		Graphics.Blit (from, to, lensFlareMaterial);
	}

	private function BrightFilter (thresh : float, useAlphaAsMask : float, from : RenderTexture, to : RenderTexture) {
		if(doHdr)
			brightPassFilterMaterial.SetVector ("threshhold", Vector4 (thresh, 1.0f, 0.0f, 0.0f));
		else
			brightPassFilterMaterial.SetVector ("threshhold", Vector4 (thresh, 1.0f / (1.0f-thresh), 0.0f, 0.0f));
		brightPassFilterMaterial.SetFloat ("useSrcAlphaAsMask", useAlphaAsMask);
		Graphics.Blit (from, to, brightPassFilterMaterial);
	}

	private function Vignette (amount : float, from : RenderTexture, to : RenderTexture) {
		if(lensFlareVignetteMask) {
			screenBlend.SetTexture ("_ColorBuffer", lensFlareVignetteMask);
			Graphics.Blit (from, to, screenBlend, 3);
		}
		else {
			vignetteMaterial.SetFloat ("vignetteIntensity", amount);
			Graphics.Blit (from, to, vignetteMaterial);
		}
	}

}
