using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rigidbody2D;
    private bool isGround;

	// Use this for initialization
	void Start ()
	{
	    animator = this.GetComponent<Animator>();
	    rigidbody2D = this.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
