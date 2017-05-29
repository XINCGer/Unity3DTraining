using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标控制器：实现切水果功能
/// </summary>
public class MouseController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    /// <summary>
    /// 是否鼠标第一次按下
    /// </summary>
    private bool isFirstMouseDown = false;

    /// <summary>
    /// 鼠标是否按下
    /// </summary>
    private bool isMouseDown = false;

    /// <summary>
    /// 鼠标的位置信息
    /// </summary>
    private Vector3[] mousePositions=new Vector3[10];
   
    /// <summary>
    /// 位置计数器
    /// </summary>
    private int posCounter = 0;

    /// <summary>
    /// 当前帧下鼠标的位置
    /// </summary>
    private Vector3 head;

    /// <summary>
    /// 上一帧的鼠标位置
    /// </summary>
    private Vector3 last;


    /// <summary>
    /// 设置LineRender位置
    /// </summary>
    /// <param name="positions"></param>
    public void SetPositions(Vector3[] positions)
    {
        lineRenderer.SetPositions(positions);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFirstMouseDown = true;
            isMouseDown = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
        }

        DrawLine();
        isFirstMouseDown = false;
    }

    private void DrawLine()
    {
        if (isFirstMouseDown)
        {
            posCounter = 0;
            head = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            last = head;
        }
        if (isMouseDown)
        {
            head = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //如果距离比较远就采样
            if (Vector3.Distance(head, last)>0.01f)
            {
                SavePosition(head);
                posCounter++;
            }
            last = head;
        }
    }

    private void SavePosition(Vector3 pos)
    {
    }
}
