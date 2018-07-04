using UnityEngine;

namespace UnityProject
{
	public class PlayerMovement : MonoBehaviour
	{
		public float speed = 6f;
		
		Vector3 movement;
		Animator anim;
		Rigidbody rb;
		int floorMask;
		float camRayLength = 100f;

		void Awake()
		{
			floorMask = LayerMask.GetMask("Floor");
			anim = GetComponent<Animator>();
			rb = GetComponent<Rigidbody>();
		}

		void FixedUpdate ()
		{
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");

			Move(h, v);
			Turning ();
			Animating(h, v);
		}

		void Move (float h, float v)
		{
			movement.Set (h, 0f, v);
			movement = movement.normalized * speed * Time.deltaTime;
			rb.MovePosition (transform.position + movement);
		}

		void Turning ()
		{
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit floorHit;
			if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
			{
				Vector3 playerToMouse = floorHit.point - transform.position;
				playerToMouse.y = 0f;
				Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
				rb.MoveRotation (newRotation);
			}
		}

		void Animating (float h, float v)
		{
			bool walking = h != 0f || v != 0f;
			anim.SetBool ("IsWalking", walking);
		}
	}
}
