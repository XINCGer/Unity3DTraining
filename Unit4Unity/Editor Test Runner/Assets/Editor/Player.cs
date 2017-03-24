using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{

    public float Health { get; set; }

#region 正确的方法

    public void Damage(float value)
    {
        Health -= value;
        if (Health < 0)
        {
            throw new NegativeHealthException();
        }
    }

    public void Recover(float value)
    {
        Health += value;
    }
#endregion

#region 错误的方法

    public void DamageWrong(float value)
    {
        Health -= value + 1;
    }

    public void DamageNoException(float value)
    {
        Health -= value;
    }
#endregion
}
