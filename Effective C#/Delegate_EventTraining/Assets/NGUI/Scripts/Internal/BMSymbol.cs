//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Symbols are a sequence of characters such as ":)" that get replaced with a sprite, such as the smiley face.
/// </summary>

[System.Serializable]
public class BMSymbol
{
	public string sequence;
	public string spriteName;

	UISpriteData mSprite = null;
	bool mIsValid = false;
	int mLength = 0;
	int mOffsetX = 0;		// (outer - inner) in pixels
	int mOffsetY = 0;		// (outer - inner) in pixels
	int mWidth = 0;			// Symbol's width in pixels (sprite.outer.width)
	int mHeight = 0;		// Symbol's height in pixels (sprite.outer.height)
	int mAdvance = 0;		// Symbol's inner width in pixels (sprite.inner.width)
	Rect mUV;

	public int length	{ get { if (mLength == 0) mLength = sequence.Length; return mLength; } }
	public int offsetX	{ get { return mOffsetX; } }
	public int offsetY	{ get { return mOffsetY; } }
	public int width	{ get { return mWidth; } }
	public int height	{ get { return mHeight; } }
	public int advance	{ get { return mAdvance; } }
	public Rect uvRect	{ get { return mUV; } }

	/// <summary>
	/// Mark this symbol as dirty, clearing the sprite reference.
	/// </summary>

	public void MarkAsChanged () { mIsValid = false; }

	/// <summary>
	/// Validate this symbol, given the specified atlas.
	/// </summary>

	public bool Validate (UIAtlas atlas)
	{
		if (atlas == null) return false;

#if UNITY_EDITOR
		if (!Application.isPlaying || !mIsValid)
#else
		if (!mIsValid)
#endif
		{
			if (string.IsNullOrEmpty(spriteName)) return false;

			mSprite = (atlas != null) ? atlas.GetSprite(spriteName) : null;

			if (mSprite != null)
			{
				Texture tex = atlas.texture;

				if (tex == null)
				{
					mSprite = null;
				}
				else
				{
					mUV = new Rect(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
					mUV = NGUIMath.ConvertToTexCoords(mUV, tex.width, tex.height);
					mOffsetX = mSprite.paddingLeft;
					mOffsetY = mSprite.paddingTop;
					mWidth = mSprite.width;
					mHeight = mSprite.height;
					mAdvance = mSprite.width + (mSprite.paddingLeft + mSprite.paddingRight);
					mIsValid = true;
				}
			}
		}
		return (mSprite != null);
	}
}
