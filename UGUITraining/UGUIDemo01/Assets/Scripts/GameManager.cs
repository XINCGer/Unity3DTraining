/*
ProjectName: UGUI之游戏菜单
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/09/14
*/
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnStartGame(string sceneName){
		Application.LoadLevel (sceneName);
	}
	public void OnStartGame(int sceneIndex){
		Application.LoadLevel (sceneIndex);
	}
}
