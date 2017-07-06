using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体控制脚本
/// </summary>
public class ObjectController : MonoBehaviour {

    public GameObject halfGameObject;
    private bool isDead = false;
    public float radio;

    public void OnCut()
    {
        if (isDead) return;

        for (int i = 0; i < 2; i++)
        {
            if (halfGameObject == null) return;
            GameObject go= Instantiate<GameObject>(halfGameObject,transform.position,Random.rotation);
            go.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * radio, ForceMode.Impulse);
        }
        isDead = true;
    }
}
