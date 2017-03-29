//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Popup list can be used to display pop-up menus and drop-down lists.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupList : UIWidgetContainer
{
	/// <summary>
	/// Current popup list. Only available during the OnSelectionChange event callback.
	/// </summary>

	static public UIPopupList current;
	static protected GameObject mChild;
	static protected float mFadeOutComplete = 0f;

	const float animSpeed = 0.15f;

	public enum Position
	{
		Auto,
		Above,
		Below,
	}

	/// <summary>
	/// Atlas used by the sprites.
	/// </summary>

	public UIAtlas atlas;

	/// <summary>
	/// Font used by the labels.
	/// </summary>

	public UIFont bitmapFont;

	/// <summary>
	/// True type font used by the labels. Alternative to specifying a bitmap font ('font').
	/// </summary>

	public Font trueTypeFont;

	/// <summary>
	/// Font used by the popup list. Conveniently wraps both dynamic and bitmap fonts into one property.
	/// </summary>

	public Object ambigiousFont
	{
		get
		{
			if (trueTypeFont != null) return trueTypeFont;
			if (bitmapFont != null) return bitmapFont;
			return font;
		}
		set
		{
			if (value is Font)
			{
				trueTypeFont = value as Font;
				bitmapFont = null;
				font = null;
			}
			else if (value is UIFont)
			{
				bitmapFont = value as UIFont;
				trueTypeFont = null;
				font = null;
			}
		}
	}

	/// <summary>
	/// Size of the font to use for the popup list's labels.
	/// </summary>

	public int fontSize = 16;

	/// <summary>
	/// Font style used by the dynamic font.
	/// </summary>

	public FontStyle fontStyle = FontStyle.Normal;

	/// <summary>
	/// Name of the sprite used to create the popup's background.
	/// </summary>

	public string backgroundSprite;

	/// <summary>
	/// Name of the sprite used to highlight items.
	/// </summary>

	public string highlightSprite;

	/// <summary>
	/// Name of the sprite used to create the popup's background.
	/// </summary>

	public Sprite background2DSprite;

	/// <summary>
	/// Name of the sprite used to highlight items.
	/// </summary>

	public Sprite highlight2DSprite;

	/// <summary>
	/// Popup list's display style.
	/// </summary>

	public Position position = Position.Auto;

	/// <summary>
	/// Label alignment to use.
	/// </summary>

	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	/// <summary>
	/// New line-delimited list of items.
	/// </summary>

	public List<string> items = new List<string>();

	/// <summary>
	/// You can associate arbitrary data to be associated with your entries if you like.
	/// The only downside is that this must be done via code.
	/// </summary>

	public List<object> itemData = new List<object>();

	/// <summary>
	/// Amount of padding added to labels.
	/// </summary>

	public Vector2 padding = new Vector3(4f, 4f);

	/// <summary>
	/// Color tint applied to labels inside the list.
	/// </summary>

	public Color textColor = Color.white;

	/// <summary>
	/// Color tint applied to the background.
	/// </summary>

	public Color backgroundColor = Color.white;

	/// <summary>
	/// Color tint applied to the highlighter.
	/// </summary>

	public Color highlightColor = new Color(225f / 255f, 200f / 255f, 150f / 255f, 1f);

	/// <summary>
	/// Whether the popup list is animated or not. Disable for better performance.
	/// </summary>

	public bool isAnimated = true;

	/// <summary>
	/// Whether the popup list's values will be localized.
	/// </summary>

	public bool isLocalized = false;

	/// <summary>
	/// Whether a separate panel will be used to ensure that the popup will appear on top of everything else.
	/// </summary>

	public bool separatePanel = true;

	/// <summary>
	/// Amount by which the popup's border will overlap with the content that opened it.
	/// </summary>
	
	public int overlap = 0;

	public enum OpenOn
	{
		ClickOrTap,
		RightClick,
		DoubleClick,
		Manual,
	}

	/// <summary>
	/// What kind of click is needed in order to open the popup list.
	/// </summary>

	public OpenOn openOn = OpenOn.ClickOrTap;

	/// <summary>
	/// Callbacks triggered when the popup list gets a new item selection.
	/// </summary>

	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Currently selected item
	[HideInInspector][SerializeField] protected string mSelectedItem;
	[HideInInspector][SerializeField] protected UIPanel mPanel;
	[HideInInspector][SerializeField] protected UIBasicSprite mBackground;
	[HideInInspector][SerializeField] protected UIBasicSprite mHighlight;
	[HideInInspector][SerializeField] protected UILabel mHighlightedLabel = null;
	[HideInInspector][SerializeField] protected List<UILabel> mLabelList = new List<UILabel>();
	[HideInInspector][SerializeField] protected float mBgBorder = 0f;

	[Tooltip("Whether the selection will be persistent even after the popup list is closed. By default the selection is " +
		"cleared when the popup is closed so that the same selection can be chosen again the next time the popup list is opened. " +
		"If enabled, the selection will persist, but selecting the same choice in succession will not result in the onChange " +
		"notification being triggered more than once.")]
	public bool keepValue = false;

	[System.NonSerialized] protected GameObject mSelection;
	[System.NonSerialized] protected int mOpenFrame = 0;

	// Deprecated functionality
	[HideInInspector][SerializeField] GameObject eventReceiver;
	[HideInInspector][SerializeField] string functionName = "OnSelectionChange";
	[HideInInspector][SerializeField] float textScale = 0f;
	[HideInInspector][SerializeField] UIFont font; // Use 'bitmapFont' instead

	// This functionality is no longer needed as the same can be achieved by choosing a
	// OnValueChange notification targeting a label's SetCurrentSelection function.
	// If your code was list.textLabel = myLabel, change it to:
	// EventDelegate.Add(list.onChange, lbl.SetCurrentSelection);
	[HideInInspector][SerializeField] UILabel textLabel;

	// Popup list's starting position
	[System.NonSerialized] public Vector3 startingPosition;

	public delegate void LegacyEvent (string val);
	LegacyEvent mLegacyEvent;

	[System.Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public LegacyEvent onSelectionChange { get { return mLegacyEvent; } set { mLegacyEvent = value; } }

	/// <summary>
	/// Whether the popup list is currently open.
	/// </summary>

	static public bool isOpen { get { return current != null && (mChild != null || mFadeOutComplete > Time.unscaledTime); } }

	/// <summary>
	/// Current selection.
	/// </summary>

	public virtual string value { get { return mSelectedItem; } set { Set(value); } }

	/// <summary>
	/// Item data associated with the current selection.
	/// </summary>

	public virtual object data
	{
		get
		{
			int index = items.IndexOf(mSelectedItem);
			return (index > -1) && index < itemData.Count ? itemData[index] : null;
		}
	}

	/// <summary>
	/// Whether the collider is enabled and the widget can be interacted with.
	/// </summary>

	public bool isColliderEnabled
	{
		get
		{
			Collider c = GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
	}

	[System.Obsolete("Use 'value' instead")]
	public string selection { get { return value; } set { this.value = value; } }

	/// <summary>
	/// Whether the popup list is actually usable.
	/// </summary>

	protected bool isValid { get { return bitmapFont != null || trueTypeFont != null; } }

	/// <summary>
	/// Active font size.
	/// </summary>

	protected int activeFontSize { get { return (trueTypeFont != null || bitmapFont == null) ? fontSize : bitmapFont.defaultSize; } }

	/// <summary>
	/// Font scale applied to the popup list's text.
	/// </summary>

	protected float activeFontScale { get { return (trueTypeFont != null || bitmapFont == null) ? 1f : (float)fontSize / bitmapFont.defaultSize; } }

	/// <summary>
	/// Set the current selection.
	/// </summary>

	public void Set (string value, bool notify = true)
	{
		if (mSelectedItem != value)
		{
			mSelectedItem = value;
			if (mSelectedItem == null) return;
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (notify && mSelectedItem != null)
				TriggerCallbacks();

			if (!keepValue) mSelectedItem = null;
		}
	}

	/// <summary>
	/// Clear the popup list's contents.
	/// </summary>

	public virtual void Clear ()
	{
		items.Clear();
		itemData.Clear();
	}

	/// <summary>
	/// Add a new item to the popup list.
	/// </summary>

	public virtual void AddItem (string text)
	{
		items.Add(text);
		itemData.Add(null);
	}

	/// <summary>
	/// Add a new item to the popup list.
	/// </summary>

	public virtual void AddItem (string text, object data)
	{
		items.Add(text);
		itemData.Add(data);
	}

	/// <summary>
	/// Remove the specified item.
	/// </summary>

	public virtual void RemoveItem (string text)
	{
		int index = items.IndexOf(text);

		if (index != -1)
		{
			items.RemoveAt(index);
			itemData.RemoveAt(index);
		}
	}

	/// <summary>
	/// Remove the specified item.
	/// </summary>

	public virtual void RemoveItemByData (object data)
	{
		int index = itemData.IndexOf(data);

		if (index != -1)
		{
			items.RemoveAt(index);
			itemData.RemoveAt(index);
		}
	}

	[System.NonSerialized] protected bool mExecuting = false;

	/// <summary>
	/// Trigger all event notification callbacks.
	/// </summary>

	protected void TriggerCallbacks ()
	{
		if (!mExecuting)
		{
			mExecuting = true;
			UIPopupList old = current;
			current = this;

			// Legacy functionality
			if (mLegacyEvent != null) mLegacyEvent(mSelectedItem);

			if (EventDelegate.IsValid(onChange))
			{
				EventDelegate.Execute(onChange);
			}
			else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
			{
				// Legacy functionality support (for backwards compatibility)
				eventReceiver.SendMessage(functionName, mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			current = old;
			mExecuting = false;
		}
	}

	/// <summary>
	/// Remove legacy functionality.
	/// </summary>

	protected virtual void OnEnable ()
	{
		if (EventDelegate.IsValid(onChange))
		{
			eventReceiver = null;
			functionName = null;
		}

		// 'font' is no longer used
		if (font != null)
		{
			if (font.isDynamic)
			{
				trueTypeFont = font.dynamicFont;
				fontStyle = font.dynamicFontStyle;
				mUseDynamicFont = true;
			}
			else if (bitmapFont == null)
			{
				bitmapFont = font;
				mUseDynamicFont = false;
			}
			font = null;
		}

		// 'textScale' is no longer used
		if (textScale != 0f)
		{
			fontSize = (bitmapFont != null) ? Mathf.RoundToInt(bitmapFont.defaultSize * textScale) : 16;
			textScale = 0f;
		}

		// Auto-upgrade to the true type font
		if (trueTypeFont == null && bitmapFont != null && bitmapFont.isDynamic)
		{
			trueTypeFont = bitmapFont.dynamicFont;
			bitmapFont = null;
		}
	}

	protected bool mUseDynamicFont = false;

	protected virtual void OnValidate ()
	{
		Font ttf = trueTypeFont;
		UIFont fnt = bitmapFont;

		bitmapFont = null;
		trueTypeFont = null;

		if (ttf != null && (fnt == null || !mUseDynamicFont))
		{
			bitmapFont = null;
			trueTypeFont = ttf;
			mUseDynamicFont = true;
		}
		else if (fnt != null)
		{
			// Auto-upgrade from 3.0.2 and earlier
			if (fnt.isDynamic)
			{
				trueTypeFont = fnt.dynamicFont;
				fontStyle = fnt.dynamicFontStyle;
				fontSize = fnt.defaultSize;
				mUseDynamicFont = true;
			}
			else
			{
				bitmapFont = fnt;
				mUseDynamicFont = false;
			}
		}
		else
		{
			trueTypeFont = ttf;
			mUseDynamicFont = true;
		}
	}

	[System.NonSerialized] protected bool mStarted = false;

	/// <summary>
	/// Send out the selection message on start.
	/// </summary>

	public virtual void Start ()
	{
		if (mStarted) return;
		mStarted = true;

		if (keepValue)
		{
			var sel = mSelectedItem;
			mSelectedItem = null;
			value = sel;
		}
		else mSelectedItem = null;

		// Auto-upgrade legacy functionality
		if (textLabel != null)
		{
			EventDelegate.Add(onChange, textLabel.SetCurrentSelection);
			textLabel = null;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

		// Automatically choose the first item
		// Removed: This triggers callbacks, which messes with popup setting the input field's value,
		// with input field set to auto-save its value.
		//if (Application.isPlaying)
		//{
		//    if (string.IsNullOrEmpty(mSelectedItem) && items.Count > 0)
		//        mSelectedItem = items[0];
		//    if (!string.IsNullOrEmpty(mSelectedItem))
		//        TriggerCallbacks();
		//}
	}

	/// <summary>
	/// Localize the text label.
	/// </summary>

	protected virtual void OnLocalize () { if (isLocalized) TriggerCallbacks(); }

	/// <summary>
	/// Visibly highlight the specified transform by moving the highlight sprite to be over it.
	/// </summary>

	protected virtual void Highlight (UILabel lbl, bool instant)
	{
		if (mHighlight != null)
		{
			mHighlightedLabel = lbl;

			Vector3 pos = GetHighlightPosition();

			if (!instant && isAnimated)
			{
				TweenPosition.Begin(mHighlight.gameObject, 0.1f, pos).method = UITweener.Method.EaseOut;

				if (!mTweening)
				{
					mTweening = true;
					StartCoroutine("UpdateTweenPosition");
				}
			}
			else mHighlight.cachedTransform.localPosition = pos;
		}
	}

	/// <summary>
	/// Helper function that calculates where the tweened position should be.
	/// </summary>

	protected virtual Vector3 GetHighlightPosition ()
	{
		if (mHighlightedLabel == null || mHighlight == null) return Vector3.zero;
		
		Vector4 border = mHighlight.border;
		float scaleFactor = (atlas != null) ? atlas.pixelSize : 1f;
		float offsetX = border.x * scaleFactor;
		float offsetY = border.w * scaleFactor;
		return mHighlightedLabel.cachedTransform.localPosition + new Vector3(-offsetX, offsetY, 1f);
	}

	protected bool mTweening = false;

	/// <summary>
	/// Periodically update the tweened target position.
	/// It's needed because the popup list animates into view, and the target position changes.
	/// </summary>

	protected virtual IEnumerator UpdateTweenPosition ()
	{
		if (mHighlight != null && mHighlightedLabel != null)
		{
			TweenPosition tp = mHighlight.GetComponent<TweenPosition>();
			
			while (tp != null && tp.enabled)
			{
				tp.to = GetHighlightPosition();
				yield return null;
			}
		}
		mTweening = false;
	}

	/// <summary>
	/// Event function triggered when the mouse hovers over an item.
	/// </summary>

	protected virtual void OnItemHover (GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel lbl = go.GetComponent<UILabel>();
			Highlight(lbl, false);
		}
	}

	/// <summary>
	/// Event function triggered when the drop-down list item gets clicked on.
	/// </summary>

	protected virtual void OnItemPress (GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			Select(go.GetComponent<UILabel>(), true);

			UIEventListener listener = go.GetComponent<UIEventListener>();
			value = listener.parameter as string;
			UIPlaySound[] sounds = GetComponents<UIPlaySound>();

			for (int i = 0, imax = sounds.Length; i < imax; ++i)
			{
				UIPlaySound snd = sounds[i];
				if (snd.trigger == UIPlaySound.Trigger.OnClick)
					NGUITools.PlaySound(snd.audioClip, snd.volume, 1f);
			}
			CloseSelf();
		}
	}

	/// <summary>
	/// Select the specified label.
	/// </summary>

	void Select (UILabel lbl, bool instant) { Highlight(lbl, instant); }

	/// <summary>
	/// React to key-based input.
	/// </summary>

	protected virtual void OnNavigate (KeyCode key)
	{
		if (enabled && current == this)
		{
			int index = mLabelList.IndexOf(mHighlightedLabel);
			if (index == -1) index = 0;

			if (key == KeyCode.UpArrow)
			{
				if (index > 0)
				{
					Select(mLabelList[--index], false);
				}
			}
			else if (key == KeyCode.DownArrow)
			{
				if (index + 1 < mLabelList.Count)
				{
					Select(mLabelList[++index], false);
				}
			}
		}
	}

	/// <summary>
	/// React to key-based input.
	/// </summary>

	protected virtual void OnKey (KeyCode key)
	{
		if (enabled && current == this)
		{
			if (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1)
				OnSelect(false);
		}
	}

	/// <summary>
	/// Close the popup list when disabled.
	/// </summary>

	protected virtual void OnDisable () { CloseSelf(); }

	/// <summary>
	/// Get rid of the popup dialog when the selection gets lost.
	/// </summary>

	protected virtual void OnSelect (bool isSelected) { if (!isSelected) CloseSelf(); }

	/// <summary>
	/// Manually close the popup list.
	/// </summary>

	static public void Close ()
	{
		if (current != null)
		{
			current.CloseSelf();
			current = null;
		}
	}

	/// <summary>
	/// Manually close the popup list.
	/// </summary>

	public virtual void CloseSelf ()
	{
		if (mChild != null && current == this)
		{
			StopCoroutine("CloseIfUnselected");
			mSelection = null;

			mLabelList.Clear();

			if (isAnimated)
			{
				UIWidget[] widgets = mChild.GetComponentsInChildren<UIWidget>();

				for (int i = 0, imax = widgets.Length; i < imax; ++i)
				{
					UIWidget w = widgets[i];
					Color c = w.color;
					c.a = 0f;
					TweenColor.Begin(w.gameObject, animSpeed, c).method = UITweener.Method.EaseOut;
				}

				Collider[] cols = mChild.GetComponentsInChildren<Collider>();
				for (int i = 0, imax = cols.Length; i < imax; ++i) cols[i].enabled = false;
				Destroy(mChild, animSpeed);

				mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, animSpeed);
			}
			else
			{
				Destroy(mChild);
				mFadeOutComplete = Time.unscaledTime + 0.1f;
			}

			mBackground = null;
			mHighlight = null;
			mChild = null;
			current = null;
		}
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly fade in.
	/// </summary>

	protected virtual void AnimateColor (UIWidget widget)
	{
		Color c = widget.color;
		widget.color = new Color(c.r, c.g, c.b, 0f);
		TweenColor.Begin(widget.gameObject, animSpeed, c).method = UITweener.Method.EaseOut;
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly move into position.
	/// </summary>

	protected virtual void AnimatePosition (UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 target = widget.cachedTransform.localPosition;
		Vector3 start = placeAbove ? new Vector3(target.x, bottom, target.z) : new Vector3(target.x, 0f, target.z);

		widget.cachedTransform.localPosition = start;

		GameObject go = widget.gameObject;
		TweenPosition.Begin(go, animSpeed, target).method = UITweener.Method.EaseOut;
	}

	/// <summary>
	/// Helper function that causes the widget to smoothly grow until it reaches its original size.
	/// </summary>

	protected virtual void AnimateScale (UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject go = widget.gameObject;
		Transform t = widget.cachedTransform;

		float minHeight = activeFontSize * activeFontScale + mBgBorder * 2f;
		t.localScale = new Vector3(1f, minHeight / widget.height, 1f);
		TweenScale.Begin(go, animSpeed, Vector3.one).method = UITweener.Method.EaseOut;

		if (placeAbove)
		{
			Vector3 pos = t.localPosition;
			t.localPosition = new Vector3(pos.x, pos.y - widget.height + minHeight, pos.z);
			TweenPosition.Begin(go, animSpeed, pos).method = UITweener.Method.EaseOut;
		}
	}

	/// <summary>
	/// Helper function used to animate widgets.
	/// </summary>

	protected void Animate (UIWidget widget, bool placeAbove, float bottom)
	{
		AnimateColor(widget);
		AnimatePosition(widget, placeAbove, bottom);
	}

	/// <summary>
	/// Display the drop-down list when the game object gets clicked on.
	/// </summary>

	protected virtual void OnClick ()
	{
		if (mOpenFrame == Time.frameCount) return;

		if (mChild == null)
		{
			if (openOn == OpenOn.DoubleClick || openOn == OpenOn.Manual) return;
			if (openOn == OpenOn.RightClick && UICamera.currentTouchID != -2) return;
			Show();
		}
		else if (mHighlightedLabel != null)
		{
			OnItemPress(mHighlightedLabel.gameObject, true);
		}
	}

	/// <summary>
	/// Show the popup list on double-click.
	/// </summary>

	protected virtual void OnDoubleClick () { if (openOn == OpenOn.DoubleClick) Show(); }

	/// <summary>
	/// Used to keep an eye on the selected object, closing the popup if it changes.
	/// </summary>

	IEnumerator CloseIfUnselected ()
	{
		for (; ; )
		{
			yield return null;

			if (UICamera.selectedObject != mSelection)
			{
				CloseSelf();
				break;
			}
		}
	}

	public GameObject source;

	/// <summary>
	/// Show the popup list dialog.
	/// </summary>

	public virtual void Show ()
	{
		if (enabled && NGUITools.GetActive(gameObject) && mChild == null && isValid && items.Count > 0)
		{
			mLabelList.Clear();
			StopCoroutine("CloseIfUnselected");

			// Ensure the popup's source has the selection
			UICamera.selectedObject = (UICamera.hoveredObject ?? gameObject);
			mSelection = UICamera.selectedObject;
			source = UICamera.selectedObject;

			if (source == null)
			{
				Debug.LogError("Popup list needs a source object...");
				return;
			}

			mOpenFrame = Time.frameCount;

			// Automatically locate the panel responsible for this object
			if (mPanel == null)
			{
				mPanel = UIPanel.Find(transform);
				if (mPanel == null) return;
			}

			// Calculate the dimensions of the object triggering the popup list so we can position it below it
			Vector3 min;
			Vector3 max;

			// Create the root object for the list
			mChild = new GameObject("Drop-down List");
			mChild.layer = gameObject.layer;

			if (separatePanel)
			{
				if (GetComponent<Collider>() != null)
				{
					Rigidbody rb = mChild.AddComponent<Rigidbody>();
					rb.isKinematic = true;
				}
				else if (GetComponent<Collider2D>() != null)
				{
					Rigidbody2D rb = mChild.AddComponent<Rigidbody2D>();
					rb.isKinematic = true;
				}
				
				var panel = mChild.AddComponent<UIPanel>();
				panel.depth = 1000000;
				panel.sortingOrder = mPanel.sortingOrder;
			}
			current = this;

			Transform t = mChild.transform;
			t.parent = mPanel.cachedTransform;

			// Manually triggered popup list on some other game object
			if (openOn == OpenOn.Manual && mSelection != gameObject)
			{
				startingPosition = UICamera.lastEventPosition;
				min = mPanel.cachedTransform.InverseTransformPoint(mPanel.anchorCamera.ScreenToWorldPoint(startingPosition));
				max = min;
				t.localPosition = min;
				startingPosition = t.position;
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, transform, false, false);
				min = bounds.min;
				max = bounds.max;
				t.localPosition = min;
				startingPosition = t.position;
			}

			StartCoroutine("CloseIfUnselected");

			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;

			int depth = separatePanel ? 0 : NGUITools.CalculateNextDepth(mPanel.gameObject);

			// Add a sprite for the background
			if (background2DSprite != null)
			{
				UI2DSprite sp2 = mChild.AddWidget<UI2DSprite>(depth);
				sp2.sprite2D = background2DSprite;
				mBackground = sp2;
			}
			else if (atlas != null) mBackground = NGUITools.AddSprite(mChild, atlas, backgroundSprite, depth);
			else return;

			bool placeAbove = (position == Position.Above);

			if (position == Position.Auto)
			{
				UICamera cam = UICamera.FindCameraForLayer(mSelection.layer);

				if (cam != null)
				{
					Vector3 viewPos = cam.cachedCamera.WorldToViewportPoint(startingPosition);
					placeAbove = (viewPos.y < 0.5f);
				}
			}

			mBackground.pivot = UIWidget.Pivot.TopLeft;
			mBackground.color = backgroundColor;

			// We need to know the size of the background sprite for padding purposes
			Vector4 bgPadding = mBackground.border;
			mBgBorder = bgPadding.y;
			mBackground.cachedTransform.localPosition = new Vector3(0f, placeAbove ? bgPadding.y * 2f - overlap : overlap, 0f);

			// Add a sprite used for the selection
			if (highlight2DSprite != null)
			{
				UI2DSprite sp2 = mChild.AddWidget<UI2DSprite>(++depth);
				sp2.sprite2D = highlight2DSprite;
				mHighlight = sp2;
			}
			else if (atlas != null) mHighlight = NGUITools.AddSprite(mChild, atlas, highlightSprite, ++depth);
			else return;

			float hlspHeight = 0f, hlspLeft = 0f;

			if (mHighlight.hasBorder)
			{
				hlspHeight = mHighlight.border.w;
				hlspLeft = mHighlight.border.x;
			}

			mHighlight.pivot = UIWidget.Pivot.TopLeft;
			mHighlight.color = highlightColor;

			float fontHeight = activeFontSize;
			float dynScale = activeFontScale;
			float labelHeight = fontHeight * dynScale;
			float lineHeight = labelHeight + padding.y;
			float x = 0f, y = placeAbove ? bgPadding.y - padding.y - overlap : -padding.y - bgPadding.y + overlap;
			float contentHeight = bgPadding.y * 2f + padding.y;
			List<UILabel> labels = new List<UILabel>();

			// Clear the selection if it's no longer present
			if (!items.Contains(mSelectedItem))
				mSelectedItem = null;

			// Run through all items and create labels for each one
			for (int i = 0, imax = items.Count; i < imax; ++i)
			{
				string s = items[i];

				UILabel lbl = NGUITools.AddWidget<UILabel>(mChild, mBackground.depth + 2);
				lbl.name = i.ToString();
				lbl.pivot = UIWidget.Pivot.TopLeft;
				lbl.bitmapFont = bitmapFont;
				lbl.trueTypeFont = trueTypeFont;
				lbl.fontSize = fontSize;
				lbl.fontStyle = fontStyle;
				lbl.text = isLocalized ? Localization.Get(s) : s;
				lbl.color = textColor;
				lbl.cachedTransform.localPosition = new Vector3(bgPadding.x + padding.x - lbl.pivotOffset.x, y, -1f);
				lbl.overflowMethod = UILabel.Overflow.ResizeFreely;
				lbl.alignment = alignment;
				labels.Add(lbl);

				contentHeight += lineHeight;

				y -= lineHeight;
				x = Mathf.Max(x, lbl.printedSize.x);

				// Add an event listener
				UIEventListener listener = UIEventListener.Get(lbl.gameObject);
				listener.onHover = OnItemHover;
				listener.onPress = OnItemPress;
				listener.parameter = s;

				// Move the selection here if this is the right label
				if (mSelectedItem == s || (i == 0 && string.IsNullOrEmpty(mSelectedItem)))
					Highlight(lbl, true);

				// Add this label to the list
				mLabelList.Add(lbl);
			}

			// The triggering widget's width should be the minimum allowed width
			x = Mathf.Max(x, (max.x - min.x) - (bgPadding.x + padding.x) * 2f);

			float cx = x;
			Vector3 bcCenter = new Vector3(cx * 0.5f, -labelHeight * 0.5f, 0f);
			Vector3 bcSize = new Vector3(cx, (labelHeight + padding.y), 1f);

			// Run through all labels and add colliders
			for (int i = 0, imax = labels.Count; i < imax; ++i)
			{
				UILabel lbl = labels[i];
				NGUITools.AddWidgetCollider(lbl.gameObject);
				lbl.autoResizeBoxCollider = false;
				BoxCollider bc = lbl.GetComponent<BoxCollider>();

				if (bc != null)
				{
					bcCenter.z = bc.center.z;
					bc.center = bcCenter;
					bc.size = bcSize;
				}
				else
				{
					BoxCollider2D b2d = lbl.GetComponent<BoxCollider2D>();
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
					b2d.center = bcCenter;
#else
					b2d.offset = bcCenter;
#endif
					b2d.size = bcSize;
				}
			}

			int lblWidth = Mathf.RoundToInt(x);
			x += (bgPadding.x + padding.x) * 2f;
			y -= bgPadding.y;

			// Scale the background sprite to envelop the entire set of items
			mBackground.width = Mathf.RoundToInt(x);
			mBackground.height = Mathf.RoundToInt(contentHeight);

			// Set the label width to make alignment work
			for (int i = 0, imax = labels.Count; i < imax; ++i)
			{
				UILabel lbl = labels[i];
				lbl.overflowMethod = UILabel.Overflow.ShrinkContent;
				lbl.width = lblWidth;
			}

			// Scale the highlight sprite to envelop a single item
			float scaleFactor = (atlas != null) ? 2f * atlas.pixelSize : 2f;
			float w = x - (bgPadding.x + padding.x) * 2f + hlspLeft * scaleFactor;
			float h = labelHeight + hlspHeight * scaleFactor;
			mHighlight.width = Mathf.RoundToInt(w);
			mHighlight.height = Mathf.RoundToInt(h);

			// If the list should be animated, let's animate it by expanding it
			if (isAnimated)
			{
				AnimateColor(mBackground);

				if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
				{
					float bottom = y + labelHeight;
					Animate(mHighlight, placeAbove, bottom);
					for (int i = 0, imax = labels.Count; i < imax; ++i)
						Animate(labels[i], placeAbove, bottom);
					AnimateScale(mBackground, placeAbove, bottom);
				}
			}

			// If we need to place the popup list above the item, we need to reposition everything by the size of the list
			if (placeAbove)
			{
				min.y = max.y - bgPadding.y;
				max.y = min.y + mBackground.height;
				max.x = min.x + mBackground.width;
				t.localPosition = new Vector3(min.x, max.y - bgPadding.y, min.z);
			}
			else
			{
				max.y = min.y + bgPadding.y;
				min.y = max.y - mBackground.height;
				max.x = min.x + mBackground.width;
			}

			Transform pt = mPanel.cachedTransform.parent;

			if (pt != null)
			{
				min = mPanel.cachedTransform.TransformPoint(min);
				max = mPanel.cachedTransform.TransformPoint(max);
				min = pt.InverseTransformPoint(min);
				max = pt.InverseTransformPoint(max);
			}

			// Ensure that everything fits into the panel's visible range
			Vector3 offset = mPanel.hasClipping ? Vector3.zero : mPanel.CalculateConstrainOffset(min, max);
			Vector3 pos = t.localPosition + offset;
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			t.localPosition = pos;
		}
		else OnSelect(false);
	}
}
