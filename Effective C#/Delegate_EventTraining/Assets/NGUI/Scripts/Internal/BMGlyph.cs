//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Glyph structure used by BMFont. For more information see http://www.angelcode.com/products/bmfont/
/// </summary>

[System.Serializable]
public class BMGlyph
{
	public int index;	// Index of this glyph (used by BMFont)
	public int x;		// Offset from the left side of the texture to the left side of the glyph
	public int y;		// Offset from the top of the texture to the top of the glyph
	public int width;	// Glyph's width in pixels
	public int height;	// Glyph's height in pixels
	public int offsetX;	// Offset to apply to the cursor's left position before drawing this glyph
	public int offsetY; // Offset to apply to the cursor's top position before drawing this glyph
	public int advance;	// How much to move the cursor after printing this character
	public int channel;	// Channel mask (in most cases this will be 15 (RGBA, 1+2+4+8)
	public List<int> kerning;

	/// <summary>
	/// Retrieves the special amount by which to adjust the cursor position, given the specified previous character.
	/// </summary>

	public int GetKerning (int previousChar)
	{
		if (kerning != null && previousChar != 0)
		{
			for (int i = 0, imax = kerning.Count; i < imax; i += 2)
				if (kerning[i] == previousChar)
					return kerning[i + 1];
		}
		return 0;
	}

	/// <summary>
	/// Add a new kerning entry to the character (or adjust an existing one).
	/// </summary>

	public void SetKerning (int previousChar, int amount)
	{
		if (kerning == null) kerning = new List<int>();

		for (int i = 0; i < kerning.Count; i += 2)
		{
			if (kerning[i] == previousChar)
			{
				kerning[i + 1] = amount;
				return;
			}
		}

		kerning.Add(previousChar);
		kerning.Add(amount);
	}

	/// <summary>
	/// Trim the glyph, given the specified minimum and maximum dimensions in pixels.
	/// </summary>

	public void Trim (int xMin, int yMin, int xMax, int yMax)
	{
		int x1 = x + width;
		int y1 = y + height;

		if (x < xMin)
		{
			int offset = xMin - x;
			x += offset;
			width -= offset;
			offsetX += offset;
		}

		if (y < yMin)
		{
			int offset = yMin - y;
			y += offset;
			height -= offset;
			offsetY += offset;
		}

		if (x1 > xMax) width  -= x1 - xMax;
		if (y1 > yMax) height -= y1 - yMax;
	}
}
