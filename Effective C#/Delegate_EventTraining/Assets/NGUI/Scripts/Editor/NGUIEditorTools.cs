//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Tools for the editor
/// </summary>

static public class NGUIEditorTools
{
	static Texture2D mBackdropTex;
	static Texture2D mContrastTex;
	static Texture2D mGradientTex;
	static GameObject mPrevious;

	/// <summary>
	/// Returns a blank usable 1x1 white texture.
	/// </summary>

	static public Texture2D blankTexture
	{
		get
		{
			return EditorGUIUtility.whiteTexture;
		}
	}

	/// <summary>
	/// Returns a usable texture that looks like a dark checker board.
	/// </summary>

	static public Texture2D backdropTexture
	{
		get
		{
			if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
				new Color(0.1f, 0.1f, 0.1f, 0.5f),
				new Color(0.2f, 0.2f, 0.2f, 0.5f));
			return mBackdropTex;
		}
	}

	/// <summary>
	/// Returns a usable texture that looks like a high-contrast checker board.
	/// </summary>

	static public Texture2D contrastTexture
	{
		get
		{
			if (mContrastTex == null) mContrastTex = CreateCheckerTex(
				new Color(0f, 0f, 0f, 0.5f),
				new Color(1f, 1f, 1f, 0.5f));
			return mContrastTex;
		}
	}

	/// <summary>
	/// Gradient texture is used for title bars / headers.
	/// </summary>

	static public Texture2D gradientTexture
	{
		get
		{
			if (mGradientTex == null) mGradientTex = CreateGradientTex();
			return mGradientTex;
		}
	}

	/// <summary>
	/// Create a white dummy texture.
	/// </summary>

	static Texture2D CreateDummyTex ()
	{
		Texture2D tex = new Texture2D(1, 1);
		tex.name = "[Generated] Dummy Texture";
		tex.hideFlags = HideFlags.DontSave;
		tex.filterMode = FilterMode.Point;
		tex.SetPixel(0, 0, Color.white);
		tex.Apply();
		return tex;
	}

	/// <summary>
	/// Create a checker-background texture
	/// </summary>

	static Texture2D CreateCheckerTex (Color c0, Color c1)
	{
		Texture2D tex = new Texture2D(16, 16);
		tex.name = "[Generated] Checker Texture";
		tex.hideFlags = HideFlags.DontSave;

		for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
		for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
		for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
		for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

		tex.Apply();
		tex.filterMode = FilterMode.Point;
		return tex;
	}

	/// <summary>
	/// Create a gradient texture
	/// </summary>

	static Texture2D CreateGradientTex ()
	{
		Texture2D tex = new Texture2D(1, 16);
		tex.name = "[Generated] Gradient Texture";
		tex.hideFlags = HideFlags.DontSave;

		Color c0 = new Color(1f, 1f, 1f, 0f);
		Color c1 = new Color(1f, 1f, 1f, 0.4f);

		for (int i = 0; i < 16; ++i)
		{
			float f = Mathf.Abs((i / 15f) * 2f - 1f);
			f *= f;
			tex.SetPixel(0, i, Color.Lerp(c0, c1, f));
		}

		tex.Apply();
		tex.filterMode = FilterMode.Bilinear;
		return tex;
	}

	/// <summary>
	/// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
	/// </summary>

	static public void DrawTiledTexture (Rect rect, Texture tex)
	{
		GUI.BeginGroup(rect);
		{
			int width  = Mathf.RoundToInt(rect.width);
			int height = Mathf.RoundToInt(rect.height);

			for (int y = 0; y < height; y += tex.height)
			{
				for (int x = 0; x < width; x += tex.width)
				{
					GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
				}
			}
		}
		GUI.EndGroup();
	}

	/// <summary>
	/// Draw a single-pixel outline around the specified rectangle.
	/// </summary>

	static public void DrawOutline (Rect rect)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = contrastTexture;
			GUI.color = Color.white;
			DrawTiledTexture(new Rect(rect.xMin, rect.yMax, 1f, -rect.height), tex);
			DrawTiledTexture(new Rect(rect.xMax, rect.yMax, 1f, -rect.height), tex);
			DrawTiledTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
			DrawTiledTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
		}
	}

	/// <summary>
	/// Draw a single-pixel outline around the specified rectangle.
	/// </summary>

	static public void DrawOutline (Rect rect, Color color)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = blankTexture;
			GUI.color = color;
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
			GUI.color = Color.white;
		}
	}

	/// <summary>
	/// Draw a selection outline around the specified rectangle.
	/// </summary>

	static public void DrawOutline (Rect rect, Rect relative, Color color)
	{
		if (Event.current.type == EventType.Repaint)
		{
			// Calculate where the outer rectangle would be
			float x = rect.xMin + rect.width * relative.xMin;
			float y = rect.yMax - rect.height * relative.yMin;
			float width = rect.width * relative.width;
			float height = -rect.height * relative.height;
			relative = new Rect(x, y, width, height);

			// Draw the selection
			DrawOutline(relative, color);
		}
	}

	/// <summary>
	/// Draw a selection outline around the specified rectangle.
	/// </summary>

	static public void DrawOutline (Rect rect, Rect relative)
	{
		if (Event.current.type == EventType.Repaint)
		{
			// Calculate where the outer rectangle would be
			float x = rect.xMin + rect.width * relative.xMin;
			float y = rect.yMax - rect.height * relative.yMin;
			float width = rect.width * relative.width;
			float height = -rect.height * relative.height;
			relative = new Rect(x, y, width, height);

			// Draw the selection
			DrawOutline(relative);
		}
	}

	/// <summary>
	/// Draw a 9-sliced outline.
	/// </summary>

	static public void DrawOutline (Rect rect, Rect outer, Rect inner)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Color green = new Color(0.4f, 1f, 0f, 1f);

			DrawOutline(rect, new Rect(outer.x, inner.y, outer.width, inner.height));
			DrawOutline(rect, new Rect(inner.x, outer.y, inner.width, outer.height));
			DrawOutline(rect, outer, green);
		}
	}

	/// <summary>
	/// Draw a checkered background for the specified texture.
	/// </summary>

	static public Rect DrawBackground (Texture2D tex, float ratio)
	{
		Rect rect = GUILayoutUtility.GetRect(0f, 0f);
		rect.width = Screen.width - rect.xMin;
		rect.height = rect.width * ratio;
		GUILayout.Space(rect.height);

		if (Event.current.type == EventType.Repaint)
		{
			Texture2D blank = blankTexture;
			Texture2D check = backdropTexture;

			// Lines above and below the texture rectangle
			GUI.color = new Color(0f, 0f, 0f, 0.2f);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMin - 1, rect.width, 1f), blank);
			GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), blank);
			GUI.color = Color.white;

			// Checker background
			DrawTiledTexture(rect, check);
		}
		return rect;
	}

	/// <summary>
	/// Draw a visible separator in addition to adding some padding.
	/// </summary>

	static public void DrawSeparator ()
	{
		GUILayout.Space(12f);

		if (Event.current.type == EventType.Repaint)
		{
			Texture2D tex = blankTexture;
			Rect rect = GUILayoutUtility.GetLastRect();
			GUI.color = new Color(0f, 0f, 0f, 0.25f);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
			GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
			GUI.color = Color.white;
		}
	}

	/// <summary>
	/// Convenience function that displays a list of sprites and returns the selected value.
	/// </summary>

	static public string DrawList (string field, string[] list, string selection, params GUILayoutOption[] options)
	{
		if (list != null && list.Length > 0)
		{
			int index = 0;
			if (string.IsNullOrEmpty(selection)) selection = list[0];

			// We need to find the sprite in order to have it selected
			if (!string.IsNullOrEmpty(selection))
			{
				for (int i = 0; i < list.Length; ++i)
				{
					if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
					{
						index = i;
						break;
					}
				}
			}

			// Draw the sprite selection popup
			index = string.IsNullOrEmpty(field) ?
				EditorGUILayout.Popup(index, list, options) :
				EditorGUILayout.Popup(field, index, list, options);

			return list[index];
		}
		return null;
	}

	/// <summary>
	/// Convenience function that displays a list of sprites and returns the selected value.
	/// </summary>

	static public string DrawAdvancedList (string field, string[] list, string selection, params GUILayoutOption[] options)
	{
		if (list != null && list.Length > 0)
		{
			int index = 0;
			if (string.IsNullOrEmpty(selection)) selection = list[0];

			// We need to find the sprite in order to have it selected
			if (!string.IsNullOrEmpty(selection))
			{
				for (int i = 0; i < list.Length; ++i)
				{
					if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
					{
						index = i;
						break;
					}
				}
			}

			// Draw the sprite selection popup
			index = string.IsNullOrEmpty(field) ?
				DrawPrefixList(index, list, options) :
				DrawPrefixList(field, index, list, options);

			return list[index];
		}
		return null;
	}

	/// <summary>
	/// Helper function that returns the selected root object.
	/// </summary>

	static public GameObject SelectedRoot () { return SelectedRoot(false); }

	/// <summary>
	/// Helper function that returns the selected root object.
	/// </summary>

	static public GameObject SelectedRoot (bool createIfMissing)
	{
		GameObject go = Selection.activeGameObject;

		// Only use active objects
		if (go != null && !NGUITools.GetActive(go)) go = null;

		// Try to find a panel
		UIPanel p = (go != null) ? NGUITools.FindInParents<UIPanel>(go) : null;

		// No selection? Try to find the root automatically
		if (p == null)
		{
			UIPanel[] panels = NGUITools.FindActive<UIPanel>();
			if (panels.Length > 0) go = panels[0].gameObject;
		}

		if (createIfMissing && go == null)
		{
			// No object specified -- find the first panel
			if (go == null)
			{
				UIPanel panel = GameObject.FindObjectOfType(typeof(UIPanel)) as UIPanel;
				if (panel != null) go = panel.gameObject;
			}

			// No UI present -- create a new one
			if (go == null) go = UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D);
		}
		return go;
	}

	/// <summary>
	/// Helper function that checks to see if this action would break the prefab connection.
	/// </summary>

	static public bool WillLosePrefab (GameObject root)
	{
		if (root == null) return false;

		if (root.transform != null)
		{
			// Check if the selected object is a prefab instance and display a warning
			PrefabType type = PrefabUtility.GetPrefabType(root);

			if (type == PrefabType.PrefabInstance)
			{
				return EditorUtility.DisplayDialog("Losing prefab",
					"This action will lose the prefab connection. Are you sure you wish to continue?",
					"Continue", "Cancel");
			}
		}
		return true;
	}

	/// <summary>
	/// Change the import settings of the specified texture asset, making it readable.
	/// </summary>

	static public bool MakeTextureReadable (string path, bool force)
	{
		if (string.IsNullOrEmpty(path)) return false;
		TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
		if (ti == null) return false;

		TextureImporterSettings settings = new TextureImporterSettings();
		ti.ReadTextureSettings(settings);

		if (force || !settings.readable || settings.npotScale != TextureImporterNPOTScale.None || settings.alphaIsTransparency)
		{
			settings.readable = true;
#if !UNITY_4_7 && !UNITY_5_3 && !UNITY_5_4
			if (NGUISettings.trueColorAtlas)
			{
				var platform = ti.GetDefaultPlatformTextureSettings();
				platform.format = TextureImporterFormat.RGBA32;
			}
#else
			if (NGUISettings.trueColorAtlas) settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;
#endif
			settings.npotScale = TextureImporterNPOTScale.None;
			settings.alphaIsTransparency = false;
			ti.SetTextureSettings(settings);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
		}
		return true;
	}

	/// <summary>
	/// Change the import settings of the specified texture asset, making it suitable to be used as a texture atlas.
	/// </summary>

	static bool MakeTextureAnAtlas (string path, bool force, bool alphaTransparency)
	{
		if (string.IsNullOrEmpty(path)) return false;
		var ti = AssetImporter.GetAtPath(path) as TextureImporter;
		if (ti == null) return false;

		var settings = new TextureImporterSettings();
		ti.ReadTextureSettings(settings);

		if (force || settings.readable ||
#if UNITY_5_5_OR_NEWER
			ti.maxTextureSize < 4096 ||
			(NGUISettings.trueColorAtlas && ti.textureCompression != TextureImporterCompression.Uncompressed) ||
#else
			settings.maxTextureSize < 4096 ||
#endif
			settings.wrapMode != TextureWrapMode.Clamp ||
			settings.npotScale != TextureImporterNPOTScale.ToNearest)
		{
			settings.readable = false;
#if !UNITY_4_7 && !UNITY_5_3 && !UNITY_5_4
			ti.maxTextureSize = 4096;
#else
			settings.maxTextureSize = 4096;
#endif
			settings.wrapMode = TextureWrapMode.Clamp;
			settings.npotScale = TextureImporterNPOTScale.ToNearest;

			if (NGUISettings.trueColorAtlas)
			{
#if UNITY_5_5_OR_NEWER
				ti.textureCompression = TextureImporterCompression.Uncompressed;
#else
				settings.textureFormat = TextureImporterFormat.ARGB32;
#endif
				settings.filterMode = FilterMode.Trilinear;
			}

			settings.aniso = 4;
			settings.alphaIsTransparency = alphaTransparency;
			ti.SetTextureSettings(settings);
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
		}
		return true;
	}

	/// <summary>
	/// Fix the import settings for the specified texture, re-importing it if necessary.
	/// </summary>

	static public Texture2D ImportTexture (string path, bool forInput, bool force, bool alphaTransparency)
	{
		if (!string.IsNullOrEmpty(path))
		{
			if (forInput) { if (!MakeTextureReadable(path, force)) return null; }
			else if (!MakeTextureAnAtlas(path, force, alphaTransparency)) return null;
			//return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

			Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			return tex;
		}
		return null;
	}

	/// <summary>
	/// Fix the import settings for the specified texture, re-importing it if necessary.
	/// </summary>

	static public Texture2D ImportTexture (Texture tex, bool forInput, bool force, bool alphaTransparency)
	{
		if (tex != null)
		{
			string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
			return ImportTexture(path, forInput, force, alphaTransparency);
		}
		return null;
	}

	/// <summary>
	/// Figures out the saveable filename for the texture of the specified atlas.
	/// </summary>

	static public string GetSaveableTexturePath (UIAtlas atlas)
	{
		// Path where the texture atlas will be saved
		string path = "";

		// If the atlas already has a texture, overwrite its texture
		if (atlas.texture != null)
		{
			path = AssetDatabase.GetAssetPath(atlas.texture.GetInstanceID());

			if (!string.IsNullOrEmpty(path))
			{
				int dot = path.LastIndexOf('.');
				return path.Substring(0, dot) + ".png";
			}
		}

		// No texture to use -- figure out a name using the atlas
		path = AssetDatabase.GetAssetPath(atlas.GetInstanceID());
		path = string.IsNullOrEmpty(path) ? "Assets/" + atlas.name + ".png" : path.Replace(".prefab", ".png");
		return path;
	}

	/// <summary>
	/// Helper function that returns the folder where the current selection resides.
	/// </summary>

	static public string GetSelectionFolder ()
	{
		if (Selection.activeObject != null)
		{
			string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

			if (!string.IsNullOrEmpty(path))
			{
				int dot = path.LastIndexOf('.');
				int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
				if (slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
			}
		}
		return "Assets/";
	}

	/// <summary>
	/// Struct type for the integer vector field below.
	/// </summary>

	public struct IntVector
	{
		public int x;
		public int y;
	}

	/// <summary>
	/// Integer vector field.
	/// </summary>

	static public IntVector IntPair (string prefix, string leftCaption, string rightCaption, int x, int y)
	{
		GUILayout.BeginHorizontal();

		if (string.IsNullOrEmpty(prefix))
		{
			GUILayout.Space(82f);
		}
		else
		{
			GUILayout.Label(prefix, GUILayout.Width(74f));
		}

		NGUIEditorTools.SetLabelWidth(48f);

		IntVector retVal;
		retVal.x = EditorGUILayout.IntField(leftCaption, x, GUILayout.MinWidth(30f));
		retVal.y = EditorGUILayout.IntField(rightCaption, y, GUILayout.MinWidth(30f));

		NGUIEditorTools.SetLabelWidth(80f);

		GUILayout.EndHorizontal();
		return retVal;
	}

	/// <summary>
	/// Integer rectangle field.
	/// </summary>

	static public Rect IntRect (string prefix, Rect rect)
	{
		int left	= Mathf.RoundToInt(rect.xMin);
		int top		= Mathf.RoundToInt(rect.yMin);
		int width	= Mathf.RoundToInt(rect.width);
		int height	= Mathf.RoundToInt(rect.height);

		NGUIEditorTools.IntVector a = NGUIEditorTools.IntPair(prefix, "Left", "Top", left, top);
		NGUIEditorTools.IntVector b = NGUIEditorTools.IntPair(null, "Width", "Height", width, height);

		return new Rect(a.x, a.y, b.x, b.y);
	}

	/// <summary>
	/// Integer vector field.
	/// </summary>

	static public Vector4 IntPadding (string prefix, Vector4 v)
	{
		int left	= Mathf.RoundToInt(v.x);
		int top		= Mathf.RoundToInt(v.y);
		int right	= Mathf.RoundToInt(v.z);
		int bottom	= Mathf.RoundToInt(v.w);

		NGUIEditorTools.IntVector a = NGUIEditorTools.IntPair(prefix, "Left", "Top", left, top);
		NGUIEditorTools.IntVector b = NGUIEditorTools.IntPair(null, "Right", "Bottom", right, bottom);

		return new Vector4(a.x, a.y, b.x, b.y);
	}

	/// <summary>
	/// Find all scene components, active or inactive.
	/// </summary>

	static public List<T> FindAll<T> () where T : Component
	{
		T[] comps = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];

		List<T> list = new List<T>();

		foreach (T comp in comps)
		{
			if (comp.gameObject.hideFlags == 0)
			{
				string path = AssetDatabase.GetAssetPath(comp.gameObject);
				if (string.IsNullOrEmpty(path)) list.Add(comp);
			}
		}
		return list;
	}

	static public bool DrawPrefixButton (string text)
	{
		return GUILayout.Button(text, "DropDown", GUILayout.Width(76f));
	}

	static public bool DrawPrefixButton (string text, params GUILayoutOption[] options)
	{
		return GUILayout.Button(text, "DropDown", options);
	}

	static public int DrawPrefixList (int index, string[] list, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(index, list, "DropDown", options);
	}

	static public int DrawPrefixList (string text, int index, string[] list, params GUILayoutOption[] options)
	{
		return EditorGUILayout.Popup(text, index, list, "DropDown", options);
	}

	/// <summary>
	/// Draw a sprite preview.
	/// </summary>

	static public void DrawSprite (Texture2D tex, Rect rect, UISpriteData sprite, Color color)
	{
		DrawSprite(tex, rect, sprite, color, null);
	}

	/// <summary>
	/// Draw a sprite preview.
	/// </summary>

	static public void DrawSprite (Texture2D tex, Rect drawRect, UISpriteData sprite, Color color, Material mat)
	{
		if (!tex || sprite == null) return;
		DrawSprite(tex, drawRect, color, mat, sprite.x, sprite.y, sprite.width, sprite.height,
			sprite.borderLeft, sprite.borderBottom, sprite.borderRight, sprite.borderTop);
	}

	/// <summary>
	/// Draw a sprite preview.
	/// </summary>

	static public void DrawSprite (Texture2D tex, Rect drawRect, Color color, Rect textureRect, Vector4 border)
	{
		NGUIEditorTools.DrawSprite(tex, drawRect, color, null,
			Mathf.RoundToInt(textureRect.x),
			Mathf.RoundToInt(tex.height - textureRect.y - textureRect.height),
			Mathf.RoundToInt(textureRect.width),
			Mathf.RoundToInt(textureRect.height),
			Mathf.RoundToInt(border.x),
			Mathf.RoundToInt(border.y),
			Mathf.RoundToInt(border.z),
			Mathf.RoundToInt(border.w));
	}

	/// <summary>
	/// Draw a sprite preview.
	/// </summary>

	static public void DrawSprite (Texture2D tex, Rect drawRect, Color color, Material mat,
		int x, int y, int width, int height, int borderLeft, int borderBottom, int borderRight, int borderTop)
	{
		if (!tex) return;

		// Create the texture rectangle that is centered inside rect.
		Rect outerRect = drawRect;
		outerRect.width = width;
		outerRect.height = height;

		if (width > 0)
		{
			float f = drawRect.width / outerRect.width;
			outerRect.width *= f;
			outerRect.height *= f;
		}

		if (drawRect.height > outerRect.height)
		{
			outerRect.y += (drawRect.height - outerRect.height) * 0.5f;
		}
		else if (outerRect.height > drawRect.height)
		{
			float f = drawRect.height / outerRect.height;
			outerRect.width *= f;
			outerRect.height *= f;
		}

		if (drawRect.width > outerRect.width) outerRect.x += (drawRect.width - outerRect.width) * 0.5f;

		// Draw the background
		NGUIEditorTools.DrawTiledTexture(outerRect, NGUIEditorTools.backdropTexture);

		// Draw the sprite
		GUI.color = color;

		if (mat == null)
		{
			Rect uv = new Rect(x, y, width, height);
			uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);
			GUI.DrawTextureWithTexCoords(outerRect, tex, uv, true);
		}
		else
		{
			// NOTE: There is an issue in Unity that prevents it from clipping the drawn preview
			// using BeginGroup/EndGroup, and there is no way to specify a UV rect... le'suq.
			UnityEditor.EditorGUI.DrawPreviewTexture(outerRect, tex, mat);
		}

		if (Selection.activeGameObject == null || Selection.gameObjects.Length == 1)
		{
			// Draw the border indicator lines
			GUI.BeginGroup(outerRect);
			{
				tex = NGUIEditorTools.contrastTexture;
				GUI.color = Color.white;

				if (borderLeft > 0)
				{
					float x0 = (float)borderLeft / width * outerRect.width - 1;
					NGUIEditorTools.DrawTiledTexture(new Rect(x0, 0f, 1f, outerRect.height), tex);
				}

				if (borderRight > 0)
				{
					float x1 = (float)(width - borderRight) / width * outerRect.width - 1;
					NGUIEditorTools.DrawTiledTexture(new Rect(x1, 0f, 1f, outerRect.height), tex);
				}

				if (borderBottom > 0)
				{
					float y0 = (float)(height - borderBottom) / height * outerRect.height - 1;
					NGUIEditorTools.DrawTiledTexture(new Rect(0f, y0, outerRect.width, 1f), tex);
				}

				if (borderTop > 0)
				{
					float y1 = (float)borderTop / height * outerRect.height - 1;
					NGUIEditorTools.DrawTiledTexture(new Rect(0f, y1, outerRect.width, 1f), tex);
				}
			}
			GUI.EndGroup();

			// Draw the lines around the sprite
			Handles.color = Color.black;
			Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMin, outerRect.yMax));
			Handles.DrawLine(new Vector3(outerRect.xMax, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMax));
			Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMin));
			Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMax), new Vector3(outerRect.xMax, outerRect.yMax));

			// Sprite size label
			string text = string.Format("Sprite Size: {0}x{1}", Mathf.RoundToInt(width), Mathf.RoundToInt(height));
			EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(Screen.width, 18f), text);
		}
	}

	/// <summary>
	/// Draw the specified sprite.
	/// </summary>

	static public void DrawTexture (Texture2D tex, Rect rect, Rect uv, Color color)
	{
		DrawTexture(tex, rect, uv, color, null);
	}

	/// <summary>
	/// Draw the specified sprite.
	/// </summary>

	static public void DrawTexture (Texture2D tex, Rect rect, Rect uv, Color color, Material mat)
	{
		int w = Mathf.RoundToInt(tex.width * uv.width);
		int h = Mathf.RoundToInt(tex.height * uv.height);

		// Create the texture rectangle that is centered inside rect.
		Rect outerRect = rect;
		outerRect.width = w;
		outerRect.height = h;

		if (outerRect.width > 0f)
		{
			float f = rect.width / outerRect.width;
			outerRect.width *= f;
			outerRect.height *= f;
		}

		if (rect.height > outerRect.height)
		{
			outerRect.y += (rect.height - outerRect.height) * 0.5f;
		}
		else if (outerRect.height > rect.height)
		{
			float f = rect.height / outerRect.height;
			outerRect.width *= f;
			outerRect.height *= f;
		}

		if (rect.width > outerRect.width) outerRect.x += (rect.width - outerRect.width) * 0.5f;

		// Draw the background
		NGUIEditorTools.DrawTiledTexture(outerRect, NGUIEditorTools.backdropTexture);

		// Draw the sprite
		GUI.color = color;
		
		if (mat == null)
		{
			GUI.DrawTextureWithTexCoords(outerRect, tex, uv, true);
		}
		else
		{
			// NOTE: There is an issue in Unity that prevents it from clipping the drawn preview
			// using BeginGroup/EndGroup, and there is no way to specify a UV rect... le'suq.
			UnityEditor.EditorGUI.DrawPreviewTexture(outerRect, tex, mat);
		}
		GUI.color = Color.white;

		// Draw the lines around the sprite
		Handles.color = Color.black;
		Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMin, outerRect.yMax));
		Handles.DrawLine(new Vector3(outerRect.xMax, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMax));
		Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMin));
		Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMax), new Vector3(outerRect.xMax, outerRect.yMax));

		// Sprite size label
		string text = string.Format("Texture Size: {0}x{1}", w, h);
		EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(Screen.width, 18f), text);
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, UIAtlas atlas, string spriteName, SpriteSelector.Callback callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(76f));

		if (GUILayout.Button(spriteName, "MiniPullDown", options))
		{
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = spriteName;
			SpriteSelector.Show(callback);
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawPaddedSpriteField (string label, UIAtlas atlas, string spriteName, SpriteSelector.Callback callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(76f));

		if (GUILayout.Button(spriteName, "MiniPullDown", options))
		{
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = spriteName;
			SpriteSelector.Show(callback);
		}
		NGUIEditorTools.DrawPadding();
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, string caption, UIAtlas atlas, string spriteName, SpriteSelector.Callback callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(76f));

		if (atlas.GetSprite(spriteName) == null)
			spriteName = "";

		if (GUILayout.Button(spriteName, "MiniPullDown", options))
		{
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = spriteName;
			SpriteSelector.Show(callback);
		}
		
		if (!string.IsNullOrEmpty(caption))
		{
			GUILayout.Space(20f);
			GUILayout.Label(caption);
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Draw a simple sprite selection button.
	/// </summary>

	static public bool DrawSpriteField (UIAtlas atlas, string spriteName, SpriteSelector.Callback callback, params GUILayoutOption[] options)
	{
		if (atlas.GetSprite(spriteName) == null)
			spriteName = "";

		if (NGUIEditorTools.DrawPrefixButton(spriteName, options))
		{
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = spriteName;
			SpriteSelector.Show(callback);
			return true;
		}
		return false;
	}

	static string mEditedName = null;
	static string mLastSprite = null;

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, SerializedObject ob, string spriteField, params GUILayoutOption[] options)
	{
		DrawSpriteField(label, ob, ob.FindProperty("atlas"), ob.FindProperty(spriteField), 82f, false, false, options);
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, SerializedObject ob, SerializedProperty atlas, SerializedProperty sprite, params GUILayoutOption[] options)
	{
		DrawSpriteField(label, ob, atlas, sprite, 72f, false, false, options);
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, SerializedObject ob, SerializedProperty atlas, SerializedProperty sprite, bool removable, params GUILayoutOption[] options)
	{
		DrawSpriteField(label, ob, atlas, sprite, 72f, false, removable, options);
	}

	/// <summary>
	/// Draw a sprite selection field.
	/// </summary>

	static public void DrawSpriteField (string label, SerializedObject ob, SerializedProperty atlas, SerializedProperty sprite, float width, bool padded, bool removable, params GUILayoutOption[] options)
	{
		if (atlas != null && atlas.objectReferenceValue != null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(width));

			if (sprite == null)
			{
				GUILayout.Label("Invalid field name");
			}
			else
			{
				string spriteName = sprite.hasMultipleDifferentValues ? "-" : sprite.stringValue;

				GUILayout.BeginHorizontal();

				EditorGUI.BeginDisabledGroup(atlas.hasMultipleDifferentValues);
				{
					if (GUILayout.Button(spriteName, "MiniPullDown", options))
						SpriteSelector.Show(ob, sprite, atlas.objectReferenceValue as UIAtlas);
				}
				EditorGUI.EndDisabledGroup();

				EditorGUI.BeginDisabledGroup(!removable);
				if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f))) sprite.stringValue = "";
				EditorGUI.EndDisabledGroup();
				if (padded) GUILayout.Space(12f);
				else GUILayout.Space(-6f);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Convenience function that displays a list of sprites and returns the selected value.
	/// </summary>

	static public void DrawAdvancedSpriteField (UIAtlas atlas, string spriteName, SpriteSelector.Callback callback, bool editable,
		params GUILayoutOption[] options)
	{
		if (atlas == null) return;

		// Give the user a warning if there are no sprites in the atlas
		if (atlas.spriteList.Count == 0)
		{
			EditorGUILayout.HelpBox("No sprites found", MessageType.Warning);
			return;
		}

		// Sprite selection drop-down list
		GUILayout.BeginHorizontal();
		{
			if (NGUIEditorTools.DrawPrefixButton("Sprite"))
			{
				NGUISettings.atlas = atlas;
				NGUISettings.selectedSprite = spriteName;
				SpriteSelector.Show(callback);
			}

			if (editable)
			{
				if (!string.Equals(spriteName, mLastSprite))
				{
					mLastSprite = spriteName;
					mEditedName = null;
				}

				string newName = GUILayout.TextField(string.IsNullOrEmpty(mEditedName) ? spriteName : mEditedName);

				if (newName != spriteName)
				{
					mEditedName = newName;

					if (GUILayout.Button("Rename", GUILayout.Width(60f)))
					{
						UISpriteData sprite = atlas.GetSprite(spriteName);

						if (sprite != null)
						{
							NGUIEditorTools.RegisterUndo("Edit Sprite Name", atlas);
							sprite.name = newName;

							List<UISprite> sprites = FindAll<UISprite>();

							for (int i = 0; i < sprites.Count; ++i)
							{
								UISprite sp = sprites[i];

								if (sp.atlas == atlas && sp.spriteName == spriteName)
								{
									NGUIEditorTools.RegisterUndo("Edit Sprite Name", sp);
									sp.spriteName = newName;
								}
							}

							mLastSprite = newName;
							spriteName = newName;
							mEditedName = null;

							NGUISettings.atlas = atlas;
							NGUISettings.selectedSprite = spriteName;
						}
					}
				}
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(spriteName, "HelpBox", GUILayout.Height(18f));
				NGUIEditorTools.DrawPadding();
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Edit", GUILayout.Width(40f)))
				{
					NGUISettings.atlas = atlas;
					NGUISettings.selectedSprite = spriteName;
					Select(atlas.gameObject);
				}
			}
		}
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Repaints all inspector windows related to sprite drawing.
	/// </summary>

	static public void RepaintSprites ()
	{
		if (UIAtlasInspector.instance != null)
			UIAtlasInspector.instance.Repaint();

		if (UIAtlasMaker.instance != null)
			UIAtlasMaker.instance.Repaint();

		if (SpriteSelector.instance != null)
			SpriteSelector.instance.Repaint();
	}

	/// <summary>
	/// Select the specified sprite within the currently selected atlas.
	/// </summary>

	static public void SelectSprite (string spriteName)
	{
		if (NGUISettings.atlas != null)
		{
			NGUISettings.selectedSprite = spriteName;
			NGUIEditorTools.Select(NGUISettings.atlas.gameObject);
			RepaintSprites();
		}
	}

	/// <summary>
	/// Select the specified atlas and sprite.
	/// </summary>

	static public void SelectSprite (UIAtlas atlas, string spriteName)
	{
		if (atlas != null)
		{
			NGUISettings.atlas = atlas;
			NGUISettings.selectedSprite = spriteName;
			NGUIEditorTools.Select(atlas.gameObject);
			RepaintSprites();
		}
	}

	/// <summary>
	/// Select the specified game object and remember what was selected before.
	/// </summary>

	static public void Select (GameObject go)
	{
		mPrevious = Selection.activeGameObject;
		Selection.activeGameObject = go;
	}
	
	/// <summary>
	/// Select the previous game object.
	/// </summary>

	static public void SelectPrevious ()
	{
		if (mPrevious != null)
		{
			Selection.activeGameObject = mPrevious;
			mPrevious = null;
		}
	}

	/// <summary>
	/// Previously selected game object.
	/// </summary>

	static public GameObject previousSelection { get { return mPrevious; } }

	/// <summary>
	/// Helper function that checks to see if the scale is uniform.
	/// </summary>

	static public bool IsUniform (Vector3 scale)
	{
		return Mathf.Approximately(scale.x, scale.y) && Mathf.Approximately(scale.x, scale.z);
	}

	/// <summary>
	/// Check to see if the specified game object has a uniform scale.
	/// </summary>

	static public bool IsUniform (GameObject go)
	{
		if (go == null) return true;

		if (go.GetComponent<UIWidget>() != null)
		{
			Transform parent = go.transform.parent;
			return parent == null || IsUniform(parent.gameObject);
		}
		return IsUniform(go.transform.lossyScale);
	}

	/// <summary>
	/// Fix uniform scaling of the specified object.
	/// </summary>

	static public void FixUniform (GameObject go)
	{
		Transform t = go.transform;

		while (t != null && t.gameObject.GetComponent<UIRoot>() == null)
		{
			if (!NGUIEditorTools.IsUniform(t.localScale))
			{
				NGUIEditorTools.RegisterUndo("Uniform scaling fix", t);
				t.localScale = Vector3.one;
				EditorUtility.SetDirty(t);
			}
			t = t.parent;
		}
	}

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawMinimalisticHeader (string text) { return DrawHeader(text, text, false, true); }

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text) { return DrawHeader(text, text, false, NGUISettings.minimalisticLook); }

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text, string key) { return DrawHeader(text, key, false, NGUISettings.minimalisticLook); }

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text, string key, bool forceOn, bool minimalistic)
	{
		bool state = EditorPrefs.GetBool(key, true);

		if (!minimalistic) GUILayout.Space(3f);
		if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;

		if (minimalistic)
		{
			if (state) text = "\u25BC" + (char)0x200a + text;
			else text = "\u25BA" + (char)0x200a + text;

			GUILayout.BeginHorizontal();
			GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
			if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();
		}
		else
		{
			text = "<b><size=11>" + text + "</size></b>";
			if (state) text = "\u25BC " + text;
			else text = "\u25BA " + text;
			if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
		}

		if (GUI.changed) EditorPrefs.SetBool(key, state);

		if (!minimalistic) GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state) GUILayout.Space(3f);
		return state;
	}

	/// <summary>
	/// Begin drawing the content area.
	/// </summary>

	static public void BeginContents () { BeginContents(NGUISettings.minimalisticLook); }

	static bool mEndHorizontal = false;

	/// <summary>
	/// Begin drawing the content area.
	/// </summary>

	static public void BeginContents (bool minimalistic)
	{
		if (!minimalistic)
		{
			mEndHorizontal = true;
			GUILayout.BeginHorizontal();
			EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
		}
		else
		{
			mEndHorizontal = false;
			EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
			GUILayout.Space(10f);
		}
		GUILayout.BeginVertical();
		GUILayout.Space(2f);
	}

	/// <summary>
	/// End drawing the content area.
	/// </summary>

	static public void EndContents ()
	{
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (mEndHorizontal)
		{
			GUILayout.Space(3f);
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(3f);
	}

	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>

	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list)
	{
		DrawEvents(text, undoObject, list, null, null, false);
	}

	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>

	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list, bool minimalistic)
	{
		DrawEvents(text, undoObject, list, null, null, minimalistic);
	}

	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>

	static public void DrawEvents (string text, Object undoObject, List<EventDelegate> list, string noTarget, string notValid, bool minimalistic)
	{
		if (!NGUIEditorTools.DrawHeader(text, text, false, minimalistic)) return;

		if (!minimalistic)
		{
			NGUIEditorTools.BeginContents(minimalistic);
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();

			EventDelegateEditor.Field(undoObject, list, notValid, notValid, minimalistic);

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			NGUIEditorTools.EndContents();
		}
		else EventDelegateEditor.Field(undoObject, list, notValid, notValid, minimalistic);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawProperty (this SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(null, serializedObject, property, false, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawProperty (this SerializedObject serializedObject, string property, string label, params GUILayoutOption[] options)
	{
		return DrawProperty(label, serializedObject, property, false, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawProperty (string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(label, serializedObject, property, false, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawPaddedProperty (this SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(null, serializedObject, property, true, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawPaddedProperty (string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
	{
		return DrawProperty(label, serializedObject, property, true, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public SerializedProperty DrawProperty (string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
	{
		SerializedProperty sp = serializedObject.FindProperty(property);

		if (sp != null)
		{
			if (NGUISettings.minimalisticLook) padding = false;

			if (padding) EditorGUILayout.BeginHorizontal();

			if (sp.isArray && sp.type != "string") DrawArray(serializedObject, property, label ?? property);
			else if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
			else EditorGUILayout.PropertyField(sp, options);

			if (padding)
			{
				NGUIEditorTools.DrawPadding();
				EditorGUILayout.EndHorizontal();
			}
		}
		else Debug.LogWarning("Unable to find property " + property);
		return sp;
	}

	/// <summary>
	/// Helper function that draws an array property.
	/// </summary>

	static public void DrawArray (this SerializedObject obj, string property, string title)
	{
		SerializedProperty sp = obj.FindProperty(property + ".Array.size");

		if (sp != null && NGUIEditorTools.DrawHeader(title))
		{
			NGUIEditorTools.BeginContents();
			int size = sp.intValue;
			int newSize = EditorGUILayout.IntField("Size", size);
			if (newSize != size) obj.FindProperty(property + ".Array.size").intValue = newSize;

			EditorGUI.indentLevel = 1;

			for (int i = 0; i < newSize; i++)
			{
				SerializedProperty p = obj.FindProperty(string.Format("{0}.Array.data[{1}]", property, i));
				if (p != null) EditorGUILayout.PropertyField(p);
			}
			EditorGUI.indentLevel = 0;
			NGUIEditorTools.EndContents();
		}
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public void DrawProperty (string label, SerializedProperty sp, params GUILayoutOption[] options)
	{
		DrawProperty(label, sp, true, options);
	}

	/// <summary>
	/// Helper function that draws a serialized property.
	/// </summary>

	static public void DrawProperty (string label, SerializedProperty sp, bool padding, params GUILayoutOption[] options)
	{
		if (sp != null)
		{
			if (padding) EditorGUILayout.BeginHorizontal();

			if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
			else EditorGUILayout.PropertyField(sp, options);

			if (padding)
			{
				NGUIEditorTools.DrawPadding();
				EditorGUILayout.EndHorizontal();
			}
		}
	}

	/// <summary>
	/// Helper function that draws a compact Vector4.
	/// </summary>

	static public void DrawBorderProperty (string name, SerializedObject serializedObject, string field)
	{
		if (serializedObject.FindProperty(field) != null)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(name, GUILayout.Width(75f));

				NGUIEditorTools.SetLabelWidth(50f);
				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("Left", serializedObject, field + ".x", GUILayout.MinWidth(80f));
				NGUIEditorTools.DrawProperty("Bottom", serializedObject, field + ".y", GUILayout.MinWidth(80f));
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("Right", serializedObject, field + ".z", GUILayout.MinWidth(80f));
				NGUIEditorTools.DrawProperty("Top", serializedObject, field + ".w", GUILayout.MinWidth(80f));
				GUILayout.EndVertical();

				NGUIEditorTools.SetLabelWidth(80f);
			}
			GUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Helper function that draws a compact Rect.
	/// </summary>

	static public void DrawRectProperty (string name, SerializedObject serializedObject, string field)
	{
		DrawRectProperty(name, serializedObject, field, 56f, 18f);
	}

	/// <summary>
	/// Helper function that draws a compact Rect.
	/// </summary>

	static public void DrawRectProperty (string name, SerializedObject serializedObject, string field, float labelWidth, float spacing)
	{
		if (serializedObject.FindProperty(field) != null)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(name, GUILayout.Width(labelWidth));

				NGUIEditorTools.SetLabelWidth(20f);
				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("X", serializedObject, field + ".x", GUILayout.MinWidth(50f));
				NGUIEditorTools.DrawProperty("Y", serializedObject, field + ".y", GUILayout.MinWidth(50f));
				GUILayout.EndVertical();

				NGUIEditorTools.SetLabelWidth(50f);
				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("Width", serializedObject, field + ".width", GUILayout.MinWidth(80f));
				NGUIEditorTools.DrawProperty("Height", serializedObject, field + ".height", GUILayout.MinWidth(80f));
				GUILayout.EndVertical();

				NGUIEditorTools.SetLabelWidth(80f);
				if (spacing != 0f) GUILayout.Space(spacing);
			}
			GUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Determine the distance from the mouse position to the world rectangle specified by the 4 points.
	/// </summary>

	static public float SceneViewDistanceToRectangle (Vector3[] worldPoints, Vector2 mousePos)
	{
		Vector2[] screenPoints = new Vector2[4];
		for (int i = 0; i < 4; ++i)
			screenPoints[i] = HandleUtility.WorldToGUIPoint(worldPoints[i]);
		return NGUIMath.DistanceToRectangle(screenPoints, mousePos);
	}

	/// <summary>
	/// Raycast into the specified panel, returning a list of widgets.
	/// Just like NGUIMath.Raycast, but doesn't rely on having a camera.
	/// </summary>

	static public List<UIWidget> SceneViewRaycast (Vector2 mousePos)
	{
		List<UIWidget> list = new List<UIWidget>();

		for (int i = 0; i < UIPanel.list.Count; ++i)
		{
			UIPanel p = UIPanel.list[i];

			for (int b = 0; b < p.widgets.Count; ++b)
			{
				UIWidget w = p.widgets[b];
				if (!w.isVisible) continue;
				Vector3[] corners = w.worldCorners;
				if (SceneViewDistanceToRectangle(corners, mousePos) == 0f)
					list.Add(w);
			}
		}
		list.Sort(UIWidget.FullCompareFunc);
		return list;
	}

	/// <summary>
	/// Select the topmost widget underneath the specified screen coordinate.
	/// </summary>

	static public bool SelectWidget (Vector2 pos) { return SelectWidget(null, pos, true); }

	/// <summary>
	/// Select the next widget in line.
	/// </summary>

	static public bool SelectWidget (GameObject start, Vector2 pos, bool inFront)
	{
		GameObject go = null;
		List<UIWidget> widgets = SceneViewRaycast(pos);
		if (widgets == null || widgets.Count == 0) return false;
		bool found = false;

		if (!inFront)
		{
			if (start != null)
			{
				for (int i = 0; i < widgets.Count; ++i)
				{
					UIWidget w = widgets[i];

					if (w.cachedGameObject == start)
					{
						found = true;
						break;
					}
					go = w.cachedGameObject;
				}
			}
			if (!found) go = widgets[0].cachedGameObject;
		}
		else
		{
			if (start != null)
			{
				for (int i = widgets.Count; i > 0; )
				{
					UIWidget w = widgets[--i];

					if (w.cachedGameObject == start)
					{
						found = true;
						break;
					}
					go = w.cachedGameObject;
				}
			}
			if (!found) go = widgets[widgets.Count - 1].cachedGameObject;
		}

		if (go != null && go != start)
		{
			Selection.activeGameObject = go;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Unity 4.3 changed the way LookLikeControls works.
	/// </summary>

	static public void SetLabelWidth (float width)
	{
		EditorGUIUtility.labelWidth = width;
	}

	/// <summary>
	/// Create an undo point for the specified objects.
	/// </summary>

	static public void RegisterUndo (string name, params Object[] objects)
	{
		if (objects != null && objects.Length > 0)
		{
			UnityEditor.Undo.RecordObjects(objects, name);

			foreach (Object obj in objects)
			{
				if (obj == null) continue;
				EditorUtility.SetDirty(obj);
			}
		}
	}

	/// <summary>
	/// Unity 4.5+ makes it possible to hide the move tool.
	/// </summary>

	static public void HideMoveTool (bool hide)
	{
#if !UNITY_4_3
		UnityEditor.Tools.hidden = hide &&
 #if !UNITY_4_5
			(UnityEditor.Tools.current == UnityEditor.Tool.Rect) &&
 #else
			(UnityEditor.Tools.current == UnityEditor.Tool.Move) &&
 #endif
			UIWidget.showHandlesWithMoveTool && !NGUISettings.showTransformHandles;
#endif
	}

	/// <summary>
	/// Gets the internal class ID of the specified type.
	/// </summary>

	static public int GetClassID (System.Type type)
	{
		GameObject go = EditorUtility.CreateGameObjectWithHideFlags("Temp", HideFlags.HideAndDontSave);
		Component uiSprite = go.AddComponent(type);
		SerializedObject ob = new SerializedObject(uiSprite);
		int classID = ob.FindProperty("m_Script").objectReferenceInstanceIDValue;
		NGUITools.DestroyImmediate(go);
		return classID;
	}

	/// <summary>
	/// Gets the internal class ID of the specified type.
	/// </summary>

	static public int GetClassID<T> () where T : MonoBehaviour { return GetClassID(typeof(T)); }

	/// <summary>
	/// Convenience function that replaces the specified MonoBehaviour with one of specified type.
	/// </summary>

	static public SerializedObject ReplaceClass (MonoBehaviour mb, System.Type type)
	{
		int id = GetClassID(type);
		SerializedObject ob = new SerializedObject(mb);
		ob.Update();
		ob.FindProperty("m_Script").objectReferenceInstanceIDValue = id;
		ob.ApplyModifiedProperties();
		ob.Update();
		return ob;
	}

	/// <summary>
	/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
	/// </summary>

	static public SerializedObject ReplaceClass (MonoBehaviour mb, int classID)
	{
		SerializedObject ob = new SerializedObject(mb);
		ob.Update();
		ob.FindProperty("m_Script").objectReferenceInstanceIDValue = classID;
		ob.ApplyModifiedProperties();
		ob.Update();
		return ob;
	}

	/// <summary>
	/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
	/// </summary>

	static public void ReplaceClass (SerializedObject ob, int classID)
	{
		ob.FindProperty("m_Script").objectReferenceInstanceIDValue = classID;
		ob.ApplyModifiedProperties();
		ob.Update();
	}

	/// <summary>
	/// Convenience function that replaces the specified MonoBehaviour with one of specified class ID.
	/// </summary>

	static public void ReplaceClass (SerializedObject ob, System.Type type)
	{
		ob.FindProperty("m_Script").objectReferenceInstanceIDValue = GetClassID(type);
		ob.ApplyModifiedProperties();
		ob.Update();
	}

	/// <summary>
	/// Convenience function that replaces the specified MonoBehaviour with one of specified type.
	/// </summary>

	static public T ReplaceClass<T> (MonoBehaviour mb) where T : MonoBehaviour { return ReplaceClass(mb, typeof(T)).targetObject as T; }

	/// <summary>
	/// Automatically upgrade all of the UITextures in the scene to Sprites if they can be found within the specified atlas.
	/// </summary>

	static public void UpgradeTexturesToSprites (UIAtlas atlas)
	{
		if (atlas == null) return;
		List<UITexture> uits = FindAll<UITexture>();

		if (uits.Count > 0)
		{
			UIWidget selectedTex = (UIWidgetInspector.instance != null && UIWidgetInspector.instance.target != null) ?
				UIWidgetInspector.instance.target as UITexture : null;

			// Determine the object instance ID of the UISprite class
			int spriteID = GetClassID<UISprite>();

			// Run through all the UI textures and change them to sprites
			for (int i = 0; i < uits.Count; ++i)
			{
				UIWidget uiTexture = uits[i];

				if (uiTexture != null && uiTexture.mainTexture != null)
				{
					UISpriteData atlasSprite = atlas.GetSprite(uiTexture.mainTexture.name);

					if (atlasSprite != null)
					{
						SerializedObject ob = ReplaceClass(uiTexture, spriteID);
						ob.FindProperty("mSpriteName").stringValue = uiTexture.mainTexture.name;
						ob.FindProperty("mAtlas").objectReferenceValue = NGUISettings.atlas;
						ob.ApplyModifiedProperties();
					}
				}
			}

			if (selectedTex != null)
			{
				// Repaint() doesn't work in this case because Unity doesn't realize that the underlying
				// script type has changed and that a new editor script needs to be chosen.
				//UIWidgetInspector.instance.Repaint();
				Selection.activeGameObject = null;
			}
		}
	}

	class MenuEntry
	{
		public string name;
		public GameObject go;
		public MenuEntry (string name, GameObject go) { this.name = name; this.go = go; }
	}

	/// <summary>
	/// Show a sprite selection context menu listing all sprites under the specified screen position.
	/// </summary>

	static public void ShowSpriteSelectionMenu (Vector2 screenPos)
	{
		List<UIWidget> widgets = NGUIEditorTools.SceneViewRaycast(screenPos);
		List<UIWidgetContainer> containers = new List<UIWidgetContainer>();
		List<MenuEntry> entries = new List<MenuEntry>();
		List<UIPanel> panels = new List<UIPanel>();

		bool divider = false;
		UIWidget topWidget = null;
		UIPanel topPanel = null;

		// Process widgets and their containers in the raycast order
		for (int i = 0; i < widgets.Count; ++i)
		{
			UIWidget w = widgets[i];
			if (topWidget == null) topWidget = w;

			UIPanel panel = w.panel;
			if (topPanel == null) topPanel = panel;

			if (panel != null && !panels.Contains(panel))
			{
				panels.Add(panel);

				if (!divider)
				{
					entries.Add(null);
					divider = true;
				}
				entries.Add(new MenuEntry(panel.name + " (panel)", panel.gameObject));
			}

			UIWidgetContainer wc = NGUITools.FindInParents<UIWidgetContainer>(w.cachedGameObject);

			// If we get a new container, we should add it to the list
			if (wc != null && !containers.Contains(wc))
			{
				containers.Add(wc);

				// Only proceed if there is no widget on the container
				if (wc.gameObject != w.cachedGameObject)
				{
					if (!divider)
					{
						entries.Add(null);
						divider = true;
					}
					entries.Add(new MenuEntry(wc.name + " (container)", wc.gameObject));
				}
			}

			string name = (i + 1 == widgets.Count) ? (w.name + " (top-most)") : w.name;
			entries.Add(new MenuEntry(name, w.gameObject));
			divider = false;
		}

		// Common items used by NGUI
		NGUIContextMenu.AddCommonItems(Selection.activeGameObject);

		// Add widgets to the menu in the reverse order so that they are shown with the top-most widget first (on top)
		for (int i = entries.Count; i > 0; )
		{
			MenuEntry ent = entries[--i];

			if (ent != null)
			{
				NGUIContextMenu.AddItem("Select/" + ent.name, Selection.activeGameObject == ent.go,
					delegate(object go) { Selection.activeGameObject = (GameObject)go; }, ent.go);
			}
			else if (!divider)
			{
				NGUIContextMenu.AddSeparator("Select/");
			}
		}
		NGUIContextMenu.AddHelp(Selection.activeGameObject, true);
		NGUIContextMenu.Show();
	}
	/// <summary>
	/// Load the asset at the specified path.
	/// </summary>

	static public Object LoadAsset (string path)
	{
		if (string.IsNullOrEmpty(path)) return null;
		return AssetDatabase.LoadMainAssetAtPath(path);
	}

	/// <summary>
	/// Convenience function to load an asset of specified type, given the full path to it.
	/// </summary>

	static public T LoadAsset<T> (string path) where T: Object
	{
		Object obj = LoadAsset(path);
		if (obj == null) return null;

		T val = obj as T;
		if (val != null) return val;

		if (typeof(T).IsSubclassOf(typeof(Component)))
		{
			if (obj.GetType() == typeof(GameObject))
			{
				GameObject go = obj as GameObject;
				return go.GetComponent(typeof(T)) as T;
			}
		}
		return null;
	}

	/// <summary>
	/// Get the specified object's GUID.
	/// </summary>

	static public string ObjectToGUID (Object obj)
	{
		string path = AssetDatabase.GetAssetPath(obj);
		return (!string.IsNullOrEmpty(path)) ? AssetDatabase.AssetPathToGUID(path) : null;
	}

	static MethodInfo s_GetInstanceIDFromGUID;

	/// <summary>
	/// Convert the specified GUID to an object reference.
	/// </summary>

	static public Object GUIDToObject (string guid)
	{
		if (string.IsNullOrEmpty(guid)) return null;
		
		if (s_GetInstanceIDFromGUID == null)
			s_GetInstanceIDFromGUID = typeof(AssetDatabase).GetMethod("GetInstanceIDFromGUID", BindingFlags.Static | BindingFlags.NonPublic);

		int id = (int)s_GetInstanceIDFromGUID.Invoke(null, new object[] { guid });
		if (id != 0) return EditorUtility.InstanceIDToObject(id);
		string path = AssetDatabase.GUIDToAssetPath(guid);
		if (string.IsNullOrEmpty(path)) return null;
		return AssetDatabase.LoadAssetAtPath(path, typeof(Object));
	}

	/// <summary>
	/// Convert the specified GUID to an object reference of specified type.
	/// </summary>

	static public T GUIDToObject<T> (string guid) where T : Object
	{
		Object obj = GUIDToObject(guid);
		if (obj == null) return null;

		System.Type objType = obj.GetType();
		if (objType == typeof(T) || objType.IsSubclassOf(typeof(T))) return obj as T;

		if (objType == typeof(GameObject) && typeof(T).IsSubclassOf(typeof(Component)))
		{
			GameObject go = obj as GameObject;
			return go.GetComponent(typeof(T)) as T;
		}
		return null;
	}

	/// <summary>
	/// Add a border around the specified color buffer with the width and height of a single pixel all around.
	/// The returned color buffer will have its width and height increased by 2.
	/// </summary>

	static public Color32[] AddBorder (Color32[] colors, int width, int height)
	{
		int w2 = width + 2;
		int h2 = height + 2;

		Color32[] c2 = new Color32[w2 * h2];

		for (int y2 = 0; y2 < h2; ++y2)
		{
			int y1 = NGUIMath.ClampIndex(y2 - 1, height);

			for (int x2 = 0; x2 < w2; ++x2)
			{
				int x1 = NGUIMath.ClampIndex(x2 - 1, width);
				int i2 = x2 + y2 * w2;
				c2[i2] = colors[x1 + y1 * width];

				if (x2 == 0 || x2 + 1 == w2 || y2 == 0 || y2 + 1 == h2)
					c2[i2].a = 0;
			}
		}
		return c2;
	}

	/// <summary>
	/// Add a soft shadow to the specified color buffer.
	/// The buffer must have some padding around the edges in order for this to work properly.
	/// </summary>

	static public void AddShadow (Color32[] colors, int width, int height, Color shadow)
	{
		Color sh = shadow;
		sh.a = 1f;

		for (int y2 = 0; y2 < height; ++y2)
		{
			for (int x2 = 0; x2 < width; ++x2)
			{
				int index = x2 + y2 * width;
				Color32 uc = colors[index];
				if (uc.a == 255) continue;

				Color original = uc;
				float val = original.a;
				int count = 1;
				float div1 = 1f / 255f;
				float div2 = 2f / 255f;
				float div3 = 3f / 255f;

				// Left
				if (x2 != 0)
				{
					val += colors[x2 - 1 + y2 * width].a * div1;
					count += 1;
				}

				// Top
				if (y2 + 1 != height)
				{
					val += colors[x2 + (y2 + 1) * width].a * div2;
					count += 2;
				}

				// Top-left
				if (x2 != 0 && y2 + 1 != height)
				{
					val += colors[x2 - 1 + (y2 + 1) * width].a * div3;
					count += 3;
				}

				val /= count;

				Color c = Color.Lerp(original, sh, shadow.a * val);
				colors[index] = Color.Lerp(c, original, original.a);
			}
		}
	}

	/// <summary>
	/// Add a visual depth effect to the specified color buffer.
	/// The buffer must have some padding around the edges in order for this to work properly.
	/// </summary>

	static public void AddDepth (Color32[] colors, int width, int height, Color shadow)
	{
		Color sh = shadow;
		sh.a = 1f;

		for (int y2 = 0; y2 < height; ++y2)
		{
			for (int x2 = 0; x2 < width; ++x2)
			{
				int index = x2 + y2 * width;
				Color32 uc = colors[index];
				if (uc.a == 255) continue;

				Color original = uc;
				float val = original.a * 4f;
				int count = 4;
				float div1 = 1f / 255f;
				float div2 = 2f / 255f;

				if (x2 != 0)
				{
					val += colors[x2 - 1 + y2 * width].a * div2;
					count += 2;
				}

				if (x2 + 1 != width)
				{
					val += colors[x2 + 1 + y2 * width].a * div2;
					count += 2;
				}

				if (y2 != 0)
				{
					val += colors[x2 + (y2 - 1) * width].a * div2;
					count += 2;
				}

				if (y2 + 1 != height)
				{
					val += colors[x2 + (y2 + 1) * width].a * div2;
					count += 2;
				}

				if (x2 != 0 && y2 != 0)
				{
					val += colors[x2 - 1 + (y2 - 1) * width].a * div1;
					++count;
				}

				if (x2 != 0 && y2 + 1 != height)
				{
					val += colors[x2 - 1 + (y2 + 1) * width].a * div1;
					++count;
				}

				if (x2 + 1 != width && y2 != 0)
				{
					val += colors[x2 + 1 + (y2 - 1) * width].a * div1;
					++count;
				}

				if (x2 + 1 != width && y2 + 1 != height)
				{
					val += colors[x2 + 1 + (y2 + 1) * width].a * div1;
					++count;
				}

				val /= count;

				Color c = Color.Lerp(original, sh, shadow.a * val);
				colors[index] = Color.Lerp(c, original, original.a);
			}
		}
	}

	/// <summary>
	/// Draw 18 pixel padding on the right-hand side. Used to align fields.
	/// </summary>

	static public void DrawPadding ()
	{
		if (!NGUISettings.minimalisticLook)
			GUILayout.Space(18f);
	}

	static System.Collections.Generic.Dictionary<string, TextureImporterType> mOriginal = new Dictionary<string, TextureImporterType>();

	/// <summary>
	/// Force the texture to be readable. Returns the asset database path to the texture.
	/// </summary>

	static public string MakeReadable (this Texture2D tex, bool readable = true)
	{
		string path = AssetDatabase.GetAssetPath(tex);

		if (!string.IsNullOrEmpty(path))
		{
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

			if (textureImporter != null && textureImporter.isReadable != readable)
			{
				textureImporter.isReadable = readable;

				if (readable)
				{
					mOriginal[path] = textureImporter.textureType;
#if UNITY_5_5_OR_NEWER
					textureImporter.textureType = TextureImporterType.Default;
#else
					textureImporter.textureType = TextureImporterType.Image;
#endif
				}
				else
				{
					TextureImporterType type;

					if (mOriginal.TryGetValue(path, out type))
					{
						textureImporter.textureType = type;
						mOriginal.Remove(path);
					}
				}
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			}
		}
		return path;
	}
}
