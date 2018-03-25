using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]

/// <summary>
/// Base player script
/// </summary>
public class Player : MonoBehaviour {
	
    private Animator m_Animator;
    private Locomotion m_Locomotion = null;

	private float m_Speed = 0;	
    private float m_Direction = 0;
	
	public bool hasLog = false;

	void Start () 
	{
        m_Animator = GetComponent<Animator>();        
		m_Animator.logWarnings = false; // so we dont get warning when updating controller in live link ( undocumented/unsupported function!)
	}
    
	void Update ()  
	{
		if(m_Locomotion == null) m_Locomotion = new Locomotion(m_Animator);
		
        if (m_Animator && Camera.main)
		{
            JoystickToWorld.ComputeSpeedDirection(transform,ref m_Speed, ref m_Direction);		
		}
		
		
		m_Locomotion.Do(m_Speed * 6, m_Direction * 180);
		
		m_Animator.SetBool("HoldLog", hasLog);
	}		
	
}
