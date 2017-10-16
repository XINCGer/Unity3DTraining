using UnityEngine;
using System.Collections;
using LuaInterface;
using System.Collections.Generic;
using System;

//需要导出委托类型如下：
//System.Predicate<int>
//System.Action<int>
//System.Comparison<int>
public class UseList : LuaClient
{
    private string script =
        @"
            function Exist2(v)
                return v == 2
            end

            function IsEven(v)
                return v % 2 == 0
            end

            function NotExist(v)
                return false
            end

            function Compare(a, b)
                if a > b then 
                    return 1
                elseif a == b then
                    return 0
                else
                    return -1
                end
            end

            function Test(list, list1)        
                list:Add(123)
                print('Add result: list[0] is '..list[0])
                list:AddRange(list1)
                print(string.format('AddRange result: list[1] is %d, list[2] is %d', list[1], list[2]))

                local const = list:AsReadOnly()
                print('AsReadOnley:'..const[0])    

                index = const:IndexOf(123)
                
                if index == 0 then
                    print('const IndexOf is ok')
                end

                local pos = list:BinarySearch(1)
                print('BinarySearch 1 result is: '..pos)

                if list:Contains(123) then
                    print('list Contain 123')
                else
                    error('list Contains result fail')
                end

                if list:Exists(Exist2) then
                    print('list exists 2')
                else
                    error('list exists result fail')
                end                    
                
                if list:Find(Exist2) then
                    print('list Find is ok')
                else
                    print('list Find error')
                end

                local fa = list:FindAll(IsEven)

                if fa.Count == 2 then
                    print('FindAll is ok')
                end

                --注意推导后的委托声明必须注册, 这里是System.Predicate<int>
                local index = list:FindIndex(System.Predicate_int(Exist2))

                if index == 2 then
                    print('FindIndex is ok')
                end

                index = list:FindLastIndex(System.Predicate_int(Exist2))

                if index == 2 then
                    print('FindLastIndex is ok')
                end                
                
                index = list:IndexOf(123)
                
                if index == 0 then
                    print('IndexOf is ok')
                end

                index = list:LastIndexOf(123)
                
                if index == 0 then
                    print('LastIndexOf is ok')
                end

                list:Remove(123)

                if list[0] ~= 123 then
                    print('Remove is ok')
                end

                list:Insert(0, 123)

                if list[0] == 123 then
                    print('Insert is ok')
                end

                list:RemoveAt(0)

                if list[0] ~= 123 then
                    print('RemoveAt is ok')
                end

                list:Insert(0, 123)
                list:ForEach(function(v) print('foreach: '..v) end)
                local count = list.Count      

                list:Sort(System.Comparison_int(Compare))
                print('--------------sort list over----------------------')
                                
                for i = 0, count - 1 do
                    print('for:'..list[i])
                end

                list:Clear()
                print('list Clear not count is '..list.Count)
            end
        ";


    protected override LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
    }

    //屏蔽，例子不需要运行
    protected override void CallMain() { }

    protected override void OnLoadFinished()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif          
        base.OnLoadFinished();
        luaState.DoString(script, "UseList.cs");
        List<int> list1 = new List<int>();
        list1.Add(1);
        list1.Add(2);
        list1.Add(4);

        LuaFunction func = luaState.GetFunction("Test");
        func.BeginPCall();
        func.Push(new List<int>());
        func.Push(list1);
        func.PCall();
        func.EndPCall();
        func.Dispose();
        func = null;        
    }

    string tips;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    new void OnApplicationQuit()
    {
        base.OnApplicationQuit();
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600), tips);
    }
}
