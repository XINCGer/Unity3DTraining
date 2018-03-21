using UnityEngine;
using System.Collections;

public class PreciseTurnOnSpot : MonoBehaviour {


	protected Animator animator;
	float targetTurn = 90;
	bool doTurn = false;

	Quaternion targetRotation;
	
	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void OnGUI()
	{

		GUILayout.Label("Simple example to get precise turn on spot while keeping animation as intact as possible");
		GUILayout.Label("Uses a 'Turn On Spot' BlendTree (in Turn state) in conjunction with Mecanim's MatchTarget call");		
		GUILayout.Label("Details in PreciseTurnOnSpot.cs");


		GUILayout.BeginHorizontal();
		targetTurn = GUILayout.HorizontalSlider(targetTurn, -180, 180);
		GUILayout.Label(targetTurn.ToString());		

		if(GUILayout.Button("Do Turn"))
		{
			doTurn = true;
		}
		GUILayout.EndHorizontal();
		
	}
	
	void Update()
	{

		animator.SetBool("Turn", doTurn);
		animator.SetFloat("Direction", targetTurn);

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle"))
		{
			if (doTurn) // just triggered
			{								
				targetRotation = transform.rotation * Quaternion.AngleAxis(targetTurn, Vector3.up); // Compute target rotation when doTurn is triggered
				doTurn = false;
			}
		}
		else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Turn"))
		{
			// calls MatchTarget when in Turn state, subsequent calls are ignored until targetTime (0.9f) is reached .
			animator.MatchTarget(Vector3.one, targetRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.zero, 1), animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0.9f);			
		}

	}
}
