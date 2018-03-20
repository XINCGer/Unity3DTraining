using UnityEngine;
using System.Collections;

public class MyBazooka : MonoBehaviour 
{
	public GameObject targetA = null;
	public Transform leftHandPos = null;
	public Transform rightHandPos = null;
	public GameObject bazoo = null;
	public GameObject bullet = null;
	public Transform spawn = null;
	
	private Animator animator;
	private bool load = false;

	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	void Update () 
	{
		if(animator == null)
			return;
		
		animator.SetFloat("Aim", load ? 1 : 0, .1f, Time.deltaTime);
		float aim = animator.GetFloat("Aim");
		
		if(Input.GetButton("Fire2"))
		{
			if(load && aim > 0.99) { load = false; }
			else if(!load && aim < 0.01) load = true;
		}
		
		//这部分运动逻辑与之前场景中的相同
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		
		animator.SetFloat("Speed", h*h+v*v);
		animator.SetFloat("Direction", h, 0.25f, Time.deltaTime);

		float fire = animator.GetFloat("Fire");
		
		if(Input.GetButton("Fire1") && fire < 0.01 && aim > 0.99)
		{
			animator.SetFloat("Fire",1);
			
			if(bullet != null && spawn != null)
			{
				GameObject newBullet = Instantiate(bullet, spawn.transform.position , Quaternion.Euler(0, 0, 0)) as GameObject;
				Rigidbody rb = newBullet.GetComponent<Rigidbody>();
				if(rb != null)
				{
					rb.velocity = spawn.transform.TransformDirection(Vector3.forward * 20);
				}
			}
		}
		else
		{
			animator.SetFloat("Fire",0, 0.1f, Time.deltaTime);
		}
	}

	void OnAnimatorIK(int layerIndex)
	{
		float aim = animator.GetFloat("Aim");
		
		// 在Base Layer上处理角色瞄准目标对象和火箭抬起与放下的逻辑
		if (layerIndex == 0)
		{
			if (targetA != null)
			{
				Vector3 target = targetA.transform.position;
				target.y = target.y + 0.2f * (target - animator.rootPosition).magnitude;
				animator.SetLookAtPosition(target);
				animator.SetLookAtWeight(aim, 0.5f, 0.5f, 0.0f, 0.5f);
				
				if (bazoo != null)
				{
					float fire = animator.GetFloat("Fire");
					Vector3 pos = new Vector3(0.195f, -0.0557f, -0.155f);
					Vector3 scale = new Vector3(0.2f, 0.8f, 0.2f);
					pos.x -= fire * 0.2f;
					scale = scale * aim;
					bazoo.transform.localScale = scale;
					bazoo.transform.localPosition = pos;
				}
			}
		}

		// 在HandIK层上处理抬火箭时双手举起的逻辑
		if (layerIndex == 1)
		{
			if (leftHandPos != null)
			{
				animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos. position);
				animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPos. rotation);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, aim);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, aim);
			}
			
			if (rightHandPos != null)
			{
				animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos. position);
				animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPos. rotation);
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand, aim);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand, aim);
			}
		}
	}

	void OnGUI()
	{
		GUILayout.Label("按Fire1键发射炮弹");
		GUILayout.Label("按Fire2键抬起或放下火箭炮");
	}

}
