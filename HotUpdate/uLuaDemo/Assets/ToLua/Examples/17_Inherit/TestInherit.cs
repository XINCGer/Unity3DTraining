using UnityEngine;
using System.Collections;
using LuaInterface;

public class TestInherit : MonoBehaviour 
{
    private string script =
    @"  LuaTransform = 
        {                          
        }                                                   

        function LuaTransform.Extend(u)         
            local t = {}                        
            local _position = u.position      
            tolua.setpeer(u, t)     

            t.__index = t
            local get = tolua.initget(t)
            local set = tolua.initset(t)   

            local _base = u.base            

            --重写同名属性获取        
            get.position = function(self)                              
                return _position                
            end            

            --重写同名属性设置
            set.position = function(self, v)                 	                                            
                if _position ~= v then         
                    _position = v                    
                    _base.position = v                                                                      	            
                end
            end

            --重写同名函数
            function t:Translate(...)            
	            print('child Translate')
	            _base:Translate(...)                   
            end    
                           
            return u
        end
        
        
        --既保证支持继承函数，又支持go.transform == transform 这样的比较
        function Test(node)        
            local v = Vector3.one           
            local transform = LuaTransform.Extend(node)                                                         

            local t = os.clock()            
            for i = 1, 200000 do
                transform.position = transform.position
            end
            print('LuaTransform get set cost', os.clock() - t)

            transform:Translate(1,1,1)                                                                     
                        
            local child = transform:FindChild('child')
            print('child is: ', tostring(child))
            
            if child.parent == transform then            
                print('LuaTransform compare to userdata transform is ok')
            end

            transform.xyz = 123
            transform.xyz = 456
            print('extern field xyz is: '.. transform.xyz)
        end
        ";

    LuaState lua = null;

	void Start () 
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif   
        new LuaResLoader();
        lua = new LuaState();        
        lua.Start();
        LuaBinder.Bind(lua);
        lua.DoString(script, "TestInherit.cs");

        float time = Time.realtimeSinceStartup;

        for (int i = 0; i < 200000; i++)
        {
            Vector3 v = transform.position;            
            transform.position = v;
        }

        time = Time.realtimeSinceStartup - time;
        Debugger.Log("c# Transform get set cost time: " + time);

        LuaFunction func = lua.GetFunction("Test");
        func.BeginPCall();
        func.Push(transform);
        func.PCall();
        func.EndPCall();

        lua.CheckTop();
        lua.Dispose();
        lua = null;        
	}

    string tips;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnDestroy()
    {
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
