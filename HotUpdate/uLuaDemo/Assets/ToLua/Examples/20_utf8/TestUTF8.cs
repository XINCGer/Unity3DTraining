using UnityEngine;
using LuaInterface;

public class TestUTF8 : LuaClient
{
    string script =
@"
    local utf8 = utf8

    function Test()        
	    local l1 = utf8.len('你好')
        local l2 = utf8.len('こんにちは')
        print('chinese string len is: '..l1..' japanese sting len: '..l2)     

        local s = '遍历字符串'                                        

        for i in utf8.byte_indices(s) do            
            local next = utf8.next(s, i)                   
            print(s:sub(i, next and next -1))
        end   

        local s1 = '天下风云出我辈'        
        print('风云 count is: '..utf8.count(s1, '风云'))
        s1 = s1:gsub('风云', '風雲')

        local function replace(s, i, j, repl_char)            
	        if s:sub(i, j) == '辈' then
		        return repl_char            
	        end
        end

        print(utf8.replace(s1, replace, '輩'))
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
        luaState.DoString(script);
        LuaFunction func = luaState.GetFunction("Test");
        func.Call();
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
