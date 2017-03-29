//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

/// <summary>
/// This script adds the NGUI menu options to the Unity Editor.
/// </summary>

static public class NGUIMenu
{
#region Selection

	static public GameObject SelectedRoot () { return NGUIEditorTools.SelectedRoot(); }

	[MenuItem("NGUI/Selection/Bring To Front &#=", false, 0)]
	static public void BringForward2 ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1000);

		if ((val & 1) != 0)
		{
			NGUITools.NormalizePanelDepths();
			if (UIPanelTool.instance != null)
				UIPanelTool.instance.Repaint();
		}
		if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
	}

	[MenuItem("NGUI/Selection/Bring To Front &#=", true)]
	static public bool BringForward2Validation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Push To Back &#-", false, 0)]
	static public void PushBack2 ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1000);

		if ((val & 1) != 0)
		{
			NGUITools.NormalizePanelDepths();
			if (UIPanelTool.instance != null)
				UIPanelTool.instance.Repaint();
		}
		if ((val & 2) != 0) NGUITools.NormalizeWidgetDepths();
	}

	[MenuItem("NGUI/Selection/Push To Back &#-", true)]
	static public bool PushBack2Validation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Adjust Depth By +1 %=", false, 0)]
	static public void BringForward ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], 1);
		if (((val & 1) != 0) && UIPanelTool.instance != null)
			UIPanelTool.instance.Repaint();
	}

	[MenuItem("NGUI/Selection/Adjust Depth By +1 %=", true)]
	static public bool BringForwardValidation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Adjust Depth By -1 %-", false, 0)]
	static public void PushBack ()
	{
		int val = 0;
		for (int i = 0; i < Selection.gameObjects.Length; ++i)
			val |= NGUITools.AdjustDepth(Selection.gameObjects[i], -1);
		if (((val & 1) != 0) && UIPanelTool.instance != null)
			UIPanelTool.instance.Repaint();
	}

	[MenuItem("NGUI/Selection/Adjust Depth By -1 %-", true)]
	static public bool PushBackValidation () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Selection/Make Pixel Perfect &#p", false, 0)]
	static void PixelPerfectSelection ()
	{
		foreach (Transform t in Selection.transforms)
			NGUITools.MakePixelPerfect(t);
	}

	[MenuItem("NGUI/Selection/Make Pixel Perfect &#p", true)]
	static bool PixelPerfectSelectionValidation ()
	{
		return (Selection.activeTransform != null);
	}

