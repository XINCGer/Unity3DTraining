using UnityEngine;
using System.Collections;

/// <summary>
/// Makes NPC shoot at player
/// </summary>
public class NPC_ShootPlayer : MonoBehaviour {
	
	const float m_AttackDistance = 5;
	const float m_BulletSpeed = 15.0f;
	const float m_BulletDuration = 10.0f;
	
	public bool CheatRoot = false;
	public Transform BulletSpawnPoint;
	public GameObject Bullet;
	
	public Transform BulletParent;
	
	Animator  	m_Animator = null;	
	GameObject 	m_Player = null;	 	
	Animator 	m_PlayerAnimator = null;	
	bool 		m_HasShootInCycle;
	float 		m_PrevStateTime;
	Vector3		m_LookAtPosition = new Vector3();
	
	void Start () 
	{
		m_Animator = GetComponent<Animator>();
		m_Animator.logWarnings = false; // so we dont get warning when updating controller in live link ( undocumented/unsupported function!)
		m_Player = GameObject.FindWithTag ("Player");	
		m_PlayerAnimator = m_Player.GetComponent<Animator>();
	}
		
	void Update ()
	{	
		if(ShouldShootPlayer())
		{			
			m_Animator.SetBool("Shoot", true); // tell animator to shoot
			
			ManageShootCycle();
			
			if(BulletSpawnPoint && !m_HasShootInCycle)
			{								
				if( m_Animator.GetFloat("Throw") > 0.99f )  // additionnal curve in the shoot animation
				{					
					SpawnBullet();							
				}
			}					
		}
		else
		{
			m_Animator.SetBool("Shoot", false);
		}
	}
	
	
	bool ShouldShootPlayer()
	{
		float distanceToPlayer = Vector3.Distance(m_Player.transform.position, transform.position);		
		if(distanceToPlayer < m_AttackDistance)
		{
			AnimatorStateInfo info = m_PlayerAnimator.GetCurrentAnimatorStateInfo(0);
			// real bears don't shoot at dead player!
			if( !info.IsName("Base Layer.Dying") && !info.IsName("Base Layer.Death") && !info.IsName("Base Layer.Reviving"))
			{
				return true;
			}			
		}
		
		return false;		
	}
	
	
	// Makes sure we only shoot once per cycle
	void ManageShootCycle()
	{
		AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
		float time = Mathf.Repeat(stateInfo.normalizedTime,1.0f);
					
		if(time < m_PrevStateTime) // has looped
			m_HasShootInCycle = false;
		
		m_PrevStateTime = time;
	}
		
	void OnAnimatorMove()
	{
		if(CheatRoot)
		{
			if(!enabled || !GetComponent<CharacterController>().enabled) return;
			
			//  Cheat the root to align to player target.
			if(m_Animator.GetBool("Shoot"))
			{
				m_LookAtPosition.Set(m_Player.transform.position.x, transform.position.y, m_Player.transform.position.z); // Kill Y.
				
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_LookAtPosition-transform.position), Time.deltaTime * 5);
				m_Animator.rootRotation =  transform.rotation;
			}							
		
			GetComponent<CharacterController>().Move(m_Animator.deltaPosition);					
			transform.rotation = m_Animator.rootRotation;
		
			ForceOnFloor();		
		}
	}
	
	// Spawns bullet
	void SpawnBullet() 
	{
		GameObject newBullet = Instantiate(Bullet, BulletSpawnPoint.position , Quaternion.Euler(0, 0, 0)) as GameObject;		  										
		Destroy(newBullet, m_BulletDuration);							
		Vector3 direction = m_Player.transform.position - BulletSpawnPoint.position;
		direction.y = 0;
		newBullet.GetComponent<Rigidbody>().velocity = Vector3.Normalize(direction)* m_BulletSpeed;								
		if(BulletParent)newBullet.transform.parent = BulletParent;
		m_HasShootInCycle = true;				
	}
		
	/// Make the NPC always on floor, even when colliding
	void ForceOnFloor()
	{		
		Vector3 position = transform.position;
  		position.y = 0;
  		transform.position = position;
	}
}
