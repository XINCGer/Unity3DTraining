using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour
{
	public bool destroyOnAwake;			// Whether or not this gameobject should destroyed after a delay, on Awake.
	public float awakeDestroyDelay;		// The delay for destroying it on Awake.
	public bool findChild = false;				// Find a child game object and delete it
	public string namedChild;			// Name the child object in Inspector


	void Awake ()
	{
		// If the gameobject should be destroyed on awake,
		if(destroyOnAwake)
		{
			if(findChild)
			{
				Destroy (transform.Find(namedChild).gameObject);
			}
			else
			{
				// ... destroy the gameobject after the delay.
				Destroy(gameObject, awakeDestroyDelay);
			}

		}

	}

	void DestroyChildGameObject ()
	{
		// Destroy this child gameobject, this can be called from an Animation Event.
		if(transform.Find(namedChild).gameObject != null)
			Destroy (transform.Find(namedChild).gameObject);
	}

	void DisableChildGameObject ()
	{
		// Destroy this child gameobject, this can be called from an Animation Event.
		if(transform.Find(namedChild).gameObject.activeSelf == true)
			transform.Find(namedChild).gameObject.SetActive(false);
	}

	void DestroyGameObject ()
	{
		// Destroy this gameobject, this can be called from an Animation Event.
		Destroy (gameObject);
	}
}
