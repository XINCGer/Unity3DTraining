using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DeepCopyAllComponent : EditorWindow
{
    [MenuItem("GameObject/Copy All Components #%&C")]
    static void Copy()
    {
        GetAllChilds(Selection.activeGameObject,pri_my_list);
    }

    [MenuItem("GameObject/Paste All Components #%&P")]
    static void Paste()
    {
        GameObject tmpGameObj = Selection.activeGameObject;
        PasteChildComponent(tmpGameObj, pri_my_list);

    }


    public class MyComponentList
    {
        public MyComponentList()
        {
        }

        public List<Component> gameObjList;
        public List<MyComponentList> nextList;
    }

    private static void PasteChildComponent(GameObject gameObj, MyComponentList next)
    {
        if (next.gameObjList != null)
        {
            foreach (var copiedComponent in next.gameObjList)
            {
                if (!copiedComponent) continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObj);
            }
        }

        if (next.nextList != null)
        {
            List<Transform> TmpListTrans = new List<Transform>();
            foreach (Transform item in gameObj.transform)
            {
                TmpListTrans.Add(item);
            }
            int i = 0;
            foreach (var item in next.nextList)
            {
                if (i < TmpListTrans.Count)
                {
                    PasteChildComponent(TmpListTrans[i].gameObject, item);
                }
                i++;
            }
        }
    }


    static MyComponentList pri_my_list = new MyComponentList();

    private static void GetAllChilds(GameObject transformForSearch, MyComponentList next)
    {
        List<Component> childsOfGameobject = new List<Component>();
        next.gameObjList = childsOfGameobject;
        next.nextList = new List<MyComponentList>();

        foreach (var item in transformForSearch.GetComponents<Component>())
        {
            childsOfGameobject.Add(item);
        }
        
        foreach (Transform item in transformForSearch.transform)
        {
            MyComponentList tmpnext = new MyComponentList();
            GetAllChilds(item.gameObject, tmpnext);
            next.nextList.Add(tmpnext);
        }
        return;
    }
 
}