#endregion
#region Create

	[MenuItem("NGUI/Create/Sprite &#s", false, 6)]
	static public void AddSprite ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			Selection.activeGameObject = NGUISettings.AddSprite(go).gameObject;
		}
		else Debug.Log("You must select a game object first.");
	}

	[MenuItem("NGUI/Create/Label &#l", false, 6)]
	static public void AddLabel ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			Selection.activeGameObject = NGUISettings.AddLabel(go).gameObject;
		}
		else Debug.Log("You must select a game object first.");
	}

	[MenuItem("NGUI/Create/Texture &#t", false, 6)]
	static public void AddTexture ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			Selection.activeGameObject = NGUISettings.AddTexture(go).gameObject;
		}
		else Debug.Log("You must select a game object first.");
	}

	[MenuItem("NGUI/Create/Unity 2D Sprite &#d", false, 6)]
	static public void AddSprite2D ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);
		if (go != null) Selection.activeGameObject = NGUISettings.Add2DSprite(go).gameObject;
		else Debug.Log("You must select a game object first.");
	}

	[MenuItem("NGUI/Create/Widget &#w", false, 6)]
	static public void AddWidget ()
	{
		GameObject go = NGUIEditorTools.SelectedRoot(true);

		if (go != null)
		{
			Selection.activeGameObject = NGUISettings.AddWidget(go).gameObject;
		}
		else Debug.Log("You must select a game object first.");
	}

	[MenuItem("NGUI/Create/", false, 6)]
	static void AddBreaker123 () {}

	[MenuItem("NGUI/Create/Anchor (Legacy)", false, 6)]
	static void AddAnchor2 () { Add<UIAnchor>(); }

	[MenuItem("NGUI/Create/Panel", false, 6)]
	static void AddPanel ()
	{
		UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
		Selection.activeGameObject = (panel == null) ? NGUIEditorTools.SelectedRoot(true) : panel.gameObject;
	}

	[MenuItem("NGUI/Create/Scroll View", false, 6)]
	static void AddScrollView ()
	{
		UIPanel panel = NGUISettings.AddPanel(SelectedRoot());
		if (panel == null) panel = NGUIEditorTools.SelectedRoot(true).GetComponent<UIPanel>();
		panel.clipping = UIDrawCall.Clipping.SoftClip;
		panel.name = "Scroll View";
		panel.gameObject.AddComponent<UIScrollView>();
		Selection.activeGameObject = panel.gameObject;
	}

	[MenuItem("NGUI/Create/Grid", false, 6)]
	static void AddGrid () { Add<UIGrid>(); }

	[MenuItem("NGUI/Create/Table", false, 6)]
	static void AddTable () { Add<UITable>(); }

	static T Add<T> () where T : MonoBehaviour
	{
		T t = NGUITools.AddChild<T>(SelectedRoot());
		Selection.activeGameObject = t.gameObject;
		return t;
	}

	[MenuItem("NGUI/Create/2D UI", false, 6)]
	[MenuItem("Assets/NGUI/Create 2D UI", false, 1)]
	static void Create2D () { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Simple2D); }

	[MenuItem("NGUI/Create/2D UI", true)]
	[MenuItem("Assets/NGUI/Create 2D UI", true, 1)]
	static bool Create2Da ()
	{
		if (UIRoot.list.Count == 0 || UICamera.list.size == 0) return true;
		foreach (UICamera c in UICamera.list)
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (NGUITools.GetActive(c) && c.camera.isOrthoGraphic)
#else
			if (NGUITools.GetActive(c) && c.GetComponent<Camera>().orthographic)
#endif
				return false;
		return true;
	}

	[MenuItem("NGUI/Create/3D UI", false, 6)]
	[MenuItem("Assets/NGUI/Create 3D UI", false, 1)]
	static void Create3D () { UICreateNewUIWizard.CreateNewUI(UICreateNewUIWizard.CameraType.Advanced3D); }

	[MenuItem("NGUI/Create/3D UI", true)]
	[MenuItem("Assets/NGUI/Create 3D UI", true, 1)]
	static bool Create3Da ()
	{
		if (UIRoot.list.Count == 0 || UICamera.list.size == 0) return true;
		foreach (UICamera c in UICamera.list)
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (NGUITools.GetActive(c) && !c.camera.isOrthoGraphic)
#else
			if (NGUITools.GetActive(c) && !c.GetComponent<Camera>().orthographic)
#endif
				return false;
		return true;
	}

#endregion
#region Attach

	static void AddIfMissing<T> () where T : Component
	{
		if (Selection.activeGameObject != null)
		{
			for (int i = 0; i < Selection.gameObjects.Length; ++i)
				Selection.gameObjects[i].AddMissingComponent<T>();
		}
		else Debug.Log("You must select a game object first.");
	}

	static bool Exists<T> () where T : Component
	{
		GameObject go = Selection.activeGameObject;
		if (go != null) return go.GetComponent<T>() != null;
		return false;
	}

	[MenuItem("NGUI/Attach/Collider &#c", false, 7)]
	static public void AddCollider ()
	{
		if (Selection.activeGameObject != null)
		{
			for (int i = 0; i < Selection.gameObjects.Length; ++i)
				NGUITools.AddWidgetCollider(Selection.gameObjects[i]);
		}
		else Debug.Log("You must select a game object first, such as your button.");
	}

	//[MenuItem("NGUI/Attach/Anchor", false, 7)]
	//static public void Add1 () { AddIfMissing<UIAnchor>(); }

	//[MenuItem("NGUI/Attach/Anchor", true)]
	//static public bool Add1a () { return !Exists<UIAnchor>(); }

	//[MenuItem("NGUI/Attach/Stretch (Legacy)", false, 7)]
	//static public void Add2 () { AddIfMissing<UIStretch>(); }

	//[MenuItem("NGUI/Attach/Stretch (Legacy)", true)]
	//static public bool Add2a () { return !Exists<UIStretch>(); }

	//[MenuItem("NGUI/Attach/", false, 7)]
	//static public void Add3s () {}

	[MenuItem("NGUI/Attach/Button Script", false, 7)]
	static public void Add3 () { AddIfMissing<UIButton>(); }

	[MenuItem("NGUI/Attach/Toggle Script", false, 7)]
	static public void Add4 () { AddIfMissing<UIToggle>(); }

	[MenuItem("NGUI/Attach/Slider Script", false, 7)]
	static public void Add5 () { AddIfMissing<UISlider>(); }

	[MenuItem("NGUI/Attach/Scroll Bar Script", false, 7)]
	static public void Add6 () { AddIfMissing<UIScrollBar>(); }

	[MenuItem("NGUI/Attach/Progress Bar Script", false, 7)]
	static public void Add7 () { AddIfMissing<UIProgressBar>(); }

	[MenuItem("NGUI/Attach/Popup List Script", false, 7)]
	static public void Add8 () { AddIfMissing<UIPopupList>(); }

	[MenuItem("NGUI/Attach/Input Field Script", false, 7)]
	static public void Add9 () { AddIfMissing<UIInput>(); }

	[MenuItem("NGUI/Attach/Key Binding Script", false, 7)]
	static public void Add10 () { AddIfMissing<UIKeyBinding>(); }

	[MenuItem("NGUI/Attach/Key Navigation Script", false, 7)]
	static public void Add10a () { AddIfMissing<UIKeyNavigation>(); }

	[MenuItem("NGUI/Attach/Play Tween Script", false, 7)]
	static public void Add11 () { AddIfMissing<UIPlayTween>(); }

	[MenuItem("NGUI/Attach/Play Animation Script", false, 7)]
	static public void Add12 () { AddIfMissing<UIPlayAnimation>(); }

	[MenuItem("NGUI/Attach/Play Sound Script", false, 7)]
	static public void Add13 () { AddIfMissing<UIPlaySound>(); }

	[MenuItem("NGUI/Attach/Localization Script", false, 7)]
	static public void Add14 () { AddIfMissing<UILocalize>(); }

