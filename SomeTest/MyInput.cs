using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput : MonoBehaviour {
    //移动方向枚举
    enum MoveDir
    {
        None = 0,       //不动
        Up = 1,         //上8
        Down = -1,      //下2
        Left = 10,      //左4
        Right = -10,    //右6
        UL = 11,        //左上7
        UR = -9,        //右上9
        DL = 9,         //左下1
        DR = -11,       //右下3
    }

    //输入按键常量（之后走配置）
    const KeyCode INPUT_UP = KeyCode.W;
    const KeyCode INPUT_DOWN = KeyCode.S;
    const KeyCode INPUT_LEFT = KeyCode.A;
    const KeyCode INPUT_RIGHT = KeyCode.D;

    //默认移动方向
    private MoveDir moveDir = MoveDir.None;
    //按压值
    private int moveDirValue = 0;
    //按压记录
    private bool isUpPress = false;
    private bool isDownPress = false;
    private bool isLeftPress = false;
    private bool isRightPress = false;

    //是否可以移动
    private bool canMove = true;
    //右移动
    private Vector3 MOVE_RIGHT = new Vector3(1, 0, 0);
    //上移动
    private Vector3 MOVE_UP = new Vector3(0, 1, 0);

    //外部调控速度
    public float speed = 2f;
    //移动速度向量
    private Vector3 move_speed_dir = Vector3.zero;
    //移动距离
    private Vector3 move_dis = Vector3.zero;

    //控制目标
    public Transform target;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        CheckInputKey();
        CheckMoveDir();
    }

    void FixedUpdate()
    {
        CheckMove();
    }

    //检测输入按键
    void CheckInputKey()
    {
        //检测单一输入
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                Debug.Log("Single KeyCode: " + kcode);
                ChangeKeyPressState(kcode, true);
            }

            if (Input.GetKeyUp(kcode))
            {
                Debug.Log("Single KeyCode: " + kcode);
                ChangeKeyPressState(kcode, false);
            }
        }
    }

    //记录按键的按压状态
    void ChangeKeyPressState(KeyCode keyCode, bool isPress)
    {
        switch(keyCode)
        {
            case INPUT_UP:
                isUpPress = isPress;
                break;
            case INPUT_DOWN:
                isDownPress = isPress;
                break;
            case INPUT_LEFT:
                isLeftPress = isPress;
                break;
            case INPUT_RIGHT:
                isRightPress = isPress;
                break;
        }
    }

    //确定移动方向
    void CheckMoveDir()
    {
        moveDirValue = 0;
        //确定方向
        if(isUpPress)
        {
            moveDirValue += (int)MoveDir.Up;
        }
        if (isDownPress)
        {
            moveDirValue += (int)MoveDir.Down;
        }
        if (isLeftPress)
        {
            moveDirValue += (int)MoveDir.Left;
        }
        if (isRightPress)
        {
            moveDirValue += (int)MoveDir.Right;
        }
    }

    //检测是否可以移动
    void CheckMove()
    {
        //某些情况下可能禁止移动，例如暂停，播放CG等
        if(canMove && moveDirValue != (int)MoveDir.None)
        {
            PlayerMove(target, speed);
        }
    }

    //移动
    void PlayerMove(Transform target, float speed)
    {
        move_dis = speed * Time.deltaTime * GetSpeedDir();
        target.position += move_dis;
    }

    //速度向量
    Vector3 GetSpeedDir()
    {
        switch(moveDirValue)
        {
            case (int)MoveDir.Up:
                move_speed_dir = MOVE_UP;
                break;
            case (int)MoveDir.Down:
                move_speed_dir = -MOVE_UP;
                break;
            case (int)MoveDir.Left:
                move_speed_dir = -MOVE_RIGHT;
                break;
            case (int)MoveDir.Right:
                move_speed_dir = MOVE_RIGHT;
                break;
            case (int)MoveDir.UL:
                move_speed_dir = MOVE_UP - MOVE_RIGHT;
                break;
            case (int)MoveDir.UR:
                move_speed_dir = MOVE_UP + MOVE_RIGHT;
                break;
            case (int)MoveDir.DL:
                move_speed_dir = -MOVE_UP - MOVE_RIGHT;
                break;
            case (int)MoveDir.DR:
                move_speed_dir = -MOVE_UP + MOVE_RIGHT;
                break;
        }
        return move_speed_dir.normalized;
    }
}