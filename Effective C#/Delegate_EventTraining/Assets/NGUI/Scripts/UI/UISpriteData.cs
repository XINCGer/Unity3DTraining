//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UISpriteData
{
	public string name = "Sprite";
	public int x = 0;
	public int y = 0;
	public int width = 0;
	public int height = 0;

	public int borderLeft = 0;
	public int borderRight = 0;
	public int borderTop = 0;
	public int borderBottom = 0;

	public int paddingLeft = 0;
	public int paddingRight = 0;
	public int paddingTop = 0;
	public int paddingBottom = 0;

	//bool rotated = false;

	/// <summary>
	/// Whether the sprite has a border.
	/// </summary>

	public bool hasBorder { get { return (borderLeft | borderRight | borderTop | borderBottom) != 0; } }

	/// <summary>
	/// Whether the sprite has been offset via padding.
	/// </summary>

	public bool hasPadding { get { return (paddingLeft | paddingRight | paddingTop | paddingBottom) != 0; } }

	/// <summary>
	/// Convenience function -- set the X, Y, width, and height.
	/// </summary>

	public void SetRect (int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	/// <summary>
	/// Convenience function -- set the sprite's padding.
	/// </summary>

	public void SetPadding (int left, int bottom, int right, int top)
	{
		paddingLeft = left;
		paddingBottom = bottom;
		paddingRight = right;
		paddingTop = top;
	}

	/// <summary>
	/// Convenience function -- set the sprite's border.
	/// </summary>

	public void SetBorder (int left, int bottom, int right, int top)
	{
		borderLeft = left;
		borderBottom = bottom;
		borderRight = right;
		borderTop = top;
	}

	/// <summary>
	/// Copy all values of the specified sprite data.
	/// </summary>

	public void CopyFrom (UISpriteData sd)
	{
		name = sd.name;

		x = sd.x;
		y = sd.y;
		width = sd.width;
		height = sd.height;
		
		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;
		
		paddingLeft = sd.paddingLeft;
		paddingRight = sd.paddingRight;
		paddingTop = sd.paddingTop;
		paddingBottom = sd.paddingBottom;
	}

	/// <summary>
	/// Copy the border information from the specified sprite.
	/// </summary>

	public void CopyBorderFrom (UISpriteData sd)
	{
		borderLeft = sd.borderLeft;
		borderRight = sd.borderRight;
		borderTop = sd.borderTop;
		borderBottom = sd.borderBottom;
	}
}
