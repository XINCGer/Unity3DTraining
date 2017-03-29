//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Show or hide the widget based on whether the control scheme is appropriate.
/// </summary>

public class UIShowControlScheme : MonoBehaviour
{
	public GameObject target;
	public bool mouse = false;
	public bool touch = false;
	public bool controller = true;

	void OnEnable () { UICamera.onSchemeChange += OnScheme; OnScheme(); }
	void OnDisable () { UICamera.onSchemeChange -= OnScheme; }

	void OnScheme ()
	{
		if (target != null)
		{
			UICamera.ControlScheme scheme = UICamera.currentScheme;
			if (scheme == UICamera.ControlScheme.Mouse) target.SetActive(mouse);
			else if (scheme == UICamera.ControlScheme.Touch) target.SetActive(touch);
			else if (scheme == UICamera.ControlScheme.Controller) target.SetActive(controller);
		}
	}
}
