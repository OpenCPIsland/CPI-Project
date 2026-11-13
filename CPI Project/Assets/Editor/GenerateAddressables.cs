using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.Build.Pipeline;
using UnityEngine;
using Disney.Kelowna.Common;

public static class GenerateAddressables
{
    [MenuItem("Project/AssetBundles/Generated/Generate client side AssetBundles")]
    private static void BuildAllAddressables()
    {
        string platform = GetPlatformName(EditorUserBuildSettings.activeBuildTarget);
        string outputDirectory = Path.Combine(Application.dataPath, "StreamingAssets", "assetbundles", "generated", platform);
        EnsureDirectoryIsEmpty(outputDirectory);
        ModifyClientInfoAsset(platform);
        ModifyTextFile(platform);

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        NormalizePathBasedAddresses(settings);
        ConfigureLocalGroups(settings, new List<AddressableAssetGroup>(settings.groups), platform);
        settings.ActivePlayerDataBuilderIndex = GetAddressableBundleNameBuilderIndex(settings);
        AssetDatabase.SaveAssets();

        AddressablesPlayerBuildResult result;
        AddressableAssetSettings.BuildPlayerContent(out result);
        if (!string.IsNullOrEmpty(result.Error))
        {
            throw new InvalidOperationException(result.Error);
        }
        List<string> expectedBundlePaths = GetExpectedBundlePaths(settings, outputDirectory);
        List<string> missingBundlePaths = new List<string>();
        foreach (string bundlePath in expectedBundlePaths)
        {
            if (!File.Exists(bundlePath))
            {
                missingBundlePaths.Add(bundlePath);
            }
        }
        if (missingBundlePaths.Count > 0)
        {
            throw new InvalidOperationException("Addressables build did not generate the expected bundles:\n" + string.Join("\n", missingBundlePaths.ToArray()));
        }
        CleanupPlatformBundles(outputDirectory, platform);
        CleanupPlatformAddressableData(platform);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static int GetAddressableBundleNameBuilderIndex(AddressableAssetSettings settings)
    {
        for (int i = 0; i < settings.DataBuilders.Count; i++)
        {
            if (settings.DataBuilders[i] is AddressableBundleNameBuilder)
            {
                return i;
            }
        }

        AddressableBundleNameBuilder builder = ScriptableObject.CreateInstance<AddressableBundleNameBuilder>();
        builder.name = nameof(AddressableBundleNameBuilder);
        AssetDatabase.AddObjectToAsset(builder, settings);
        if (!settings.AddDataBuilder(builder, false))
        {
            throw new InvalidOperationException("Failed to add the Addressables bundle name builder.");
        }
        AssetDatabase.SaveAssets();
        return settings.DataBuilders.Count - 1;
    }

    private static List<string> GetExpectedBundlePaths(AddressableAssetSettings settings, string outputDirectory)
    {
        List<string> bundlePaths = new List<string>();
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null || IsCdnGroup(group))
            {
                continue;
            }

            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null || !schema.IncludeInBuild)
            {
                continue;
            }

