using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ScanLegacyTextComponents : EditorWindow
{
    private Vector2 scroll;
    private static List<Result> results = new List<Result>();
    private static bool searchScripts = true;
    private static bool searchPrefabs = true;
    private static bool searchScenes = true;
    private static bool searchAssets = true;
    private static bool isScanning = false;

    public class Result
    {
        public string path;
        public int line;
        public string content;
    }



    [MenuItem("Project/Temp/Scan All Assets for Legacy Text")]
    public static void OpenWindow()
    {
        GetWindow<ScanLegacyTextComponents>("Legacy Text Results");
    }

    private static void PerformScan()
    {
        results.Clear();

        var files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
            .Where(f =>
                ((searchScripts && f.EndsWith(".cs")) ||
                 (searchScenes && f.EndsWith(".unity")) ||
                 (searchPrefabs && f.EndsWith(".prefab")) ||
                 (searchAssets && f.EndsWith(".asset"))) &&
                !IsIgnored(f)
            )
            .ToArray();

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];

            if (EditorUtility.DisplayCancelableProgressBar(
                "Scanning Assets",
                file,
                (float)i / files.Length))
            {
                break;
            }

            string assetPath = file.Replace(Application.dataPath, "Assets");

            if (file.EndsWith(".cs"))
            {
                ScanScriptFile(file, assetPath);
            }
            else if (file.EndsWith(".prefab") || file.EndsWith(".unity") || file.EndsWith(".asset"))
            {
                ScanUnityAsset(file, assetPath);
            }
        }

        EditorUtility.ClearProgressBar();
        isScanning = false;
    }

    private static void ScanScriptFile(string file, string assetPath)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(file);
        }
        catch
        {
            return;
        }

        string[] patterns = new string[]
        {
            @"\bTextMesh\b(?!Pro)",
            @"\bUnityEngine\.UI\.Text\b",
            @"\bText\s+[a-zA-Z0-9_]+\b",
            @"GetComponent\s*<\s*Text\s*>"
        };

        for (int line = 0; line < lines.Length; line++)
        {
            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(lines[line], pattern))
                {
                    results.Add(new Result
                    {
                        path = assetPath,
                        line = line + 1,
                        content = lines[line].Trim()
                    });

                    break;
                }
            }
        }
    }

    private static void ScanUnityAsset(string file, string assetPath)
    {
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        if (obj == null)
        {
            return;
        }

        GameObject gameObject = obj as GameObject;
        if (gameObject != null)
        {
            bool hasLegacyText = CheckGameObjectForLegacyText(gameObject);
            if (hasLegacyText)
            {
                results.Add(new Result
                {
                    path = assetPath,
                    line = 0,
                    content = "Contains legacy text components"
                });
            }
        }
    }

    private static bool CheckGameObjectForLegacyText(GameObject gameObject)
    {
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        if (textMesh != null)
        {
            return true;
        }

        Text uiText = gameObject.GetComponent<Text>();
        if (uiText != null)
        {
            return true;
        }

        foreach (Transform child in gameObject.transform)
        {
            if (CheckGameObjectForLegacyText(child.gameObject))
            {
                return true;
            }
        }

        return false;
    }

    static bool IsIgnored(string path)
    {
        string normalized = path.Replace("\\", "/");

        return
            normalized.Contains("/PlayMaker/") ||
            normalized.Contains("/TextMesh Pro/") ||
            normalized.Contains("/Editor/");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField($"Results: {results.Count}", EditorStyles.boldLabel);

        if (isScanning)
        {
            GUI.enabled = false;
            GUILayout.Button("Scanning...", GUILayout.Width(100));
            GUI.enabled = true;
        }
        else
        {
            if (GUILayout.Button("Scan", GUILayout.Width(100)))
            {
                isScanning = true;
                PerformScan();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Search Options:", EditorStyles.boldLabel);
        searchScripts = EditorGUILayout.Toggle(" Scripts (.cs)", searchScripts);
        searchPrefabs = EditorGUILayout.Toggle(" Prefabs (.prefab)", searchPrefabs);
        searchScenes = EditorGUILayout.Toggle(" Unity Scenes (.unity)", searchScenes);
        searchAssets = EditorGUILayout.Toggle(" Assets (.asset)", searchAssets);
        EditorGUIUtility.labelWidth = 0;

        EditorGUILayout.Space();

        if (results.Count == 0 && !isScanning)
        {
            EditorGUILayout.HelpBox("No matches found.", MessageType.Info);
            return;
        }

        if (isScanning)
        {
            EditorGUILayout.HelpBox("Scanning in progress...", MessageType.Info);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var r in results)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(r.path, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Line {r.line}: {r.content}", EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Go To Asset"))
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(r.path);
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }

            if (r.path.EndsWith(".unity"))
            {
                if (GUILayout.Button("Open Scene"))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(r.path, OpenSceneMode.Single);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }
}