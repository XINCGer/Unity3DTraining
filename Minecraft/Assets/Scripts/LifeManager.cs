using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{

    private Transform myTransform;
    private Vector3 startPoint;
    public GameObject gameOverUI;
    private bool isGameOver;
	// Use this for initialization
	void Start ()
	{
	    myTransform = this.transform;
	    startPoint = myTransform.position;
	    isGameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if (myTransform.position.y < -10)
	    {
	        GameOver();
	    }
	    if (Input.GetKeyDown(KeyCode.R) && isGameOver ==false)
	    {
	        Restart();
	    }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        isGameOver = true;
    }

    private void Restart()
    {
        myTransform.position = startPoint;
        isGameOver = false;
        gameOverUI.SetActive(false);
    }
}
