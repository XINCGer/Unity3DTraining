using UnityEngine;
using System.Collections;

public class MyBear : MonoBehaviour 
{
	public float AvatarRange = 25;
	
	private Animator avatar;
	private float SpeedDampTime = .25f;	
	private float DirectionDampTime = .25f;	
	private Vector3 TargetPosition = new Vector3(0,0,0);

	void Start () 
	{
		avatar = GetComponent<Animator>();
	}

	void Update () 
	{
		if(avatar == null) return;
		
		int r = Random.Range(0, 50);
		avatar.SetBool("Jump", r == 20);
		avatar.SetBool("Dive", r == 30);
		//print("animator.rootPosition == transform.position is " + (avatar.rootPosition == transform.position).ToString());		
		if(Vector3.Distance(TargetPosition, avatar.rootPosition) > 5)
		{
			avatar.SetFloat("Speed", 1, SpeedDampTime, Time.deltaTime);
			Vector3 curentDir = avatar.rootRotation * Vector3.forward;
			Vector3 wantedDir = (TargetPosition - avatar.rootPosition).normalized;
			
			if(Vector3.Dot(curentDir,wantedDir) > 0)
				avatar.SetFloat("Direction",
				                  Vector3.Cross(curentDir,wantedDir).y, 
				                  DirectionDampTime, Time.deltaTime);
			else
				avatar.SetFloat("Direction", 
				                  Vector3.Cross(curentDir,wantedDir).y > 0 ? 1 : -1, 
				                  DirectionDampTime, Time.deltaTime);
		}
		else
		{
			avatar.SetFloat("Speed", 0, SpeedDampTime, Time.deltaTime);
			if(avatar.GetFloat("Speed") < 0.01f)
				TargetPosition = new Vector3(
					Random.Range(-AvatarRange,AvatarRange), 0, 
					Random.Range(-AvatarRange,AvatarRange));
		}	

		var nextState = avatar.GetNextAnimatorStateInfo(0);
		if (nextState.IsName("Base Layer.Dying"))
		{
			avatar.SetBool("Dying", false);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (avatar != null)
		{
			AnimatorStateInfo currentState = avatar.GetCurrentAnimatorStateInfo(0);
			AnimatorStateInfo nextState = avatar.GetNextAnimatorStateInfo(0);
			if (!currentState.IsName("Base Layer.Dying") && !nextState.IsName("Base Layer.Dying"))
				avatar.SetBool("Dying", true);
		}        
	}
}
