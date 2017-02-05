using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{

    public GameObject EnemyPerfab;
    public int EnemyNums;
    public override void OnStartServer()
    {
        for (int i = 0; i < EnemyNums; i++)
        {
            Vector3 position = new Vector3(Random.Range(-6f,6f),0,Random.Range(-6f,6f));
            Quaternion quaternion = Quaternion.Euler(0,Random.Range(0,360),0);
            GameObject Enemy = Instantiate(EnemyPerfab, position, quaternion);

            NetworkServer.Spawn(Enemy);
        }
    }
}
