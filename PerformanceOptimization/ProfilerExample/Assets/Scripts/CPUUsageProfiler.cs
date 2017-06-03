using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUUsageProfiler : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		Func();
	}

    /// <summary>
    /// 具体实现
    /// </summary>
    private void Func()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Func");
        subFunc();
        subFunc();
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void subFunc()
    {
        UnityEngine.Profiling.Profiler.BeginSample("subFunc");
        for (int i = 0; i < 100; i++)
        {
            int []arr = new int[100];
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
