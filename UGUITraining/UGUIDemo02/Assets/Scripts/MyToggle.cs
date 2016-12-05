using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyToggle : MonoBehaviour
{

    public GameObject IsOnGameObject;
    public GameObject IsOffGameObject;
    private bool _isOn;
    private Toggle _toggle;
	// Use this for initialization
	void Start ()
	{
	    _toggle = this.GetComponent<Toggle>();
	    _isOn = _toggle.isOn;
        OnValueChange(_isOn);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnValueChange(bool isOn)
    {
        IsOnGameObject.SetActive(isOn);
        IsOffGameObject.SetActive(!isOn);
    }
}
