//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This is a script used to keep the game object scaled to 2/(Screen.height).
/// If you use it, be sure to NOT use UIOrthoCamera at the same time.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Root")]
public class UIRoot : MonoBehaviour
{
	/// <summary>
	/// List of all UIRoots present in the scene.
	/// </summary>

	static public List<UIRoot> list = new List<UIRoot>();

	public enum Scaling
	{
		Flexible,
		Constrained,
		ConstrainedOnMobiles,
	}

	public enum Constraint
	{
		Fit,
		Fill,
		FitWidth,
		FitHeight,
	}

	/// <summary>
	/// Type of scaling used by the UIRoot.
	/// </summary>

	public Scaling scalingStyle = Scaling.Flexible;

	/// <summary>
	/// When the UI scaling is constrained, this controls the type of constraint that further fine-tunes how it's scaled.
	/// </summary>

	public Constraint constraint
	{
		get
		{
			if (fitWidth)
			{
				if (fitHeight) return Constraint.Fit;
				return Constraint.FitWidth;
			}
			else if (fitHeight) return Constraint.FitHeight;
			return Constraint.Fill;
		}
	}

	/// <summary>
	/// Width of the screen, used when the scaling style is set to Flexible.
	/// </summary>

	public int manualWidth = 1280;

	/// <summary>
	/// Height of the screen when the scaling style is set to FixedSize or Flexible.
	/// </summary>

	public int manualHeight = 720;

	/// <summary>
	/// If the screen height goes below this value, it will be as if the scaling style
	/// is set to FixedSize with manualHeight of this value.
	/// </summary>

	public int minimumHeight = 320;

	/// <summary>
	/// If the screen height goes above this value, it will be as if the scaling style
	/// is set to Fixed Height with manualHeight of this value.
	/// </summary>

	public int maximumHeight = 1536;

	/// <summary>
	/// When Constraint is on, controls whether the content must be restricted horizontally to be at least 'manualWidth' wide.
	/// </summary>

	public bool fitWidth = false;

	/// <summary>
	/// When Constraint is on, controls whether the content must be restricted vertically to be at least 'Manual Height' tall.
	/// </summary>

	public bool fitHeight = true;

	/// <summary>
	/// Whether the final value will be adjusted by the device's DPI setting.
	/// Used when the Scaling is set to Pixel-Perfect.
	/// </summary>

	public bool adjustByDPI = false;

	/// <summary>
	/// If set and the game is in portrait mode, the UI will shrink based on the screen's width instead of height.
	/// Used when the Scaling is set to Pixel-Perfect.
	/// </summary>

	public bool shrinkPortraitUI = false;

	/// <summary>
	/// Active scaling type, based on platform.
	/// </summary>

	public Scaling activeScaling
	{
		get
		{
			Scaling scaling = scalingStyle;

			if (scaling == Scaling.ConstrainedOnMobiles)
#if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_WP_8_1 || UNITY_BLACKBERRY
				return Scaling.Constrained;
#else
				return Scaling.Flexible;
#endif
			return scaling;
		}
	}

	/// <summary>
	/// UI Root's active height, based on the size of the screen.
	/// </summary>

	public int activeHeight
	{
		get
		{
			Scaling scaling = activeScaling;

			if (scaling == Scaling.Flexible)
			{
				Vector2 screen = NGUITools.screenSize;
				float aspect = screen.x / screen.y;

				if (screen.y < minimumHeight)
				{
					screen.y = minimumHeight;
					screen.x = screen.y * aspect;
				}
				else if (screen.y > maximumHeight)
				{
					screen.y = maximumHeight;
					screen.x = screen.y * aspect;
				}

				// Portrait mode uses the maximum of width or height to shrink the UI
				int height = Mathf.RoundToInt((shrinkPortraitUI && screen.y > screen.x) ? screen.y / aspect : screen.y);

				// Adjust the final value by the DPI setting
				return adjustByDPI ? NGUIMath.AdjustByDPI(height) : height;
			}
			else
			{
				Constraint cons = constraint;
				if (cons == Constraint.FitHeight)
					return manualHeight;

				Vector2 screen = NGUITools.screenSize;
				float aspect = screen.x / screen.y;
				float initialAspect = (float)manualWidth / manualHeight;

				switch (cons)
				{
					case Constraint.FitWidth:
					{
						return Mathf.RoundToInt(manualWidth / aspect);
					}
					case Constraint.Fit:
					{
						return (initialAspect > aspect) ?
							Mathf.RoundToInt(manualWidth / aspect) :
							manualHeight;
					}
					case Constraint.Fill:
					{
						return (initialAspect < aspect) ?
							Mathf.RoundToInt(manualWidth / aspect) :
							manualHeight;
					}
				}
				return manualHeight;
			}
		}
	}

