using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private bool isGround;
    public BgController bgController;
	// Use this for initialization
	void Start ()
	{
	    animator = this.GetComponent<Animator>();
	    rigidbody2D = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{

	    float horizontal = Input.GetAxis("Horizontal");
	    if (horizontal != 0)
	    {
	        if (horizontal > 0)
	        {
	            bgController.RollingMap(Direction.Right);
	        }
	        else if (horizontal < 0)
	        {
	            bgController.RollingMap(Direction.Left);
	        }
	        animator.SetBool("IsRun", true);
	    }
	    else
	    {
	        animator.SetBool("IsRun",false);
	    }

	    if (Input.GetKeyDown(KeyCode.Space) && isGround)
	    {
	        animator.SetBool("IsJump",true);
            rigidbody2D.AddForce(Vector2.up * 170);
	    }
	}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isGround = true;
        animator.SetBool("IsJump", false);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        isGround = false;
    }


}
