//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Sample script showing how easy it is to implement a standard button that swaps sprites.
/// </summary>

[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;
	public string normalSprite;
	public string hoverSprite;
	public string pressedSprite;
	public string disabledSprite;
	public bool pixelSnap = true;

	public bool isEnabled
	{
		get
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Collider col = collider;
#else
			Collider col = gameObject.GetComponent<Collider>();
#endif
			return col && col.enabled;
		}
		set
		{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Collider col = collider;
#else
			Collider col = gameObject.GetComponent<Collider>();
#endif
			if (!col) return;

			if (col.enabled != value)
			{
				col.enabled = value;
				UpdateImage();
			}
		}
	}

	void OnEnable ()
	{
		if (target == null) target = GetComponentInChildren<UISprite>();
		UpdateImage();
	}

	void OnValidate ()
	{
		if (target != null)
		{
			if (string.IsNullOrEmpty(normalSprite)) normalSprite = target.spriteName;
			if (string.IsNullOrEmpty(hoverSprite)) hoverSprite = target.spriteName;
			if (string.IsNullOrEmpty(pressedSprite)) pressedSprite = target.spriteName;
			if (string.IsNullOrEmpty(disabledSprite)) disabledSprite = target.spriteName;
		}
	}

	void UpdateImage()
	{
		if (target != null)
		{
			if (isEnabled) SetSprite(UICamera.IsHighlighted(gameObject) ? hoverSprite : normalSprite);
			else SetSprite(disabledSprite);
		}
	}

	void OnHover (bool isOver)
	{
		if (isEnabled && target != null)
			SetSprite(isOver ? hoverSprite : normalSprite);
	}

	void OnPress (bool pressed)
	{
		if (pressed) SetSprite(pressedSprite);
		else UpdateImage();
	}

	void SetSprite (string sprite)
	{
		if (target.atlas == null || target.atlas.GetSprite(sprite) == null) return;
		target.spriteName = sprite;
		if (pixelSnap) target.MakePixelPerfect();
	}
}
