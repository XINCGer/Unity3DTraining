//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

// Dynamic font support contributed by the NGUI community members:
// Unisip, zh4ox, Mudwiz, Nicki, DarkMagicCK.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to view and edit UIFonts.
/// </summary>

[CustomEditor(typeof(UIFont))]
public class UIFontInspector : Editor
{
	enum View
	{
		Nothing,
		Atlas,
		Font,
	}

	enum FontType
	{
		Bitmap,
		Reference,
		Dynamic,
	}

	static View mView = View.Font;
	static bool mUseShader = false;

	UIFont mFont;
	FontType mType = FontType.Bitmap;
	UIFont mReplacement = null;
	string mSymbolSequence = "";
	string mSymbolSprite = "";
	BMSymbol mSelectedSymbol = null;
	AnimationCurve mCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public override bool HasPreviewGUI () { return mView != View.Nothing; }

	void OnSelectFont (Object obj)
	{
		// Undo doesn't work correctly in this case... so I won't bother.
		//NGUIEditorTools.RegisterUndo("Font Change");
		//NGUIEditorTools.RegisterUndo("Font Change", mFont);

		mFont.replacement = obj as UIFont;
		mReplacement = mFont.replacement;
		NGUITools.SetDirty(mFont);
	}

	void OnSelectAtlas (Object obj)
	{
		if (mFont != null && mFont.atlas != obj)
		{
			NGUIEditorTools.RegisterUndo("Font Atlas", mFont);
			mFont.atlas = obj as UIAtlas;
			MarkAsChanged();
		}
	}

