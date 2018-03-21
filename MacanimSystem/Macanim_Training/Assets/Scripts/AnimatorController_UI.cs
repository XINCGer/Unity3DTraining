using UnityEngine;
using System.Collections;

public class AnimatorController_UI : MonoBehaviour {

	void OnGUI()
	{

		GUILayout.Label("Basic Animator Controller, with Additionnal layer for with RightArm body mask");
		GUILayout.Label("Press Fire1 to trigger Jump animation");
		GUILayout.Label("Press Fire2 to trigger Hi animation on 2nd layer");
		GUILayout.Label("Interaction with CharacterController in OnAnimatorMove of IdleRunJump.cs");
	}
		
}
