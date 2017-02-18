using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    private float timer = 0f;
    // Use this for initialization
    void Start() {
        timer = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Time.timeScale = 2;
        }
        Debug.Log("----------------------------");
        Debug.Log("Update: " + Time.deltaTime);
        Debug.Log("Time.time: " + Time.time);
    }

    void LateUpdate() {
        Debug.Log("----------------------------");
        Debug.Log("LateUpdate " + Time.deltaTime);
        Debug.Log("Time.time: " + Time.time);
    }

    void FixedUpdate() {
        Debug.Log("----------------------------");
        Debug.Log("FixedUpdate: " + Time.fixedDeltaTime);
        Debug.Log("Time.time: " + Time.time);
    }

}