#endregion
#region Tweens

	[MenuItem("NGUI/Tween/Alpha", false, 8)]
	static void Tween1 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenAlpha>(); }

	[MenuItem("NGUI/Tween/Alpha", true)]
	static bool Tween1a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Color", false, 8)]
	static void Tween2 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenColor>(); }

	[MenuItem("NGUI/Tween/Color", true)]
	static bool Tween2a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Width", false, 8)]
	static void Tween3 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenWidth>(); }

	[MenuItem("NGUI/Tween/Width", true)]
	static bool Tween3a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Height", false, 8)]
	static void Tween4 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenHeight>(); }

	[MenuItem("NGUI/Tween/Height", true)]
	static bool Tween4a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<UIWidget>() != null); }

	[MenuItem("NGUI/Tween/Position", false, 8)]
	static void Tween5 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenPosition>(); }

	[MenuItem("NGUI/Tween/Position", true)]
	static bool Tween5a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Rotation", false, 8)]
	static void Tween6 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenRotation>(); }

	[MenuItem("NGUI/Tween/Rotation", true)]
	static bool Tween6a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Scale", false, 8)]
	static void Tween7 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenScale>(); }

	[MenuItem("NGUI/Tween/Scale", true)]
	static bool Tween7a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Transform", false, 8)]
	static void Tween8 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenTransform>(); }

	[MenuItem("NGUI/Tween/Transform", true)]
	static bool Tween8a () { return (Selection.activeGameObject != null); }

	[MenuItem("NGUI/Tween/Volume", false, 8)]
	static void Tween9 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenVolume>(); }

	[MenuItem("NGUI/Tween/Volume", true)]
	static bool Tween9a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<AudioSource>() != null); }

	[MenuItem("NGUI/Tween/Field of View", false, 8)]
	static void Tween10 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenFOV>(); }

	[MenuItem("NGUI/Tween/Field of View", true)]
	static bool Tween10a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

	[MenuItem("NGUI/Tween/Orthographic Size", false, 8)]
	static void Tween11 () { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenOrthoSize>(); }

	[MenuItem("NGUI/Tween/Orthographic Size", true)]
	static bool Tween11a () { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Camera>() != null); }

