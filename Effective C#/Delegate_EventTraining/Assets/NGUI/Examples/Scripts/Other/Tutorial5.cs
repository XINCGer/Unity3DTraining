using UnityEngine;

/// <summary>
/// This simple example script is used in Tutorial 5 to show how custom events work.
/// </summary>

public class Tutorial5 : MonoBehaviour
{
	/// <summary>
	/// This function is called by the duration slider. Since it's called by a slider,
	/// UIProgressBar.current will contain the caller. It's identical to other NGUI components.
	/// A button's callback will set UIButton.current, for example. Input field? UIInput.current, etc.
	/// </summary>

	public void SetDurationToCurrentProgress ()
	{
		UITweener[] tweens = GetComponentsInChildren<UITweener>();
		
		foreach (UITweener tw in tweens)
		{
			// The slider's value is always limited in 0 to 1 range, however it's trivial to change it.
			// For example, to make it range from 2.0 to 0.5 instead, you can do this:
			tw.duration = Mathf.Lerp(2.0f, 0.5f, UIProgressBar.current.value);
		}
	}
}
