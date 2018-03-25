using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

/// <summary>
/// IK Pass in the 3rd layer, for bazooka/log holding
/// </summary>
public class Bazooka : MonoBehaviour {
		
	public Transform hollowLog;
	public Transform leftHandle;
	public Transform rightHandle;
	
	private Animator m_Animator;
	
	
	void Start () 
	{
        m_Animator = GetComponent<Animator>();        
	}
		
	void Update () 
	{		
		m_Animator.SetLayerWeight(2,1);
		
		bool isHolding = m_Animator.GetCurrentAnimatorStateInfo(2).IsName("HoldLog.HoldLog");
		if(hollowLog != null)
		{
			hollowLog.localScale = isHolding ? new Vector3(0.2f,0.2f,0.4f) : new Vector3(0.001f,0.001f,0.001f);
		}
	}
	
    void OnAnimatorIK(int layerIndex)
    {
		if(!enabled) return;
						
        if (layerIndex == 2) // do the log holding on the last layer, since LookAt is done in previous layer
        {
			float ikWeight =  m_Animator.GetCurrentAnimatorStateInfo(2).IsName("HoldLog.HoldLog") ? 1 : 0;
			
            if (leftHandle != null)
            {
                m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandle.transform.position);
                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandle.transform.rotation);
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            }

            if (rightHandle != null)
            {
                m_Animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandle.transform.position);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandle.transform.rotation);
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            }
        }
    }
}
