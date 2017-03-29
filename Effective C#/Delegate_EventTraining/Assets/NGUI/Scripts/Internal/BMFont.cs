//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// BMFont reader. C# implementation of http://www.angelcode.com/products/bmfont/
/// </summary>

[System.Serializable]
public class BMFont
{
	[HideInInspector][SerializeField] int mSize = 16;			// How much to move the cursor when moving to the next line
	[HideInInspector][SerializeField] int mBase = 0;			// Offset from the top of the line to the base of each character
	[HideInInspector][SerializeField] int mWidth = 0;			// Original width of the texture
	[HideInInspector][SerializeField] int mHeight = 0;			// Original height of the texture
	[HideInInspector][SerializeField] string mSpriteName;

	// List of serialized glyphs
	[HideInInspector][SerializeField] List<BMGlyph> mSaved = new List<BMGlyph>();

	// Actual glyphs that we'll be working with are stored in a dictionary, making the lookup faster
	Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();

	/// <summary>
	/// Whether the font can be used.
	/// </summary>

	public bool isValid { get { return (mSaved.Count > 0); } }

	/// <summary>
	/// Size of this font (for example 32 means 32 pixels).
	/// </summary>

	public int charSize { get { return mSize; } set { mSize = value; } }

	/// <summary>
	/// Base offset applied to characters.
	/// </summary>

	public int baseOffset { get { return mBase; } set { mBase = value; } }

	/// <summary>
	/// Original width of the texture.
	/// </summary>

	public int texWidth { get { return mWidth; } set { mWidth = value; } }

	/// <summary>
	/// Original height of the texture.
	/// </summary>

	public int texHeight { get { return mHeight; } set { mHeight = value; } }

	/// <summary>
	/// Number of valid glyphs.
	/// </summary>

	public int glyphCount { get { return isValid ? mSaved.Count : 0; } }

	/// <summary>
	/// Original name of the sprite that the font is expecting to find (usually the name of the texture).
	/// </summary>

	public string spriteName { get { return mSpriteName; } set { mSpriteName = value; } }

	/// <summary>
	/// Access to BMFont's entire set of glyphs.
	/// </summary>

	public List<BMGlyph> glyphs { get { return mSaved; } }

	/// <summary>
	/// Helper function that retrieves the specified glyph, creating it if necessary.
	/// </summary>

	public BMGlyph GetGlyph (int index, bool createIfMissing)
	{
		// Get the requested glyph
		BMGlyph glyph = null;

		if (mDict.Count == 0)
		{
			// Populate the dictionary for faster access
			for (int i = 0, imax = mSaved.Count; i < imax; ++i)
			{
				BMGlyph bmg = mSaved[i];
				mDict.Add(bmg.index, bmg);
			}
		}

		// Saved check is here so that the function call is not needed if it's true
		if (!mDict.TryGetValue(index, out glyph) && createIfMissing)
		{
			glyph = new BMGlyph();
			glyph.index = index;
			mSaved.Add(glyph);
			mDict.Add(index, glyph);
		}
		return glyph;
	}

	/// <summary>
	/// Retrieve the specified glyph, if it's present.
	/// </summary>

	public BMGlyph GetGlyph (int index) { return GetGlyph(index, false); }

	/// <summary>
	/// Clear the glyphs.
	/// </summary>

	public void Clear ()
	{
		mDict.Clear();
		mSaved.Clear();
	}

	/// <summary>
	/// Trim the glyphs, ensuring that they will never go past the specified bounds.
	/// </summary>

	public void Trim (int xMin, int yMin, int xMax, int yMax)
	{
		if (isValid)
		{
			for (int i = 0, imax = mSaved.Count; i < imax; ++i)
			{
				BMGlyph glyph = mSaved[i];
				if (glyph != null) glyph.Trim(xMin, yMin, xMax, yMax);
			}
		}
	}
}
