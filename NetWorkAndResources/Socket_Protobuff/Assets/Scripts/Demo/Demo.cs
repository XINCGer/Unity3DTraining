using Fitness.SocketClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour {

    public string Address;
    public int Port;
	// Use this for initialization
	void Start () {
        SocketManager.Instance.Init(Address, Port);
    }
}
