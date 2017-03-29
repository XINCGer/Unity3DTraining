//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

// Dynamic font support contributed by the NGUI community members:
// Unisip, zh4ox, Mudwiz, Nicki, DarkMagicCK.

#if !UNITY_3_5
#define DYNAMIC_FONT
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// UIFont contains everything needed to be able to print text.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Font")]
public class UIFont : MonoBehaviour
{
	[HideInInspector][SerializeField] Material mMat;
	[HideInInspector][SerializeField] Rect mUVRect = new Rect(0f, 0f, 1f, 1f);
	[HideInInspector][SerializeField] BMFont mFont = new BMFont();
	[HideInInspector][SerializeField] UIAtlas mAtlas;
	[HideInInspector][SerializeField] UIFont mReplacement;

	// List of symbols, such as emoticons like ":)", ":(", etc
	[HideInInspector][SerializeField] List<BMSymbol> mSymbols = new List<BMSymbol>();

	// Used for dynamic fonts
	[HideInInspector][SerializeField] Font mDynamicFont;
	[HideInInspector][SerializeField] int mDynamicFontSize = 16;
	[HideInInspector][SerializeField] FontStyle mDynamicFontStyle = FontStyle.Normal;

	// Cached value
	[System.NonSerialized]
	UISpriteData mSprite = null;
	int mPMA = -1;
	int mPacked = -1;

	/// <summary>
	/// Access to the BMFont class directly.
	/// </summary>

	public BMFont bmFont
	{
		get
		{
			return (mReplacement != null) ? mReplacement.bmFont : mFont;
		}
		set
		{
			if (mReplacement != null) mReplacement.bmFont = value;
			else mFont = value;
		}
	}

	/// <summary>
	/// Original width of the font's texture in pixels.
	/// </summary>

