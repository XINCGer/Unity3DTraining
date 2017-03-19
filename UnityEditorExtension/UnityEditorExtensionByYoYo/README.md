## Unity编辑器拓展练习  

Unity编辑器拓展练习--参考泰课Yoyo课程  
* 知识点总结    
    * [MenuItem("ClearMemory/ClearAll",false,1)] 添加菜单栏拓展并设置优先级,false为显示面板，true为不显示面板  
``` C#
[MenuItem("ClearMemory/ClearAll",false,1)]
    static void ClearAll() {
        Debug.Log("清除所有的缓存！");
    }
```     
   * 如果需要在两个按钮之间插入分隔符，则需要控制其优先级相差11。比如一个为1，则另外一个为12。
