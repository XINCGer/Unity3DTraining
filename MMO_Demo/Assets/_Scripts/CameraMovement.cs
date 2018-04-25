using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
	public Collider target;
	public Camera camera;
	public LayerMask obstacleLayers = -1;
	public LayerMask groundLayers = -1;
	public float groundedCheckOffset = 0.7f;
	public float rotationUpdateSpeed = 60.0f;
	public float lookUpSpeed = 20.0f;
	public float distanceUpdateSpeed = 10.0f;
	public float followUpdateSpeed = 10.0f;
	public float maxForwardAngle = 80.0f;
	public float minDistance = 0.1f;
	public float maxDistance = 10.0f;
	public float zoomSpeed = 1.0f;
	public bool requireLock = true;
	public bool controlLock = true;
	
	private const float movementThreshold = 0.1f;
	private const float rotationThreshold = 0.1f;
	private const float groundedDistance = 0.5f;
	private Vector3 lastStationaryPosition;
	private float optimalDistance;
	private float targetDistance;
	private bool grounded = false;

	void Start ()
	{
		if (target == null)
			target = GetComponent<Collider>();
		if (camera == null && Camera.main != null)
			camera = Camera.main;

		if (target == null)
		{
			Debug.LogError ("target未赋值.");
			enabled = false;
			return;
		}
		if (camera == null)
		{
			Debug.LogError ("camera未赋值.");
			enabled = false;
			return;
		}
		
		lastStationaryPosition = target.transform.position;
		targetDistance = optimalDistance = (camera.transform.position - target.transform.position).magnitude;
	}

	void Update ()
	{
		optimalDistance = Mathf.Clamp (
			optimalDistance + Input.GetAxis ("Mouse ScrollWheel") * -zoomSpeed * Time.deltaTime,
			minDistance,
			maxDistance
			);
	}

	float ViewRadius
	{
		get
		{
			float fieldOfViewRadius = (optimalDistance * Mathf.Tan(camera.fieldOfView / 2.0f) * Mathf.Deg2Rad);
			float doubleCharacterRadius = Mathf.Max (target.bounds.extents.x, target.bounds.extents.z) * 2.0f;
			
			return Mathf.Min (doubleCharacterRadius, fieldOfViewRadius);
		}
	}
	
	Vector3 SnappedCameraForward
	{
		get
		{
			Vector3 f = camera.transform.forward;
			Vector2 planeForward = new Vector2 (f.x, f.z);
			planeForward = new Vector2 (target.transform.forward.x, target.transform.forward.z).normalized * planeForward.magnitude;
			return new Vector3 (planeForward.x, f.y, planeForward.y);
		}
	}

	void FixedUpdate ()
	{
		grounded = Physics.Raycast (camera.transform.position + target.transform.up * -groundedCheckOffset, target.transform.up * -1, groundedDistance, groundLayers);

		Vector3 inverseLineOfSight = camera.transform.position -  
			target.transform.position;
		
		RaycastHit hit;
		if (Physics.SphereCast (target.transform.position, 
		                        ViewRadius, inverseLineOfSight, 
		                        out hit, optimalDistance, obstacleLayers))
		{
			targetDistance = Mathf.Min ((hit.point - target.transform.position).magnitude, optimalDistance);
		}
		else
			targetDistance = optimalDistance;
	}

	void FollowUpdate ()
	{
		Vector3 cameraForward = target.transform.position - camera.transform.position;
		cameraForward = new Vector3 (cameraForward.x, 0.0f, cameraForward.z);
		float rotationAmount = Vector3.Angle (cameraForward, target.transform.forward);
		
		if (rotationAmount < rotationThreshold)
			lastStationaryPosition = target.transform.position;

		rotationAmount *= followUpdateSpeed * Time.deltaTime;
		
		if (Vector3.Angle (cameraForward, target.transform.right) < Vector3.Angle (cameraForward, target.transform.right * -1.0f))
			rotationAmount *= -1.0f;
		
		camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
	}

	void FreeUpdate ()
	{
		float rotationAmount;
		
		if (Input.GetMouseButton (1))
			FollowUpdate ();
		else
		{
			rotationAmount = Input.GetAxis ("Mouse X") * rotationUpdateSpeed * Time.deltaTime;
			camera.transform.RotateAround (target.transform.position, Vector3.up, rotationAmount);
		}
		
		rotationAmount = Input.GetAxis ("Mouse Y") * -1.0f * lookUpSpeed * Time.deltaTime;
		bool lookFromBelow = Vector3.Angle (camera.transform.forward, target.transform.up * -1) >
			Vector3.Angle (camera.transform.forward, target.transform.up);
		
		if (grounded && lookFromBelow)
			camera.transform.RotateAround (camera.transform.position, camera.transform.right, rotationAmount);
		else
		{
			camera.transform.RotateAround (target.transform.position, camera.transform.right, rotationAmount);
			camera.transform.LookAt (target.transform.position);
			
			float forwardAngle = Vector3.Angle (target.transform.forward, SnappedCameraForward);
			
			if (forwardAngle > maxForwardAngle)
				camera.transform.RotateAround (	target.transform.position,
					camera.transform.right,
					lookFromBelow ? forwardAngle - maxForwardAngle : maxForwardAngle - forwardAngle
					);
		}
	}

	void DistanceUpdate ()
	{
		Vector3 targetPosition = target.transform.position + (camera.transform.position - target.transform.position).normalized * targetDistance;
		camera.transform.position = Vector3.Lerp (camera.transform.position, targetPosition, Time.deltaTime * distanceUpdateSpeed);
	}

	void LateUpdate ()
	{
		if ((Input.GetMouseButton (0) || Input.GetMouseButton (1)) &&
			(!requireLock || controlLock || Screen.lockCursor))
		{
			if (controlLock)
				Screen.lockCursor = true;

			FreeUpdate ();
			lastStationaryPosition = target.transform.position;
		}
		else
		{
			if (controlLock)
				Screen.lockCursor = false;

			Vector3 movement = target.transform.position - lastStationaryPosition;
			if (new Vector2 (movement.x, movement.z).magnitude > movementThreshold)
				FollowUpdate ();
		}
		
		DistanceUpdate ();
	}

}
