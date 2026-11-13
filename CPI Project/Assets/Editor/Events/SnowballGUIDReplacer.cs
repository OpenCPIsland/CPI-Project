using UnityEngine;
using UnityEditor;
using System.IO;

public static class SnowballGUIDReplacer
{
    [MenuItem("Project/Events/Color Party/Replace the snowball to the Color Party material")]
    public static void ReplaceGUID()
    {
        string prefabPath = "Assets/Game/World/Prefabs/Snowball.prefab";
        string oldGUID = "1f562df86703cd44ab12d7bab58c0dc9";
        string newGUID = "2760ef39cefb9d246b81f8ce5e39bac6";

        if (!File.Exists(prefabPath))
        {
            Debug.LogError($"Prefab not found: {prefabPath}");
            return;
        }

        string text = File.ReadAllText(prefabPath);

        if (!text.Contains(oldGUID))
        {
            Debug.LogWarning("GUID not found in prefab. Nothing to replace.");
            return;
        }

        int countBefore = CountOccurrences(text, oldGUID);
        Debug.Log($"Found {countBefore} occurrences of GUID {oldGUID}");

        string updatedText = text.Replace(oldGUID, newGUID);
        int countAfter = CountOccurrences(updatedText, oldGUID);

        if (text == updatedText)
        {
            Debug.LogWarning("Replacement made no changes. Aborting write.");
            return;
        }

        File.WriteAllText(prefabPath, updatedText);
        Debug.Log($"Wrote updated prefab file with {countBefore - countAfter} replacements.");

        AssetDatabase.ImportAsset(prefabPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Replaced GUID {oldGUID} with {newGUID} in {prefabPath}");
    }

    private static int CountOccurrences(string text, string target)
    {
        int count = 0, index = 0;
        while ((index = text.IndexOf(target, index)) != -1)
        {
            count++;
            index += target.Length;
        }
        return count;
    }
}

