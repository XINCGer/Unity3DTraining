#include "drawCircle.h"
#include <cmath>
using namespace  std;

void ArcOpenGl(int xc, int yc, double r, double ts, double te)
{
	double PI = 3.1415926;

	if(te<ts)
	{
		te += 2 * PI;  
	}
	double dt = 0.4 / r;
	int n = (int)((te - ts) / dt + 0.5);
	double ta = ts;
	int x = xc + int(r*cos(ts));
	int y = yc + int(r*sin(ts));
	glBegin(GL_LINE_LOOP);
	glVertex2f(x, y);
	for(int i =1;i<=n;i++)
	{
		ta += dt;
		double cost = cos(ta);
		double sint = sin(ta);
		x = int(xc + r * cost);
		y = int(yc + r * sint);
		glVertex2f(x, y);
	}
	glEnd();
}
