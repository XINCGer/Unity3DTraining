using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// This editor helper class makes it easy to create and show a context menu.
/// It ensures that it's possible to add multiple items with the same name.
/// </summary>

static public class NGUIContextMenu
{
	[MenuItem("Help/NGUI Documentation (v.3.11.0)")]
	static void ShowHelp0 (MenuCommand command) { NGUIHelp.Show(); }

	[MenuItem("Help/NGUI Support Forum")]
	static void ShowHelp01 (MenuCommand command) { Application.OpenURL("http://www.tasharen.com/forum/index.php?board=1.0"); }

	[MenuItem("CONTEXT/UIWidget/Copy Widget")]
	static void CopyStyle (MenuCommand command) { NGUISettings.CopyWidget(command.context as UIWidget); }

	[MenuItem("CONTEXT/UIWidget/Paste Widget Values")]
	static void PasteStyle (MenuCommand command) { NGUISettings.PasteWidget(command.context as UIWidget, true); }

	[MenuItem("CONTEXT/UIWidget/Paste Widget Style")]
	static void PasteStyle2 (MenuCommand command) { NGUISettings.PasteWidget(command.context as UIWidget, false); }

	[MenuItem("CONTEXT/UIWidget/Help")]
	static void ShowHelp1 (MenuCommand command) { NGUIHelp.Show(command.context); }

	[MenuItem("CONTEXT/UIButton/Help")]
	static void ShowHelp2 (MenuCommand command) { NGUIHelp.Show(typeof(UIButton)); }

	[MenuItem("CONTEXT/UIToggle/Help")]
	static void ShowHelp3 (MenuCommand command) { NGUIHelp.Show(typeof(UIToggle)); }

	[MenuItem("CONTEXT/UIRoot/Help")]
	static void ShowHelp4 (MenuCommand command) { NGUIHelp.Show(typeof(UIRoot)); }

	[MenuItem("CONTEXT/UICamera/Help")]
	static void ShowHelp5 (MenuCommand command) { NGUIHelp.Show(typeof(UICamera)); }

	[MenuItem("CONTEXT/UIAnchor/Help")]
	static void ShowHelp6 (MenuCommand command) { NGUIHelp.Show(typeof(UIAnchor)); }

	[MenuItem("CONTEXT/UIStretch/Help")]
	static void ShowHelp7 (MenuCommand command) { NGUIHelp.Show(typeof(UIStretch)); }

	[MenuItem("CONTEXT/UISlider/Help")]
	static void ShowHelp8 (MenuCommand command) { NGUIHelp.Show(typeof(UISlider)); }

	[MenuItem("CONTEXT/UI2DSprite/Help")]
	static void ShowHelp9 (MenuCommand command) { NGUIHelp.Show(typeof(UI2DSprite)); }

	[MenuItem("CONTEXT/UIScrollBar/Help")]
	static void ShowHelp10 (MenuCommand command) { NGUIHelp.Show(typeof(UIScrollBar)); }

	[MenuItem("CONTEXT/UIProgressBar/Help")]
	static void ShowHelp11 (MenuCommand command) { NGUIHelp.Show(typeof(UIProgressBar)); }

	[MenuItem("CONTEXT/UIPopupList/Help")]
	static void ShowHelp12 (MenuCommand command) { NGUIHelp.Show(typeof(UIPopupList)); }

	[MenuItem("CONTEXT/UIInput/Help")]
	static void ShowHelp13 (MenuCommand command) { NGUIHelp.Show(typeof(UIInput)); }

	[MenuItem("CONTEXT/UIKeyBinding/Help")]
	static void ShowHelp14 (MenuCommand command) { NGUIHelp.Show(typeof(UIKeyBinding)); }

	[MenuItem("CONTEXT/UIGrid/Help")]
	static void ShowHelp15 (MenuCommand command) { NGUIHelp.Show(typeof(UIGrid)); }

	[MenuItem("CONTEXT/UITable/Help")]
	static void ShowHelp16 (MenuCommand command) { NGUIHelp.Show(typeof(UITable)); }