	void MarkAsChanged ()
	{
		List<UILabel> labels = NGUIEditorTools.FindAll<UILabel>();

		foreach (UILabel lbl in labels)
		{
			if (UIFont.CheckIfRelated(lbl.bitmapFont, mFont))
			{
				lbl.bitmapFont = null;
				lbl.bitmapFont = mFont;
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		mFont = target as UIFont;
		NGUIEditorTools.SetLabelWidth(80f);

		GUILayout.Space(6f);

		if (mFont.replacement != null)
		{
			mType = FontType.Reference;
			mReplacement = mFont.replacement;
		}
		else if (mFont.dynamicFont != null)
		{
			mType = FontType.Dynamic;
		}

		GUI.changed = false;
		GUILayout.BeginHorizontal();
		mType = (FontType)EditorGUILayout.EnumPopup("Font Type", mType);
		NGUIEditorTools.DrawPadding();
		GUILayout.EndHorizontal();

		if (GUI.changed)
		{
			if (mType == FontType.Bitmap)
				OnSelectFont(null);

			if (mType != FontType.Dynamic && mFont.dynamicFont != null)
				mFont.dynamicFont = null;
		}

		if (mType == FontType.Reference)
		{
			ComponentSelector.Draw<UIFont>(mFont.replacement, OnSelectFont, true);

			GUILayout.Space(6f);
			EditorGUILayout.HelpBox("You can have one font simply point to " +
				"another one. This is useful if you want to be " +
				"able to quickly replace the contents of one " +
				"font with another one, for example for " +
				"swapping an SD font with an HD one, or " +
				"replacing an English font with a Chinese " +
				"one. All the labels referencing this font " +
				"will update their references to the new one.", MessageType.Info);

			if (mReplacement != mFont && mFont.replacement != mReplacement)
			{
				NGUIEditorTools.RegisterUndo("Font Change", mFont);
				mFont.replacement = mReplacement;
				NGUITools.SetDirty(mFont);
			}
			return;
		}
		else if (mType == FontType.Dynamic)
		{
#if UNITY_3_5
			EditorGUILayout.HelpBox("Dynamic fonts require Unity 4.0 or higher.", MessageType.Error);
#else
			Font fnt = EditorGUILayout.ObjectField("TTF Font", mFont.dynamicFont, typeof(Font), false) as Font;
			
			if (fnt != mFont.dynamicFont)
			{
				NGUIEditorTools.RegisterUndo("Font change", mFont);
				mFont.dynamicFont = fnt;
			}

			Material mat = EditorGUILayout.ObjectField("Material", mFont.material, typeof(Material), false) as Material;

			if (mFont.material != mat)
			{
				NGUIEditorTools.RegisterUndo("Font Material", mFont);
				mFont.material = mat;
			}

			GUILayout.BeginHorizontal();
			int size = EditorGUILayout.IntField("Default Size", mFont.defaultSize, GUILayout.Width(120f));
			FontStyle style = (FontStyle)EditorGUILayout.EnumPopup(mFont.dynamicFontStyle);
			NGUIEditorTools.DrawPadding();
			GUILayout.EndHorizontal();

			if (size != mFont.defaultSize)
			{
				NGUIEditorTools.RegisterUndo("Font change", mFont);
				mFont.defaultSize = size;
			}

			if (style != mFont.dynamicFontStyle)
			{
				NGUIEditorTools.RegisterUndo("Font change", mFont);
				mFont.dynamicFontStyle = style;
			}
#endif
		}
		else
		{
			ComponentSelector.Draw<UIAtlas>(mFont.atlas, OnSelectAtlas, true);

			if (mFont.atlas != null)
			{
				if (mFont.bmFont.isValid)
				{
					NGUIEditorTools.DrawAdvancedSpriteField(mFont.atlas, mFont.spriteName, SelectSprite, false);
				}
				EditorGUILayout.Space();
			}
			else
			{
				// No atlas specified -- set the material and texture rectangle directly
				Material mat = EditorGUILayout.ObjectField("Material", mFont.material, typeof(Material), false) as Material;

				if (mFont.material != mat)
				{
					NGUIEditorTools.RegisterUndo("Font Material", mFont);
					mFont.material = mat;
				}
			}

			// For updating the font's data when importing from an external source, such as the texture packer
			bool resetWidthHeight = false;

			if (mFont.atlas != null || mFont.material != null)
			{
				TextAsset data = EditorGUILayout.ObjectField("Import Data", null, typeof(TextAsset), false) as TextAsset;

				if (data != null)
				{
					NGUIEditorTools.RegisterUndo("Import Font Data", mFont);
					BMFontReader.Load(mFont.bmFont, NGUITools.GetHierarchy(mFont.gameObject), data.bytes);
					mFont.MarkAsChanged();
					resetWidthHeight = true;
					Debug.Log("Imported " + mFont.bmFont.glyphCount + " characters");
				}
			}

			if (mFont.bmFont.isValid)
			{
				Texture2D tex = mFont.texture;

				if (tex != null && mFont.atlas == null)
				{
					// Pixels are easier to work with than UVs
					Rect pixels = NGUIMath.ConvertToPixels(mFont.uvRect, tex.width, tex.height, false);

					// Automatically set the width and height of the rectangle to be the original font texture's dimensions
					if (resetWidthHeight)
					{
						pixels.width = mFont.texWidth;
						pixels.height = mFont.texHeight;
					}

					// Font sprite rectangle
					pixels = EditorGUILayout.RectField("Pixel Rect", pixels);

					// Convert the pixel coordinates back to UV coordinates
					Rect uvRect = NGUIMath.ConvertToTexCoords(pixels, tex.width, tex.height);

					if (mFont.uvRect != uvRect)
					{
						NGUIEditorTools.RegisterUndo("Font Pixel Rect", mFont);
						mFont.uvRect = uvRect;
					}
					//NGUIEditorTools.DrawSeparator();
					EditorGUILayout.Space();
				}
			}
		}

		// Dynamic fonts don't support emoticons
		if (!mFont.isDynamic && mFont.bmFont.isValid)
		{
			if (mFont.atlas != null)
			{
				if (NGUIEditorTools.DrawHeader("Symbols and Emoticons"))
				{
					NGUIEditorTools.BeginContents();

					List<BMSymbol> symbols = mFont.symbols;

					for (int i = 0; i < symbols.Count; )
					{
						BMSymbol sym = symbols[i];

						GUILayout.BeginHorizontal();
						GUILayout.Label(sym.sequence, GUILayout.Width(40f));
						if (NGUIEditorTools.DrawSpriteField(mFont.atlas, sym.spriteName, ChangeSymbolSprite, GUILayout.MinWidth(100f)))
							mSelectedSymbol = sym;

						if (GUILayout.Button("Edit", GUILayout.Width(40f)))
						{
							if (mFont.atlas != null)
							{
								NGUISettings.atlas = mFont.atlas;
								NGUISettings.selectedSprite = sym.spriteName;
								NGUIEditorTools.Select(mFont.atlas.gameObject);
							}
						}

						GUI.backgroundColor = Color.red;

						if (GUILayout.Button("X", GUILayout.Width(22f)))
						{
							NGUIEditorTools.RegisterUndo("Remove symbol", mFont);
							mSymbolSequence = sym.sequence;
							mSymbolSprite = sym.spriteName;
							symbols.Remove(sym);
							mFont.MarkAsChanged();
						}
						GUI.backgroundColor = Color.white;
						GUILayout.EndHorizontal();
						GUILayout.Space(4f);
						++i;
					}

					if (symbols.Count > 0)
					{
						GUILayout.Space(6f);
					}

					GUILayout.BeginHorizontal();
					mSymbolSequence = EditorGUILayout.TextField(mSymbolSequence, GUILayout.Width(40f));
					NGUIEditorTools.DrawSpriteField(mFont.atlas, mSymbolSprite, SelectSymbolSprite);

					bool isValid = !string.IsNullOrEmpty(mSymbolSequence) && !string.IsNullOrEmpty(mSymbolSprite);
					GUI.backgroundColor = isValid ? Color.green : Color.grey;

					if (GUILayout.Button("Add", GUILayout.Width(40f)) && isValid)
					{
						NGUIEditorTools.RegisterUndo("Add symbol", mFont);
						mFont.AddSymbol(mSymbolSequence, mSymbolSprite);
						mFont.MarkAsChanged();
						mSymbolSequence = "";
						mSymbolSprite = "";
					}
					GUI.backgroundColor = Color.white;
					GUILayout.EndHorizontal();

					if (symbols.Count == 0)
					{
						EditorGUILayout.HelpBox("Want to add an emoticon to your font? In the field above type ':)', choose a sprite, then hit the Add button.", MessageType.Info);
					}
					else GUILayout.Space(4f);

					NGUIEditorTools.EndContents();
				}
			}
		}

		if (mFont.bmFont != null && mFont.bmFont.isValid)
		{
			if (NGUIEditorTools.DrawHeader("Modify"))
			{
				NGUIEditorTools.BeginContents();

				UISpriteData sd = mFont.sprite;

				bool disable = (sd != null && (sd.paddingLeft != 0 || sd.paddingBottom != 0));
				EditorGUI.BeginDisabledGroup(disable || mFont.packedFontShader);

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20f);
				EditorGUILayout.BeginVertical();

				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				NGUISettings.foregroundColor = EditorGUILayout.ColorField("Foreground", NGUISettings.foregroundColor);
				NGUISettings.backgroundColor = EditorGUILayout.ColorField("Background", NGUISettings.backgroundColor);
				GUILayout.EndVertical();
				mCurve = EditorGUILayout.CurveField("", mCurve, GUILayout.Width(40f), GUILayout.Height(40f));
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Add a Shadow")) ApplyEffect(Effect.Shadow, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Add a Soft Outline")) ApplyEffect(Effect.Outline, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Rebalance Colors")) ApplyEffect(Effect.Rebalance, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Apply Curve to Alpha")) ApplyEffect(Effect.AlphaCurve, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Apply Curve to Foreground")) ApplyEffect(Effect.ForegroundCurve, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Apply Curve to Background")) ApplyEffect(Effect.BackgroundCurve, NGUISettings.foregroundColor, NGUISettings.backgroundColor);

