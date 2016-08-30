/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/26
*/
using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
	public GameObject explosion;	//小行星爆炸粒子对象
	public GameObject playerExplosion;	//飞船爆炸粒子对象
	public int scoreValue;		//表示小行星被击中后玩家分数增加的数量
	private GameController gameController;	//用来调用gameObject的AddScore函数

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.FindWithTag ("GameController");
		if (go != null) {
			gameController = go.GetComponent<GameController> ();
		} else {
			Debug.Log("找不到tag为GameController的对象");
		}
		if(gameController==null){
			Debug.Log("找不到脚本GameCOntroller.cs");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if(other.tag=="Boundary"){
			return;
		}
		Instantiate (explosion,transform.position,transform.rotation);
		if(other.tag=="Player"){
			Instantiate(playerExplosion,other.transform.position,other.transform.rotation);
			gameController.GameOver();
		}
		gameController.AddScore (scoreValue);
		Destroy (other.gameObject);
		Destroy (gameObject);
	}
}
