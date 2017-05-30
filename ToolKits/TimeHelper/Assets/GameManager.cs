using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
        TimeHelperA.SetTimer(3.0f,()=>{Debug.Log("Ring!");});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Delete()
    {
        TimeHelperA.Delete();
    }
}
