using UnityEngine;
using UnityEditor;
using System.Collections;

public class CopyAllComponent : EditorWindow
{
    static Component[] copiedComponents;
    [MenuItem("GameObject/Copy Current Components #&C")]
    static void Copy()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("GameObject/Paste Current Components #&P")]
    static void Paste()
    {
        foreach (var targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject || copiedComponents == null) continue;
            foreach (var copiedComponent in copiedComponents)
            {
                if (!copiedComponent) continue;
                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
            }
        }
    }

}