#endregion
#region Open

	[MenuItem("NGUI/Open/Atlas Maker", false, 9)]
	[MenuItem("Assets/NGUI/Open Atlas Maker", false, 0)]
	static public void OpenAtlasMaker ()
	{
		EditorWindow.GetWindow<UIAtlasMaker>(false, "Atlas Maker", true).Show();
	}

	[MenuItem("NGUI/Open/Font Maker", false, 9)]
	[MenuItem("Assets/NGUI/Open Bitmap Font Maker", false, 0)]
	static public void OpenFontMaker ()
	{
		EditorWindow.GetWindow<UIFontMaker>(false, "Font Maker", true).Show();
	}

	[MenuItem("NGUI/Open/", false, 9)]
	[MenuItem("Assets/NGUI/", false, 0)]
	static public void OpenSeparator2 () { }

	[MenuItem("NGUI/Open/Prefab Toolbar", false, 9)]
	static public void OpenPrefabTool ()
	{
		EditorWindow.GetWindow<UIPrefabTool>(false, "Prefab Toolbar", true).Show();
	}

	[MenuItem("NGUI/Open/Panel Tool", false, 9)]
	static public void OpenPanelWizard ()
	{
		EditorWindow.GetWindow<UIPanelTool>(false, "Panel Tool", true).Show();
	}

	[MenuItem("NGUI/Open/Draw Call Tool", false, 9)]
	static public void OpenDCTool ()
	{
		EditorWindow.GetWindow<UIDrawCallViewer>(false, "Draw Call Tool", true).Show();
	}

	[MenuItem("NGUI/Open/Camera Tool", false, 9)]
	static public void OpenCameraWizard ()
	{
		EditorWindow.GetWindow<UICameraTool>(false, "Camera Tool", true).Show();
	}

	[MenuItem("NGUI/Open/Widget Wizard (Legacy)", false, 9)]
	static public void CreateWidgetWizard ()
	{
		EditorWindow.GetWindow<UICreateWidgetWizard>(false, "Widget Tool", true).Show();
	}

	//[MenuItem("NGUI/Open/UI Wizard (Legacy)", false, 9)]
	//static public void CreateUIWizard ()
	//{
	//    EditorWindow.GetWindow<UICreateNewUIWizard>(false, "UI Tool", true).Show();
	//}