	/// <summary>
	/// Pixel size adjustment. Most of the time it's at 1, unless the scaling style is set to FixedSize.
	/// </summary>

	public float pixelSizeAdjustment
	{
		get
		{
			int height = Mathf.RoundToInt(NGUITools.screenSize.y);
			return height == -1 ? 1f : GetPixelSizeAdjustment(height);
		}
	}

	/// <summary>
	/// Helper function that figures out the pixel size adjustment for the specified game object.
	/// </summary>

	static public float GetPixelSizeAdjustment (GameObject go)
	{
		UIRoot root = NGUITools.FindInParents<UIRoot>(go);
		return (root != null) ? root.pixelSizeAdjustment : 1f;
	}

	/// <summary>
	/// Calculate the pixel size adjustment at the specified screen height value.
	/// </summary>

	public float GetPixelSizeAdjustment (int height)
	{
		height = Mathf.Max(2, height);

		if (activeScaling == Scaling.Constrained)
			return (float)activeHeight / height;

		if (height < minimumHeight) return (float)minimumHeight / height;
		if (height > maximumHeight) return (float)maximumHeight / height;
		return 1f;
	}

	Transform mTrans;

	protected virtual void Awake () { mTrans = transform; }
	protected virtual void OnEnable () { list.Add(this); }
	protected virtual void OnDisable () { list.Remove(this); }

	protected virtual void Start ()
	{
		UIOrthoCamera oc = GetComponentInChildren<UIOrthoCamera>();

		if (oc != null)
		{
			Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", oc);
			Camera cam = oc.gameObject.GetComponent<Camera>();
			oc.enabled = false;
			if (cam != null) cam.orthographicSize = 1f;
		}
		else UpdateScale(false);
	}

	void Update ()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying && gameObject.layer != 0)
			UnityEditor.EditorPrefs.SetInt("NGUI Layer", gameObject.layer);
#endif
		UpdateScale();
	}

	/// <summary>
	/// Immediately update the root's scale. Call this function after changing the min/max/manual height values.
	/// </summary>

	public void UpdateScale (bool updateAnchors = true)
	{
		if (mTrans != null)
		{
			float calcActiveHeight = activeHeight;

			if (calcActiveHeight > 0f)
			{
				float size = 2f / calcActiveHeight;

				Vector3 ls = mTrans.localScale;

				if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.y - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.z - size) <= float.Epsilon))
				{
					mTrans.localScale = new Vector3(size, size, size);
					if (updateAnchors) BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	/// <summary>
	/// Broadcast the specified message to the entire UI.
	/// </summary>

	static public void Broadcast (string funcName)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
#endif
		{
			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				UIRoot root = list[i];
				if (root != null) root.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	/// <summary>
	/// Broadcast the specified message to the entire UI.
	/// </summary>

	static public void Broadcast (string funcName, object param)
	{
		if (param == null)
		{
			// More on this: http://answers.unity3d.com/questions/55194/suggested-workaround-for-sendmessage-bug.html
			Debug.LogError("SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
		}
		else
		{
			for (int i = 0, imax = list.Count; i < imax; ++i)
			{
				UIRoot root = list[i];
				if (root != null) root.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
