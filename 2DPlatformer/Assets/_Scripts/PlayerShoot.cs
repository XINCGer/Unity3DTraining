using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour 
{
	public Rigidbody2D rocket;
	public float speed = 20f;
	private PlayerController playerCtrl;
	private Animator anim;
	
	void Awake()
	{
		anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.GetComponent<PlayerController>();
	}
	
	void Update ()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			anim.SetTrigger("Shoot");

			if(playerCtrl.facingRight)
			{
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
		}
	}

}
