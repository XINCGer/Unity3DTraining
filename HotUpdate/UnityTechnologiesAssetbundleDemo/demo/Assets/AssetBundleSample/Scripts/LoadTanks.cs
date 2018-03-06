using UnityEngine;
using System.Collections;
using AssetBundles;

/*  On iOS with ODR it's not possible to demonstrate dynamic asset bundle
    variant selection which is shown in this scene. Please use the 
    development asset server instead of ODR on iOS
*/
public class LoadTanks : MonoBehaviour
{
    public string sceneAssetBundle;
    public string sceneName;
    public string textAssetBundle;
    public string textAssetName;
    private bool bundlesLoaded;
    private bool sd, hd, normal, desert, english, danish;
    private string tankAlbedoStyle, tankAlbedoResolution, language;

    void Awake()
    {
        bundlesLoaded = false;
    }

    // Creating the Temp UI for the demo in IMGui.
    void OnGUI()
    {
        if (!bundlesLoaded)
        {
            // GUI Padding
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();

            // GUI Buttons
            // New Line - Get HD/SD
            GUILayout.BeginHorizontal();
            // Display the choice
            GUILayout.Toggle(sd, "");
            // Get player choice
            if (GUILayout.Button("Load SD")) {sd = true; hd = false; tankAlbedoResolution = "sd"; }
            // Display the choice
            GUILayout.Toggle(hd, "");
            // Get player choice
            if (GUILayout.Button("Load HD")) {sd = false; hd = true; tankAlbedoResolution = "hd"; }
            GUILayout.EndHorizontal();

            // New Line - Get Normal/Desert
            GUILayout.BeginHorizontal();
            // Display the choice
            GUILayout.Toggle(normal, "");
            // Get player choice
            if (GUILayout.Button("Normal")) {normal = true; desert = false; tankAlbedoStyle = "normal"; }
            // Display the choice
            GUILayout.Toggle(desert, "");
            // Get player choice
            if (GUILayout.Button("Desert")) {normal = false; desert = true; tankAlbedoStyle = "desert"; }
            GUILayout.EndHorizontal();

            // New Line - Get Language
            GUILayout.BeginHorizontal();
            // Display the choice
            GUILayout.Toggle(english, "");
            // Get player choice
            if (GUILayout.Button("English")) {english = true; danish = false; language = "english"; }
            // Display the choice
            GUILayout.Toggle(danish, "");
            // Get player choice
            if (GUILayout.Button("Danish")) {english = false; danish = true; language = "danish"; }
            GUILayout.EndHorizontal();

            // GUI Padding
            GUILayout.Space(15);

            // Load the Scene
            if (GUILayout.Button("Load Scene"))
            {
                // Remove the buttons
                bundlesLoaded = true;
                // Set the activeVariant
                string[] activeVariants =
                {
                    tankAlbedoStyle + "-" + tankAlbedoResolution,
                    language
                };

                // Show this in the log to make sure it is correct
                Debug.Log(string.Format("Variants: '{0}', '{1}'", activeVariants[0], activeVariants[1]));
                // Load the scene now!
                StartCoroutine(BeginExample(activeVariants));
            }

            // End GUI Padding
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }

    // Use this for initialization
    IEnumerator BeginExample(string[] variants)
    {
        yield return StartCoroutine(Initialize());

        // Set active variants.
        AssetBundleManager.ActiveVariants = variants;

        // Load variant level which depends on variants.
        yield return StartCoroutine(InitializeLevelAsync(sceneName, true));

        // Load additonal assets, in this case a language specific banner
        yield return StartCoroutine(InstantiateGameObjectAsync(textAssetBundle, textAssetName));
    }

    // Initialize the downloading URL.
    // eg. Development server / iOS ODR / web URL
    void InitializeSourceURL()
    {
        // If ODR is available and enabled, then use it and let Xcode handle download requests.
        #if ENABLE_IOS_ON_DEMAND_RESOURCES
        if (UnityEngine.iOS.OnDemandResources.enabled)
        {
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

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
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
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(sceneAssetBundle, levelName, isAdditive);
        if (request == null)
            yield break;

        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }

    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
        {
            Debug.LogError("Failed AssetBundleLoadAssetOperation on " + assetName + " from the AssetBundle " + assetBundleName + ".");
            yield break;
        }
        yield return StartCoroutine(request);

        // Get the Asset.
        GameObject prefab = request.GetAsset<GameObject>();

        // Instantiate the Asset, or log an error.
        if (prefab != null)
            GameObject.Instantiate(prefab);
        else
            Debug.LogError("Failed to GetAsset from request");

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }
}
