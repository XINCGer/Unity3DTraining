using UnityEngine;
using System.Collections;

public class PlayerFall : MonoBehaviour 
{
	void Update () 
	{
		if(transform.position.y < -5)
		{
			transform.position = new Vector3(0, 25, 3);
		}	
	}

}
