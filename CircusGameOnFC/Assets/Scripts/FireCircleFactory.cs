using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCircleFactory : MonoBehaviour
{

    private GameObject fireCirclePrefab;
    private PlayerController playerController;
    private float spawnTime = 0f;
	// Use this for initialization
	void Start ()
	{
	    playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	    if (null == fireCirclePrefab)
	    {
	        fireCirclePrefab = Resources.Load<GameObject>("Prefabs/FireCircle");
	    }
	    spawnTime = Random.Range(1.5f, 2.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
        if(playerController.Hp<=0)return;

	    spawnTime -= Time.deltaTime;
	    if (spawnTime <= 0)
	    {
            Instantiate(fireCirclePrefab, transform.position, Quaternion.identity);
            spawnTime = Random.Range(1.5f, 2.5f);
	    }

	}
}