	public int texWidth
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texWidth : ((mFont != null) ? mFont.texWidth : 1);
		}
		set
		{
			if (mReplacement != null) mReplacement.texWidth = value;
			else if (mFont != null) mFont.texWidth = value;
		}
	}

	/// <summary>
	/// Original height of the font's texture in pixels.
	/// </summary>

	public int texHeight
	{
		get
		{
			return (mReplacement != null) ? mReplacement.texHeight : ((mFont != null) ? mFont.texHeight : 1);
		}
		set
		{
			if (mReplacement != null) mReplacement.texHeight = value;
			else if (mFont != null) mFont.texHeight = value;
		}
	}

	/// <summary>
	/// Whether the font has any symbols defined.
	/// </summary>

	public bool hasSymbols { get { return (mReplacement != null) ? mReplacement.hasSymbols : (mSymbols != null && mSymbols.Count != 0); } }

	/// <summary>
	/// List of symbols within the font.
	/// </summary>

	public List<BMSymbol> symbols { get { return (mReplacement != null) ? mReplacement.symbols : mSymbols; } }

	/// <summary>
	/// Atlas used by the font, if any.
	/// </summary>

	public UIAtlas atlas
	{
		get
		{
			return (mReplacement != null) ? mReplacement.atlas : mAtlas;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.atlas = value;
			}
			else if (mAtlas != value)
			{
				mPMA = -1;
				mAtlas = value;

				if (mAtlas != null)
				{
					mMat = mAtlas.spriteMaterial;
					if (sprite != null) mUVRect = uvRect;
				}
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Get or set the material used by this font.
	/// </summary>

	public Material material
	{
		get
		{
			if (mReplacement != null) return mReplacement.material;

			if (mAtlas != null) return mAtlas.spriteMaterial;

			if (mMat != null)
			{
				if (mDynamicFont != null && mMat != mDynamicFont.material)
				{
					mMat.mainTexture = mDynamicFont.material.mainTexture;
				}
				return mMat;
			}

			if (mDynamicFont != null)
			{
				return mDynamicFont.material;
			}
			return null;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.material = value;
			}
			else if (mMat != value)
			{
				mPMA = -1;
				mMat = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the font is using a premultiplied alpha material.
	/// </summary>

	[System.Obsolete("Use UIFont.premultipliedAlphaShader instead")]
	public bool premultipliedAlpha { get { return premultipliedAlphaShader; } }

	/// <summary>
	/// Whether the font is using a premultiplied alpha material.
	/// </summary>

	public bool premultipliedAlphaShader
	{
		get
		{
			if (mReplacement != null) return mReplacement.premultipliedAlphaShader;

			if (mAtlas != null) return mAtlas.premultipliedAlpha;

			if (mPMA == -1)
			{
				Material mat = material;
				mPMA = (mat != null && mat.shader != null && mat.shader.name.Contains("Premultiplied")) ? 1 : 0;
			}
			return (mPMA == 1);
		}
	}

	/// <summary>
	/// Whether the font is a packed font.
	/// </summary>

	public bool packedFontShader
	{
		get
		{
			if (mReplacement != null) return mReplacement.packedFontShader;
			if (mAtlas != null) return false;

			if (mPacked == -1)
			{
				Material mat = material;
				mPacked = (mat != null && mat.shader != null && mat.shader.name.Contains("Packed")) ? 1 : 0;
			}
			return (mPacked == 1);
		}
	}

	/// <summary>
	/// Convenience function that returns the texture used by the font.
	/// </summary>

	public Texture2D texture
	{
		get
		{
			if (mReplacement != null) return mReplacement.texture;
			Material mat = material;
			return (mat != null) ? mat.mainTexture as Texture2D : null;
		}
	}

	/// <summary>
	/// Offset and scale applied to all UV coordinates.
	/// </summary>

	public Rect uvRect
	{
		get
		{
			if (mReplacement != null) return mReplacement.uvRect;
			return (mAtlas != null && sprite != null) ? mUVRect : new Rect(0f, 0f, 1f, 1f);
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.uvRect = value;
			}
			else if (sprite == null && mUVRect != value)
			{
				mUVRect = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Sprite used by the font, if any.
	/// </summary>

	public string spriteName
	{
		get
		{
			return (mReplacement != null) ? mReplacement.spriteName : mFont.spriteName;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.spriteName = value;
			}
			else if (mFont.spriteName != value)
			{
				mFont.spriteName = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether this is a valid font.
	/// </summary>

#if DYNAMIC_FONT
	public bool isValid { get { return mDynamicFont != null || mFont.isValid; } }
#else
	public bool isValid { get { return mFont.isValid; } }
#endif

	[System.Obsolete("Use UIFont.defaultSize instead")]
	public int size
	{
		get { return defaultSize; }
		set { defaultSize = value; }
	}

	/// <summary>
	/// Pixel-perfect size of this font.
	/// </summary>

	public int defaultSize
	{
		get
		{
			if (mReplacement != null) return mReplacement.defaultSize;
			if (isDynamic || mFont == null) return mDynamicFontSize;
			return mFont.charSize;
		}
		set
		{
			if (mReplacement != null) mReplacement.defaultSize = value;
			else mDynamicFontSize = value;
		}
	}

	/// <summary>
	/// Retrieves the sprite used by the font, if any.
	/// </summary>

	public UISpriteData sprite
	{
		get
		{
			if (mReplacement != null) return mReplacement.sprite;

			if (mSprite == null && mAtlas != null && !string.IsNullOrEmpty(mFont.spriteName))
			{
				mSprite = mAtlas.GetSprite(mFont.spriteName);

				if (mSprite == null) mSprite = mAtlas.GetSprite(name);
				if (mSprite == null) mFont.spriteName = null;
				else UpdateUVRect();

				for (int i = 0, imax = mSymbols.Count; i < imax; ++i)
					symbols[i].MarkAsChanged();
			}
			return mSprite;
		}
	}

	/// <summary>
	/// Setting a replacement atlas value will cause everything using this font to use the replacement font instead.
	/// Suggested use: set up all your widgets to use a dummy font that points to the real font. Switching that font to
	/// another one (for example an eastern language one) is then a simple matter of setting this field on your dummy font.
	/// </summary>

	public UIFont replacement
	{
		get
		{
			return mReplacement;
		}
		set
		{
			UIFont rep = value;
			if (rep == this) rep = null;

			if (mReplacement != rep)
			{
				if (rep != null && rep.replacement == this) rep.replacement = null;
				if (mReplacement != null) MarkAsChanged();
				mReplacement = rep;

				if (rep != null)
				{
					mPMA = -1;
					mMat = null;
					mFont = null;
					mDynamicFont = null;
				}
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the font is dynamic.
	/// </summary>

	public bool isDynamic { get { return (mReplacement != null) ? mReplacement.isDynamic : (mDynamicFont != null); } }

	/// <summary>
	/// Get or set the dynamic font source.
	/// </summary>

	public Font dynamicFont
	{
		get
		{
			return (mReplacement != null) ? mReplacement.dynamicFont : mDynamicFont;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFont = value;
			}
			else if (mDynamicFont != value)
			{
				if (mDynamicFont != null) material = null;
				mDynamicFont = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Get or set the dynamic font's style.
	/// </summary>

	public FontStyle dynamicFontStyle
	{
		get
		{
			return (mReplacement != null) ? mReplacement.dynamicFontStyle : mDynamicFontStyle;
		}
		set
		{
			if (mReplacement != null)
			{
				mReplacement.dynamicFontStyle = value;
			}
			else if (mDynamicFontStyle != value)
			{
				mDynamicFontStyle = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Trim the glyphs, making sure they never go past the trimmed texture bounds.
	/// </summary>

	void Trim ()
	{
		Texture tex = mAtlas.texture;

		if (tex != null && mSprite != null)
		{
			Rect full = NGUIMath.ConvertToPixels(mUVRect, texture.width, texture.height, true);
			Rect trimmed = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);

			int xMin = Mathf.RoundToInt(trimmed.xMin - full.xMin);
			int yMin = Mathf.RoundToInt(trimmed.yMin - full.yMin);
			int xMax = Mathf.RoundToInt(trimmed.xMax - full.xMin);
			int yMax = Mathf.RoundToInt(trimmed.yMax - full.yMin);

			mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	/// <summary>
	/// Helper function that determines whether the font uses the specified one, taking replacements into account.
	/// </summary>

	bool References (UIFont font)
	{
		if (font == null) return false;
		if (font == this) return true;
		return (mReplacement != null) ? mReplacement.References(font) : false;
	}

	/// <summary>
	/// Helper function that determines whether the two atlases are related.
	/// </summary>

	static public bool CheckIfRelated (UIFont a, UIFont b)
	{
		if (a == null || b == null) return false;
#if DYNAMIC_FONT && !UNITY_FLASH
		if (a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0]) return true;
#endif
		return a == b || a.References(b) || b.References(a);
	}

	Texture dynamicTexture
	{
		get
		{
			if (mReplacement) return mReplacement.dynamicTexture;
			if (isDynamic) return mDynamicFont.material.mainTexture;
			return null;
		}
	}

	/// <summary>
	/// Refresh all labels that use this font.
	/// </summary>

	public void MarkAsChanged ()
	{
#if UNITY_EDITOR
		NGUITools.SetDirty(gameObject);
#endif
		if (mReplacement != null) mReplacement.MarkAsChanged();

		mSprite = null;
		UILabel[] labels = NGUITools.FindActive<UILabel>();

		for (int i = 0, imax = labels.Length; i < imax; ++i)
		{
			UILabel lbl = labels[i];

			if (lbl.enabled && NGUITools.GetActive(lbl.gameObject) && CheckIfRelated(this, lbl.bitmapFont))
			{
				UIFont fnt = lbl.bitmapFont;
				lbl.bitmapFont = null;
				lbl.bitmapFont = fnt;
			}
		}

		// Clear all symbols
		for (int i = 0, imax = symbols.Count; i < imax; ++i)
			symbols[i].MarkAsChanged();
	}

	/// <summary>
	/// Forcefully update the font's sprite reference.
	/// </summary>

	public void UpdateUVRect ()
	{
		if (mAtlas == null) return;
		Texture tex = mAtlas.texture;

		if (tex != null)
		{
			mUVRect = new Rect(
				mSprite.x - mSprite.paddingLeft,
				mSprite.y - mSprite.paddingTop,
				mSprite.width + mSprite.paddingLeft + mSprite.paddingRight,
				mSprite.height + mSprite.paddingTop + mSprite.paddingBottom);

			mUVRect = NGUIMath.ConvertToTexCoords(mUVRect, tex.width, tex.height);
#if UNITY_EDITOR
			// The font should always use the original texture size
			if (mFont != null)
			{
				float tw = (float)mFont.texWidth / tex.width;
				float th = (float)mFont.texHeight / tex.height;

				if (tw != mUVRect.width || th != mUVRect.height)
				{
					//Debug.LogWarning("Font sprite size doesn't match the expected font texture size.\n" +
					//	"Did you use the 'inner padding' setting on the Texture Packer? It must remain at '0'.", this);
					mUVRect.width = tw;
					mUVRect.height = th;
				}
			}
#endif
			// Trimmed sprite? Trim the glyphs
			if (mSprite.hasPadding) Trim();
		}
	}

	/// <summary>
	/// Retrieve the specified symbol, optionally creating it if it's missing.
	/// </summary>

	BMSymbol GetSymbol (string sequence, bool createIfMissing)
	{
		for (int i = 0, imax = mSymbols.Count; i < imax; ++i)
		{
			BMSymbol sym = mSymbols[i];
			if (sym.sequence == sequence) return sym;
		}

		if (createIfMissing)
		{
			BMSymbol sym = new BMSymbol();
			sym.sequence = sequence;
			mSymbols.Add(sym);
			return sym;
		}
		return null;
	}

	/// <summary>
	/// Retrieve the symbol at the beginning of the specified sequence, if a match is found.
	/// </summary>

	public BMSymbol MatchSymbol (string text, int offset, int textLength)
	{
		// No symbols present
		int count = mSymbols.Count;
		if (count == 0) return null;
		textLength -= offset;

		// Run through all symbols
		for (int i = 0; i < count; ++i)
		{
			BMSymbol sym = mSymbols[i];

			// If the symbol's length is longer, move on
			int symbolLength = sym.length;
			if (symbolLength == 0 || textLength < symbolLength) continue;

			bool match = true;

			// Match the characters
			for (int c = 0; c < symbolLength; ++c)
			{
				if (text[offset + c] != sym.sequence[c])
				{
					match = false;
					break;
				}
			}

			// Match found
			if (match && sym.Validate(atlas)) return sym;
		}
		return null;
	}

	/// <summary>
	/// Add a new symbol to the font.
	/// </summary>

	public void AddSymbol (string sequence, string spriteName)
	{
		BMSymbol symbol = GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		MarkAsChanged();
	}

	/// <summary>
	/// Remove the specified symbol from the font.
	/// </summary>

	public void RemoveSymbol (string sequence)
	{
		BMSymbol symbol = GetSymbol(sequence, false);
		if (symbol != null) symbols.Remove(symbol);
		MarkAsChanged();
	}

	/// <summary>
	/// Change an existing symbol's sequence to the specified value.
	/// </summary>

	public void RenameSymbol (string before, string after)
	{
		BMSymbol symbol = GetSymbol(before, false);
		if (symbol != null) symbol.sequence = after;
		MarkAsChanged();
	}

	/// <summary>
	/// Whether the specified sprite is being used by the font.
	/// </summary>

	public bool UsesSprite (string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(spriteName))
				return true;

			for (int i = 0, imax = symbols.Count; i < imax; ++i)
			{
				BMSymbol sym = symbols[i];
				if (s.Equals(sym.spriteName))
					return true;
			}
		}
		return false;
	}
}

