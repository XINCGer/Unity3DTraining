using UnityEngine;
using System.Collections;

public class GamePauser : MonoBehaviour 
{
	private bool paused = false;
	
	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.P))
		{
			paused = !paused;
		}
		
		if(paused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}
}
