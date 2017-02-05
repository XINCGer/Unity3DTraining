using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;
    [SyncVar(hook = "OnHealthChanged")]
    public int currentHealth = maxHealth;
    public Slider _slider;
    public bool DestoryOnDamage = false;
    private NetworkStartPosition[] spawnPositions;
    
    public void TakeDamage(int damage)
    {
        if (isServer == false) return;
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            if (DestoryOnDamage)
            {
                Destroy(gameObject);
                return;
            }
            currentHealth = 100;
            Debug.Log("Dead!");
            RpcReSpawn();
        }
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPositions = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    void OnHealthChanged(int health)
    {
        _slider.value = health / (float)maxHealth;
    }

    [ClientRpc]
    void RpcReSpawn()
    {
        if(isLocalPlayer == false)return;
        Vector3 spawnPoint = Vector3.zero;
        if (spawnPositions != null && spawnPositions.Length > 0)
        {
            spawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)].transform.position;
        }
        transform.position = spawnPoint;
    }
}
