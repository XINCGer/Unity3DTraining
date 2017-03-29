//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI Widget Creation Wizard
/// </summary>

public class UICreateWidgetWizard : EditorWindow
{
	public enum WidgetType
	{
		Label,
		Sprite,
		Texture,
		Button,
		ImageButton,
		Toggle,
		ProgressBar,
		Slider,
		Input,
		PopupList,
		PopupMenu,
		ScrollBar,
	}

	static WidgetType mWidgetType = WidgetType.Button;
	static string mButton = "";
	static string mImage0 = "";
	static string mImage1 = "";
	static string mImage2 = "";
	static string mImage3 = "";
	static string mSliderBG = "";
	static string mSliderFG = "";
	static string mSliderTB = "";
	static string mCheckBG = "";
	static string mCheck = "";
	static string mInputBG = "";
	static string mListFG = "";
	static string mListBG = "";
	static string mListHL = "";
	static string mScrollBG = "";
	static string mScrollFG = "";
	static Color mColor = Color.white;
	static bool mLoaded = false;
	static bool mScrollCL = true;
	static UIScrollBar.FillDirection mFillDir = UIScrollBar.FillDirection.LeftToRight;

	/// <summary>
	/// Save the specified string into player prefs.
	/// </summary>

	static void SaveString (string field, string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			EditorPrefs.DeleteKey(field);
		}
		else
		{
			EditorPrefs.SetString(field, val);
		}
	}

	/// <summary>
	/// Load the specified string from player prefs.
	/// </summary>

	static string LoadString (string field) { string s = EditorPrefs.GetString(field); return (string.IsNullOrEmpty(s)) ? "" : s; }

	/// <summary>
	/// Save all serialized values in editor prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Save ()
	{
		EditorPrefs.SetInt("NGUI Widget Type", (int)mWidgetType);
		EditorPrefs.SetInt("NGUI Color", NGUIMath.ColorToInt(mColor));
		EditorPrefs.SetBool("NGUI ScrollCL", mScrollCL);
		EditorPrefs.SetInt("NGUI Fill Dir", (int)mFillDir);

		SaveString("NGUI Button", mButton);
		SaveString("NGUI Image 0", mImage0);
		SaveString("NGUI Image 1", mImage1);
		SaveString("NGUI Image 2", mImage2);
		SaveString("NGUI Image 3", mImage3);
		SaveString("NGUI CheckBG", mCheckBG);
		SaveString("NGUI Check", mCheck);
		SaveString("NGUI SliderBG", mSliderBG);
		SaveString("NGUI SliderFG", mSliderFG);
		SaveString("NGUI SliderTB", mSliderTB);
		SaveString("NGUI InputBG", mInputBG);
		SaveString("NGUI ListFG", mListFG);
		SaveString("NGUI ListBG", mListBG);
		SaveString("NGUI ListHL", mListHL);
		SaveString("NGUI ScrollBG", mScrollBG);
		SaveString("NGUI ScrollFG", mScrollFG);
	}

	/// <summary>
	/// Load all serialized values from editor prefs.
	/// This is necessary because static values get wiped out as soon as scripts get recompiled.
	/// </summary>

	static void Load ()
	{
		mWidgetType = (WidgetType)EditorPrefs.GetInt("NGUI Widget Type", 0);
		mFillDir = (UIScrollBar.FillDirection)EditorPrefs.GetInt("NGUI Fill Dir", 0);

		int color = EditorPrefs.GetInt("NGUI Color", -1);
		if (color != -1) mColor = NGUIMath.IntToColor(color);

		mButton		= LoadString("NGUI Button");
		mImage0		= LoadString("NGUI Image 0");
		mImage1		= LoadString("NGUI Image 1");
		mImage2		= LoadString("NGUI Image 2");
		mImage3		= LoadString("NGUI Image 3");
		mCheckBG	= LoadString("NGUI CheckBG");
		mCheck		= LoadString("NGUI Check");
		mSliderBG	= LoadString("NGUI SliderBG");
		mSliderFG	= LoadString("NGUI SliderFG");
		mSliderTB	= LoadString("NGUI SliderTB");
		mInputBG	= LoadString("NGUI InputBG");
		mListFG		= LoadString("NGUI ListFG");
		mListBG		= LoadString("NGUI ListBG");
		mListHL		= LoadString("NGUI ListHL");
		mScrollBG	= LoadString("NGUI ScrollBG");
		mScrollFG	= LoadString("NGUI ScrollFG");
		mScrollCL	= EditorPrefs.GetBool("NGUI ScrollCL", true);
	}

	/// <summary>
	/// Atlas selection function.
	/// </summary>

	void OnSelectAtlas (Object obj)
	{
		if (NGUISettings.atlas != obj)
		{
			NGUISettings.atlas = obj as UIAtlas;
			Repaint();
		}
	}

	/// <summary>
	/// Font selection function.
	/// </summary>

	void OnSelectFont (Object obj)
	{
		Object fnt = obj as UIFont;

		if (NGUISettings.ambigiousFont != fnt)
		{
			NGUISettings.ambigiousFont = fnt;
			Repaint();
		}
	}

	/// <summary>
	/// Convenience function -- creates the "Add To" button and the parent object field to the right of it.
	/// </summary>

	static public bool ShouldCreate (GameObject go, bool isValid)
	{
		GUI.color = isValid ? Color.green : Color.grey;

		GUILayout.BeginHorizontal();
		bool retVal = GUILayout.Button("Add To", GUILayout.Width(76f));
		GUI.color = Color.white;
		GameObject sel = EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(140f)) as GameObject;
		GUILayout.Label("Select the parent in the Hierarchy View", GUILayout.MinWidth(10000f));
		GUILayout.EndHorizontal();

		if (sel != go) Selection.activeGameObject = sel;

		if (retVal && isValid)
		{
			NGUIEditorTools.RegisterUndo("Add a Widget");
			return true;
		}
		return false;
	}

	/// <summary>
	/// Label creation function.
	/// </summary>

	void CreateLabel (GameObject go)
	{
		GUILayout.BeginHorizontal();
		Color c = EditorGUILayout.ColorField("Color", mColor, GUILayout.Width(220f));
		GUILayout.Label("Color tint the label will start with");
		GUILayout.EndHorizontal();

		if (mColor != c)
		{
			mColor = c;
			Save();
		}

		if (ShouldCreate(go, NGUISettings.ambigiousFont != null))
		{
			Selection.activeGameObject = NGUISettings.AddLabel(go).gameObject;
		}
	}

	/// <summary>
	/// Sprite creation function.
	/// </summary>

	void CreateSprite (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Sprite", "Sprite that will be created", NGUISettings.atlas, NGUISettings.selectedSprite, OnSprite, GUILayout.Width(120f));

			if (!string.IsNullOrEmpty(NGUISettings.selectedSprite))
			{
				GUILayout.BeginHorizontal();
				NGUISettings.pivot = (UIWidget.Pivot)EditorGUILayout.EnumPopup("Pivot", NGUISettings.pivot, GUILayout.Width(200f));
				GUILayout.Space(20f);
				GUILayout.Label("Initial pivot point used by the sprite");
				GUILayout.EndHorizontal();
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			Selection.activeGameObject = NGUISettings.AddSprite(go).gameObject;
		}
	}

	void OnSprite (string val)
	{
		if (NGUISettings.selectedSprite != val)
		{
			NGUISettings.selectedSprite = val;
			Repaint();
		}
	}

	/// <summary>
	/// UI Texture doesn't do anything other than creating the widget.
	/// </summary>

	void CreateSimpleTexture (GameObject go)
	{
		if (ShouldCreate(go, true))
		{
			Selection.activeGameObject = NGUISettings.AddTexture(go).gameObject;
		}
	}

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateButton (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Background", "Sliced Sprite for the background", NGUISettings.atlas, mButton, OnButton, GUILayout.Width(120f));
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Button";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mButton;
			bg.width = 200;
			bg.height = 50;
			bg.MakePixelPerfect();

			if (NGUISettings.ambigiousFont != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.ambigiousFont = NGUISettings.ambigiousFont;
				lbl.text = go.name;
				lbl.AssumeNaturalSize();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIPlaySound>();

			Selection.activeGameObject = go;
		}
	}

	void OnButton (string val) { mButton = val; Save(); Repaint(); }

	/// <summary>
	/// Button creation function.
	/// </summary>

	void CreateImageButton (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Normal", "Normal state sprite", NGUISettings.atlas, mImage0, OnImage0, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Hover", "Hover state sprite", NGUISettings.atlas, mImage1, OnImage1, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Pressed", "Pressed state sprite", NGUISettings.atlas, mImage2, OnImage2, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Disabled", "Disabled state sprite", NGUISettings.atlas, mImage3, OnImage3, GUILayout.Width(120f));
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Image Button";

			UISpriteData sp = NGUISettings.atlas.GetSprite(mImage0);
			UISprite sprite = NGUITools.AddWidget<UISprite>(go);
			sprite.type = sp.hasBorder ? UISprite.Type.Sliced : UISprite.Type.Simple;
			sprite.name = "Background";
			sprite.depth = depth;
			sprite.atlas = NGUISettings.atlas;
			sprite.spriteName = mImage0;
			sprite.width = 150;
			sprite.height = 40;
			sprite.MakePixelPerfect();

			if (NGUISettings.ambigiousFont != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.ambigiousFont = NGUISettings.ambigiousFont;
				lbl.text = go.name;
				lbl.AssumeNaturalSize();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			UIImageButton ib	= go.AddComponent<UIImageButton>();
			ib.target			= sprite;
			ib.normalSprite		= mImage0;
			ib.hoverSprite		= mImage1;
			ib.pressedSprite	= mImage2;
			ib.disabledSprite	= mImage3;
			go.AddComponent<UIPlaySound>();

			Selection.activeGameObject = go;
		}
	}

	void OnImage0 (string val) { mImage0 = val; Save(); Repaint(); }
	void OnImage1 (string val) { mImage1 = val; Save(); Repaint(); }
	void OnImage2 (string val) { mImage2 = val; Save(); Repaint(); }
	void OnImage3 (string val) { mImage3 = val; Save(); Repaint(); }

	/// <summary>
	/// Toggle creation function.
	/// </summary>

	void CreateToggle (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Background", "Sprite used for the background", NGUISettings.atlas, mCheckBG, OnCheckBG, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Checkmark", "Sprite used for the checkmark", NGUISettings.atlas, mCheck, OnCheck, GUILayout.Width(120f));
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Toggle";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mCheckBG;
			bg.width = 26;
			bg.height = 26;
			bg.MakePixelPerfect();

			UISprite fg = NGUITools.AddWidget<UISprite>(go);
			fg.name = "Checkmark";
			fg.atlas = NGUISettings.atlas;
			fg.spriteName = mCheck;
			fg.MakePixelPerfect();

			if (NGUISettings.ambigiousFont != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.ambigiousFont = NGUISettings.ambigiousFont;
				lbl.text = go.name;
				lbl.pivot = UIWidget.Pivot.Left;
				lbl.transform.localPosition = new Vector3(16f, 0f, 0f);
				lbl.AssumeNaturalSize();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIToggle>().activeSprite = fg;
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>().tweenTarget = bg.transform;
			go.AddComponent<UIPlaySound>();

			Selection.activeGameObject = go;
		}
	}

	void OnCheckBG (string val) { mCheckBG = val; Save(); Repaint(); }
	void OnCheck (string val) { mCheck = val; Save(); Repaint(); }

	/// <summary>
	/// Scroll bar template.
	/// </summary>

	void CreateScrollBar (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Background", "Sprite used for the background", NGUISettings.atlas, mScrollBG, OnScrollBG, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Foreground", "Sprite used for the foreground (thumb)", NGUISettings.atlas, mScrollFG, OnScrollFG, GUILayout.Width(120f));

			GUILayout.BeginHorizontal();
			UIScrollBar.FillDirection dir = (UIScrollBar.FillDirection)EditorGUILayout.EnumPopup("Direction", mFillDir, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Add colliders?", GUILayout.Width(90f));
			bool draggable = EditorGUILayout.Toggle(mScrollCL);
			GUILayout.EndHorizontal();

			if (mScrollCL != draggable || mFillDir != dir)
			{
				mScrollCL = draggable;
				mFillDir = dir;
				Save();
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Scroll Bar";

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mScrollBG;

			Vector4 border = bg.border;
			bg.width = Mathf.RoundToInt(400f + border.x + border.z);
			bg.height = Mathf.RoundToInt(14f + border.y + border.w);
			bg.MakePixelPerfect();

			UISprite fg = NGUITools.AddWidget<UISprite>(go);
			fg.type = UISprite.Type.Sliced;
			fg.name = "Foreground";
			fg.atlas = NGUISettings.atlas;
			fg.spriteName = mScrollFG;

			UIScrollBar sb = go.AddComponent<UIScrollBar>();
			sb.foregroundWidget = fg;
			sb.backgroundWidget = bg;
			sb.fillDirection = mFillDir;
			sb.barSize = 0.3f;
			sb.value = 0.3f;
			sb.ForceUpdate();

			if (mScrollCL)
			{
				NGUITools.AddWidgetCollider(bg.gameObject);
				NGUITools.AddWidgetCollider(fg.gameObject);
			}
			Selection.activeGameObject = go;
		}
	}

	void OnScrollBG (string val) { mScrollBG = val; Save(); Repaint(); }
	void OnScrollFG (string val) { mScrollFG = val; Save(); Repaint(); }

	/// <summary>
	/// Progress bar creation function.
	/// </summary>

	void CreateSlider (GameObject go, bool slider)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Empty", "Sprite for the background (empty bar)", NGUISettings.atlas, mSliderBG, OnSliderBG, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Full", "Sprite for the foreground (full bar)", NGUISettings.atlas, mSliderFG, OnSliderFG, GUILayout.Width(120f));

			if (slider)
			{
				NGUIEditorTools.DrawSpriteField("Thumb", "Sprite for the thumb indicator", NGUISettings.atlas, mSliderTB, OnSliderTB, GUILayout.Width(120f));
			}
		}

		if (ShouldCreate(go, NGUISettings.atlas != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = slider ? "Slider" : "Progress Bar";

			// Background sprite
			UISpriteData bgs = NGUISettings.atlas.GetSprite(mSliderBG);
			UISprite back = (UISprite)NGUITools.AddWidget<UISprite>(go);

			back.type = bgs.hasBorder ? UISprite.Type.Sliced : UISprite.Type.Simple;
			back.name = "Background";
			back.depth = depth;
			back.pivot = UIWidget.Pivot.Left;
			back.atlas = NGUISettings.atlas;
			back.spriteName = mSliderBG;
			back.width = 200;
			back.height = 30;
			back.transform.localPosition = Vector3.zero;
			back.MakePixelPerfect();

			// Foreground sprite
			UISpriteData fgs = NGUISettings.atlas.GetSprite(mSliderFG);
			UISprite front = NGUITools.AddWidget<UISprite>(go);
			front.type = fgs.hasBorder ? UISprite.Type.Sliced : UISprite.Type.Simple;
			front.name = "Foreground";
			front.pivot = UIWidget.Pivot.Left;
			front.atlas = NGUISettings.atlas;
			front.spriteName = mSliderFG;
			front.width = 200;
			front.height = 30;
			front.transform.localPosition = Vector3.zero;
			front.MakePixelPerfect();

			// Add a collider
			if (slider) NGUITools.AddWidgetCollider(go);

			// Add the slider script
			UISlider uiSlider = go.AddComponent<UISlider>();
			uiSlider.foregroundWidget = front;

			// Thumb sprite
			if (slider)
			{
				UISpriteData tbs = NGUISettings.atlas.GetSprite(mSliderTB);
				UISprite thb = NGUITools.AddWidget<UISprite>(go);

				thb.type = tbs.hasBorder ? UISprite.Type.Sliced : UISprite.Type.Simple;
				thb.name = "Thumb";
				thb.atlas = NGUISettings.atlas;
				thb.spriteName = mSliderTB;
				thb.width = 20;
				thb.height = 40;
				thb.transform.localPosition = new Vector3(200f, 0f, 0f);
				thb.MakePixelPerfect();

				NGUITools.AddWidgetCollider(thb.gameObject);
				thb.gameObject.AddComponent<UIButtonColor>();
				thb.gameObject.AddComponent<UIButtonScale>();

				uiSlider.thumb = thb.transform;
			}
			uiSlider.value = 1f;

			// Select the slider
			Selection.activeGameObject = go;
		}
	}

	void OnSliderBG (string val) { mSliderBG = val; Save(); Repaint(); }
	void OnSliderFG (string val) { mSliderFG = val; Save(); Repaint(); }
	void OnSliderTB (string val) { mSliderTB = val; Save(); Repaint(); }

	/// <summary>
	/// Input field creation function.
	/// </summary>

	void CreateInput (GameObject go)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Background", "Sliced Sprite for the background", NGUISettings.atlas, mInputBG, OnInputBG, GUILayout.Width(120f));
		}

		if (ShouldCreate(go, NGUISettings.atlas != null && NGUISettings.ambigiousFont != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Input";
			int padding = 3;

			UISprite bg = NGUITools.AddWidget<UISprite>(go);
			bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NGUISettings.atlas;
			bg.spriteName = mInputBG;
			bg.pivot = UIWidget.Pivot.Left;
			bg.width = 400;
			bg.height = NGUISettings.fontSize + padding * 2;
			bg.transform.localPosition = Vector3.zero;
			bg.MakePixelPerfect();

			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.ambigiousFont = NGUISettings.ambigiousFont;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.transform.localPosition = new Vector3(padding, 0f, 0f);
			lbl.multiLine = false;
			lbl.supportEncoding = false;
			lbl.width = Mathf.RoundToInt(400f - padding * 2f);
			lbl.text = "You can type here";
			lbl.AssumeNaturalSize();

			// Add a collider to the background
			NGUITools.AddWidgetCollider(go);

			// Add an input script to the background and have it point to the label
			UIInput input = go.AddComponent<UIInput>();
			input.label = lbl;

			// Update the selection
			Selection.activeGameObject = go;
		}
	}

	void OnInputBG (string val) { mInputBG = val; Save(); Repaint(); }

	/// <summary>
	/// Create a popup list or a menu.
	/// </summary>

	void CreatePopup (GameObject go, bool isDropDown)
	{
		if (NGUISettings.atlas != null)
		{
			NGUIEditorTools.DrawSpriteField("Foreground", "Foreground sprite (shown on the button)", NGUISettings.atlas, mListFG, OnListFG, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Background", "Background sprite (envelops the options)", NGUISettings.atlas, mListBG, OnListBG, GUILayout.Width(120f));
			NGUIEditorTools.DrawSpriteField("Highlight", "Sprite used to highlight the selected option", NGUISettings.atlas, mListHL, OnListHL, GUILayout.Width(120f));
		}

		if (ShouldCreate(go, NGUISettings.atlas != null && NGUISettings.ambigiousFont != null))
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = isDropDown ? "Popup List" : "Popup Menu";

			UISpriteData sphl = NGUISettings.atlas.GetSprite(mListHL);
			UISpriteData spfg = NGUISettings.atlas.GetSprite(mListFG);

			Vector2 hlPadding = new Vector2(Mathf.Max(4f, sphl.paddingLeft), Mathf.Max(4f, sphl.paddingTop));
			Vector2 fgPadding = new Vector2(Mathf.Max(4f, spfg.paddingLeft), Mathf.Max(4f, spfg.paddingTop));

			// Background sprite
			UISprite sprite = NGUITools.AddSprite(go, NGUISettings.atlas, mListFG);
			sprite.depth = depth;
			sprite.atlas = NGUISettings.atlas;
			sprite.pivot = UIWidget.Pivot.Left;
			sprite.width = Mathf.RoundToInt(150f + fgPadding.x * 2f);
			sprite.height = Mathf.RoundToInt(NGUISettings.fontSize + fgPadding.y * 2f);
			sprite.transform.localPosition = Vector3.zero;
			sprite.MakePixelPerfect();

			// Text label
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.ambigiousFont = NGUISettings.ambigiousFont;
			lbl.fontSize = NGUISettings.fontSize;
			lbl.fontStyle = NGUISettings.fontStyle;
			lbl.text = go.name;
			lbl.pivot = UIWidget.Pivot.Left;
			lbl.cachedTransform.localPosition = new Vector3(fgPadding.x, 0f, 0f);
			lbl.AssumeNaturalSize();

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the popup list
			UIPopupList list = go.AddComponent<UIPopupList>();
			list.atlas = NGUISettings.atlas;
			list.ambigiousFont = NGUISettings.ambigiousFont;
			list.fontSize = NGUISettings.fontSize;
			list.fontStyle = NGUISettings.fontStyle;
			list.backgroundSprite = mListBG;
			list.highlightSprite = mListHL;
			list.padding = hlPadding;
			if (isDropDown) EventDelegate.Add(list.onChange, lbl.SetCurrentSelection);
			for (int i = 0; i < 5; ++i) list.items.Add(isDropDown ? ("List Option " + i) : ("Menu Option " + i));

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = sprite.gameObject;
			go.AddComponent<UIPlaySound>();

			Selection.activeGameObject = go;
		}
	}

	void OnListFG (string val) { mListFG = val; Save(); Repaint(); }
	void OnListBG (string val) { mListBG = val; Save(); Repaint(); }
	void OnListHL (string val) { mListHL = val; Save(); Repaint(); }

	/// <summary>
	/// Repaint the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

