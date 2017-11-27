using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MenuOptionOverwrite
{

    [MenuItem("GameObject/UI/Image")]
    static void CreateImage()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("image", typeof(Image));
                go.GetComponent<Image>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
            }
        }
    }

    [MenuItem("GameObject/UI/Text")]
    static void CreateText()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("text", typeof(Text));
                go.GetComponent<Text>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
            }
        }
    }

}
