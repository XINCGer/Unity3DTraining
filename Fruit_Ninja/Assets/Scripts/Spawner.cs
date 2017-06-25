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
    public float yMinScaleFactor = 1.2f;
    [Tooltip("伸缩因子")]
    public float yMaxScaleFactor = 1.8f;

    private int offsetZ = 0;

    private AudioSource audioSource;
    public void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (false == isSpawning) return;

        countTime += Time.deltaTime;

        if (countTime >= spawnTime)
        {
            //生成随机数量的水果
            int randCount = Random.Range(1, 5);
            for (int i = 0; i < randCount; i++)
            {
                SpawnObject(SpwanType.Fruits);
            }

            //随机生成炸弹
            int randRatio = Random.Range(1, 100);
            if (randRatio > 70)
            {
                SpawnObject(SpwanType.Bomb);
            }

            countTime = 0f;
        }
    }

    /// <summary>
    /// 产生obj
    /// </summary>
    /// <param name="type"></param>
    private void SpawnObject(SpwanType type)
    {
        audioSource.Play();

        float x = Random.Range(-7.5f, 7.5f);
        float y = transform.position.y;
        //避免生成在同一个平面上
        offsetZ -= 2;
        if (offsetZ <= -10)
        {
            offsetZ = 0;
        }

        GameObject prefab = null;
        switch (type)
        {
            case SpwanType.Fruits:
                prefab = fruits[Random.Range(0, fruits.Length)];
                break;
            case SpwanType.Bomb:
                prefab = bomb;
                break;
        }
        GameObject obj = Instantiate(prefab, new Vector3(x, y, offsetZ), Random.rotation);
        Vector3 velocity = new Vector3(-x * Random.Range(xMinScaleFactor, xMaxScaleFactor), -Physics.gravity.y * Random.Range(yMinScaleFactor, yMaxScaleFactor), 0);
        obj.GetComponent<Rigidbody>().velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }




}

public enum SpwanType : byte
{
    Fruits,
    Bomb
}
