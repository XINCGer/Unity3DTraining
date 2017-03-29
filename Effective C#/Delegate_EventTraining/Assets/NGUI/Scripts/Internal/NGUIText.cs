//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5
#define DYNAMIC_FONT
#endif

using UnityEngine;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

/// <summary>
/// Helper class containing functionality related to using dynamic fonts.
/// </summary>

static public class NGUIText
{
	public enum Alignment
	{
		Automatic,
		Left,
		Center,
		Right,
		Justified,
	}

	public enum SymbolStyle
	{
		None,
		Normal,
		Colored,
	}

	public class GlyphInfo
	{
		public Vector2 v0;
		public Vector2 v1;
		public Vector2 u0;
		public Vector2 u1;
		public Vector2 u2;
		public Vector2 u3;
		public float advance = 0f;
		public int channel = 0;
	}

	/// <summary>
	/// When printing text, a lot of additional data must be passed in. In order to save allocations,
	/// this data is not passed at all, but is rather set in a single place before calling the functions that use it.
	/// </summary>

	static public UIFont bitmapFont;
#if DYNAMIC_FONT
	static public Font dynamicFont;
#endif
	static public GlyphInfo glyph = new GlyphInfo();

	static public int fontSize = 16;
	static public float fontScale = 1f;
	static public float pixelDensity = 1f;
	static public FontStyle fontStyle = FontStyle.Normal;
	static public Alignment alignment = Alignment.Left;
	static public Color tint = Color.white;

	static public int rectWidth = 1000000;
	static public int rectHeight = 1000000;
	static public int regionWidth = 1000000;
	static public int regionHeight = 1000000;
	static public int maxLines = 0;

	static public bool gradient = false;
	static public Color gradientBottom = Color.white;
	static public Color gradientTop = Color.white;

	static public bool encoding = false;
	static public float spacingX = 0f;
	static public float spacingY = 0f;
	static public bool premultiply = false;
	static public SymbolStyle symbolStyle;

	static public int finalSize = 0;
	static public float finalSpacingX = 0f;
	static public float finalLineHeight = 0f;
	static public float baseline = 0f;
	static public bool useSymbols = false;

	/// <summary>
	/// Recalculate the 'final' values.
	/// </summary>

	static public void Update () { Update(true); }

	/// <summary>
	/// Recalculate the 'final' values.
	/// </summary>

	static public void Update (bool request)
	{
		finalSize = Mathf.RoundToInt(fontSize / pixelDensity);
		finalSpacingX = spacingX * fontScale;
		finalLineHeight = (fontSize + spacingY) * fontScale;
		useSymbols = (dynamicFont != null || bitmapFont != null) && encoding && symbolStyle != SymbolStyle.None;

#if DYNAMIC_FONT
		Font font = dynamicFont;

		if (font != null && request)
		{
			font.RequestCharactersInTexture(")_-", finalSize, fontStyle);

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (!font.GetCharacterInfo(')', out mTempChar, finalSize, fontStyle) || mTempChar.vert.height == 0f)
			{
				font.RequestCharactersInTexture("A", finalSize, fontStyle);
				{
					if (!font.GetCharacterInfo('A', out mTempChar, finalSize, fontStyle))
					{
						baseline = 0f;
						return;
					}
				}
			}

			float y0 = mTempChar.vert.yMax;
			float y1 = mTempChar.vert.yMin;
#else
			if (!font.GetCharacterInfo(')', out mTempChar, finalSize, fontStyle) || mTempChar.maxY == 0f)
			{
				font.RequestCharactersInTexture("A", finalSize, fontStyle);
				{
					if (!font.GetCharacterInfo('A', out mTempChar, finalSize, fontStyle))
					{
						baseline = 0f;
						return;
					}
				}
			}

			float y0 = mTempChar.maxY;
			float y1 = mTempChar.minY;
#endif
			baseline = Mathf.Round(y0 + (finalSize - y0 + y1) * 0.5f);
		}
#endif
	}

	/// <summary>
	/// Prepare to use the specified text.
	/// </summary>

	static public void Prepare (string text)
	{
#if DYNAMIC_FONT
		if (dynamicFont != null)
			dynamicFont.RequestCharactersInTexture(text, finalSize, fontStyle);
#endif
	}

	/// <summary>
	/// Get the specified symbol.
	/// </summary>

	static public BMSymbol GetSymbol (string text, int index, int textLength)
	{
		return (bitmapFont != null) ? bitmapFont.MatchSymbol(text, index, textLength) : null;
	}

	/// <summary>
	/// Get the width of the specified glyph. Returns zero if the glyph could not be retrieved.
	/// </summary>

	static public float GetGlyphWidth (int ch, int prev)
	{
		if (bitmapFont != null)
		{
			bool thinSpace = false;

			if (ch == '\u2009')
			{
				thinSpace = true;
				ch = ' ';
			}

			BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

			if (bmg != null)
			{
				int adv = bmg.advance;
				if (thinSpace) adv >>= 1;
				return fontScale * ((prev != 0) ? adv + bmg.GetKerning(prev) : bmg.advance);
			}
		}
#if DYNAMIC_FONT
		else if (dynamicFont != null)
		{
			if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
 #if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				return mTempChar.width * fontScale * pixelDensity;
 #else
				return mTempChar.advance * fontScale * pixelDensity;
 #endif
		}
#endif
		return 0f;
	}

	/// <summary>
	/// Get the specified glyph.
	/// </summary>

	static public GlyphInfo GetGlyph (int ch, int prev)
	{
		if (bitmapFont != null)
		{
			bool thinSpace = false;

			if (ch == '\u2009')
			{
				thinSpace = true;
				ch = ' ';
			}

			BMGlyph bmg = bitmapFont.bmFont.GetGlyph(ch);

			if (bmg != null)
			{
				int kern = (prev != 0) ? bmg.GetKerning(prev) : 0;
				glyph.v0.x = (prev != 0) ? bmg.offsetX + kern : bmg.offsetX;
				glyph.v1.y = -bmg.offsetY;

				glyph.v1.x = glyph.v0.x + bmg.width;
				glyph.v0.y = glyph.v1.y - bmg.height;

				glyph.u0.x = bmg.x;
				glyph.u0.y = bmg.y + bmg.height;

				glyph.u2.x = bmg.x + bmg.width;
				glyph.u2.y = bmg.y;

				glyph.u1.x = glyph.u0.x;
				glyph.u1.y = glyph.u2.y;

				glyph.u3.x = glyph.u2.x;
				glyph.u3.y = glyph.u0.y;

				int adv = bmg.advance;
				if (thinSpace) adv >>= 1;
				glyph.advance = adv + kern;
				glyph.channel = bmg.channel;

				if (fontScale != 1f)
				{
					glyph.v0 *= fontScale;
					glyph.v1 *= fontScale;
					glyph.advance *= fontScale;
				}
				return glyph;
			}
		}
#if DYNAMIC_FONT
		else if (dynamicFont != null)
		{
			if (dynamicFont.GetCharacterInfo((char)ch, out mTempChar, finalSize, fontStyle))
			{
 #if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				glyph.v0.x = mTempChar.vert.xMin;
				glyph.v1.x = glyph.v0.x + mTempChar.vert.width;

				glyph.v0.y = mTempChar.vert.yMax - baseline;
				glyph.v1.y = glyph.v0.y - mTempChar.vert.height;

				glyph.u0.x = mTempChar.uv.xMin;
				glyph.u0.y = mTempChar.uv.yMin;

				glyph.u2.x = mTempChar.uv.xMax;
				glyph.u2.y = mTempChar.uv.yMax;

				if (mTempChar.flipped)
				{
					glyph.u1 = new Vector2(glyph.u2.x, glyph.u0.y);
					glyph.u3 = new Vector2(glyph.u0.x, glyph.u2.y);
				}
				else
				{
					glyph.u1 = new Vector2(glyph.u0.x, glyph.u2.y);
					glyph.u3 = new Vector2(glyph.u2.x, glyph.u0.y);
				}

				glyph.advance = mTempChar.width;
				glyph.channel = 0;
#else
				glyph.v0.x = mTempChar.minX;
				glyph.v1.x = mTempChar.maxX;

				glyph.v0.y = mTempChar.maxY - baseline;
				glyph.v1.y = mTempChar.minY - baseline;

				glyph.u0 = mTempChar.uvTopLeft;
				glyph.u1 = mTempChar.uvBottomLeft;
				glyph.u2 = mTempChar.uvBottomRight;
				glyph.u3 = mTempChar.uvTopRight;

				glyph.advance = mTempChar.advance;
				glyph.channel = 0;
 #endif
				glyph.v0.x = Mathf.Round(glyph.v0.x);
				glyph.v0.y = Mathf.Round(glyph.v0.y);
				glyph.v1.x = Mathf.Round(glyph.v1.x);
				glyph.v1.y = Mathf.Round(glyph.v1.y);

				float pd = fontScale * pixelDensity;

				if (pd != 1f)
				{
					glyph.v0 *= pd;
					glyph.v1 *= pd;
					glyph.advance *= pd;
				}
				return glyph;
			}
		}
#endif
		return null;
	}

