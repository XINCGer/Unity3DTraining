//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Example script showing how to activate or deactivate MonoBehaviours with a toggle.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UIToggle))]
[AddComponentMenu("NGUI/Interaction/Toggled Components")]
public class UIToggledComponents : MonoBehaviour
{
	public List<MonoBehaviour> activate;
	public List<MonoBehaviour> deactivate;

	// Deprecated functionality
	[HideInInspector][SerializeField] MonoBehaviour target;
	[HideInInspector][SerializeField] bool inverse = false;

	void Awake ()
	{
		// Legacy functionality -- auto-upgrade
		if (target != null)
		{
			if (activate.Count == 0 && deactivate.Count == 0)
			{
				if (inverse) deactivate.Add(target);
				else activate.Add(target);
			}
			else target = null;

#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

#if UNITY_EDITOR
		if (!Application.isPlaying) return;
#endif
		UIToggle toggle = GetComponent<UIToggle>();
		EventDelegate.Add(toggle.onChange, Toggle);
	}

	public void Toggle ()
	{
		if (enabled)
		{
			for (int i = 0; i < activate.Count; ++i)
			{
				MonoBehaviour comp = activate[i];
				comp.enabled = UIToggle.current.value;
			}

			for (int i = 0; i < deactivate.Count; ++i)
			{
				MonoBehaviour comp = deactivate[i];
				comp.enabled = !UIToggle.current.value;
			}
		}
	}
}
