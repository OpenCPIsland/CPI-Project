using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Project/AssetBundles/Generated/Generate client-side AssetBundles/Build StreamAssets (needs to be done on each editor update)")]
    static void BuildAllAssetBundles()
    {
#if UNITY_IOS
        string assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/ios";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
#elif UNITY_ANDROID
        string assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/android";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
#elif UNITY_STANDALONE_OSX
        string assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standaloneosx";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
#elif UNITY_STANDALONE_WIN
        string assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standalonewindows";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#elif UNITY_STANDALONE_LINUX
        string assetBundleDirectory = "Assets/StreamingAssets/assetbundles/generated/standalonelinux";
        EnsureAndClearDirectory(assetBundleDirectory);
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneLinux64);
#endif
    }

    // Ensure the directory exists and clear its contents if it does
    static void EnsureAndClearDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            // Create directory if it doesn't exist
            Directory.CreateDirectory(path);
        }
        else
        {
            // Clear directory if it exists
            ClearDirectory(path);
        }
    }

    // Function to clear the directory
    static void ClearDirectory(string path)
    {
        // Delete all files in the directory
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        // Delete all subdirectories in the directory
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }
}
