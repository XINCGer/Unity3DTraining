##Unity编辑器拓展练习  

Unity编辑器拓展练习--参考泰课Yoyo课程  
* 知识点总结     
```C#
// [MenuItem("ClearMemory/ClearAll",false,1)] 添加菜单栏拓展并设置优先级,false为显示面板，true为不显示面板
[MenuItem("ClearMemory/ClearAll",false,1)]
    static void ClearAll() {
        Debug.Log("清除所有的缓存！");
    }
```
