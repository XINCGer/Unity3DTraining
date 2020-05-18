using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIExporter
{
    private static Dictionary<string, int> uiExportElementDic = new Dictionary<string, int>();
    private static Dictionary<string, string> uiExportTypeElementDic = new Dictionary<string, string>();
    private static List<Type> ExportComponentTypes = new List<Type>() { typeof(Button),typeof(InputField),typeof(Dropdown),typeof(Toggle),typeof(Slider),
        typeof(ScrollRect),typeof(Scrollbar)};
    private static List<Type> ExportPropertyTypes = new List<Type>() { typeof(Image), typeof(RawImage), typeof(Text), typeof(RectTransform), typeof(Transform) };
    private static UICollection uiCollection;
    private static int UIComponentIndex = -1;
    private const string UIExportPrefabPath = "Assets/Resources/UI/";
    private const string UIViewTag = "UIView";
    private const string UIPropertyTag = "UIProperty";
    private const string UIIgnoreTag = "UIIgnore";
    private const string UIExportCSViewPath = "Assets/Code/Logic/Demo/UIBindView/";

    [MenuItem("GameObject/UI/ColaUI/ExportUIView", false, 1)]
    public static void ExportUIView()
    {
        var uiObj = Selection.activeGameObject;
        if (null == uiObj) return;
        var UIViewName = uiObj.name;
        if (!UIViewName.StartsWith("UI", StringComparison.CurrentCultureIgnoreCase))
        {
            UIViewName = "UI" + UIViewName;
        }
        UIViewName = UIViewName.Substring(0, 2).ToUpper() + UIViewName.Substring(2);  //ToUpperFirst

        uiObj.name = UIViewName;
        uiExportTypeElementDic.Clear();
        uiExportElementDic.Clear();
        uiCollection = null;
        UIComponentIndex = -1;

        ProcessUIPrefab(uiObj);
        GenUIViewCode(UIViewName);
        var prefabPath = UIExportPrefabPath + UIViewName + ".prefab";
        CreateOrReplacePrefab(uiObj, prefabPath);
        AssetDatabase.Refresh();
    }
    private static void ProcessUIPrefab(GameObject gameObject)
    {
        if (null == gameObject) return;
        if (gameObject.CompareTag(UIViewTag))
        {
            uiCollection = gameObject.GetComponent<UICollection>();
            if (null == uiCollection)
            {
                uiCollection = gameObject.AddComponent<UICollection>();
            }
            uiCollection.components.Clear();
        }
        foreach (Transform transform in gameObject.transform)
        {
            if (transform.CompareTag(UIIgnoreTag))
            {
                continue;
            }
            ProcessUIPrefab(transform.gameObject);
            bool isHandled = false;
            foreach (var type in ExportComponentTypes)
            {
                var UIComp = transform.GetComponent(type);
                if (null != UIComp)
                {
                    UIComponentIndex++;
                    uiCollection.components.Add(UIComp);
                    var componentName = "m_" + transform.name;
                    uiExportElementDic[componentName] = UIComponentIndex;
                    uiExportTypeElementDic[componentName] = type.ToString();
                    isHandled = true;
                    break;
                }
            }
            if (isHandled) continue;
            foreach (var type in ExportPropertyTypes)
            {
                var UIComp = transform.GetComponent(type);
                if (null != UIComp && transform.CompareTag(UIPropertyTag))
                {
                    UIComponentIndex++;
                    uiCollection.components.Add(UIComp);
                    var componentName = "m_" + transform.name;
                    uiExportElementDic[componentName] = UIComponentIndex;
                    uiExportTypeElementDic[componentName] = type.ToString();
                    isHandled = true;
                    break;
                }
            }
        }
    }
    private static void GenUIViewCode(string UIViewName)
    {
        var codePath = UIExportCSViewPath + UIViewName + ".BindView.cs";

        StringBuilder sb = new StringBuilder(16);
        sb.Append("///[[Notice:This cs uiview file is auto generate by UIViewExporter，don't modify it manually! --]]\n\n");
        sb.Append("public partial class " + UIViewName + " : UIBase\n{\n");
        if (uiExportTypeElementDic.Count > 0)
        {
            foreach (var item in uiExportTypeElementDic)
            {
                sb.Append("\tprivate ").Append(item.Value).Append(" ").Append(item.Key).Append(";\n");
            }
        }
        sb.Append("\tprotected override void BindView()\n\t{\n\t\tvar UICollection = this.transform.GetComponent<UICollection>();\n");
        if (uiExportElementDic.Count > 0)
        {
            foreach (var item in uiExportElementDic)
            {
                sb.Append("\t\t").Append(item.Key).Append(" = UICollection.GetComponent<").Append(uiExportTypeElementDic[item.Key]).Append(">(").Append(item.Value).Append(");\n");
            }
        }
        sb.Append("\t}\n");
        sb.Append("}\n");

        File.WriteAllText(codePath, sb.ToString());
    }

    public static void CreateOrReplacePrefab(GameObject gameobject, string path, ReplacePrefabOptions options = ReplacePrefabOptions.ConnectToPrefab)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        if (prefab != null)
        {
            PrefabUtility.ReplacePrefab(gameobject, prefab, options);
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            PrefabUtility.CreatePrefab(path, gameobject, options);
        }
    }
}
