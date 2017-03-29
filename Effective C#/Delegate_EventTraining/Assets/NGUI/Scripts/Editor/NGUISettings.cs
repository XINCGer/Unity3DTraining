//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Unity doesn't keep the values of static variables after scripts change get recompiled. One way around this
/// is to store the references in EditorPrefs -- retrieve them at start, and save them whenever something changes.
/// </summary>

public class NGUISettings
{
	public enum ColorMode
	{
		Orange,
		Green,
		Blue,
	}

#region Generic Get and Set methods
	/// <summary>
	/// Save the specified boolean value in settings.
	/// </summary>

	static public void SetBool (string name, bool val) { EditorPrefs.SetBool(name, val); }

	/// <summary>
	/// Save the specified integer value in settings.
	/// </summary>

	static public void SetInt (string name, int val) { EditorPrefs.SetInt(name, val); }

	/// <summary>
	/// Save the specified float value in settings.
	/// </summary>

	static public void SetFloat (string name, float val) { EditorPrefs.SetFloat(name, val); }

	/// <summary>
	/// Save the specified string value in settings.
	/// </summary>

	static public void SetString (string name, string val) { EditorPrefs.SetString(name, val); }

	/// <summary>
	/// Save the specified color value in settings.
	/// </summary>

	static public void SetColor (string name, Color c) { SetString(name, c.r + " " + c.g + " " + c.b + " " + c.a); }

	/// <summary>
	/// Save the specified enum value to settings.
	/// </summary>

	static public void SetEnum (string name, System.Enum val) { SetString(name, val.ToString()); }

	/// <summary>
	/// Save the specified object in settings.
	/// </summary>

	static public void Set (string name, Object obj)
	{
		if (obj == null)
		{
			EditorPrefs.DeleteKey(name);
		}
		else
		{
			if (obj != null)
			{
				string path = AssetDatabase.GetAssetPath(obj);

				if (!string.IsNullOrEmpty(path))
				{
					EditorPrefs.SetString(name, path);
				}
				else
				{
					EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
				}
			}
			else EditorPrefs.DeleteKey(name);
		}
	}

	/// <summary>
	/// Get the previously saved boolean value.
	/// </summary>

