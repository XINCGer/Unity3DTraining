using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDecetor : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            int index = Random.Range(0, 10);
            KnapsackManager.Instance.StoreItem(index);
        }
    }
}
