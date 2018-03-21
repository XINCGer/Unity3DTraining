using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	public GameObject			particle;
	protected UnityEngine.AI.NavMeshAgent		agent;
	protected Animator			animator;

	protected Locomotion locomotion;
	protected Object particleClone;


	// Use this for initialization
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.updateRotation = false;

		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);

		particleClone = null;
	}

	protected void SetDestination()
	{
		// Construct a ray from the current mouse coordinates
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit))
		{
			if (particleClone != null)
			{
				GameObject.Destroy(particleClone);
				particleClone = null;
			}

			// Create a particle if hit
			Quaternion q = new Quaternion();
			q.SetLookRotation(hit.normal, Vector3.forward);
			particleClone = Instantiate(particle, hit.point, q);

			agent.destination = hit.point;
		}
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

			float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;

			locomotion.Do(speed, angle);
		}
	}

    void OnAnimatorMove()
    {
        agent.velocity = animator.deltaPosition / Time.deltaTime;
		transform.rotation = animator.rootRotation;
    }

	protected bool AgentDone()
	{
		return !agent.pathPending && AgentStopping();
	}

	protected bool AgentStopping()
	{
		return agent.remainingDistance <= agent.stoppingDistance;
	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown ("Fire1")) 
			SetDestination();

		SetupAgentLocomotion();
	}
}
