using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExportSubSprites
{
    [MenuItem("Assets/Extract Sub-Sprites")]
    public static void DoExportSubSprites()
    {
        var folder = EditorUtility.OpenFolderPanel("Extract subsprites to which folder?", "", "");
        if (string.IsNullOrEmpty(folder)) return;

        foreach (var obj in Selection.objects)
        {
            var texture = obj as Texture2D;
            if (texture == null) continue;

            string path = AssetDatabase.GetAssetPath(texture);
            EnsureTextureIsReadable(path);  // Ensure texture is readable

            var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var subSprite in sprites)
            {
                if (subSprite is Sprite sprite)
                {
                    var extracted = ExtractAndName(sprite);
                    SaveSubSprite(extracted, folder);
                }
            }
        }

        AssetDatabase.Refresh(); // Refresh to show the new files in Unity
    }

    [MenuItem("Assets/Extract Sub-Sprites", true)]
    private static bool CanExportSubSprites()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D texture)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                // Check if the texture is a sprite and if it has multiple sprites sliced
                if (importer != null && importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Multiple)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static void EnsureTextureIsReadable(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    private static Texture2D ExtractAndName(Sprite sprite)
    {
        var r = sprite.textureRect;
        var output = new Texture2D((int)r.width, (int)r.height);
        var pixels = sprite.texture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        output.SetPixels(pixels);
        output.Apply();
        output.name = sprite.texture.name + "_" + sprite.name;
        return output;
    }

    private static void SaveSubSprite(Texture2D tex, string saveToDirectory)
    {
        if (!System.IO.Directory.Exists(saveToDirectory)) System.IO.Directory.CreateDirectory(saveToDirectory);
        System.IO.File.WriteAllBytes(System.IO.Path.Combine(saveToDirectory, tex.name + ".png"), tex.EncodeToPNG());
    }
}
