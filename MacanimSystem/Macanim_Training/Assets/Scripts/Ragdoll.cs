using UnityEngine;
using System.Collections;

public class Ragdoll : MonoBehaviour 
{

	protected Animator animator;
	public float DirectionDampTime = .25f;	
	
	public GameObject ragdoll;
	Transform[] hierarchy;
	Transform[] ragdollHierarchy;
	Vector3[] localT;
	Quaternion[] localQ;


	Rigidbody[] ragdollRB;

	private float blendWeight = 0;
	
	// Use this for initialization
	void Start () 
	{		
		animator = GetComponent<Animator>();
		
		hierarchy = GetComponentsInChildren<Transform>();
		ragdollHierarchy = ragdoll.GetComponentsInChildren<Transform>();

		ragdollRB = ragdoll.GetComponentsInChildren<Rigidbody>();
	
		localT = new Vector3[hierarchy.Length];
		localQ = new Quaternion[hierarchy.Length];
	}
		
	// Update is called once per frame
	void Update () 
	{

		if (animator)
		{
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);			

			if (stateInfo.IsName("Base Layer.Run"))
			{
				if (Input.GetButton("Fire1")) animator.SetBool("Dive", true);                
            }
			else
			{
				animator.SetBool("Dive", false);                
            }

			if(Input.GetButtonDown("Fire2"))
			{
				if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Ragdoll"))
					animator.SetBool("ExitRagdoll", true);
				else
					animator.SetBool("EnterRagdoll", true);
				
			}
			
		
      		float h = Input.GetAxis("Horizontal");
        	float v = Input.GetAxis("Vertical");
			
			animator.SetFloat("Speed", h*h+v*v);
            animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);	
		}   		  
	}


	void OnAnimatorIK(int layerIndex)
	{
		if (!enabled)
			return;

		if (layerIndex == 0)
		{
			blendWeight = 0;
			if (( animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).IsName("Base Layer.Ragdoll") ) || (!animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Ragdoll")))
			{
				animator.SetBool("EnterRagdoll", false);				
				blendWeight = 1;
			}
			else if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Ragdoll"))
			{
				blendWeight = 1 - animator.GetAnimatorTransitionInfo(0).normalizedTime;
				animator.SetBool("ExitRagdoll", false);
			}



			Vector3 velocity = animator.deltaPosition / Time.deltaTime;
			for (int i = 0; i < ragdollRB.Length; ++i)
			{
				if (blendWeight > 0)
				{
					bool wasKinematic = ragdollRB[i].isKinematic;
					ragdollRB[i].isKinematic = false;

					if (wasKinematic) // just turned ragdoll on. Give it some velocity!
						ragdollRB[i].velocity = velocity;
					
				}
				else
					ragdollRB[i].isKinematic = true;
					
			}

			for (int i = 0; i < hierarchy.Length; ++i)
			{
				if (blendWeight > 0)
				{
					localQ[i] = Quaternion.Lerp( hierarchy[i].localRotation,  ragdollHierarchy[i].localRotation, Mathf.Clamp01(blendWeight) );
					localT[i] = Vector3.Lerp( hierarchy[i].localPosition, ragdollHierarchy[i].localPosition,Mathf.Clamp01(blendWeight));
				}
				else
				{
					localT[i] = hierarchy[i].localPosition;
					localQ[i] = hierarchy[i].localRotation;
					ragdollHierarchy[i].localRotation = hierarchy[i].localRotation;
					ragdollHierarchy[i].localPosition = hierarchy[i].localPosition;
				}

			}

		}
	}

	void LateUpdate()
	{
		for (int i = 0; i < hierarchy.Length; ++i)
		{
			hierarchy[i].localPosition = localT[i];
			hierarchy[i].localRotation = localQ[i];			
		}

		if (blendWeight > 0) //  must tell the animator the new position, based on 1st RigidBody
		{			
			animator.rootPosition = new Vector3(ragdollRB[0].transform.position.x, 0, ragdollRB[0].transform.position.z);// project on floor			
		}		
	}


	void OnGUI()
	{
		GUILayout.Label("Simple Integration of Ragdoll and Mecanim");
		GUILayout.Label("Uses duplicated rigs, 1 for Animations and 1 for Ragdoll");
		GUILayout.Label("Rigs are synchronized in OnAnimatorIK in a temporary buffer");
		GUILayout.Label("Temporary buffer writes back to transform on laterUpdate");
		GUILayout.Label("Ik Pass must be active on the base layer for OnAnimatorIK to be called");
		GUILayout.Label("Blending weight of ragdolls/animation is computed based on StateMachine state");
		GUILayout.Label("StateMachine has an AnyState transition to the *Ragdoll* state");	

	}
}

