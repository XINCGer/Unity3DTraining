/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/26
*/
using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//若要处理游戏对象移出触发器时的事件，需要重载OnTriggerExit函数
	//参数other表示移出触发器的对象
	void OnTriggerExit(Collider other){
		Destroy (other.gameObject);
	}
}
