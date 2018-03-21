using UnityEngine;
using System.Collections;

public class FootPlanting : MonoBehaviour {

	protected Animator animator;

	private static int m_SpeedId = Animator.StringToHash("Speed");


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		float v = Input.GetAxis("Vertical");
		animator.SetFloat(m_SpeedId, v);
	}

	protected void FixFoot(AvatarIKGoal footGoal, RaycastHit hit, float weight)
	{
		animator.SetIKPosition(footGoal, hit.point + (hit.normal * 0.02f));
		animator.SetIKPositionWeight(footGoal, weight);

		Quaternion q = animator.GetIKRotation(footGoal);
		Vector3 up = q * Vector3.up;

		q = Quaternion.FromToRotation(up, hit.normal) * q;

		animator.SetIKRotation(footGoal, q);
		animator.SetIKRotationWeight(footGoal, weight);
	}

	protected void SetupFootPlanting()
	{
		float leftWeight = 0;
		float rightWeight = 0;
		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
		AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(0);

		if (state.IsName("Locomotion.Idle") && !animator.IsInTransition(0))
			leftWeight = rightWeight = 1.0f;
		else if (state.IsName("Locomotion.Idle") && animator.IsInTransition(0))
			leftWeight = rightWeight = Mathf.Pow(1.0f - animator.GetAnimatorTransitionInfo(0).normalizedTime, 4);
		else if (animator.IsInTransition(0) && nextState.IsName("Locomotion.Idle"))
			leftWeight = rightWeight = Mathf.Pow(animator.GetAnimatorTransitionInfo(0).normalizedTime, 4);

		Vector3 leftT = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
		Vector3 rightT = animator.GetIKPosition(AvatarIKGoal.RightFoot);

		//float distance = 0;

		RaycastHit leftHit = new RaycastHit();
		if (Physics.Raycast(leftT, -Vector3.up, out leftHit))
			FixFoot(AvatarIKGoal.LeftFoot, leftHit, leftWeight);

		RaycastHit rightHit = new RaycastHit();
		if (Physics.Raycast(rightT, -Vector3.up, out rightHit))
			FixFoot(AvatarIKGoal.RightFoot, rightHit, rightWeight);

		//Vector3 bodyT = animator.bodyPosition;
		//bodyT.y -= distance - 0.14f;
		//animator.bodyPosition = bodyT;
	}


	void OnAnimatorIK(int layerIndex)
	{
		//if (layerIndex == 0)
		{
			SetupFootPlanting();
		}
	}
}