	[MenuItem("CONTEXT/UIPlayTween/Help")]
	static void ShowHelp17 (MenuCommand command) { NGUIHelp.Show(typeof(UIPlayTween)); }

	[MenuItem("CONTEXT/UIPlayAnimation/Help")]
	static void ShowHelp18 (MenuCommand command) { NGUIHelp.Show(typeof(UIPlayAnimation)); }

	[MenuItem("CONTEXT/UIPlaySound/Help")]
	static void ShowHelp19 (MenuCommand command) { NGUIHelp.Show(typeof(UIPlaySound)); }

	[MenuItem("CONTEXT/UIScrollView/Help")]
	static void ShowHelp20 (MenuCommand command) { NGUIHelp.Show(typeof(UIScrollView)); }

	[MenuItem("CONTEXT/UIDragScrollView/Help")]
	static void ShowHelp21 (MenuCommand command) { NGUIHelp.Show(typeof(UIDragScrollView)); }

	[MenuItem("CONTEXT/UICenterOnChild/Help")]
	static void ShowHelp22 (MenuCommand command) { NGUIHelp.Show(typeof(UICenterOnChild)); }

	[MenuItem("CONTEXT/UICenterOnClick/Help")]
	static void ShowHelp23 (MenuCommand command) { NGUIHelp.Show(typeof(UICenterOnClick)); }

	[MenuItem("CONTEXT/UITweener/Help")]
	[MenuItem("CONTEXT/UIPlayTween/Help")]
	static void ShowHelp24 (MenuCommand command) { NGUIHelp.Show(typeof(UITweener)); }

	[MenuItem("CONTEXT/ActiveAnimation/Help")]
	[MenuItem("CONTEXT/UIPlayAnimation/Help")]
	static void ShowHelp25 (MenuCommand command) { NGUIHelp.Show(typeof(UIPlayAnimation)); }

	[MenuItem("CONTEXT/UIScrollView/Help")]
	[MenuItem("CONTEXT/UIDragScrollView/Help")]
	static void ShowHelp26 (MenuCommand command) { NGUIHelp.Show(typeof(UIScrollView)); }

	[MenuItem("CONTEXT/UIPanel/Help")]
	static void ShowHelp27 (MenuCommand command) { NGUIHelp.Show(typeof(UIPanel)); }

	[MenuItem("CONTEXT/UILocalize/Help")]
	static void ShowHelp28 (MenuCommand command) { NGUIHelp.Show(typeof(UILocalize)); }

	[MenuItem("CONTEXT/Localization/Help")]
	static void ShowHelp29 (MenuCommand command) { NGUIHelp.Show(typeof(Localization)); }

	[MenuItem("CONTEXT/UIKeyNavigation/Help")]
	static void ShowHelp30 (MenuCommand command) { NGUIHelp.Show(typeof(UIKeyNavigation)); }
	
	[MenuItem("CONTEXT/PropertyBinding/Help")]
	static void ShowHelp31 (MenuCommand command) { NGUIHelp.Show(typeof(PropertyBinding)); }

	public delegate UIWidget AddFunc (GameObject go);

	static List<string> mEntries = new List<string>();
	static GenericMenu mMenu;

	/// <summary>
	/// Clear the context menu list.
	/// </summary>

	static public void Clear ()
	{
		mEntries.Clear();
		mMenu = null;
	}

	/// <summary>
	/// Add a new context menu entry.
	/// </summary>

	static public void AddItem (string item, bool isChecked, GenericMenu.MenuFunction2 callback, object param)
	{
		if (callback != null)
		{
			if (mMenu == null) mMenu = new GenericMenu();
			int count = 0;

			for (int i = 0; i < mEntries.Count; ++i)
			{
				string str = mEntries[i];
				if (str == item) ++count;
			}
			mEntries.Add(item);

			if (count > 0) item += " [" + count + "]";
			mMenu.AddItem(new GUIContent(item), isChecked, callback, param);
		}
		else AddDisabledItem(item);
	}

