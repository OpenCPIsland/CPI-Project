using System.Collections.Generic;
using System.IO;
using ClubPenguin;
using UnityEditor;
using UnityEngine;

public class BlueprintIconRendererWindow : EditorWindow
{
    private const string MenuPath = "Project/Tools/Blueprint Icon Renderer";
    private const string EquipmentDefinitionsFolder = "Assets/Game/ItemCustomizer/Resources/Definitions/Equipment";
    private const string OutlineShaderPath = "Assets/Editor/Tools/BlueprintIconRendering/BlueprintIconOutline.shader";
    private const int IconSize = 256;

    private static readonly Color DefaultFillColor = new Color(0f, 0.275f, 0.569f, 1f);

    private TemplateDefinition selectedTemplate;
    private TemplateRenderData renderData;

    private Color fillColor = DefaultFillColor;
    private Color outlineColor = Color.white;
    private int outlinePixels = 3;
    private Vector3 rotation;
    private Texture2D lastRenderedIcon;
    private string lastSavePath;
    private Vector2 scrollPosition;

    private GameObject previewRoot;
    private Camera previewCamera;
    private RenderTexture previewRenderTexture;
    private Material previewMaterial;
    private bool previewActive;

    [MenuItem(MenuPath)]
    private static void OpenWindow()
    {
        BlueprintIconRendererWindow window = GetWindow<BlueprintIconRendererWindow>("Blueprint Icon Renderer");
        window.minSize = new Vector2(460f, 660f);
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;

        if (selectedTemplate != null)
        {
            LoadRenderData();

            if (renderData != null)
            {
                SetupPreviewScene();
            }
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        TeardownPreviewScene();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawTemplateSelection();
        DrawRenderDataInfo();
        DrawRenderSettings();
        DrawPreview();
        DrawRenderAndSave();

        EditorGUILayout.EndScrollView();
    }

    private void DrawTemplateSelection()
    {
        EditorGUILayout.LabelField("Blueprint Template", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        selectedTemplate = (TemplateDefinition)EditorGUILayout.ObjectField(
            "Template Definition", selectedTemplate, typeof(TemplateDefinition), false);

        if (EditorGUI.EndChangeCheck())
        {
            LoadRenderData();
            TeardownPreviewScene();

            if (selectedTemplate != null)
            {
                SetupPreviewScene();
            }
        }

        GUILayout.Space(8f);
    }

    private void DrawRenderDataInfo()
    {
        if (selectedTemplate == null)
        {
            return;
        }

        EditorGUILayout.LabelField("Template Info", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("ID", selectedTemplate.Id.ToString());
        EditorGUILayout.LabelField("Asset Name", selectedTemplate.AssetName);
        EditorGUILayout.LabelField("Name Token", selectedTemplate.Name);
        EditorGUI.indentLevel--;

        GUILayout.Space(4f);

        if (renderData != null)
        {
            EditorGUILayout.LabelField("Render Data", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.Vector3Field("Item Position", renderData.ItemPosition);
            EditorGUILayout.FloatField("Camera FOV", renderData.CameraFOV);
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(8f);
    }

    private void DrawRenderSettings()
    {
        if (selectedTemplate == null)
        {
            return;
        }

        EditorGUILayout.LabelField("Render Settings", EditorStyles.boldLabel);
        fillColor = EditorGUILayout.ColorField("Fill Color", fillColor);
        outlineColor = EditorGUILayout.ColorField("Outline Color", outlineColor);
        outlinePixels = EditorGUILayout.IntSlider("Outline Pixels", outlinePixels, 1, 6);
        rotation.x = EditorGUILayout.Slider("Rotation X", rotation.x, 0f, 360f);
        rotation.y = EditorGUILayout.Slider("Rotation Y", rotation.y, 0f, 360f);
        rotation.z = EditorGUILayout.Slider("Rotation Z", rotation.z, 0f, 360f);

        GUILayout.Space(8f);
    }

    private void DrawPreview()
    {
        if (lastRenderedIcon == null)
        {
            return;
        }

        EditorGUILayout.LabelField("Rendered Icon Preview", EditorStyles.boldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
        EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f, 1f));
        EditorGUI.DrawPreviewTexture(previewRect, lastRenderedIcon);

        GUILayout.Space(8f);
    }

    private void DrawRenderAndSave()
    {
        if (selectedTemplate == null)
        {
            return;
        }

        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Rebuild Preview", GUILayout.Height(32f)))
        {
            SetupPreviewScene();
        }

        GUI.enabled = lastRenderedIcon != null;

        if (GUILayout.Button("Save to Assets", GUILayout.Height(32f)))
        {
            SaveRenderedIcon();
        }

        if (GUILayout.Button("Save Icon As...", GUILayout.Height(32f)))
        {
            SaveRenderedIconAs();
        }

        GUI.enabled = true;
    }

    private void LoadRenderData()
    {
        renderData = null;

        if (selectedTemplate == null)
        {
            return;
        }

        string renderDataKey = selectedTemplate.RenderDataKey.Key;
        if (string.IsNullOrEmpty(renderDataKey))
        {
            return;
        }

        string assetPath = "Assets/Game/ItemCustomizer/Resources/" + renderDataKey + ".asset";
        renderData = AssetDatabase.LoadAssetAtPath<TemplateRenderData>(assetPath);

        if (renderData == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:TemplateRenderData " + selectedTemplate.AssetName);
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                renderData = AssetDatabase.LoadAssetAtPath<TemplateRenderData>(path);
            }
        }
    }

    private void SetupPreviewScene()
    {
        TeardownPreviewScene();

        if (selectedTemplate == null)
        {
            return;
        }

        Shader fillShader = AssetDatabase.LoadAssetAtPath<Shader>(OutlineShaderPath);
        if (fillShader == null)
        {
            return;
        }

        List<Mesh> meshes = LoadEquipmentMeshes(selectedTemplate.AssetName);
        if (meshes.Count == 0)
        {
            return;
        }

        previewMaterial = new Material(fillShader);
        previewMaterial.SetColor("_FillColor", fillColor);

        int renderLayer = 31;
        previewRoot = new GameObject("EquipmentPreviewRoot");
        previewRoot.hideFlags = HideFlags.HideAndDontSave;
        previewRoot.layer = renderLayer;

        Bounds combinedBounds = new Bounds();
        bool boundsInitialized = false;

        foreach (Mesh mesh in meshes)
        {
            GameObject part = new GameObject("Part");
            part.transform.SetParent(previewRoot.transform, false);
            part.hideFlags = HideFlags.HideAndDontSave;
            part.layer = renderLayer;

            MeshFilter mf = part.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            MeshRenderer mr = part.AddComponent<MeshRenderer>();
            mr.sharedMaterial = previewMaterial;

            if (!boundsInitialized)
            {
                combinedBounds = mesh.bounds;
                boundsInitialized = true;
            }
            else
            {
                combinedBounds.Encapsulate(mesh.bounds);
            }
        }

        previewRenderTexture = new RenderTexture(IconSize, IconSize, 24, RenderTextureFormat.ARGB32);
        previewRenderTexture.antiAliasing = 8;
        previewRenderTexture.Create();

        GameObject camGO = new GameObject("PreviewCam");
        camGO.hideFlags = HideFlags.HideAndDontSave;
        previewCamera = camGO.AddComponent<Camera>();
        previewCamera.targetTexture = previewRenderTexture;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        previewCamera.cullingMask = 1 << renderLayer;
        previewCamera.nearClipPlane = 0.001f;
        previewCamera.farClipPlane = 100f;
        previewCamera.orthographic = false;
        previewCamera.fieldOfView = 30f;
        previewCamera.enabled = false;
        previewCamera.scene = default;

        Vector3 center = combinedBounds.center;
        float maxExtent = Mathf.Max(combinedBounds.extents.x, combinedBounds.extents.y, combinedBounds.extents.z);
        float distance = maxExtent / Mathf.Tan(previewCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        distance *= 1.3f;

        previewCamera.transform.position = center + Vector3.forward * distance;
        previewCamera.transform.LookAt(center);

        lastRenderedIcon = new Texture2D(IconSize, IconSize, TextureFormat.RGBA32, false);

        previewActive = true;
    }

    private void OnEditorUpdate()
    {
        if (!previewActive || previewRoot == null || previewCamera == null || previewMaterial == null || previewRenderTexture == null || lastRenderedIcon == null)
        {
            previewActive = false;
            return;
        }

        previewMaterial.SetColor("_FillColor", fillColor);
        previewRoot.transform.rotation = Quaternion.Euler(rotation);

        previewCamera.Render();

        RenderTexture.active = previewRenderTexture;
        lastRenderedIcon.ReadPixels(new Rect(0, 0, IconSize, IconSize), 0, 0);
        lastRenderedIcon.Apply();
        RenderTexture.active = null;

        ApplyPixelOutline(lastRenderedIcon, outlineColor, outlinePixels);

        Repaint();
    }

    private void TeardownPreviewScene()
    {
        previewActive = false;

        if (previewCamera != null)
        {
            DestroyImmediate(previewCamera.gameObject);
            previewCamera = null;
        }

        if (previewRoot != null)
        {
            DestroyImmediate(previewRoot);
            previewRoot = null;
        }

        if (previewMaterial != null)
        {
            DestroyImmediate(previewMaterial);
            previewMaterial = null;
        }

        if (previewRenderTexture != null)
        {
            previewRenderTexture.Release();
            DestroyImmediate(previewRenderTexture);
            previewRenderTexture = null;
        }

        DestroyRenderedIcon();
    }

    private List<Mesh> LoadEquipmentMeshes(string equipmentName)
    {
        List<Mesh> meshes = new List<Mesh>();

        string[] guids = AssetDatabase.FindAssets(equipmentName + " t:ScriptableObject",
            new[] { EquipmentDefinitionsFolder });

        List<string> assetPaths = new List<string>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName.StartsWith(equipmentName + "_", System.StringComparison.OrdinalIgnoreCase) &&
                fileName.EndsWith("_0LOD", System.StringComparison.OrdinalIgnoreCase))
            {
                assetPaths.Add(path);
            }
        }

        if (assetPaths.Count == 0)
        {
            return meshes;
        }

        foreach (string assetPath in assetPaths)
        {
            ScriptableObject viewDef = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (viewDef == null)
            {
                continue;
            }

            SerializedObject so = new SerializedObject(viewDef);
            SerializedProperty meshProp = so.FindProperty("SkinnedMesh.Mesh");
            if (meshProp == null || meshProp.objectReferenceValue == null)
            {
                continue;
            }

            Mesh mesh = meshProp.objectReferenceValue as Mesh;
            if (mesh != null)
            {
                meshes.Add(mesh);
            }
        }

        return meshes;
    }

    private static void ApplyPixelOutline(Texture2D texture, Color outlineColor, int radius)
    {
        int w = texture.width;
        int h = texture.height;
        Color[] pixels = texture.GetPixels();
        Color[] result = new Color[pixels.Length];
        System.Array.Copy(pixels, result, pixels.Length);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = y * w + x;
                if (pixels[idx].a > 0.1f)
                {
                    continue;
                }

                bool nearFilled = false;
                for (int dy = -radius; dy <= radius && !nearFilled; dy++)
                {
                    for (int dx = -radius; dx <= radius && !nearFilled; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (dx * dx + dy * dy > radius * radius) continue;

                        int nx = x + dx;
                        int ny = y + dy;
                        if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;

                        if (pixels[ny * w + nx].a > 0.1f)
                        {
                            nearFilled = true;
                        }
                    }
                }

                if (nearFilled)
                {
                    result[idx] = outlineColor;
                }
            }
        }

        texture.SetPixels(result);
        texture.Apply();
    }

