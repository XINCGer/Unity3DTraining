using UnityEngine;
using System.Collections;

public class MyIK : MonoBehaviour 
{
	public Transform bodyObj = null;
	public Transform leftFootObj = null;
	public Transform rightFootObj = null;
	public Transform leftHandObj = null;
	public Transform rightHandObj = null;
	public Transform lookAtObj = null;
	
	private Animator avatar;
	private bool ikActive = false;

	void Start () 
	{
		avatar = GetComponent<Animator>();	
	}
	
	void Update () 
	{
		if(!ikActive)
		{
			if(bodyObj != null)
			{
				bodyObj.position = avatar.bodyPosition;
				bodyObj.rotation = avatar.bodyRotation;
			}				
			
			if(leftFootObj != null)
			{
				leftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
				leftFootObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
			}				
			
			if(rightFootObj != null)
			{
				rightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
				rightFootObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
			}				
			
			if(leftHandObj != null)
			{
				leftHandObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);
				leftHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
			}				
			
			if(rightHandObj != null)
			{
				rightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
				rightHandObj.rotation  = avatar.GetIKRotation(AvatarIKGoal.RightHand);
			}				
			
			if(lookAtObj != null)
			{
				lookAtObj.position = avatar.bodyPosition + avatar.bodyRotation * new Vector3(0,0.5f,1);
			}
		}

	}

	void OnAnimatorIK(int layerIndex)
	{
		if(avatar == null)
			return;
		
		if(ikActive)
		{
			avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
			avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
			
			avatar.SetLookAtWeight(1.0f, 0.3f, 0.6f, 1.0f, 0.5f);
			
			if(bodyObj != null)
			{
				avatar.bodyPosition = bodyObj.position;
				avatar.bodyRotation = bodyObj.rotation;
			}				
			
			if(leftFootObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootObj.position);
				avatar.SetIKRotation(AvatarIKGoal.LeftFoot,leftFootObj.rotation);
			}				
			
			if(rightFootObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.RightFoot,rightFootObj.position);
				avatar.SetIKRotation(AvatarIKGoal.RightFoot,rightFootObj.rotation);
			}				
			
			if(leftHandObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.LeftHand,leftHandObj.position);
				avatar.SetIKRotation(AvatarIKGoal.LeftHand,leftHandObj.rotation);
			}				
			
			if(rightHandObj != null)
			{
				avatar.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
				avatar.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
			}				
			
			if(lookAtObj != null)
			{
				avatar.SetLookAtPosition(lookAtObj.position);
			}				
		}
		else
		{
			avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot,0);
			avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot,0);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot,0);
			avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot,0);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
			avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);
			
			avatar.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
			avatar.SetIKRotationWeight(AvatarIKGoal.RightHand,0);
			
			avatar.SetLookAtWeight(0.0f);
		}
	}

	void OnGUI()
	{
		GUILayout.Label("激活IK然后在场景中移动Effector对象观察效果");
		ikActive = GUILayout.Toggle(ikActive, "激活IK");
	}
}
