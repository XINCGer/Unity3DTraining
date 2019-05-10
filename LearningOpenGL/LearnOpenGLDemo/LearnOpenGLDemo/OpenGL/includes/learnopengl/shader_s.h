#ifndef SHADER_H
#define SHADER_H
#include <glad/glad.h>  // 包含glad来获取所有的必须OpenGL头文件

#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

class Shader {
public:
	//程序ID
	unsigned int ID;

	//构造器，读取并构建着色器
	Shader(const GLchar* vertexPath, const GLchar* fragmentPath);

	//使用/激活程序
	void use();

	//uniform工具
	void setBool(const std::string &name, bool value) const;
	void setInt(const std::string &name, int value) const;
	void setFloat(const std::string &name, float value) const;
};
#endif // !SHADER_H


