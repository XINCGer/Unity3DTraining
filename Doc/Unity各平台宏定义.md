## Unity各平台宏定义

### 对照表  
| 宏          |  定义   |  
| :----      | :----  |
| UNITY_EDITOR      | #define directive for calling Unity Editor scripts from your game code.   |
| UNITY_EDITOR_WIN       | #define directive for Editor code on Windows.    |
| UNITY_EDITOR_OSX       | #define directive for Editor code on Mac OS X.    | 
| UNITY_STANDALONE      | #define directive for compiling/executing code for any standalone platform (Mac OS X, Windows or Linux).     |
| UNITY_STANDALONE_WIN      | #define directive for compiling/executing code specifically for Windows standalone applications.     |
| UNITY_STANDALONE_OSX     | #define directive for compiling/executing code specifically for Mac OS X (including Universal, PPC and Intel architectures).    | 
| UNITY_STANDALONE_LINUX      | #define directive for compiling/executing code specifically for Linux standalone applications.     |
| UNITY_ANDROID      | #define directive for the Android platform.    |
| UNITY_IOS       | #define directive for compiling/executing code for the iOS platform.    | 
| UNITY_IPHONE        | Deprecated. Use UNITY_IOS instead.    |
| UNITY_WEBGL        | #define directive for WebGL.   |
| UNITY_WP_8_1        | #define directive for Windows Phone 8.1.   | 
| UNITY_PS4        | #define directive for running PlayStation 4 code.  |
| UNITY_XBOXONE      | #define directive for executing Xbox One code.    |
| UNITY_WII     | 	#define directive for compiling/executing code for the Wii console.    | 
| UNITY_SAMSUNGTV     | 	#define directive for executing Samsung TV code.    | 

### 样例  
``` C#
using UnityEngine;
using System.Collections;

public class PlatformDefines : MonoBehaviour {

  void Start () {
    #if UNITY_EDITOR
        Debug.Log("Unity Editor");
    #elif UNITY_IOS
        Debug.Log("Unity iPhone");
    #else
        Debug.Log("Any other platform");
    #endif
  }

}
```

``` C#
function Awake() {
  #if UNITY_EDITOR
    Debug.Log("Unity Editor");
  #endif
    
  #if UNITY_IPHONE
    Debug.Log("Iphone");
  #endif

  #if UNITY_STANDALONE_OSX
    Debug.Log("Stand Alone OSX");
  #endif

  #if UNITY_STANDALONE_WIN
    Debug.Log("Stand Alone Windows");
  #endif
}
```
* [Unity宏定义官方文档](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html)

