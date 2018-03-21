using UnityEngine;
using System.Collections;

public class TargetMatching : MonoBehaviour
{

	private Animator animator;
	public Transform RightHand;
	bool hasJumped = false;


	// Use this for initialization
	void Start () {

		animator = GetComponent<Animator>();
	
	}

	void OnGUI()
	{
		GUILayout.Label("Make your character grab any edge!");
		GUILayout.Label("Press Fire1 to trigger Jump animation");
		GUILayout.Label("Added *MatchStart* and *MatchEnd* parameters to the Animator Controller");
		GUILayout.Label("Added *MatchStart* and *MatchEnd* additionnal curves to the Idle_ToJumpUpHigh animation");
		GUILayout.Label("*MatchStart* and *MatchEnd* tell the system when to start cheating the root using MatchTarget API");
		GUILayout.Label("Added a RightHand model child of the MoveThis container, to tell where the right hand should go");
		GUILayout.Label("On the update function, we call MatchTarget");
		GUILayout.Label("Translate the MoveThis object and see how character's hand always reach it");		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (animator)
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

			if (Input.GetButton("Fire1")) animator.SetBool("Jump", true);

			if (state.IsName("Base Layer.JumpUp") || state.IsName("Base Layer.FullJump")) 
			{
				animator.SetBool("Jump", false);
								
				animator.MatchTarget(RightHand.position, RightHand.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask(new Vector3(1, 1, 1), 0), animator.GetFloat("MatchStart"), animator.GetFloat("MatchEnd"));
				hasJumped = true;
			}

			if (hasJumped && state.normalizedTime > 1.2)
			{
				hasJumped = false;
				Application.LoadLevel(0);
			}
		}
	
	}
}
