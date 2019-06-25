#ifndef _CAMERA_H
#define _CAMERA_H

#include<glad/glad.h>
#include<glm/glm.hpp>
#include<glm/gtc/matrix_transform.hpp>

#include<vector>

//相机移动的方向
enum CAMERA_MOVETYPE {
	FORWARD,
	BACKWARD,
	LEFT,
	RIGHT,
};

//相机常量，相机初始化和复位的时候用到
const float YAW = -90.0f;
const float PITCH = 0.0f;
const float SPEED = 2.5f;
const float SENSITIVITY = 0.1f;
const float ZOOM = 45.0f;

//适用于OpenGL的相机类
class Camera {
public:
private:
};
#endif // !_CAMERA_H

