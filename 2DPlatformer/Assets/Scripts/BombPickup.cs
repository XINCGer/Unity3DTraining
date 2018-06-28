using UnityEngine;
using System.Collections;

public class BombPickup : MonoBehaviour
{
	public AudioClip pickupClip;		// Sound for when the bomb crate is picked up.


	private Animator anim;				// Reference to the animator component.
	private bool landed = false;		// Whether or not the crate has landed yet.


	void Awake()
	{
		// Setting up the reference.
		anim = transform.root.GetComponent<Animator>();
	}


	void OnTriggerEnter2D (Collider2D other)
	{
		// If the player enters the trigger zone...
		if(other.tag == "Player")
		{
			// ... play the pickup sound effect.
			AudioSource.PlayClipAtPoint(pickupClip, transform.position);

			// Increase the number of bombs the player has.
			other.GetComponent<LayBombs>().bombCount++;

			// Destroy the crate.
			Destroy(transform.root.gameObject);
		}
		// Otherwise if the crate lands on the ground...
		else if(other.tag == "ground" && !landed)
		{
			// ... set the animator trigger parameter Land.
			anim.SetTrigger("Land");
			transform.parent = null;
			gameObject.AddComponent<Rigidbody2D>();
			landed = true;		
		}
	}
}