	/// <summary>
	/// Wrapper function called by the menu that in turn calls the correct callback.
	/// </summary>

	static public void AddChild (object obj)
	{
		AddFunc func = obj as AddFunc;
		UIWidget widget = func(Selection.activeGameObject);
		if (widget != null) Selection.activeGameObject = widget.gameObject;
	}

	/// <summary>
	/// Add a new context menu entry.
	/// </summary>

	static public void AddChildWidget (string item, bool isChecked, AddFunc callback)
	{
		if (callback != null)
		{
			if (mMenu == null) mMenu = new GenericMenu();
			int count = 0;

			for (int i = 0; i < mEntries.Count; ++i)
			{
				string str = mEntries[i];
				if (str == item) ++count;
			}
			mEntries.Add(item);

			if (count > 0) item += " [" + count + "]";
			mMenu.AddItem(new GUIContent(item), isChecked, AddChild, callback);
		}
		else AddDisabledItem(item);
	}

	/// <summary>
	/// Wrapper function called by the menu that in turn calls the correct callback.
	/// </summary>

	static public void AddSibling (object obj)
	{
		AddFunc func = obj as AddFunc;
		UIWidget widget = func(Selection.activeTransform.parent.gameObject);
		if (widget != null) Selection.activeGameObject = widget.gameObject;
	}

	/// <summary>
	/// Add a new context menu entry.
	/// </summary>

	static public void AddSiblingWidget (string item, bool isChecked, AddFunc callback)
	{
		if (callback != null)
		{
			if (mMenu == null) mMenu = new GenericMenu();
			int count = 0;

			for (int i = 0; i < mEntries.Count; ++i)
			{
				string str = mEntries[i];
				if (str == item) ++count;
			}
			mEntries.Add(item);

			if (count > 0) item += " [" + count + "]";
			mMenu.AddItem(new GUIContent(item), isChecked, AddSibling, callback);
		}
		else AddDisabledItem(item);
	}

	/// <summary>
	/// Add commonly NGUI context menu options.
	/// </summary>

