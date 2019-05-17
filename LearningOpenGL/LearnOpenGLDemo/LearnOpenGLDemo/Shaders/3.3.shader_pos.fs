#version 330 core
out vec4 FragColor;

in vec3 ourPosition;

void main()
{
    FragColor = vec4(ourPosition, 1.0f);
}