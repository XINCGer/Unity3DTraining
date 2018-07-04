using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
	public Transform target;
	public float smoothing = 5f;
	
	Vector3 offset;

	void Start()
	{
		offset = transform.position - target.position;
	}

	void FixedUpdate()
	{
		Vector3 camPos = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, camPos, smoothing * Time.deltaTime);
	}
}
