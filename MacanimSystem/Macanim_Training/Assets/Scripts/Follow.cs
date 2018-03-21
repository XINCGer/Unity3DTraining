 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class Follow : MonoBehaviour {
	
	public Transform TargetObj = null;

	protected Animator avatar;
	protected CharacterController controller;
	
	private float SpeedDampTime = .25f;	
	private float DirectionDampTime = .25f;	
	
	// Use this for initialization
	void Start () 
	{
		avatar = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();

		avatar.speed = 1 + UnityEngine.Random.Range(-0.4f, 0.4f);		
	}
    
	void Update () 
	{
		if (avatar && TargetObj)
		{			
			if(Vector3.Distance(TargetObj.position,avatar.rootPosition) > 4)
			{
				avatar.SetFloat("Speed",1,SpeedDampTime, Time.deltaTime);
				
				Vector3 curentDir = avatar.rootRotation * Vector3.forward;
				Vector3 wantedDir = (TargetObj.position - avatar.rootPosition).normalized;
	
				if(Vector3.Dot(curentDir,wantedDir) > 0)
				{
					avatar.SetFloat("Direction",Vector3.Cross(curentDir,wantedDir).y,DirectionDampTime, Time.deltaTime);
				}
				else
				{
            		avatar.SetFloat("Direction", Vector3.Cross(curentDir,wantedDir).y > 0 ? 1 : -1, DirectionDampTime, Time.deltaTime);
				}
			}
			else
			{
            	avatar.SetFloat("Speed",0,SpeedDampTime, Time.deltaTime);
			}
		}		
	}

	void OnAnimatorMove()
	{
		controller.Move(avatar.deltaPosition);
		transform.rotation = avatar.rootRotation;
	}
}
