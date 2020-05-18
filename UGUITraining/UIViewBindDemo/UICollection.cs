using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UICollection组件集合
/// </summary>
public sealed class UICollection : MonoBehaviour
{
    [SerializeField]
    internal List<Component> components = new List<Component>();
    public T GetComponent<T>(int index) where T : Object
    {
        return components[index] as T;
    }
}
