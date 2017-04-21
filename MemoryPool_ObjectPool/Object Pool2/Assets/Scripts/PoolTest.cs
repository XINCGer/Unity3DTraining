using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ：根本原理就是：打一颗 “5mm子弹” 就创建一个gameobject，这 “5mm颗子 弹” 需要销毁的时候不销毁它，
/// 把它的激活状态变为false，然后把它存入 “5mm子弹” 数组中，当你再打出一颗“5mm子弹”的时候，不需要创建一颗新的“5mm子弹”
/// 只需要把刚才那颗未激活“5mm子弹”从ArrayList里拿出来激活一下，然后删除掉ArrayList里关于这个gameobje的数据就行了
/// 然后这颗子弹再销毁。。。。如此往复，子弹多了也同理
/// </summary>
public class PoolTest : MonoBehaviour
{

    public Transform trans;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        GameObject bullet = GameObjectPool.Get("bullet", trans.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(trans.forward*30,ForceMode.Impulse);
	    }
	}
}
