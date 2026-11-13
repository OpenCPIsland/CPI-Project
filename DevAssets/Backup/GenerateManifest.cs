using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GenerateManifest
{
    // Bundle dependencies
    private static readonly Dictionary<string, string> bundleDependencies = new Dictionary<string, string>
    {
        { "assetbundles/generated/standalonewindows64/quest.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/questmascotitem_rookie.sa.unity3d", "assetbundles/generated/standalonewindows64/questmascotitem_rockhopper.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/tasks.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q04.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c01q01.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c01q02.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c01q04.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d,assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q03.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q04.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d,assetbundles/generated/standalonewindows64/quest.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q06.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q07.sa.unity3d", "assetbundles/generated/standalonewindows64/quest.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q08.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q10.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q02.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q04.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q05.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q06.sa.unity3d,assetbundles/generated/standalonewindows64/quest/auntarctic/c02q07.sa.unity3d,assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/djcadence/c01q01.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/djcadence/c01q05.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/djcadence/c01q02.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d,assetbundles/generated/standalonewindows64/quest/djcadence/c01q03.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/djcadence/c01q05.sa.unity3d", "assetbundles/generated/standalonewindows64/igloo.sa.unity3d,assetbundles/generated/standalonewindows64/mascot.sa.unity3d,assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q03.sa.unity3d", "assetbundles/generated/standalonewindows64/mascot.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q06.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q04.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q08.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q07.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rookie/c01q01.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d,assetbundles/generated/standalonewindows64/quest/rookie/c01q05.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rookie/c01q05.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/auntarctic/c02q02.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rookie/c02q03.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d" },
        { "assetbundles/generated/standalonewindows64/quest/rookie/c02q05.sa.unity3d", "assetbundles/generated/standalonewindows64/quest/rockhopper/c01q01.sa.unity3d,assetbundles/generated/standalonewindows64/quest/rookie/c02q03.sa.unity3d" }
    };

    [MenuItem("Project/Generate Client Content Manifest")]
    public static void GenerateManifestFile()
    {
        string EmbeddedManifest = Application.dataPath + "/Generated/Resources/Configuration/";
        string manifestFilePath = Path.Combine(EmbeddedManifest, "embedded_content_manifest.txt");

        try
        {
            HashSet<string> writtenLines = new HashSet<string>();
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

                    if (!lowerPath.Contains("/resources/"))
                        continue;

                    string relative = GetRelativePathInResources(assetPath);
                    if (string.IsNullOrEmpty(relative))
                        continue;

                    string dir = Path.GetDirectoryName(relative)?.Replace("\\", "/");
                    string fileWithNoExt = Path.GetFileNameWithoutExtension(relative);
                    string ext = Path.GetExtension(relative).TrimStart('.').ToLower();

                    string assetPartNoExt = string.IsNullOrEmpty(dir)
                        ? fileWithNoExt.ToLower()
                        : $"{dir}/{fileWithNoExt}".ToLower();

                    string assetLine = $"";
                    if (assetPartNoExt.Contains("disneyshop_bighero6_logo") || assetPartNoExt.Contains("disneyshop_cars3_logo") || assetPartNoExt.Contains("disneyshop_descendants2_logo") || assetPartNoExt.Contains("disneyshop_ducktales_logo") || assetPartNoExt.Contains("disneyshop_elena_logo") || assetPartNoExt.Contains("disneyshop_findingdory_logo") || assetPartNoExt.Contains("disneyshop_frozen_logo") || assetPartNoExt.Contains("disneyshop_monstersinc_logo") || assetPartNoExt.Contains("disneyshop_olaf_logo") || assetPartNoExt.Contains("disneyshop_tangled_logo") || assetPartNoExt.Contains("disneyshop_toystory_logo") || assetPartNoExt.Contains("disneyshop_zootopia_logo") || assetPartNoExt.Contains("sharedassets_cpilogo") || assetPartNoExt.Contains("homescreen_cpilogo") || assetPartNoExt.Contains("logincreate_disneylogo_en_us") || assetPartNoExt.Contains("logincreate_disneylogo_fr_fr") || assetPartNoExt.Contains("logincreate_disneylogo_pt_br") || assetPartNoExt.Contains("logincreate_disneylogo_es_la") || assetPartNoExt.Contains("logincreate_disneylogo_gold_en_us") || assetPartNoExt.Contains("logincreate_disneylogo_gold_fr_fr") || assetPartNoExt.Contains("logincreate_disneylogo_gold_es_la") || assetPartNoExt.Contains("logincreate_disneylogo_gold_pt_br"))
                    {
                        assetLine = $"asset:{assetPartNoExt}?dl=res&x={ext}".ToLower();
                        if (!writtenLines.Contains($"asset:{assetPartNoExt.Replace("_en_us", "").Replace("_pt_br", "").Replace("_es_la", "").Replace("_fr_fr", "")}?dl=res&x={ext}".ToLower() + "&l=true&ld=en_US"))
                        {
                            writtenLines.Add($"asset:{assetPartNoExt.Replace("_en_us", "").Replace("_pt_br", "").Replace("_es_la", "").Replace("_fr_fr", "")}?dl=res&x={ext}".ToLower() + "&l=true&ld=en_US");
                            writer.WriteLine($"asset:{assetPartNoExt.Replace("_en_us", "").Replace("_pt_br", "").Replace("_es_la", "").Replace("_fr_fr", "")}?dl=res&x={ext}".ToLower() + "&l=true&ld=en_US");
                        }
                    }
                    else
                    {
                        assetLine = $"asset:{assetPartNoExt}?dl=res&x={ext}".ToLower();
                    }


                    if (!writtenLines.Contains(assetLine))
                    {
                        writtenLines.Add(assetLine);
                        writer.WriteLine(assetLine);
                    }
                }

                // Process Bundle assets
                string[] allPaths = AssetDatabase.GetAllAssetPaths();
                var bundleAssets = allPaths
                    .Where(p => !AssetDatabase.IsValidFolder(p))
                    .Where(p =>
                    {
                        var imp = AssetImporter.GetAtPath(p);
                        return imp != null && !string.IsNullOrEmpty(imp.assetBundleName) && !imp.assetBundleName.StartsWith("cdn/");
                    })
                    .ToArray();

                for (int i = 0; i < bundleAssets.Length; i++)
                {
                    string assetPath = bundleAssets[i];
                    EditorUtility.DisplayProgressBar("Generating Manifest", $"Processing bundle assets {i + 1}/{bundleAssets.Length}", (float)(i + 1) / bundleAssets.Length);

                    var importer = AssetImporter.GetAtPath(assetPath);
                    if (importer == null) continue;

                    string bundleLabel = importer.assetBundleName;
                    string bundleVariant = importer.assetBundleVariant;

                    // Remove .txt from bundle labels
                    if (bundleLabel.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        bundleLabel = bundleLabel.Substring(0, bundleLabel.Length - 4);

                    if (!string.IsNullOrEmpty(bundleVariant) && bundleVariant.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        bundleVariant = bundleVariant.Substring(0, bundleVariant.Length - 4);

                    string bundleFileName = string.IsNullOrEmpty(bundleVariant) ? bundleLabel : $"{bundleLabel}.{bundleVariant}";

                    // Look for generated bundle files in StreamingAssets
                    string bundlesRoot = Path.Combine(Application.dataPath, "StreamingAssets/assetbundles/generated").Replace("\\", "/");
                    string foundBundlePath = null;
                    if (Directory.Exists(bundlesRoot))
                    {
                        var allFiles = Directory.GetFiles(bundlesRoot, "*.*", SearchOption.AllDirectories);
                        foreach (var f in allFiles)
                        {
                            string fNorm = f.Replace("\\", "/");

                            // Remove .txt from filename for matching
                            string fNormNoTxt = fNorm;
                            if (fNormNoTxt.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                                fNormNoTxt = fNormNoTxt.Substring(0, fNormNoTxt.Length - 4);

                            if (fNormNoTxt.EndsWith(bundleFileName, StringComparison.OrdinalIgnoreCase))
                            {
                                int idx = fNorm.IndexOf("assetbundles/");
                                if (idx >= 0)
                                {
                                    foundBundlePath = fNorm.Substring(idx).Replace("\\", "/").ToLower();

                                    // Remove .txt from found path
                                    if (foundBundlePath.EndsWith(".txt"))
                                        foundBundlePath = foundBundlePath.Substring(0, foundBundlePath.Length - 4);

                                    break;
                                }
                            }
                        }
                    }

                    string fallbackBundlePath = $"assetbundles/{bundleFileName}".ToLower();
                    if (fallbackBundlePath.EndsWith(".txt"))
                        fallbackBundlePath = fallbackBundlePath.Substring(0, fallbackBundlePath.Length - 4);

                    string bundlePathForManifest = foundBundlePath ?? fallbackBundlePath;

                    // Correct assetPart: relative path after AssetBundles folder
                    string relAsset = assetPath.Replace("\\", "/");

                    string[] folders = relAsset.Split('/');
                    int idxAssetBundles = Array.FindIndex(folders, f => string.Equals(f, "AssetBundles", StringComparison.OrdinalIgnoreCase));
                    string assetPart = idxAssetBundles >= 0
                        ? string.Join("/", folders.Skip(idxAssetBundles + 1))
                        : Path.GetFileNameWithoutExtension(relAsset);

                    assetPart = Path.Combine(Path.GetDirectoryName(assetPart) ?? "", Path.GetFileNameWithoutExtension(assetPart))
                        .Replace("\\", "/").ToLower();

                    // Detect original asset extension for x=
                    string extForX = Path.GetExtension(assetPath).TrimStart('.').ToLower();
                    if (string.IsNullOrEmpty(extForX)) extForX = "";

                    string pFolder = relAsset.ToLower().Remove(0, 7).Replace("/" + assetPart + "." + extForX, "");

                    // Generate manifest line for bundles with proper x= and d=
                    string dValue = "";
                    if (bundleDependencies.ContainsKey(bundlePathForManifest))
                        dValue = bundleDependencies[bundlePathForManifest];

                    string assetLine = $"asset:{assetPart}?dl=bundle:sa-bundle&x={extForX}&b={bundlePathForManifest}&p={pFolder}";
                    if (!writtenLines.Contains(assetLine))
                    {
                        writtenLines.Add(assetLine);
                        writer.WriteLine(assetLine);
                    }
                }

                EditorUtility.ClearProgressBar();

                // Write all bundle files for reference
                string bundlesRootDir2 = Path.Combine(Application.dataPath, "StreamingAssets/assetbundles/generated").Replace("\\", "/");
                if (Directory.Exists(bundlesRootDir2))
                {
                    var bundleFiles = Directory.GetFiles(bundlesRootDir2, "*.*", SearchOption.AllDirectories)
                        .Where(f => !f.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                        .Select(f => f.Replace("\\", "/").ToLower())
                        .ToArray();

                    Array.Sort(bundleFiles, StringComparer.Ordinal);

                    foreach (var f in bundleFiles)
                    {
                        // Remove .txt
                        string cleanPath = f;
                        if (cleanPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            cleanPath = cleanPath.Substring(0, cleanPath.Length - 4);

                        int idx = cleanPath.IndexOf("assetbundles/");
                        if (idx == -1) continue;
                        string clean = cleanPath.Substring(idx);

                        string dValue = "";
                        if (bundleDependencies.ContainsKey(clean))
                            dValue = bundleDependencies[clean];

                        string line = $"bundle:{clean}?d={dValue}&p=0";
                        if (!writtenLines.Contains(line))
                        {
                            writtenLines.Add(line);
                            writer.WriteLine(line);
                        }
                    }
                }

                writer.WriteLine("baseuri:");
                writer.WriteLine("contentversion:66ee6b724b9ae215a0c4f39907ece0fda5c5328a");
                writer.WriteLine("contentmanifesthash:");
            }

            EditorUtility.ClearProgressBar();
            Debug.Log("Manifest generated: " + manifestFilePath);
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError("Error generating manifest file: " + e.Message);
        }
    }

    private static string GetRelativePathInResources(string assetPath)
    {
        int idx = assetPath.ToLower().IndexOf("/resources/");
        if (idx == -1) return null;
        return assetPath.Substring(idx + "/resources/".Length).Replace("\\", "/");
    }
}
