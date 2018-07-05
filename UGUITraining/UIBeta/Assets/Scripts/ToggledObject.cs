using UnityEngine;
using UnityEngine.UI;

public class ToggledObject : MonoBehaviour
{
	public void OnToggle (Toggle tg)
	{
		gameObject.SetActive(tg.isOn);
	}
}
