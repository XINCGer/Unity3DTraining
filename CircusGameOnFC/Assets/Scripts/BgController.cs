using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    private Material material;
    private float rollOffset = 0f;
    public float RollSpeed;

    // Use this for initialization
    void Start()
    {
        this.material = this.GetComponent<SpriteRenderer>().material;
    }

    /// <summary>
    /// 滚动地图
    /// </summary>
    public void RollingMap(Direction direction)
    {
        if (Direction.Left == direction)
        {
            this.material.SetTextureOffset("_MainTex", new Vector2(rollOffset -= RollSpeed * Time.deltaTime, 0));
        }
        else if (Direction.Right == direction)
        {
            this.material.SetTextureOffset("_MainTex", new Vector2(rollOffset += RollSpeed * Time.deltaTime, 0));
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RollingMap(Direction.Left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RollingMap(Direction.Right);
        }
    }



}

/// <summary>
/// 移动方向的枚举
/// </summary>
public enum Direction : byte
{
    UP,
    Down,
    Left,
    Right,
}
