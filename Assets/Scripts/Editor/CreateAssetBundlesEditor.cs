using System.IO;
using UnityEditor;
using UnityEngine;

namespace DakotaLib
{
    public static class CreateAssetBundlesEditor
    {
        private static string m_AssetBundleRelativeFolderPath = "AssetBundles";
        private static string streamingAssetsPath = Application.streamingAssetsPath;

        /// <summary>
        /// Builds all AssetBundles in Unity to the StreamingAssets/AssetBundles folder.
        /// </summary>
        /// <param name="DeleteExistingFiles">
        /// Whether or not to delete all files in the AssetBundles folder first (defaults to false).
        /// </param>
        /// <param name="BuildTarget">
        /// What operating system to build to (defaults to Standalone Windows).
        /// </param>
        private static void BuildAllAssetBundles(bool DeleteExistingFiles = false, BuildTarget BuildTarget = BuildTarget.StandaloneWindows)
        {
            string fullFolderPath = Path.Combine(streamingAssetsPath, m_AssetBundleRelativeFolderPath);

            if (Directory.Exists(fullFolderPath) && DeleteExistingFiles)
            {
                Directory.Delete(fullFolderPath, true);
            }
            
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            // Builds all asset bundles to folder
            BuildPipeline.BuildAssetBundles(fullFolderPath, BuildAssetBundleOptions.None, BuildTarget);
        }

        [MenuItem("Assets/Build all AssetBundles (adds to existing folder)")]
        private static void BuildAndAddAllAssetBundles()
        {
            BuildAllAssetBundles(false);
        }

        [MenuItem("Assets/Build all AssetBundles (first WIPES existing folder)")]
        private static void WipeAndBuildAllAssetBundles()
        {
            BuildAllAssetBundles(true);
        }
    }
}
