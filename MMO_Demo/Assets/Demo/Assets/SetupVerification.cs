using UnityEngine;
using System.Collections;

public class SetupVerification : MonoBehaviour
{
	public string message = "";
	
	
	private bool badSetup = false;
	
	
	void Awake ()
	{
		Application.RegisterLogCallback (OnLog);
	}
	

	void OnLog (string message, string stacktrace, LogType type)
	{
		if (message.IndexOf ("UnityException: Input Axis") == 0 ||
			message.IndexOf ("UnityException: Input Button") == 0
		)
		{
			((ThirdPersonController)FindObjectOfType (typeof (ThirdPersonController))).enabled = false;
			badSetup = true;
		}
	}
	
	
	void OnGUI ()
	{
		if (!badSetup)
		{
			return;
		}

		GUILayout.BeginArea (new Rect (0.0f, 0.0f, Screen.width, Screen.height));
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Box (message);
				GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();
		GUILayout.EndArea ();
	}
}
