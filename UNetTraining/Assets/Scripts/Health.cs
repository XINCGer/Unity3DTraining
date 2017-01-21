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

    public void TakeDamage(int damage)
    {
        if (isServer == false) return;
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");
            RpcReSpawn();
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
        transform.position=Vector3.zero;
    }
}
