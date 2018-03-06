using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundles
{
    public class AssetBundlesMenuItems
    {
        const string kSimulationMode = "Assets/AssetBundles/Simulation Mode";

        [MenuItem(kSimulationMode)]
        public static void ToggleSimulationMode()
        {
            AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
        }

        [MenuItem(kSimulationMode, true)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kSimulationMode, AssetBundleManager.SimulateAssetBundleInEditor);
            return true;
        }

        [MenuItem("Assets/AssetBundles/Build AssetBundles")]
        static public void BuildAssetBundles()
        {
            BuildScript.BuildAssetBundles();
        }

        [MenuItem ("Assets/AssetBundles/Build Player (for use with engine code stripping)")]
        static public void BuildPlayer ()
        {
            BuildScript.BuildPlayer();
        }

        [MenuItem("Assets/AssetBundles/Build AssetBundles from Selection")]
        private static void BuildBundlesFromSelection()
        {
            // Get all selected *assets*
            var assets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToArray();
            
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            HashSet<string> processedBundles = new HashSet<string>();

            // Get asset bundle names from selection
            foreach (var o in assets)
            {
                var assetPath = AssetDatabase.GetAssetPath(o);
                var importer = AssetImporter.GetAtPath(assetPath);

                if (importer == null)
                {
                    continue;
                }

                // Get asset bundle name & variant
                var assetBundleName = importer.assetBundleName;
                var assetBundleVariant = importer.assetBundleVariant;
                var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;
                
                // Only process assetBundleFullName once. No need to add it again.
                if (processedBundles.Contains(assetBundleFullName))
                {
                    continue;
                }

                processedBundles.Add(assetBundleFullName);
                
                AssetBundleBuild build = new AssetBundleBuild();

                build.assetBundleName = assetBundleName;
                build.assetBundleVariant = assetBundleVariant;
                build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName);
                
                assetBundleBuilds.Add(build);
            }

            BuildScript.BuildAssetBundles(assetBundleBuilds.ToArray());
        }
    }
}