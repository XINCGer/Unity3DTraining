using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LifeManager : MonoBehaviour
{

    private Transform myTransform;
    private Vector3 startPoint;
    public GameObject gameOverUI;
    private FirstPersonController firstPersonController;
    public GameObject Env;
    private GameObject envGameObject;
    void Awake()
    {
       envGameObject = Instantiate(Env, Vector3.zero,Quaternion.identity);
    }
	// Use this for initialization
	void Start ()
	{
	    myTransform = this.transform;
	    startPoint = myTransform.position;
	    firstPersonController = this.GetComponent<FirstPersonController>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (myTransform.position.y < -10)
	    {
	        GameOver();
	    }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        firstPersonController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        myTransform.position = startPoint;
        gameOverUI.SetActive(false);
        firstPersonController.enabled = true;
        DestroyImmediate(envGameObject,false);
        envGameObject= Instantiate(Env, Vector3.zero,Quaternion.identity);
    }
}
