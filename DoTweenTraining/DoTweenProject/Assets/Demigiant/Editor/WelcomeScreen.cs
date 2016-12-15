using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Appload
{
    static Appload()
    {
        bool hasKey = PlayerPrefs.HasKey("sikiwelcomescreen");
        if (hasKey==false)
        {
            //EditorApplication.update += Update;
            PlayerPrefs.SetInt("sikiwelcomescreen", 1);
            WelcomeScreen.ShowWindow();
        }
    }

    //static void Update() 
    //{
    //    bool isSuccess = EditorApplication.ExecuteMenuItem("Welcome Screen");
    //    if (isSuccess) EditorApplication.update -= Update;
    //}
}

public class WelcomeScreen : EditorWindow
{
    private Texture mSamplesImage;
    private Rect imageRect = new Rect(30f, 90f, 350f, 350f);
    private Rect textRect = new Rect(15f, 15f, 380f, 100f);

    public void OnEnable()
    {
        //this.mWelcomeScreenImage = EditorGUIUtility.Load("WelcomeScreenHeader.png") as Texture;
        //BehaviorDesignerUtility.LoadTexture("WelcomeScreenHeader.png", false, this);
        this.mSamplesImage = LoadTexture("wechat.jpg");
    }

    
    Texture LoadTexture(string name)
    {
        string path = "Assets/Demigiant/Editor/";
        return (Texture)AssetDatabase.LoadAssetAtPath(path + name, typeof(Texture));
    }

    public void OnGUI()
    {
        //GUI.DrawTexture(this.mWelcomeScreenImageRect, this.mWelcomeScreenImage);
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        GUI.Label(this.textRect, "欢迎扫一扫siki的微信，关注微信号\n我会在上面推送一套关于独立游戏开发者的游戏视频教程 免费的！\n时刻更新中！\n这个页面只会显示一次",style);
        GUI.DrawTexture(this.imageRect, this.mSamplesImage);
    }
    
    public static void ShowWindow()
    {
        WelcomeScreen window = EditorWindow.GetWindow<WelcomeScreen>(true, "Hello 你好 我是你们最亲爱的siki老师");
        window.minSize = window.maxSize = new Vector2(410f, 470f);
        UnityEngine.Object.DontDestroyOnLoad(window);
    }
}


