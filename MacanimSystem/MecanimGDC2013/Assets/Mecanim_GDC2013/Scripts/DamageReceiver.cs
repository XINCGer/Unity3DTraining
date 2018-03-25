using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]

/// <summary>
/// Damage receiver for the player
/// </summary>
public class DamageReceiver : MonoBehaviour 
{		
	const float m_WounderDampTime = 0.15f;
	
	Animator m_Animator;	
	float m_Damage;
	
	void Start()
	{		
		m_Animator = GetComponent<Animator>();        
		m_Damage = 0;
	}
	
	void Update()
	{
		m_Animator.SetFloat("Wounded",m_Damage, m_WounderDampTime, Time.deltaTime);	
		
		float wounded = m_Animator.GetFloat("Wounded"); // to get the damped value
		m_Animator.SetLayerWeight(1,Mathf.Clamp01(wounded));
			
		AnimatorStateInfo info = m_Animator.GetCurrentAnimatorStateInfo(0);
			
		if(info.IsName("Base Layer.Dying"))
		{			
			m_Animator.SetBool("Dead",true);
		}
		else if(info.IsName("Base Layer.Reviving"))
		{			
			m_Animator.SetBool("Dead",false);
		}
		else if(info.IsName ("Base Layer.Death") && info.normalizedTime > 3)
		{	
			m_Damage = 0;			
		}
	}
	
	public void DoDamage(float damage)
	{	
		m_Damage += damage;		
	}
}
