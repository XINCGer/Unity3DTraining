using UnityEngine;
using System.Collections;

/// <summary>
/// Collects log
/// </summary>

public class CollectLog : MonoBehaviour {
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "Player")
		{
			Destroy(gameObject);			
			collider.gameObject.GetComponent<Player>().hasLog = true;
		}
	}
}
