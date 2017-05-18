using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] fruits;
    public GameObject bomb;

    public float spawnTime = 1f;
    public float countTime = 0f;
    public void Start()
    {

    }

    public void Update()
    {
        countTime += Time.deltaTime;

        if (countTime >= spawnTime)
        {
            SwapnObject(SpwanType.Apple);
            countTime = 0f;
        }
    }

    private void SwapnObject(SpwanType type)
    {
        Debug.Log("Swpan" + Time.time);

    }



}

public enum SpwanType : byte
{
    Apple,
    Lemon,
    Watermelon,
    Bomb
}
