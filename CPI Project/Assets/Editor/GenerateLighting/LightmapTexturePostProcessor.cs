using UnityEditor;
using UnityEngine;

internal static class LightmapTexturePostProcessor
{
    private const int LightmapTextureSize = 1024;

    internal static void SetLightmapTextureSize()
    {
        foreach (LightmapData lightmap in LightmapSettings.lightmaps)
        {
            SetTextureSize(lightmap.lightmapColor);
            SetTextureSize(lightmap.lightmapDir);
            SetTextureSize(lightmap.shadowMask);
        }
    }

    private static void SetTextureSize(Texture texture)
    {
        if (texture == null)
        {
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        bool needsReimport = importer.maxTextureSize != LightmapTextureSize
            || importer.mipmapEnabled
            || importer.textureCompression != TextureImporterCompression.CompressedHQ
            || importer.filterMode != FilterMode.Trilinear
            || importer.anisoLevel != 16;

        importer.maxTextureSize = LightmapTextureSize;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.filterMode = FilterMode.Trilinear;
        importer.anisoLevel = 16;

        if (needsReimport)
        {
            importer.SaveAndReimport();
        }
    }
}