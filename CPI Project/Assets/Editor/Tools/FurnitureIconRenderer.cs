using UnityEngine;
using UnityEditor;
using System.IO;

public class FurnitureIconRenderer : EditorWindow
{
    private const int MaxExportSupersampleFactor = 8;

    private GameObject prefab;
    private GameObject spawnedInstance;

    private Vector3 modelPosition = Vector3.zero;
    private Vector3 rotation = new Vector3(0, 180, 0);
    private Vector3 scale = Vector3.one;

    private float cameraDistance;
    private float framedCameraDistance;
    private bool isDragging;
    private bool isRotatingVertical;
    private bool isPanning;
    private bool showGridLines = true;
    private bool snapToGrid = false;
    private float snapIncrement = 0.1f;
    private Vector2 cameraPan = Vector2.zero;

    private Color lightColor = Color.white;
    private float lightIntensity = 1.0f;
    private float lightXRotation = 50f;
    private float lightYRotation = -30f;

    private static readonly int[] iconSizes = { 64, 128, 256, 512, 1024 };
    private static readonly string[] iconSizeLabels = { "64x64", "128x128", "256x256", "512x512", "1024x1024" };
    private int iconSizeIndex = 2;
    private int iconSize => iconSizes[iconSizeIndex];
    private Color backgroundColor = new Color(0.439f, 0.537f, 0.863f, 1f);
    private bool transparentBackground = false;

    private PreviewRenderUtility previewRenderUtility;
    private Vector2 scrollPos;

    [MenuItem("Project/Tools/Furniture Icon Renderer")]
    public static void ShowWindow()
    {
        var window = GetWindow<FurnitureIconRenderer>("Furniture Icon Renderer");
        window.minSize = new Vector2(300, 620);
    }

    private void OnEnable() => InitPreview();
    private void OnDisable() => CleanupPreview();

    private void InitPreview()
    {
        CleanupPreview();
        previewRenderUtility = new PreviewRenderUtility();
        previewRenderUtility.camera.fieldOfView = 30f;
        previewRenderUtility.camera.nearClipPlane = 0.01f;
        previewRenderUtility.camera.farClipPlane = 100f;
        previewRenderUtility.camera.transform.position = new Vector3(0, 1, -5);
        previewRenderUtility.camera.transform.LookAt(Vector3.zero);
        previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
        previewRenderUtility.camera.backgroundColor = backgroundColor;
        UpdatePreviewLight();
    }

