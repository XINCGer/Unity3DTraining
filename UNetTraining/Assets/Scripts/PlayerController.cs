using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    public float rotateSpeed=120f;
    public float moveSpeed = 10f;
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
	// Update is called once per frame
	void Update ()
	{
	    if (isLocalPlayer == false) return;     //只有是localPlayer时才控制移动
	    float h = Input.GetAxis("Horizontal");
	    float v = Input.GetAxis("Vertical");
        this.transform.Rotate(Vector3.up*h*rotateSpeed*Time.deltaTime);
        this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * v);

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        Fire();
	    }
	}

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color=Color.blue;
    }

    private void Fire()
    {
       GameObject _bullet =  Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        _bullet.GetComponent<Rigidbody>().velocity = BulletSpawn.forward*10;
        Destroy(_bullet,2);
    }

    void OnCollisionEnter(Collision collider)
    {
        Debug.Log("1111");
    }
}
