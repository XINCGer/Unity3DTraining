using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppendCharacter : MonoBehaviour {

	public Text text;

	public void AppendText(string textToAppend)
	{
		text.text += textToAppend;
	}
}