	static public void AddCommonItems (GameObject target)
	{
		if (target != null)
		{
			UIWidget widget = target.GetComponent<UIWidget>();

			string myName = string.Format("Selected {0}", (widget != null) ? NGUITools.GetTypeName(widget) : "Object");

			AddItem(myName + "/Bring to Front", false,
				delegate(object obj)
				{
					for (int i = 0; i < Selection.gameObjects.Length; ++i)
						NGUITools.BringForward(Selection.gameObjects[i]);
				},
				null);

			AddItem(myName + "/Push to Back", false,
				delegate(object obj)
				{
					for (int i = 0; i < Selection.gameObjects.Length; ++i)
						NGUITools.PushBack(Selection.gameObjects[i]);
				},
				null);

			AddItem(myName + "/Nudge Forward", false,
				delegate(object obj)
				{
					for (int i = 0; i < Selection.gameObjects.Length; ++i)
						NGUITools.AdjustDepth(Selection.gameObjects[i], 1);
				},
				null);

			AddItem(myName + "/Nudge Back", false,
				delegate(object obj)
				{
					for (int i = 0; i < Selection.gameObjects.Length; ++i)
						NGUITools.AdjustDepth(Selection.gameObjects[i], -1);
				},
				null);

			if (widget != null)
			{
				NGUIContextMenu.AddSeparator(myName + "/");

				AddItem(myName + "/Make Pixel-Perfect", false, OnMakePixelPerfect, Selection.activeTransform);

				if (target.GetComponent<BoxCollider>() != null)
				{
					AddItem(myName + "/Reset Collider Size", false, OnBoxCollider, target);
				}
			}

			NGUIContextMenu.AddSeparator(myName + "/");
			AddItem(myName + "/Delete", false, OnDelete, target);
			NGUIContextMenu.AddSeparator("");

			if (Selection.activeTransform.parent != null && widget != null)
			{
				AddChildWidget("Create/Sprite/Child", false, NGUISettings.AddSprite);
				AddChildWidget("Create/Label/Child", false, NGUISettings.AddLabel);
				AddChildWidget("Create/Invisible Widget/Child", false, NGUISettings.AddWidget);
				AddChildWidget("Create/Simple Texture/Child", false, NGUISettings.AddTexture);
				AddChildWidget("Create/Unity 2D Sprite/Child", false, NGUISettings.Add2DSprite);
				AddSiblingWidget("Create/Sprite/Sibling", false, NGUISettings.AddSprite);
				AddSiblingWidget("Create/Label/Sibling", false, NGUISettings.AddLabel);
				AddSiblingWidget("Create/Invisible Widget/Sibling", false, NGUISettings.AddWidget);
				AddSiblingWidget("Create/Simple Texture/Sibling", false, NGUISettings.AddTexture);
				AddSiblingWidget("Create/Unity 2D Sprite/Sibling", false, NGUISettings.Add2DSprite);
			}
			else
			{
				AddChildWidget("Create/Sprite", false, NGUISettings.AddSprite);
				AddChildWidget("Create/Label", false, NGUISettings.AddLabel);
				AddChildWidget("Create/Invisible Widget", false, NGUISettings.AddWidget);
				AddChildWidget("Create/Simple Texture", false, NGUISettings.AddTexture);
				AddChildWidget("Create/Unity 2D Sprite", false, NGUISettings.Add2DSprite);
			}

			NGUIContextMenu.AddSeparator("Create/");

			AddItem("Create/Panel", false, AddPanel, target);
			AddItem("Create/Scroll View", false, AddScrollView, target);
			AddItem("Create/Grid", false, AddChild<UIGrid>, target);
			AddItem("Create/Table", false, AddChild<UITable>, target);
			AddItem("Create/Anchor (Legacy)", false, AddChild<UIAnchor>, target);

			if (target.GetComponent<UIPanel>() != null)
			{
				if (target.GetComponent<UIScrollView>() == null)
				{
					AddItem("Attach/Scroll View", false, Attach, typeof(UIScrollView));
					NGUIContextMenu.AddSeparator("Attach/");
				}
			}
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			else if (target.collider == null && target.GetComponent<Collider2D>() == null)
#else
			else if (target.GetComponent<Collider>() == null && target.GetComponent<Collider2D>() == null)
#endif
			{
				AddItem("Attach/Box Collider", false, AttachCollider, null);
				NGUIContextMenu.AddSeparator("Attach/");
			}

			bool header = false;
			UIScrollView scrollView = NGUITools.FindInParents<UIScrollView>(target);

			if (scrollView != null)
			{
				if (scrollView.GetComponentInChildren<UICenterOnChild>() == null)
				{
					AddItem("Attach/Center Scroll View on Child", false, Attach, typeof(UICenterOnChild));
					header = true;
				}
			}

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			if (target.collider != null || target.GetComponent<Collider2D>() != null)
#else
			if (target.GetComponent<Collider>() != null || target.GetComponent<Collider2D>() != null)
#endif
			{
				if (scrollView != null)
				{
					if (target.GetComponent<UIDragScrollView>() == null)
					{
						AddItem("Attach/Drag Scroll View", false, Attach, typeof(UIDragScrollView));
						header = true;
					}

					if (target.GetComponent<UICenterOnClick>() == null && NGUITools.FindInParents<UICenterOnChild>(target) != null)
					{
						AddItem("Attach/Center Scroll View on Click", false, Attach, typeof(UICenterOnClick));
						header = true;
					}
				}

				if (header) NGUIContextMenu.AddSeparator("Attach/");

				AddItem("Attach/Button Script", false, Attach, typeof(UIButton));
				AddItem("Attach/Toggle Script", false, Attach, typeof(UIToggle));
				AddItem("Attach/Slider Script", false, Attach, typeof(UISlider));
				AddItem("Attach/Scroll Bar Script", false, Attach, typeof(UIScrollBar));
				AddItem("Attach/Progress Bar Script", false, Attach, typeof(UISlider));
				AddItem("Attach/Popup List Script", false, Attach, typeof(UIPopupList));
				AddItem("Attach/Input Field Script", false, Attach, typeof(UIInput));
				NGUIContextMenu.AddSeparator("Attach/");
				
				if (target.GetComponent<UIDragResize>() == null)
					AddItem("Attach/Drag Resize Script", false, Attach, typeof(UIDragResize));

				if (target.GetComponent<UIDragScrollView>() == null)
				{
					for (int i = 0; i < UIPanel.list.Count; ++i)
					{
						UIPanel pan = UIPanel.list[i];
						if (pan.clipping == UIDrawCall.Clipping.None) continue;

						UIScrollView dr = pan.GetComponent<UIScrollView>();
						if (dr == null) continue;

						AddItem("Attach/Drag Scroll View", false, delegate(object obj)
						{ target.AddComponent<UIDragScrollView>().scrollView = dr; }, null);

						header = true;
						break;
					}
				}

				AddItem("Attach/Key Binding Script", false, Attach, typeof(UIKeyBinding));

				if (target.GetComponent<UIKeyNavigation>() == null)
					AddItem("Attach/Key Navigation Script", false, Attach, typeof(UIKeyNavigation));

				NGUIContextMenu.AddSeparator("Attach/");

				AddItem("Attach/Play Tween Script", false, Attach, typeof(UIPlayTween));
				AddItem("Attach/Play Animation Script", false, Attach, typeof(UIPlayAnimation));
				AddItem("Attach/Play Sound Script", false, Attach, typeof(UIPlaySound));
			}

			AddItem("Attach/Property Binding", false, Attach, typeof(PropertyBinding));

			if (target.GetComponent<UILocalize>() == null)
				AddItem("Attach/Localization Script", false, Attach, typeof(UILocalize));

			if (widget != null)
			{
				AddMissingItem<TweenAlpha>(target, "Tween/Alpha");
				AddMissingItem<TweenColor>(target, "Tween/Color");
				AddMissingItem<TweenWidth>(target, "Tween/Width");
				AddMissingItem<TweenHeight>(target, "Tween/Height");
			}
			else if (target.GetComponent<UIPanel>() != null)
			{
				AddMissingItem<TweenAlpha>(target, "Tween/Alpha");
			}

			NGUIContextMenu.AddSeparator("Tween/");

			AddMissingItem<TweenPosition>(target, "Tween/Position");
			AddMissingItem<TweenRotation>(target, "Tween/Rotation");
			AddMissingItem<TweenScale>(target, "Tween/Scale");
			AddMissingItem<TweenTransform>(target, "Tween/Transform");

			if (target.GetComponent<AudioSource>() != null)
				AddMissingItem<TweenVolume>(target, "Tween/Volume");

			if (target.GetComponent<Camera>() != null)
			{
				AddMissingItem<TweenFOV>(target, "Tween/Field of View");
				AddMissingItem<TweenOrthoSize>(target, "Tween/Orthographic Size");
			}
		}
	}

