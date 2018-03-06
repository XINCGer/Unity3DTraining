#if ENABLE_IOS_ON_DEMAND_RESOURCES || ENABLE_IOS_APP_SLICING
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS; 
using System.Collections;
using System.IO;


public class BuildResources
{
    [InitializeOnLoadMethod]
    static void SetupResourcesBuild()
    {
        UnityEditor.iOS.BuildPipeline.collectResources += CollectResources;
    }

    static string GetPath(string relativePath)
    {
        string root = Path.Combine(AssetBundles.Utility.AssetBundlesOutputPath, 
                                   AssetBundles.Utility.GetPlatformName());
        return Path.Combine(root, relativePath);
    }
 
    static UnityEditor.iOS.Resource[] CollectResources()
    {
        string manifest = AssetBundles.Utility.GetPlatformName();
        return new Resource[]
        {
            new Resource(manifest, GetPath(manifest)).AddOnDemandResourceTags(manifest),
            new Resource("scene-bundle", GetPath("scene-bundle")).AddOnDemandResourceTags("scene-bundle"),
            new Resource("cube-bundle", GetPath("cube-bundle")).AddOnDemandResourceTags("cube-bundle"),
            new Resource("material-bundle", GetPath("material-bundle")).AddOnDemandResourceTags("material-bundle"),
            
            /*  For now we are replacing '/' character with '>' in resource tags in
                an attempt to work around Xcode crash when opening Resources tab.
                The additional changes needed to support this work around are in
                the implementations of AssetBundleDownloadFromODROperation.
            */
            new Resource("variants/variant-scene", GetPath("variants/variant-scene")).AddOnDemandResourceTags("variants>variant-scene"),
            new Resource("variants/myassets").BindVariant(GetPath("variants/myassets.hd"), "hd")
                                             .BindVariant(GetPath("variants/myassets.sd"), "sd")
                                             .AddOnDemandResourceTags("variants>myassets"),

            new Resource("variants/logo").BindVariant(GetPath("variants/logo.hd"), "hd")
                                         .BindVariant(GetPath("variants/logo.sd"), "sd"),
            /*  Note that in the tanks scene dynamic asset bundle variant selection
                is demonstrated, which is not possible with ODR (the asset bundle
                variants may only depend on the hardware features of the devices).
                We only ensure that the required asset bundles are available.

                Please test the scene without ODR
            */
            new Resource("banner", GetPath("banner.english")).AddOnDemandResourceTags("banner"),
            new Resource("tanks-scene-bundle", GetPath("tanks-scene-bundle")).AddOnDemandResourceTags("tanks-scene-bundle"),
            new Resource("tanks-albedo", GetPath("tanks-albedo.normal-sd")).AddOnDemandResourceTags("tanks-albedo")
        };
    }
}
#endif