	static Color mInvisible = new Color(0f, 0f, 0f, 0f);
	static BetterList<Color> mColors = new BetterList<Color>();
	static float mAlpha = 1f;
#if DYNAMIC_FONT
	static CharacterInfo mTempChar;
#endif

	/// <summary>
	/// Parse Aa syntax alpha encoded in the string.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float ParseAlpha (string text, int index)
	{
		int a = (NGUIMath.HexToDecimal(text[index + 1]) << 4) | NGUIMath.HexToDecimal(text[index + 2]);
		return Mathf.Clamp01(a / 255f);
	}

	/// <summary>
	/// Parse a RrGgBb color encoded in the string.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public Color ParseColor (string text, int offset = 0) { return ParseColor24(text, offset); }

	/// <summary>
	/// Parse a RrGgBb color encoded in the string.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public Color ParseColor24 (string text, int offset = 0)
	{
		int r = (NGUIMath.HexToDecimal(text[offset])     << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int g = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int b = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		float f = 1f / 255f;
		return new Color(f * r, f * g, f * b);
	}

	/// <summary>
	/// Parse a RrGgBbAa color encoded in the string.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public Color ParseColor32 (string text, int offset)
	{
		int r = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
		int g = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
		int b = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
		int a = (NGUIMath.HexToDecimal(text[offset + 6]) << 4) | NGUIMath.HexToDecimal(text[offset + 7]);
		float f = 1f / 255f;
		return new Color(f * r, f * g, f * b, f * a);
	}

	/// <summary>
	/// The reverse of ParseColor -- encodes a color in RrGgBb format.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string EncodeColor (Color c) { return EncodeColor24(c); }

	/// <summary>
	/// Convenience function that wraps the specified text block in a color tag.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string EncodeColor (string text, Color c) { return "[c][" + EncodeColor24(c) + "]" + text + "[-][/c]"; }

	/// <summary>
	/// The reverse of ParseAlpha -- encodes a color in Aa format.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string EncodeAlpha (float a)
	{
		int i = Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255);
		return NGUIMath.DecimalToHex8(i);
	}

	/// <summary>
	/// The reverse of ParseColor24 -- encodes a color in RrGgBb format.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string EncodeColor24 (Color c)
	{
		int i = 0xFFFFFF & (NGUIMath.ColorToInt(c) >> 8);
		return NGUIMath.DecimalToHex24(i);
	}

	/// <summary>
	/// The reverse of ParseColor32 -- encodes a color in RrGgBb format.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string EncodeColor32 (Color c)
	{
		int i = NGUIMath.ColorToInt(c);
		return NGUIMath.DecimalToHex32(i);
	}

	/// <summary>
	/// Parse an embedded symbol, such as [FFAA00] (set color) or [-] (undo color change). Returns whether the index was adjusted.
	/// </summary>

	static public bool ParseSymbol (string text, ref int index)
	{
		int n = 1;
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strikethrough = false;
		bool ignoreColor = false;
		return ParseSymbol(text, ref index, null, false, ref n, ref bold, ref italic, ref underline, ref strikethrough, ref ignoreColor);
	}

	/// <summary>
	/// Whether the specified character falls under the 'hex' character category (0-9, A-F).
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public bool IsHex (char ch)
	{
		return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
	}

	/// <summary>
	/// Parse the symbol, if possible. Returns 'true' if the 'index' was adjusted.
	/// Advanced symbol support originally contributed by Rudy Pangestu.
	/// </summary>

