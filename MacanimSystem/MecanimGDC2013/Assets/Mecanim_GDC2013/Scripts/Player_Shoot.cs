using UnityEngine;
using System.Collections;

/// <summary>
/// Makes player shoot bubbles
/// </summary>
/// 
public class Player_Shoot : MonoBehaviour {
	
	public GameObject Bullet;
	public Transform BulletSpawnPoint;
	public Transform BulletParent;
	
	const float m_BulletSpeed = 20.0f;
	const float m_BulletDuration = 2.0f;
	float m_Timer = 0;
	
	Animator m_Animator;	
	
	void Start () 
	{
		m_Animator = GetComponent<Animator>();	
	}
	
	// Update is called once per frame
	void Update () 
	{		
		bool shoot = Input.GetButton("Fire1");
		m_Animator.SetBool("Shoot",shoot);

		if(CanShoot() && shoot)
		{			
			if(m_Timer > 0.1f) // firing rate
			{
				GameObject newBullet = Instantiate(Bullet, BulletSpawnPoint.position , Quaternion.Euler(0, 0, 0)) as GameObject;		  										
				Destroy(newBullet, m_BulletDuration);										
				newBullet.GetComponent<Rigidbody>().velocity = -BulletSpawnPoint.forward* m_BulletSpeed;						
				newBullet.GetComponent<DamageProvider>().SetScaleBullet(); 
				newBullet.SetActive(true);
			
				if(BulletParent) newBullet.transform.parent = BulletParent;
				
				m_Timer = 0;
			}
		}	
				
		m_Timer += Time.deltaTime;
	}
	
	bool CanShoot()
	{
		AnimatorStateInfo info = m_Animator.GetCurrentAnimatorStateInfo(0);
		
		if(info.IsName("Base Layer.Death") || info.IsName("Base Layer.Reviving") || info.IsName("Base Layer.Dying"))
			return false;
			
		return m_Animator.GetBool("HoldLog");
		
	}
}
