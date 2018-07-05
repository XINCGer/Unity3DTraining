using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour, IPointerClickHandler
{
	void OnEnable ()
	{
	}

	public void SetRed(float value)
	{
		OnValueChanged(value, 0);
	}
	
	public void SetGreen(float value)
	{
		OnValueChanged(value, 1);
	}
	
	public void SetBlue(float value)
	{
		OnValueChanged(value, 2);
	}
	
	public void OnValueChanged(float value, int channel)
	{
		Color c = Color.white;

		if (GetComponent<Renderer>() != null)
			c = GetComponent<Renderer>().material.color;
		else if (GetComponent<Light>() != null)
			c = GetComponent<Light>().color;
		
		c[channel] = value;

		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().material.color = c;
		else if (GetComponent<Light>() != null)
			GetComponent<Light>().color = c;
	}

	public void OnPointerClick(PointerEventData data)
	{
		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
		else if (GetComponent<Light>() != null)
			GetComponent<Light>().color = new Color(Random.value, Random.value, Random.value, 1.0f);
	}
}
