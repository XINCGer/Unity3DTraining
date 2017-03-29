//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Attaching this script to a widget makes it react to key events such as tab, up, down, etc.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	/// <summary>
	/// List of all the active UINavigation components.
	/// </summary>

	static public BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	public enum Constraint
	{
		None,
		Vertical,
		Horizontal,
		Explicit,
	}

	/// <summary>
	/// If a selection target is not set, the target can be determined automatically, restricted by this constraint.
	/// 'None' means free movement on both horizontal and vertical axis. 'Explicit' means the automatic logic will
	/// not execute, and only the explicitly set values will be used.
	/// </summary>

	public Constraint constraint = Constraint.None;

	/// <summary>
	/// Which object will be selected when the Up button is pressed.
	/// </summary>

	public GameObject onUp;

	/// <summary>
	/// Which object will be selected when the Down button is pressed.
	/// </summary>

	public GameObject onDown;

	/// <summary>
	/// Which object will be selected when the Left button is pressed.
	/// </summary>

	public GameObject onLeft;

	/// <summary>
	/// Which object will be selected when the Right button is pressed.
	/// </summary>

	public GameObject onRight;

	/// <summary>
	/// Which object will get selected on click.
	/// </summary>

	public GameObject onClick;

	/// <summary>
	/// Which object will get selected on tab.
	/// </summary>

	public GameObject onTab;

	/// <summary>
	/// Whether the object this script is attached to will get selected as soon as this script is enabled.
	/// </summary>

	public bool startsSelected = false;

	/// <summary>
	/// Convenience function that returns the current key navigation selection.
	/// </summary>

	static public UIKeyNavigation current
	{
		get
		{
			GameObject go = UICamera.hoveredObject;
			if (go == null) return null;
			return go.GetComponent<UIKeyNavigation>();
		}
	}

	/// <summary>
	/// Whether the collider is enabled and the widget can be interacted with.
	/// </summary>

	public bool isColliderEnabled
	{
		get
		{
			if (enabled && gameObject.activeInHierarchy)
			{
				Collider c = GetComponent<Collider>();
				if (c != null) return c.enabled;
				Collider2D b = GetComponent<Collider2D>();
				return (b != null && b.enabled);
			}
			return false;
		}
	}

	[System.NonSerialized] bool mStarted = false;

	protected virtual void OnEnable ()
	{
		list.Add(this);
		if (mStarted) Start();
	}

	void Start ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		mStarted = true;
		if (startsSelected && isColliderEnabled)
			UICamera.hoveredObject = gameObject;
	}

	protected virtual void OnDisable () { list.Remove(this); }

	static bool IsActive (GameObject go)
	{
		if (go && go.activeInHierarchy)
		{
			Collider c = go.GetComponent<Collider>();
			if (c != null) return c.enabled;
			Collider2D b = go.GetComponent<Collider2D>();
			return (b != null && b.enabled);
		}
		return false;
	}

	public GameObject GetLeft ()
	{
		if (IsActive(onLeft)) return onLeft;
		if (constraint == Constraint.Vertical || constraint == Constraint.Explicit) return null;
		return Get(Vector3.left, 1f, 2f);
	}

	public GameObject GetRight ()
	{
		if (IsActive(onRight)) return onRight;
		if (constraint == Constraint.Vertical || constraint == Constraint.Explicit) return null;
		return Get(Vector3.right, 1f, 2f);
	}

	public GameObject GetUp ()
	{
		if (IsActive(onUp)) return onUp;
		if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit) return null;
		return Get(Vector3.up, 2f, 1f);
	}

	public GameObject GetDown ()
	{
		if (IsActive(onDown)) return onDown;
		if (constraint == Constraint.Horizontal || constraint == Constraint.Explicit) return null;
		return Get(Vector3.down, 2f, 1f);
	}

	public GameObject Get (Vector3 myDir, float x = 1f, float y = 1f)
	{
		Transform t = transform;
		myDir = t.TransformDirection(myDir);
		Vector3 myCenter = GetCenter(gameObject);
		float min = float.MaxValue;
		GameObject go = null;

		for (int i = 0; i < list.size; ++i)
		{
			UIKeyNavigation nav = list[i];
			if (nav == this || nav.constraint == Constraint.Explicit || !nav.isColliderEnabled) continue;

			// Ignore invisible widgets
			UIWidget widget = nav.GetComponent<UIWidget>();
			if (widget != null && widget.alpha == 0f) continue;

			// Reject objects that are not within a 45 degree angle of the desired direction
			Vector3 dir = GetCenter(nav.gameObject) - myCenter;
			float dot = Vector3.Dot(myDir, dir.normalized);
			if (dot < 0.707f) continue;

			// Exaggerate the movement in the undesired direction
			dir = t.InverseTransformDirection(dir);
			dir.x *= x;
			dir.y *= y;

			// Compare the distance
			float mag = dir.sqrMagnitude;
			if (mag > min) continue;
			go = nav.gameObject;
			min = mag;
		}
		return go;
	}

	static protected Vector3 GetCenter (GameObject go)
	{
		UIWidget w = go.GetComponent<UIWidget>();
		UICamera cam = UICamera.FindCameraForLayer(go.layer);

		if (cam != null)
		{
			Vector3 center = go.transform.position;

			if (w != null)
			{
				Vector3[] corners = w.worldCorners;
				center = (corners[0] + corners[2]) * 0.5f;
			}

			center = cam.cachedCamera.WorldToScreenPoint(center);
			center.z = 0;
			return center;
		}
		else if (w != null)
		{
			Vector3[] corners = w.worldCorners;
			return (corners[0] + corners[2]) * 0.5f;
		}
		return go.transform.position;
	}

	static public int mLastFrame = 0;

	/// <summary>
	/// React to navigation.
	/// </summary>

	public virtual void OnNavigate (KeyCode key)
	{
		if (UIPopupList.isOpen) return;
		if (mLastFrame == Time.frameCount) return;
		mLastFrame = Time.frameCount;

		GameObject go = null;

		switch (key)
		{
			case KeyCode.LeftArrow:		go = GetLeft();		break;
			case KeyCode.RightArrow:	go = GetRight();	break;
			case KeyCode.UpArrow:		go = GetUp();		break;
			case KeyCode.DownArrow:		go = GetDown();		break;
		}

		if (go != null) UICamera.hoveredObject = go;
	}

	/// <summary>
	/// React to any additional keys, such as Tab.
	/// </summary>

	public virtual void OnKey (KeyCode key)
	{
		if (UIPopupList.isOpen) return;
		if (mLastFrame == Time.frameCount) return;
		mLastFrame = Time.frameCount;

		if (key == KeyCode.Tab)
		{
			GameObject go = onTab;

			if (go == null)
			{
				if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
				{
					go = GetLeft();
					if (go == null) go = GetUp();
					if (go == null) go = GetDown();
					if (go == null) go = GetRight();
				}
				else
				{
					go = GetRight();
					if (go == null) go = GetDown();
					if (go == null) go = GetUp();
					if (go == null) go = GetLeft();
				}
			}

			if (go != null)
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.hoveredObject = go;
				UIInput inp = go.GetComponent<UIInput>();
				if (inp != null) inp.isSelected = true;
			}
		}
	}

	protected virtual void OnClick ()
	{
		if (NGUITools.GetActive(onClick))
			UICamera.hoveredObject = onClick;
	}
}
