using UnityEngine;
using System.Collections;

public class ScaleCapsule : MonoBehaviour {


	private Animator animator;
	private CharacterController characterController;
	private float baseCapsuleHeight;


	public bool applyScale = false;
	public float timeScale = 0.5f;
	bool preventJump = false;
	bool hasJumped = false;


	// Use this for initialization
	void Start () { 

		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
		baseCapsuleHeight = characterController.height;
	 
	}

	void OnGUI()
	{

		GUILayout.Label("Make your character jump trough hoops!");
		GUILayout.Label("Press Fire1 to trigger Dive animation");
		GUILayout.Label("Added *ScaleOffsetCapsule* parameter to the Animator Controller");
		GUILayout.Label("Added *ScaleOffsetCapsule* additionnal curve to Dive animation, that tells when the character is in the air");
		GUILayout.Label("If ApplyScale toggle is on,  *characterController.height = baseCapsuleHeight * (1 + animator.GetFloat(\"ScaleOffsetCapsule\"))* is called");		
		
		bool prevValue = applyScale;
		applyScale = GUILayout.Toggle(applyScale, "Apply Scale");

		if (prevValue != applyScale) preventJump = true;
		else preventJump = false;
			
	}
	
	// Update is called once per frame
	void Update () {

		Time.timeScale = timeScale;

		if (animator)
		{
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

			if (!preventJump && Input.GetButtonUp("Fire1")) animator.SetBool("Jump", true);
		
			if (state.IsName("Base Layer.Dive"))
			{
				animator.SetBool("Jump", false);
				hasJumped = true;
			}
			if (hasJumped && state.IsName("Base Layer.Idle"))
			{
				hasJumped = false;
				Application.LoadLevel(0);
			}
			
			if(applyScale)
				characterController.height = baseCapsuleHeight * (1 + 2*animator.GetFloat("ScaleOffsetCapsule"));
	
		}
	
	}
}