#if DYNAMIC_FONT
	UILabelInspector.FontType mType = UILabelInspector.FontType.Unity;
#else
	UILabelInspector.FontType mType = UILabelInspector.FontType.Unity;
#endif

	void OnFont (Object obj) { NGUISettings.ambigiousFont = obj; }

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		// Load the saved preferences
		if (!mLoaded)
		{
			mLoaded = true;
			Load();
#if DYNAMIC_FONT
			Object font = NGUISettings.ambigiousFont;
			mType = ((font != null) && (font is UIFont)) ? UILabelInspector.FontType.NGUI : UILabelInspector.FontType.Unity;
#else
			mType = UILabelInspector.FontType.NGUI;
#endif
		}

		NGUIEditorTools.SetLabelWidth(80f);
		GameObject go = NGUIEditorTools.SelectedRoot();

		if (go == null)
		{
			GUILayout.Label("You must create a UI first.");
			
			if (GUILayout.Button("Open the New UI Wizard"))
			{
				EditorWindow.GetWindow<UICreateNewUIWizard>(false, "New UI", true);
			}
		}
		else
		{
			GUILayout.Space(4f);

			GUILayout.BeginHorizontal();
			ComponentSelector.Draw<UIAtlas>(NGUISettings.atlas, OnSelectAtlas, false, GUILayout.Width(140f));
			GUILayout.Label("Texture atlas used by widgets", GUILayout.Width(10000f));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			if (NGUIEditorTools.DrawPrefixButton("Font"))
			{
				if (mType == UILabelInspector.FontType.NGUI)
				{
					ComponentSelector.Show<UIFont>(OnFont);
				}
				else
				{
					ComponentSelector.Show<Font>(OnFont, new string[] { ".ttf", ".otf" });
				}
			}

#if DYNAMIC_FONT
			GUI.changed = false;

			if (mType == UILabelInspector.FontType.Unity)
			{
				NGUISettings.ambigiousFont = EditorGUILayout.ObjectField(NGUISettings.ambigiousFont, typeof(Font), false, GUILayout.Width(140f));
			}
			else
			{
				NGUISettings.ambigiousFont = EditorGUILayout.ObjectField(NGUISettings.ambigiousFont, typeof(UIFont), false, GUILayout.Width(140f));
			}
			mType = (UILabelInspector.FontType)EditorGUILayout.EnumPopup(mType, GUILayout.Width(62f));
#else
			NGUISettings.ambigiousFont = EditorGUILayout.ObjectField(NGUISettings.ambigiousFont, typeof(UIFont), false, GUILayout.Width(140f));
#endif
			GUILayout.Label("size", GUILayout.Width(30f));
			EditorGUI.BeginDisabledGroup(mType == UILabelInspector.FontType.NGUI);
			NGUISettings.fontSize = EditorGUILayout.IntField(NGUISettings.fontSize, GUILayout.Width(30f));
			EditorGUI.EndDisabledGroup();
			GUILayout.Label("font used by the labels");
			GUILayout.EndHorizontal();
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			WidgetType wt = (WidgetType)EditorGUILayout.EnumPopup("Template", mWidgetType, GUILayout.Width(200f));
			GUILayout.Space(20f);
			GUILayout.Label("Select a widget template to use");
			GUILayout.EndHorizontal();

			if (mWidgetType != wt) { mWidgetType = wt; Save(); }

			switch (mWidgetType)
			{
				case WidgetType.Label:			CreateLabel(go); break;
				case WidgetType.Sprite:			CreateSprite(go); break;
				case WidgetType.Texture:		CreateSimpleTexture(go); break;
				case WidgetType.Button:			CreateButton(go); break;
				case WidgetType.ImageButton:	CreateImageButton(go); break;
				case WidgetType.Toggle:			CreateToggle(go); break;
				case WidgetType.ProgressBar:	CreateSlider(go, false); break;
				case WidgetType.Slider:			CreateSlider(go, true); break;
				case WidgetType.Input:			CreateInput(go); break;
				case WidgetType.PopupList:		CreatePopup(go, true); break;
				case WidgetType.PopupMenu:		CreatePopup(go, false); break;
				case WidgetType.ScrollBar:		CreateScrollBar(go); break;
			}

			EditorGUILayout.HelpBox("Widget Tool has become far less useful with NGUI 3.0.6. Search the Project view for 'Control', then simply drag & drop one of them into your Scene View.", MessageType.Warning);
		}
	}
}
