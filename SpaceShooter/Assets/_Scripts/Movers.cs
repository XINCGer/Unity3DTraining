/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/25
*/
using UnityEngine;
using System.Collections;

public class Movers : MonoBehaviour {

	public float speed = 20.0f;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
