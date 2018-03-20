using UnityEngine;
using System.Collections;

public class MyAgent : MonoBehaviour 
{
	public GameObject particle;
	protected NavMeshAgent agent;
	protected Animator animator;
	
	protected MyLocomotion locomotion;
	protected Object particleClone;

	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		
		animator = GetComponent<Animator>();
		locomotion = new MyLocomotion(animator);
		
		particleClone = null;
	}

	protected void SetDestination()
	{
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			// 如果粒子特效实例已经存在，则先销毁之
			if (particleClone != null)
			{
				GameObject.Destroy(particleClone);
				particleClone = null;
			}
			
			Quaternion q = new Quaternion();
			q.SetLookRotation(hit.normal, Vector3.forward);
			particleClone = Instantiate(particle, hit.point, q);
			
			agent.destination = hit.point;
		}
	}

	protected bool AgentStopping()
	{
		return agent.remainingDistance <= agent.stoppingDistance;
	}

	protected bool AgentDone()
	{
		return !agent.pathPending && AgentStopping();
	}

	void OnAnimatorMove()
	{
		agent.velocity = animator.deltaPosition / Time.deltaTime;
		transform.rotation = animator.rootRotation;
	}

	protected void SetupAgentLocomotion()
	{
		if (AgentDone())
		{
			locomotion.Do(0, 0);
			if (particleClone != null)
			{
				GameObject.Destroy(particleClone);
				particleClone = null;
			}
		}
		else
		{
			float speed = agent.desiredVelocity.magnitude;
			Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;
			float angle = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
			locomotion.Do(speed, angle);
		}
	}

	void Update () 
	{
		if (Input.GetButtonDown ("Fire1")) 
			SetDestination();
		
		SetupAgentLocomotion();
	}

	void OnGUI()
	{
		GUILayout.Label("按鼠标左键选择目标点，然后Teddy熊会寻找过去。");
	}

}
