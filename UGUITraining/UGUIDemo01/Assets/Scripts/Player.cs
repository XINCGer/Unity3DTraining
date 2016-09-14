/*
ProjectName: UGUI之游戏菜单
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/09/14
*/
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed=90f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.forward*Time.deltaTime*speed);
	}

	public void ChangeSpeed(float newSpeed){
		speed = newSpeed;
	}
}
