using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TextMeshProRenderer : EditorWindow
{
    private GameObject prefab;
    private GameObject spawnedInstance;
    private Canvas canvas;
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private bool transparentBackground = false;
    private int renderWidth = 512;
    private int renderHeight = 512;
    private float zoomScale = 1f;

    private Vector2 textPosition = Vector2.zero;
    private Vector3 textScale = Vector3.one;

    private static readonly int[] exportSizes = { 64, 128, 256, 512, 1024, 2048, 4096 };
    private static readonly string[] exportSizeLabels = { "64x64", "128x128", "256x256", "512x512", "1024x1024", "2048x2048", "4096x4096" };
    private int exportSizeIndex = 1;

    private Camera renderCamera;
    private RenderTexture previewRT;

    private bool dragging;
    private Vector2 lastMousePos;

    [MenuItem("Project/Tools/TextMeshPro Renderer")]
    public static void ShowWindow()
    {
        var window = GetWindow<TextMeshProRenderer>("TMP Renderer");
        window.minSize = new Vector2(400, 600);
    }

    private int GetSupersampleFactor()
    {
        if (renderWidth <= 64) return 16;
        if (renderWidth <= 128) return 8;
        if (renderWidth <= 256) return 4;
        if (renderWidth <= 512) return 4;
        return 2;
    }

    private void OnEnable()
    {
        renderWidth = exportSizes[exportSizeIndex];
        renderHeight = exportSizes[exportSizeIndex];
    }

    private void OnDisable()
    {
        CleanupPreview();
    }

    private void CleanupPreview()
    {
        if (spawnedInstance != null)
            DestroyImmediate(spawnedInstance);
        spawnedInstance = null;
        canvas = null;

        if (renderCamera != null)
            DestroyImmediate(renderCamera.gameObject);
        renderCamera = null;

        if (previewRT != null)
            DestroyImmediate(previewRT);
        previewRT = null;
    }

    private void SpawnPrefab()
    {
        CleanupPreview();

        if (prefab == null) return;

        int ss = GetSupersampleFactor();
        int ssWidth = renderWidth * ss;
        int ssHeight = renderHeight * ss;

        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.layer = LayerMask.NameToLayer("UI");

        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(ssWidth, ssHeight);

        canvasGO.AddComponent<GraphicRaycaster>();

        var canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(ssWidth, ssHeight);

        spawnedInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvasGO.transform);

        var instanceRT = spawnedInstance.GetComponent<RectTransform>();
        if (instanceRT != null)
        {
            instanceRT.anchorMin = Vector2.zero;
            instanceRT.anchorMax = Vector2.one;
            instanceRT.offsetMin = Vector2.zero;
            instanceRT.offsetMax = Vector2.zero;
            instanceRT.anchoredPosition = textPosition;
            instanceRT.localScale = Vector3.Scale(textScale, new Vector3(zoomScale, zoomScale, 1f));
        }

        var cameraObj = new GameObject("RenderCamera");
        renderCamera = cameraObj.AddComponent<Camera>();
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        renderCamera.cullingMask = LayerMask.GetMask("UI");
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = ssHeight / 2f;

        canvas.worldCamera = renderCamera;
    }

    private void ApplyTransform()
    {
        if (spawnedInstance == null) return;

        var rt = spawnedInstance.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.anchoredPosition = textPosition;
        rt.localScale = Vector3.Scale(textScale, new Vector3(zoomScale, zoomScale, 1f));
    }

    private void HandleInput()
    {
        if (spawnedInstance == null) return;

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePos = mouse.position.ReadValue();

        if (mouse.middleButton.wasPressedThisFrame)
        {
            dragging = true;
            lastMousePos = mousePos;
        }

        if (mouse.middleButton.wasReleasedThisFrame)
            dragging = false;

        if (dragging)
        {
            Vector2 delta = mousePos - lastMousePos;
            lastMousePos = mousePos;
            textPosition += delta * 0.5f;
            ApplyTransform();
        }

        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            zoomScale = Mathf.Clamp(zoomScale + scroll * 0.01f, 0.1f, 20f);
            ApplyTransform();
        }
    }

    private void RenderPreview()
    {
        if (spawnedInstance == null || renderCamera == null) return;

        int ss = GetSupersampleFactor();
        int ssWidth = renderWidth * ss;
        int ssHeight = renderHeight * ss;

        if (previewRT == null || previewRT.width != ssWidth || previewRT.height != ssHeight)
        {
            if (previewRT != null) DestroyImmediate(previewRT);

            previewRT = new RenderTexture(ssWidth, ssHeight, 24, RenderTextureFormat.ARGB32);
            previewRT.antiAliasing = 8;
            previewRT.filterMode = FilterMode.Trilinear;
            previewRT.anisoLevel = 16;
            previewRT.Create();
        }

        renderCamera.orthographicSize = ssHeight / 2f;
        renderCamera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
        renderCamera.targetTexture = previewRT;
        renderCamera.Render();
        renderCamera.targetTexture = null;
    }

    private void OnGUI()
    {
        HandleInput();

        GUILayout.Label("TextMesh Pro Prefab Renderer", EditorStyles.boldLabel);

        GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (newPrefab != prefab)
        {
            prefab = newPrefab;
            if (prefab != null)
                SpawnPrefab();
        }

        if (prefab == null)
        {
            EditorGUILayout.HelpBox("Please select a prefab", MessageType.Info);
            return;
        }

        transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);

        if (!transparentBackground)
            backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);

        int newSize = GUILayout.SelectionGrid(exportSizeIndex, exportSizeLabels, 3);
        if (newSize != exportSizeIndex)
        {
            exportSizeIndex = newSize;
            renderWidth = exportSizes[exportSizeIndex];
            renderHeight = exportSizes[exportSizeIndex];
            SpawnPrefab();
        }

        textPosition = EditorGUILayout.Vector2Field("Position", textPosition);
        textScale = EditorGUILayout.Vector3Field("Scale", textScale);
        zoomScale = EditorGUILayout.Slider("Zoom", zoomScale, 0.1f, 20f);

        if (GUI.changed)
            ApplyTransform();

        if (GUILayout.Button("Export PNG", GUILayout.Height(30)))
            ExportToPNG();

        GUILayout.Space(10);

        Rect previewRect = GUILayoutUtility.GetRect(300, 300, GUILayout.ExpandWidth(true));

        RenderPreview();
        if (previewRT != null)
            GUI.DrawTexture(previewRect, previewRT, ScaleMode.ScaleToFit);

        Repaint();
    }

    private void ExportToPNG()
    {
        if (renderCamera == null || spawnedInstance == null) return;

        string path = EditorUtility.SaveFilePanel("Save PNG", "", "text_render", "png");
        if (string.IsNullOrEmpty(path)) return;

        int ss = GetSupersampleFactor();
        int ssWidth = renderWidth * ss;
        int ssHeight = renderHeight * ss;

        RenderTexture ssRT = new RenderTexture(ssWidth, ssHeight, 24, RenderTextureFormat.ARGB32);
        ssRT.antiAliasing = 8;
        ssRT.filterMode = FilterMode.Trilinear;
        ssRT.anisoLevel = 16;
        ssRT.Create();

        renderCamera.orthographicSize = ssHeight / 2f;
        renderCamera.targetTexture = ssRT;
        renderCamera.Render();
        renderCamera.targetTexture = null;

        RenderTexture finalRT = new RenderTexture(renderWidth, renderHeight, 24, RenderTextureFormat.ARGB32);
        finalRT.filterMode = FilterMode.Trilinear;
        finalRT.anisoLevel = 16;
        finalRT.Create();

        Graphics.Blit(ssRT, finalRT);

        RenderTexture.active = finalRT;
        Texture2D tex = new Texture2D(renderWidth, renderHeight, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, renderWidth, renderHeight), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());

        DestroyImmediate(tex);
        DestroyImmediate(ssRT);
        DestroyImmediate(finalRT);
    }
}