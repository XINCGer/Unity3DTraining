#ifndef __MyDLL_H__
#define __MyDLL_H__

#include <string>

/*  To use this exported function of dll, include this header
*  in your project.
*/

#ifdef BUILD_DLL
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif


#ifdef __cplusplus
extern "C"
{
#endif

	void DLL_EXPORT SomeFunction(std::string &str);
	int DLL_EXPORT Pow2(int num);

#ifdef __cplusplus
}
#endif

#endif // __MAIN_H__
