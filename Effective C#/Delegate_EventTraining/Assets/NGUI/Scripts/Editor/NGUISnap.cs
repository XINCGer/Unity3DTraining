//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Utility class that makes it easy to perform snapping while dragging widgets.
/// </summary>

static public class NGUISnap
{
	const float SNAP_THRESHOLD = 10f;

	static BetterList<Vector3> mSnapCenter = new BetterList<Vector3>();
	static BetterList<Vector3> mSnapCorners = new BetterList<Vector3>();

	static int mSnapping = -1;

	/// <summary>
	/// Whether widgets will snap to edges of other widgets when dragged around.
	/// </summary>

	static public bool allow
	{
		get
		{
			if (mSnapping == -1)
			{
				mSnapping = UnityEditor.EditorPrefs.GetInt("NGUI Snap", 1);
			}
			return (mSnapping == 1);
		}
		set
		{
			int val = value ? 1 : 0;

			if (mSnapping != val)
			{
				mSnapping = val;
				UnityEditor.EditorPrefs.SetInt("NGUI Handles", mSnapping);
			}
		}
	}

	/// <summary>
	/// Recalculate all snapping edges.
	/// </summary>

	static public void Recalculate (Object obj)
	{
		mSnapCenter.Clear();
		mSnapCorners.Clear();

		if (obj is UIWidget)
		{
			UIWidget w = obj as UIWidget;
			Recalculate(w.cachedTransform);
		}
		else if (obj is UIPanel)
		{
			UIPanel p = obj as UIPanel;
			Recalculate(p.cachedTransform);
		}
	}

	/// <summary>
	/// Recalculate all snapping edges.
	/// </summary>

	static void Recalculate (Transform t)
	{
		// If the transform is rotated, ignore it
		if (Vector3.Dot(t.localRotation * Vector3.up, Vector3.up) < 0.999f) return;

		Transform parent = t.parent;

		if (parent != null)
		{
			Add(t, parent);

			for (int i = 0; i < parent.childCount; ++i)
			{
			    Transform child = parent.GetChild(i);
				if (child != t) Add(t, child);
			}
		}
	}

	/// <summary>
	/// Add the specified transform's edges to the lists.
	/// </summary>

	static void Add (Transform root, Transform child)
	{
		UIWidget w = child.GetComponent<UIWidget>();
		if (w != null) Add(root, child, w.localCorners);

		UIPanel p = child.GetComponent<UIPanel>();
		if (p != null) Add(root, child, p.localCorners);
	}

	/// <summary>
	/// Add the specified transform's edges to the list.
	/// </summary>

	static void Add (Transform root, Transform child, Vector3[] local)
	{
		// If the transform is rotated, ignore it
		if (Vector3.Dot(child.localRotation * Vector3.forward, Vector3.forward) < 0.999f) return;

		// Make the coordinates relative to 'mine' transform
		if (root != child)
		{
			for (int i = 0; i < 4; ++i)
			{
				local[i] = root.InverseTransformPoint(child.TransformPoint(local[i]));
			}
		}

		Vector3 pos = root.localPosition;
		mSnapCenter.Add(pos + (local[0] + local[2]) * 0.5f);
		mSnapCorners.Add(pos + local[0]);
		mSnapCorners.Add(pos + local[2]);
	}

	/// <summary>
	/// Snap the X coordinate using the previously calculated snapping edges.
	/// </summary>

	static public Vector3 Snap (Vector3 pos, Vector3[] local, bool snapToEdges)
	{
		if (snapToEdges && allow)
		{
			Vector3 center = pos + (local[0] + local[2]) * 0.5f;
			Vector3 bl = pos + local[0];
			Vector3 tr = pos + local[2];
			Vector2 best = new Vector2(float.MaxValue, float.MaxValue);

			for (int i = 0; i < mSnapCenter.size; ++i)
				ChooseBest(ref best, mSnapCenter[i] - center);

			for (int i = 0; i < mSnapCorners.size; ++i)
			{
				ChooseBest(ref best, mSnapCorners[i] - bl);
				ChooseBest(ref best, mSnapCorners[i] - tr);
			}

			if (Mathf.Abs(best.x) < SNAP_THRESHOLD) pos.x += best.x;
			if (Mathf.Abs(best.y) < SNAP_THRESHOLD) pos.y += best.y;
		}
		
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		pos.z = Mathf.Round(pos.z);
		return pos;
	}

	/// <summary>
	/// Choose the closest edge.
	/// </summary>

	static void ChooseBest (ref Vector2 best, Vector3 diff)
	{
		if (Mathf.Abs(best.x) > Mathf.Abs(diff.x)) best.x = diff.x;
		if (Mathf.Abs(best.y) > Mathf.Abs(diff.y)) best.y = diff.y;
	}
}
