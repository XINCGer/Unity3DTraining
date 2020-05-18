using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIView基类
/// </summary>
public class UIBase : MonoBehaviour
{
    private void Awake()
    {
        BindView();
    }
    private void Start()
    {
        OnShow();
    }
    protected virtual void BindView()
    {

    }
    protected virtual void OnShow()
    {

    }
}
