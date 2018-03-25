using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour 
{
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			Destroy(gameObject);
			collider.gameObject.GetComponent<PlayerController>().hasLog = true;
		}
	}
}