	static public bool GetBool (string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

	/// <summary>
	/// Get the previously saved integer value.
	/// </summary>

	static public int GetInt (string name, int defaultValue) { return EditorPrefs.GetInt(name, defaultValue); }

	/// <summary>
	/// Get the previously saved float value.
	/// </summary>

	static public float GetFloat (string name, float defaultValue) { return EditorPrefs.GetFloat(name, defaultValue); }

	/// <summary>
	/// Get the previously saved string value.
	/// </summary>

	static public string GetString (string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }
	
	/// <summary>
	/// Get a previously saved color value.
	/// </summary>

	static public Color GetColor (string name, Color c)
	{
		string strVal = GetString(name, c.r + " " + c.g + " " + c.b + " " + c.a);
		string[] parts = strVal.Split(' ');

		if (parts.Length == 4)
		{
			float.TryParse(parts[0], out c.r);
			float.TryParse(parts[1], out c.g);
			float.TryParse(parts[2], out c.b);
			float.TryParse(parts[3], out c.a);
		}
		return c;
	}

	/// <summary>
	/// Get a previously saved enum from settings.
	/// </summary>

	static public T GetEnum<T> (string name, T defaultValue)
	{
		string val = GetString(name, defaultValue.ToString());
		string[] names = System.Enum.GetNames(typeof(T));
		System.Array values = System.Enum.GetValues(typeof(T));
		
		for (int i = 0; i < names.Length; ++i)
		{
			if (names[i] == val)
				return (T)values.GetValue(i);
		}
		return defaultValue;
	}

	/// <summary>
	/// Get a previously saved object from settings.
	/// </summary>

	static public T Get<T> (string name, T defaultValue) where T : Object
	{
		string path = EditorPrefs.GetString(name);
		if (string.IsNullOrEmpty(path)) return null;
		
		T retVal = NGUIEditorTools.LoadAsset<T>(path);
		
		if (retVal == null)
		{
			int id;
			if (int.TryParse(path, out id))
				return EditorUtility.InstanceIDToObject(id) as T;
		}
		return retVal;
	}
#endregion

#region Convenience accessor properties

	static public bool showTransformHandles
	{
		get { return GetBool("NGUI Transform Handles", false); }
		set { SetBool("NGUI Transform Handles", value); }
	}

	static public bool minimalisticLook
	{
		get { return GetBool("NGUI Minimalistic", false); }
		set { SetBool("NGUI Minimalistic", value); }
	}

	static public bool unifiedTransform
	{
		get { return GetBool("NGUI Unified", false); }
		set { SetBool("NGUI Unified", value); }
	}

	static public Color color
	{
		get { return GetColor("NGUI Color", Color.white); }
		set { SetColor("NGUI Color", value); }
	}

	static public Color foregroundColor
	{
		get { return GetColor("NGUI FG Color", Color.white); }
		set { SetColor("NGUI FG Color", value); }
	}

	static public Color backgroundColor
	{
		get { return GetColor("NGUI BG Color", Color.black); }
		set { SetColor("NGUI BG Color", value); }
	}

	static public ColorMode colorMode
	{
		get { return GetEnum("NGUI Color Mode", ColorMode.Blue); }
		set { SetEnum("NGUI Color Mode", value); }
	}

	static public Object ambigiousFont
	{
		get
		{
			Font fnt = Get<Font>("NGUI Dynamic Font", null);
			if (fnt != null) return fnt;
			return Get<UIFont>("NGUI Bitmap Font", null);
		}
		set
		{
			if (value == null)
			{
				Set("NGUI Bitmap Font", null);
				Set("NGUI Dynamic Font", null);
			}
			else if (value is Font)
			{
				Set("NGUI Bitmap Font", null);
				Set("NGUI Dynamic Font", value as Font);
			}
			else if (value is UIFont)
			{
				Set("NGUI Bitmap Font", value as UIFont);
				Set("NGUI Dynamic Font", null);
			}
		}
	}

	static public UIAtlas atlas
	{
		get { return Get<UIAtlas>("NGUI Atlas", null); }
		set { Set("NGUI Atlas", value); }
	}

	static public Texture texture
	{
		get { return Get<Texture>("NGUI Texture", null); }
		set { Set("NGUI Texture", value); }
	}

	static public Sprite sprite2D
	{
		get { return Get<Sprite>("NGUI Sprite2D", null); }
		set { Set("NGUI Sprite2D", value); }
	}

	static public string selectedSprite
	{
		get { return GetString("NGUI Sprite", null); }
		set { SetString("NGUI Sprite", value); }
	}

	static public UIWidget.Pivot pivot
	{
		get { return GetEnum("NGUI Pivot", UIWidget.Pivot.Center); }
		set { SetEnum("NGUI Pivot", value); }
	}

	static public int layer
	{
		get
		{
			int layer = GetInt("NGUI Layer", -1);
			if (layer == -1) layer = LayerMask.NameToLayer("UI");
			if (layer == -1) layer = LayerMask.NameToLayer("2D UI");
			return (layer == -1) ? 9 : layer;
		}
		set
		{
			SetInt("NGUI Layer", value);
		}
	}

	static public TextAsset fontData
	{
		get { return Get<TextAsset>("NGUI Font Data", null); }
		set { Set("NGUI Font Data", value); }
	}

	static public Texture2D fontTexture
	{
		get { return Get<Texture2D>("NGUI Font Texture", null); }
		set { Set("NGUI Font Texture", value); }
	}

	static public int fontSize
	{
		get { return GetInt("NGUI Font Size", 16); }
		set { SetInt("NGUI Font Size", value); }
	}

	static public int FMSize
	{
		get { return GetInt("NGUI FM Size", 16); }
		set { SetInt("NGUI FM Size", value); }
	}

	static public bool fontKerning
	{
		get { return GetBool("NGUI Font Kerning", true); }
		set { SetBool("NGUI Font Kerning", value); }
	}

	static public FontStyle fontStyle
	{
		get { return GetEnum("NGUI Font Style", FontStyle.Normal); }
		set { SetEnum("NGUI Font Style", value); }
	}

	static public Font dynamicFont
	{
		get { return Get<Font>("NGUI Dynamic Font", null); }
		set { Set("NGUI Dynamic Font", value); }
	}

	static public Font FMFont
	{
		get { return Get<Font>("NGUI FM Font", null); }
		set { Set("NGUI FM Font", value); }
	}

	static public UIFont BMFont
	{
		get { return Get<UIFont>("NGUI BM Font", null); }
		set { Set("NGUI BM Font", value); }
	}

	static public UILabel.Overflow overflowStyle
	{
		get { return GetEnum("NGUI Overflow", UILabel.Overflow.ShrinkContent); }
		set { SetEnum("NGUI Overflow", value); }
	}

	static public string partialSprite
	{
		get { return GetString("NGUI Partial", null); }
		set { SetString("NGUI Partial", value); }
	}

	static public int atlasPadding
	{
		get { return GetInt("NGUI Padding", 1); }
		set { SetInt("NGUI Padding", value); }
	}

	static public bool atlasTrimming
	{
		get { return GetBool("NGUI Trim", true); }
		set { SetBool("NGUI Trim", value); }
	}

	static public bool atlasPMA
	{
		get { return GetBool("NGUI PMA", false); }
		set { SetBool("NGUI PMA", value); }
	}

	static public bool unityPacking
	{
		get { return GetBool("NGUI Packing", true); }
		set { SetBool("NGUI Packing", value); }
	}

	static public bool trueColorAtlas
	{
		get { return GetBool("NGUI Truecolor", true); }
		set { SetBool("NGUI Truecolor", value); }
	}

	static public bool autoUpgradeSprites
	{
		get { return GetBool("NGUI AutoUpgrade", false); }
		set { SetBool("NGUI AutoUpgrade", value); }
	}

	static public bool keepPadding
	{
		get { return GetBool("NGUI KeepPadding", false); }
		set { SetBool("NGUI KeepPadding", value); }
	}

	static public bool forceSquareAtlas
	{
		get { return GetBool("NGUI Square", false); }
		set { SetBool("NGUI Square", value); }
	}

	static public bool allow4096
	{
		get { return GetBool("NGUI 4096", true); }
		set { SetBool("NGUI 4096", value); }
	}

	static public bool showAllDCs
	{
		get { return GetBool("NGUI DCs", true); }
		set { SetBool("NGUI DCs", value); }
	}

	static public bool drawGuides
	{
		get { return GetBool("NGUI Guides", false); }
		set { SetBool("NGUI Guides", value); }
	}

	static public string charsToInclude
	{
		get { return GetString("NGUI Chars", ""); }
		set { SetString("NGUI Chars", value); }
	}

	static public string defaultPathToFreeType
	{
		get
		{
			string path = Application.dataPath;
			if (System.IntPtr.Size == 8) path = System.IO.Path.Combine(path, "NGUI/Editor/x86_64/");
			else path = System.IO.Path.Combine(path, "NGUI/Editor/x86/");

			var platform = Application.platform;
			if (platform == RuntimePlatform.WindowsEditor) path = System.IO.Path.Combine(path, "FreeType.dll");
			else if (platform == RuntimePlatform.OSXEditor) path = System.IO.Path.Combine(path, "FreeType.dylib");
			return path.Replace('\\', '/');
		}
	}

	static public string pathToFreeType
	{
		get
		{
			string s = GetString(System.IntPtr.Size == 8 ? "NGUI FreeType64" : "NGUI FreeType", null);
			if (string.IsNullOrEmpty(s)) s = defaultPathToFreeType;
			else if (!System.IO.File.Exists(s)) s = defaultPathToFreeType;
			return s;
		}
		set { SetString(System.IntPtr.Size == 8 ? "NGUI FreeType64" : "NGUI FreeType", value); }
	}

	static public string searchField
	{
		get { return GetString("NGUI Search", null); }
		set { SetString("NGUI Search", value); }
	}

	static public string currentPath
	{
		get { return GetString("NGUI Path", "Assets/"); }
		set { SetString("NGUI Path", value); }
	}
#endregion

	/// <summary>
	/// Convenience method -- add a widget.
	/// </summary>

	static public UIWidget AddWidget (GameObject go)
	{
		UIWidget w = NGUITools.AddWidget<UIWidget>(go);
		w.name = "Container";
		w.pivot = pivot;
		w.width = 100;
		w.height = 100;
		return w;
	}

	/// <summary>
	/// Convenience method -- add a texture.
	/// </summary>

	static public UITexture AddTexture (GameObject go)
	{
		UITexture w = NGUITools.AddWidget<UITexture>(go);
		w.name = "Texture";
		w.pivot = pivot;
		w.mainTexture = texture;
		w.width = 100;
		w.height = 100;
		return w;
	}

	/// <summary>
	/// Convenience method -- add a UnityEngine.Sprite.
	/// </summary>

	static public UI2DSprite Add2DSprite (GameObject go)
	{
		UI2DSprite w = NGUITools.AddWidget<UI2DSprite>(go);
		w.name = "2D Sprite";
		w.pivot = pivot;
		w.sprite2D = sprite2D;
		w.width = 100;
		w.height = 100;
		return w;
	}

	/// <summary>
	/// Convenience method -- add a sprite.
	/// </summary>

	static public UISprite AddSprite (GameObject go)
	{
		UISprite w = NGUITools.AddWidget<UISprite>(go);
		w.name = "Sprite";
		w.atlas = atlas;
		w.spriteName = selectedSprite;

		if (w.atlas != null && !string.IsNullOrEmpty(w.spriteName))
		{
			UISpriteData sp = w.atlas.GetSprite(w.spriteName);
			if (sp != null && sp.hasBorder)
				w.type = UISprite.Type.Sliced;
		}

		w.pivot = pivot;
		w.width = 100;
		w.height = 100;
		w.MakePixelPerfect();
		return w;
	}

	/// <summary>
	/// Convenience method -- add a label with default parameters.
	/// </summary>

	static public UILabel AddLabel (GameObject go)
	{
		UILabel w = NGUITools.AddWidget<UILabel>(go);
		w.name = "Label";
		w.ambigiousFont = ambigiousFont;
		w.text = "New Label";
		w.pivot = pivot;
		w.width = 120;
		w.height = Mathf.Max(20, GetInt("NGUI Font Height", 16));
		w.fontStyle = fontStyle;
		w.fontSize = fontSize;
		w.applyGradient = true;
		w.gradientBottom = new Color(0.7f, 0.7f, 0.7f);
		w.AssumeNaturalSize();
		return w;
	}

	/// <summary>
	/// Convenience method -- add a new panel.
	/// </summary>

	static public UIPanel AddPanel (GameObject go)
	{
		if (go == null) return null;
		int depth = UIPanel.nextUnusedDepth;
		UIPanel panel = NGUITools.AddChild<UIPanel>(go);
		panel.depth = depth;
		return panel;
	}

	/// <summary>
	/// Copy the specified widget's parameters.
	/// </summary>

	static public void CopyWidget (UIWidget widget)
	{
		SetInt("Width", widget.width);
		SetInt("Height", widget.height);
		SetInt("Depth", widget.depth);
		SetColor("Widget Color", widget.color);
		SetEnum("Widget Pivot", widget.pivot);

		if (widget is UISprite) CopySprite(widget as UISprite);
		else if (widget is UILabel) CopyLabel(widget as UILabel);
	}

	/// <summary>
	/// Paste the specified widget's style.
	/// </summary>

	static public void PasteWidget (UIWidget widget, bool fully)
	{
		widget.color = GetColor("Widget Color", widget.color);
		widget.pivot = GetEnum<UIWidget.Pivot>("Widget Pivot", widget.pivot);

		if (fully)
		{
			widget.width = GetInt("Width", widget.width);
			widget.height = GetInt("Height", widget.height);
			widget.depth = GetInt("Depth", widget.depth);
		}

		if (widget is UISprite) PasteSprite(widget as UISprite, fully);
		else if (widget is UILabel) PasteLabel(widget as UILabel, fully);
	}

	/// <summary>
	/// Copy the specified sprite's style.
	/// </summary>

	static void CopySprite (UISprite sp)
	{
		SetString("Atlas", NGUIEditorTools.ObjectToGUID(sp.atlas));
		SetString("Sprite", sp.spriteName);
		SetEnum("Sprite Type", sp.type);
		SetEnum("Left Type", sp.leftType);
		SetEnum("Right Type", sp.rightType);
		SetEnum("Top Type", sp.topType);
		SetEnum("Bottom Type", sp.bottomType);
		SetEnum("Center Type", sp.centerType);
		SetFloat("Fill", sp.fillAmount);
		SetEnum("FDir", sp.fillDirection);
	}

	/// <summary>
	/// Copy the specified label's style.
	/// </summary>

	static void CopyLabel (UILabel lbl)
	{
		SetString("Font", NGUIEditorTools.ObjectToGUID(lbl.ambigiousFont));
		SetInt("Font Size", lbl.fontSize);
		SetEnum("Font Style", lbl.fontStyle);
		SetEnum("Overflow", lbl.overflowMethod);
		SetBool("UseFloatSpacing", lbl.useFloatSpacing);
		SetFloat("FloatSpacingX", lbl.floatSpacingX);
		SetFloat("FloatSpacingY", lbl.floatSpacingY);
		SetInt("SpacingX", lbl.spacingX);
		SetInt("SpacingY", lbl.spacingY);
		SetInt("MaxLines", lbl.maxLineCount);
		SetBool("Encoding", lbl.supportEncoding);
		SetBool("Gradient", lbl.applyGradient);
		SetColor("Gradient B", lbl.gradientBottom);
		SetColor("Gradient T", lbl.gradientTop);
		SetEnum("Effect", lbl.effectStyle);
		SetColor("Effect C", lbl.effectColor);
		SetFloat("Effect X", lbl.effectDistance.x);
		SetFloat("Effect Y", lbl.effectDistance.y);
	}

	/// <summary>
	/// Paste the specified sprite's style.
	/// </summary>

	static void PasteSprite (UISprite sp, bool fully)
	{
		if (fully) sp.atlas = NGUIEditorTools.GUIDToObject<UIAtlas>(GetString("Atlas", null));
		sp.spriteName = GetString("Sprite", sp.spriteName);
		sp.type = GetEnum<UISprite.Type>("Sprite Type", sp.type);
		sp.leftType = GetEnum<UISprite.AdvancedType>("Left Type", UISprite.AdvancedType.Sliced);
		sp.rightType = GetEnum<UISprite.AdvancedType>("Right Type", UISprite.AdvancedType.Sliced);
		sp.topType = GetEnum<UISprite.AdvancedType>("Top Type", UISprite.AdvancedType.Sliced);
		sp.bottomType = GetEnum<UISprite.AdvancedType>("Bottom Type", UISprite.AdvancedType.Sliced);
		sp.centerType = GetEnum<UISprite.AdvancedType>("Center Type", UISprite.AdvancedType.Sliced);
		sp.fillAmount = GetFloat("Fill", sp.fillAmount);
		sp.fillDirection = GetEnum<UISprite.FillDirection>("FDir", sp.fillDirection);
		NGUITools.SetDirty(sp);
	}

	/// <summary>
	/// Paste the specified label's style.
	/// </summary>

	static void PasteLabel (UILabel lbl, bool fully)
	{
		if (fully)
		{
			Object obj = NGUIEditorTools.GUIDToObject(GetString("Font", null));

			if (obj != null)
			{
				if (obj.GetType() == typeof(Font))
				{
					lbl.ambigiousFont = obj as Font;
				}
				else if (obj.GetType() == typeof(GameObject))
				{
					lbl.ambigiousFont = (obj as GameObject).GetComponent<UIFont>();
				}
			}
			lbl.fontSize = GetInt("Font Size", lbl.fontSize);
			lbl.fontStyle = GetEnum<FontStyle>("Font Style", lbl.fontStyle);
		}

		lbl.overflowMethod = GetEnum<UILabel.Overflow>("Overflow", lbl.overflowMethod);
		lbl.useFloatSpacing = GetBool("UseFloatSpacing", lbl.useFloatSpacing);
		lbl.floatSpacingX = GetFloat("FloatSpacingX", lbl.floatSpacingX);
		lbl.floatSpacingY = GetFloat("FloatSpacingY", lbl.floatSpacingY);
		lbl.spacingX = GetInt("SpacingX", lbl.spacingX);
		lbl.spacingY = GetInt("SpacingY", lbl.spacingY);
		lbl.maxLineCount = GetInt("MaxLines", lbl.maxLineCount);
		lbl.supportEncoding = GetBool("Encoding", lbl.supportEncoding);
		lbl.applyGradient = GetBool("Gradient", lbl.applyGradient);
		lbl.gradientBottom = GetColor("Gradient B", lbl.gradientBottom);
		lbl.gradientTop = GetColor("Gradient T", lbl.gradientTop);
		lbl.effectStyle = GetEnum<UILabel.Effect>("Effect", lbl.effectStyle);
		lbl.effectColor = GetColor("Effect C", lbl.effectColor);

		float x = GetFloat("Effect X", lbl.effectDistance.x);
		float y = GetFloat("Effect Y", lbl.effectDistance.y);
		lbl.effectDistance = new Vector2(x, y);
		NGUITools.SetDirty(lbl);
	}
}
