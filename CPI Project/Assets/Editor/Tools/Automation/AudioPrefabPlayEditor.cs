using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class AudioPrefabPlayEditor : EditorWindow
{
    private GameObject selectedPrefab;
    private string prefabPath;
    private string[] playEvents = new string[0];
    private string[] newPlayEvents = new string[0];

    [MenuItem("Project/Tools/Automation/Igloo Audio Prefab Play Editor")]
    public static void ShowWindow()
    {
        GetWindow<AudioPrefabPlayEditor>("Igloo Audio Play Editor");
    }

    private void OnGUI()
    {
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Audio Prefab", selectedPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        if (selectedPrefab != null)
        {
            prefabPath = AssetDatabase.GetAssetPath(selectedPrefab);

            if (GUILayout.Button("Search Play/"))
            {
                Scan();
            }

            if (playEvents.Length > 0)
            {
                for (int i = 0; i < playEvents.Length; i++)
                {
                    newPlayEvents[i] = EditorGUILayout.TextField($"Play {i + 1}", newPlayEvents[i]);
                }

                if (GUILayout.Button("Apply Overrides"))
                {
                    Apply();
                }
            }
        }
    }

    private void Scan()
    {
        if (string.IsNullOrEmpty(prefabPath) || !File.Exists(prefabPath)) return;

        string text = File.ReadAllText(prefabPath);

        var matches = Regex.Matches(text, @"Play/.*");

        playEvents = matches.Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).ToArray();
        newPlayEvents = (string[])playEvents.Clone();
    }

    private void Apply()
    {
        if (string.IsNullOrEmpty(prefabPath) || !File.Exists(prefabPath)) return;

        string text = File.ReadAllText(prefabPath);

        int index = 0;

        text = Regex.Replace(text, @"Play/.*", m =>
        {
            if (index >= newPlayEvents.Length) return m.Value;
            string replacement = newPlayEvents[index];
            index++;
            return replacement;
        });

        File.WriteAllText(prefabPath, text);

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }
}
