using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour 
{
    IEnumerator Start () 
    {
        string a = "file://" + Application.dataPath + "/MyData.assetbundle";
            
        WWW www = WWW.LoadFromCacheOrDownload(a, 1);
        yield return www;
            
        if(! string.IsNullOrEmpty(www.error))
        {
            print (www.error);
            return false;
        }
            
        MyData md = www.assetBundle.mainAsset as MyData;
        if(md != null)
        {
            print (md.content[0]);
        }
    }
}