    private void SaveRenderedIcon()
    {
        if (lastRenderedIcon == null || selectedTemplate == null)
        {
            return;
        }

        string fileName = selectedTemplate.AssetName + "_icon.png";
        string fullPath = "Assets/" + fileName;

        byte[] pngBytes = lastRenderedIcon.EncodeToPNG();
        string absolutePath = Path.GetFullPath(fullPath);
        File.WriteAllBytes(absolutePath, pngBytes);

        AssetDatabase.Refresh();
        lastSavePath = fullPath;

        EditorUtility.DisplayDialog("Blueprint Icon Saved",
            "Icon saved to:\n" + fullPath, "OK");

        Object savedAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);
        if (savedAsset != null)
        {
            EditorGUIUtility.PingObject(savedAsset);
        }

        Repaint();
    }

    private void SaveRenderedIconAs()
    {
        if (lastRenderedIcon == null || selectedTemplate == null)
        {
            return;
        }

        string defaultName = selectedTemplate.AssetName + "_icon.png";
        string path = EditorUtility.SaveFilePanel("Save Blueprint Icon", "Assets", defaultName, "png");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        byte[] pngBytes = lastRenderedIcon.EncodeToPNG();
        File.WriteAllBytes(path, pngBytes);

        if (path.Replace("\\", "/").Contains("/Assets/"))
        {
            AssetDatabase.Refresh();
        }

        lastSavePath = path;
        Repaint();
    }

    private void DestroyRenderedIcon()
    {
        if (lastRenderedIcon != null)
        {
            DestroyImmediate(lastRenderedIcon);
            lastRenderedIcon = null;
        }

        lastSavePath = null;
    }
}