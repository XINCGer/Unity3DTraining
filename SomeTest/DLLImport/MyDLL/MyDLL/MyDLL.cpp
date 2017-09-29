// MyDLL.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "MyDLL.h"

void SomeFunction(std::string &str)
{
	str += "Hello";
}

int Pow2(int num) {
	return num*num;
}

