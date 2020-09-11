using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public partial class Controller_01
{
	public object Excecute(string funcName, params object[] param)
	{
		if(funcName == "Ex_Func1")
		{
			var arg_0 = Convert.ToInt32(param[0]);;
			var arg_1 = Convert.ToInt32(param[1]);;
			this.Ex_Func1(arg_0, arg_1);
		}
		if(funcName == "Ex_Func2")
		{
			var arg_0 = param[0] as System.String;
			var arg_1 = Convert.ToInt32(param[1]);;
			return this.Ex_Func2(arg_0, arg_1);
		}
		return 0;
	}
}
