using UnityEngine;
using System.Collections;

public class NPCPatrol : MonoBehaviour 
{
	public Transform[] WayPoints;
	
	const float m_MaxSpeed = 3;
	const float m_SpeedDampTime = .25f;	
	const float m_DirectionDampTime = .25f;		
	const float m_ThresholdDistance = 1.5f;
	
	int 		m_WayPointIndex = 0;	
	Animator  	m_Animator = null;	
	
	void Start () 
	{
		m_Animator = GetComponent<Animator>();		
	}

	void Update () 
	{
		if(WayPoints.Length > 0)
		{			
			Transform target = WayPoints[m_WayPointIndex];
			if (target)
			{			
				if(Vector3.Distance(target.position, m_Animator.rootPosition) > m_ThresholdDistance)
				{
					m_Animator.SetFloat("Speed", m_MaxSpeed, m_SpeedDampTime, Time.deltaTime);
					
					Vector3 curentDir = m_Animator.rootRotation * Vector3.forward;
					Vector3 wantedDir = (target.position - m_Animator.rootPosition).normalized;
					
					if(Vector3.Dot(curentDir,wantedDir) > 0) //方向相同
					{
						m_Animator.SetFloat("Direction",Vector3.Cross(curentDir,wantedDir).y,m_DirectionDampTime, Time.deltaTime);
					}
					else //方向不同
					{
						m_Animator.SetFloat("Direction", Vector3.Cross(curentDir,wantedDir).y > 0 ? 1 : -1, m_DirectionDampTime, Time.deltaTime);
					}
				}
				else
				{
					m_Animator.SetFloat("Speed",0,m_SpeedDampTime, Time.deltaTime);
					m_WayPointIndex = (m_WayPointIndex+1) % WayPoints.Length;
				}
			}		
		}			
	}
}