				GUILayout.Space(10f);
				if (GUILayout.Button("Add Transparent Border (+1)")) ApplyEffect(Effect.Border, NGUISettings.foregroundColor, NGUISettings.backgroundColor);
				if (GUILayout.Button("Remove Border (-1)")) ApplyEffect(Effect.Crop, NGUISettings.foregroundColor, NGUISettings.backgroundColor);

				EditorGUILayout.EndVertical();
				GUILayout.Space(20f);
				EditorGUILayout.EndHorizontal();

				EditorGUI.EndDisabledGroup();

				if (disable)
				{
					GUILayout.Space(3f);
					EditorGUILayout.HelpBox("The sprite used by this font has been trimmed and is not suitable for modification. " +
						"Try re-adding this sprite with 'Trim Alpha' disabled.", MessageType.Warning);
				}

				NGUIEditorTools.EndContents();
			}
		}

		// The font must be valid at this point for the rest of the options to show up
		if (mFont.isDynamic || mFont.bmFont.isValid)
		{
			if (mFont.atlas == null)
			{
				mView = View.Font;
				mUseShader = false;
			}
		}

		// Preview option
		if (!mFont.isDynamic && mFont.atlas != null)
		{
			GUILayout.BeginHorizontal();
			{
				mView = (View)EditorGUILayout.EnumPopup("Preview", mView);
				GUILayout.Label("Shader", GUILayout.Width(45f));
				mUseShader = EditorGUILayout.Toggle(mUseShader, GUILayout.Width(20f));
			}
			GUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// "New Sprite" selection.
	/// </summary>

	void SelectSymbolSprite (string spriteName)
	{
		mSymbolSprite = spriteName;
		Repaint();
	}

	/// <summary>
	/// Existing sprite selection.
	/// </summary>

	void ChangeSymbolSprite (string spriteName)
	{
		if (mSelectedSymbol != null && mFont != null)
		{
			NGUIEditorTools.RegisterUndo("Change symbol", mFont);
			mSelectedSymbol.spriteName = spriteName;
			Repaint();
			mFont.MarkAsChanged();
		}
	}

	/// <summary>
	/// Draw the font preview window.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		mFont = target as UIFont;
		if (mFont == null) return;
		Texture2D tex = mFont.texture;

		if (mView != View.Nothing && tex != null)
		{
			Material m = (mUseShader ? mFont.material : null);

			if (mView == View.Font && mFont.atlas != null && mFont.sprite != null)
			{
				NGUIEditorTools.DrawSprite(tex, rect, mFont.sprite, Color.white, m);
			}
			else
			{
				NGUIEditorTools.DrawTexture(tex, rect, new Rect(0f, 0f, 1f, 1f), Color.white, m);
			}
		}
	}

	/// <summary>
	/// Sprite selection callback.
	/// </summary>

	void SelectSprite (string spriteName)
	{
		NGUIEditorTools.RegisterUndo("Font Sprite", mFont);
		mFont.spriteName = spriteName;
		Repaint();
	}

	enum Effect
	{
		Rebalance,
		ForegroundCurve,
		BackgroundCurve,
		AlphaCurve,
		Border,
		Shadow,
		Outline,
		Crop,
	}

	/// <summary>
	/// Apply an effect to the font.
	/// </summary>

	void ApplyEffect (Effect effect, Color foreground, Color background)
	{
		BMFont bf = mFont.bmFont;
		int offsetX = 0;
		int offsetY = 0;

		if (mFont.atlas != null)
		{
			UISpriteData sd = mFont.atlas.GetSprite(bf.spriteName);
			if (sd == null) return;
			offsetX = sd.x;
			offsetY = sd.y + mFont.texHeight - sd.paddingTop;
		}

		string path = AssetDatabase.GetAssetPath(mFont.texture);
		Texture2D bfTex = NGUIEditorTools.ImportTexture(path, true, true, false);
		Color32[] atlas = bfTex.GetPixels32();

		// First we need to extract textures for all the glyphs, making them bigger in the process
		List<BMGlyph> glyphs = bf.glyphs;
		List<Texture2D> glyphTextures = new List<Texture2D>(glyphs.Count);

		for (int i = 0, imax = glyphs.Count; i < imax; ++i)
		{
			BMGlyph glyph = glyphs[i];
			if (glyph.width < 1 || glyph.height < 1) continue;

			int width = glyph.width;
			int height = glyph.height;

			if (effect == Effect.Outline || effect == Effect.Shadow || effect == Effect.Border)
			{
				width += 2;
				height += 2;
				--glyph.offsetX;
				--glyph.offsetY;
			}
			else if (effect == Effect.Crop && width > 2 && height > 2)
			{
				width -= 2;
				height -= 2;
				++glyph.offsetX;
				++glyph.offsetY;
			}

			int size = width * height;
			Color32[] colors = new Color32[size];
			Color32 clear = background;
			clear.a = 0;
			for (int b = 0; b < size; ++b) colors[b] = clear;

			if (effect == Effect.Crop)
			{
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						int fx = x + glyph.x + offsetX + 1;
						int fy = y + (mFont.texHeight - glyph.y - glyph.height) + 1;
						if (mFont.atlas != null) fy += bfTex.height - offsetY;
						colors[x + y * width] = atlas[fx + fy * bfTex.width];
					}
				}
			}
			else
			{
				for (int y = 0; y < glyph.height; ++y)
				{
					for (int x = 0; x < glyph.width; ++x)
					{
						int fx = x + glyph.x + offsetX;
						int fy = y + (mFont.texHeight - glyph.y - glyph.height);
						if (mFont.atlas != null) fy += bfTex.height - offsetY;

						Color c = atlas[fx + fy * bfTex.width];

						if (effect == Effect.Border)
						{
							colors[x + 1 + (y + 1) * width] = c;
						}
						else
						{
							if (effect == Effect.AlphaCurve) c.a = Mathf.Clamp01(mCurve.Evaluate(c.a));

							Color bg = background;
							bg.a = (effect == Effect.BackgroundCurve) ? Mathf.Clamp01(mCurve.Evaluate(c.a)) : c.a;

							Color fg = foreground;
							fg.a = (effect == Effect.ForegroundCurve) ? Mathf.Clamp01(mCurve.Evaluate(c.a)) : c.a;

							if (effect == Effect.Outline || effect == Effect.Shadow)
							{
								colors[x + 1 + (y + 1) * width] = Color.Lerp(bg, c, c.a);
							}
							else
							{
								colors[x + y * width] = Color.Lerp(bg, fg, c.a);
							}
						}
					}
				}

				// Apply the appropriate affect
				if (effect == Effect.Shadow) NGUIEditorTools.AddShadow(colors, width, height, NGUISettings.backgroundColor);
				else if (effect == Effect.Outline) NGUIEditorTools.AddDepth(colors, width, height, NGUISettings.backgroundColor);
			}

			Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
			tex.SetPixels32(colors);
			tex.Apply();
			glyphTextures.Add(tex);
		}

		// Pack all glyphs into a new texture
		Texture2D final = new Texture2D(bfTex.width, bfTex.height, TextureFormat.ARGB32, false);
		Rect[] rects = final.PackTextures(glyphTextures.ToArray(), 1);
		final.Apply();

		// Make RGB channel use the background color (Unity makes it black by default)
		Color32[] fcs = final.GetPixels32();
		Color32 bc = background;

		for (int i = 0, imax = fcs.Length; i < imax; ++i)
		{
			if (fcs[i].a == 0)
			{
				fcs[i].r = bc.r;
				fcs[i].g = bc.g;
				fcs[i].b = bc.b;
			}
		}

		final.SetPixels32(fcs);
		final.Apply();

		// Update the glyph rectangles
		int index = 0;
		int tw = final.width;
		int th = final.height;

		for (int i = 0, imax = glyphs.Count; i < imax; ++i)
		{
			BMGlyph glyph = glyphs[i];
			if (glyph.width < 1 || glyph.height < 1) continue;

			Rect rect = rects[index++];
			glyph.x = Mathf.RoundToInt(rect.x * tw);
			glyph.y = Mathf.RoundToInt(rect.y * th);
			glyph.width = Mathf.RoundToInt(rect.width * tw);
			glyph.height = Mathf.RoundToInt(rect.height * th);
			glyph.y = th - glyph.y - glyph.height;
		}

		// Update the font's texture dimensions
		mFont.texWidth = final.width;
		mFont.texHeight = final.height;

		if (mFont.atlas == null)
		{
			// Save the final texture
			byte[] bytes = final.EncodeToPNG();
			NGUITools.DestroyImmediate(final);
			System.IO.File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}
		else
		{
			// Update the atlas
			final.name = mFont.spriteName;
			bool val = NGUISettings.atlasTrimming;
			NGUISettings.atlasTrimming = false;
			UIAtlasMaker.AddOrUpdate(mFont.atlas, final);
			NGUISettings.atlasTrimming = val;
			NGUITools.DestroyImmediate(final);
		}

		// Cleanup
		for (int i = 0; i < glyphTextures.Count; ++i)
			NGUITools.DestroyImmediate(glyphTextures[i]);

		// Refresh all labels
		mFont.MarkAsChanged();
	}
}
