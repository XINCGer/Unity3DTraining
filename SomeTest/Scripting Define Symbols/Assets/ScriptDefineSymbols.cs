using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptDefineSymbols : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
//自定义宏，在PlayerSetting中设置，分号分割，记得敲回车
#if SHOEDEBUG     
//unity默认宏
#if UNITY_EDITOR
        Debug.Log("编辑器模式下运行！");
#endif
#if UC
        Debug.Log("UC渠道");
#endif
#if QQ
        Debug.Log("QQ渠道");
#endif
#endif
//未定义的宏会自动被注释
#if KKK
        Debug.Log("UC渠道");
#endif
    }
}
