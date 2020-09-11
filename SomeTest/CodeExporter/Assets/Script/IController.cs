using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    object Excecute(string funcName, params object[] param);
}