#endregion
#region Options

	[MenuItem("NGUI/Options/Transform Move Gizmo/Turn On", false, 10)]
	static public void TurnGizmosOn ()
	{
		NGUISettings.showTransformHandles = true;
		NGUIEditorTools.HideMoveTool(false);
	}

	[MenuItem("NGUI/Options/Transform Move Gizmo/Turn On", true, 10)]
	static public bool TurnGizmosOnCheck () { return !NGUISettings.showTransformHandles; }

	[MenuItem("NGUI/Options/Transform Move Gizmo/Turn Off", false, 10)]
	static public void TurnGizmosOff () { NGUISettings.showTransformHandles = false; }

	[MenuItem("NGUI/Options/Transform Move Gizmo/Turn Off", true, 10)]
	static public bool TurnGizmosOffCheck () { return NGUISettings.showTransformHandles; }

	[MenuItem("NGUI/Options/Handles/Turn On", false, 10)]
	static public void TurnHandlesOn () { UIWidget.showHandlesWithMoveTool = true; }

	[MenuItem("NGUI/Options/Handles/Turn On", true, 10)]
	static public bool TurnHandlesOnCheck () { return !UIWidget.showHandlesWithMoveTool; }

	[MenuItem("NGUI/Options/Handles/Turn Off", false, 10)]
	static public void TurnHandlesOff () { UIWidget.showHandlesWithMoveTool = false; }

	[MenuItem("NGUI/Options/Handles/Turn Off", true, 10)]
	static public bool TurnHandlesOffCheck () { return UIWidget.showHandlesWithMoveTool; }

	[MenuItem("NGUI/Options/Handles/Set to Blue", false, 10)]
	static public void SetToBlue () { NGUISettings.colorMode = NGUISettings.ColorMode.Blue; }

	[MenuItem("NGUI/Options/Handles/Set to Blue", true, 10)]
	static public bool SetToBlueCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Blue; }

	[MenuItem("NGUI/Options/Handles/Set to Orange", false, 10)]
	static public void SetToOrange () { NGUISettings.colorMode = NGUISettings.ColorMode.Orange; }

	[MenuItem("NGUI/Options/Handles/Set to Orange", true, 10)]
	static public bool SetToOrangeCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Orange; }

	[MenuItem("NGUI/Options/Handles/Set to Green", false, 10)]
	static public void SetToGreen () { NGUISettings.colorMode = NGUISettings.ColorMode.Green; }

	[MenuItem("NGUI/Options/Handles/Set to Green", true, 10)]
	static public bool SetToGreenCheck () { return UIWidget.showHandlesWithMoveTool && NGUISettings.colorMode != NGUISettings.ColorMode.Green; }

	[MenuItem("NGUI/Options/Inspector Look/Set to Minimalistic", false, 10)]
	static public void SetToMin ()
	{
		NGUISettings.minimalisticLook = true;
		if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
	}

	[MenuItem("NGUI/Options/Inspector Look/Set to Minimalistic", true, 10)]
	static public bool SetToMinCheck () { return !NGUISettings.minimalisticLook; }

	[MenuItem("NGUI/Options/Inspector Look/Set to Distinct", false, 10)]
	static public void SetToDistinct ()
	{
		NGUISettings.minimalisticLook = false;
		if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
	}

	[MenuItem("NGUI/Options/Inspector Look/Set to Distinct", true, 10)]
	static public bool SetToDistinctCheck () { return NGUISettings.minimalisticLook; }

	[MenuItem("NGUI/Options/Inspector Look/Set to Unified", false, 10)]
	static public void SetToUnified ()
	{
		NGUISettings.unifiedTransform = true;
		if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
	}

	[MenuItem("NGUI/Options/Inspector Look/Set to Unified", true, 10)]
	static public bool SetToUnifiedCheck () { return !NGUISettings.unifiedTransform; }

	[MenuItem("NGUI/Options/Inspector Look/Set to Traditional", false, 10)]
	static public void SetToTraditional ()
	{
		NGUISettings.unifiedTransform = false;
		if (NGUITransformInspector.instance != null) NGUITransformInspector.instance.Repaint();
	}

	[MenuItem("NGUI/Options/Inspector Look/Set to Traditional", true, 10)]
	static public bool SetToTraditionalCheck () { return NGUISettings.unifiedTransform; }

	[MenuItem("NGUI/Options/Snapping/Turn On", false, 10)]
	static public void TurnSnapOn () { NGUISnap.allow = true; }

	[MenuItem("NGUI/Options/Snapping/Turn On", true, 10)]
	static public bool TurnSnapOnCheck () { return !NGUISnap.allow; }

	[MenuItem("NGUI/Options/Snapping/Turn Off", false, 10)]
	static public void TurnSnapOff () { NGUISnap.allow = false; }

	[MenuItem("NGUI/Options/Snapping/Turn Off", true, 10)]
	static public bool TurnSnapOffCheck () { return NGUISnap.allow; }

	[MenuItem("NGUI/Options/Guides/Set to Always On", false, 10)]
	static public void TurnGuidesOn () { NGUISettings.drawGuides = true; }

	[MenuItem("NGUI/Options/Guides/Set to Always On", true, 10)]
	static public bool TurnGuidesOnCheck () { return !NGUISettings.drawGuides; }

	[MenuItem("NGUI/Options/Guides/Set to Only When Needed", false, 10)]
	static public void TurnGuidesOff () { NGUISettings.drawGuides = false; }

	[MenuItem("NGUI/Options/Guides/Set to Only When Needed", true, 10)]
	static public bool TurnGuidesOffCheck () { return NGUISettings.drawGuides; }

	[MenuItem("NGUI/Options/Reset Prefab Toolbar", false, 10)]
	static public void ResetPrefabTool ()
	{
		if (UIPrefabTool.instance == null) OpenPrefabTool();
		UIPrefabTool.instance.Reset();
		UIPrefabTool.instance.Repaint();
	}

	[MenuItem("NGUI/Extras/Switch to 2D Colliders", false, 10)]
	static public void SwitchTo2D ()
	{
		BoxCollider[] colliders = NGUITools.FindActive<BoxCollider>();
		
		for (int i = 0; i < colliders.Length; ++i)
		{
			BoxCollider c = colliders[i];
			GameObject go = c.gameObject;

			UICamera cam = UICamera.FindCameraForLayer(go.layer);
			if (cam == null) continue;
			if (cam.eventType == UICamera.EventType.World_3D) continue;
			if (cam.eventType == UICamera.EventType.World_2D) continue;

			cam.eventType = UICamera.EventType.UI_2D;

			Vector3 center = c.center;
			Vector3 size = c.size;
			NGUITools.DestroyImmediate(c);

			BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
			bc.size = size;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			bc.center = center;
#else
			bc.offset = center;
#endif
			bc.isTrigger = true;
			NGUITools.SetDirty(go);

			UIPanel p = NGUITools.FindInParents<UIPanel>(go);
			
			if (p != null)
			{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				if (p.rigidbody != null) NGUITools.Destroy(p.rigidbody);
#else
				if (p.GetComponent<Rigidbody>() != null) NGUITools.Destroy(p.GetComponent<Rigidbody>());
#endif
				// It's unclear if having a 2D rigidbody actually helps or not
				//if (p.GetComponent<Rigidbody2D>() == null)
				//{
				//    Rigidbody2D rb = p.gameObject.AddComponent<Rigidbody2D>();
				//    rb.isKinematic = true;
				//}
			}
		}
	}

	[MenuItem("NGUI/Extras/Switch to 3D Colliders", false, 10)]
	static public void SwitchTo3D ()
	{
		BoxCollider2D[] colliders = NGUITools.FindActive<BoxCollider2D>();

		for (int i = 0; i < colliders.Length; ++i)
		{
			BoxCollider2D c = colliders[i];
			GameObject go = c.gameObject;

			UICamera cam = UICamera.FindCameraForLayer(go.layer);
			if (cam == null) continue;
			if (cam.eventType == UICamera.EventType.World_3D) continue;
			if (cam.eventType == UICamera.EventType.World_2D) continue;

			cam.eventType = UICamera.EventType.UI_3D;

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Vector3 center = c.center;
#else
			Vector3 center = c.offset;
#endif
			Vector3 size = c.size;
			NGUITools.DestroyImmediate(c);

			BoxCollider bc = go.AddComponent<BoxCollider>();

			if (bc != null)
			{
				bc.size = size;
				bc.center = center;
				bc.isTrigger = true;
			}
			NGUITools.SetDirty(go);

			UIPanel p = NGUITools.FindInParents<UIPanel>(go);
			
			if (p != null)
			{
				if (p.GetComponent<Rigidbody2D>() != null)
					NGUITools.Destroy(p.GetComponent<Rigidbody2D>());

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				if (p.rigidbody == null)
#else
				if (p.GetComponent<Rigidbody>() == null)
#endif
				{
					Rigidbody rb = p.gameObject.AddComponent<Rigidbody>();
					rb.isKinematic = true;
					rb.useGravity = false;
				}
			}
		}
	}

	[MenuItem("NGUI/Extras/Align Scene View to UI", false, 10)]
	static public void AlignSVToUI ()
	{
		GameObject go = Selection.activeGameObject ?? UICamera.list[0].gameObject;
		Camera cam = NGUITools.FindCameraForLayer(go.layer);
		SceneView sv = SceneView.lastActiveSceneView;
		Camera svc = sv.camera;
		svc.nearClipPlane = cam.nearClipPlane;
		svc.farClipPlane = cam.farClipPlane;
		sv.size = Mathf.Sqrt(svc.aspect) / 0.7071068f;
		sv.pivot = cam.transform.position;
		sv.rotation = cam.transform.rotation;
		sv.orthographic = true;
		sv.Repaint();
	}

	[MenuItem("NGUI/Extras/Align Scene View to UI", true, 10)]
	static public bool AlignSVToUICheck ()
	{
		if (SceneView.lastActiveSceneView == null) return false;
		if (UICamera.list.size == 0) return false;
		
		GameObject go = Selection.activeGameObject ?? UICamera.list[0].gameObject;
		if (go == null) return false;
		
		Camera cam = NGUITools.FindCameraForLayer(go.layer);
		if (cam == null || !cam.orthographic) return false;
		return true;
	}

	[MenuItem("GameObject/Align View To Selected UI &f", false, 999)]
	static public void AlignSVWithSelectedUI () { AlignSVToUI(); }

	[MenuItem("GameObject/Align View To Selected UI &f", true, 999)]
	static public bool AlignSVWithSelectedUICheck ()
	{
		GameObject go = Selection.activeGameObject;
		if (go == null) return false;
		return AlignSVToUICheck();
	}
#endregion

	[MenuItem("NGUI/Normalize Depth Hierarchy &#0", false, 11)]
	static public void Normalize () { NGUITools.NormalizeDepths(); }

	[MenuItem("NGUI/Help", false, 120)]
	static public void Help () { NGUIHelp.Show(); }
}
