using UnityEngine;
using System.Collections;

/// <summary>
/// Damage receiver for NPC
/// </summary>
public class NPC_DamageReceiver : MonoBehaviour {
	
	Animator m_Animator;
	
	void Start()
	{
		m_Animator = GetComponent<Animator>();
	}
	
	public void DoDamage()
	{	
		// die right away when recieving damade
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SimpleLocomotion"))
			m_Animator.SetBool("Die",true);
	}
	
	void Update()
	{		
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Dying"))
		{
			m_Animator.SetBool("Die",false);
			GetComponent<CharacterController>().enabled = false; // disable collision when dead.
		}
		
		if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Death")) // when dead, character start floating up (rapture!)
		{
			Vector3 position = transform.position;
			position.y += Time.deltaTime;
			transform.position = position;
		}
	}
}