	/// <summary>
	/// Helper function that adds a widget collider to the specified object.
	/// </summary>

	static void AttachCollider (object obj)
	{
		if (Selection.activeGameObject != null)
			for (int i = 0; i < Selection.gameObjects.Length; ++i)
				NGUITools.AddWidgetCollider(Selection.gameObjects[i]);
	}

	/// <summary>
	/// Helper function that adds the specified type to all selected game objects. Used with the menu options above.
	/// </summary>

	static void Attach (object obj)
	{
		if (Selection.activeGameObject == null) return;
		System.Type type = (System.Type)obj;

		for (int i = 0; i < Selection.gameObjects.Length; ++i)
		{
			GameObject go = Selection.gameObjects[i];
			if (go.GetComponent(type) != null) continue;
#if !UNITY_3_5
			Component cmp = go.AddComponent(type);
			Undo.RegisterCreatedObjectUndo(cmp, "Attach " + type);
#endif
		}
	}

	/// <summary>
	/// Helper function.
	/// </summary>

	static void AddMissingItem<T> (GameObject target, string name) where T : MonoBehaviour
	{
		if (target.GetComponent<T>() == null)
			AddItem(name, false, Attach, typeof(T));
	}

	/// <summary>
	/// Helper function for menu creation.
	/// </summary>

