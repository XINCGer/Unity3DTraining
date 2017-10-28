using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCircleController : MonoBehaviour
{

    private PlayerController playerController;
    public float Speed;
    // Use this for initialization
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.Hp <= 0)
        {
            return;
        }
        this.transform.Translate(Vector3.left * Time.deltaTime * Speed);
        if (this.transform.position.x <= -2.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
