using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log("Dead!");
        }
    }
}
