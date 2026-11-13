using UnityEngine;
using UnityEditor;
using System.IO;

public static class SnowballGUIDRestorer
{
    [MenuItem("Project/Events/Default/Restore the default snowball material")]
    public static void RestoreGUID()
    {
        string prefabPath = "Assets/Game/World/Prefabs/Snowball.prefab";
        string newGUID = "2760ef39cefb9d246b81f8ce5e39bac6"; // Color Party
        string originalGUID = "1f562df86703cd44ab12d7bab58c0dc9"; // Default

        if (!File.Exists(prefabPath))
        {
            Debug.LogError($"Prefab not found: {prefabPath}");
            return;
        }

        string text = File.ReadAllText(prefabPath);

        if (!text.Contains(newGUID))
        {
            Debug.LogWarning("New GUID not found in prefab. Nothing to replace.");
            return;
        }

        int countBefore = CountOccurrences(text, newGUID);
        Debug.Log($"Found {countBefore} occurrences of GUID {newGUID}");

        string updatedText = text.Replace(newGUID, originalGUID);
        int countAfter = CountOccurrences(updatedText, newGUID);

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

        Debug.Log($"Replaced GUID {newGUID} with {originalGUID} in {prefabPath}");
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

