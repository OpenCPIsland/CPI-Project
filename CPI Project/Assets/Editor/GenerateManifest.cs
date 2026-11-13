using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Application = UnityEngine.Application;

public class GenerateManifest
{
    public class ManifestData
    {
        public List<string> assets { get; set; }
    }

    private static readonly string[] localizedResourceLanguageSuffixes = new[]
    {
        "_en_us",
        "_pt_br",
        "_fr_fr",
        "_es_la"
    };

    private static readonly HashSet<string> localizedResourceGenericKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "disneyshop/images/disneyshop_bighero6_logo",
        "disneyshop/images/disneyshop_cars3_logo",
        "disneyshop/images/disneyshop_descendants2_logo",
        "disneyshop/images/disneyshop_ducktales_logo",
        "disneyshop/images/disneyshop_elena_logo",
        "disneyshop/images/disneyshop_findingdory_logo",
        "disneyshop/images/disneyshop_frozen_logo",
        "disneyshop/images/disneyshop_monstersinc_logo",
        "disneyshop/images/disneyshop_olaf_logo",
        "disneyshop/images/disneyshop_tangled_logo",
        "disneyshop/images/disneyshop_toystory_logo",
        "disneyshop/images/disneyshop_zootopia_logo",
        "images/sharedassets_cpilogo",
        "images/homescreen_cpilogo",
        "images/logincreate_disneylogo",
        "images/logincreate_disneylogo_gold",
        "textures/bighero6",
        "textures/disneystoresigndescendantscotillion",
        "textures/disneystoresignolafsfrozenadventure",
        "textures/ducktalessign"
    };

    // Bundle dependencies, quests uses these for loading stuff like images and such.
    private static readonly Dictionary<string, string> bundleDependenciesTemplate = new Dictionary<string, string>
    {
        { "assetbundles/generated/PLATFORM/quest.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/questmascotitem_rookie.sa.unity3d", "assetbundles/generated/PLATFORM/questmascotitem_rockhopper.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/tasks.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic.sa.unity3d", "assetbundles/generated/PLATFORM/quest/auntarctic/c02q04.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c01q01.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c01q02.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c01q04.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d,assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q03.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q04.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d,assetbundles/generated/PLATFORM/quest.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q06.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q07.sa.unity3d", "assetbundles/generated/PLATFORM/quest.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q08.sa.unity3d", "assetbundles/generated/PLATFORM/quest/auntarctic/c02q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/auntarctic/c02q10.sa.unity3d", "assetbundles/generated/PLATFORM/quest/auntarctic/c02q02.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q04.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q05.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q06.sa.unity3d,assetbundles/generated/PLATFORM/quest/auntarctic/c02q07.sa.unity3d,assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/djcadence/c01q01.sa.unity3d", "assetbundles/generated/PLATFORM/quest/djcadence/c01q05.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/djcadence/c01q02.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d,assetbundles/generated/PLATFORM/quest/djcadence/c01q03.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/djcadence/c01q05.sa.unity3d", "assetbundles/generated/PLATFORM/igloo.sa.unity3d,assetbundles/generated/PLATFORM/mascot.sa.unity3d,assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rockhopper/c01q03.sa.unity3d", "assetbundles/generated/PLATFORM/mascot.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rockhopper/c01q06.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q04.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rockhopper/c01q08.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q07.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rookie/c01q01.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d,assetbundles/generated/PLATFORM/quest/rookie/c01q05.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rookie/c01q05.sa.unity3d", "assetbundles/generated/PLATFORM/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rookie/c02q03.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/PLATFORM/quest/rookie/c02q05.sa.unity3d", "assetbundles/generated/PLATFORM/quest/rockhopper/c01q01.sa.unity3d,assetbundles/generated/PLATFORM/quest/rookie/c02q03.sa.unity3d" }
    };

    [MenuItem("Project/Generate Client Content Manifest")]
    public static void GenerateManifestFile()
    {
        string EmbeddedManifest = Application.dataPath + "/Generated/Resources/Configuration/";
        string manifestFilePath = Path.Combine(EmbeddedManifest, "embedded_content_manifest.txt");
        string manifestFileJsonPath = Path.Combine(EmbeddedManifest, "embedded_content_manifest.json.json");
        string ContentVersionPath = Application.dataPath + "/Game/Resources/Configuration/ContentVersion.txt";

        string platformFolder = GetPlatformFolderFromActiveBuildTarget();
        if (platformFolder == "unknown")
        {
            Debug.LogError("Unknown platform, aborting manifest generation.");
            return;
        }

        Dictionary<string, string> bundleDependencies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in bundleDependenciesTemplate)
        {
            string k = (kv.Key ?? "").Replace("PLATFORM", platformFolder).Replace("\\", "/").ToLower();

            string v = kv.Value ?? "";
            if (!string.IsNullOrEmpty(v))
            {
                var parts = v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .Select(s => (s ?? "").Replace("PLATFORM", platformFolder).Replace("\\", "/").ToLower());
                v = string.Join(",", parts);
            }
            v = (v ?? "").Replace("\\", "/").ToLower();

            if (!string.IsNullOrEmpty(k))
                bundleDependencies[k] = v;
        }

        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        using var sha1 = SHA1.Create();
        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
        string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        try
        {
            HashSet<string> writtenLines = new HashSet<string>();
            List<string> assetLines = new List<string>();
            using (StreamWriter writer = new StreamWriter(manifestFilePath))
            {
                // Process Resources assets
                string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });
                for (int i = 0; i < guids.Length; i++)
                {
                    string guid = guids[i];
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    if (AssetDatabase.IsValidFolder(assetPath))
                        continue;

                    string lowerPath = assetPath.ToLower();
                    if (lowerPath.Contains("textmesh pro/resources") || lowerPath.Contains("zfbrowser/"))
                        continue;

                    bool isOrientationResource = TryGetOrientationResourceFolder(assetPath, out string resourceOrientation, out string orientationFolder);
                    if (!lowerPath.Contains("/resources/") && !isOrientationResource)
                        continue;

                    if (isOrientationResource && !string.Equals(resourceOrientation, GetOrientationForPlatform(platformFolder), StringComparison.OrdinalIgnoreCase))
                        continue;

                    string relative = isOrientationResource
                        ? GetRelativePathInOrientationResourceFolder(assetPath, orientationFolder)
                        : GetRelativePathInResources(assetPath);
                    if (string.IsNullOrEmpty(relative))
                        continue;

                    string dir = Path.GetDirectoryName(relative)?.Replace("\\", "/");
                    string fileWithNoExt = Path.GetFileNameWithoutExtension(relative);
                    string ext = Path.GetExtension(relative).TrimStart('.').ToLower();

                    string assetPartNoExt = string.IsNullOrEmpty(dir)
                        ? fileWithNoExt.ToLower()
                        : $"{dir}/{fileWithNoExt}".ToLower();

                    string assetLine = $"asset:{assetPartNoExt}?dl=res&x={ext}".ToLowerInvariant();
                    if (TryGetLocalizedResourceGenericKey(assetPartNoExt, out string genericLocalizedKey))
                    {
                        string localizedAssetLine = GetLocalizedResourceManifestLine(genericLocalizedKey, ext);
                        if (!writtenLines.Contains(localizedAssetLine))
                        {
                            writtenLines.Add(localizedAssetLine);
                            writer.WriteLine(localizedAssetLine);
                            assetLines.Add(localizedAssetLine);
                        }
                    }

                    if (!writtenLines.Contains(assetLine))
                    {
                        writtenLines.Add(assetLine);
                        writer.WriteLine(assetLine);

                        assetLines.Add(assetLine);
                    }
                }

                string forcedMissingResource = "asset:rewards/rewardpopup/itemrewardpopupbg_default?dl=res&x=prefab".ToLower();
                if (!writtenLines.Contains(forcedMissingResource))
                {
                    writtenLines.Add(forcedMissingResource);
                    writer.WriteLine(forcedMissingResource);
                    assetLines.Add(forcedMissingResource);
                }

                AddressableAssetSettings addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                if (addressableSettings == null)
                {
                    throw new InvalidOperationException("Addressables settings were not found.");
                }
                foreach (AddressableAssetGroup group in addressableSettings.groups)
                {
                    if (group == null ||
                        group.Name.StartsWith("CDN/", StringComparison.OrdinalIgnoreCase) ||
                        group.Name.IndexOf(".sa.unity3d.txt", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    string groupBundlePath = GetLegacyBundlePath(group.Name, platformFolder);
                    foreach (AddressableAssetEntry entry in group.entries)
                    {
                        string bundlePath = GetLegacyBundlePath(entry, platformFolder);
                        if (!string.IsNullOrEmpty(bundlePath))
                        {
                            groupBundlePath = bundlePath;
                            break;
                        }
                    }

                    HashSet<string> bundlePaths = new HashSet<string>();
                    int groupAssetCount = 0;
                    foreach (AddressableAssetEntry entry in group.entries)
                    {
                        string entryAssetPath = entry.AssetPath;
                        if (string.IsNullOrEmpty(entryAssetPath) || AssetDatabase.IsValidFolder(entryAssetPath))
                        {
                            continue;
                        }
                        if (!entryAssetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidOperationException("Addressable asset path is outside the Assets folder: " + entryAssetPath);
                        }

                        string assetPath = entryAssetPath.Replace("\\", "/");
                        string assetPart = GetAssetPart(assetPath);

                        string extForX = Path.GetExtension(assetPath).TrimStart('.').ToLowerInvariant();
                        string pFolder = assetPath.Substring("Assets/".Length).ToLowerInvariant()
                            .Replace("/" + assetPart + "." + extForX, "");
                        string bundlePathForManifest = GetLegacyBundlePath(entry, platformFolder);
                        if (string.IsNullOrEmpty(bundlePathForManifest))
                        {
                            bundlePathForManifest = groupBundlePath;
                        }
                        bundlePaths.Add(bundlePathForManifest);
                        bool isLocalizedAddressableAsset = TryGetLocalizedResourceGenericKey(assetPart, out string genericLocalizedKey);
                        if (isLocalizedAddressableAsset)
                        {
                            string localizedAssetLine = GetLocalizedAddressableManifestLine(genericLocalizedKey, extForX, bundlePathForManifest, pFolder);
                            if (writtenLines.Add(localizedAssetLine))
                            {
                                writer.WriteLine(localizedAssetLine);
                            }
                        }
                        string assetLine = $"asset:{assetPart}?dl=bundle:sa-bundle&x={extForX}&b={bundlePathForManifest}&p={pFolder}";
                        groupAssetCount++;

                        if (writtenLines.Add(assetLine))
                        {
                            writer.WriteLine(assetLine);
                        }
                    }

                    if (groupAssetCount > 0)
                    {
                        foreach (string bundlePathForManifest in bundlePaths)
                        {
                            string dValue = bundleDependencies.TryGetValue(bundlePathForManifest, out string dependencies) ? dependencies : "";
                            string bundleLine = $"bundle:{bundlePathForManifest}?d={dValue}&p=0";
                            if (writtenLines.Add(bundleLine))
                            {
                                writer.WriteLine(bundleLine);
                            }
                        }
                    }
                }

                EditorUtility.ClearProgressBar();
            }

            File.AppendAllLines(manifestFilePath, new[]
            {
                "baseuri:",
                $"contentversion:{hash}",
                "contentmanifesthash:"
            });

            EditorUtility.ClearProgressBar();
            Debug.Log("Manifest generated: " + manifestFilePath);

            var manifest = new ManifestData { assets = assetLines };
            string jsonOutput = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            File.WriteAllText(manifestFileJsonPath, jsonOutput);

            Debug.Log("Manifest Json generated: " + manifestFileJsonPath);

            File.WriteAllText(ContentVersionPath, hash);

            Debug.Log($"ContentVersion got updated with new sha-1: {hash} and at path: {ContentVersionPath}");

            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError("Error generating manifest file: " + e.Message);
        }
    }

    private static string GetAssetPart(string assetPath)
    {
        string[] folders = assetPath.Replace("\\", "/").Split('/');
        int assetBundlesIndex = Array.FindIndex(folders, folder => string.Equals(folder, "AssetBundles", StringComparison.OrdinalIgnoreCase));
        string assetPart = assetBundlesIndex >= 0
            ? string.Join("/", folders.Skip(assetBundlesIndex + 1))
            : Path.GetFileName(assetPath);
        return Path.Combine(Path.GetDirectoryName(assetPart) ?? "", Path.GetFileNameWithoutExtension(assetPart))
            .Replace("\\", "/").ToLowerInvariant();
    }

    private static string GetLegacyBundlePath(AddressableAssetEntry entry, string platformFolder)
    {
        foreach (string label in entry.labels)
        {
            if (label.EndsWith(".sa.unity3d.txt", StringComparison.OrdinalIgnoreCase))
            {
                return $"assetbundles/generated/{platformFolder}/{label.Substring(0, label.Length - ".txt".Length)}".ToLowerInvariant();
            }
        }
        return null;
    }

    private static string GetLegacyBundlePath(string groupName, string platformFolder)
    {
        string bundleFileName = groupName;
        if (bundleFileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            bundleFileName = bundleFileName.Substring(0, bundleFileName.Length - ".txt".Length);
        }
        return $"assetbundles/generated/{platformFolder}/{bundleFileName}".ToLowerInvariant();
    }

    private static string GetRelativePathInResources(string assetPath)
    {
        int idx = assetPath.ToLower().IndexOf("/resources/");
        if (idx == -1) return null;
        return assetPath.Substring(idx + "/resources/".Length).Replace("\\", "/");
    }

    private static string GetRelativePathInOrientationResourceFolder(string assetPath, string orientationFolder)
    {
        string normalizedPath = assetPath.Replace("\\", "/");
        int idx = normalizedPath.IndexOf(orientationFolder + "/", StringComparison.OrdinalIgnoreCase);
        if (idx == -1) return null;
        return normalizedPath.Substring(idx + orientationFolder.Length + 1);
    }

    private static bool TryGetLocalizedResourceGenericKey(string assetPartNoExt, out string genericKey)
    {
        for (int i = 0; i < localizedResourceLanguageSuffixes.Length; i++)
        {
            string suffix = localizedResourceLanguageSuffixes[i];
            if (assetPartNoExt.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                string candidate = assetPartNoExt.Substring(0, assetPartNoExt.Length - suffix.Length);
                if (localizedResourceGenericKeys.Contains(candidate))
                {
                    genericKey = candidate.ToLowerInvariant();
                    return true;
                }
                break;
            }
        }

        genericKey = null;
        return false;
    }

    private static string GetLocalizedResourceManifestLine(string genericKey, string ext)
    {
        return $"asset:{genericKey}?dl=res&x={ext.ToLowerInvariant()}&l=true&ld=en_US";
    }

    private static string GetLocalizedAddressableManifestLine(string genericKey, string ext, string bundlePath, string pathPrefix)
    {
        return $"asset:{genericKey}?dl=bundle:sa-bundle&x={ext.ToLowerInvariant()}&b={bundlePath}&p={pathPrefix}&l=true&ld=en_US";
    }

    private static bool TryGetOrientationResourceFolder(string assetPath, out string orientation, out string folderName)
    {
        string normalizedPath = assetPath.Replace("\\", "/");
        string[] variants = { "resources_portrait", "resources_landscape" };
        foreach (string variant in variants)
        {
            if (normalizedPath.StartsWith(variant + "/", StringComparison.OrdinalIgnoreCase) ||
                normalizedPath.IndexOf("/" + variant + "/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                orientation = variant == "resources_portrait" ? "portrait" : "landscape";
                folderName = variant;
                return true;
            }
        }

        orientation = null;
        folderName = null;
        return false;
    }

    private static string GetOrientationForPlatform(string platformFolder)
    {
        if (platformFolder == "android" || platformFolder == "ios")
            return "portrait";

        return "landscape";
    }

    private static string GetPlatformFolderFromActiveBuildTarget()
    {
        BuildTarget t = EditorUserBuildSettings.activeBuildTarget;

        if (t == BuildTarget.StandaloneWindows || t == BuildTarget.StandaloneWindows64)
            return "standalonewindows64";
        if (t == BuildTarget.StandaloneOSX)
            return "standaloneosx";
        if (t == BuildTarget.StandaloneLinux64)
            return "standalonelinux64";
        if (t == BuildTarget.Android)
            return "android";
        if (t == BuildTarget.iOS)
            return "ios";
        if (t == BuildTarget.WebGL)
            return "webgl";

        return "unknown";
    }
}
