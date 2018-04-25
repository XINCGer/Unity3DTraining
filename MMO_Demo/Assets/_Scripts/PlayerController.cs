using UnityEngine;
using System.Collections;

public delegate void MyJumpDelegate ();

public class PlayerController : MonoBehaviour 
{
	public Rigidbody target;
	public float speed = 1.0f;
	public float walkSpeedDownscale = 2.0f;
	public float turnSpeed = 2.0f;
	public float mouseTurnSpeed = 0.3f;
	public float jumpSpeed = 1.0f;
	public LayerMask groundLayers = -1;
	public float groundedCheckOffset = 0.7f;
	public bool showGizmos = true;
	public bool requireLock = true;
	public bool controlLock = false;
	public MyJumpDelegate onJump = null;
	private const float inputThreshold = 0.01f;
	private const float groundDrag = 5.0f;
	private const float directionalJumpFactor = 0.7f;
	private const float groundedDistance = 0.5f;
	private bool grounded;
	private bool walking;

	void Start () 
	{
		if (target == null)
			target = GetComponent<Rigidbody> ();
		if (target == null)
		{
			Debug.LogError ("变量target未赋值");
			enabled = false;
			return;
		}
		target.freezeRotation = true;
		walking = false;
	}
	
	void Update ()
	{
		float rotationAmount;
		
		if (Input.GetMouseButton (1) && (!requireLock || controlLock || Cursor.lockState == CursorLockMode.Locked))
		{
			if (controlLock)
				Cursor.lockState = CursorLockMode.Locked;

			rotationAmount = Input.GetAxis ("Mouse X") * mouseTurnSpeed * Time.deltaTime;
		}
		else
		{
			if (controlLock)
				Cursor.lockState = CursorLockMode.None;

			rotationAmount = Input.GetAxis ("Horizontal") * turnSpeed * Time.deltaTime;
		}
		target.transform.RotateAround(target.transform.up, rotationAmount);
		
		if (Input.GetButtonDown ("ToggleWalk"))
			walking = !walking;
	}

	float SidestepAxisInput
	{
		get
		{
			if (Input.GetMouseButton (1))
			{
				float sidestep = Input.GetAxis ("Sidestep"), horizontal = Input.GetAxis ("Horizontal");
				return Mathf.Abs (sidestep) > Mathf.Abs (horizontal) ? sidestep : horizontal;
			}
			else
				return Input.GetAxis ("Sidestep");
		}
	}

	public bool Grounded
	{
		get { return grounded; }
	}

	void FixedUpdate ()
	{
		grounded = Physics.Raycast (
			target.transform.position + target.transform.up * -groundedCheckOffset,
			target.transform.up * -1,
			groundedDistance,
			groundLayers
			);
		
		if (grounded)
		{
			target.drag = groundDrag;
			if (Input.GetButton ("Jump"))
			{
				target.AddForce (
					jumpSpeed * target.transform.up +
					target.velocity.normalized * directionalJumpFactor,
					ForceMode.VelocityChange
					);
				if (onJump != null)
					onJump ();
			}
			else
			{
				Vector3 movement = Input.GetAxis ("Vertical") * target.transform.forward +
					SidestepAxisInput * target.transform.right;
				float appliedSpeed = walking ? speed / walkSpeedDownscale : speed;
				
				if (Input.GetAxis ("Vertical") < 0.0f)
					appliedSpeed /= walkSpeedDownscale;

				if (movement.magnitude > inputThreshold)
					target.AddForce (movement.normalized * appliedSpeed, ForceMode.VelocityChange);
				else
				{
					target.velocity = new Vector3 (0.0f, target.velocity.y, 0.0f);
					return;
				}
			}
		}
		else
			target.drag = 0.0f;
	}

	void OnDrawGizmos ()
	{
		if (!showGizmos || target == null)
			return;
		Gizmos.color = grounded ? Color.blue : Color.red;
		Vector3 p  = target.transform.position;
		Vector3 a = p + target.transform.up * -groundedCheckOffset;
		Gizmos.DrawLine(a, a + target.transform.up * -groundedDistance);
	}
}