	static public bool ParseSymbol (string text, ref int index, BetterList<Color> colors, bool premultiply,
		ref int sub, ref bool bold, ref bool italic, ref bool underline, ref bool strike, ref bool ignoreColor)
	{
		int length = text.Length;

		if (index + 3 > length || text[index] != '[') return false;

		if (text[index + 2] == ']')
		{
			if (text[index + 1] == '-')
			{
				if (colors != null && colors.size > 1)
					colors.RemoveAt(colors.size - 1);
				index += 3;
				return true;
			}

			string sub3 = text.Substring(index, 3);

			switch (sub3)
			{
				case "[b]":
				case "[B]":
				bold = true;
				index += 3;
				return true;

				case "[i]":
				case "[I]":
				italic = true;
				index += 3;
				return true;

				case "[u]":
				case "[U]":
				underline = true;
				index += 3;
				return true;

				case "[s]":
				case "[S]":
				strike = true;
				index += 3;
				return true;

				case "[c]":
				case "[C]":
				ignoreColor = true;
				index += 3;
				return true;
			}
		}

		if (index + 4 > length) return false;

		if (text[index + 3] == ']')
		{
			string sub4 = text.Substring(index, 4);

			switch (sub4)
			{
				case "[/b]":
				case "[/B]":
				bold = false;
				index += 4;
				return true;

				case "[/i]":
				case "[/I]":
				italic = false;
				index += 4;
				return true;

				case "[/u]":
				case "[/U]":
				underline = false;
				index += 4;
				return true;

				case "[/s]":
				case "[/S]":
				strike = false;
				index += 4;
				return true;

				case "[/c]":
				case "[/C]":
				ignoreColor = false;
				index += 4;
				return true;

				default:
				{
					char ch0 = text[index + 1];
					char ch1 = text[index + 2];

					if (IsHex(ch0) && IsHex(ch1))
					{
						int a = (NGUIMath.HexToDecimal(ch0) << 4) | NGUIMath.HexToDecimal(ch1);
						mAlpha = a / 255f;
						index += 4;
						return true;
					}
				}
				break;
			}
		}

		if (index + 5 > length) return false;

		if (text[index + 4] == ']')
		{
			string sub5 = text.Substring(index, 5);

			switch (sub5)
			{
				case "[sub]":
				case "[SUB]":
				sub = 1;
				index += 5;
				return true;

				case "[sup]":
				case "[SUP]":
				sub = 2;
				index += 5;
				return true;
			}
		}

		if (index + 6 > length) return false;

		if (text[index + 5] == ']')
		{
			string sub6 = text.Substring(index, 6);

			switch (sub6)
			{
				case "[/sub]":
				case "[/SUB]":
				sub = 0;
				index += 6;
				return true;

				case "[/sup]":
				case "[/SUP]":
				sub = 0;
				index += 6;
				return true;

				case "[/url]":
				case "[/URL]":
				index += 6;
				return true;
			}
		}

		if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
		{
			int closingBracket = text.IndexOf(']', index + 4);

			if (closingBracket != -1)
			{
				index = closingBracket + 1;
				return true;
			}
			else
			{
				index = text.Length;
				return true;
			}
		}

		if (index + 8 > length) return false;

		if (text[index + 7] == ']')
		{
			Color c = ParseColor24(text, index + 1);

			if (EncodeColor24(c) != text.Substring(index + 1, 6).ToUpper())
				return false;

			if (colors != null)
			{
				c.a = colors[colors.size - 1].a;
				if (premultiply && c.a != 1f)
					c = Color.Lerp(mInvisible, c, c.a);
				colors.Add(c);
			}
			index += 8;
			return true;
		}

		if (index + 10 > length) return false;

		if (text[index + 9] == ']')
		{
			Color c = ParseColor32(text, index + 1);
			if (EncodeColor32(c) != text.Substring(index + 1, 8).ToUpper())
				return false;

			if (colors != null)
			{
				if (premultiply && c.a != 1f)
					c = Color.Lerp(mInvisible, c, c.a);
				colors.Add(c);
			}
			index += 10;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Runs through the specified string and removes all color-encoding symbols.
	/// </summary>

	static public string StripSymbols (string text)
	{
		if (text != null)
		{
			for (int i = 0, imax = text.Length; i < imax; )
			{
				char c = text[i];

				if (c == '[')
				{
					int sub = 0;
					bool bold = false;
					bool italic = false;
					bool underline = false;
					bool strikethrough = false;
					bool ignoreColor = false;
					int retVal = i;

					if (ParseSymbol(text, ref retVal, null, false, ref sub, ref bold, ref italic, ref underline, ref strikethrough, ref ignoreColor))
					{
						text = text.Remove(i, retVal - i);
						imax = text.Length;
						continue;
					}
				}
				++i;
			}
		}
		return text;
	}

	/// <summary>
	/// Align the vertices to be right or center-aligned given the line width specified by NGUIText.lineWidth.
	/// </summary>

	static public void Align (List<Vector3> verts, int indexOffset, float printedWidth, int elements = 4)
	{
		switch (alignment)
		{
			case Alignment.Right:
			{
				float padding = rectWidth - printedWidth;
				if (padding < 0f) return;

				for (int i = indexOffset, imax = verts.Count; i < imax; ++i)
				{
					var v = verts[i];
					v.x += padding;
					verts[i] = v;
				}
				break;
			}

			case Alignment.Center:
			{
				float padding = (rectWidth - printedWidth) * 0.5f;
				if (padding < 0f) return;

				// Keep it pixel-perfect
				int diff = Mathf.RoundToInt(rectWidth - printedWidth);
				int intWidth = Mathf.RoundToInt(rectWidth);

				bool oddDiff = (diff & 1) == 1;
				bool oddWidth = (intWidth & 1) == 1;
				if ((oddDiff && !oddWidth) || (!oddDiff && oddWidth))
					padding += 0.5f * fontScale;

				for (int i = indexOffset, imax = verts.Count; i < imax; ++i)
				{
					var v = verts[i];
					v.x += padding;
					verts[i] = v;
				}
				break;
			}

			case Alignment.Justified:
			{
				// Printed text needs to reach at least 65% of the width in order to be justified
				if (printedWidth < rectWidth * 0.65f) return;

				// There must be some padding involved
				float padding = (rectWidth - printedWidth) * 0.5f;
				if (padding < 1f) return;

				// There must be at least two characters
				int chars = (verts.Count - indexOffset) / elements;
				if (chars < 1) return;

				float progressPerChar = 1f / (chars - 1);
				float scale = rectWidth / printedWidth;
				Vector3 v;

				for (int i = indexOffset + elements, charIndex = 1, imax = verts.Count; i < imax; ++charIndex)
				{
					float x0 = verts[i].x;
					float x1 = verts[i + elements / 2].x;
					float w = x1 - x0;
					float x0a = x0 * scale;
					float x1a = x0a + w;
					float x1b = x1 * scale;
					float x0b = x1b - w;
					float progress = charIndex * progressPerChar;

					x1 = Mathf.Lerp(x1a, x1b, progress);
					x0 = Mathf.Lerp(x0a, x0b, progress);
					x0 = Mathf.Round(x0);
					x1 = Mathf.Round(x1);

					if (elements == 4)
					{
						v = verts[i]; v.x = x0; verts[i++] = v;
						v = verts[i]; v.x = x0; verts[i++] = v;
						v = verts[i]; v.x = x1; verts[i++] = v;
						v = verts[i]; v.x = x1; verts[i++] = v;
					}
					else if (elements == 2)
					{
						v = verts[i]; v.x = x0; verts[i++] = v;
						v = verts[i]; v.x = x1; verts[i++] = v;
					}
					else if (elements == 1)
					{
						v = verts[i]; v.x = x0; verts[i++] = v;
					}
				}
				break;
			}
		}
	}

	/// <summary>
	/// Get the index of the closest character within the provided list of values.
	/// Meant to be used with the arrays created by PrintExactCharacterPositions().
	/// </summary>

	static public int GetExactCharacterIndex (List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		for (int i = 0, imax = indices.Count; i < imax; ++i)
		{
			int i0 = (i << 1);
			int i1 = i0 + 1;

			float x0 = verts[i0].x;
			if (pos.x < x0) continue;

			float x1 = verts[i1].x;
			if (pos.x > x1) continue;

			float y0 = verts[i0].y;
			if (pos.y < y0) continue;

			float y1 = verts[i1].y;
			if (pos.y > y1) continue;

			return indices[i];
		}
		return 0;
	}

	/// <summary>
	/// Get the index of the closest vertex within the provided list of values.
	/// This function first sorts by Y, and only then by X.
	/// Meant to be used with the arrays created by PrintApproximateCharacterPositions().
	/// </summary>

	static public int GetApproximateCharacterIndex (List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		// First sort by Y, and only then by X
		float bestX = float.MaxValue;
		float bestY = float.MaxValue;
		int bestIndex = 0;

		for (int i = 0, imax = verts.Count; i < imax; ++i)
		{
			float diffY = Mathf.Abs(pos.y - verts[i].y);
			if (diffY > bestY) continue;

			float diffX = Mathf.Abs(pos.x - verts[i].x);

			if (diffY < bestY)
			{
				bestY = diffY;
				bestX = diffX;
				bestIndex = i;
			}
			else if (diffX < bestX)
			{
				bestX = diffX;
				bestIndex = i;
			}
		}
		return indices[bestIndex];
	}

	/// <summary>
	/// Whether the specified character is a space.
	/// </summary>

	[DebuggerHidden]
	[DebuggerStepThrough]
	static bool IsSpace (int ch) { return (ch == ' ' || ch == 0x200a || ch == 0x200b || ch == '\u2009'); }

	/// <summary>
	/// Convenience function that ends the line by either appending a new line character or replacing a space with one.
	/// </summary>

	[DebuggerHidden]
	[DebuggerStepThrough]
	static public void EndLine (ref StringBuilder s)
	{
		int i = s.Length - 1;
		if (i > 0 && IsSpace(s[i])) s[i] = '\n';
		else s.Append('\n');
	}

	/// <summary>
	/// Convenience function that ends the line by replacing a space with a newline character.
	/// </summary>

	[DebuggerHidden]
	[DebuggerStepThrough]
	static void ReplaceSpaceWithNewline (ref StringBuilder s)
	{
		int i = s.Length - 1;
		if (i > 0 && IsSpace(s[i])) s[i] = '\n';
	}

	/// <summary>
	/// Get the printed size of the specified string. The returned value is in pixels.
	/// </summary>

	static public Vector2 CalculatePrintedSize (string text)
	{
		Vector2 v = Vector2.zero;

		if (!string.IsNullOrEmpty(text))
		{
			// When calculating printed size, get rid of all symbols first since they are invisible anyway
			if (encoding) text = StripSymbols(text);

			// Ensure we have characters to work with
			Prepare(text);

			float x = 0f, y = 0f, maxX = 0f;
			int textLength = text.Length, ch = 0, prev = 0;

			for (int i = 0; i < textLength; ++i)
			{
				ch = text[i];

				// Start a new line
				if (ch == '\n')
				{
					if (x > maxX) maxX = x;
					x = 0f;
					y += finalLineHeight;
					continue;
				}

				// Skip invalid characters
				if (ch < ' ') continue;

				// See if there is a symbol matching this text
				BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

				if (symbol == null)
				{
					float w = GetGlyphWidth(ch, prev);

					if (w != 0f)
					{
						w += finalSpacingX;

						if (Mathf.RoundToInt(x + w) > regionWidth)
						{
							if (x > maxX) maxX = x - finalSpacingX;
							x = w;
							y += finalLineHeight;
						}
						else x += w;

						prev = ch;
					}
				}
				else
				{
					float w = finalSpacingX + symbol.advance * fontScale;

					if (Mathf.RoundToInt(x + w) > regionWidth)
					{
						if (x > maxX) maxX = x - finalSpacingX;
						x = w;
						y += finalLineHeight;
					}
					else x += w;

					i += symbol.sequence.Length - 1;
					prev = 0;
				}
			}

			v.x = ((x > maxX) ? x - finalSpacingX : maxX);
			v.y = (y + finalLineHeight);
		}
		return v;
	}

	static BetterList<float> mSizes = new BetterList<float>();

	/// <summary>
	/// Calculate the character index offset required to print the end of the specified text.
	/// </summary>

	static public int CalculateOffsetToFit (string text)
	{
		if (string.IsNullOrEmpty(text) || regionWidth < 1) return 0;

		Prepare(text);

		int textLength = text.Length, ch = 0, prev = 0;

		for (int i = 0, imax = text.Length; i < imax; ++i)
		{
			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

			if (symbol == null)
			{
				ch = text[i];
				float w = GetGlyphWidth(ch, prev);
				if (w != 0f) mSizes.Add(finalSpacingX + w);
				prev = ch;
			}
			else
			{
				mSizes.Add(finalSpacingX + symbol.advance * fontScale);
				for (int b = 0, bmax = symbol.sequence.Length - 1; b < bmax; ++b) mSizes.Add(0);
				i += symbol.sequence.Length - 1;
				prev = 0;
			}
		}

		float remainingWidth = regionWidth;
		int currentCharacterIndex = mSizes.size;

		while (currentCharacterIndex > 0 && remainingWidth > 0)
			remainingWidth -= mSizes[--currentCharacterIndex];

		mSizes.Clear();

		if (remainingWidth < 0) ++currentCharacterIndex;
		return currentCharacterIndex;
	}

	/// <summary>
	/// Get the end of line that would fit into a field of given width.
	/// </summary>

	static public string GetEndOfLineThatFits (string text)
	{
		int textLength = text.Length;
		int offset = CalculateOffsetToFit(text);
		return text.Substring(offset, textLength - offset);
	}

	/// <summary>
	/// Text wrapping functionality. The 'width' and 'height' should be in pixels.
	/// </summary>

	static public bool WrapText (string text, out string finalText, bool wrapLineColors = false)
	{
		return WrapText(text, out finalText, false, wrapLineColors);
	}

	/// <summary>
	/// Text wrapping functionality. The 'width' and 'height' should be in pixels.
	/// </summary>

	static public bool WrapText (string text, out string finalText, bool keepCharCount, bool wrapLineColors, bool useEllipsis = false)
	{
		if (regionWidth < 1 || regionHeight < 1 || finalLineHeight < 1f)
		{
			finalText = "";
			return false;
		}

		float height = (maxLines > 0) ? Mathf.Min(regionHeight, finalLineHeight * maxLines) : regionHeight;
		int maxLineCount = (maxLines > 0) ? maxLines : 1000000;
		maxLineCount = Mathf.FloorToInt(Mathf.Min(maxLineCount, height / finalLineHeight) + 0.01f);

		if (maxLineCount == 0)
		{
			finalText = "";
			return false;
		}

		if (string.IsNullOrEmpty(text)) text = " ";
		Prepare(text);

		StringBuilder sb = new StringBuilder();
		int textLength = text.Length;
		float remainingWidth = regionWidth;
		int start = 0, offset = 0, lineCount = 1, prev = 0;
		bool lineIsEmpty = true;
		bool fits = true;
		bool eastern = false;

		Color c = tint;
		int subscriptMode = 0;  // 0 = normal, 1 = subscript, 2 = superscript
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strikethrough = false;
		bool ignoreColor = false;

		if (!useSymbols) wrapLineColors = false;
		if (wrapLineColors)
		{
			mColors.Add(c);
			sb.Append("[");
			sb.Append(NGUIText.EncodeColor(c));
			sb.Append("]");
		}

		// Run through all characters
		for (; offset < textLength; ++offset)
		{
			char ch = text[offset];
			if (ch > 12287) eastern = true;

			// New line character -- start a new line
			if (ch == '\n')
			{
				if (lineCount == maxLineCount) break;
				remainingWidth = regionWidth;

				// Add the previous word to the final string
				if (start < offset) sb.Append(text.Substring(start, offset - start + 1));
				else sb.Append(ch);

				if (wrapLineColors)
				{
					for (int i = 0; i < mColors.size; ++i)
						sb.Insert(sb.Length - 1, "[-]");

					for (int i = 0; i < mColors.size; ++i)
					{
						sb.Append("[");
						sb.Append(NGUIText.EncodeColor(mColors[i]));
						sb.Append("]");
					}
				}

				lineIsEmpty = true;
				++lineCount;
				start = offset + 1;
				prev = 0;
				continue;
			}

			// When encoded symbols such as [RrGgBb] or [-] are encountered, skip past them
			if (encoding)
			{
				if (!wrapLineColors)
				{
					if (ParseSymbol(text, ref offset))
					{
						--offset;
						continue;
					}
				}
				else if (ParseSymbol(text, ref offset, mColors, premultiply, ref subscriptMode, ref bold,
					ref italic, ref underline, ref strikethrough, ref ignoreColor))
				{
					if (ignoreColor)
					{
						c = mColors[mColors.size - 1];
						c.a *= mAlpha * tint.a;
					}
					else
					{
						c = tint * mColors[mColors.size - 1];
						c.a *= mAlpha;
					}

					for (int b = 0, bmax = mColors.size - 2; b < bmax; ++b)
						c.a *= mColors[b].a;

					--offset;

					// Add the previous word to the final string
					if (start < offset) sb.Append(text.Substring(start, offset - start + 1));
					else sb.Append(ch);
					start = offset + 1;

					continue;
				}
			}

			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, offset, textLength) : null;

			// Calculate how wide this symbol or character is going to be
			float glyphWidth;

			if (symbol == null)
			{
				// Find the glyph for this character
				float w = GetGlyphWidth(ch, prev);
				if (w == 0f && !IsSpace(ch)) continue;
				glyphWidth = finalSpacingX + w;
			}
			else glyphWidth = finalSpacingX + symbol.advance * fontScale;

			// Reduce the width
			remainingWidth -= glyphWidth;

			// If this marks the end of a word, add it to the final string.
			if (IsSpace(ch) && !eastern && start < offset)
			{
				int end = offset - start + 1;

				// Last word on the last line should not include an invisible character
				if (lineCount == maxLineCount && remainingWidth <= 0f && offset < textLength)
				{
					char cho = text[offset];
					if (cho < ' ' || IsSpace(cho)) --end;
				}

				sb.Append(text.Substring(start, end));
				lineIsEmpty = false;
				start = offset + 1;
				prev = ch;
			}

			// Doesn't fit?
			if (Mathf.RoundToInt(remainingWidth) < 0)
			{
				// Can't start a new line
				if (lineIsEmpty || lineCount == maxLineCount)
				{
					// Adds "..." at the end of text that doesn't fit. Contributed by Jason Nollan.
					if (useEllipsis && lineCount == maxLineCount && offset > 1)
					{
						float ellipsisWidth = GetGlyphWidth('.', '.') * 3f;

						if (ellipsisWidth < regionWidth)
						{
							remainingWidth += glyphWidth;
							int tempOffset = offset;
							int removeCount = 0;

							while (tempOffset > 1 && remainingWidth < ellipsisWidth)
							{
								--tempOffset;
								char prevCh = text[tempOffset - 1];
								char characterToRemove = text[tempOffset];
								bool isCaseWhereSpaceShouldBeInStringBuilderButIsnt = (remainingWidth == 0 && IsSpace(characterToRemove));
								remainingWidth += GetGlyphWidth(characterToRemove, prevCh);
								if (tempOffset < start && !isCaseWhereSpaceShouldBeInStringBuilderButIsnt)
									++removeCount;
							}

							if (remainingWidth >= ellipsisWidth)
							{
								if (removeCount > 0)
									sb.Length = Mathf.Max(0, sb.Length - removeCount);

								sb.Append(text.Substring(start, Mathf.Max(0, tempOffset - start)));
								while (sb.Length > 0 && IsSpace(sb[sb.Length - 1])) --sb.Length;
								sb.Append("...");

								++lineCount;
								start = offset = tempOffset;
								break;
							}
						}
					}

					// This is the first word on the line -- add it up to the character that fits
					sb.Append(text.Substring(start, Mathf.Max(0, offset - start)));
					bool space = IsSpace(ch);
					if (!space && !eastern) fits = false;

					if (wrapLineColors && mColors.size > 0) sb.Append("[-]");

					if (lineCount++ == maxLineCount)
					{
						start = offset;
						break;
					}

					if (keepCharCount) ReplaceSpaceWithNewline(ref sb);
					else EndLine(ref sb);

					if (wrapLineColors)
					{
						for (int i = 0; i < mColors.size; ++i)
							sb.Insert(sb.Length - 1, "[-]");

						for (int i = 0; i < mColors.size; ++i)
						{
							sb.Append("[");
							sb.Append(NGUIText.EncodeColor(mColors[i]));
							sb.Append("]");
						}
					}

					// Start a brand-new line
					lineIsEmpty = true;

					if (space)
					{
						start = offset + 1;
						remainingWidth = regionWidth;
					}
					else
					{
						start = offset;
						remainingWidth = regionWidth - glyphWidth;
					}
					prev = 0;
				}
				else
				{
					// Revert the position to the beginning of the word and reset the line
					lineIsEmpty = true;
					remainingWidth = regionWidth;
					offset = start - 1;
					prev = 0;

					if (lineCount++ == maxLineCount) break;
					if (keepCharCount) ReplaceSpaceWithNewline(ref sb);
					else EndLine(ref sb);

					if (wrapLineColors)
					{
						// Negate previous colors prior to the newline character
						for (int i = 0; i < mColors.size; ++i)
							sb.Insert(sb.Length - 1, "[-]");

						// Add all the current colors before going forward
						for (int i = 0; i < mColors.size; ++i)
						{
							sb.Append("[");
							sb.Append(NGUIText.EncodeColor(mColors[i]));
							sb.Append("]");
						}
					}
					continue;
				}
			}
			else prev = ch;

			// Advance the offset past the symbol
			if (symbol != null)
			{
				offset += symbol.length - 1;
				prev = 0;
			}
		}

		if (start < offset) sb.Append(text.Substring(start, offset - start));
		if (wrapLineColors && mColors.size > 0) sb.Append("[-]");
		finalText = sb.ToString();
		mColors.Clear();
		return fits && ((offset == textLength) || (lineCount <= Mathf.Min(maxLines, maxLineCount)));
	}

	static Color s_c0, s_c1;

	/// <summary>
	/// Print the specified text into the buffers.
	/// </summary>

	static public void Print (string text, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (string.IsNullOrEmpty(text)) return;

		int indexOffset = verts.Count;
		Prepare(text);

		// Start with the white tint
		mColors.Add(Color.white);
		mAlpha = 1f;

		int ch = 0, prev = 0;
		float x = 0f, y = 0f, maxX = 0f;
		float sizeF = finalSize;

		Color gb = (tint * gradientBottom);
		Color gt = (tint * gradientTop);
		Color gc = tint;
		int textLength = text.Length;

		Rect uvRect = new Rect();
		float invX = 0f, invY = 0f;
		float sizePD = sizeF * pixelDensity;

		// Advanced symbol support contributed by Rudy Pangestu.
		bool subscript = false;
		int subscriptMode = 0;  // 0 = normal, 1 = subscript, 2 = superscript
		bool bold = false;
		bool italic = false;
		bool underline = false;
		bool strikethrough = false;
		bool ignoreColor = false;
		const float sizeShrinkage = 0.75f;

		float v0x;
		float v1x;
		float v1y;
		float v0y;
		float prevX = 0;

		if (bitmapFont != null)
		{
			uvRect = bitmapFont.uvRect;
			invX = uvRect.width / bitmapFont.texWidth;
			invY = uvRect.height / bitmapFont.texHeight;
		}

		for (int i = 0; i < textLength; ++i)
		{
			ch = text[i];

			prevX = x;

			// New line character -- skip to the next line
			if (ch == '\n')
			{
				if (x > maxX) maxX = x;

				if (alignment != Alignment.Left)
				{
					Align(verts, indexOffset, x - finalSpacingX);
					indexOffset = verts.Count;
				}

				x = 0;
				y += finalLineHeight;
				prev = 0;
				continue;
			}

			// Invalid character -- skip it
			if (ch < ' ')
			{
				prev = ch;
				continue;
			}

			// Color changing symbol
			if (encoding && ParseSymbol(text, ref i, mColors, premultiply, ref subscriptMode, ref bold,
				ref italic, ref underline, ref strikethrough, ref ignoreColor))
			{
				if (ignoreColor)
				{
					gc = mColors[mColors.size - 1];
					gc.a *= mAlpha * tint.a;
				}
				else
				{
					gc = tint * mColors[mColors.size - 1];
					gc.a *= mAlpha;
				}

				for (int b = 0, bmax = mColors.size - 2; b < bmax; ++b)
					gc.a *= mColors[b].a;

				if (gradient)
				{
					gb = (gradientBottom * gc);
					gt = (gradientTop * gc);
				}
				--i;
				continue;
			}

			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

			if (symbol != null)
			{
				v0x = x + symbol.offsetX * fontScale;
				v1x = v0x + symbol.width * fontScale;
				v1y = -(y + symbol.offsetY * fontScale);
				v0y = v1y - symbol.height * fontScale;

				// Doesn't fit? Move down to the next line
				if (Mathf.RoundToInt(x + symbol.advance * fontScale) > regionWidth)
				{
					if (x == 0f) return;

					if (alignment != Alignment.Left && indexOffset < verts.Count)
					{
						Align(verts, indexOffset, x - finalSpacingX);
						indexOffset = verts.Count;
					}

					v0x -= x;
					v1x -= x;
					v0y -= finalLineHeight;
					v1y -= finalLineHeight;

					x = 0;
					y += finalLineHeight;
					prevX = 0;
				}

				verts.Add(new Vector3(v0x, v0y));
				verts.Add(new Vector3(v0x, v1y));
				verts.Add(new Vector3(v1x, v1y));
				verts.Add(new Vector3(v1x, v0y));

				x += finalSpacingX + symbol.advance * fontScale;
				i += symbol.length - 1;
				prev = 0;

				if (uvs != null)
				{
					Rect uv = symbol.uvRect;

					float u0x = uv.xMin;
					float u0y = uv.yMin;
					float u1x = uv.xMax;
					float u1y = uv.yMax;

					uvs.Add(new Vector2(u0x, u0y));
					uvs.Add(new Vector2(u0x, u1y));
					uvs.Add(new Vector2(u1x, u1y));
					uvs.Add(new Vector2(u1x, u0y));
				}

				if (cols != null)
				{
					if (symbolStyle == SymbolStyle.Colored)
					{
						for (int b = 0; b < 4; ++b) cols.Add(gc);
					}
					else
					{
						Color col = Color.white;
						col.a = gc.a;
						for (int b = 0; b < 4; ++b) cols.Add(col);
					}
				}
			}
			else // No symbol present
			{
				GlyphInfo glyph = GetGlyph(ch, prev);
				if (glyph == null) continue;
				prev = ch;

				if (subscriptMode != 0)
				{
					glyph.v0.x *= sizeShrinkage;
					glyph.v0.y *= sizeShrinkage;
					glyph.v1.x *= sizeShrinkage;
					glyph.v1.y *= sizeShrinkage;

					if (subscriptMode == 1)
					{
						glyph.v0.y -= fontScale * fontSize * 0.4f;
						glyph.v1.y -= fontScale * fontSize * 0.4f;
					}
					else
					{
						glyph.v0.y += fontScale * fontSize * 0.05f;
						glyph.v1.y += fontScale * fontSize * 0.05f;
					}
				}

				v0x = glyph.v0.x + x;
				v0y = glyph.v0.y - y;
				v1x = glyph.v1.x + x;
				v1y = glyph.v1.y - y;

				float w = glyph.advance;
				if (finalSpacingX < 0f) w += finalSpacingX;

				// Doesn't fit? Move down to the next line
				if (Mathf.RoundToInt(x + w) > regionWidth)
				{
					if (x == 0f) return;

					if (alignment != Alignment.Left && indexOffset < verts.Count)
					{
						Align(verts, indexOffset, x - finalSpacingX);
						indexOffset = verts.Count;
					}

					v0x -= x;
					v1x -= x;
					v0y -= finalLineHeight;
					v1y -= finalLineHeight;

					x = 0;
					y += finalLineHeight;
					prevX = 0;
				}

				if (IsSpace(ch))
				{
					if (underline)
					{
						ch = '_';
					}
					else if (strikethrough)
					{
						ch = '-';
					}
				}

				// Advance the position
				x += (subscriptMode == 0) ? finalSpacingX + glyph.advance :
					(finalSpacingX + glyph.advance) * sizeShrinkage;

				// Subscript may cause pixels to no longer be aligned
				if (subscriptMode != 0) x = Mathf.Round(x);

				// No need to continue if this is a space character
				if (IsSpace(ch)) continue;

				// Texture coordinates
				if (uvs != null)
				{
					if (bitmapFont != null)
					{
						glyph.u0.x = uvRect.xMin + invX * glyph.u0.x;
						glyph.u2.x = uvRect.xMin + invX * glyph.u2.x;
						glyph.u0.y = uvRect.yMax - invY * glyph.u0.y;
						glyph.u2.y = uvRect.yMax - invY * glyph.u2.y;

						glyph.u1.x = glyph.u0.x;
						glyph.u1.y = glyph.u2.y;

						glyph.u3.x = glyph.u2.x;
						glyph.u3.y = glyph.u0.y;
					}

					for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
					{
						uvs.Add(glyph.u0);
						uvs.Add(glyph.u1);
						uvs.Add(glyph.u2);
						uvs.Add(glyph.u3);
					}
				}

				// Vertex colors
				if (cols != null)
				{
					if (glyph.channel == 0 || glyph.channel == 15)
					{
						if (gradient)
						{
							float min = sizePD + glyph.v0.y / fontScale;
							float max = sizePD + glyph.v1.y / fontScale;

							min /= sizePD;
							max /= sizePD;

							s_c0 = Color.Lerp(gb, gt, min);
							s_c1 = Color.Lerp(gb, gt, max);

							for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
							{
								cols.Add(s_c0);
								cols.Add(s_c1);
								cols.Add(s_c1);
								cols.Add(s_c0);
							}
						}
						else
						{
							for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
								cols.Add(gc);
						}
					}
					else
					{
						// Packed fonts come as alpha masks in each of the RGBA channels.
						// In order to use it we need to use a special shader.
						//
						// Limitations:
						// - Effects (drop shadow, outline) will not work.
						// - Should not be a part of the atlas (eastern fonts rarely are anyway).
						// - Lower color precision

						Color col = gc;

						col *= 0.49f;

						switch (glyph.channel)
						{
							case 1: col.b += 0.51f; break;
							case 2: col.g += 0.51f; break;
							case 4: col.r += 0.51f; break;
							case 8: col.a += 0.51f; break;
						}

						for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
							cols.Add(col);
					}
				}

				// Bold and italic contributed by Rudy Pangestu.
				if (!bold)
				{
					if (!italic)
					{
						verts.Add(new Vector3(v0x, v0y));
						verts.Add(new Vector3(v0x, v1y));
						verts.Add(new Vector3(v1x, v1y));
						verts.Add(new Vector3(v1x, v0y));
					}
					else // Italic
					{
						float slant = fontSize * 0.1f * ((v1y - v0y) / fontSize);
						verts.Add(new Vector3(v0x - slant, v0y));
						verts.Add(new Vector3(v0x + slant, v1y));
						verts.Add(new Vector3(v1x + slant, v1y));
						verts.Add(new Vector3(v1x - slant, v0y));
					}
				}
				else // Bold
				{
					for (int j = 0; j < 4; ++j)
					{
						float a = mBoldOffset[j * 2];
						float b = mBoldOffset[j * 2 + 1];

						float slant = (italic ? fontSize * 0.1f * ((v1y - v0y) / fontSize) : 0f);
						verts.Add(new Vector3(v0x + a - slant, v0y + b));
						verts.Add(new Vector3(v0x + a + slant, v1y + b));
						verts.Add(new Vector3(v1x + a + slant, v1y + b));
						verts.Add(new Vector3(v1x + a - slant, v0y + b));
					}
				}

				// Underline and strike-through contributed by Rudy Pangestu.
				if (underline || strikethrough)
				{
					GlyphInfo dash = GetGlyph(strikethrough ? '-' : '_', prev);
					if (dash == null) continue;

					if (uvs != null)
					{
						if (bitmapFont != null)
						{
							dash.u0.x = uvRect.xMin + invX * dash.u0.x;
							dash.u2.x = uvRect.xMin + invX * dash.u2.x;
							dash.u0.y = uvRect.yMax - invY * dash.u0.y;
							dash.u2.y = uvRect.yMax - invY * dash.u2.y;
						}

						float cx = (dash.u0.x + dash.u2.x) * 0.5f;

						for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
						{
							uvs.Add(new Vector2(cx, dash.u0.y));
							uvs.Add(new Vector2(cx, dash.u2.y));
							uvs.Add(new Vector2(cx, dash.u2.y));
							uvs.Add(new Vector2(cx, dash.u0.y));
						}
					}

					if (subscript && strikethrough)
					{
						v0y = (-y + dash.v0.y) * sizeShrinkage;
						v1y = (-y + dash.v1.y) * sizeShrinkage;
					}
					else
					{
						v0y = (-y + dash.v0.y);
						v1y = (-y + dash.v1.y);
					}

					if (bold)
					{
						for (int j = 0; j < 4; ++j)
						{
							float a = mBoldOffset[j * 2];
							float b = mBoldOffset[j * 2 + 1];

							verts.Add(new Vector3(prevX + a, v0y + b));
							verts.Add(new Vector3(prevX + a, v1y + b));
							verts.Add(new Vector3(x + a, v1y + b));
							verts.Add(new Vector3(x + a, v0y + b));
						}
					}
					else
					{
						verts.Add(new Vector3(prevX, v0y));
						verts.Add(new Vector3(prevX, v1y));
						verts.Add(new Vector3(x, v1y));
						verts.Add(new Vector3(x, v0y));
					}

					if (gradient)
					{
						float min = sizePD + dash.v0.y / fontScale;
						float max = sizePD + dash.v1.y / fontScale;

						min /= sizePD;
						max /= sizePD;

						s_c0 = Color.Lerp(gb, gt, min);
						s_c1 = Color.Lerp(gb, gt, max);

						for (int j = 0, jmax = (bold ? 4 : 1); j < jmax; ++j)
						{
							cols.Add(s_c0);
							cols.Add(s_c1);
							cols.Add(s_c1);
							cols.Add(s_c0);
						}
					}
					else
					{
						for (int j = 0, jmax = (bold ? 16 : 4); j < jmax; ++j)
							cols.Add(gc);
					}
				}
			}
		}

		if (alignment != Alignment.Left && indexOffset < verts.Count)
		{
			Align(verts, indexOffset, x - finalSpacingX);
			indexOffset = verts.Count;
		}
		mColors.Clear();
	}

	static float[] mBoldOffset = new float[]
	{
		-0.25f, 0f, 0.25f, 0f,
		0f, -0.25f, 0f, 0.25f
	};

	/// <summary>
	/// Print character positions and indices into the specified buffer. Meant to be used with the "find closest vertex" calculations.
	/// </summary>

	static public void PrintApproximateCharacterPositions (string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text)) text = " ";

