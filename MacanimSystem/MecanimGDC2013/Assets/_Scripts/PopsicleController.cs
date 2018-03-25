using UnityEngine;
using System.Collections;

public class PopsicleController : MonoBehaviour 
{
	public GameObject NPC;
	
	void OnTriggerEnter(Collider collider)
	{
		if(!enabled) return;
		if(collider.gameObject.tag == "Player")
		{
			Destroy(gameObject);
			NPC.SetActive(true);
		}
	}
}
