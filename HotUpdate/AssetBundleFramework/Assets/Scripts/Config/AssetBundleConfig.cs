using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleConfig  {

    static AssetBundleConfig instance;

    public static AssetBundleConfig Instance
    {
        get
        {
            if (instance == null)
                instance = new AssetBundleConfig();
            return instance;
        }
    }

    public const string downLoadDir = "Down";

    public const string httpAddress = "http://192.168.1.111/StreamingAssets/";
    public const string server_BundleManifestName = "StreamingAssets";
    public const string local_BundleManifestName = "StreamingAssets";
}
