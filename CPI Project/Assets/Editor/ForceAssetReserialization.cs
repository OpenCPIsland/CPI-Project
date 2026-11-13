using System;
using System.Collections.Generic;
using UnityEditor;

public static class ForceAssetReserialization
{
    [MenuItem("Project/Force Asset Reserialization")]
    public static void ReserializeAllAssets()
    {
        HashSet<string> assetSet = new HashSet<string>(StringComparer.Ordinal);
        AddPaths(assetSet, AssetDatabase.GetAllAssetPaths());

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            if (!string.IsNullOrEmpty(path))
                assetSet.Add(path);
        }

        List<string> assets = new List<string>(assetSet);

        if (assets.Count == 0)
            return;

        try
        {
            Dictionary<string, List<string>> assetsByType = new Dictionary<string, List<string>>();

            for (int i = 0; i < assets.Count; i++)
            {
                string path = assets[i];
                string extension = System.IO.Path.GetExtension(path).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension))
                    extension = "<no extension>";

                if (!assetsByType.TryGetValue(extension, out List<string> typeAssets))
                {
                    typeAssets = new List<string>();
                    assetsByType.Add(extension, typeAssets);
                }

                typeAssets.Add(path);
            }

            int processed = 0;

            foreach (KeyValuePair<string, List<string>> type in assetsByType)
            {
                List<string> typeAssets = type.Value;

                for (int i = 0; i < typeAssets.Count; i++)
                {
                    string path = typeAssets[i];

                    if (IsTaskDefinition(path))
                    {
                        processed++;
                        continue;
                    }

                    float progress = (float)processed / assets.Count;

                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Force Asset Reserialization",
                        $"Reserializing {type.Key} ({i + 1}/{typeAssets.Count})",
                        progress))
                    {
                        return;
                    }

                    AssetDatabase.ForceReserializeAssets(
                        new[] { path },
                        ForceReserializeAssetsOptions.ReserializeAssets);

                    processed++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void AddPaths(HashSet<string> paths, string[] candidates)
    {
        for (int i = 0; i < candidates.Length; i++)
        {
            string path = candidates[i];
            if (path.StartsWith("Assets/", StringComparison.Ordinal) && !AssetDatabase.IsValidFolder(path))
                paths.Add(path);
        }
    }

    private static bool IsTaskDefinition(string path)
    {
        return path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)
            && path.IndexOf("/Definitions/Tasks/", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
