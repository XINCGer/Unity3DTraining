using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.IO;
using UnityEditor;

public class CodeExporter
{
    private const string prefix = "Ex_";
    private const string CodeGenPath = "Assets/Script/Generate/";

    private static List<Type> baseTypeList = new List<Type>()
    {
        typeof(int),
        typeof(bool),
        typeof(double),
        typeof(float),
        //TODO:待完善其他值类型
    };

    private static Dictionary<Type, string> type2ConverterMap = new Dictionary<Type, string>()
    {
        {typeof(int),"Convert.ToInt32({0});"},
        {typeof(bool),"Convert.ToBoolean({0});"},
        {typeof(double),"Convert.ToDouble({0});"},
        {typeof(float),"Convert.ToSingle({0});"},
    };

    [MenuItem("CodeExporter/生成代码")]
    public static void Export()
    {
        var typeList = CodeExportSetting.ExportTypeList;

        foreach (var item in typeList)
        {
            GenCode(item);
        }
    }

    private static void GenCode(Type type)
    {
        var sb = new StringBuilder();

        //添加命名空间
        sb.AppendLine("using System.Collections;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using System;");

        //生成class定义
        sb.Append("public partial class " + type.ToString() + "\n{\n");

        //生成自适应Excecute代码块
        sb.Append("\tpublic object Excecute(string funcName, params object[] param)\n\t{\n");

        //生成包装方法代码块
        MethodInfo[] infos = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var md in infos)
        {
            if (!md.Name.StartsWith(prefix))
            {
                continue;
            }
            var pms = md.GetParameters();
            var returnType = md.ReturnType;
            if (pms.Length == 0)
            {
                sb.Append("\t\tif(funcName == \"" + md.Name + "\")\n\t\t{\n");
                if (returnType == null || returnType == typeof(void))
                {
                    sb.Append("\t\t\tthis." + md.Name + "();\n\t\t}\n");
                }
                else
                {
                    sb.Append("\t\t\treturn this." + md.Name + "();\n\t\t}\n");
                }
            }
            else
            {
                sb.Append("\t\tif(funcName == \"" + md.Name + "\")\n\t\t{\n");
                var argvs = "";
                for (int i = 0; i < pms.Length; i++)
                {
                    var pm = pms[i];
                    var pmType = pm.ParameterType;
                    string argDesc = "";
                    if (isBaseType(pmType))
                    {
                        var converter = string.Format(GetConverterByType(pmType), "param[" + i + "]");
                        argDesc = "var arg_" + i + " = " + converter + ";";
                    }
                    else
                    {
                        argDesc = "var arg_" + i + " = " + "param[" + i + "]" + " as " + pmType + ";";
                    }
                    sb.AppendLine("\t\t\t" + argDesc);
                    if (i < pms.Length - 1)
                    {
                        argvs += "arg_" + i + ", ";
                    }
                    else
                    {
                        argvs += "arg_" + i;
                    }
                }
                if (returnType == null || returnType == typeof(void))
                {
                    sb.Append("\t\t\tthis." + md.Name + "(" + argvs + ");\n\t\t}\n");
                }
                else
                {
                    sb.Append("\t\t\treturn this." + md.Name + "(" + argvs + ");\n\t\t}\n");
                }
            }
        }
        sb.AppendLine("\t\treturn 0;");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        var filePath = CodeGenPath + type.ToString() + "Bind.cs";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        using (var sw = new StreamWriter(filePath))
        {
            sw.Write(sb.ToString());
        }
        AssetDatabase.Refresh();
    }

    public static bool isBaseType(Type type)
    {
        if (baseTypeList.Contains(type))
        {
            return true;
        }
        return false;
    }

    public static string GetConverterByType(Type type)
    {
        var converter = "";
        if (type2ConverterMap.TryGetValue(type, out converter))
        {
            return converter;
        }
        return string.Empty;
    }
}
