using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float DeadTime;
    // Use this for initialization
    void Start()
    {
        Invoke("Dead", DeadTime);
    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}
