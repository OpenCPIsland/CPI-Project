using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MeshVertexColorReplacer : EditorWindow
{
    [System.Serializable]
    private class ColorReplacement
    {
        public Color findColor = Color.white;
        public Color replaceColor = Color.black;
    }

    private Mesh sourceMesh;
    private Material[] materials;
    private List<ColorReplacement> replacements = new List<ColorReplacement>();

    private Vector2 scroll;
    private Vector2 colorListScroll;
    private Dictionary<Color32, int> detectedColors = new Dictionary<Color32, int>();

    [MenuItem("Project/Editor/Mesh Vertex Color Replacer")]
    private static void OpenWindow()
    {
        GetWindow<MeshVertexColorReplacer>("Vertex Color Replacer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Source Mesh", EditorStyles.boldLabel);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField(sourceMesh, typeof(Mesh), false);

        if (sourceMesh != null)
        {
            int subMeshCount = sourceMesh.subMeshCount;
            if (materials == null || materials.Length != subMeshCount)
                materials = new Material[subMeshCount];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Materials (per Submesh)", EditorStyles.boldLabel);
            for (int i = 0; i < subMeshCount; i++)
            {
                materials[i] = (Material)EditorGUILayout.ObjectField(
                    $"Submesh {i}",
                    materials[i],
                    typeof(Material),
                    false
                );
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Display Vertex Colors") && sourceMesh != null)
            ScanVertexColors();

        DrawDetectedColors();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Color Replacements", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < replacements.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Entry {i + 1}", EditorStyles.boldLabel);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                replacements.RemoveAt(i);
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();

            replacements[i].findColor = EditorGUILayout.ColorField("Find Color", replacements[i].findColor);
            replacements[i].replaceColor = EditorGUILayout.ColorField("Replace With", replacements[i].replaceColor);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Color Replacement"))
            replacements.Add(new ColorReplacement());

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(sourceMesh == null);
        if (GUILayout.Button("Create Mesh Copy With Replaced Colors", GUILayout.Height(40)))
            CreateMeshCopy();
        EditorGUI.EndDisabledGroup();
    }

    private void DrawDetectedColors()
    {
        if (detectedColors.Count == 0) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Detected Vertex Colors", EditorStyles.boldLabel);

        colorListScroll = EditorGUILayout.BeginScrollView(colorListScroll, GUILayout.Height(260));
        foreach (var entry in detectedColors)
        {
            Color32 c = entry.Key;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            Rect r = GUILayoutUtility.GetRect(30, 18);
            EditorGUI.DrawRect(r, c);

            EditorGUILayout.LabelField(GetColorName(c), GUILayout.Width(120));
            EditorGUILayout.LabelField(GetHexCode(c), GUILayout.Width(90));
            EditorGUILayout.LabelField($"Vertices: {entry.Value}", GUILayout.Width(90));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"RGBA ({c.r}, {c.g}, {c.b}, {c.a})");

            if (GUILayout.Button("Copy HEX", GUILayout.Width(80)))
                EditorGUIUtility.systemCopyBuffer = GetHexCode(c);

            if (GUILayout.Button("Copy RGBA", GUILayout.Width(80)))
                EditorGUIUtility.systemCopyBuffer = $"RGBA({c.r},{c.g},{c.b},{c.a})";

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }

    private void ScanVertexColors()
    {
        detectedColors.Clear();
        Color32[] colors = sourceMesh.colors32;

        if (colors == null || colors.Length == 0)
        {
            EditorUtility.DisplayDialog("No Vertex Colors", "Mesh has no vertex colors.", "OK");
            return;
        }

        foreach (Color32 c in colors)
        {
            if (detectedColors.ContainsKey(c))
                detectedColors[c]++;
            else
                detectedColors[c] = 1;
        }
    }

    private void CreateMeshCopy()
    {
        string assetPath = AssetDatabase.GetAssetPath(sourceMesh);
        string dir = Path.GetDirectoryName(assetPath);
        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, sourceMesh.name + ".asset"));

        Mesh newMesh = new Mesh();
        newMesh.name = sourceMesh.name;

        // --- Copy geometry ---
        newMesh.vertices = sourceMesh.vertices;
        newMesh.normals = sourceMesh.normals;
        newMesh.tangents = sourceMesh.tangents;

        // --- Apply Color32 replacement ---
        Color32[] srcColors = sourceMesh.colors32;
        if (srcColors != null && srcColors.Length > 0)
            newMesh.colors32 = ApplyReplacements32(srcColors);

        // --- Copy all UV channels ---
        newMesh.uv = sourceMesh.uv;
        newMesh.uv2 = sourceMesh.uv2;
        newMesh.uv3 = sourceMesh.uv3;
        newMesh.uv4 = sourceMesh.uv4;
        newMesh.uv5 = sourceMesh.uv5;
        newMesh.uv6 = sourceMesh.uv6;
        newMesh.uv7 = sourceMesh.uv7;
        newMesh.uv8 = sourceMesh.uv8;

        // --- Copy submeshes ---
        newMesh.subMeshCount = sourceMesh.subMeshCount;
        for (int s = 0; s < sourceMesh.subMeshCount; s++)
            newMesh.SetTriangles(sourceMesh.GetTriangles(s), s);

        // --- Bone weights ---
#if UNITY_2019_1_OR_NEWER
        if (sourceMesh.boneWeights != null && sourceMesh.boneWeights.Length > 0)
        {
            newMesh.boneWeights = sourceMesh.boneWeights;
            newMesh.bindposes = sourceMesh.bindposes;
        }
#endif

        newMesh.RecalculateBounds();

        AssetDatabase.CreateAsset(newMesh, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = newMesh;
        EditorUtility.FocusProjectWindow();
    }

    private Color32[] ApplyReplacements32(Color32[] source)
    {
        Color32[] c32 = new Color32[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            Color c = source[i];
            foreach (var r in replacements)
            {
                if (ColorsMatch(c, r.findColor))
                {
                    c = r.replaceColor;
                    break;
                }
            }
            c32[i] = c; // Color -> Color32 conversion
        }
        return c32;
    }

    private bool ColorsMatch(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.0039f &&
               Mathf.Abs(a.g - b.g) < 0.0039f &&
               Mathf.Abs(a.b - b.b) < 0.0039f &&
               Mathf.Abs(a.a - b.a) < 0.0039f;
    }

    private string GetHexCode(Color32 c)
    {
        return $"#{c.r:X2}{c.g:X2}{c.b:X2}{c.a:X2}";
    }

    private string GetColorName(Color32 c)
    {
        if (c.a < 250) return "Transparent";
        if (c.r < 10 && c.g < 10 && c.b < 10) return "Black";
        if (c.r > 245 && c.g > 245 && c.b > 245) return "White";
        if (Mathf.Abs(c.r - c.g) < 10 && Mathf.Abs(c.g - c.b) < 10) return "Gray";

        Color.RGBToHSV(c, out float h, out _, out _);

        if (h < 0.04f || h > 0.96f) return "Red";
        if (h < 0.08f) return "Orange";
        if (h < 0.16f) return "Yellow";
        if (h < 0.33f) return "Green";
        if (h < 0.5f) return "Cyan";
        if (h < 0.66f) return "Blue";
        if (h < 0.83f) return "Purple";
        return "Pink";
    }
}
