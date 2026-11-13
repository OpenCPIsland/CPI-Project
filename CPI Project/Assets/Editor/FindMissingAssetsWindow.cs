using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class FindMissingAssetsWindow : EditorWindow
{
    private class MissingAssetInfo
    {
        public Object asset;
        public string assetPath;
        public string propertyPath;
        public string fileId;
        public string guid;
        public string description;
    }

    private Vector2 scrollPos;
    private List<MissingAssetInfo> missingAssets = new List<MissingAssetInfo>();
    private bool isScanning = false;

    private string[] ignoreFolders = new string[] {
        "meshes", "fbx", "sourceassets",
        "mesh", "models", "costumes", "costumefbxs", "model"
    };

    [MenuItem("Project/Editor/Find Missing Assets")]
    public static void ShowWindow()
    {
        var window = GetWindow<FindMissingAssetsWindow>();
        window.titleContent = new GUIContent("Find Missing Assets");
        window.minSize = new Vector2(700, 400);
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Scan for Missing Assets", GUILayout.Height(30)) && !isScanning)
        {
            missingAssets.Clear();
            ScanProject();
        }

        EditorGUILayout.Space();

        if (missingAssets.Count > 0)
        {
            EditorGUILayout.LabelField($"Found {missingAssets.Count} missing references:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            foreach (var missing in missingAssets)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField(missing.description, EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("Property:");
                EditorGUILayout.SelectableLabel(missing.propertyPath ?? "", GUILayout.Height(16));
                EditorGUILayout.LabelField("Asset Path:");
                EditorGUILayout.SelectableLabel(missing.assetPath ?? "", GUILayout.Height(16));
                if (!string.IsNullOrEmpty(missing.guid))
                {
                    EditorGUILayout.LabelField("YAML fileID and guid:");
                    EditorGUILayout.SelectableLabel($"fileID: {missing.fileId}    guid: {missing.guid}", GUILayout.Height(16));
                }
                else
                {
                    EditorGUILayout.LabelField("YAML fileID/guid not found (binary or unknown format)");
                }
                if (GUILayout.Button("Select Asset", GUILayout.Width(120)))
                {
                    Selection.activeObject = missing.asset;
                    EditorGUIUtility.PingObject(missing.asset);
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }
        else if (!isScanning)
        {
            EditorGUILayout.LabelField("No missing references found or scan not yet performed.");
        }
    }

    private void ScanProject()
    {
        isScanning = true;
        string[] assetGUIDs = AssetDatabase.FindAssets("");
        int total = assetGUIDs.Length;

        for (int i = 0; i < total; i++)
        {
            string guid = assetGUIDs[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (ShouldIgnorePath(path))
                continue;

            EditorUtility.DisplayProgressBar("Scanning Assets", $"Checking: {path}", i / (float)total);

            Object obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj == null)
                continue;

            SerializedObject serializedObj = null;
            try
            {
                serializedObj = new SerializedObject(obj);
            }
            catch
            {
                continue;
            }

            SerializedProperty prop = serializedObj.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                    {
                        string fileId = null, assetGuid = null;
                        TryGetFileIdAndGuidFromYAML(path, prop.propertyPath, out fileId, out assetGuid);

                        missingAssets.Add(new MissingAssetInfo
                        {
                            asset = obj,
                            assetPath = path,
                            propertyPath = prop.propertyPath,
                            fileId = fileId,
                            guid = assetGuid,
                            description = $"Found missing reference in {Path.GetFileName(path)} at {path}"
                        });
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        isScanning = false;

        Debug.Log($"Scan complete. Found {missingAssets.Count} assets with missing references.");
    }

    private bool ShouldIgnorePath(string path)
    {
        foreach (var folder in ignoreFolders)
        {
            if (path.ToLower().Contains("/" + folder.ToLower() + "/"))
                return true;
        }
        return false;
    }

    private void TryGetFileIdAndGuidFromYAML(string assetPath, string propertyPath, out string fileId, out string guid)
    {
        fileId = null;
        guid = null;

        string absPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        if (!File.Exists(absPath))
            return;
        string[] lines = File.ReadAllLines(absPath);


        string leaf = propertyPath.Contains(".") ? propertyPath.Substring(propertyPath.LastIndexOf('.') + 1) : propertyPath;
        leaf = Regex.Replace(leaf, @"\.Array\.data\[\d+\]", "");
        Regex refRegex = new Regex($@"{leaf}:\s*\{{fileID:\s*([-\d]+),\s*guid:\s*([a-fA-F0-9]+),", RegexOptions.Compiled);

        foreach (var line in lines)
        {
            var m = refRegex.Match(line);
            if (m.Success)
            {
                fileId = m.Groups[1].Value;
                guid = m.Groups[2].Value;
                return;
            }
        }
    }
}