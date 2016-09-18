using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour {

	public float coldTime=2.0f;
	public KeyCode keyCode;
	private Image img;
	private float timer=0f;
	private bool isStartTimer=false;

	// Use this for initialization
	void Start () {
		img = transform.Find ("FillMask").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (keyCode)) {
			isStartTimer=true;
		}
		if (isStartTimer) {
			timer+=Time.deltaTime;
			img.fillAmount=(coldTime-timer)/coldTime;
			if(timer>=coldTime){
				img.fillAmount=0;
				timer=0;
				isStartTimer=false;
			}
		}
	}

	public void OnClick(){
		isStartTimer = true;
	}
}
