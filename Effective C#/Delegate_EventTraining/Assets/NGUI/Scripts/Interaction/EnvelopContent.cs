//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This script is capable of resizing the widget it's attached to in order to
/// completely envelop targeted UI content.
/// </summary>

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Interaction/Envelop Content")]
public class EnvelopContent : MonoBehaviour
{
	public Transform targetRoot;
	public int padLeft = 0;
	public int padRight = 0;
	public int padBottom = 0;
	public int padTop = 0;

	bool mStarted = false;

	void Start ()
	{
		mStarted = true;
		Execute();
	}

	void OnEnable () { if (mStarted) Execute(); }

	[ContextMenu("Execute")]
	public void Execute ()
	{
		if (targetRoot == transform)
		{
			Debug.LogError("Target Root object cannot be the same object that has Envelop Content. Make it a sibling instead.", this);
		}
		else if (NGUITools.IsChild(targetRoot, transform))
		{
			Debug.LogError("Target Root object should not be a parent of Envelop Content. Make it a sibling instead.", this);
		}
		else
		{
			Bounds b = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, targetRoot, false);
			float x0 = b.min.x + padLeft;
			float y0 = b.min.y + padBottom;
			float x1 = b.max.x + padRight;
			float y1 = b.max.y + padTop;

			UIWidget w = GetComponent<UIWidget>();
			w.SetRect(x0, y0, x1 - x0, y1 - y0);
			BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
		}
	}
}
