using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using System.Reflection;

public class TestEnum
{
    public TestEnum()
    {

    }

    public void Test(UnityEngine.Space e)
    {

    }
}

public sealed class TestExport
{
    [LuaByteBufferAttribute]
    public delegate void TestBuffer(byte[] buffer);       

    public enum Space
    {
        World = 1
    }

    public int field = 1024;

    public System.Action OnClick = delegate { };

    public delegate void TestRefEvent(ref GameObject go);

    public TestRefEvent OnRefEvent;

    //public int Item { get; set; }
    private int number = 123;
    public int Number
    {
        get
        {            
            return number;
        }

        set
        {            
            number = value;            
        }
    }

    public int this[int pos]
    {
        get { return pos; }
        set { Debugger.Log(value); }
    }

    public int this[char index, int pos]
    {
        get { return 0; }
        set { Debugger.Log(value); }
    }

    [LuaByteBufferAttribute]
    public byte[] buffer;

    public static int get_Item(string pos) { return 0; }
    public static int get_Item(double pos) { return 0; }
    public static int get_Item(int i, int j, int k) { return 0; }

    public static int set_Item(double pos) { return 0; }

    public void TestByteBuffer(TestBuffer tb)
    {

    }

    public int Test(object o, string str)
    {
        Debugger.Log("call Test(object o, string str)");
        return 1;
    }

    public int Test(object o, string str, int n)
    {
        Debugger.Log("call Test(object o, string str, int n)");
        return 111;
    }

    [LuaRenameAttribute(Name = "TestChar")]
    public int Test(char c)
    {
        Debugger.Log("call Test(char c)");
        return 2;
    }

    public int Test()
    {
        return -1;
    }

    public int Test(bool b)
    {
        Debugger.Log("call Test(bool b)");
        return 15;
    }

    public int Test(int[,] objs)
    {
        Debugger.Log("call Test(int[,] objs)");
        return 16;
    }

    public int Test(int i)
    {
        Debugger.Log("call Test(int i)");
        return 3;
    }

    //有这个函数要扔掉上面两个精度不匹配的，因为lua是double
    public int Test(double d)
    {
        Debugger.Log("call Test(double d)");
        return 4;
    }

    public int Test(out int i)
    {
        i = 1024;
        Debugger.Log("call Test(ref int i)");
        return 3;
    }


    public int Test(int i, int j)
    {
        Debugger.Log("call Test(int i, int j)");
        return 5;
    }


    public int Test(string str)
    {
        Debugger.Log("call Test(string str)");
        return 6;
    }

    public static int Test(string str1, string str2)
    {
        Debugger.Log("call static Test(string str1, string str2)");
        return 7;
    }

    public int Test(object o)
    {
        Debugger.Log("call Test(object o)");
        return 8;
    }

    public int Test(params int[] objs)
    {
        Debugger.Log("call Test(params int[] objs)");
        return 12;
    }

    public int Test(params string[] objs)
    {
        Debugger.Log("call Test(params int[] objs)");
        return 13;
    }

    public int Test(string[] objs, bool flag)
    {
        Debugger.Log("call Test(string[] objs, bool flag)");
        return 20;
    }

    public int Test(params object[] objs)
    {
        Debugger.Log("call Test(params object[] objs)");
        return 9;
    }

    public int Test(Space e)
    {
        Debugger.Log("call Test(Space e)");
        return 10;
    }

    public int Test33(ref System.Action<int> action)
    {
        Debugger.Log("ref System.Action action");
        return 14;
    }

    public int TestGeneric<T> (T t) where T : Component
    {
        Debugger.Log("TestGeneric(T t) Call");
        return 11;
    }

    public int TestEnum(Space e)
    {
        Debugger.Log("call TestEnum(Space e)");
        return 10;
    }

    public int TestCheckParamNumber(params int[] ns)
    {
        Debugger.Log("call TestCheckParamNumber(params int[] ns)");
        int n = 0;

        for (int i = 0; i < ns.Length; i++)
        {
            n += ns[i];
        }

        return n;
    }

    public string TestCheckParamString(params string[] ss)
    {
        Debugger.Log("call TestCheckParamNumber(params string[] ss)");
        string str = null;

        for (int i = 0; i < ss.Length; i++)
        {
            str += ss[i];
        }

        return str;
    }

    public static void TestReflection()
    {
        Debugger.Log("call TestReflection()");        
    }

    public static void TestRefGameObject(ref GameObject go)
    {

    }

    public void DoClick()
    {
        OnClick();
    }

    public TestExport()
    {
        Debugger.Log("call TestExport()");
    }

    public TestExport(Vector3[] v)
    {
        Debugger.Log("call TestExport(params Vector3[] v)");
    }

    public TestExport(Vector3 v, string str)
    {
        Debugger.Log("call TestExport(Vector3 v, string str)");
    }

    public TestExport(Vector3 v)
    {
        Debugger.Log("call TestExport(Vector3 v)");
    }

    public Nullable<Vector3> TestNullable(Nullable<Vector3> v)
    {
        Debugger.Log("call TestNullable(Nullable<Vector3> v)");
        return v;
    }
}

public class TestOverload : MonoBehaviour
{
    private string script =
@"                  
        require 'TestExport'                                        
        local out = require 'tolua.out'
        local GameObject = UnityEngine.GameObject                                

        function Test(to)            
            assert(to:Test(1) == 4)            
            local flag, num = to:Test(out.int)
            assert(flag == 3 and num == 1024, 'Test(out)')
            assert(to:Test('hello') == 6, 'Test(string)')
            assert(to:Test(System.Object.New()) == 8)            
            assert(to:Test(true) == 15)
            assert(to:Test(123, 456) == 5)            
            assert(to:Test('123', '456') == 1)
            assert(to:Test(System.Object.New(), '456') == 1)
            assert(to:Test('123', 456) == 9)
            assert(to:Test('123', System.Object.New()) == 9)
            assert(to:Test(1,2,3) == 12)            
            assert(to:Test('hello') == 6)
            assert(TestExport.Test('hello', 'world') == 7)
            assert(to:TestGeneric(GameObject().transform) == 11)
            assert(to:TestCheckParamNumber(1,2,3) == 6)
            assert(to:TestCheckParamString('1', '2', '3') == '123')
            assert(to:Test(TestExport.Space.World) == 10)        
            print(to.this:get(123))
            to.this:set(1, 456)          
            local v = to:TestNullable(Vector3.New(1,2,3)) 
            print(v.z)
        end
    ";
    
    void Awake ()
    {
        LuaState state = new LuaState();
        state.Start();
        LuaBinder.Bind(state);
        Bind(state);        
        state.DoString(script, "TestOverload.cs");

        TestExport to = new TestExport();
        LuaFunction func = state.GetFunction("Test");
        func.Call(to);          
        state.Dispose();                     
    }

    void Bind(LuaState state)
    {
        state.BeginModule(null);
        TestExportWrap.Register(state);
        state.BeginModule("TestExport");
        TestExport_SpaceWrap.Register(state);
        state.EndModule();
        state.EndModule();
    }
}
