//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;
using Alignment = NGUIText.Alignment;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Label")]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None,
		Shadow,
		Outline,
		Outline8,
	}

	public enum Overflow
	{
		ShrinkContent,
		ClampContent,
		ResizeFreely,
		ResizeHeight,
	}

	public enum Crispness
	{
		Never,
		OnDesktop,
		Always,
	}

	public enum Modifier
	{
		None,
		ToUppercase,
		ToLowercase,
		Custom = 255,
	}

	/// <summary>
	/// Whether the label will keep its content crisp even when shrunk.
	/// You may want to turn this off on mobile devices.
	/// </summary>

	public Crispness keepCrispWhenShrunk = Crispness.OnDesktop;

	[HideInInspector][SerializeField] Font mTrueTypeFont;
	[HideInInspector][SerializeField] UIFont mFont;
#if !UNITY_3_5
	[MultilineAttribute(6)]
#endif
	[HideInInspector][SerializeField] string mText = "";
	[HideInInspector][SerializeField] int mFontSize = 16;
	[HideInInspector][SerializeField] FontStyle mFontStyle = FontStyle.Normal;
	[HideInInspector][SerializeField] Alignment mAlignment = Alignment.Automatic;
	[HideInInspector][SerializeField] bool mEncoding = true;
	[HideInInspector][SerializeField] int mMaxLineCount = 0; // 0 denotes unlimited
	[HideInInspector][SerializeField] Effect mEffectStyle = Effect.None;
	[HideInInspector][SerializeField] Color mEffectColor = Color.black;
	[HideInInspector][SerializeField] NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Normal;
	[HideInInspector][SerializeField] Vector2 mEffectDistance = Vector2.one;
	[HideInInspector][SerializeField] Overflow mOverflow = Overflow.ShrinkContent;
	[HideInInspector][SerializeField] bool mApplyGradient = false;
	[HideInInspector][SerializeField] Color mGradientTop = Color.white;
	[HideInInspector][SerializeField] Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);
	[HideInInspector][SerializeField] int mSpacingX = 0;
	[HideInInspector][SerializeField] int mSpacingY = 0;
	[HideInInspector][SerializeField] bool mUseFloatSpacing = false;
	[HideInInspector][SerializeField] float mFloatSpacingX = 0;
	[HideInInspector][SerializeField] float mFloatSpacingY = 0;
	[HideInInspector][SerializeField] bool mOverflowEllipsis = false;
	[HideInInspector][SerializeField] int mOverflowWidth = 0;
	[HideInInspector][SerializeField] Modifier mModifier = Modifier.None;

	// Obsolete values
	[HideInInspector][SerializeField] bool mShrinkToFit = false;
	[HideInInspector][SerializeField] int mMaxLineWidth = 0;
	[HideInInspector][SerializeField] int mMaxLineHeight = 0;
	[HideInInspector][SerializeField] float mLineWidth = 0;
	[HideInInspector][SerializeField] bool mMultiline = true;

	[System.NonSerialized] Font mActiveTTF = null;
	[System.NonSerialized] float mDensity = 1f;
	[System.NonSerialized] bool mShouldBeProcessed = true;
	[System.NonSerialized] string mProcessedText = null;
	[System.NonSerialized] bool mPremultiply = false;
	[System.NonSerialized] Vector2 mCalculatedSize = Vector2.zero;
	[System.NonSerialized] float mScale = 1f;
	[System.NonSerialized] int mFinalFontSize = 0;
	[System.NonSerialized] int mLastWidth = 0;
	[System.NonSerialized] int mLastHeight = 0;

	/// <summary>
	/// Font size after modifications got taken into consideration such as shrinking content.
	/// </summary>

	public int finalFontSize
	{
		get
		{
			if (trueTypeFont) return Mathf.RoundToInt(mScale * mFinalFontSize);
			return Mathf.RoundToInt(mFinalFontSize * mScale);
		}
	}

	/// <summary>
	/// Function used to determine if something has changed (and thus the geometry must be rebuilt)
	/// </summary>

	bool shouldBeProcessed
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	/// <summary>
	/// Whether the rectangle is anchored horizontally.
	/// </summary>

	public override bool isAnchoredHorizontally { get { return base.isAnchoredHorizontally || mOverflow == Overflow.ResizeFreely; } }

	/// <summary>
	/// Whether the rectangle is anchored vertically.
	/// </summary>

	public override bool isAnchoredVertically
	{
		get
		{
			return base.isAnchoredVertically ||
				mOverflow == Overflow.ResizeFreely ||
				mOverflow == Overflow.ResizeHeight;
		}
	}

	/// <summary>
	/// Retrieve the material used by the font.
	/// </summary>

	public override Material material
	{
		get
		{
			if (mMat != null) return mMat;
			if (mFont != null) return mFont.material;
			if (mTrueTypeFont != null) return mTrueTypeFont.material;
			return null;
		}
		set
		{
			base.material = value;
		}
	}

	/// <summary>
	/// Label's main texture comes from the font itself.
	/// </summary>

	public override Texture mainTexture
	{
		get
		{
			if (mFont != null) return mFont.texture;
			if (mTrueTypeFont != null) { var mat = mTrueTypeFont.material; if (mat != null) return mat.mainTexture; }
			return null;
		}
		set
		{
			base.mainTexture = value;
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font { get { return bitmapFont; } set { bitmapFont = value; } }

	/// <summary>
	/// Set the font used by this label.
	/// </summary>

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				RemoveFromPanel();
				mFont = value;
				mTrueTypeFont = null;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Set the font used by this label.
	/// </summary>

	public Font trueTypeFont
	{
		get
		{
			if (mTrueTypeFont != null) return mTrueTypeFont;
			return (mFont != null ? mFont.dynamicFont : null);
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				RemoveFromPanel();
				mTrueTypeFont = value;
				shouldBeProcessed = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
					base.MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Ambiguous helper function.
	/// </summary>

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (UnityEngine.Object)mFont ?? (UnityEngine.Object)mTrueTypeFont;
		}
		set
		{
			UIFont bf = value as UIFont;
			if (bf != null) bitmapFont = bf;
			else trueTypeFont = value as Font;
		}
	}

	/// <summary>
	/// Text that's being displayed by the label.
	/// </summary>

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (mText == value) return;

			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = "";
					MarkAsChanged();
					ProcessAndRequest();
				}
			}
			else if (mText != value)
			{
				mText = value;
				MarkAsChanged();
				ProcessAndRequest();
			}

			if (autoResizeBoxCollider) ResizeCollider();
		}
	}

	/// <summary>
	/// Default font size.
	/// </summary>

	public int defaultFontSize { get { return (trueTypeFont != null) ? mFontSize : (mFont != null ? mFont.defaultSize : 16); } }

	/// <summary>
	/// Active font size used by the label.
	/// </summary>

	public int fontSize
	{
		get
		{
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);

			if (mFontSize != value)
			{
				mFontSize = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Dynamic font style used by the label.
	/// </summary>

	public FontStyle fontStyle
	{
		get
		{
			return mFontStyle;
		}
		set
		{
			if (mFontStyle != value)
			{
				mFontStyle = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Text alignment option.
	/// </summary>

	public Alignment alignment
	{
		get
		{
			return mAlignment;
		}
		set
		{
			if (mAlignment != value)
			{
				mAlignment = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	/// <summary>
	/// Whether a gradient will be applied.
	/// </summary>

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Top gradient color.
	/// </summary>

	public Color gradientTop
	{
		get
		{
			return mGradientTop;
		}
		set
		{
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Bottom gradient color.
	/// </summary>

	public Color gradientBottom
	{
		get
		{
			return mGradientBottom;
		}
		set
		{
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient) MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional horizontal spacing between characters when printing text.
	/// </summary>

	public int spacingX
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional vertical spacing between lines when printing text.
	/// </summary>

	public int spacingY
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether this label will use float text spacing values, instead of integers.
	/// </summary>

	public bool useFloatSpacing
	{
		get
		{
			return mUseFloatSpacing;
		}
		set
		{
			if (mUseFloatSpacing != value)
			{
				mUseFloatSpacing = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Additional horizontal spacing between characters when printing text.
	/// For this to have any effect useFloatSpacing must be true.
	/// </summary>

	public float floatSpacingX
	{
		get
		{
			return mFloatSpacingX;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingX, value))
			{
				mFloatSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Additional vertical spacing between lines when printing text.
	/// For this to have any effect useFloatSpacing must be true.
	/// </summary>

	public float floatSpacingY
	{
		get
		{
			return mFloatSpacingY;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingY, value))
			{
				mFloatSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Convenience property to get the used y spacing.
	/// </summary>

	public float effectiveSpacingY
	{
		get
		{
			return mUseFloatSpacing ? mFloatSpacingY : mSpacingY;
		}
	}

	/// <summary>
	/// Convenience property to get the used x spacing.
	/// </summary>

	public float effectiveSpacingX
	{
		get
		{
			return mUseFloatSpacing ? mFloatSpacingX : mSpacingX;
		}
	}

	/// <summary>
	/// Whether to append "..." at the end of clamped text that didn't fit.
	/// </summary>

	public bool overflowEllipsis
	{
		get
		{
			return mOverflowEllipsis;
		}
		set
		{
			if (mOverflowEllipsis != value)
			{
				mOverflowEllipsis = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Maximum width used when Resize Freely overflow type is enabled.
	/// If the printed text exceeds this width, it will wrap onto the following line.
	/// </summary>

	public int overflowWidth
	{
		get
		{
			return mOverflowWidth;
		}
		set
		{
			if (mOverflowWidth != value)
			{
				mOverflowWidth = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the label will use the printed size instead of font size when printing the label.
	/// It's a dynamic font feature that will ensure that the text is crisp when shrunk.
	/// </summary>

	bool keepCrisp
	{
		get
		{
			if (trueTypeFont != null && keepCrispWhenShrunk != Crispness.Never)
			{
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY
				return (keepCrispWhenShrunk == Crispness.Always);
#else
				return true;
#endif
			}
			return false;
		}
	}

	/// <summary>
	/// Whether this label will support color encoding in the format of [RRGGBB] and new line in the form of a "\\n" string.
	/// </summary>

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Style used for symbols.
	/// </summary>

	public NGUIText.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Overflow method controls the label's behaviour when its content doesn't fit the bounds.
	/// </summary>

	public Overflow overflowMethod
	{
		get
		{
			return mOverflow;
		}
		set
		{
			if (mOverflow != value)
			{
				mOverflow = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Maximum width of the label in pixels.
	/// </summary>

	[System.Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get { return width; }
		set { width = value; }
	}

	/// <summary>
	/// Maximum height of the label in pixels.
	/// </summary>

	[System.Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get { return height; }
		set { height = value; }
	}

	/// <summary>
	/// Whether the label supports multiple lines.
	/// </summary>
	
	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if ((mMaxLineCount != 1) != value)
			{
				mMaxLineCount = (value ? 0 : 1);
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Process the label's text before returning its corners.
	/// </summary>

	public override Vector3[] localCorners
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.localCorners;
		}
	}

	/// <summary>
	/// Process the label's text before returning its corners.
	/// </summary>

	public override Vector3[] worldCorners
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.worldCorners;
		}
	}

	/// <summary>
	/// Process the label's text before returning its drawing dimensions.
	/// </summary>

	public override Vector4 drawingDimensions
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.drawingDimensions;
		}
	}

	/// <summary>
	/// The max number of lines to be displayed for the label
	/// </summary>

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				shouldBeProcessed = true;
				if (overflowMethod == Overflow.ShrinkContent) MakePixelPerfect();
			}
		}
	}

	/// <summary>
	/// What effect is used by the label.
	/// </summary>

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Color used by the effect, if it's enabled.
	/// </summary>

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (mEffectColor != value)
			{
				mEffectColor = value;
				if (mEffectStyle != Effect.None) shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// Effect distance in pixels.
	/// </summary>

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				shouldBeProcessed = true;
			}
		}
	}

	/// <summary>
	/// How many quads there are per printed character.
	/// </summary>
	
	public int quadsPerCharacter
	{
		get
		{
			if (mEffectStyle == Effect.Shadow) return 2;
			else if (mEffectStyle == Effect.Outline) return 5;
			else if (mEffectStyle == Effect.Outline8) return 9;
			return 1;
		}
	}

	/// <summary>
	/// Whether the label will automatically shrink its size in order to fit the maximum line width.
	/// </summary>

	[System.Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return mOverflow == Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				overflowMethod = Overflow.ShrinkContent;
			}
		}
	}

	/// <summary>
	/// Returns the processed version of 'text', with new line characters, line wrapping, etc.
	/// </summary>

	public string processedText
	{
		get
		{
			if (mLastWidth != mWidth || mLastHeight != mHeight)
			{
				mLastWidth = mWidth;
				mLastHeight = mHeight;
				mShouldBeProcessed = true;
			}

			// Process the text if necessary
			if (shouldBeProcessed) ProcessText();
			return mProcessedText;
		}
	}

	/// <summary>
	/// Actual printed size of the text, in pixels.
	/// </summary>

	public Vector2 printedSize
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return mCalculatedSize;
		}
	}

	/// <summary>
	/// Local size of the widget, in pixels.
	/// </summary>

	public override Vector2 localSize
	{
		get
		{
			if (shouldBeProcessed) ProcessText();
			return base.localSize;
		}
	}

	/// <summary>
	/// Whether the label has a valid font.
	/// </summary>

	bool isValid { get { return mFont != null || mTrueTypeFont != null; } }

	/// <summary>
	/// Custom text modifier that can transform the visible text when the label's text is assigned.
	/// </summary>

	public ModifierFunc customModifier;
	public delegate string ModifierFunc (string s);

	/// <summary>
	/// Text modifier can transform the text that's actually printed, without altering the label's text value.
	/// </summary>

	public Modifier modifier
	{
		get
		{
			return mModifier;
		}
		set
		{
			if (mModifier != value)
			{
				mModifier = value;
				MarkAsChanged();
				ProcessAndRequest();
			}
		}
	}

	static BetterList<UILabel> mList = new BetterList<UILabel>();
	static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	/// <summary>
	/// Register the font texture change listener.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();
		mList.Add(this);
		SetActiveFont(trueTypeFont);
	}

	/// <summary>
	/// Remove the font texture change listener.
	/// </summary>

	protected override void OnDisable ()
	{
		SetActiveFont(null);
		mList.Remove(this);
		base.OnDisable();
	}

	/// <summary>
	/// Set the active font, correctly setting and clearing callbacks.
	/// </summary>

	protected void SetActiveFont (Font fnt)
	{
		if (mActiveTTF != fnt)
		{
			Font font = mActiveTTF;

			if (font != null)
			{
				int usage;

				if (mFontUsage.TryGetValue(font, out usage))
				{
					usage = Mathf.Max(0, --usage);

					if (usage == 0)
					{
#if UNITY_4_3 || UNITY_4_5
						font.textureRebuildCallback = null;
#endif
						mFontUsage.Remove(font);
					}
					else mFontUsage[font] = usage;
				}
#if UNITY_4_3 || UNITY_4_5
				else font.textureRebuildCallback = null;
#endif
			}

			mActiveTTF = fnt;
			font = fnt;

			if (font != null)
			{
				int usage = 0;

				// Font hasn't been used yet? Register a change delegate callback
#if UNITY_4_3 || UNITY_4_5
				if (!mFontUsage.TryGetValue(font, out usage))
					font.textureRebuildCallback = delegate() { OnFontChanged(font); };
#endif
#if UNITY_FLASH
				mFontUsage[font] = usage + 1;
#else
				mFontUsage[font] = ++usage;
#endif
			}
		}
	}

	/// <summary>
	/// Label's actual printed text may be modified before being drawn.
	/// </summary>

	public string printedText
	{
		get
		{
			if (!string.IsNullOrEmpty(mText))
			{
				if (mModifier == Modifier.None) return mText;
				if (mModifier == Modifier.ToLowercase) return mText.ToLower();
				if (mModifier == Modifier.ToUppercase) return mText.ToUpper();
				if (mModifier == Modifier.Custom && customModifier != null) return customModifier(mText);
			}
			return mText;
		}
	}

	/// <summary>
	/// Notification called when the Unity's font's texture gets rebuilt.
	/// Unity's font has a nice tendency to simply discard other characters when the texture's dimensions change.
	/// By requesting them inside the notification callback, we immediately force them back in.
	/// Originally I was subscribing each label to the font individually, but as it turned out
	/// mono's delegate system causes an insane amount of memory allocations when += or -= to a delegate.
	/// So... queue yet another work-around.
	/// </summary>

	static void OnFontChanged (Font font)
	{
		for (int i = 0; i < mList.size; ++i)
		{
			UILabel lbl = mList[i];

			if (lbl != null)
			{
				Font fnt = lbl.trueTypeFont;

				if (fnt == font)
				{
					fnt.RequestCharactersInTexture(lbl.printedText, lbl.mFinalFontSize, lbl.mFontStyle);
					lbl.MarkAsChanged();

					if (lbl.panel == null)
						lbl.CreatePanel();

					if (mTempDrawcalls == null)
						mTempDrawcalls = new List<UIDrawCall>();

					if (lbl.drawCall != null && !mTempDrawcalls.Contains(lbl.drawCall))
						mTempDrawcalls.Add(lbl.drawCall);
				}
			}
		}

		if (mTempDrawcalls != null)
		{
			for (int i = 0, imax = mTempDrawcalls.Count; i < imax; ++i)
			{
				UIDrawCall dc = mTempDrawcalls[i];
				if (dc.panel != null) dc.panel.FillDrawCall(dc);
			}
			mTempDrawcalls.Clear();
		}
	}

	static List<UIDrawCall> mTempDrawcalls;

	/// <summary>
	/// Get the sides of the rectangle relative to the specified transform.
	/// The order is left, top, right, bottom.
	/// </summary>

	public override Vector3[] GetSides (Transform relativeTo)
	{
		if (shouldBeProcessed) ProcessText();
		return base.GetSides(relativeTo);
	}

	/// <summary>
	/// Upgrading labels is a bit different.
	/// </summary>

	protected override void UpgradeFrom265 ()
	{
		ProcessText(true, true);

		if (mShrinkToFit)
		{
			overflowMethod = Overflow.ShrinkContent;
			mMaxLineCount = 0;
		}

		if (mMaxLineWidth != 0)
		{
			width = mMaxLineWidth;
			overflowMethod = mMaxLineCount > 0 ? Overflow.ResizeHeight : Overflow.ShrinkContent;
		}
		else overflowMethod = Overflow.ResizeFreely;

		if (mMaxLineHeight != 0)
			height = mMaxLineHeight;

		if (mFont != null)
		{
			int min = mFont.defaultSize;
			if (height < min) height = min;
			fontSize = min;
		}

		mMaxLineWidth = 0;
		mMaxLineHeight = 0;
		mShrinkToFit = false;

		NGUITools.UpdateWidgetCollider(gameObject, true);
	}

	/// <summary>
	/// If the label is anchored it should not auto-resize.
	/// </summary>

	protected override void OnAnchor ()
	{
		if (mOverflow == Overflow.ResizeFreely)
		{
			if (isFullyAnchored)
				mOverflow = Overflow.ShrinkContent;
		}
		else if (mOverflow == Overflow.ResizeHeight)
		{
			if (topAnchor.target != null && bottomAnchor.target != null)
				mOverflow = Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	/// <summary>
	/// Request the needed characters in the texture.
	/// </summary>

	void ProcessAndRequest ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && !NGUITools.GetActive(this)) return;
		if (!mAllowProcessing) return;
#endif
		if (ambigiousFont != null) ProcessText();
	}

#if UNITY_EDITOR
	// Used to ensure that we don't process font more than once inside OnValidate function below
	[System.NonSerialized] bool mAllowProcessing = true;
	[System.NonSerialized] bool mUsingTTF = true;

	/// <summary>
	/// Validate the properties.
	/// </summary>

	protected override void OnValidate ()
	{
		base.OnValidate();

		if (NGUITools.GetActive(this))
		{
			Font ttf = mTrueTypeFont;
			UIFont fnt = mFont;

			// If the true type font was not used before, but now it is, clear the font reference
			if (!mUsingTTF && ttf != null) fnt = null;
			else if (mUsingTTF && fnt != null) ttf = null;

			mFont = null;
			mTrueTypeFont = null;
			mAllowProcessing = false;
			SetActiveFont(null);

			if (fnt != null)
			{
				bitmapFont = fnt;
				mUsingTTF = false;
			}
			else if (ttf != null)
			{
				trueTypeFont = ttf;
				mUsingTTF = true;
			}

			shouldBeProcessed = true;
			mAllowProcessing = true;
			ProcessAndRequest();
			if (autoResizeBoxCollider) ResizeCollider();
		}
	}
#endif

#if !UNITY_4_3 && !UNITY_4_5
	static bool mTexRebuildAdded = false;

	protected override void OnEnable ()
	{
		base.OnEnable();
		if (!mTexRebuildAdded)
		{
			mTexRebuildAdded = true;
			Font.textureRebuilt += OnFontChanged;
		}
	}
#endif

	/// <summary>
	/// Determine start-up values.
	/// </summary>

	protected override void OnStart ()
	{
		base.OnStart();

		// Legacy support
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}

		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}

		// Whether this is a premultiplied alpha shader
		mPremultiply = (material != null && material.shader != null && material.shader.name.Contains("Premultiplied"));

		// Request the text within the font
		ProcessAndRequest();
	}

	/// <summary>
	/// UILabel needs additional processing when something changes.
	/// </summary>

	public override void MarkAsChanged ()
	{
		shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	/// <summary>
	/// Process the raw text, called when something changes.
	/// </summary>

	public void ProcessText () { ProcessText(false, true); }

	/// <summary>
	/// Process the raw text, called when something changes.
	/// </summary>

	void ProcessText (bool legacyMode, bool full)
	{
		if (!isValid) return;

		mChanged = true;
		shouldBeProcessed = false;

		float regionX = mDrawRegion.z - mDrawRegion.x;
		float regionY = mDrawRegion.w - mDrawRegion.y;

		NGUIText.rectWidth    = legacyMode ? (mMaxLineWidth  != 0 ? mMaxLineWidth  : 1000000) : width;
		NGUIText.rectHeight   = legacyMode ? (mMaxLineHeight != 0 ? mMaxLineHeight : 1000000) : height;
		NGUIText.regionWidth  = (regionX != 1f) ? Mathf.RoundToInt(NGUIText.rectWidth  * regionX) : NGUIText.rectWidth;
		NGUIText.regionHeight = (regionY != 1f) ? Mathf.RoundToInt(NGUIText.rectHeight * regionY) : NGUIText.rectHeight;

		mFinalFontSize = Mathf.Abs(legacyMode ? Mathf.RoundToInt(cachedTransform.localScale.x) : defaultFontSize);
		mScale = 1f;

		if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 0)
		{
			mProcessedText = "";
			return;
		}

		bool isDynamic = (trueTypeFont != null);

		if (isDynamic && keepCrisp)
		{
			UIRoot rt = root;
			if (rt != null) mDensity = (rt != null) ? rt.pixelSizeAdjustment : 1f;
		}
		else mDensity = 1f;

		if (full) UpdateNGUIText();

		if (mOverflow == Overflow.ResizeFreely)
		{
			NGUIText.rectWidth = 1000000;
			NGUIText.regionWidth = 1000000;

			if (mOverflowWidth > 0)
			{
				NGUIText.rectWidth = Mathf.Min(NGUIText.rectWidth, mOverflowWidth);
				NGUIText.regionWidth = Mathf.Min(NGUIText.regionWidth, mOverflowWidth);
			}
		}

		if (mOverflow == Overflow.ResizeFreely || mOverflow == Overflow.ResizeHeight)
		{
			NGUIText.rectHeight = 1000000;
			NGUIText.regionHeight = 1000000;
		}

		if (mFinalFontSize > 0)
		{
			bool adjustSize = keepCrisp;

			for (int ps = mFinalFontSize; ps > 0; --ps)
			{
				// Adjust either the size, or the scale
				if (adjustSize)
				{
					mFinalFontSize = ps;
					NGUIText.fontSize = mFinalFontSize;
				}
				else
				{
					mScale = (float)ps / mFinalFontSize;
					NGUIText.fontScale = isDynamic ? mScale : ((float)mFontSize / mFont.defaultSize) * mScale;
				}

				NGUIText.Update(false);

				// Wrap the text
				bool fits = NGUIText.WrapText(printedText, out mProcessedText, true, false,
					mOverflowEllipsis && mOverflow == Overflow.ClampContent);

				if (mOverflow == Overflow.ShrinkContent && !fits)
				{
					if (--ps > 1) continue;
					else break;
				}
				else if (mOverflow == Overflow.ResizeFreely)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);

					int w = Mathf.Max(minWidth, Mathf.RoundToInt(mCalculatedSize.x));
					if (regionX != 1f) w = Mathf.RoundToInt(w / regionX);
					int h = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (regionY != 1f) h = Mathf.RoundToInt(h / regionY);

					if ((w & 1) == 1) ++w;
					if ((h & 1) == 1) ++h;

					if (mWidth != w || mHeight != h)
					{
						mWidth = w;
						mHeight = h;
						if (onChange != null) onChange();
					}
				}
				else if (mOverflow == Overflow.ResizeHeight)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					int h = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
					if (regionY != 1f) h = Mathf.RoundToInt(h / regionY);
					if ((h & 1) == 1) ++h;

					if (mHeight != h)
					{
						mHeight = h;
						if (onChange != null) onChange();
					}
				}
				else
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
				}

				// Upgrade to the new system
				if (legacyMode)
				{
					width = Mathf.RoundToInt(mCalculatedSize.x);
					height = Mathf.RoundToInt(mCalculatedSize.y);
					cachedTransform.localScale = Vector3.one;
				}
				break;
			}
		}
		else
		{
			cachedTransform.localScale = Vector3.one;
			mProcessedText = "";
			mScale = 1f;
		}
		
		if (full)
		{
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
	}

	/// <summary>
	/// Text is pixel-perfect when its scale matches the size.
	/// </summary>

	public override void MakePixelPerfect ()
	{
		if (ambigiousFont != null)
		{
			Vector3 pos = cachedTransform.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);

			cachedTransform.localPosition = pos;
			cachedTransform.localScale = Vector3.one;

			if (mOverflow == Overflow.ResizeFreely)
			{
				AssumeNaturalSize();
			}
			else
			{
				int w = width;
				int h = height;

				Overflow over = mOverflow;
				if (over != Overflow.ResizeHeight) mWidth = 100000;
				mHeight = 100000;

				mOverflow = Overflow.ShrinkContent;
				ProcessText(false, true);
				mOverflow = over;

				int minX = Mathf.RoundToInt(mCalculatedSize.x);
				int minY = Mathf.RoundToInt(mCalculatedSize.y);

				minX = Mathf.Max(minX, base.minWidth);
				minY = Mathf.Max(minY, base.minHeight);

				if ((minX & 1) == 1) ++minX;
				if ((minY & 1) == 1) ++minY;

				mWidth = Mathf.Max(w, minX);
				mHeight = Mathf.Max(h, minY);

				MarkAsChanged();
			}
		}
		else base.MakePixelPerfect();
	}

	/// <summary>
	/// Make the label assume its natural size.
	/// </summary>

	public void AssumeNaturalSize ()
	{
		if (ambigiousFont != null)
		{
			mWidth = 100000;
			mHeight = 100000;
			ProcessText(false, true);
			mWidth = Mathf.RoundToInt(mCalculatedSize.x);
			mHeight = Mathf.RoundToInt(mCalculatedSize.y);
			if ((mWidth & 1) == 1) ++mWidth;
			if ((mHeight & 1) == 1) ++mHeight;
			MarkAsChanged();
		}
	}

	[System.Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex (Vector3 worldPos) { return GetCharacterIndexAtPosition(worldPos, false); }

	[System.Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex (Vector2 localPos) { return GetCharacterIndexAtPosition(localPos, false); }

	static List<Vector3> mTempVerts = new List<Vector3>();
	static List<int> mTempIndices = new List<int>();

	/// <summary>
	/// Return the index of the character at the specified world position.
	/// </summary>

	public int GetCharacterIndexAtPosition (Vector3 worldPos, bool precise)
	{
		Vector2 localPos = cachedTransform.InverseTransformPoint(worldPos);
		return GetCharacterIndexAtPosition(localPos, precise);
	}

	/// <summary>
	/// Return the index of the character at the specified local position.
	/// </summary>

	public int GetCharacterIndexAtPosition (Vector2 localPos, bool precise)
	{
		if (isValid)
		{
			string text = processedText;
			if (string.IsNullOrEmpty(text)) return 0;

			UpdateNGUIText();

			if (precise) NGUIText.PrintExactCharacterPositions(text, mTempVerts, mTempIndices);
			else NGUIText.PrintApproximateCharacterPositions(text, mTempVerts, mTempIndices);

			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int retVal = precise ?
					NGUIText.GetExactCharacterIndex(mTempVerts, mTempIndices, localPos) :
					NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos);

				mTempVerts.Clear();
				mTempIndices.Clear();

				NGUIText.bitmapFont = null;
				NGUIText.dynamicFont = null;
				return retVal;
			}

			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
		return 0;
	}

	/// <summary>
	/// Retrieve the word directly below the specified world-space position.
	/// </summary>

	public string GetWordAtPosition (Vector3 worldPos)
	{
		int index = GetCharacterIndexAtPosition(worldPos, true);
		return GetWordAtCharacterIndex(index);
	}

	/// <summary>
	/// Retrieve the word directly below the specified relative-to-label position.
	/// </summary>

	public string GetWordAtPosition (Vector2 localPos)
	{
		int index = GetCharacterIndexAtPosition(localPos, true);
		return GetWordAtCharacterIndex(index);
	}

	/// <summary>
	/// Retrieve the word right under the specified character index.
	/// </summary>

	public string GetWordAtCharacterIndex (int characterIndex)
	{
		string s = printedText;

		if (characterIndex != -1 && characterIndex < s.Length)
		{
#if UNITY_FLASH
			int wordStart = LastIndexOfAny(s, new char[] { ' ', '\n' }, characterIndex) + 1;
			int wordEnd = IndexOfAny(s, new char[] { ' ', '\n', ',', '.' }, characterIndex);
#else
			int wordStart = s.LastIndexOfAny(new char[] {' ', '\n'}, characterIndex) + 1;
			int wordEnd = s.IndexOfAny(new char[] { ' ', '\n', ',', '.' }, characterIndex);
#endif
			if (wordEnd == -1) wordEnd = s.Length;

			if (wordStart != wordEnd)
			{
				int len = wordEnd - wordStart;

				if (len > 0)
				{
					string word = s.Substring(wordStart, len);
					return NGUIText.StripSymbols(word);
				}
			}
		}
		return null;
	}

#if UNITY_FLASH
	/// <summary>
	/// Flash is fail IRL: http://www.tasharen.com/forum/index.php?topic=11390.0
	/// </summary>

	int LastIndexOfAny (string input, char[] any, int start)
	{
		if (start >= 0 && input.Length > 0 && any.Length > 0 && start < input.Length)
		{
			for (int w = start; w >= 0; w--)
			{
				for (int r = 0; r < any.Length; r++)
				{
					if (any[r] == input[w])
					{
						return w;
					}
				}
			}
		}
		return -1;
	}

	/// <summary>
	/// Flash is fail IRL: http://www.tasharen.com/forum/index.php?topic=11390.0
	/// </summary>

	int IndexOfAny (string input, char[] any, int start)
	{
		if (start >= 0 && input.Length > 0 && any.Length > 0 && start < input.Length)
		{
			for (int w = start; w < input.Length; w++)
			{
				for (int r = 0; r < any.Length; r++)
				{
					if (any[r] == input[w])
					{
						return w;
					}
				}
			}
		}
		return -1;
	}
#endif

	/// <summary>
	/// Retrieve the URL directly below the specified world-space position.
	/// </summary>

	public string GetUrlAtPosition (Vector3 worldPos) { return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(worldPos, true)); }

	/// <summary>
	/// Retrieve the URL directly below the specified relative-to-label position.
	/// </summary>

	public string GetUrlAtPosition (Vector2 localPos) { return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(localPos, true)); }

	/// <summary>
	/// Retrieve the URL right under the specified character index.
	/// </summary>

	public string GetUrlAtCharacterIndex (int characterIndex)
	{
		string s = printedText;

		if (characterIndex != -1 && characterIndex < s.Length - 6)
		{
			int linkStart;

			// LastIndexOf() fails if the string happens to begin with the expected text
			if (s[characterIndex] == '[' &&
				s[characterIndex + 1] == 'u' &&
				s[characterIndex + 2] == 'r' &&
				s[characterIndex + 3] == 'l' &&
				s[characterIndex + 4] == '=')
			{
				linkStart = characterIndex;
			}
			else linkStart = s.LastIndexOf("[url=", characterIndex);
			
			if (linkStart == -1) return null;

			linkStart += 5;
			int linkEnd = s.IndexOf("]", linkStart);
			if (linkEnd == -1) return null;

			int urlEnd = s.IndexOf("[/url]", linkEnd);
			if (urlEnd == -1 || characterIndex <= urlEnd)
				return s.Substring(linkStart, linkEnd - linkStart);
		}
		return null;
	}

	/// <summary>
	/// Get the index of the character on the line directly above or below the current index.
	/// </summary>

	public int GetCharacterIndex (int currentIndex, KeyCode key)
	{
		if (isValid)
		{
			string text = processedText;
			if (string.IsNullOrEmpty(text)) return 0;

			int def = defaultFontSize;
			UpdateNGUIText();

			NGUIText.PrintApproximateCharacterPositions(text, mTempVerts, mTempIndices);

			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);

				for (int i = 0, imax = mTempIndices.Count; i < imax; ++i)
				{
					if (mTempIndices[i] == currentIndex)
					{
						// Determine position on the line above or below this character
						Vector2 localPos = mTempVerts[i];

						if (key == KeyCode.UpArrow) localPos.y += def + effectiveSpacingY;
						else if (key == KeyCode.DownArrow) localPos.y -= def + effectiveSpacingY;
						else if (key == KeyCode.Home) localPos.x -= 1000f;
						else if (key == KeyCode.End) localPos.x += 1000f;

						// Find the closest character to this position
						int retVal = NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos);
						if (retVal == currentIndex) break;

						mTempVerts.Clear();
						mTempIndices.Clear();
						return retVal;
					}
				}
				mTempVerts.Clear();
				mTempIndices.Clear();
			}

			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;

			// If the selection doesn't move, then we're at the top or bottom-most line
			if (key == KeyCode.UpArrow || key == KeyCode.Home) return 0;
			if (key == KeyCode.DownArrow || key == KeyCode.End) return text.Length;
		}
		return currentIndex;
	}

	/// <summary>
	/// Fill the specified geometry buffer with vertices that would highlight the current selection.
	/// </summary>

	public void PrintOverlay (int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		if (caret != null) caret.Clear();
		if (highlight != null) highlight.Clear();
		if (!isValid) return;

		string text = processedText;
		UpdateNGUIText();

		int startingCaretVerts = caret.verts.Count;
		Vector2 center = new Vector2(0.5f, 0.5f);
		float alpha = finalAlpha;

		// If we have a highlight to work with, fill the buffer
		if (highlight != null && start != end)
		{
			int startingVertices = highlight.verts.Count;
			NGUIText.PrintCaretAndSelection(text, start, end, caret.verts, highlight.verts);

			if (highlight.verts.Count > startingVertices)
			{
				ApplyOffset(highlight.verts, startingVertices);

				Color c = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * alpha);

				for (int i = startingVertices, imax = highlight.verts.Count; i < imax; ++i)
				{
					highlight.uvs.Add(center);
					highlight.cols.Add(c);
				}
			}
		}
		else NGUIText.PrintCaretAndSelection(text, start, end, caret.verts, null);

		// Fill the caret UVs and colors
		ApplyOffset(caret.verts, startingCaretVerts);
		Color cc = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * alpha);

		for (int i = startingCaretVerts, imax = caret.verts.Count; i < imax; ++i)
		{
			caret.uvs.Add(center);
			caret.cols.Add(cc);
		}

		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
	}

	/// <summary>
	/// Draw the label.
	/// </summary>

	public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (!isValid) return;

		int offset = verts.Count;
		Color col = color;
		col.a = finalAlpha;
		
		if (mFont != null && mFont.premultipliedAlphaShader) col = NGUITools.ApplyPMA(col);

		string text = processedText;
		int start = verts.Count;

		UpdateNGUIText();

		NGUIText.tint = col;
		NGUIText.Print(text, verts, uvs, cols);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;

		// Center the content within the label vertically
		Vector2 pos = ApplyOffset(verts, start);

		// Effects don't work with packed fonts
		if (mFont != null && mFont.packedFontShader) return;

		// Apply an effect if one was requested
		if (effectStyle != Effect.None)
		{
			int end = verts.Count;
			pos.x = mEffectDistance.x;
			pos.y = mEffectDistance.y;

			ApplyShadow(verts, uvs, cols, offset, end, pos.x, -pos.y);

			if ((effectStyle == Effect.Outline) || (effectStyle == Effect.Outline8))
			{
				offset = end;
				end = verts.Count;

				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, pos.y);

				offset = end;
				end = verts.Count;

				ApplyShadow(verts, uvs, cols, offset, end, pos.x, pos.y);

				offset = end;
				end = verts.Count;

				ApplyShadow(verts, uvs, cols, offset, end, -pos.x, -pos.y);

				if (effectStyle == Effect.Outline8)
				{
					offset = end;
					end = verts.Count;

					ApplyShadow(verts, uvs, cols, offset, end, -pos.x, 0);

					offset = end;
					end = verts.Count;

					ApplyShadow(verts, uvs, cols, offset, end, pos.x, 0);

					offset = end;
					end = verts.Count;

					ApplyShadow(verts, uvs, cols, offset, end, 0, pos.y);

					offset = end;
					end = verts.Count;

					ApplyShadow(verts, uvs, cols, offset, end, 0, -pos.y);
				}
			}
		}

		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}

	/// <summary>
	/// Align the vertices, making the label positioned correctly based on the pivot.
	/// Returns the offset that was applied.
	/// </summary>

	public Vector2 ApplyOffset (List<Vector3> verts, int start)
	{
		Vector2 po = pivotOffset;

		float fx = Mathf.Lerp(0f, -mWidth, po.x);
		float fy = Mathf.Lerp(mHeight, 0f, po.y) + Mathf.Lerp((mCalculatedSize.y - mHeight), 0f, po.y);

		fx = Mathf.Round(fx);
		fy = Mathf.Round(fy);

		Vector3 v;

		for (int i = start, imax = verts.Count; i < imax; ++i)
		{
			v = verts[i];
			v.x += fx;
			v.y += fy;
			verts[i] = v;
		}
		return new Vector2(fx, fy);
	}

	/// <summary>
	/// Apply a shadow effect to the buffer.
	/// </summary>

	public void ApplyShadow (List<Vector3> verts, List<Vector2> uvs, List<Color> cols, int start, int end, float x, float y)
	{
		Color c = mEffectColor;
		c.a *= finalAlpha;
		if (bitmapFont != null && bitmapFont.premultipliedAlphaShader) c = NGUITools.ApplyPMA(c);
		Color col = c;

		for (int i = start; i < end; ++i)
		{
			verts.Add(verts[i]);
			uvs.Add(uvs[i]);
			cols.Add(cols[i]);

			var v = verts[i];
			v.x += x;
			v.y += y;
			verts[i] = v;

			Color uc = cols[i];

			if (uc.a == 1f)
			{
				cols[i] = col;
			}
			else
			{
				Color fc = c;
				fc.a = uc.a * c.a;
				cols[i] = fc;
			}
		}
	}

	/// <summary>
	/// Calculate the character index offset necessary in order to print the end of the specified text.
	/// </summary>

	public int CalculateOffsetToFit (string text)
	{
		UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
		int offset = NGUIText.CalculateOffsetToFit(text);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return offset;
	}

	/// <summary>
	/// Convenience function, in case you wanted to associate progress bar, slider or scroll bar's
	/// OnValueChanged function in inspector with a label.
	/// </summary>

	public void SetCurrentProgress ()
	{
		if (UIProgressBar.current != null)
			text = UIProgressBar.current.value.ToString("F");
	}

	/// <summary>
	/// Convenience function, in case you wanted to associate progress bar, slider or scroll bar's
	/// OnValueChanged function in inspector with a label.
	/// </summary>

	public void SetCurrentPercent ()
	{
		if (UIProgressBar.current != null)
			text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
	}

	/// <summary>
	/// Convenience function, in case you wanted to automatically set some label's text
	/// by selecting a value in the UIPopupList.
	/// </summary>

	public void SetCurrentSelection ()
	{
		if (UIPopupList.current != null)
		{
			text = UIPopupList.current.isLocalized ?
				Localization.Get(UIPopupList.current.value) :
				UIPopupList.current.value;
		}
	}

	/// <summary>
	/// Convenience function -- wrap the current text given the label's settings and unlimited height.
	/// </summary>

	public bool Wrap (string text, out string final) { return Wrap(text, out final, 1000000); }

	/// <summary>
	/// Convenience function -- wrap the current text given the label's settings and the given height.
	/// </summary>

	public bool Wrap (string text, out string final, int height)
	{
		UpdateNGUIText();
		NGUIText.rectHeight = height;
		NGUIText.regionHeight = height;
		bool retVal = NGUIText.WrapText(text, out final);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return retVal;
	}

	/// <summary>
	/// Update NGUIText.current with all the properties from this label.
	/// </summary>

	public void UpdateNGUIText ()
	{
		Font ttf = trueTypeFont;
		bool isDynamic = (ttf != null);

		NGUIText.fontSize = mFinalFontSize;
		NGUIText.fontStyle = mFontStyle;
		NGUIText.rectWidth = mWidth;
		NGUIText.rectHeight = mHeight;
		NGUIText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
		NGUIText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));
		NGUIText.gradient = mApplyGradient && (mFont == null || !mFont.packedFontShader);
		NGUIText.gradientTop = mGradientTop;
		NGUIText.gradientBottom = mGradientBottom;
		NGUIText.encoding = mEncoding;
		NGUIText.premultiply = mPremultiply;
		NGUIText.symbolStyle = mSymbols;
		NGUIText.maxLines = mMaxLineCount;
		NGUIText.spacingX = effectiveSpacingX;
		NGUIText.spacingY = effectiveSpacingY;
		NGUIText.fontScale = isDynamic ? mScale : ((float)mFontSize / mFont.defaultSize) * mScale;

		if (mFont != null)
		{
			NGUIText.bitmapFont = mFont;
			
			for (; ; )
			{
				UIFont fnt = NGUIText.bitmapFont.replacement;
				if (fnt == null) break;
				NGUIText.bitmapFont = fnt;
			}

			if (NGUIText.bitmapFont.isDynamic)
			{
				NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
				NGUIText.bitmapFont = null;
			}
			else NGUIText.dynamicFont = null;
		}
		else
		{
			NGUIText.dynamicFont = ttf;
			NGUIText.bitmapFont = null;
		}

		if (isDynamic && keepCrisp)
		{
			UIRoot rt = root;
			if (rt != null) NGUIText.pixelDensity = (rt != null) ? rt.pixelSizeAdjustment : 1f;
		}
		else NGUIText.pixelDensity = 1f;

		if (mDensity != NGUIText.pixelDensity)
		{
			ProcessText(false, false);
			NGUIText.rectWidth = mWidth;
			NGUIText.rectHeight = mHeight;
			NGUIText.regionWidth = Mathf.RoundToInt(mWidth * (mDrawRegion.z - mDrawRegion.x));
			NGUIText.regionHeight = Mathf.RoundToInt(mHeight * (mDrawRegion.w - mDrawRegion.y));
		}

		if (alignment == Alignment.Automatic)
		{
			Pivot p = pivot;

			if (p == Pivot.Left || p == Pivot.TopLeft || p == Pivot.BottomLeft)
			{
				NGUIText.alignment = Alignment.Left;
			}
			else if (p == Pivot.Right || p == Pivot.TopRight || p == Pivot.BottomRight)
			{
				NGUIText.alignment = Alignment.Right;
			}
			else NGUIText.alignment = Alignment.Center;
		}
		else NGUIText.alignment = alignment;

		NGUIText.Update();
	}

	void OnApplicationPause (bool paused)
	{
		if (!paused && mTrueTypeFont != null) Invalidate(false);
	}
}
