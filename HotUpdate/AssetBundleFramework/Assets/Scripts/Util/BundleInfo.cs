using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AllBundleInfo  {

    public List<SingleBundleInfo> BundleInfoList;

}


[System.Serializable]
public class SingleBundleInfo
{

    public string bundleName;

    public uint bundleCRC;

    public string bundleHash128;
}