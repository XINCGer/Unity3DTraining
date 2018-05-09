using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleData  {

    static BundleData instance;

    public static BundleData Instance
    {
        get
        {
            if (instance == null)
                instance = new BundleData();
            return instance;
        }
    }

    public AssetBundleManifest local_Manifest = null;
    public AssetBundleManifest server_Manifest = null;
    public Dictionary<string, AssetBundle> DicBundle = null;
    public string local_Path = string.Empty;
    public string deposit_DownFilePath = string.Empty;
}
