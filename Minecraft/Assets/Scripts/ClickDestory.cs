using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDestory : MonoBehaviour {

    void OnMouseDown()
    {
        Destroy(this.gameObject);
    }
}
