using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    public float rotateSpeed=120f;
    public float moveSpeed = 10f;
	// Update is called once per frame
	void Update ()
	{
	    if (isLocalPlayer == false) return;     //只有是localPlayer时才控制移动
	    float h = Input.GetAxis("Horizontal");
	    float v = Input.GetAxis("Vertical");
        this.transform.Rotate(Vector3.up*h*rotateSpeed*Time.deltaTime);
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * v);
	}
}