            string bundleName = GetAddressableBundleName(group);
            if (!string.IsNullOrEmpty(bundleName))
            {
                bundlePaths.Add(Path.Combine(outputDirectory, bundleName.Replace('/', Path.DirectorySeparatorChar)));
            }
        }
        return bundlePaths;
    }

    internal static string GetAddressableBundleName(AddressableAssetGroup group)
    {
        foreach (AddressableAssetEntry entry in group.entries)
        {
            foreach (string label in entry.labels)
            {
                const string legacySuffix = ".sa.unity3d.txt";
                if (label.EndsWith(legacySuffix, StringComparison.OrdinalIgnoreCase))
                {
                    return label.Substring(0, label.Length - ".txt".Length);
                }
            }
        }
        return null;
    }

    private static bool IsCdnGroup(AddressableAssetGroup group)
    {
        return group.Name.StartsWith("CDN/", StringComparison.OrdinalIgnoreCase) ||
               group.Name.StartsWith("CDN-", StringComparison.OrdinalIgnoreCase);
    }

    private static void ConfigureLocalGroups(AddressableAssetSettings settings, List<AddressableAssetGroup> groups, string platform)
    {
        string buildPath = Path.Combine(Application.dataPath, "StreamingAssets", "assetbundles", "generated", platform);
        string loadPath = "{UnityEngine.Application.streamingAssetsPath}/assetbundles/generated/" + platform;
        BundledAssetGroupSchema.BundleCompressionMode compression = platform == "webgl"
            ? BundledAssetGroupSchema.BundleCompressionMode.LZ4
            : BundledAssetGroupSchema.BundleCompressionMode.LZMA;
        foreach (AddressableAssetGroup group in groups)
        {
            if (group == null || IsCdnGroup(group))
            {
                continue;
            }
            BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
            if (schema == null)
            {
                continue;
            }
            schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
            schema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.AppendHash;
            schema.Compression = compression;
            schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
            schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
        }

        settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kLocalBuildPath, buildPath);
        settings.profileSettings.SetValue(settings.activeProfileId, AddressableAssetSettings.kLocalLoadPath, loadPath);
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.ProfileModified, settings, true, true);
    }

    private static void NormalizePathBasedAddresses(AddressableAssetSettings settings)
    {
        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null)
            {
                continue;
            }

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (string.Equals(entry.address, entry.AssetPath, StringComparison.OrdinalIgnoreCase))
                {
                    entry.address = entry.AssetPath.ToLowerInvariant();
                }
            }
        }
    }

    private static string GetPlatformName(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "android";
            case BuildTarget.iOS:
                return "ios";
            case BuildTarget.StandaloneOSX:
                return "standaloneosx";
            case BuildTarget.StandaloneWindows64:
                return "standalonewindows64";
            case BuildTarget.StandaloneLinux64:
                return "standalonelinux64";
            case BuildTarget.WebGL:
                return "webgl";
            default:
                throw new InvalidOperationException("Unsupported Addressables build target: " + target);
        }
    }

    private static void ModifyClientInfoAsset(string platform)
    {
        const string assetPath = "Assets/Generated/Resources/Configuration/client_info.asset";
        ClientInfo clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>(assetPath);
        if (clientInfo == null)
        {
            throw new FileNotFoundException("Client info asset was not found.", assetPath);
        }

        SerializedObject serializedObject = new SerializedObject(clientInfo);
        SerializedProperty platformProperty = serializedObject.FindProperty("Platform");
        if (platformProperty == null)
        {
            throw new InvalidOperationException("Client info asset does not contain the Platform property.");
        }
        platformProperty.stringValue = platform;
        serializedObject.ApplyModifiedProperties();
    }

    private static void ModifyTextFile(string platform)
    {
        const string txtFilePath = "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt";
        if (!File.Exists(txtFilePath))
        {
            throw new FileNotFoundException("Embedded content manifest was not found.", txtFilePath);
        }

        string[] lines = File.ReadAllLines(txtFilePath);
        for (int i = 0; i < lines.Length; i++)
        {
            if (ShouldSkipLine(lines[i]))
            {
                continue;
            }
            if (!lines[i].Contains("assetbundles/generated/") && !lines[i].Contains("assetbundles\\generated\\"))
            {
                continue;
            }
            lines[i] = ReplaceGeneratedPlatformSegments(lines[i], platform, "assetbundles/generated/");
            lines[i] = ReplaceGeneratedPlatformSegments(lines[i], platform, "assetbundles\\generated\\");
        }
        File.WriteAllLines(txtFilePath, lines);
    }

    private static bool ShouldSkipLine(string line)
    {
        return line.Contains("asset:worldtraynodeprefabs/androidcontainer?dl=res&x=prefab") ||
               line.Contains("asset:mockauthproject/assetbundles/android/test_cube?dl=res&x=unity3d") ||
               line.Contains("asset:swrveassets.xcassets/app_icons.imageset/1.0.0_android?dl=res&x=png") ||
               line.Contains("asset:swrveassets.xcassets/app_icons.imageset/androidgo?dl=res&x=png") ||
               line.Contains("asset:swrveassets.xcassets/app_icons.imageset/round_android?dl=res&x=png") ||
               line.Contains("asset:swrveassets.xcassets/app_icons.imageset/1.0.0_ios?dl=res&x=png") ||
               line.Contains("asset:definitions/disneystoreitems/disneystoreitem_137_rainbowkiosk?dl=res&x=asset") ||
               line.Contains("asset:definitions/disneystoreitems/disneystoreitem_137_rainbowkiosk.reward?dl=res&x=asset") ||
               line.Contains("asset:definitions/decorations/decoration148_rainbowkiosk?dl=res&x=asset");
    }

    private static string ReplaceGeneratedPlatformSegments(string line, string newPlatform, string marker)
    {
        int searchFrom = 0;
        while (true)
        {
            int index = line.IndexOf(marker, searchFrom, StringComparison.Ordinal);
            if (index < 0)
            {
                break;
            }
            int segmentStart = index + marker.Length;
            int segmentEnd = FindNextDelimiter(line, segmentStart);
            if (segmentEnd <= segmentStart)
            {
                searchFrom = segmentStart;
                continue;
            }
            string currentPlatform = line.Substring(segmentStart, segmentEnd - segmentStart);
            if (IsKnownPlatform(currentPlatform))
            {
                line = line.Substring(0, segmentStart) + newPlatform + line.Substring(segmentEnd);
                segmentEnd = segmentStart + newPlatform.Length;
            }
            searchFrom = segmentEnd;
        }
        return line;
    }

    private static int FindNextDelimiter(string value, int startIndex)
    {
        int best = value.Length;
        char[] delimiters = { '/', '\\', '?', '&', '#' };
        foreach (char delimiter in delimiters)
        {
            int index = value.IndexOf(delimiter, startIndex);
            if (index >= 0 && index < best)
            {
                best = index;
            }
        }
        return best;
    }

    private static bool IsKnownPlatform(string platform)
    {
        return platform == "standalonewindows64" ||
               platform == "standalonelinux64" ||
               platform == "standaloneosx" ||
               platform == "android" ||
               platform == "ios" ||
               platform == "webgl";
    }

    private static void EnsureDirectoryIsEmpty(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return;
        }
        foreach (string file in Directory.GetFiles(path))
        {
            File.Delete(file);
        }
        foreach (string directory in Directory.GetDirectories(path))
        {
            Directory.Delete(directory, true);
        }
    }

    private static void CleanupPlatformBundles(string rootPath, string currentPlatform)
    {
        string[] knownPlatforms =
        {
            "android",
            "ios",
            "standalonelinux64",
            "standaloneosx",
            "standalonewindows64",
            "webgl"
        };
        string generatedRoot = Path.GetDirectoryName(rootPath);
        if (!string.IsNullOrEmpty(generatedRoot) && Directory.Exists(generatedRoot))
        {
            foreach (string platform in knownPlatforms)
            {
                if (string.Equals(platform, currentPlatform, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string otherDirectory = Path.Combine(generatedRoot, platform);
                if (Directory.Exists(otherDirectory))
                {
                    Directory.Delete(otherDirectory, true);
                    string otherMeta = otherDirectory + ".meta";
                    if (File.Exists(otherMeta))
                    {
                        File.Delete(otherMeta);
                    }
                }
            }
        }
        if (!Directory.Exists(rootPath))
        {
            return;
        }
        foreach (string file in Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file).ToLowerInvariant();
            if (fileName.EndsWith(".manifest", StringComparison.Ordinal))
            {
                string metaFile = file + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Delete(metaFile);
                }
                File.Delete(file);
            }
        }
    }

    private static void CleanupPlatformAddressableData(string currentPlatform)
    {
        string[] knownPlatformFolders =
        {
            "Android",
            "Linux",
            "Windows"
        };
        string currentPlatformFolder = null;
        switch (currentPlatform)
        {
            case "android":
                currentPlatformFolder = "Android";
                break;
            case "standalonelinux64":
                currentPlatformFolder = "Linux";
                break;
            case "standalonewindows64":
                currentPlatformFolder = "Windows";
                break;
        }

        string addressableDataDirectory = Path.Combine(Application.dataPath, "AddressableAssetsData");
        foreach (string platformFolder in knownPlatformFolders)
        {
            if (string.Equals(platformFolder, currentPlatformFolder, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string platformDirectory = Path.Combine(addressableDataDirectory, platformFolder);
            if (!Directory.Exists(platformDirectory))
            {
                continue;
            }

            Directory.Delete(platformDirectory, true);
            string platformMeta = platformDirectory + ".meta";
            if (File.Exists(platformMeta))
            {
                File.Delete(platformMeta);
            }
        }
    }

}
