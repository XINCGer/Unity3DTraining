//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Deprecated component. Use UIKeyNavigation instead.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Button Keys (Legacy)")]
public class UIButtonKeys : UIKeyNavigation
{
	public UIButtonKeys selectOnClick;
	public UIButtonKeys selectOnUp;
	public UIButtonKeys selectOnDown;
	public UIButtonKeys selectOnLeft;
	public UIButtonKeys selectOnRight;

	protected override void OnEnable ()
	{
		Upgrade();
		base.OnEnable();
	}

	public void Upgrade ()
	{
		if (onClick == null && selectOnClick != null)
		{
			onClick = selectOnClick.gameObject;
			selectOnClick = null;
			NGUITools.SetDirty(this);
		}

		if (onLeft == null && selectOnLeft != null)
		{
			onLeft = selectOnLeft.gameObject;
			selectOnLeft = null;
			NGUITools.SetDirty(this);
		}

		if (onRight == null && selectOnRight != null)
		{
			onRight = selectOnRight.gameObject;
			selectOnRight = null;
			NGUITools.SetDirty(this);
		}

		if (onUp == null && selectOnUp != null)
		{
			onUp = selectOnUp.gameObject;
			selectOnUp = null;
			NGUITools.SetDirty(this);
		}

		if (onDown == null && selectOnDown != null)
		{
			onDown = selectOnDown.gameObject;
			selectOnDown = null;
			NGUITools.SetDirty(this);
		}
	}
}
