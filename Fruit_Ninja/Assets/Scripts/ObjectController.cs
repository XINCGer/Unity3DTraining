using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体控制脚本
/// </summary>
public class ObjectController : MonoBehaviour {

    public GameObject halfGameObject;
    public GameObject spalsh;
    public GameObject spalshFlat;

    private bool isDead = false;
    public float radio;
    public AudioClip audioClip;

    public void OnCut()
    {
        //防止重复调用
        if (isDead) return;

        //如果是炸弹
        if (gameObject.name.Contains("Bomb"))
        {
            //生成特效
            Instantiate(spalsh, transform.position, Quaternion.identity);

            UIScore.Instance.RemoveScore(20);
        }
        //如果是水果
        else
        {
            //生成分瓣水果
            for (int i = 0; i < 2; i++)
            {
                if (halfGameObject == null) return;
                GameObject go = Instantiate<GameObject>(halfGameObject, transform.position, Random.rotation);
                go.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * radio, ForceMode.Impulse);
            }

            //生成特效
            Instantiate(spalsh, transform.position, Quaternion.identity);
            Instantiate(spalshFlat, transform.position, Quaternion.identity);

            UIScore.Instance.AddScore(10);
        }
        AudioSource.PlayClipAtPoint(audioClip,transform.position);

        Destroy(gameObject);
        isDead = true;
    }
}
