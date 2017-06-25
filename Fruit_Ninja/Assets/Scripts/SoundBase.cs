using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBase : MonoBehaviour
{
    public AudioClip backGround;
    public AudioClip fruitLaunch;

    private static SoundBase _instance = null;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);        
    }

    public static SoundBase GetInstance()
    {
        return _instance;
    }
}
