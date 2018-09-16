#include<iostream>
#define GLEW_STATIC
#include <GL/glew.h>
#include <GLFW\glfw3.h>

using namespace std;

int main(int argc, char** argv[])
{
	/*glewExperimental = GL_TRUE;
	if (glewInit()!=GLEW_OK)
	{
	cout << "failed to initalize GLEW" << endl;
	return -1;
	}*/

	glfwInit();//³õÊ¼»¯
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);//ÅäÖÃGLFW
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);//ÅäÖÃGLFW
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);//
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

	GLFWwindow* window = glfwCreateWindow(800, 600, "LearnOpenGL", nullptr, nullptr);
	if (window == nullptr)
	{
		cout << "Failed to create GLFW window" << endl;
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);
	while (!glfwWindowShouldClose(window))
	{
		glfwPollEvents();
		glfwSwapBuffers(window);
	}
	glfwTerminate();
	return 0;


}