using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float moveForce = 365f; //运动力大小
	public float maxSpeed = 5f; //角色最大速度
	public float jumpForce = 1000f; // 角色跳跃时力大小

	private Transform groundCheck; //用来检测角色是否在地面上的对象
	private bool grounded = false; // 角色是否在地面上，默认为false
	private Animator anim; // 角色对象上的Animator组件
	
	[HideInInspector]
	public bool facingRight = true; //角色是否朝向右侧
	[HideInInspector]
	public bool jump = false; //角色是否在跳跃

	void Start () 
	{
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
	}
	
	void Update () 
	{
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		if(Input.GetButtonDown("Jump") && grounded)
			jump = true;
	}

	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");
		anim.SetFloat("Speed", Mathf.Abs(h));
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if(h * rb.velocity.x < maxSpeed)
			rb.AddForce(Vector2.right * h * moveForce);
		
		if(Mathf.Abs(rb.velocity.x) > maxSpeed)
			rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

		if(h > 0 && !facingRight)
			Flip();
		else if(h < 0 && facingRight)
			Flip();

		if(jump)
		{
			anim.SetTrigger("Jump");
			rb.AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}
	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
