//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.IO;

/// <summary>
/// This class contains NGUI's extensions to Unity Editor's functionality.
/// </summary>

static public class NGUIEditorExtensions
{
	/// <summary>
	/// Render the camera into a render texture. If the camera has a render texture assigned, it will be re-used.
	/// If it doesn't, a new render texture will be created.
	/// </summary>

	static public RenderTexture RenderToTexture (this Camera cam, int width, int height)
	{
		// Render textures only work in Unity Pro
		if (!UnityEditorInternal.InternalEditorUtility.HasPro()) return null;

		RenderTexture rt = cam.targetTexture;

		if (rt != null && (rt.width != width || rt.height != height))
			NGUITools.DestroyImmediate(rt);

		// Set up the render texture for the camera
		if (rt == null)
		{
			rt = new RenderTexture(width, height, 1);
			rt.hideFlags = HideFlags.HideAndDontSave;
#if UNITY_5_5_OR_NEWER
			rt.autoGenerateMips = false;
#else
			rt.generateMips = false;
#endif
			rt.format = RenderTextureFormat.ARGB32;
			rt.filterMode = FilterMode.Trilinear;
			rt.anisoLevel = 4;
		}
		cam.targetTexture = rt;
		if (rt != null) cam.Render();
		return rt;
	}

	/// <summary>
	/// If the camera has a render texture, save its contents into the specified file using PNG image format.
	/// </summary>

	static public bool SaveRenderTextureAsPNG (this Camera cam, string filename)
	{
		// Render textures only work in Unity Pro
		if (!UnityEditorInternal.InternalEditorUtility.HasPro()) return false;

		RenderTexture rt = cam.targetTexture;
		if (rt == null) return false;

		// Read the render texture's contents into the 2D texture
		RenderTexture.active = rt;
		Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
		tex.hideFlags = HideFlags.HideAndDontSave;
		tex.ReadPixels(new Rect(0f, 0f, rt.width, rt.height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;

		try
		{
			// Save the contents into the specified PNG
			byte[] bytes = tex.EncodeToPNG();
			FileStream fs = File.OpenWrite(filename);
			fs.Write(bytes, 0, bytes.Length);
			fs.Close();
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.Message);
			return false;
		}
		finally
		{
			NGUITools.DestroyImmediate(tex);
		}
		return true;
	}
}

#if !UNITY_4_3 && !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
// Unity 5 bug fix. Source: http://www.tasharen.com/forum/index.php?topic=13231.0
internal class Unity5DynamicLabelWorkAround : UnityEditor.AssetModificationProcessor
{
	static string[] OnWillSaveAssets (string[] paths)
	{
		// Older versions: UnityEditor.EditorApplication.currentScene
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		string current = UnityEditor.EditorApplication.currentScene;
#else
		string current = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
#endif
		foreach (var path in paths)
		{
			if (path == current)
			{
				UILabel[] labels = Object.FindObjectsOfType<UILabel>();
				for (int i = 0, imax = labels.Length; i < imax; ++i) labels[i].MarkAsChanged();
			}
		}
		return paths;
	}
}
#endif
