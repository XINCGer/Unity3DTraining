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
    private Vector3[] mousePositions = new Vector3[10];

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

    private AudioSource audioSource;
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
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFirstMouseDown = true;
            isMouseDown = true;

            audioSource.Play();
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
            if (Vector3.Distance(head, last) > 0.01f)
            {
                SavePosition(head);
                posCounter++;

                //发射一条射线
                OnRayCast(head);
            }
            last = head;
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                mousePositions[i] = Vector3.zero;
            }
        }
        this.SetPositions(mousePositions);
    }

    private void SavePosition(Vector3 pos)
    {
        pos.z = 0;
        if (posCounter <= 9)
        {
            for (int i = posCounter; i < 10; i++)
            {
                mousePositions[i] = pos;
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                mousePositions[i] = mousePositions[i + 1];
            }
            mousePositions[9] = pos;
        }
    }

    private void OnRayCast(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log(hits[i].collider.name);
            hits[i].collider.gameObject.SendMessage("",SendMessageOptions.DontRequireReceiver);
        }
    }
}
