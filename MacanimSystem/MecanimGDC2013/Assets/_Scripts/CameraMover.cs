using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public Transform follow;
	public float distanceAway;			
	public float distanceUp;			
	public float smooth;				

	private Vector3 targetPosition;		

	void LateUpdate ()
	{
		targetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
		
		transform.LookAt(follow);
	}
}
