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
	        CmdFire();
	    }
	}

    public override void OnStartLocalPlayer()
    {
        //只会在生成本地角色时调用
        GetComponent<MeshRenderer>().material.color=Color.blue;
    }

    [Command]   //called in clint ,run in server
    private void CmdFire()
    {
        //在Server端生成子弹，然后同步到各个客户端
        GameObject _bullet =  Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        _bullet.GetComponent<Rigidbody>().velocity = BulletSpawn.forward*10;
        Destroy(_bullet,2);
        NetworkServer.Spawn(_bullet);
    }

    void OnCollisionEnter(Collision collider)
    {
        Debug.Log("1111");
    }
}
