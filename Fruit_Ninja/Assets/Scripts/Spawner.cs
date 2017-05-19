using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("水果预设")]
    public GameObject[] fruits;
    [Tooltip("炸弹预设")]
    public GameObject bomb;

    [Tooltip("生成时间间隔")]
    public float spawnTime = 1f;
    public float countTime = 0f;

    [Tooltip("是否生成")]
    public bool isSpawning = true;
    
    [Tooltip("伸缩因子")]
    public float xMinScaleFactor = 0.2f;
    [Tooltip("伸缩因子")]
    public float xMaxScaleFactor = 0.8f;
    [Tooltip("伸缩因子")]
    public float yMinScaleFactor = 5.0f;
    [Tooltip("伸缩因子")]
    public float yMaxScaleFactor = 6.0f;

    public void Start()
    {

    }

    public void Update()
    {
        if (false == isSpawning) return;

        countTime += Time.deltaTime;

        if (countTime >= spawnTime)
        {
            SwapnObject(SpwanType.Fruits);
            countTime = 0f;
        }
    }

    /// <summary>
    /// 产生obj
    /// </summary>
    /// <param name="type"></param>
    private void SwapnObject(SpwanType type)
    {
        float x = Random.Range(-7.5f, 7.5f);
        float y = transform.position.y;
        GameObject go = null;
        switch (type)
        {
            case SpwanType.Fruits:
                go = fruits[Random.Range(0, fruits.Length)];
                break;
            case SpwanType.Bomb:
                go = bomb;
                break;
        }
        Instantiate(go, new Vector3(x, y, 0), Random.rotation);
        Vector3 velocity = new Vector3(-x*Random.Range(xMinScaleFactor,xMaxScaleFactor),-Physics.gravity.y*Random.Range(yMinScaleFactor,yMaxScaleFactor),0);
        go.GetComponent<Rigidbody>().velocity = velocity;
    }



}

public enum SpwanType : byte
{
    Fruits,
    Bomb
}
