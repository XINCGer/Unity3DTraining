using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private Animator m_Animator;
	private MyLocomotion m_Locomotion = null;
	private float m_Speed = 0;	
	private float m_Direction = 0;
	public bool hasLog = false;

	void Start () 
	{
		m_Animator = GetComponent<Animator>();        
		m_Animator.logWarnings = false; 
		m_Locomotion = new MyLocomotion(m_Animator);
	}

	void Update ()  
	{
		if (m_Animator && Camera.main)
		{
			Vector3  worldDir;	        
			float horizontal = Input.GetAxis("Horizontal");
			float vertical = Input.GetAxis("Vertical");		
			Vector3 dir = new Vector3(horizontal, 0, vertical);                        		        
			worldDir = Camera.main.transform.rotation * dir; // Converts joystick input in Worldspace coordinates
			worldDir.y = 0; // Kill Z
			worldDir.Normalize();

			float speed = Mathf.Clamp(worldDir.magnitude, 0, 1);
			float direction = 0.0f;
			if (speed > 0.01f) // dead zone
			{
				Vector3 axis = Vector3.Cross(transform.forward, worldDir);
				direction = Vector3.Angle(transform.forward, worldDir) / 180.0f * (axis.y < 0 ? -1 : 1);
			}
			m_Speed = speed;
			m_Direction = direction;
			//JoystickToWorld.ComputeSpeedDirection(transform,ref m_Speed, ref m_Direction);		
		}
		
		m_Locomotion.Do(m_Speed * 6, m_Direction * 180);
		m_Animator.SetBool("HoldLog", hasLog);
	}
}
