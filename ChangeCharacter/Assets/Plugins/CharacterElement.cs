using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[Serializable]
public class CharacterElement
{
    public string name;
    public string bundleName;
    static Dictionary<string, WWW> wwws = new Dictionary<string, WWW>();
    AssetBundleRequest gameObjectRequest;
    AssetBundleRequest materialRequest;
    AssetBundleRequest boneNameRequest;

    public CharacterElement(string name, string bundleName)
    {
        this.name = name;
        this.bundleName = bundleName;
    }

    public SkinnedMeshRenderer GetSkinnedMeshRenderer()
    {
        GameObject go = (GameObject)Object.Instantiate(gameObjectRequest.asset);
        go.GetComponent<Renderer>().material = (Material)materialRequest.asset;
        return (SkinnedMeshRenderer)go.GetComponent<Renderer>();
    }

    public string[] GetBoneNames()
    {
        var holder = (StringHolder)boneNameRequest.asset;
        return holder.content;
    }

    public WWW WWW
    {
        get
        {
            if (!wwws.ContainsKey(bundleName))
                wwws.Add(bundleName, new WWW(AssetbundleBaseURL + bundleName));
            return wwws[bundleName];
        }
    }

    public bool IsLoaded
    {
        get
        {
            if (!WWW.isDone) return false;

            if (gameObjectRequest == null)
                gameObjectRequest = WWW.assetBundle.LoadAssetAsync("rendererobject", typeof(GameObject));

            if (materialRequest == null)
                materialRequest = WWW.assetBundle.LoadAssetAsync(name, typeof(Material));

            if (boneNameRequest == null)
                boneNameRequest = WWW.assetBundle.LoadAssetAsync("bonenames", typeof(StringHolder));

            if (!gameObjectRequest.isDone) return false;
            if (!materialRequest.isDone) return false;
            if (!boneNameRequest.isDone) return false;

            return true;
        }
    }

    public static string AssetbundleBaseURL
    {
        get
        {
            return "file://" + Application.dataPath + "/assetbundles/";
        }
    }
}