    private void CleanupPreview()
    {
        if (spawnedInstance != null)
        {
            DestroyImmediate(spawnedInstance);
            spawnedInstance = null;
        }
        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }
    }

    private void SpawnPrefabInPreview()
    {
        if (spawnedInstance != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
            spawnedInstance = null;
            InitPreview();
        }
        if (prefab == null) return;
        spawnedInstance = previewRenderUtility.InstantiatePrefabInScene(prefab);
        spawnedInstance.hideFlags = HideFlags.HideAndDontSave;
        spawnedInstance.transform.position = modelPosition;
        spawnedInstance.transform.eulerAngles = rotation;
        spawnedInstance.transform.localScale = scale;
        AutoFrameObject();
    }

    private void AutoFrameObject()
    {
        if (spawnedInstance == null) return;
        var bounds = GetBounds(spawnedInstance);
        float size = bounds.size.magnitude;
        if (size < 0.001f) size = 1f;
        float distance = size / (2f * Mathf.Tan(previewRenderUtility.camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        distance *= 1.2f;
        framedCameraDistance = distance;
        cameraDistance = distance;
        var cam = previewRenderUtility.camera;
        cam.transform.position = bounds.center + new Vector3(cameraPan.x, cameraPan.y, -distance);
        cam.transform.LookAt(bounds.center + new Vector3(cameraPan.x, cameraPan.y, 0));
    }

    private Bounds GetBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);
        var bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    private float GetMaxCameraDistance()
    {
        float baseDistance = framedCameraDistance > 0f ? framedCameraDistance : cameraDistance;
        return Mathf.Max(0.11f, baseDistance * 5f);
    }

    private int GetExportSupersampleFactor()
    {
        int availableFactor = Mathf.Max(1, SystemInfo.maxTextureSize / iconSize);
        return Mathf.Clamp(availableFactor, 1, MaxExportSupersampleFactor);
    }

    private void UpdatePreviewLight()
    {
        if (previewRenderUtility == null) return;
        var light = previewRenderUtility.lights[0];
        if (light == null) return;
        light.type = LightType.Directional;
        light.enabled = true;
        light.color = lightColor;
        light.intensity = lightIntensity;
        light.transform.rotation = Quaternion.Euler(lightXRotation, lightYRotation, 0);
        if (previewRenderUtility.lights.Length > 1 && previewRenderUtility.lights[1] != null)
        {
            previewRenderUtility.lights[1].intensity = 0f;
            previewRenderUtility.lights[1].enabled = false;
        }
    }

    private void UpdateInstanceTransform()
    {
        if (spawnedInstance == null) return;
        if (snapToGrid)
        {
            modelPosition.x = Mathf.Round(modelPosition.x / snapIncrement) * snapIncrement;
            modelPosition.y = Mathf.Round(modelPosition.y / snapIncrement) * snapIncrement;
            modelPosition.z = Mathf.Round(modelPosition.z / snapIncrement) * snapIncrement;
        }
        spawnedInstance.transform.position = modelPosition;
        spawnedInstance.transform.eulerAngles = rotation;
        spawnedInstance.transform.localScale = scale;
    }

    private Color32[] RenderAndReadPixels(int size, Color bgColor)
    {
        Rect renderRect = new Rect(0, 0, size, size);
        previewRenderUtility.BeginPreview(renderRect, GUIStyle.none);

        var cam = previewRenderUtility.camera;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.Render();

        RenderTexture sourceRT = cam.targetTexture;
        var rt = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(sourceRT, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();
        Color32[] pixels = tex.GetPixels32();
        RenderTexture.active = prev;

        RenderTexture.ReleaseTemporary(rt);
        DestroyImmediate(tex);

        previewRenderUtility.EndPreview();

        return pixels;
    }

    private Color32[] ForceAlphaOpaque(Color32[] pixels)
    {
        for (int i = 0; i < pixels.Length; i++)
            pixels[i].a = 255;
        return pixels;
    }

    private Color32[] CombineDualRender(Color32[] blackPixels, Color32[] whitePixels, int count)
    {
        var result = new Color32[count];
        for (int i = 0; i < count; i++)
        {
            float bR = blackPixels[i].r / 255f;
            float bG = blackPixels[i].g / 255f;
            float bB = blackPixels[i].b / 255f;

            float wR = whitePixels[i].r / 255f;
            float wG = whitePixels[i].g / 255f;
            float wB = whitePixels[i].b / 255f;

            float a = 1f - ((wR - bR) + (wG - bG) + (wB - bB)) / 3f;
            a = Mathf.Clamp01(a);

            byte outA = (byte)Mathf.RoundToInt(a * 255f);
            byte outR, outG, outB;

            if (a > 0.0001f)
            {
                outR = (byte)Mathf.Clamp(Mathf.RoundToInt((bR / a) * 255f), 0, 255);
                outG = (byte)Mathf.Clamp(Mathf.RoundToInt((bG / a) * 255f), 0, 255);
                outB = (byte)Mathf.Clamp(Mathf.RoundToInt((bB / a) * 255f), 0, 255);
            }
            else
            {
                outR = outG = outB = 0;
            }

            result[i] = new Color32(outR, outG, outB, outA);
        }
        return result;
    }

    private Texture2D PixelsToTexture(Color32[] pixels, int width, int height)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }

    private Texture2D DownsampleTexture(Texture2D source, int targetSize, int supersampleFactor)
    {
        if (supersampleFactor <= 1)
            return source;

        int sourceWidth = source.width;
        Color32[] sourcePixels = source.GetPixels32();
        var outputPixels = new Color32[targetSize * targetSize];
        float sampleCount = supersampleFactor * supersampleFactor;

        for (int y = 0; y < targetSize; y++)
        {
            int sourceYBase = y * supersampleFactor;
            for (int x = 0; x < targetSize; x++)
            {
                int sourceXBase = x * supersampleFactor;
                float accumR = 0f, accumG = 0f, accumB = 0f, accumA = 0f;

                for (int sy = 0; sy < supersampleFactor; sy++)
                {
                    int rowOffset = (sourceYBase + sy) * sourceWidth + sourceXBase;
                    for (int sx = 0; sx < supersampleFactor; sx++)
                    {
                        Color32 s = sourcePixels[rowOffset + sx];
                        float alpha = s.a / 255f;
                        accumA += alpha;
                        accumR += (s.r / 255f) * alpha;
                        accumG += (s.g / 255f) * alpha;
                        accumB += (s.b / 255f) * alpha;
                    }
                }

                float outA = accumA / sampleCount;
                byte byteA = (byte)Mathf.Clamp(Mathf.RoundToInt(outA * 255f), 0, 255);

                if (accumA > 0.0001f)
                {
                    float inv = 1f / accumA;
                    byte byteR = (byte)Mathf.Clamp(Mathf.RoundToInt(accumR * inv * 255f), 0, 255);
                    byte byteG = (byte)Mathf.Clamp(Mathf.RoundToInt(accumG * inv * 255f), 0, 255);
                    byte byteB = (byte)Mathf.Clamp(Mathf.RoundToInt(accumB * inv * 255f), 0, 255);
                    outputPixels[y * targetSize + x] = new Color32(byteR, byteG, byteB, byteA);
                }
                else
                {
                    outputPixels[y * targetSize + x] = new Color32(0, 0, 0, 0);
                }
            }
        }

        var output = new Texture2D(targetSize, targetSize, TextureFormat.RGBA32, false);
        output.SetPixels32(outputPixels);
        output.Apply();
        return output;
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));
        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField("Furniture Icon Renderer", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUI.BeginChangeCheck();
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            modelPosition = Vector3.zero;
            rotation = new Vector3(0, 180, 0);
            scale = Vector3.one;
            SpawnPrefabInPreview();
        }

        EditorGUILayout.Space(5);

        if (prefab != null)
        {
            EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            modelPosition = EditorGUILayout.Vector3Field("Position", modelPosition);
            rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
            scale = EditorGUILayout.Vector3Field("Scale", scale);
            if (EditorGUI.EndChangeCheck()) UpdateInstanceTransform();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Directional Light", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            lightColor = EditorGUILayout.ColorField("Light Color", lightColor);
            lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0f, 8f);
            lightXRotation = EditorGUILayout.Slider("Light X Rotation", lightXRotation, -180f, 180f);
            lightYRotation = EditorGUILayout.Slider("Light Y Rotation", lightYRotation, -180f, 180f);
            if (EditorGUI.EndChangeCheck()) UpdatePreviewLight();

            EditorGUILayout.Space(5);
            iconSizeIndex = EditorGUILayout.Popup("Image Size", iconSizeIndex, iconSizeLabels);
            transparentBackground = EditorGUILayout.Toggle("Transparent Background", transparentBackground);
            if (!transparentBackground)
            {
                EditorGUI.BeginChangeCheck();
                backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
                if (EditorGUI.EndChangeCheck() && previewRenderUtility != null)
                    previewRenderUtility.camera.backgroundColor = backgroundColor;
            }
            if (previewRenderUtility != null)
            {
                previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
                previewRenderUtility.camera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
            }

            EditorGUILayout.Space(10);
            showGridLines = EditorGUILayout.Toggle("Show Grid Lines", showGridLines);

            EditorGUILayout.BeginHorizontal();
            snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid);
            if (snapToGrid)
            {
                snapIncrement = EditorGUILayout.FloatField(snapIncrement, GUILayout.Width(50));
                snapIncrement = Mathf.Max(0.01f, snapIncrement);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Live Preview", EditorStyles.boldLabel);

            if (cameraDistance > 0)
            {
                float maxCameraDistance = GetMaxCameraDistance();
                float zoomValue = maxCameraDistance - cameraDistance + 0.1f;
                EditorGUI.BeginChangeCheck();
                zoomValue = EditorGUILayout.Slider("Camera Zoom", zoomValue, 0.1f, maxCameraDistance);
                if (EditorGUI.EndChangeCheck())
                {
                    cameraDistance = maxCameraDistance - zoomValue + 0.1f;
                    UpdateCameraPosition();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (spawnedInstance != null && previewRenderUtility != null)
            {
                HandlePreviewInput(previewRect);
                previewRenderUtility.BeginPreview(previewRect, GUIStyle.none);
                previewRenderUtility.camera.backgroundColor = transparentBackground ? new Color(0, 0, 0, 0) : backgroundColor;
                previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
                previewRenderUtility.camera.Render();
                var tex = previewRenderUtility.EndPreview();
                GUI.DrawTexture(previewRect, tex, ScaleMode.ScaleToFit);
                if (showGridLines) DrawGridOverlay(previewRect);
            }
            else
            {
                EditorGUI.DrawRect(previewRect, backgroundColor);
            }

            EditorGUILayout.Space(10);

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            string saveDir = Path.GetDirectoryName(prefabPath);
            string defaultPath = Path.Combine(saveDir, prefabName + "_FurnitureIcon.png");
            EditorGUILayout.LabelField("Save Path:", defaultPath);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Center Item", GUILayout.Height(30)))
            {
                modelPosition = Vector3.zero;
                UpdateInstanceTransform();
            }
            if (GUILayout.Button("Reset All", GUILayout.Height(30)))
            {
                modelPosition = Vector3.zero;
                rotation = new Vector3(0, 180, 0);
                scale = Vector3.one;
                lightColor = Color.white;
                lightIntensity = 1.0f;
                lightXRotation = 50f;
                lightYRotation = -30f;
                backgroundColor = new Color(0.439f, 0.537f, 0.863f, 1f);
                transparentBackground = false;
                showGridLines = true;
                snapToGrid = false;
                snapIncrement = 0.1f;
                cameraPan = Vector2.zero;
                UpdatePreviewLight();
                if (previewRenderUtility != null)
                    previewRenderUtility.camera.backgroundColor = backgroundColor;
                SpawnPrefabInPreview();
            }
            if (GUILayout.Button("Save Icon", GUILayout.Height(30)))
                SaveIcon(defaultPath);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save Icon As...", GUILayout.Height(25)))
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(defaultPath));
                string fileName = prefabName + "_FurnitureIcon.png";
                string chosen = EditorUtility.SaveFilePanel("Save Furniture Icon", dir, fileName, "png");
                if (!string.IsNullOrEmpty(chosen))
                    SaveIconToPath(chosen);
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        if (spawnedInstance != null)
            Repaint();
    }

    private void HandlePreviewInput(Rect previewRect)
    {
        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.GetTypeForControl(controlId))
        {
            case EventType.MouseDown:
                if (previewRect.Contains(e.mousePosition) && e.button == 0) { isDragging = true; GUIUtility.hotControl = controlId; e.Use(); }
                else if (previewRect.Contains(e.mousePosition) && e.button == 1) { isRotatingVertical = true; GUIUtility.hotControl = controlId; e.Use(); }
                else if (previewRect.Contains(e.mousePosition) && e.button == 2) { isPanning = true; GUIUtility.hotControl = controlId; e.Use(); }
                break;

            case EventType.MouseDrag:
                if (isDragging) { rotation.y -= e.delta.x * 0.5f; UpdateInstanceTransform(); e.Use(); }
                else if (isRotatingVertical) { rotation.x -= e.delta.y * 0.5f; UpdateInstanceTransform(); e.Use(); }
                else if (isPanning)
                {
                    cameraPan.x -= e.delta.x * cameraDistance * 0.002f;
                    cameraPan.y += e.delta.y * cameraDistance * 0.002f;
                    if (snapToGrid)
                    {
                        cameraPan.x = Mathf.Round(cameraPan.x / snapIncrement) * snapIncrement;
                        cameraPan.y = Mathf.Round(cameraPan.y / snapIncrement) * snapIncrement;
                    }
                    UpdateCameraPosition();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDragging || isRotatingVertical || isPanning)
                {
                    isDragging = false; isRotatingVertical = false; isPanning = false;
                    GUIUtility.hotControl = 0; e.Use();
                }
                break;

            case EventType.ScrollWheel:
                if (previewRect.Contains(e.mousePosition) && cameraDistance > 0)
                {
                    cameraDistance += e.delta.y * cameraDistance * 0.1f;
                    cameraDistance = Mathf.Max(cameraDistance, 0.1f);
                    UpdateCameraPosition();
                    e.Use();
                }
                break;
        }
    }

    private void UpdateCameraPosition()
    {
        if (spawnedInstance == null || previewRenderUtility == null) return;
        var bounds = GetBounds(spawnedInstance);
        var cam = previewRenderUtility.camera;
        cam.transform.position = bounds.center + new Vector3(cameraPan.x, cameraPan.y, -cameraDistance);
        cam.transform.LookAt(bounds.center + new Vector3(cameraPan.x, cameraPan.y, 0));
    }

    private void DrawGridOverlay(Rect rect)
    {
        float cx = rect.x + rect.width * 0.5f;
        float cy = rect.y + rect.height * 0.5f;
        float third = rect.width / 3f;
        float thirdH = rect.height / 3f;

        Handles.BeginGUI();
        Handles.color = new Color(1f, 1f, 1f, 0.3f);
        Handles.DrawLine(new Vector3(cx, rect.y), new Vector3(cx, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x, cy), new Vector3(rect.xMax, cy));
        Handles.color = new Color(1f, 1f, 1f, 0.15f);
        Handles.DrawLine(new Vector3(rect.x + third, rect.y), new Vector3(rect.x + third, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x + third * 2f, rect.y), new Vector3(rect.x + third * 2f, rect.yMax));
        Handles.DrawLine(new Vector3(rect.x, rect.y + thirdH), new Vector3(rect.xMax, rect.y + thirdH));
        Handles.DrawLine(new Vector3(rect.x, rect.y + thirdH * 2f), new Vector3(rect.xMax, rect.y + thirdH * 2f));
        Handles.EndGUI();
    }

    private void SaveIcon(string savePath)
    {
        string fullPath = Path.GetFullPath(savePath);
        SaveIconToPath(fullPath);
        AssetDatabase.Refresh();
        var savedAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
        if (savedAsset != null)
            EditorGUIUtility.PingObject(savedAsset);
    }

    private void SaveIconToPath(string fullPath)
    {
        if (spawnedInstance == null || previewRenderUtility == null)
        {
            EditorUtility.DisplayDialog("Error", "No prefab is spawned to capture.", "OK");
            return;
        }

        UpdatePreviewLight();

        int supersampleFactor = GetExportSupersampleFactor();
        int renderSize = iconSize * supersampleFactor;

        Texture2D final;

        if (transparentBackground)
        {
            Color32[] blackPixels = RenderAndReadPixels(renderSize, new Color(0f, 0f, 0f, 1f));
            Color32[] whitePixels = RenderAndReadPixels(renderSize, new Color(1f, 1f, 1f, 1f));
            Color32[] combined = CombineDualRender(blackPixels, whitePixels, blackPixels.Length);
            Texture2D combinedTex = PixelsToTexture(combined, renderSize, renderSize);
            final = DownsampleTexture(combinedTex, iconSize, supersampleFactor);
            if (combinedTex != final) DestroyImmediate(combinedTex);
        }
        else
        {
            Color32[] pixels = RenderAndReadPixels(renderSize, backgroundColor);
            pixels = ForceAlphaOpaque(pixels);
            Texture2D flat = PixelsToTexture(pixels, renderSize, renderSize);
            final = DownsampleTexture(flat, iconSize, supersampleFactor);
            if (flat != final) DestroyImmediate(flat);
        }

        byte[] pngData = final.EncodeToPNG();
        File.WriteAllBytes(fullPath, pngData);
        DestroyImmediate(final);

        Debug.Log("Furniture icon saved to: " + fullPath);
        EditorUtility.DisplayDialog("Success", "Icon saved to:\n" + fullPath, "OK");
    }
}