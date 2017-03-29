//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UITextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UI2DSprite), true)]
public class UI2DSpriteEditor : UIBasicSpriteEditor
{
	UI2DSprite mSprite;

	protected override void OnEnable ()
	{
		base.OnEnable();
		mSprite = target as UI2DSprite;
	}

	/// <summary>
	/// Should we draw the widget's custom properties?
	/// </summary>

	protected override bool ShouldDrawProperties ()
	{
		GUI.changed = false;
		SerializedProperty sp = NGUIEditorTools.DrawProperty("2D Sprite", serializedObject, "mSprite");

#if !UNITY_4_3
		if (GUI.changed)
		{
			UnityEngine.Sprite sprite = sp.objectReferenceValue as Sprite;

			if (sprite != null)
			{
				SerializedProperty border = serializedObject.FindProperty("mBorder");
				border.vector4Value = sprite.border;
			}
		}
#endif
		NGUISettings.sprite2D = sp.objectReferenceValue as Sprite;

		NGUIEditorTools.DrawProperty("Material", serializedObject, "mMat");

		if (mSprite.material == null || serializedObject.isEditingMultipleObjects)
		{
			NGUIEditorTools.DrawProperty("Shader", serializedObject, "mShader");
		}

		NGUIEditorTools.DrawProperty("Pixel Size", serializedObject, "mPixelSize");

		SerializedProperty fa = serializedObject.FindProperty("mFixedAspect");
		bool before = fa.boolValue;
		NGUIEditorTools.DrawProperty("Fixed Aspect", fa);
		if (fa.boolValue != before) (target as UIWidget).drawRegion = new Vector4(0f, 0f, 1f, 1f);

		if (fa.boolValue)
		{
			EditorGUILayout.HelpBox("Note that Fixed Aspect mode is not compatible with Draw Region modifications done by sliders and progress bars.", MessageType.Info);
		}
		return (sp.objectReferenceValue != null);
	}

	/// <summary>
	/// Allow the texture to be previewed.
	/// </summary>

	public override bool HasPreviewGUI ()
	{
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1) &&
			(mSprite != null) && (mSprite.mainTexture as Texture2D != null);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>

	public override void OnPreviewGUI (Rect rect, GUIStyle background)
	{
		if (mSprite != null && mSprite.sprite2D != null)
		{
			Texture2D tex = mSprite.mainTexture as Texture2D;
			if (tex != null) NGUIEditorTools.DrawSprite(tex, rect, mSprite.color, mSprite.sprite2D.textureRect, mSprite.border);
		}
	}
}