		Prepare(text);

		float x = 0f, y = 0f, maxX = 0f, halfSize = fontSize * fontScale * 0.5f;
		int textLength = text.Length, indexOffset = verts.Count, ch = 0, prev = 0;

		for (int i = 0; i < textLength; ++i)
		{
			ch = text[i];

			verts.Add(new Vector3(x, -y - halfSize));
			indices.Add(i);

			if (ch == '\n')
			{
				if (x > maxX) maxX = x;

				if (alignment != Alignment.Left)
				{
					Align(verts, indexOffset, x - finalSpacingX, 1);
					indexOffset = verts.Count;
				}

				x = 0;
				y += finalLineHeight;
				prev = 0;
				continue;
			}
			else if (ch < ' ')
			{
				prev = 0;
				continue;
			}

			if (encoding && ParseSymbol(text, ref i))
			{
				--i;
				continue;
			}

			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

			if (symbol == null)
			{
				float w = GetGlyphWidth(ch, prev);

				if (w != 0f)
				{
					w += finalSpacingX;

					if (Mathf.RoundToInt(x + w) > regionWidth)
					{
						if (x == 0f) return;

						if (alignment != Alignment.Left && indexOffset < verts.Count)
						{
							Align(verts, indexOffset, x - finalSpacingX, 1);
							indexOffset = verts.Count;
						}

						x = w;
						y += finalLineHeight;
					}
					else x += w;

					verts.Add(new Vector3(x, -y - halfSize));
					indices.Add(i + 1);
					prev = ch;
				}
			}
			else
			{
				float w = symbol.advance * fontScale + finalSpacingX;

				if (Mathf.RoundToInt(x + w) > regionWidth)
				{
					if (x == 0f) return;

					if (alignment != Alignment.Left && indexOffset < verts.Count)
					{
						Align(verts, indexOffset, x - finalSpacingX, 1);
						indexOffset = verts.Count;
					}

					x = w;
					y += finalLineHeight;
				}
				else x += w;

				verts.Add(new Vector3(x, -y - halfSize));
				indices.Add(i + 1);
				i += symbol.sequence.Length - 1;
				prev = 0;
			}
		}

