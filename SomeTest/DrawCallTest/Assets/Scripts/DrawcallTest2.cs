using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawcallTest2 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Init());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Init()
    {
        yield return null;
        GameObject prefab = Resources.Load<GameObject>("Cube");
        //动态合批
        for (int i = 0; i < 500; i++)
        {
            GameObject cube = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            if (i < 100)
            {
                cube.transform.localScale = new Vector3(2 + i, 2 + i, 2 + i);
            }
        }
    }
}
