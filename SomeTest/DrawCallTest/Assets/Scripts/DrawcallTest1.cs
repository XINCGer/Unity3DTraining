using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawcallTest1 : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(Init());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Init()
    {
        yield return null;
        GameObject prefab = Resources.Load<GameObject>("Cube");
        //动态合批
        for (int i = 0; i < 500; i++)
        {
            GameObject.Instantiate(prefab, Random.insideUnitSphere * 3, Quaternion.identity);
        }
    }
}
