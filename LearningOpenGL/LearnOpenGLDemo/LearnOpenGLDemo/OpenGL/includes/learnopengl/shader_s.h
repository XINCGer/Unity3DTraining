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
	Shader(const GLchar* vertexPath, const GLchar* fragmentPath) {

		//1.从文件读取vertex/fragment source code
		std::string vertexCode;
		std::string fragmentCode;
		std::ifstream vShaderFile;
		std::ifstream fShaderFile;

		//确保ifstream对象可以抛出异常
		vShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);
		fShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);

		try
		{
			//打开文件
			vShaderFile.open(vertexPath);
			fShaderFile.open(fragmentPath);
			std::stringstream vShaderStream, fShaderStream;
			//读取文件内容到流中
			vShaderStream << vShaderFile.rdbuf();
			fShaderStream << fShaderFile.rdbuf();
			//关闭文件句柄
			vShaderFile.close();
			fShaderFile.close();
			//流转为string
			vertexCode = vShaderStream.str();
			fragmentCode = fShaderStream.str();
		}
		catch (const std::ifstream::failure e)
		{
			std::cout << "ERROR::SHADER::FILE_NOT_SUCCESFULLY_READ" << std::endl;
		}

		const char* vShaderCode = vertexCode.c_str();
		const char* fShaderCode = fragmentCode.c_str();

		//编译shader
		unsigned int vertex, fragment;
		vertex = glCreateShader(GL_VERTEX_SHADER);
		glShaderSource(vertex, 1, &vShaderCode, NULL);
		glCompileShader(vertex);
		checkComplieErrors(vertex, "VERTEX");

		fragment = glCreateShader(GL_FRAGMENT_SHADER);
		glShaderSource(fragment, 1, &fShaderCode, NULL);
		glCompileShader(fragment);
		checkComplieErrors(fragment, "FRAGMENT");

		ID = glCreateProgram();
		glAttachShader(ID, vertex);
		glAttachShader(ID, fragment);
		glLinkProgram(ID);
		checkComplieErrors(ID, "PROGRAM");

		glDeleteShader(vertex);
		glDeleteShader(fragment);
	}

	//使用/激活程序
	void use() {
		glUseProgram(ID);
	}

	//uniform工具
	void setBool(const std::string &name, bool value) const {
		glUniform1i(glGetUniformLocation(ID, name.c_str()), (int)value);
	}

	void setInt(const std::string &name, int value) const {
		glUniform1i(glGetUniformLocation(ID, name.c_str()), value);
	}
	void setFloat(const std::string &name, float value) const {
		glUniform1f(glGetUniformLocation(ID, name.c_str()), value);
	}

private:
	//检测Shader编译、链接错误的工具方法
	void checkComplieErrors(unsigned int shader, std::string type) {
		int success;
		char infoLog[1024];
		if ("PROGRAM" != type) {
			glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
			if (!success) {
				glGetShaderInfoLog(shader, 1024, NULL, infoLog);
				std::cout << "ERROR::SHADER_COMPILATION_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
			}
		}
		else {
			glGetProgramiv(shader, GL_LINK_STATUS, &success);
			if (!success) {
				glGetProgramInfoLog(shader, 1024, NULL, infoLog);
				std::cout << "ERROR::PROGRAM_LINKING_ERROR of type: " << type << "\n" << infoLog << "\n -- --------------------------------------------------- -- " << std::endl;
			}

		}
	}
};
#endif // !SHADER_H


