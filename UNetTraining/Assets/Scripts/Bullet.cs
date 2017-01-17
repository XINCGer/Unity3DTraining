using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    public void OnCollisionEnter(Collision collider)
    {
        GameObject player = collider.gameObject;
        Health _health = player.GetComponent<Health>();
        if (_health != null)
        {
            _health.TakeDamage(damage);
        }
        Destroy(this.gameObject);
    }
}
