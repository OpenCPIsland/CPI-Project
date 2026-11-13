using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ControlsDisabledIconRenderer : EditorWindow
{
    private const string MenuPath = "Project/Tools/Controls Disabled Icon Renderer";
    private static readonly Color DefaultTintColor = new Color32(0x08, 0x84, 0xDE, 0xFF);

    private Texture2D sourceIcon;
    private Texture2D importSettingsSource;
    private Texture2D renderedIcon;
    private Vector2 scrollPosition;
    private string outputPath = string.Empty;
    private Color tintColor = DefaultTintColor;
    private float tintOpacity = 0.9f;
    private float iconVisibility = 1f;
    private bool convertToGrayscale = false;
    private bool isolateForeground = true;
    private float backgroundSimilarityThreshold = 0.12f;
    private bool copyImportSettings = true;
    private bool autoRefreshPreview = true;

    [MenuItem(MenuPath)]
    private static void OpenWindow()
    {
        ControlsDisabledIconRenderer window = GetWindow<ControlsDisabledIconRenderer>("Disabled Icon Renderer");
        window.minSize = new Vector2(420f, 520f);
    }

    [MenuItem("Assets/Render Disabled Controls Icon", true)]
    private static bool ValidateOpenWindowFromSelection()
    {
        return Selection.activeObject is Texture2D;
    }

    [MenuItem("Assets/Render Disabled Controls Icon")]
    private static void OpenWindowFromSelection()
    {
        ControlsDisabledIconRenderer window = GetWindow<ControlsDisabledIconRenderer>("Disabled Icon Renderer");
        window.minSize = new Vector2(420f, 520f);
        window.AssignSourceTexture(Selection.activeObject as Texture2D);
    }

    private void OnDisable()
    {
        DestroyRenderedIcon();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawSourceSection();
        DrawRenderSettings();
        DrawOutputSection();
        DrawActionButtons();
        DrawPreviewSection();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSourceSection()
    {
        EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        Texture2D newSourceIcon = (Texture2D)EditorGUILayout.ObjectField(
            "Controls Icon",
            sourceIcon,
            typeof(Texture2D),
            false);

        if (EditorGUI.EndChangeCheck())
        {
            AssignSourceTexture(newSourceIcon);
        }

        if (sourceIcon != null)
        {
            string sourcePath = AssetDatabase.GetAssetPath(sourceIcon);
            EditorGUILayout.LabelField("Asset Path", sourcePath);
        }

        GUILayout.Space(8f);
    }

    private void DrawRenderSettings()
    {
        EditorGUILayout.LabelField("Render Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        tintColor = EditorGUILayout.ColorField("Tint Color", tintColor);
        tintOpacity = EditorGUILayout.Slider("Tint Opacity", tintOpacity, 0f, 1f);
        iconVisibility = EditorGUILayout.Slider("Icon Visibility", iconVisibility, 0f, 4f);
        convertToGrayscale = EditorGUILayout.Toggle("Convert To Grayscale", convertToGrayscale);
        isolateForeground = EditorGUILayout.Toggle("Auto Isolate Object", isolateForeground);
        using (new EditorGUI.DisabledScope(!isolateForeground))
        {
            backgroundSimilarityThreshold = EditorGUILayout.Slider("Background Threshold", backgroundSimilarityThreshold, 0.02f, 0.35f);
        }
        autoRefreshPreview = EditorGUILayout.Toggle("Auto Refresh Preview", autoRefreshPreview);

        if (EditorGUI.EndChangeCheck() && autoRefreshPreview)
        {
            RefreshPreview();
        }

        GUILayout.Space(8f);
    }

    private void DrawOutputSection()
    {
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(sourceIcon == null))
        {
            EditorGUILayout.BeginHorizontal();
            outputPath = EditorGUILayout.TextField("Asset Path", outputPath);

            if (GUILayout.Button("Suggested", GUILayout.Width(82f)))
            {
                outputPath = GetSuggestedOutputPath(sourceIcon);
            }

            EditorGUILayout.EndHorizontal();
        }

        copyImportSettings = EditorGUILayout.Toggle("Copy Import Settings", copyImportSettings);

        using (new EditorGUI.DisabledScope(!copyImportSettings))
        {
            importSettingsSource = (Texture2D)EditorGUILayout.ObjectField(
                "Import Settings Source",
                importSettingsSource,
                typeof(Texture2D),
                false);
        }

        GUILayout.Space(8f);
    }

    private void DrawActionButtons()
    {
        using (new EditorGUI.DisabledScope(sourceIcon == null))
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh Preview", GUILayout.Height(28f)))
            {
                RefreshPreview();
            }

            if (GUILayout.Button("Render And Save", GUILayout.Height(28f)))
            {
                RenderAndSave();
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(8f);
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        if (sourceIcon == null)
        {
            EditorGUILayout.HelpBox("Select a controls icon to generate a disabled preview.", MessageType.None);
            return;
        }

        if (renderedIcon == null && autoRefreshPreview)
        {
            RefreshPreview();
        }

        DrawTexturePreview("Source", sourceIcon);
        DrawTexturePreview("Disabled", renderedIcon);
    }

    private static void DrawTexturePreview(string label, Texture2D texture)
    {
        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(220f, 220f, GUILayout.ExpandWidth(true));
        GUI.Box(previewRect, GUIContent.none);

        if (texture == null)
        {
            EditorGUI.LabelField(previewRect, "No preview available", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(6f);
            return;
        }

        EditorGUI.DrawPreviewTexture(previewRect, texture, null, ScaleMode.ScaleToFit);
        GUILayout.Space(6f);
    }

    private void AssignSourceTexture(Texture2D texture)
    {
        sourceIcon = texture;
        outputPath = GetSuggestedOutputPath(sourceIcon);
        importSettingsSource = GetSuggestedImportSettingsSource(sourceIcon);

        if (autoRefreshPreview)
        {
            RefreshPreview();
        }
        else
        {
            DestroyRenderedIcon();
        }
    }

    private void RefreshPreview()
    {
        DestroyRenderedIcon();

        if (sourceIcon == null)
        {
            return;
        }

        renderedIcon = CreateDisabledIconTexture(
            sourceIcon,
            tintColor,
            tintOpacity,
            iconVisibility,
            convertToGrayscale,
            isolateForeground,
            backgroundSimilarityThreshold);
        if (renderedIcon != null)
        {
            renderedIcon.name = sourceIcon.name + "_DisabledPreview";
            renderedIcon.hideFlags = HideFlags.HideAndDontSave;
        }

        Repaint();
    }

    private void RenderAndSave()
    {
        if (sourceIcon == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            outputPath = GetSuggestedOutputPath(sourceIcon);
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            EditorUtility.DisplayDialog("Disabled Icon Renderer", "Unable to resolve an output path.", "OK");
            return;
        }

        if (!outputPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
        {
            EditorUtility.DisplayDialog("Disabled Icon Renderer", "The output path must stay inside the Assets folder.", "OK");
            return;
        }

        Texture2D outputTexture = CreateDisabledIconTexture(
            sourceIcon,
            tintColor,
            tintOpacity,
            iconVisibility,
            convertToGrayscale,
            isolateForeground,
            backgroundSimilarityThreshold);
        if (outputTexture == null)
        {
            EditorUtility.DisplayDialog("Disabled Icon Renderer", "Failed to render the disabled icon texture.", "OK");
            return;
        }

        try
        {
            string absolutePath = Path.GetFullPath(outputPath);
            string directory = Path.GetDirectoryName(absolutePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(absolutePath, outputTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);

            if (copyImportSettings)
            {
                Texture2D settingsSourceTexture = importSettingsSource != null ? importSettingsSource : sourceIcon;
                CopyTextureImportSettings(AssetDatabase.GetAssetPath(settingsSourceTexture), outputPath);
                AssetDatabase.ImportAsset(outputPath, ImportAssetOptions.ForceUpdate);
            }

            Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(outputPath);
            if (savedTexture != null)
            {
                Selection.activeObject = savedTexture;
                EditorGUIUtility.PingObject(savedTexture);
            }

            AssignSourceTexture(sourceIcon);
        }
        finally
        {
            DestroyImmediate(outputTexture);
        }
    }

    private void DestroyRenderedIcon()
    {
        if (renderedIcon != null)
        {
            DestroyImmediate(renderedIcon);
            renderedIcon = null;
        }
    }

    private static Texture2D CreateDisabledIconTexture(
        Texture2D source,
        Color tint,
        float opacity,
        float visibility,
        bool grayscale,
        bool isolateObject,
        float backgroundThreshold)
    {
        if (source == null)
        {
            return null;
        }

        Texture2D readableSource = MakeReadableCopy(source);
        if (readableSource == null)
        {
            return null;
        }

        try
        {
            Texture2D outputTexture = new Texture2D(readableSource.width, readableSource.height, TextureFormat.RGBA32, false);
            Color[] sourcePixels = readableSource.GetPixels();
            Color[] outputPixels = new Color[sourcePixels.Length];
            float[] foregroundMask = isolateObject
                ? CreateForegroundMask(sourcePixels, readableSource.width, readableSource.height, backgroundThreshold)
                : null;
            Color tintWithoutAlpha = new Color(tint.r, tint.g, tint.b, 1f);
            float blendAmount = Mathf.Clamp01(opacity * tint.a);
            float iconDetailAmount = Mathf.Max(0f, visibility);

            // Use grayscale as the base so the output lands much closer to the existing disabled icon palette.
            for (int i = 0; i < sourcePixels.Length; i++)
            {
                Color sourcePixel = sourcePixels[i];
                if (sourcePixel.a <= 0f)
                {
                    outputPixels[i] = Color.clear;
                    continue;
                }

                float luminance = sourcePixel.grayscale;
                Color grayscaleBaseColor = grayscale
                    ? new Color(luminance, luminance, luminance, sourcePixel.a)
                    : sourcePixel;
                Color sourceBaseColor = new Color(sourcePixel.r, sourcePixel.g, sourcePixel.b, sourcePixel.a);

                Color grayscaleTintedColor = Color.Lerp(grayscaleBaseColor, tintWithoutAlpha, blendAmount);
                Color sourceTintedColor = Color.Lerp(sourceBaseColor, tintWithoutAlpha, blendAmount);
                Color defaultTintedColor = grayscale ? grayscaleTintedColor : sourceTintedColor;
                Color tintOnlyColor = tintWithoutAlpha;
                Color objectColor;
                if (iconDetailAmount <= 1f)
                {
                    objectColor = Color.Lerp(tintOnlyColor, defaultTintedColor, iconDetailAmount);
                }
                else if (iconDetailAmount <= 2f)
                {
                    float sourceTintBlend = Mathf.Clamp01(iconDetailAmount - 1f);
                    objectColor = Color.Lerp(defaultTintedColor, sourceTintedColor, sourceTintBlend);
                }
                else
                {
                    float sourceColorBlend = Mathf.Clamp01((iconDetailAmount - 2f) / 2f);
                    objectColor = Color.Lerp(sourceTintedColor, sourceBaseColor, sourceColorBlend);
                }

                float maskStrength = foregroundMask != null ? foregroundMask[i] : 1f;
                Color finalColor = Color.Lerp(tintOnlyColor, objectColor, maskStrength);
                finalColor.a = sourcePixel.a;
                outputPixels[i] = finalColor;
            }

            outputTexture.SetPixels(outputPixels);
            outputTexture.Apply();
            return outputTexture;
        }
        finally
        {
            DestroyImmediate(readableSource);
        }
    }

    private static float[] CreateForegroundMask(Color[] pixels, int width, int height, float backgroundThreshold)
    {
        float[] mask = new float[pixels.Length];
        if (pixels.Length == 0 || width <= 0 || height <= 0)
        {
            return mask;
        }

        Color backgroundColor = EstimateBackgroundColor(pixels, width, height);
        bool[] backgroundPixels = new bool[pixels.Length];
        bool[] visited = new bool[pixels.Length];
        Queue<int> queue = new Queue<int>();
        float threshold = Mathf.Clamp(backgroundThreshold, 0.02f, 1f);

        EnqueueBackgroundSeedRange(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, 0, width, 1);
        EnqueueBackgroundSeedRange(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, (height - 1) * width, width, 1);
        for (int y = 1; y < height - 1; y++)
        {
            EnqueueBackgroundSeed(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, y * width);
            EnqueueBackgroundSeed(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, y * width + width - 1);
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            int x = index % width;
            int y = index / width;

            TryVisitBackgroundNeighbor(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, height, x - 1, y);
            TryVisitBackgroundNeighbor(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, height, x + 1, y);
            TryVisitBackgroundNeighbor(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, height, x, y - 1);
            TryVisitBackgroundNeighbor(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, width, height, x, y + 1);
        }

        int foregroundCount = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0f && !backgroundPixels[i])
            {
                mask[i] = 1f;
                foregroundCount++;
            }
        }

        if (foregroundCount == 0)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                mask[i] = pixels[i].a > 0f ? 1f : 0f;
            }
            return mask;
        }

        mask = BlurMask(mask, width, height, 2);
        for (int i = 0; i < pixels.Length; i++)
        {
            if (backgroundPixels[i] || pixels[i].a <= 0f)
            {
                mask[i] = 0f;
            }
        }
        return mask;
    }

    private static void EnqueueBackgroundSeedRange(
        Queue<int> queue,
        bool[] visited,
        bool[] backgroundPixels,
        Color[] pixels,
        Color backgroundColor,
        float threshold,
        int width,
        int startIndex,
        int count,
        int step)
    {
        for (int i = 0; i < count; i += step)
        {
            EnqueueBackgroundSeed(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, startIndex + i);
        }
    }

    private static void EnqueueBackgroundSeed(
        Queue<int> queue,
        bool[] visited,
        bool[] backgroundPixels,
        Color[] pixels,
        Color backgroundColor,
        float threshold,
        int index)
    {
        if (visited[index] || pixels[index].a <= 0f || !IsBackgroundLike(pixels[index], backgroundColor, threshold))
        {
            return;
        }

        visited[index] = true;
        backgroundPixels[index] = true;
        queue.Enqueue(index);
    }

    private static void TryVisitBackgroundNeighbor(
        Queue<int> queue,
        bool[] visited,
        bool[] backgroundPixels,
        Color[] pixels,
        Color backgroundColor,
        float threshold,
        int width,
        int height,
        int x,
        int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return;
        }

        int index = y * width + x;
        EnqueueBackgroundSeed(queue, visited, backgroundPixels, pixels, backgroundColor, threshold, index);
    }

    private static Color EstimateBackgroundColor(Color[] pixels, int width, int height)
    {
        Color sum = Color.black;
        int count = 0;

        for (int x = 0; x < width; x++)
        {
            AddColorIfOpaque(ref sum, ref count, pixels[x]);
            if (height > 1)
            {
                AddColorIfOpaque(ref sum, ref count, pixels[(height - 1) * width + x]);
            }
        }

        for (int y = 1; y < height - 1; y++)
        {
            AddColorIfOpaque(ref sum, ref count, pixels[y * width]);
            if (width > 1)
            {
                AddColorIfOpaque(ref sum, ref count, pixels[y * width + width - 1]);
            }
        }

        if (count == 0)
        {
            return Color.clear;
        }

        Color backgroundColor = sum / count;
        backgroundColor.a = 1f;
        return backgroundColor;
    }

    private static void AddColorIfOpaque(ref Color sum, ref int count, Color color)
    {
        if (color.a <= 0f)
        {
            return;
        }

        sum += new Color(color.r, color.g, color.b, 1f);
        count++;
    }

    private static bool IsBackgroundLike(Color color, Color backgroundColor, float threshold)
    {
        float dr = color.r - backgroundColor.r;
        float dg = color.g - backgroundColor.g;
        float db = color.b - backgroundColor.b;
        float distance = Mathf.Sqrt((dr * dr + dg * dg + db * db) / 3f);
        return distance <= threshold;
    }

    private static float[] BlurMask(float[] mask, int width, int height, int passes)
    {
        float[] current = (float[])mask.Clone();
        float[] next = new float[mask.Length];

        for (int pass = 0; pass < passes; pass++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sum = 0f;
                    int count = 0;
                    for (int offsetY = -1; offsetY <= 1; offsetY++)
                    {
                        int sampleY = y + offsetY;
                        if (sampleY < 0 || sampleY >= height)
                        {
                            continue;
                        }

                        for (int offsetX = -1; offsetX <= 1; offsetX++)
                        {
                            int sampleX = x + offsetX;
                            if (sampleX < 0 || sampleX >= width)
                            {
                                continue;
                            }

                            sum += current[sampleY * width + sampleX];
                            count++;
                        }
                    }

                    next[y * width + x] = count > 0 ? sum / count : current[y * width + x];
                }
            }

            float[] swap = current;
            current = next;
            next = swap;
        }

        return current;
    }

    private static Texture2D MakeReadableCopy(Texture2D source)
    {
        RenderTexture temporary = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.sRGB);

        RenderTexture previous = RenderTexture.active;
        try
        {
            Graphics.Blit(source, temporary);
            RenderTexture.active = temporary;

            Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            readable.ReadPixels(new Rect(0f, 0f, source.width, source.height), 0, 0);
            readable.Apply();
            return readable;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(temporary);
        }
    }

    private static string GetSuggestedOutputPath(Texture2D texture)
    {
        if (texture == null)
        {
            return string.Empty;
        }

        string sourcePath = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(sourcePath))
        {
            return string.Empty;
        }

        string directory = Path.GetDirectoryName(sourcePath);
        string extension = Path.GetExtension(sourcePath);
        string fileName = Path.GetFileNameWithoutExtension(sourcePath);

        string outputName = fileName;
        string[] knownSuffixes = { "_On", "_Off", "_Inactive", "_Disabled" };
        for (int i = 0; i < knownSuffixes.Length; i++)
        {
            string suffix = knownSuffixes[i];
            if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                outputName = fileName.Substring(0, fileName.Length - suffix.Length) + "_Disabled";
                break;
            }
        }

        if (outputName == fileName)
        {
            outputName += "_Disabled";
        }

        return Path.Combine(directory ?? "Assets", outputName + extension).Replace("\\", "/");
    }

    private static Texture2D GetSuggestedImportSettingsSource(Texture2D texture)
    {
        if (texture == null)
        {
            return null;
        }

        if (UsesSpriteImportSettings(texture))
        {
            return texture;
        }

        string sourcePath = AssetDatabase.GetAssetPath(texture);
        string directory = Path.GetDirectoryName(sourcePath)?.Replace("\\", "/");
        if (string.IsNullOrEmpty(directory))
        {
            return texture;
        }

        string[] preferredSuffixes = { "_Disabled.png", "_Off.png", "_On.png", ".png" };
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { directory });

        for (int suffixIndex = 0; suffixIndex < preferredSuffixes.Length; suffixIndex++)
        {
            string suffix = preferredSuffixes[suffixIndex];
            for (int i = 0; i < guids.Length; i++)
            {
                string candidatePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (string.Equals(candidatePath, sourcePath, StringComparison.OrdinalIgnoreCase) ||
                    !candidatePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Texture2D candidateTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(candidatePath);
                if (candidateTexture != null && UsesSpriteImportSettings(candidateTexture))
                {
                    return candidateTexture;
                }
            }
        }

        return texture;
    }

    private static bool UsesSpriteImportSettings(Texture2D texture)
    {
        string assetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        return importer != null && importer.textureType == TextureImporterType.Sprite;
    }

    private static void CopyTextureImportSettings(string sourcePath, string targetPath)
    {
        TextureImporter sourceImporter = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
        TextureImporter targetImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
        if (sourceImporter == null || targetImporter == null)
        {
            return;
        }

        TextureImporterSettings settings = new TextureImporterSettings();
        sourceImporter.ReadTextureSettings(settings);
        targetImporter.SetTextureSettings(settings);

        targetImporter.textureType = sourceImporter.textureType;
        targetImporter.textureShape = sourceImporter.textureShape;
        targetImporter.sRGBTexture = sourceImporter.sRGBTexture;
        targetImporter.alphaSource = sourceImporter.alphaSource;
        targetImporter.alphaIsTransparency = sourceImporter.alphaIsTransparency;
        targetImporter.mipmapEnabled = sourceImporter.mipmapEnabled;
        targetImporter.npotScale = sourceImporter.npotScale;
        targetImporter.textureCompression = sourceImporter.textureCompression;
        targetImporter.crunchedCompression = sourceImporter.crunchedCompression;
        targetImporter.compressionQuality = sourceImporter.compressionQuality;
        targetImporter.filterMode = sourceImporter.filterMode;
        targetImporter.wrapMode = sourceImporter.wrapMode;
        targetImporter.anisoLevel = sourceImporter.anisoLevel;
        targetImporter.spriteImportMode = sourceImporter.spriteImportMode;
        targetImporter.spritePixelsPerUnit = sourceImporter.spritePixelsPerUnit;
        targetImporter.spritePivot = sourceImporter.spritePivot;
        targetImporter.spriteBorder = sourceImporter.spriteBorder;
        targetImporter.isReadable = sourceImporter.isReadable;
        targetImporter.SaveAndReimport();
    }
}