		if (alignment != Alignment.Left && indexOffset < verts.Count)
			Align(verts, indexOffset, x - finalSpacingX, 1);
	}

	/// <summary>
	/// Print character positions and indices into the specified buffer.
	/// This function's data is meant to be used for precise character selection, such as clicking on a link.
	/// There are 2 vertices for every index: Bottom Left + Top Right.
	/// </summary>

	static public void PrintExactCharacterPositions (string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text)) text = " ";

		Prepare(text);

		float fullSize = fontSize * fontScale;
		float x = 0f, y = 0f, maxX = 0f;
		int textLength = text.Length, indexOffset = verts.Count, ch = 0, prev = 0;

		for (int i = 0; i < textLength; ++i)
		{
			ch = text[i];

			if (ch == '\n')
			{
				if (x > maxX) maxX = x;

				if (alignment != Alignment.Left)
				{
					Align(verts, indexOffset, x - finalSpacingX, 2);
					indexOffset = verts.Count;
				}

				x = 0;
				y += finalLineHeight;
				prev = 0;
				continue;
			}
			else if (ch < ' ')
			{
				prev = 0;
				continue;
			}

			if (encoding && ParseSymbol(text, ref i))
			{
				--i;
				continue;
			}

			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, i, textLength) : null;

			if (symbol == null)
			{
				float gw = GetGlyphWidth(ch, prev);

				if (gw != 0f)
				{
					float w = gw + finalSpacingX;

					if (Mathf.RoundToInt(x + w) > regionWidth)
					{
						if (x == 0f) return;

						if (alignment != Alignment.Left && indexOffset < verts.Count)
						{
							Align(verts, indexOffset, x - finalSpacingX, 2);
							indexOffset = verts.Count;
						}

						x = 0f;
						y += finalLineHeight;
						prev = 0;
						--i;
						continue;
					}

					indices.Add(i);
					verts.Add(new Vector3(x, -y - fullSize));
					verts.Add(new Vector3(x + w, -y));
					prev = ch;
					x += w;
				}
			}
			else
			{
				float w = symbol.advance * fontScale + finalSpacingX;

				if (Mathf.RoundToInt(x + w) > regionWidth)
				{
					if (x == 0f) return;

					if (alignment != Alignment.Left && indexOffset < verts.Count)
					{
						Align(verts, indexOffset, x - finalSpacingX, 2);
						indexOffset = verts.Count;
					}

					x = 0f;
					y += finalLineHeight;
					prev = 0;
					--i;
					continue;
				}

				indices.Add(i);
				verts.Add(new Vector3(x, -y - fullSize));
				verts.Add(new Vector3(x + w, -y));
				i += symbol.sequence.Length - 1;
				x += w;
				prev = 0;
			}
		}

		if (alignment != Alignment.Left && indexOffset < verts.Count)
			Align(verts, indexOffset, x - finalSpacingX, 2);
	}

	/// <summary>
	/// Print the caret and selection vertices. Note that it's expected that 'text' has been stripped clean of symbols.
	/// </summary>

	static public void PrintCaretAndSelection (string text, int start, int end, List<Vector3> caret, List<Vector3> highlight)
	{
		if (string.IsNullOrEmpty(text)) text = " ";

		Prepare(text);

		int caretPos = end;

		if (start > end)
		{
			end = start;
			start = caretPos;
		}

		float x = 0f, y = 0f, maxX = 0f, fs = fontSize * fontScale;
		int caretOffset = (caret != null) ? caret.Count : 0;
		int highlightOffset = (highlight != null) ? highlight.Count : 0;
		int textLength = text.Length, index = 0, ch = 0, prev = 0;
		bool highlighting = false, caretSet = false;

		Vector2 last0 = Vector2.zero;
		Vector2 last1 = Vector2.zero;

		for (; index < textLength; ++index)
		{
			// Print the caret
			if (caret != null && !caretSet && caretPos <= index)
			{
				caretSet = true;
				caret.Add(new Vector3(x - 1f, -y - fs));
				caret.Add(new Vector3(x - 1f, -y));
				caret.Add(new Vector3(x + 1f, -y));
				caret.Add(new Vector3(x + 1f, -y - fs));
			}

			ch = text[index];

			if (ch == '\n')
			{
				// Used for alignment purposes
				if (x > maxX) maxX = x;

				// Align the caret
				if (caret != null && caretSet)
				{
					if (alignment != Alignment.Left) Align(caret, caretOffset, x - finalSpacingX);
					caret = null;
				}

				if (highlight != null)
				{
					if (highlighting)
					{
						// Close the selection on this line
						highlighting = false;
						highlight.Add(last1);
						highlight.Add(last0);
					}
					else if (start <= index && end > index)
					{
						// This must be an empty line. Add a narrow vertical highlight.
						highlight.Add(new Vector3(x, -y - fs));
						highlight.Add(new Vector3(x, -y));
						highlight.Add(new Vector3(x + 2f, -y));
						highlight.Add(new Vector3(x + 2f, -y - fs));
					}

					// Align the highlight
					if (alignment != Alignment.Left && highlightOffset < highlight.Count)
					{
						Align(highlight, highlightOffset, x - finalSpacingX);
						highlightOffset = highlight.Count;
					}
				}

				x = 0;
				y += finalLineHeight;
				prev = 0;
				continue;
			}
			else if (ch < ' ')
			{
				prev = 0;
				continue;
			}

			if (encoding && ParseSymbol(text, ref index))
			{
				--index;
				continue;
			}

			// See if there is a symbol matching this text
			BMSymbol symbol = useSymbols ? GetSymbol(text, index, textLength) : null;
			float w = (symbol != null) ? symbol.advance * fontScale : GetGlyphWidth(ch, prev);

			if (w != 0f)
			{
				float v0x = x;
				float v1x = x + w;
				float v0y = -y - fs;
				float v1y = -y;

				if (Mathf.RoundToInt(v1x + finalSpacingX) > regionWidth)
				{
					if (x == 0f) return;

					// Used for alignment purposes
					if (x > maxX) maxX = x;

					// Align the caret
					if (caret != null && caretSet)
					{
						if (alignment != Alignment.Left) Align(caret, caretOffset, x - finalSpacingX);
						caret = null;
					}

					if (highlight != null)
					{
						if (highlighting)
						{
							// Close the selection on this line
							highlighting = false;
							highlight.Add(last1);
							highlight.Add(last0);
						}
						else if (start <= index && end > index)
						{
							// This must be an empty line. Add a narrow vertical highlight.
							highlight.Add(new Vector3(x, -y - fs));
							highlight.Add(new Vector3(x, -y));
							highlight.Add(new Vector3(x + 2f, -y));
							highlight.Add(new Vector3(x + 2f, -y - fs));
						}

						// Align the highlight
						if (alignment != Alignment.Left && highlightOffset < highlight.Count)
						{
							Align(highlight, highlightOffset, x - finalSpacingX);
							highlightOffset = highlight.Count;
						}
					}

					v0x -= x;
					v1x -= x;
					v0y -= finalLineHeight;
					v1y -= finalLineHeight;

					x = 0;
					y += finalLineHeight;
				}

				x += w + finalSpacingX;

				// Print the highlight
				if (highlight != null)
				{
					if (start > index || end <= index)
					{
						if (highlighting)
						{
							// Finish the highlight
							highlighting = false;
							highlight.Add(last1);
							highlight.Add(last0);
						}
					}
					else if (!highlighting)
					{
						// Start the highlight
						highlighting = true;
						highlight.Add(new Vector3(v0x, v0y));
						highlight.Add(new Vector3(v0x, v1y));
					}
				}

				// Save what the character ended with
				last0 = new Vector2(v1x, v0y);
				last1 = new Vector2(v1x, v1y);
				prev = ch;
			}
		}

		// Ensure we always have a caret
		if (caret != null)
		{
			if (!caretSet)
			{
				caret.Add(new Vector3(x - 1f, -y - fs));
				caret.Add(new Vector3(x - 1f, -y));
				caret.Add(new Vector3(x + 1f, -y));
				caret.Add(new Vector3(x + 1f, -y - fs));
			}

			if (alignment != Alignment.Left)
				Align(caret, caretOffset, x - finalSpacingX);
		}

		// Close the selection
		if (highlight != null)
		{
			if (highlighting)
			{
				// Finish the highlight
				highlight.Add(last1);
				highlight.Add(last0);
			}
			else if (start < index && end == index)
			{
				// Happens when highlight ends on an empty line. Highlight it with a thin line.
				highlight.Add(new Vector3(x, -y - fs));
				highlight.Add(new Vector3(x, -y));
				highlight.Add(new Vector3(x + 2f, -y));
				highlight.Add(new Vector3(x + 2f, -y - fs));
			}

			// Align the highlight
			if (alignment != Alignment.Left && highlightOffset < highlight.Count)
				Align(highlight, highlightOffset, x - finalSpacingX);
		}
	}

	/// <summary>
	/// Replace the specified link.
	/// </summary>

	static public bool ReplaceLink (ref string text, ref int index, string prefix)
	{
		if (index == -1) return false;
		index = text.IndexOf(prefix, index);
		if (index == -1) return false;

		int domainStart = index + prefix.Length;
		int end = text.IndexOf(' ', domainStart);
		if (end == -1) end = text.Length;

		int domainEnd = text.IndexOfAny(new char[] { '/', ' ' }, domainStart);

		if (domainEnd == -1 || domainEnd == domainStart)
		{
			index += 7;
			return true;
		}

		string left = text.Substring(0, index);
		string link = text.Substring(index, end - index);
		string right = text.Substring(end);
		string urlName = text.Substring(domainStart, domainEnd - domainStart);

		text = left + "[url=" + link + "][u]" + urlName + "[/u][/url]";
		index = text.Length;
		text += right;
		return true;
	}

	/// <summary>
	/// Insert a hyperlink around the specified keyword.
	/// </summary>

	static public bool InsertHyperlink (ref string text, ref int index, string keyword, string link)
	{
		int patchStart = text.IndexOf(keyword, index, System.StringComparison.CurrentCultureIgnoreCase);
		if (patchStart == -1) return false;

		string left = text.Substring(0, patchStart);
		string url = "[url=" + link + "][u]";
		string middle = text.Substring(patchStart, keyword.Length) + "[/u][/url]";
		string right = text.Substring(patchStart + keyword.Length);

		text = left + url + middle;
		index = text.Length;
		text += right;
		return true;
	}

	/// <summary>
	/// Helper function that replaces links within text with clickable ones.
	/// </summary>

	static public void ReplaceLinks (ref string text)
	{
		for (int index = 0; index < text.Length; )
		{
			if (!ReplaceLink(ref text, ref index, "http://")) break;
		}

		for (int index = 0; index < text.Length; )
		{
			if (!ReplaceLink(ref text, ref index, "https://")) break;
		}
	}
}
