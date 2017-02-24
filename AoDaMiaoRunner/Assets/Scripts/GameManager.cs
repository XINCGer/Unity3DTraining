using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInfo
{
    private float height;
    private float width;

    public float Height
    {
        get { return height; }
        set { height = value; }
    }

    public float Width
    {
        get { return width; }
        set { width = value; }
    }
}
public class GameManager : MonoBehaviour
{

    public ScreenInfo screenInfo;
    public GameObject player;

    void Awake()
    {
        screenInfo = new ScreenInfo();
        screenInfo.Height = Screen.height;
        screenInfo.Width = Screen.width;
    }
	// Use this for initialization
	void Start () {
		Debug.Log(screenInfo.Width+" "+screenInfo.Height);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
