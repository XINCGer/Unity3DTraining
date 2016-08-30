/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/26
*/
using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {

	public float tumble=10.0f;	//表示小行星的旋转系数
	// Use this for initialization
	void Start () {
		//angularVelocity表示刚体的角速度，Random.insideUnitSphere表示单位长度半径球体内的一个随机点
		GetComponent<Rigidbody> ().angularVelocity = Random.insideUnitSphere * tumble;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
