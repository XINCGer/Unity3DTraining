using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

public static class BreakAtlasTool {
    private static UIAtlas ua;
    private static Texture source;
    private static List<UISpriteData> spriteList = null;
    private static string rootPath = "";
    //--------------拆分图集--------------------

    [MenuItem("AtlasTool/BreakAtlas")]
	static void StartBreakAtlas () {
        rootPath = Application.dataPath + "/BreakAtlas/";

        DirectoryInfo dir = new DirectoryInfo("Assets/TGameResources/SubSys/AllAtlas");

        List<string> list = GetDirectoryFile(dir);

        DirectoryInfo[] dirs = dir.GetDirectories();


        for (int i = 0; i < dirs.Length; i++)
        {
            list.AddRange(GetDirectoryFile(dirs[i]));
            
            DirectoryInfo[] mDirs = null;
            mDirs = dirs[i].GetDirectories();
            if (mDirs.Length != 0) {
                for (int j = 0; j < mDirs.Length; j++)
                {
                    list.AddRange(GetDirectoryFile(mDirs[j]));
                }
            }
        }
        string targetPath = "";
        for (int j = 0; j < list.Count; j++)
        {
            EditorUtility.DisplayProgressBar(string.Format("拆分{0}", list[j]), j.ToString(), j / list.Count);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(list[j], typeof(UIAtlas));
            if (obj == null)
                Debug.Log(list[j] + " is null");

            ua = obj as UIAtlas;
            spriteList = ua.spriteList;
            Rect r = new Rect();
            source = ua.texture;
            Texture2D tt2 = NGUIEditorTools.ImportTexture(ua.texture, true, true, false);
            targetPath = list[j].Replace("Assets/TGameResources/","");
            string[] strs = targetPath.Split('/');
            targetPath = "";
            for (int i = 0; i < strs.Length-1; i++)
            {
                targetPath += "/";
                targetPath += strs[i];
            }

            for (int i = 0; i < spriteList.Count; i++)
            {
                r.x = spriteList[i].x;
                r.y = source.height - spriteList[i].y - spriteList[i].height;
                r.width = spriteList[i].width;
                r.height = spriteList[i].height;

                GetBytes(source, r, ua.name, spriteList[i].name, spriteList[i].paddingLeft, spriteList[i].paddingRight, spriteList[i].paddingTop, spriteList[i].paddingBottom, targetPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.LogError("拆分完毕");
	}

    static void GetBytes(Texture _sourceTex, Rect sourceRect, string atlasName, string spriteName, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom,string targetPath)
    {
        Texture2D sourceTex = _sourceTex as Texture2D;
        int x = Mathf.FloorToInt(sourceRect.x);
        int y = Mathf.FloorToInt(sourceRect.y);
        int width = Mathf.FloorToInt(sourceRect.width);
        int height = Mathf.FloorToInt(sourceRect.height);
        Color[] pix = sourceTex.GetPixels(x, y, width, height);
        
        int mWidth = width + paddingLeft + paddingRight;
        int mHeight = height + paddingTop + paddingBottom;
        
        int count = mWidth * mHeight;
        Texture2D destTex = new Texture2D(mWidth, mHeight);
        for (int i = 0; i < mHeight; i++)
        {
            for (int j = 0; j < mWidth; j++)
            {
                if ((i < paddingBottom || i > paddingBottom + height) || (j<paddingLeft || j>paddingLeft+width))
                {
                    //Debug.LogError("-----------------");
                    destTex.SetPixel(j,i, new Color(0, 0, 0, 0));
                }
                else
                {
                    //destTex.SetPixel(j, i, new Color(0, 0.3f, 0, 1));
                    Color c = sourceTex.GetPixel(x + j - paddingLeft , y + i - paddingBottom);
                    destTex.SetPixel(j, i, c);
                }
            }
        }
        destTex.Apply();
        byte [] bytes ;
        bytes = destTex.EncodeToPNG();

        GetPic(bytes,atlasName,spriteName,targetPath);
    }
    static void GetPic(byte[] bytes, string atlasName, string spriteName,string TargetPath)
    {
        String targetPath = Application.dataPath.Replace("Assets", "") + "BreakAtlasPic" + TargetPath + "/";
        if (!Directory.Exists(targetPath + atlasName))
            Directory.CreateDirectory(targetPath + atlasName);

        File.WriteAllBytes(targetPath + atlasName + "/" + spriteName + ".png", bytes);
        //Debug.LogError("targetPath:" + targetPath+","+atlasName);
    }

    static List<string> GetPath(DirectoryInfo dir)
    {
        List<string> list = new List<string>();
        //获取文件下的子文件夹
        DirectoryInfo[] dirs = dir.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            list.AddRange(GetDirectoryFile(dirs[i]));
        }

        list.AddRange(GetDirectoryFile(dir));
        return list;
    }

    static List<string> GetDirectoryFile(DirectoryInfo dir)
    {
        List<string> list = new List<string>();
        //当前文件夹下的所有Prefab
        FileInfo[] infos = dir.GetFiles();
        foreach (var item in infos)
        {
            if (item.FullName.Contains(".prefab") && !item.FullName.Contains(".prefab.meta"))
            {
                string str = "";
                str = item.FullName;
                str = str.Replace(@"\", @"/");
                str = str.Replace(Application.dataPath, "Assets");

                list.Add(str);
               // Debug.LogError(item.FullName + ",new:" + str);
            }
        }
        return list;
    }

    //--------------拆分图集结束--------------------




    //--------------合新图集-------------------
    [MenuItem("AtlasTool/NewAtlas")]
    static void GetNewAtlas() {
        List<string> list = new List<string>();

        rootPath = "Assets/TGameResources/SubSys/AllAtlas";

        //String targetPath = Application.dataPath.Replace("Assets", "") + "BreakAtlasPic" + TargetPath + "/";
        string picPath = "";
        picPath = "Assets/" + "BreakAtlasPic/SubSys" + "/AllAtlas";

        DirectoryInfo dir = new DirectoryInfo(picPath);
        DirectoryInfo[] dirs = dir.GetDirectories();

        for (int i = 0; i < dirs.Length; i++)
        {
            GetNewAtlas(dirs[i]);
            //Debug.Log("---------" + dirs[i].FullName + "----------");
        }

        AssetDatabase.Refresh();
        Debug.Log("---------------合图集完毕-----------------");
    } 
    /// <summary>
    /// 传进文件夹，将本文件夹下的所有文件打成以本文件夹为名的Atlas
    /// </summary>
    /// <param name="dir"></param>
    private static void GetNewAtlas(DirectoryInfo dir)
    {
        FileInfo[] infos = dir.GetFiles();

        bool haveInfo = false;

        List<string> list = new List<string>();

        foreach (var item in infos)
        {
            if (item.FullName.Contains(".png") && !item.FullName.Contains(".png.meta"))
            {
                haveInfo = true;
                string str = "";
                str = item.FullName;
                str = str.Replace(@"\", @"/");
                str = str.Replace(Application.dataPath, "Assets");

                list.Add(str);
            }
        }

        if (!haveInfo)
        {
            DirectoryInfo[] infos1 = dir.GetDirectories();
            for (int i = 0; i < infos1.Length; i++)
            {
                GetNewAtlas(infos1[i]);

            }
            return;
        }


        List<Texture> texTures = new List<Texture>();
        for (int i = 0; i < list.Count; i++)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(list[i], typeof(Texture));
            FileImporterSetting.SetFileImporterSetting(EFileImporterSettingType.TextureForRGBA32, 1024, AssetDatabase.GetAssetPath(obj));
            texTures.Add(obj as Texture);
            //Debug.LogError("Texture:" +i+":"+ list[i]);
        }

        string dirPath = dir.FullName.Replace(@"\", @"/");

        dirPath = dirPath.Replace(Application.dataPath + "/BreakAtlasPic/", "Assets/TGameResources/");
        
        
        string[] dirPaths = dirPath.Split('/');

        dirPath = "Assets";
        for (int i = 1; i < dirPaths.Length - 1; i++)
        {
            dirPath += "/";
            dirPath += dirPaths[i];
        }


        //Debug.LogError("dirPath:" + dirPath);
        //return;
        //Debug.LogError("dirPath:" + dirPath);


        UnityEngine.Object obj1 = AssetDatabase.LoadAssetAtPath(dirPath + "/" + dirPaths[dirPaths.Length-1] + ".prefab", typeof(UIAtlas));
        ua = obj1 as UIAtlas;

        UIAtlasUtil.UncompressAtlas(dirPath + "/" + dirPaths[dirPaths.Length-1] + ".prefab");

        FileImporterSetting.SetFileImporterSetting(EFileImporterSettingType.TextureForRGBA32, 1024, AssetDatabase.GetAssetPath(ua.spriteMaterial.mainTexture));
         //(ua.spriteMaterial.mainTexture);

        
        spriteList = ua.spriteList;
        List<UIAtlasMaker.SpriteEntry> ui = UIAtlasMaker.CreateSprites(texTures);

        for (int i = 0; i < ui.Count; i++)
        {
            for (int j = 0; j < spriteList.Count; j++)
            {
                if (ui[i].name == spriteList[j].name)
                {
                    ui[i].borderLeft = spriteList[j].borderLeft;
                    ui[i].borderRight = spriteList[j].borderRight;
                    ui[i].borderBottom = spriteList[j].borderBottom;
                    ui[i].borderTop = spriteList[j].borderTop;

                }
            }
        }
        //ua.UpdateAtlas();
        UIAtlasMaker.UpdateAtlas(ua, ui);

        UIAtlasUtil.CompressAtlas(dirPath + "/" + dirPaths[dirPaths.Length-1] + ".prefab");


        //UnityEngine.Object obj1 = AssetDatabase.LoadAssetAtPath("Assets/zcyAtlas/LobbySys_Atlas1.prefab", typeof(UIAtlas));
        //ua = obj1 as UIAtlas;
        AssetDatabase.Refresh();
        Debug.Log("---------图集打包完毕-----------");
    }


    //fullName转成Assets 路径
    static string GetFileAdr(string fullPath) {
        string str = "";
        str = fullPath;
        str = str.Replace(@"\", @"/");
        str = str.Replace(Application.dataPath, "Assets");

        return str;

    }

    //------------合新图集结束-------------------


    [MenuItem("AtlasTool/更新图集")]
    public static void UpdateAtlas()
    {
        List<string> list = new List<string>();

        rootPath = "Assets/TGameResources/SubSys/AllAtlas";
        string picPath = "";
        picPath = "Assets/" + "BreakAtlasPic/SubSys" + "/AllAtlas";

        DirectoryInfo dir = new DirectoryInfo(picPath);
        DirectoryInfo[] dirs = dir.GetDirectories();

        for (int i = 0; i < dirs.Length; i++)
        {
            UpdateAtlas(dirs[i]);
        }

        AssetDatabase.Refresh();
    }

    private static void UpdateAtlas(DirectoryInfo dir)
    {
        FileInfo[] infos = dir.GetFiles();

        bool haveInfo = false;

        List<string> list = new List<string>();

        foreach (var item in infos)
        {
            if (item.FullName.Contains(".png") && !item.FullName.Contains(".png.meta"))
            {
                haveInfo = true;
                string str = "";
                str = item.FullName;
                str = str.Replace(@"\", @"/");
                str = str.Replace(Application.dataPath, "Assets");

                list.Add(str);
            }
        }

        if (!haveInfo)
        {
            DirectoryInfo[] infos1 = dir.GetDirectories();
            for (int i = 0; i < infos1.Length; i++)
            {
                GetNewAtlas(infos1[i]);

            }
            return;
        }


        List<Texture> texTures = new List<Texture>();
        for (int i = 0; i < list.Count; i++)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(list[i], typeof(Texture));
            FileImporterSetting.SetFileImporterSetting(EFileImporterSettingType.TextureForRGBA32, 1024, AssetDatabase.GetAssetPath(obj));
            texTures.Add(obj as Texture);
        }

        string dirPath = dir.FullName.Replace(@"\", @"/");

        dirPath = dirPath.Replace(Application.dataPath + "/BreakAtlasPic/", "Assets/TGameResources/");


        string[] dirPaths = dirPath.Split('/');

        dirPath = "Assets";
        for (int i = 1; i < dirPaths.Length - 1; i++)
        {
            dirPath += "/";
            dirPath += dirPaths[i];
        }


        UnityEngine.Object obj1 = AssetDatabase.LoadAssetAtPath(dirPath + "/" + dirPaths[dirPaths.Length - 1] + ".prefab", typeof(UIAtlas));
        ua = obj1 as UIAtlas;

        UIAtlasUtil.UncompressAtlas(dirPath + "/" + dirPaths[dirPaths.Length - 1] + ".prefab");

        FileImporterSetting.SetFileImporterSetting(EFileImporterSettingType.TextureForRGBA32, 1024, AssetDatabase.GetAssetPath(ua.spriteMaterial.mainTexture));


        spriteList = ua.spriteList;
        List<UIAtlasMaker.SpriteEntry> ui = UIAtlasMaker.CreateSprites(texTures);

        for (int i = 0; i < ui.Count; i++)
        {
            for (int j = 0; j < spriteList.Count; j++)
            {
                if (ui[i].name == spriteList[j].name)
                {
                    ui[i].borderLeft = spriteList[j].borderLeft;
                    ui[i].borderRight = spriteList[j].borderRight;
                    ui[i].borderBottom = spriteList[j].borderBottom;
                    ui[i].borderTop = spriteList[j].borderTop;

                }
            }
        }

        for (int i = 0; i < ui.Count; i++)
        {
            UIAtlasMaker.AddOrUpdate(ua,ui[i]);
        }

        UIAtlasUtil.CompressAtlas(dirPath + "/" + dirPaths[dirPaths.Length - 1] + ".prefab");

        AssetDatabase.Refresh();
    }




    /// <summary>
    /// 不拆分图集只生成目录
    /// </summary>
    [MenuItem("AtlasTool/生成目录")]
    public static void GenerateDirs()
    {
        rootPath = Application.dataPath + "/BreakAtlas/";

        DirectoryInfo dir = new DirectoryInfo("Assets/TGameResources/SubSys/AllAtlas");

        List<string> list = GetDirectoryFile(dir);

        DirectoryInfo[] dirs = dir.GetDirectories();


        for (int i = 0; i < dirs.Length; i++)
        {
            list.AddRange(GetDirectoryFile(dirs[i]));

            DirectoryInfo[] mDirs = null;
            mDirs = dirs[i].GetDirectories();
            if (mDirs.Length != 0)
            {
                for (int j = 0; j < mDirs.Length; j++)
                {
                    list.AddRange(GetDirectoryFile(mDirs[j]));
                }
            }
        }
        string targetPath = "";
        for (int j = 0; j < list.Count; j++)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(list[j], typeof(UIAtlas));
            if (obj == null)
                Debug.Log(list[j] + " is null");

            ua = obj as UIAtlas;
            targetPath = list[j].Replace("Assets/TGameResources/", "");
            string[] strs = targetPath.Split('/');
            targetPath = "";
            for (int i = 0; i < strs.Length - 1; i++)
            {
                targetPath += "/";
                targetPath += strs[i];
            }
            MKDirs(ua.name, targetPath);
        }
    }

    /// <summary>
    /// 创建空的目录
    /// </summary>
    /// <param name="atlasName"></param>
    /// <param name="TargetPath"></param>
    private static void MKDirs(string atlasName, string TargetPath)
    {
        String targetPath = Application.dataPath.Replace("Assets", "") + "BreakAtlasPic" + TargetPath + "/";
        if (!Directory.Exists(targetPath + atlasName))
            Directory.CreateDirectory(targetPath + atlasName);
    }
}
