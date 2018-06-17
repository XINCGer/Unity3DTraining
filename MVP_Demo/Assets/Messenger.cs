using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void CallBack();

public delegate void CallBack<T>(T t);

public delegate void CallBack<T, D>(T t, D d);

public delegate void CallBack<T, D, U>(T t, D d, U u);

public delegate T FuncCallBack<T>();

public delegate T FuncCallBack<T, D>(D t);

public delegate T FuncCallBack<T, D, U>(D t, U d);

public delegate T FuncCallBack<T, D, U, V>(D d, U u, V v);

public delegate T FuncCallBack<T, D, U, V, W>(D d, U u, V v, W w);

public class Messenger
{
    public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    private static void OnListenerAdding(string eventkey, Delegate listenerDelegate)
    {
        if (!eventTable.ContainsKey(eventkey))
        {
            eventTable.Add(eventkey, null);
        }
    }

    public static void AddListener(string eventtype, CallBack handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as CallBack) + (handler);
    }

    public static void AddListener<T>(string eventtype, CallBack<T> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as CallBack<T>) + (handler);
    }

    public static void AddListener<T, D>(string eventtype, CallBack<T, D> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as CallBack<T, D>) + (handler);
    }

    public static void AddListener<T, D, U>(string eventtype, CallBack<T, D, U> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as CallBack<T, D, U>) + (handler);
    }

    private static void OnBroadcast(string eventtype)
    {
        if (!eventTable.ContainsKey(eventtype))
        {
            Debug.LogError("不包含此监听");
            return;
        }
    }

    public static void Broadcast(string eventtype)
    {
        OnBroadcast(eventtype);
        CallBack callback;
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as CallBack;
            if (callback != null)
                callback.Invoke();//如果不为空调用，unity2017以下不可简写
        }
    }

    public static void Broadcast<T>(string eventtype, T t)
    {
        OnBroadcast(eventtype);
        CallBack<T> callback;
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as CallBack<T>;
            if (callback != null)
                callback.Invoke(t);//如果不为空调用，unity2017以下不可简写
        }
    }

    public static void Broadcast<T, D>(string eventtype, T t, D d)
    {
        OnBroadcast(eventtype);
        CallBack<T, D> callback;
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as CallBack<T, D>;
            if (callback != null)
                callback.Invoke(t, d);//如果不为空调用，unity2017以下不可简写
        }
    }

    public static void Broadcast<T, D, U>(string eventtype, T t, D d, U u)
    {
        CallBack<T, D, U> callback;
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as CallBack<T, D, U>;
            if (callback != null)
                callback.Invoke(t, d, u); //如果不为空调用，unity2017以下不可简写
        }
    }

    //==========================  上边是无返回值，下边是带返回值  ================================

    public static void AddListener<T>(string eventtype, FuncCallBack<T> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as FuncCallBack<T>) + (handler);
    }

    public static void AddListener<T, D>(string eventtype, FuncCallBack<T, D> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as FuncCallBack<T, D>) + (handler);
    }

    public static void AddListener<T, D, U>(string eventtype, FuncCallBack<T, D, U> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as FuncCallBack<T, D, U>) + (handler);
    }

    public static void AddListener<T, D, U, V>(string eventtype, FuncCallBack<T, D, U, V> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as FuncCallBack<T, D, U, V>) + (handler);
    }

    public static void AddListener<T, D, U, V, W>(string eventtype, FuncCallBack<T, D, U, V, W> handler)
    {
        OnListenerAdding(eventtype, handler);
        eventTable[eventtype] = (eventTable[eventtype] as FuncCallBack<T, D, U, V, W>) + (handler);
    }

    public static T Broadcast<T>(string eventtype)
    {
        OnBroadcast(eventtype);
        FuncCallBack<T> callback;
        T t = default(T);
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as FuncCallBack<T>;
            if (callback != null)
                t = callback.Invoke();//如果不为空调用，unity2017以下不可简写
        }
        return t;
    }

    public static T Broadcast<T, D>(string eventtype, D d)
    {
        OnBroadcast(eventtype);
        FuncCallBack<T, D> callback;
        T t = default(T);
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as FuncCallBack<T, D>;
            if (callback != null)
                t = callback.Invoke(d);//如果不为空调用，unity2017以下不可简写
        }
        return t;
    }

    public static T Broadcast<T, D, U>(string eventtype, D d, U u)
    {
        OnBroadcast(eventtype);
        FuncCallBack<T, D, U> callback;
        T t = default(T);
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as FuncCallBack<T, D, U>;
            if (callback != null)
                t = callback.Invoke(d, u);//如果不为空调用，unity2017以下不可简写
        }
        return t;
    }

    public static T Broadcast<T, D, U, V>(string eventtype, D d, U u, V v)
    {
        FuncCallBack<T, D, U, V> callback;
        T t = default(T);
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as FuncCallBack<T, D, U, V>;
            if (callback != null)
                t = callback.Invoke(d, u, v); //如果不为空调用，unity2017以下不可简写
        }
        return t;
    }

    public static T Broadcast<T, D, U, V, W>(string eventtype, D d, U u, V v, W w)
    {
        FuncCallBack<T, D, U, V, W> callback;
        T t = default(T);
        if (eventTable.ContainsKey(eventtype))
        {
            callback = eventTable[eventtype] as FuncCallBack<T, D, U, V, W>;
            if (callback != null)
                t = callback.Invoke(d, u, v, w); //如果不为空调用，unity2017以下不可简写
        }
        return t;
    }
}

