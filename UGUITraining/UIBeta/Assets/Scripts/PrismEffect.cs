using UnityEngine;
using UnityEngine.UI;

public class PrismEffect : MonoBehaviour
{
	[Range(-10, 10)]
	public float rate = 1f;

	Graphic mWidget;
	float mDefault = 0f;

	void OnEnable ()
	{
		mWidget = GetComponent<Graphic>();
		
		if (mWidget)
			mDefault = mWidget.material.GetFloat("_Focus");
	}

	void OnDisable ()
	{
		if (mWidget)
			mWidget.material.SetFloat("_Focus", mDefault);
	}

	void Update ()
	{
		float focus = Mathf.Sin(Time.time * rate) * 100f;

		if (mWidget)
			mWidget.material.SetFloat("_Focus", focus);
	}
}
