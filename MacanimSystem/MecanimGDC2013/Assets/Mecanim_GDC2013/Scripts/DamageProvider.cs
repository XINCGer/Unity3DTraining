using UnityEngine;
using System.Collections;

/// <summary>
/// Damage provider when bullet hits Player or NPC
/// </summary>
public class DamageProvider : MonoBehaviour 
{
	const float m_DamageAmount = 0.25f;			
	bool m_ScaleBullet = false;
	
	public void SetScaleBullet()
	{
		m_ScaleBullet  =true;
	}
	
	
	void OnCollisionEnter(Collision collider) 
	{	
		DamageReceiver player = collider.gameObject.GetComponent<DamageReceiver>();
		NPC_DamageReceiver npc = collider.gameObject.GetComponent<NPC_DamageReceiver>();
		if(player)
		{
			player.DoDamage(m_DamageAmount);
		}			
		else if(npc)
		{
			npc.DoDamage(); // NPC dies automatically
		}				
	}
		
	void Update()
	{
		if(m_ScaleBullet && transform.localScale.magnitude < 5 )
		{
			transform.localScale *= 1 + Time.deltaTime / 0.25f;  // makes bullet scale overtime
		}
	}
}
