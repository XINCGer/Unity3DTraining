using UnityEngine;
using System.Collections;
using LuaInterface;

public class ToLua_UnityEngine_RectTransform
{
    public static string GetLocalCornersDefined =
@"            if (count == 1)
            {
                UnityEngine.RectTransform obj = (UnityEngine.RectTransform)ToLua.CheckObject(L, 1, typeof(UnityEngine.RectTransform));
                UnityEngine.Vector3[] arg0 = new UnityEngine.Vector3[4];
                obj.GetLocalCorners(arg0);
                ToLua.Push(L, arg0);
                return 1;
            }";

    public static string GetWorldCornersDefined =
@"            if (count == 1)
            {
                UnityEngine.RectTransform obj = (UnityEngine.RectTransform)ToLua.CheckObject(L, 1, typeof(UnityEngine.RectTransform));
                UnityEngine.Vector3[] arg0 = new UnityEngine.Vector3[4];
                obj.GetWorldCorners(arg0);
                ToLua.Push(L, arg0);
                return 1;
            }";

    [OverrideDefinedAttribute]
    public Vector3[] GetLocalCorners()
    {
        return null;
    }

    [OverrideDefinedAttribute]
    public Vector3[] GetWorldCorners()
    {
        return null;
    }
}
