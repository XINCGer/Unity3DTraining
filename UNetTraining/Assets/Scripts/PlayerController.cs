using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float rotateSpeed=120f;
    public float moveSpeed = 10f;
	// Update is called once per frame
	void Update ()
	{
	    float h = Input.GetAxis("Horizontal");
	    float v = Input.GetAxis("Vertical");
        this.transform.Rotate(Vector3.up*h*rotateSpeed*Time.deltaTime);
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * v);
	}
}
