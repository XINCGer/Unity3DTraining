using Fitness.SocketClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketMono : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        this.InitManager();
    }

    private void InitManager()
    {
        HeartManager.Instance.CreateModel(gameObject);
    }

    private void Update()
    {
        SocketManager.Instance.UpdateProto();
    }

    private void OnApplicationQuit()
    {
        SocketManager.Instance.OnApplicationQuit();
    }
}
