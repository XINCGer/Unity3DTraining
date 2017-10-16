using UnityEngine;
using System.Collections.Generic;
using LuaInterface;

public sealed class TestAccount
{
    public int id;
    public string name;
    public int sex;

    public TestAccount(int id, string name, int sex)
    {
        this.id = id;
        this.name = name;
        this.sex = sex;
    }
}

public class UseDictionary : MonoBehaviour 
{
    Dictionary<int, TestAccount> map = new Dictionary<int, TestAccount>();

    string script =
        @"              
            function TestDict(map)                        
                local iter = map:GetEnumerator() 
                
                while iter:MoveNext() do
                    local v = iter.Current.Value
                    print('id: '..v.id ..' name: '..v.name..' sex: '..v.sex)                                
                end

                local flag, account = map:TryGetValue(1, nil)

                if flag then
                    print('TryGetValue result ok: '..account.name)
                end

                local keys = map.Keys
                iter = keys:GetEnumerator()
                print('------------print dictionary keys---------------')
                while iter:MoveNext() do
                    print(iter.Current)
                end
                print('----------------------over----------------------')

                local values = map.Values
                iter = values:GetEnumerator()
                print('------------print dictionary values---------------')
                while iter:MoveNext() do
                    print(iter.Current.name)
                end
                print('----------------------over----------------------')                

                print('kick '..map[2].name)
                map:Remove(2)
                iter = map:GetEnumerator() 

                while iter:MoveNext() do
                    local v = iter.Current.Value
                    print('id: '..v.id ..' name: '..v.name..' sex: '..v.sex)                                
                end
            end                        
        ";

	void Awake () 
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        map.Add(1, new TestAccount(1, "水水", 0));
        map.Add(2, new TestAccount(2, "王伟", 1));
        map.Add(3, new TestAccount(3, "王芳", 0));

        LuaState luaState = new LuaState();
        luaState.Start();
        BindMap(luaState);

        luaState.DoString(script, "UseDictionary.cs");
        LuaFunction func = luaState.GetFunction("TestDict");
        func.BeginPCall();
        func.Push(map);
        func.PCall();
        func.EndPCall();

        func.Dispose();
        func = null;
        luaState.CheckTop();
        luaState.Dispose();
        luaState = null;
    }

    void OnApplicationQuit()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif        
    }

    string tips = "";

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400), tips);
    }

    //示例方式，方便删除，正常导出无需手写下面代码
    void BindMap(LuaState L)
    {
        L.BeginModule(null);
        TestAccountWrap.Register(L);
        L.BeginModule("System");
        L.BeginModule("Collections");
        L.BeginModule("Generic");
        System_Collections_Generic_Dictionary_int_TestAccountWrap.Register(L);
        System_Collections_Generic_KeyValuePair_int_TestAccountWrap.Register(L);
        L.BeginModule("Dictionary");
        System_Collections_Generic_Dictionary_int_TestAccount_KeyCollectionWrap.Register(L);
        System_Collections_Generic_Dictionary_int_TestAccount_ValueCollectionWrap.Register(L);
        L.EndModule();
        L.EndModule();
        L.EndModule();
        L.EndModule();
        L.EndModule();
    }
}
