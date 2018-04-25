using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
	enum CharacterState
	{
		Normal,
		Jumping,
		Falling,
		Landing
	}

	public Animation target;
	public Rigidbody rigidbody;
	public Transform root, spine, hub;
	public float walkSpeed = 0.2f;
	public float runSpeed = 1.0f;
	public float rotationSpeed = 6.0f;
	public float shuffleSpeed = 7.0f;
	public float runningLandingFactor = 0.2f;

	private PlayerController controller;
	private CharacterState state = CharacterState.Falling;
	private bool canLand = true;
	private float currentRotation;
	private Vector3 lastRootForward;

	private Vector3 HorizontalMovement
	{
		get
		{
			return new Vector3 (rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);
		}
	}

	void Start ()
	{
		if (target == null)
			target = GetComponent<Animation> ();
		if (rigidbody == null)
			rigidbody = GetComponent<Rigidbody> ();

		if (VerifySetups())
		{
			controller = GetComponent<PlayerController>();
			controller.onJump += OnJump;
			currentRotation = 0.0f;
			lastRootForward = root.forward;
		}
	}
	
	bool VerifySetup (Component component, string name)
	{
		if (component == null)
		{
			Debug.LogError ("参数 " + name + " 未赋值.");
			enabled = false;
            return false;
        }
        
        return true;
    }	

	bool VerifySetups()
	{
		return VerifySetup (target, "target") &&
				VerifySetup (rigidbody, "rigidbody") &&
				VerifySetup (root, "root") &&
				VerifySetup (spine, "spine") &&
				VerifySetup (hub, "hub");
	}

	void OnJump ()
	{
		canLand = false;
		state = CharacterState.Jumping;
		
		Invoke ("Fall", target["Jump"].length);
	}

	void OnLand ()
	{
		canLand = false;
		state = CharacterState.Landing;
		
		Invoke ("Land", target["Land"].length * (HorizontalMovement.magnitude < 
		                                         walkSpeed ? 1.0f : runningLandingFactor)
			);
	}

	void Fall ()
	{
		if (controller.Grounded)
			return;
		state = CharacterState.Falling;
	}

	void Land ()
	{
		if (state != CharacterState.Landing)
			return;
		state = CharacterState.Normal;
	}

	void FixedUpdate ()
	{
		if (controller.Grounded)
		{
			if (state == CharacterState.Falling || (state == CharacterState.Jumping && canLand))
				OnLand ();
		}
		else if (state == CharacterState.Jumping)
			canLand = true;
	}

	void Update ()
	{
		switch (state)
		{
		case CharacterState.Normal:
			Vector3 movement = HorizontalMovement; 
			
			if (movement.magnitude < walkSpeed)
			{
				if (Vector3.Angle (lastRootForward, root.forward) > 1.0f)
				{
					target.CrossFade ("Shuffle");
					lastRootForward = Vector3.Slerp (lastRootForward, root.forward, Time.deltaTime * shuffleSpeed);
				}
				else
					target.CrossFade ("Idle");
			}
			else
			{
				target["Walk"].speed = target["Run"].speed =
					Vector3.Angle (root.forward, movement) > 91.0f ? -1.0f : 1.0f;

				if (movement.magnitude < runSpeed)
					target.CrossFade ("Walk");
				else
					target.CrossFade ("Run");

				lastRootForward = root.forward;
			}
			break;
		case CharacterState.Jumping:
			target.CrossFade ("Jump");
			break;
		case CharacterState.Falling:
			target.CrossFade ("Fall");
			break;
		case CharacterState.Landing:
			target.CrossFade ("Land");
			break;
		}
	}
	
	void LateUpdate ()
	{
		float targetAngle = 0.0f;
		Vector3 movement = HorizontalMovement;
		if (movement.magnitude >= walkSpeed)
		{
			targetAngle = Vector3.Angle (movement, new Vector3 (root.forward.x, 0.0f, root.forward.z));
			if (Vector3.Angle (movement, root.right) > Vector3.Angle (movement, root.right * -1))
				targetAngle *= -1.0f;

			if (Mathf.Abs (targetAngle) > 91.0f)
				targetAngle = targetAngle + (targetAngle > 0 ? -180.0f : 180.0f);
		}
		currentRotation = Mathf.Lerp (currentRotation, targetAngle, Time.deltaTime * rotationSpeed);
		hub.RotateAround (hub.position, root.up, currentRotation);
		spine.RotateAround (spine.position, root.up, currentRotation * -1.0f);
	}
}
