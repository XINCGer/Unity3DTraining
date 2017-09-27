using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{


    private NavMeshAgent navMeshAgent;

    private Rigidbody rigidbody;

    private bool isJumping = false;
    // Use this for initialization
    void Start()
    {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (!hit.collider.tag.Equals("Plane"))
                {
                    return;
                }
                Vector3 point = hit.point;
                transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
                navMeshAgent.SetDestination(point);
            }
        }

        if (navMeshAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jumping());
        }
    }

    IEnumerator Jumping()
    {
        if (isJumping) yield break;

        navMeshAgent.enabled = false;

        rigidbody.AddForce(transform.up * 20, ForceMode.Impulse);
        rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);

        yield return new WaitForSeconds(2.0f);
        navMeshAgent.enabled = true;
        isJumping = false;
    }
}
