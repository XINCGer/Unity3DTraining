using UnityEngine;
using System.Collections;

public class CrowdMovement : MonoBehaviour 
{
	public float AvatarRange = 25;
	
	private Animator animator;
	private float SpeedDampTime = .25f; 
	private float DirectionDampTime = .25f; 
	private Vector3 TargetPosition = Vector3.zero;

	void Start () 
	{
		animator = GetComponent<Animator>();	
	}

	void Update () 
	{
		if(animator == null) return;
		
		int r = Random.Range(0, 50);
		animator.SetBool("Jump", r == 20);
		animator.SetBool("Dive", r == 30);
		print("animator.rootPosition == transform.position is " + (animator.rootPosition == transform.position).ToString());		
		if(Vector3.Distance(TargetPosition, animator.rootPosition) > 5)
		{
			animator.SetFloat("Speed", 1, SpeedDampTime, Time.deltaTime);
			Vector3 curentDir = animator.rootRotation * Vector3.forward;
			Vector3 wantedDir = (TargetPosition - animator.rootPosition).normalized;
			
			if(Vector3.Dot(curentDir,wantedDir) > 0)
				animator.SetFloat("Direction",
				                  Vector3.Cross(curentDir,wantedDir).y, 
				                  DirectionDampTime, Time.deltaTime);
			else
				animator.SetFloat("Direction", 
				                  Vector3.Cross(curentDir,wantedDir).y > 0 ? 1 : -1, 
				                  DirectionDampTime, Time.deltaTime);
		}
		else
		{
			animator.SetFloat("Speed", 0, SpeedDampTime, Time.deltaTime);
			if(animator.GetFloat("Speed") < 0.01f)
				TargetPosition = new Vector3(
					Random.Range(-AvatarRange,AvatarRange), 0, 
					Random.Range(-AvatarRange,AvatarRange));
		}
	}
}
