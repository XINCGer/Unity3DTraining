using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AssetBundles;


public class LoadVariants : MonoBehaviour
{
    const string variantSceneAssetBundle = "variants/variant-scene";
    const string variantSceneName = "VariantScene";

    // The following are used only if app slicing is not enabled.
    private string[] activeVariants;
    private bool bundlesLoaded;             // used to remove the loading buttons

    void Awake()
    {
        activeVariants = new string[1];
        bundlesLoaded = false;
    }

    #if ENABLE_IOS_ON_DEMAND_RESOURCES
    
    /*  On iOS it's not possible to load different asset bundle variants on demand.
        We load asset bundle variant in the same way as usual asset bundle. Depending
        on whether we use ODR or not App Store will serve us correct resource on demand,
        or the resource will be already downloaded within the asset catalog.
    */
    void Start()
    {
        StartCoroutine(BeginExample());
    }
    #else
    void OnGUI()
    {
        if (!bundlesLoaded)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Load SD"))
            {
                activeVariants[0] = "sd";
                bundlesLoaded = true;
                StartCoroutine(BeginExample());
                Debug.Log("Loading SD");
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Load HD"))
            {
                activeVariants[0] = "hd";
                bundlesLoaded = true;
                StartCoroutine(BeginExample());
                Debug.Log("Loading HD");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    #endif

    // Use this for initialization
    IEnumerator BeginExample()
    {
        yield return StartCoroutine(Initialize());

        // Set active variants.
        AssetBundleManager.ActiveVariants = activeVariants;

        // Load variant level which depends on variants.
        yield return StartCoroutine(InitializeLevelAsync(variantSceneName, true));
    }

    // Initialize the downloading URL.
    // eg. Development server / iOS ODR / web URL
    void InitializeSourceURL()
    {
        // If ODR is available and enabled, then use it and let Xcode handle download requests.
        #if ENABLE_IOS_ON_DEMAND_RESOURCES
        if (UnityEngine.iOS.OnDemandResources.enabled)
        {
            AssetBundleManager.overrideBaseDownloadingURL += OverrideDownloadingURLForLocalBundles;
            AssetBundleManager.SetSourceAssetBundleURL("odr://");
            return;
        }
        #endif
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project.
        //      Another approach would be to make this configurable in the standalone player.)
        AssetBundleManager.SetDevelopmentAssetBundleServer();
        return;
        #else
        // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
        AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
        // Or customize the URL based on your deployment or configuration
        //AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
        return;
        #endif
    }

    #if ENABLE_IOS_ON_DEMAND_RESOURCES
    // The following asset bundles do not use ODR
    List<string> localBundles = new List<string>{ "variants/logo" };

    protected string OverrideDownloadingURLForLocalBundles(string baseAssetBundleName)
    {
        if (localBundles.Contains(baseAssetBundleName))
            return "res://";
        return null;
    }

    #endif

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy the game object as we base on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        InitializeSourceURL();

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();

        if (request != null)
            yield return StartCoroutine(request);
    }

    protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(variantSceneAssetBundle, levelName, isAdditive);
        if (request == null)
            yield break;

        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }
}
