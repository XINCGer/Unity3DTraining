using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProfiler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //FuncWithProfiler();
        FuncWithoutProfiler();
    }

    private void FuncWithProfiler()
    {
        UnityEngine.Profiling.Profiler.BeginSample("FuncA");
        FuncA();
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("FuncB");
        FuncB();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void FuncWithoutProfiler()
    {
        FuncA();
        FuncB();
    }

    private void FuncA()
    {
        int[] arr = new int[10];
    }

    private void FuncB()
    {

    }
}
