using UnityEngine;
using UnityEditor;
using System.IO;

public class VertexColorExtractor : EditorWindow
{
    private Mesh selectedMesh;

    [MenuItem("Project/Editor/Vertex Color Extractor")]
    public static void ShowWindow()
    {
        GetWindow<VertexColorExtractor>("Vertex Color Extractor");
    }

    void OnGUI()
    {
        GUILayout.Label("Select a Mesh", EditorStyles.boldLabel);

        selectedMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", selectedMesh, typeof(Mesh), false);

        if (selectedMesh != null)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Extract Vertex Colors"))
            {
                ExtractVertexColors(selectedMesh);
            }
        }
    }

    void ExtractVertexColors(Mesh mesh)
    {
        if (mesh.colors32 == null || mesh.colors32.Length == 0)
        {
            EditorUtility.DisplayDialog("No Vertex Colors", "This mesh has no vertex colors.", "OK");
            return;
        }

        string filePath = EditorUtility.SaveFilePanel("Save Vertex Colors", "", mesh.name + "_VertexColors.txt", "txt");
        if (string.IsNullOrEmpty(filePath)) return;

        Color32[] colors = mesh.colors32;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Color32 c = colors[i];
                string hex = $"#{c.r:X2}{c.g:X2}{c.b:X2}";
                writer.WriteLine($"Vertex {i}: {c.r},{c.g},{c.b} ({hex})");

                if (i % 100 == 0)
                    EditorUtility.DisplayProgressBar("Extracting Vertex Colors", $"Processing vertex {i}/{colors.Length}", (float)i / colors.Length);
            }
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", $"Vertex colors saved to:\n{filePath}", "OK");
        Debug.Log($"Vertex colors written to: {filePath}");
    }
}
