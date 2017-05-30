using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimeHelperA : MonoBehaviour
{

    private static GameObject helperGO;
    private static float timeValue;
    private static float timeCounter;
    private static Action action1;
    private static bool isStart = false;
    private static int repeatCounter;
    private static float repeatTimeCounter;
    private static Action action2;
    // Update is called once per frame
    void Update()
    {
        if (isStart == false) return;

        timeCounter += Time.deltaTime;
        if (timeCounter >= timeValue)
        {
            if (action1 != null)
            {
                action1();
                Debug.Log("当前时间:" + timeCounter);
                Stop();
            }
        }
    }

    public static void SetTimer(float time, Action action)
    {
        if (helperGO == null)
        {
            helperGO = new GameObject();
            helperGO.name = "Timer";
            helperGO.AddComponent<TimeHelperA>();
        }

        init();
        timeValue = time;
        action1 = action;
    }

    public static void SetRepeatTimer(float time, Action action, int repeat)
    {
        
    }

    public static void Stop()
    {
        isStart = false;
        init();
    }

    private static void init()
    {
        timeCounter = 0;
        isStart = true;
        action1 = null;
        action1 = null;
        timeValue = 0;
        repeatCounter = 0;
        repeatTimeCounter = 0;
    }

    public static void Delete()
    {
        Destroy(helperGO);
    }

    void OnDestroy()
    {
        init();
        Debug.Log("OnDestory!");
    }


}
