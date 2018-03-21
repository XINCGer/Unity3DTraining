 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class Responsive : MonoBehaviour {

    protected Animator animator;

    private Locomotion locomotion = null;

	// Use this for initialization
	void Start () 
	{
        animator = GetComponent<Animator>();
        locomotion = new Locomotion(animator);
	}
    
	void Update () 
	{
        if (animator)
		{
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            float speed = (h * h + v * v) * 6;
            float direction = Mathf.Atan2(h, v) * 180.0f / 3.14159f;

            locomotion.Do(speed, direction);
		}		
	}
}