	static void AddChild<T> (object obj) where T : MonoBehaviour
	{
		GameObject go = obj as GameObject;
		T t = NGUITools.AddChild<T>(go);
		Selection.activeGameObject = t.gameObject;
	}

	/// <summary>
	/// Helper function for menu creation.
	/// </summary>

	static void AddPanel (object obj)
	{
		GameObject go = obj as GameObject;
		if (go.GetComponent<UIWidget>() != null) go = go.transform.parent.gameObject;
		UIPanel panel = NGUISettings.AddPanel(go);
		Selection.activeGameObject = panel.gameObject;
	}

	/// <summary>
	/// Helper function for menu creation.
	/// </summary>

	static void AddScrollView (object obj)
	{
		GameObject go = obj as GameObject;
		if (go.GetComponent<UIWidget>() != null) go = go.transform.parent.gameObject;
		UIPanel panel = NGUISettings.AddPanel(go);
		panel.clipping = UIDrawCall.Clipping.SoftClip;
		panel.gameObject.AddComponent<UIScrollView>();
		panel.name = "Scroll View";
		Selection.activeGameObject = panel.gameObject;
	}

	/// <summary>
	/// Add help options based on the components present on the specified game object.
	/// </summary>

	static public void AddHelp (GameObject go, bool addSeparator)
	{
		MonoBehaviour[] comps = Selection.activeGameObject.GetComponents<MonoBehaviour>();

		bool addedSomething = false;

		for (int i = 0; i < comps.Length; ++i)
		{
			System.Type type = comps[i].GetType();
			string url = NGUIHelp.GetHelpURL(type);
			
			if (url != null)
			{
				if (addSeparator)
				{
					addSeparator = false;
					AddSeparator("");
				}

				AddItem("Help/" + type, false, delegate(object obj) { Application.OpenURL(url); }, null);
				addedSomething = true;
			}
		}

		if (addedSomething) AddSeparator("Help/");
		AddItem("Help/All Topics", false, delegate(object obj) { NGUIHelp.Show(); }, null);
	}

	static void OnHelp (object obj) { NGUIHelp.Show(obj); }
	static void OnMakePixelPerfect (object obj) { NGUITools.MakePixelPerfect(obj as Transform); }
	static void OnBoxCollider (object obj) { NGUITools.AddWidgetCollider(obj as GameObject); }
	static void OnDelete (object obj)
	{
		GameObject go = obj as GameObject;
		Selection.activeGameObject = go.transform.parent.gameObject;
		Undo.DestroyObjectImmediate(go);
	}

	/// <summary>
	/// Add a new disabled context menu entry.
	/// </summary>

	static public void AddDisabledItem (string item)
	{
		if (mMenu == null) mMenu = new GenericMenu();
		mMenu.AddDisabledItem(new GUIContent(item));
	}

	/// <summary>
	/// Add a separator to the menu.
	/// </summary>

	static public void AddSeparator (string path)
	{
		if (mMenu == null) mMenu = new GenericMenu();

		// For some weird reason adding separators on OSX causes the entire menu to be disabled. Wtf?
		if (Application.platform != RuntimePlatform.OSXEditor)
			mMenu.AddSeparator(path);
	}

	/// <summary>
	/// Show the context menu with all the added items.
	/// </summary>

	static public void Show ()
	{
		if (mMenu != null)
		{
			mMenu.ShowAsContext();
			mMenu = null;
			mEntries.Clear();
		}
	}
}
