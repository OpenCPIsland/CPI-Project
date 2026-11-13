using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SearchInAssets : EditorWindow
{
    private string searchTermsRaw = "";
    private string outputFileName = "SearchResults.txt";

    private static readonly string[] AllowedExtensions =
    {
        ".prefab",
        ".unity",
        ".cs",
        ".asset",
        ".json",
        ".txt",
        ".xml",
        ".bytes"
    };

    [MenuItem("Project/Editor/Search in Assets")]
    public static void OpenWindow()
    {
        GetWindow<SearchInAssets>("Search in Assets");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Search Terms (one per line)", EditorStyles.boldLabel);
        searchTermsRaw = EditorGUILayout.TextArea(searchTermsRaw, GUILayout.Height(120));

        EditorGUILayout.Space();

        outputFileName = EditorGUILayout.TextField("Output .txt File Name", outputFileName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Search and Export"))
        {
            RunSearch();
        }
    }

    private void RunSearch()
    {
        if (string.IsNullOrWhiteSpace(searchTermsRaw))
        {
            EditorUtility.DisplayDialog("Error", "Please enter at least one search term.", "OK");
            return;
        }

        string[] searchTerms = searchTermsRaw
            .Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (searchTerms.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No valid search terms found.", "OK");
            return;
        }

        string projectPath = Application.dataPath;
        string outputPath = Path.Combine(projectPath, outputFileName);

        List<string> filesToScan = new List<string>();

        foreach (string file in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();
            for (int i = 0; i < AllowedExtensions.Length; i++)
            {
                if (extension == AllowedExtensions[i])
                {
                    filesToScan.Add(file);
                    break;
                }
            }
        }

        StringBuilder results = new StringBuilder();
        results.AppendLine("Search Results:");
        results.AppendLine("Terms:");
        foreach (string term in searchTerms)
            results.AppendLine(" - " + term);
        results.AppendLine();
        results.AppendLine("Matches:");
        results.AppendLine();

        int totalFiles = filesToScan.Count;
        int matchesFound = 0;

        try
        {
            for (int i = 0; i < totalFiles; i++)
            {
                string filePath = filesToScan[i];
                float progress = (float)i / totalFiles;

                EditorUtility.DisplayProgressBar(
                    "Searching Assets",
                    $"Scanning {Path.GetFileName(filePath)} ({i + 1}/{totalFiles})",
                    progress
                );

                string fileContents;

                try
                {
                    fileContents = File.ReadAllText(filePath);
                }
                catch
                {
                    continue;
                }

                foreach (string term in searchTerms)
                {
                    if (fileContents.Contains(term))
                    {
                        matchesFound++;
                        results.AppendLine("File: " + filePath.Replace(projectPath, "Assets"));
                        results.AppendLine("Matched Term: " + term);
                        results.AppendLine();
                        break;
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        File.WriteAllText(outputPath, results.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Search Complete",
            $"Search finished.\n\nMatches found: {matchesFound}\n\nOutput file:\n{outputPath}",
            "OK"
        );
    }
}
