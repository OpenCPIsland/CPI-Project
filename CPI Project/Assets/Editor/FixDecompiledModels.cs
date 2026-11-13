using UnityEngine;
using UnityEditor;

public class FixDecompiledModels
{
    private const string TARGET_PATH = "Assets/";

    private static readonly string[] SKIP_FOLDERS = new string[]
    {
        "Modules",
        "Town",
        "Boardwalk",
        "Diving",
        "BoxDimension",
        "MtBlizzard",
        "Beach",
        "HerbertBase",
        "PartyAssets",
        "MtBlizzardSummit"
    };

    [MenuItem("Project/Models/Fix Decompiled Models")]
    public static void FixMeshes()
    {
        string[] guids = AssetDatabase.FindAssets("t:Mesh", new[] { TARGET_PATH });
        if (guids == null || guids.Length == 0)
        {
            return;
        }

        int processed = 0;
        int changed = 0;
        int skipped = 0;

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);

            if (EditorUtility.DisplayCancelableProgressBar("Fixing Models", path, (float)i / guids.Length))
                break;

            if (ShouldSkip(path))
            {
                skipped++;
                continue;
            }

            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null)
                continue;

            processed++;
            if (FixMesh(mesh))
            {
                changed++;
                EditorUtility.SetDirty(mesh);
                AssetDatabase.SaveAssetIfDirty(mesh);
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static bool ShouldSkip(string path)
    {
        for (int i = 0; i < SKIP_FOLDERS.Length; i++)
        {
            if (path.Contains("/" + SKIP_FOLDERS[i] + "/"))
                return true;
        }
        return false;
    }

    private static bool FixMesh(Mesh mesh)
    {
        if (mesh == null)
            return false;

        int vertexCount = mesh.vertexCount;
        if (vertexCount <= 0)
            return false;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector2[] uv0 = mesh.uv;
        Vector2[] uv1 = mesh.uv2;
        Color32[] colors = mesh.colors32;
        Vector3[] normals = mesh.normals;
        Vector4[] tangents = mesh.tangents;
        BoneWeight[] boneWeights = null;
        Matrix4x4[] bindPoses = null;

#if UNITY_2019_1_OR_NEWER
        boneWeights = mesh.boneWeights;
        bindPoses = mesh.bindposes;
#endif

        if (uv0 == null || uv0.Length != vertexCount)
            uv0 = new Vector2[vertexCount];

        if (uv1 == null || uv1.Length != vertexCount)
            uv1 = new Vector2[vertexCount];

        if (colors == null || colors.Length != vertexCount || IsAllZero(colors))
            colors = CreateDefaultColors(vertexCount);

        if (normals == null || normals.Length != vertexCount || HasNaN(normals))
            normals = null;

        if (tangents == null || tangents.Length != vertexCount || HasNaN(tangents))
            tangents = null;

        mesh.Clear(false);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv0;
        mesh.uv2 = uv1;
        mesh.colors32 = colors;

#if UNITY_2019_1_OR_NEWER
        if (boneWeights != null && boneWeights.Length > 0)
        {
            mesh.boneWeights = boneWeights;
            mesh.bindposes = bindPoses;
        }
#endif

        try
        {
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
        catch (System.Exception)
        {
            return false;
        }

        if (normals != null)
            mesh.normals = normals;

        if (tangents != null)
            mesh.tangents = tangents;

        mesh.RecalculateBounds();
        return true;
    }

    private static Color32[] CreateDefaultColors(int vertexCount)
    {
        Color32[] colors = new Color32[vertexCount];
        for (int i = 0; i < vertexCount; i++)
            colors[i] = new Color32(255, 255, 255, 255);
        return colors;
    }

    private static bool IsAllZero(Color32[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].r != 0 || colors[i].g != 0 || colors[i].b != 0 || colors[i].a != 0)
                return false;
        }
        return true;
    }

    private static bool HasNaN(Vector3[] values)
    {
        if (values == null)
            return true;

        for (int i = 0; i < values.Length; i++)
        {
            if (float.IsNaN(values[i].x) || float.IsNaN(values[i].y) || float.IsNaN(values[i].z))
                return true;
        }
        return false;
    }

    private static bool HasNaN(Vector4[] values)
    {
        if (values == null)
            return true;

        for (int i = 0; i < values.Length; i++)
        {
            if (float.IsNaN(values[i].x) || float.IsNaN(values[i].y) || float.IsNaN(values[i].z) || float.IsNaN(values[i].w))
                return true;
        }
        return false;
    }
}