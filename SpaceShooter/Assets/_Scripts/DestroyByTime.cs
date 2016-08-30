/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/29
*/
using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public float lifeTime=2.0f;
	// Use this for initialization
	void Start () {
		Destroy (gameObject,lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
