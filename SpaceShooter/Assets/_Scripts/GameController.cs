/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/29
*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

	public GameObject hazard;	//准备实例化的陨石
	public Vector3 spawnValues;	//在x轴上极大值
	private Vector3 spawnPosition = Vector3.zero;	//实例化位置
	private Quaternion spawnRotation;
	public int hazardCount;	//陨石的生成数量
	public float spawnWait ; //	每次生成陨石后的延迟时间
	public float startWait=1.0f;
	public float waveWait=2.0f; //两批小行星阵列的时间间隔
	public Text scoreText;	//更新计分Text组件
	private int score;	//用于保存当前分值
	public Text gameOverText; 	//用于更新text组件的显示
	private bool gameOver;	//表示游戏是否结束
	public Text restartText;
	private bool restart; //是否重新开始游戏

	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while(true){
			if(gameOver){
				restartText.text="按【R】键重新开始\n或点击屏幕";
				restart=true;
				break;
			}
			for (int i=0; i<hazardCount; i++) {
				spawnPosition.x = Random.Range (-spawnValues.x, spawnValues.x);
				spawnPosition.z = spawnValues.z;
				spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds(waveWait);
		}

	}
	// Use this for initialization
	void Start ()
	{
		gameOverText.text = "";
		gameOver = false;
		restartText.text = "";
		restart = false;
		score = 0;
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (restart) {
			if(Input.GetKeyDown(KeyCode.R)||Input.GetMouseButtonDown(0)){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}

	public void AddScore(int newScoreValue){
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore(){
		scoreText.text = "得分：" + score;
	}

	public void GameOver(){
		gameOver = true;
		gameOverText.text = "游戏结束!";
	}
}
