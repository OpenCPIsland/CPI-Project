using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

public class GUIDReplacer : EditorWindow
{
    private Vector2 scrollPos;

    private List<GuidPair> guidPairs = new List<GuidPair>();
    private int selectedTab = 0;
    private string[] tabs = new string[] { "Sprite", "Texture", "Audio", "Material", "Animation", "Mesh", "Prefab", "MonoBehaviour", "Font", "Audio Mixer", "Shader", "Sprite Restorer" };

    private List<AssetPair>[] assetPairs;
    private List<AssetPair> animationClipPairs = new List<AssetPair>();
    private List<AssetPair> animatorControllerPairs = new List<AssetPair>();
    private List<AssetPair> animatorOverrideControllerPairs = new List<AssetPair>();

    private List<AssetPair> audioMixerPairs = new List<AssetPair>();
    private List<AudioMixerGroupMapping> audioMixerGroupMappings = new List<AudioMixerGroupMapping>();
    private Vector2 mixerGroupScroll;

    private List<Texture2D> selectedTextures = new List<Texture2D>();
    private Vector2 spriteRestorerScroll;
    private Vector2 spriteRestorerTextureScroll;
    private List<string> foundAssetPaths = new List<string>();
    private Dictionary<string, Sprite> originalSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> newSprites = new Dictionary<string, Sprite>();

    private string[] ignoreFolders = new string[] { "meshes", "fbx", "mesh", "models", "costumes", "costumefbxs", "model" };

    private static readonly string Manual =
        "There is a duplicate button for assets that has multiple duplications. " +
        "Once you get the amount of stuff that you want (can be multiple of assets), press replace. It will run. " +
        "After it finishes, press File -> Save project. \n" +
        "Review any error dialogs to make sure replacement did not fail because of a missing script. " +
        "If that is the case, manually fix that prefab and then rerun the replacement task and once it finishes, you can press the \"delete find assets\" button. " +
        "Be careful with the texture tab replacing though, sometimes that can be a hit or miss with materials. \n" +
        "Be careful with the Sprites and Sprites Restorer tab. Sometimes it forgets or leaves out replacements inside of Animation Clips. \n" +
        "Most of the time, it will work properly but in case if it doesn't, you will need to manually update it by replacing the old sprites with the new sprites \n" +
        "with the Project -> Editor -> Animation Clip Sprite Replacer tool before deleting the old sprites. Only way to see if it replaces properly is to delete \n" +
        "the old ones first while having a backup, then test. If everything is fine, then you will be good to go. If not, then you will need to see \n" +
        "what doesn't get replaced properly after restoring the previous backup.";

    private Rect manualIconRect;
    private bool showManualTooltip = false;

    [MenuItem("Project/Tools/Editor/GUID Replacer")]
    public static void ShowWindow()
    {
        GetWindow<GUIDReplacer>("GUID & Asset Replacer");
    }

    private void OnEnable()
    {
        if (assetPairs == null || assetPairs.Length != tabs.Length - 1)
        {
            assetPairs = new List<AssetPair>[tabs.Length - 1];
            for (int i = 0; i < tabs.Length - 1; i++)
                assetPairs[i] = new List<AssetPair>();
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        GUILayout.Label("GUID Replacements", EditorStyles.boldLabel);
        if (GUILayout.Button("Add GUID Pair"))
            guidPairs.Add(new GuidPair());

        for (int i = 0; i < guidPairs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Find", GUILayout.Width(40));
            guidPairs[i].findGuid = EditorGUILayout.TextField(guidPairs[i].findGuid, GUILayout.Width(200));

            GUILayout.Label("Replace", GUILayout.Width(55));
            guidPairs[i].replaceGuid = EditorGUILayout.TextField(guidPairs[i].replaceGuid, GUILayout.Width(200));

            if (GUILayout.Button("Duplicate", GUILayout.Width(75)))
            {
                guidPairs.Insert(i + 1, new GuidPair
                {
                    findGuid = guidPairs[i].findGuid,
                    replaceGuid = guidPairs[i].replaceGuid
                });
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                guidPairs.RemoveAt(i);
                i--;
            }

            if (!string.IsNullOrEmpty(guidPairs[i].findGuid))
            {
                EditorGUILayout.SelectableLabel(guidPairs[i].findGuid, GUILayout.Width(150));
            }
            if (!string.IsNullOrEmpty(guidPairs[i].replaceGuid))
            {
                EditorGUILayout.SelectableLabel(guidPairs[i].replaceGuid, GUILayout.Width(150));
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(15);

        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
        GUILayout.Space(10);

        if (selectedTab == 4)
        {
            DrawAnimationTab();
        }
        else if (selectedTab == 9)
        {
            DrawAudioMixerTab();
        }
        else if (selectedTab == 11)
        {
            DrawSpriteRestorerTab();
        }
        else
        {
            DrawNormalTab(selectedTab);
        }

        GUILayout.Space(20);

        if (selectedTab != 11 && GUILayout.Button("Start Replacement", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Confirm Replacement",
                "Are you sure you want to perform the replacement? This operation will modify files.\n\nNote: Sprite references may not resolve if not imported properly.",
                "Yes", "No"))
            {
                StartReplacement();
            }
        }

        GUILayout.Space(8);
        GUILayout.BeginHorizontal();
        GUILayout.Space(6);
        GUIContent iconContent = EditorGUIUtility.IconContent("_Help");
        GUIStyle iconStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fixedWidth = 18,
            fixedHeight = 18,
            padding = new RectOffset(0, 0, 0, 0)
        };

        manualIconRect = GUILayoutUtility.GetRect(iconContent, iconStyle, GUILayout.Width(18), GUILayout.Height(18));
        GUI.Label(manualIconRect, iconContent, iconStyle);

        GUILayout.Label("Manual", EditorStyles.boldLabel, GUILayout.Height(18));

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        Vector2 mousePos = Event.current.mousePosition;
        showManualTooltip = manualIconRect.Contains(mousePos);

        if (showManualTooltip)
        {
            GUIStyle tooltipStyle = new GUIStyle(EditorStyles.helpBox);
            tooltipStyle.fontSize = 15;
            tooltipStyle.wordWrap = true;

            float tooltipWidth = 400f;
            float tooltipHeight = tooltipStyle.CalcHeight(new GUIContent(Manual), tooltipWidth);

            Rect tooltipRect = new Rect(
                manualIconRect.x + 20,
                manualIconRect.yMax + 2,
                tooltipWidth,
                tooltipHeight + 8
            );
            GUI.Box(tooltipRect, Manual, tooltipStyle);
            Repaint();
        }

        EditorGUILayout.EndScrollView();
    }

    private static string GetFileID(UnityEngine.Object obj)
    {
        if (obj == null) return string.Empty;

        string path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty(path)) return string.Empty;

        string metaFile = path + ".meta";
        if (!File.Exists(metaFile)) return string.Empty;

        string[] lines = File.ReadAllLines(metaFile);
        foreach (string line in lines)
        {
            if (line.StartsWith("  fileID:"))
            {
                return line.Substring(10).Trim();
            }
        }
        return string.Empty;
    }

    private void ReplaceAnimClipSpriteReferencesYAML_SingleFile(string animFilePath, Dictionary<string, string> guidMap, Dictionary<string, string> fileIdMap)
    {
        string content = File.ReadAllText(animFilePath);
        bool updated = false;

        foreach (var oldGuid in guidMap.Keys)
        {
            string newGuid = guidMap[oldGuid];

            string pattern = $@"\{{\s*fileID:\s*21300000,\s*guid:\s*{oldGuid},\s*type:\s*\d+\s*\}}";
            string replacement = $"{{fileID: 21300000, guid: {newGuid}, type: 3}}";
            var replaced = System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);

            if (replaced != content)
            {
                updated = true;
                content = replaced;
            }

            string blockPattern = $@"fileID:\s*21300000\s*\n\s*guid:\s*{oldGuid}\s*\n\s*type:\s*\d+";
            string blockReplacement = $"fileID: 21300000\nguid: {newGuid}\ntype: 3";
            replaced = System.Text.RegularExpressions.Regex.Replace(content, blockPattern, blockReplacement);

            if (replaced != content)
            {
                updated = true;
                content = replaced;
            }
        }

        foreach (var oldFileId in fileIdMap.Keys)
        {
            string newFileId = fileIdMap[oldFileId];
            foreach (var oldGuid in guidMap.Keys)
            {
                string newGuid = guidMap[oldGuid];
                string pattern = $@"\{{\s*fileID:\s*{oldFileId},\s*guid:\s*{oldGuid},\s*type:\s*\d+\s*\}}";
                string replacement = $"{{fileID: {newFileId}, guid: {newGuid}, type: 3}}";
                var replaced = System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);

                if (replaced != content)
                {
                    updated = true;
                    content = replaced;
                }

                string blockPattern = $@"fileID:\s*{oldFileId}\s*\n\s*guid:\s*{oldGuid}\s*\n\s*type:\s*\d+";
                string blockReplacement = $"fileID: {newFileId}\nguid: {newGuid}\ntype: 3";
                replaced = System.Text.RegularExpressions.Regex.Replace(content, blockPattern, blockReplacement);

                if (replaced != content)
                {
                    updated = true;
                    content = replaced;
                }
            }
        }

        if (updated)
        {
            File.WriteAllText(animFilePath, content, Encoding.UTF8);
        }
    }

    private void ReplaceAnimClipSpriteReferencesYAML(Dictionary<string, string> guidMap, Dictionary<string, string> fileIdMap)
    {
        string[] animFiles = Directory.GetFiles(Application.dataPath, "*.anim", SearchOption.AllDirectories);
        int modified = 0;
        foreach (string filePath in animFiles)
        {
            string content = File.ReadAllText(filePath);
            bool updated = false;

            foreach (var oldGuid in guidMap.Keys)
            {
                string newGuid = guidMap[oldGuid];
                if (content.Contains(oldGuid))
                {
                    content = content.Replace(oldGuid, newGuid);
                    updated = true;
                }
            }
            foreach (var oldFileId in fileIdMap.Keys)
            {
                string newFileId = fileIdMap[oldFileId];
                content = System.Text.RegularExpressions.Regex.Replace(
                    content,
                    $"fileID: ?{oldFileId}, guid:",
                    $"fileID: {newFileId}, guid:",
                    System.Text.RegularExpressions.RegexOptions.None,
                    System.TimeSpan.FromMilliseconds(100)
                );
            }
            if (updated)
            {
                File.WriteAllText(filePath, content, Encoding.UTF8);
                modified++;
            }
        }
        if (modified > 0)
            AssetDatabase.Refresh();
    }

    private void ReplaceSpriteReferencesUsingGuids()
    {
        try
        {
            Dictionary<string, string> spriteGuidMap = new Dictionary<string, string>();
            Dictionary<string, string> spriteFileIdMap = new Dictionary<string, string>();

            foreach (var pair in assetPairs[0])
            {
                if (pair.findAsset != null && pair.replaceAsset != null)
                {
                    string findGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.findAsset));
                    string replaceGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.replaceAsset));

                    string findFileId = GetFileID(pair.findAsset);
                    string replaceFileId = GetFileID(pair.replaceAsset);

                    if (!string.IsNullOrEmpty(findGuid) && !string.IsNullOrEmpty(replaceGuid))
                    {
                        spriteGuidMap[findGuid] = replaceGuid;
                    }

                    if (!string.IsNullOrEmpty(findFileId) && !string.IsNullOrEmpty(replaceFileId))
                    {
                        spriteFileIdMap[findFileId] = replaceFileId;
                    }
                }
            }

            if (spriteGuidMap.Count == 0 && spriteFileIdMap.Count == 0)
            {
                EditorUtility.DisplayDialog("Info", "No sprite replacement pairs configured.", "OK");
                return;
            }

            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
            List<string> targetExtensions = new List<string> { ".prefab", ".unity", ".asset" };
            int modifiedFiles = 0;

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].Replace("\\", "/");

                if (EditorUtility.DisplayCancelableProgressBar("Replacing sprite references...",
                    Path.GetFileName(filePath), (float)i / files.Length))
                {
                    break;
                }

                if (!targetExtensions.Contains(Path.GetExtension(filePath).ToLower()) || filePath.EndsWith(".meta"))
                    continue;

                string assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);

                bool skip = false;
                foreach (var folder in ignoreFolders)
                {
                    if (assetPath.ToLower().Contains("/" + folder.ToLower() + "/"))
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;

                string content = File.ReadAllText(filePath);
                bool fileModified = false;

                foreach (var pair in spriteGuidMap)
                {
                    if (content.Contains(pair.Key))
                    {
                        content = content.Replace(pair.Key, pair.Value);
                        fileModified = true;
                    }
                }

                foreach (var pair in spriteFileIdMap)
                {
                    string findPattern = "fileID: " + pair.Key;
                    string replacePattern = "fileID: " + pair.Value;

                    if (content.Contains(findPattern))
                    {
                        content = content.Replace(findPattern, replacePattern);
                        fileModified = true;
                    }
                }

                if (fileModified)
                {
                    File.WriteAllText(filePath, content, Encoding.UTF8);
                    modifiedFiles++;
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done", $"Replaced sprite references in {modifiedFiles} files.", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private void DrawSpriteRestorerTab()
    {
        GUILayout.Label("Add one or more PNG textures below, then scan and restore.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add PNG Texture"))
            selectedTextures.Add(null);
        if (selectedTextures.Count > 0 && GUILayout.Button("Clear List", GUILayout.Width(90)))
            selectedTextures.Clear();
        EditorGUILayout.EndHorizontal();

        if (selectedTextures.Count > 0)
        {
            for (int i = 0; i < selectedTextures.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                selectedTextures[i] = EditorGUILayout.ObjectField($"PNG {i + 1}", selectedTextures[i], typeof(Texture2D), false) as Texture2D;
                if (GUILayout.Button("Duplicate", GUILayout.Width(75)))
                    selectedTextures.Insert(i + 1, selectedTextures[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    selectedTextures.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Find & Restore Sprites for Selected PNGs"))
        {
            bool anyValid = false;
            foreach (var t in selectedTextures)
                if (t != null) { anyValid = true; break; }
            if (!anyValid)
            {
                EditorUtility.DisplayDialog("Error", "Please add at least one PNG texture first.", "OK");
                return;
            }
            FindSpriteAssets();
        }

        GUILayout.Space(10);

        if (foundAssetPaths.Count > 0)
        {
            GUILayout.Label($"Found {foundAssetPaths.Count} sprite assets referencing selected textures:");
            foreach (var path in foundAssetPaths)
                EditorGUILayout.LabelField(path);

            if (GUILayout.Button("Restore Slices"))
            {
                RestoreSlices();
            }

            if (GUILayout.Button("Replace Sprite References"))
            {
                if (EditorUtility.DisplayDialog("Confirm Replacement",
                    "This will replace all references to these sprites. Continue?",
                    "Yes", "No"))
                {
                    ReplaceSpriteReferences();
                }
            }

            if (GUILayout.Button("Delete Old Sprite Assets"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    "This will delete all found sprite assets. Are you sure?",
                    "Yes", "No"))
                {
                    DeleteOldSpriteAssets();
                }
            }
        }
    }

    private void FindSpriteAssets()
    {
        foundAssetPaths.Clear();
        originalSprites.Clear();
        assetPairs[0].Clear();

        var validTextures = selectedTextures.Where(t => t != null).ToList();
        if (validTextures.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No valid PNG textures selected.", "OK");
            return;
        }

        var texturePaths = new HashSet<string>(validTextures.Select(t => AssetDatabase.GetAssetPath(t)).Where(p => !string.IsNullOrEmpty(p)));

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });

        int progress = 0;
        int total = guids.Length;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("Searching sprites", path, (float)progress / total);

            if (texturePaths.Contains(path))
            {
                progress++;
                continue;
            }

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null && validTextures.Any(t => sprite.texture == t))
            {
                foundAssetPaths.Add(path);
                originalSprites[path] = sprite;
            }
            progress++;
        }

        EditorUtility.ClearProgressBar();

        if (foundAssetPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("Result", "No sprites found referencing the selected PNGs.", "OK");
        }
    }

    private void RestoreSlices()
    {
        var validTextures = selectedTextures.Where(t => t != null).ToList();
        if (validTextures.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No valid PNG textures selected.", "OK");
            return;
        }

        newSprites.Clear();
        assetPairs[0].Clear();

        int totalRestored = 0;

        foreach (var tex in validTextures)
        {
            string texturePath = AssetDatabase.GetAssetPath(tex);
            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (importer == null)
            {
                EditorUtility.DisplayDialog("Texture Import Error", $"Failed to get TextureImporter for {texturePath}.", "OK");
                continue;
            }

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);

            if (dataProvider == null)
            {
                EditorUtility.DisplayDialog("Sprite Data Error", $"Failed to get sprite data provider for {texturePath}.", "OK");
                continue;
            }

            dataProvider.InitSpriteEditorDataProvider();

            List<SpriteRect> spriteRects = new List<SpriteRect>(dataProvider.GetSpriteRects());
            Dictionary<string, SpriteRect> existingSprites = spriteRects.ToDictionary(x => x.name, x => x);

            var pathsForThisTexture = foundAssetPaths.Where(p =>
            {
                Sprite s = originalSprites.ContainsKey(p) ? originalSprites[p] : null;
                return s != null && s.texture == tex;
            }).ToList();

            int progress = 0;
            int total = pathsForThisTexture.Count;

            foreach (var path in pathsForThisTexture)
            {
                EditorUtility.DisplayProgressBar($"Restoring slices for {tex.name}", path, (float)progress / Mathf.Max(total, 1));

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite != null)
                {
                    SpriteRect spriteRect;
                    if (!existingSprites.TryGetValue(sprite.name, out spriteRect))
                    {
                        spriteRect = new SpriteRect()
                        {
                            name = sprite.name,
                            rect = sprite.rect,
                            alignment = SpriteAlignment.Custom,
                            pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height),
                            border = sprite.border,
                            spriteID = GUID.Generate()
                        };
                        spriteRects.Add(spriteRect);
                        existingSprites[spriteRect.name] = spriteRect;
                    }
                    else
                    {
                        spriteRect.rect = sprite.rect;
                        spriteRect.pivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
                        spriteRect.border = sprite.border;
                    }

                    AssetPair pair = new AssetPair();
                    pair.findAsset = sprite;
                    pair.replaceAsset = null;
                    assetPairs[0].Add(pair);
                }
                progress++;
            }

            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

            Sprite[] allSprites = AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>().ToArray();
            foreach (var pair in assetPairs[0])
            {
                Sprite oldSprite = pair.findAsset as Sprite;
                if (oldSprite != null && oldSprite.texture == tex)
                {
                    Sprite newSprite = allSprites.FirstOrDefault(s => s.name == oldSprite.name);
                    if (newSprite != null)
                    {
                        pair.replaceAsset = newSprite;
                        newSprites[AssetDatabase.GetAssetPath(oldSprite)] = newSprite;
                    }
                }
            }

            totalRestored += spriteRects.Count;
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Done", $"Restored sprite slices across {validTextures.Count} texture(s). Total slices: {totalRestored}.", "OK");
    }

    private void ReplaceSpriteReferences()
    {
        if (assetPairs[0].Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprite replacement pairs configured. Did you restore slices first?", "OK");
            return;
        }

        try
        {
            Dictionary<Sprite, Sprite> spriteReplacements = new Dictionary<Sprite, Sprite>();
            Dictionary<string, string> fileIdReplacements = new Dictionary<string, string>();

            foreach (var pair in assetPairs[0])
            {
                if (pair.findAsset is Sprite findSprite && pair.replaceAsset is Sprite replaceSprite)
                {
                    spriteReplacements[findSprite] = replaceSprite;

                    string findFileId = GetFileID(findSprite);
                    string replaceFileId = GetFileID(replaceSprite);
                    if (!string.IsNullOrEmpty(findFileId) && !string.IsNullOrEmpty(replaceFileId))
                    {
                        fileIdReplacements[findFileId] = replaceFileId;
                    }
                }
            }

            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
            List<string> targetExtensions = new List<string> { ".prefab", ".unity", ".asset", ".mat", ".anim", ".controller" };
            int modifiedFiles = 0;

            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].Replace("\\", "/");

                if (EditorUtility.DisplayCancelableProgressBar("Replacing sprite references...",
                    Path.GetFileName(filePath), (float)i / files.Length))
                {
                    break;
                }

                if (!targetExtensions.Contains(Path.GetExtension(filePath).ToLower()) || filePath.EndsWith(".meta"))
                    continue;

                string assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);

                bool skip = false;
                foreach (var folder in ignoreFolders)
                {
                    if (assetPath.ToLower().Contains("/" + folder.ToLower() + "/"))
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;

                UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (mainAsset == null) continue;

                bool fileModified = false;

                if (Path.GetExtension(filePath).ToLower() == ".unity")
                {
                    var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    bool sceneModified = false;

                    foreach (var rootObj in scene.GetRootGameObjects())
                    {
                        foreach (var comp in rootObj.GetComponentsInChildren<Component>(true))
                        {
                            if (comp == null) continue;
                            SerializedObject so = new SerializedObject(comp);
                            SerializedProperty prop = so.GetIterator();

                            bool objectModified = false;

                            while (prop.NextVisible(true))
                            {
                                if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;

                                UnityEngine.Object val = prop.objectReferenceValue;
                                if (val is Sprite sprite && spriteReplacements.TryGetValue(sprite, out Sprite replacement))
                                {
                                    prop.objectReferenceValue = replacement;
                                    objectModified = true;
                                }
                            }

                            if (objectModified)
                            {
                                so.ApplyModifiedProperties();
                                sceneModified = true;
                            }
                        }
                    }

                    if (sceneModified)
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                        fileModified = true;
                    }

                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
                else if (mainAsset is AnimationClip animationClip)
                {
                    bool clipModified = false;

                    EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
                    foreach (EditorCurveBinding binding in bindings)
                    {
                        if (binding.type == typeof(SpriteRenderer) || binding.propertyName.Contains("m_Sprite"))
                        {
                            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);
                            bool curveModified = false;

                            for (int k = 0; k < keyframes.Length; k++)
                            {
                                if (keyframes[k].value is Sprite sprite)
                                {
                                    foreach (var pair in assetPairs[0])
                                    {
                                        if (pair.findAsset is Sprite findSprite &&
                                            pair.replaceAsset is Sprite replaceSprite)
                                        {
                                            if (sprite == findSprite)
                                            {
                                                keyframes[k].value = replaceSprite;
                                                curveModified = true;
                                                continue;
                                            }

                                            if (sprite.texture == findSprite.texture &&
                                                sprite.name == findSprite.name)
                                            {
                                                keyframes[k].value = replaceSprite;
                                                curveModified = true;
                                            }
                                        }
                                    }

                                    foreach (var restoredPair in newSprites)
                                    {
                                        Sprite restoredSprite = restoredPair.Value as Sprite;
                                        if (restoredSprite != null &&
                                            sprite.texture == restoredSprite.texture &&
                                            sprite.name == restoredSprite.name)
                                        {
                                            keyframes[k].value = restoredSprite;
                                            curveModified = true;
                                        }
                                    }
                                }
                            }

                            if (curveModified)
                            {
                                AnimationUtility.SetObjectReferenceCurve(animationClip, binding, keyframes);
                                clipModified = true;
                            }
                        }
                    }

                    SerializedObject clipSO = new SerializedObject(animationClip);
                    SerializedProperty spriteProperty = clipSO.FindProperty("m_Sprite");
                    if (spriteProperty != null && spriteProperty.objectReferenceValue != null)
                    {
                        Sprite sprite = spriteProperty.objectReferenceValue as Sprite;
                        if (sprite != null)
                        {
                            foreach (var pair in assetPairs[0])
                            {
                                if (pair.findAsset is Sprite findSprite &&
                                    pair.replaceAsset is Sprite replaceSprite)
                                {
                                    if (sprite == findSprite)
                                    {
                                        spriteProperty.objectReferenceValue = replaceSprite;
                                        clipModified = true;
                                        continue;
                                    }

                                    if (sprite.texture == findSprite.texture &&
                                        sprite.name == findSprite.name)
                                    {
                                        spriteProperty.objectReferenceValue = replaceSprite;
                                        clipModified = true;
                                    }
                                }
                            }
                        }
                    }

                    if (clipModified)
                    {
                        EditorUtility.SetDirty(animationClip);
                        fileModified = true;
                        AssetDatabase.SaveAssets();
                    }
                }
                else
                {
                    string content = File.ReadAllText(filePath);
                    bool contentModified = false;

                    foreach (var pair in fileIdReplacements)
                    {
                        string findPattern = "fileID: " + pair.Key;
                        string replacePattern = "fileID: " + pair.Value;

                        if (content.Contains(findPattern))
                        {
                            content = content.Replace(findPattern, replacePattern);
                            contentModified = true;
                        }
                    }

                    if (contentModified)
                    {
                        File.WriteAllText(filePath, content, Encoding.UTF8);
                        fileModified = true;
                    }

                    var allObjects = new List<UnityEngine.Object>();
                    if (mainAsset is GameObject go)
                        allObjects.AddRange(go.GetComponentsInChildren<Component>(true));
                    allObjects.Add(mainAsset);

                    foreach (var obj in allObjects)
                    {
                        if (obj == null) continue;

                        SerializedObject so = new SerializedObject(obj);
                        SerializedProperty prop = so.GetIterator();

                        while (prop.NextVisible(true))
                        {
                            if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;

                            UnityEngine.Object val = prop.objectReferenceValue;
                            if (val is Sprite sprite && spriteReplacements.TryGetValue(sprite, out Sprite replacement))
                            {
                                prop.objectReferenceValue = replacement;
                                fileModified = true;
                            }
                        }

                        if (fileModified)
                            so.ApplyModifiedProperties();
                    }
                }

                if (fileModified)
                    modifiedFiles++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            int animClipModified = 0;
            string[] animGuids = AssetDatabase.FindAssets("t:AnimationClip");
            for (int i = 0; i < animGuids.Length; i++)
            {
                string animAssetPath = AssetDatabase.GUIDToAssetPath(animGuids[i]);
                EditorUtility.DisplayProgressBar("Replacing sprites in animation clips...", Path.GetFileName(animAssetPath), (float)i / animGuids.Length);

                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animAssetPath);
                if (clip == null)
                {
                    var allClips = AssetDatabase.LoadAllAssetsAtPath(animAssetPath).OfType<AnimationClip>();
                    foreach (var embeddedClip in allClips)
                    {
                        if (ApplyAnimClipSpriteReplacement(embeddedClip, spriteReplacements))
                        {
                            EditorUtility.SetDirty(embeddedClip);
                            animClipModified++;
                        }
                    }
                    continue;
                }

                if (ApplyAnimClipSpriteReplacement(clip, spriteReplacements))
                {
                    EditorUtility.SetDirty(clip);
                    animClipModified++;
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done", $"Replaced sprite references in {modifiedFiles} files. Also updated {animClipModified} animation clip(s) directly.", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private bool ApplyAnimClipSpriteReplacement(AnimationClip clip, Dictionary<Sprite, Sprite> spriteReplacements)
    {
        bool clipModified = false;
        EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        foreach (EditorCurveBinding binding in bindings)
        {
            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            bool curveModified = false;
            for (int k = 0; k < keyframes.Length; k++)
            {
                if (keyframes[k].value is Sprite sprite && spriteReplacements.TryGetValue(sprite, out Sprite replacement))
                {
                    keyframes[k].value = replacement;
                    curveModified = true;
                }
            }
            if (curveModified)
            {
                AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
                clipModified = true;
            }
        }
        return clipModified;
    }

    private void DeleteOldSpriteAssets()
    {
        int deletedCount = 0;
        List<string> failedPaths = new List<string>();
        foreach (var path in foundAssetPaths)
        {
            if (AssetDatabase.DeleteAsset(path))
            {
                deletedCount++;
            }
            else
            {
                failedPaths.Add(path);
            }
        }

        AssetDatabase.Refresh();
        foundAssetPaths.Clear();
        originalSprites.Clear();
        newSprites.Clear();
        assetPairs[0].Clear();
        EditorUtility.DisplayDialog("Done", $"Deleted {deletedCount} old sprite assets.", "OK");
        DisplayDeleteFailures(failedPaths);
    }

    private static void DisplayDeleteFailures(List<string> failedPaths)
    {
        if (failedPaths.Count > 0)
            EditorUtility.DisplayDialog("Delete Failed", $"Failed to delete {failedPaths.Count} asset(s):\n{string.Join("\n", failedPaths)}", "OK");
    }

    private void DrawNormalTab(int tabIndex)
    {
        var currentList = assetPairs[tabIndex];

        if (GUILayout.Button($"Add {tabs[tabIndex]} Pair"))
            currentList.Add(new AssetPair());

        if (currentList.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear List"))
                currentList.Clear();

            if (GUILayout.Button("Delete 'Find' Assets"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    $"Delete all 'Find' assets from the list? This cannot be undone.",
                    "Yes", "No"))
                {
                    List<string> failedPaths = new List<string>();
                    foreach (var pair in currentList)
                    {
                        if (pair.findAsset != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(pair.findAsset);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                if (!AssetDatabase.DeleteAsset(assetPath))
                                    failedPaths.Add(assetPath);
                            }
                        }
                    }
                    AssetDatabase.Refresh();
                    DisplayDeleteFailures(failedPaths);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < currentList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Find", GUILayout.Width(30));
            currentList[i].findAsset = EditorGUILayout.ObjectField(currentList[i].findAsset, GetAssetType(tabIndex), false, GUILayout.Width(150));
            if (currentList[i].findAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(currentList[i].findAsset);
                var assetName = currentList[i].findAsset.name;
                EditorGUILayout.SelectableLabel($"{assetName} ({assetPath})", GUILayout.Width(220));
            }
            GUILayout.Label("Replace", GUILayout.Width(50));
            currentList[i].replaceAsset = EditorGUILayout.ObjectField(currentList[i].replaceAsset, GetAssetType(tabIndex), false, GUILayout.Width(150));
            if (currentList[i].replaceAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(currentList[i].replaceAsset);
                var assetName = currentList[i].replaceAsset.name;
                EditorGUILayout.SelectableLabel($"{assetName} ({assetPath})", GUILayout.Width(220));
            }
            if (GUILayout.Button("Duplicate", GUILayout.Width(75)))
            {
                currentList.Insert(i + 1, new AssetPair
                {
                    findAsset = currentList[i].findAsset,
                    replaceAsset = currentList[i].replaceAsset
                });
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                currentList.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAnimationTab()
    {
        DrawAnimationSubTab("Animation Clips", animationClipPairs, typeof(AnimationClip));
        DrawAnimationSubTab("Animator Controllers", animatorControllerPairs, typeof(AnimatorController));
        DrawAnimationSubTab("Animator Override Controllers", animatorOverrideControllerPairs, typeof(AnimatorOverrideController));
    }

    private void DrawAnimationSubTab(string label, List<AssetPair> list, System.Type assetType)
    {
        GUILayout.Space(10);
        GUILayout.Label(label, EditorStyles.boldLabel);

        if (GUILayout.Button("Add Pair"))
            list.Add(new AssetPair());

        if (list.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear List"))
                list.Clear();

            if (GUILayout.Button("Delete 'Find' Assets"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    $"Delete all 'Find' assets from the list? This cannot be undone.",
                    "Yes", "No"))
                {
                    List<string> failedPaths = new List<string>();
                    foreach (var pair in list)
                    {
                        if (pair.findAsset != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(pair.findAsset);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                if (!AssetDatabase.DeleteAsset(assetPath))
                                    failedPaths.Add(assetPath);
                            }
                        }
                    }
                    AssetDatabase.Refresh();
                    DisplayDeleteFailures(failedPaths);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Find", GUILayout.Width(30));
            list[i].findAsset = EditorGUILayout.ObjectField(list[i].findAsset, assetType, false, GUILayout.Width(150));
            if (list[i].findAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(list[i].findAsset);
                var assetName = list[i].findAsset.name;
                EditorGUILayout.SelectableLabel($"{assetName} ({assetPath})", GUILayout.Width(220));
            }
            GUILayout.Label("Replace", GUILayout.Width(50));
            list[i].replaceAsset = EditorGUILayout.ObjectField(list[i].replaceAsset, assetType, false, GUILayout.Width(150));
            if (list[i].replaceAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(list[i].replaceAsset);
                var assetName = list[i].replaceAsset.name;
                EditorGUILayout.SelectableLabel($"{assetName} ({assetPath})", GUILayout.Width(220));
            }
            if (GUILayout.Button("Duplicate", GUILayout.Width(75)))
            {
                list.Insert(i + 1, new AssetPair
                {
                    findAsset = list[i].findAsset,
                    replaceAsset = list[i].replaceAsset
                });
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                list.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawAudioMixerTab()
    {
        GUILayout.Label("Audio Mixer Replacement", EditorStyles.boldLabel);
        GUILayout.Label("Assign a Find and Replace Audio Mixer. Groups will be matched by name.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(5);

        if (GUILayout.Button("Add Audio Mixer Pair"))
            audioMixerPairs.Add(new AssetPair());

        if (audioMixerPairs.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear List"))
            {
                audioMixerPairs.Clear();
                audioMixerGroupMappings.Clear();
            }
            if (GUILayout.Button("Delete 'Find' Assets"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete",
                    "Delete all 'Find' Audio Mixer assets from the list? This cannot be undone.",
                    "Yes", "No"))
                {
                    List<string> failedPaths = new List<string>();
                    foreach (var pair in audioMixerPairs)
                    {
                        if (pair.findAsset != null)
                        {
                            string assetPath = AssetDatabase.GetAssetPath(pair.findAsset);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                if (!AssetDatabase.DeleteAsset(assetPath))
                                    failedPaths.Add(assetPath);
                            }
                        }
                    }
                    AssetDatabase.Refresh();
                    DisplayDeleteFailures(failedPaths);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < audioMixerPairs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Find", GUILayout.Width(30));
            audioMixerPairs[i].findAsset = EditorGUILayout.ObjectField(audioMixerPairs[i].findAsset, typeof(AudioMixer), false, GUILayout.Width(200));
            if (audioMixerPairs[i].findAsset != null)
            {
                var ap = AssetDatabase.GetAssetPath(audioMixerPairs[i].findAsset);
                EditorGUILayout.SelectableLabel($"{audioMixerPairs[i].findAsset.name} ({ap})", GUILayout.Width(220));
            }
            GUILayout.Label("Replace", GUILayout.Width(50));
            audioMixerPairs[i].replaceAsset = EditorGUILayout.ObjectField(audioMixerPairs[i].replaceAsset, typeof(AudioMixer), false, GUILayout.Width(200));
            if (audioMixerPairs[i].replaceAsset != null)
            {
                var ap = AssetDatabase.GetAssetPath(audioMixerPairs[i].replaceAsset);
                EditorGUILayout.SelectableLabel($"{audioMixerPairs[i].replaceAsset.name} ({ap})", GUILayout.Width(220));
            }
            if (GUILayout.Button("Duplicate", GUILayout.Width(75)))
            {
                audioMixerPairs.Insert(i + 1, new AssetPair
                {
                    findAsset = audioMixerPairs[i].findAsset,
                    replaceAsset = audioMixerPairs[i].replaceAsset
                });
            }
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                audioMixerPairs.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (audioMixerPairs.Count > 0 && GUILayout.Button("Build Group Mappings", GUILayout.Height(25)))
        {
            BuildAudioMixerGroupMappings();
        }

        if (audioMixerGroupMappings.Count > 0)
        {
            GUILayout.Space(5);
            GUILayout.Label($"Group Mappings ({audioMixerGroupMappings.Count}):", EditorStyles.boldLabel);
            mixerGroupScroll = EditorGUILayout.BeginScrollView(mixerGroupScroll, GUILayout.Height(200));

            for (int i = 0; i < audioMixerGroupMappings.Count; i++)
            {
                var mapping = audioMixerGroupMappings[i];
                EditorGUILayout.BeginHorizontal();

                Color prevColor = GUI.backgroundColor;
                if (mapping.isMixerSelf)
                    GUI.backgroundColor = new Color(0.7f, 0.85f, 1f);
                else if (mapping.replaceGroup == null)
                    GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
                else
                    GUI.backgroundColor = new Color(0.6f, 1f, 0.6f);

                GUILayout.Label(mapping.groupPath, GUILayout.Width(250));
                GUILayout.Label("→", GUILayout.Width(20));

                if (mapping.isMixerSelf)
                    GUILayout.Label("(mixer asset)", EditorStyles.miniLabel, GUILayout.Width(200));
                else if (mapping.replaceGroup != null)
                    GUILayout.Label(mapping.replaceGroup.name, GUILayout.Width(200));
                else
                    GUILayout.Label("(no match found)", EditorStyles.boldLabel, GUILayout.Width(200));

                GUILayout.Label($"[{mapping.findFileId} → {mapping.replaceFileId}]", EditorStyles.miniLabel, GUILayout.Width(200));

                GUI.backgroundColor = prevColor;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            int unmatchedCount = audioMixerGroupMappings.Count(m => m.replaceGroup == null && !m.isMixerSelf);
            if (unmatchedCount > 0)
            {
                EditorGUILayout.HelpBox($"{unmatchedCount} group(s) have no match in the replacement mixer. " +
                    "These references will NOT be updated.", MessageType.Warning);
            }
        }
    }

    private void BuildAudioMixerGroupMappings()
    {
        audioMixerGroupMappings.Clear();

        foreach (var pair in audioMixerPairs)
        {
            if (pair.findAsset == null || pair.replaceAsset == null) continue;

            AudioMixer findMixer = pair.findAsset as AudioMixer;
            AudioMixer replaceMixer = pair.replaceAsset as AudioMixer;
            if (findMixer == null || replaceMixer == null) continue;

            AudioMixerGroup[] findGroups = findMixer.FindMatchingGroups(string.Empty);
            AudioMixerGroup[] replaceGroups = replaceMixer.FindMatchingGroups(string.Empty);

            Dictionary<string, AudioMixerGroup> replaceGroupLookup = new Dictionary<string, AudioMixerGroup>();
            foreach (var g in replaceGroups)
            {
                if (g != null && !replaceGroupLookup.ContainsKey(g.name))
                    replaceGroupLookup[g.name] = g;
            }

            foreach (var findGroup in findGroups)
            {
                if (findGroup == null) continue;

                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(findGroup, out string findGuid, out long findLocalId);

                AudioMixerGroup matchedReplace = null;
                long replaceLocalId = 0;

                if (replaceGroupLookup.TryGetValue(findGroup.name, out matchedReplace))
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matchedReplace, out string replaceGuid, out replaceLocalId);
                }

                audioMixerGroupMappings.Add(new AudioMixerGroupMapping
                {
                    findGroup = findGroup,
                    replaceGroup = matchedReplace,
                    groupPath = findGroup.name,
                    findFileId = findLocalId,
                    replaceFileId = replaceLocalId
                });
            }

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(findMixer, out string findMixerGuid, out long findMixerId);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(replaceMixer, out string replaceMixerGuid, out long replaceMixerId);

            audioMixerGroupMappings.Add(new AudioMixerGroupMapping
            {
                findGroup = null,
                replaceGroup = null,
                groupPath = $"[Mixer] {findMixer.name}",
                findFileId = findMixerId,
                replaceFileId = replaceMixerId,
                isMixerSelf = true
            });
        }

    }

    private void ReplaceAudioMixerReferences()
    {
        if (audioMixerPairs.Count == 0) return;

        Dictionary<string, string> mixerGuidMap = new Dictionary<string, string>();
        Dictionary<string, string> fileIdMap = new Dictionary<string, string>();

        foreach (var pair in audioMixerPairs)
        {
            if (pair.findAsset == null || pair.replaceAsset == null) continue;

            string findGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.findAsset));
            string replaceGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.replaceAsset));

            if (!string.IsNullOrEmpty(findGuid) && !string.IsNullOrEmpty(replaceGuid) && findGuid != replaceGuid)
            {
                mixerGuidMap[findGuid] = replaceGuid;
            }
        }

        if (audioMixerGroupMappings.Count == 0)
            BuildAudioMixerGroupMappings();

        foreach (var mapping in audioMixerGroupMappings)
        {
            if (mapping.findFileId != 0 && mapping.replaceFileId != 0 && mapping.findFileId != mapping.replaceFileId)
            {
                if (mapping.replaceGroup != null || mapping.isMixerSelf)
                {
                    fileIdMap[mapping.findFileId.ToString()] = mapping.replaceFileId.ToString();
                }
            }
        }

        if (mixerGuidMap.Count == 0)
        {
            return;
        }

        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        List<string> targetExtensions = new List<string> { ".prefab", ".unity", ".asset", ".controller", ".overrideController", ".mixer" };

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i].Replace("\\", "/");

            if (EditorUtility.DisplayCancelableProgressBar("Replacing Audio Mixer references...",
                Path.GetFileName(filePath), (float)i / files.Length))
            {
                break;
            }

            string ext = Path.GetExtension(filePath).ToLower();
            if (!targetExtensions.Contains(ext) || filePath.EndsWith(".meta"))
                continue;

            string assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);

            bool skip = false;
            foreach (var folder in ignoreFolders)
            {
                if (assetPath.ToLower().Contains("/" + folder.ToLower() + "/"))
                {
                    skip = true;
                    break;
                }
            }
            if (skip) continue;

            string content = File.ReadAllText(filePath);
            bool fileModified = false;

            foreach (var guidPair in mixerGuidMap)
            {
                if (content.Contains(guidPair.Key))
                {
                    foreach (var fidPair in fileIdMap)
                    {
                        string findPattern = $"fileID: {fidPair.Key}, guid: {guidPair.Key}";
                        string replacePattern = $"fileID: {fidPair.Value}, guid: {guidPair.Value}";

                        if (content.Contains(findPattern))
                        {
                            content = content.Replace(findPattern, replacePattern);
                            fileModified = true;
                        }

                        string findCompact = $"fileID: {fidPair.Key}, guid: {guidPair.Key},";
                        string replaceCompact = $"fileID: {fidPair.Value}, guid: {guidPair.Value},";

                        if (content.Contains(findCompact))
                        {
                            content = content.Replace(findCompact, replaceCompact);
                            fileModified = true;
                        }
                    }

                    if (content.Contains(guidPair.Key))
                    {
                        content = content.Replace(guidPair.Key, guidPair.Value);
                        fileModified = true;
                    }
                }
            }

            if (fileModified)
            {
                File.WriteAllText(filePath, content, Encoding.UTF8);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    private System.Type GetAssetType(int index)
    {
        switch (index)
        {
            case 0: return typeof(Sprite);
            case 1: return typeof(Texture);
            case 2: return typeof(AudioClip);
            case 3: return typeof(Material);
            case 4: return typeof(UnityEngine.Object);
            case 5: return typeof(Mesh);
            case 6: return typeof(GameObject);
            case 7: return typeof(ScriptableObject);
            case 8: return typeof(Font);
            case 10: return typeof(Shader);
            default: return typeof(Object);
        }
    }

    private void StartReplacement()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        List<string> targetExtensions = new List<string> { ".asset", ".prefab", ".unity", ".mat", ".anim", ".controller", ".overrideController" };
        try
        {
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].Replace("\\", "/");

                if (EditorUtility.DisplayCancelableProgressBar("Replacing...", Path.GetFileName(filePath), (float)i / files.Length))
                {
                    break;
                }

                if (!targetExtensions.Contains(Path.GetExtension(filePath).ToLower()) || filePath.EndsWith(".meta"))
                    continue;

                string assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);

                bool skip = false;
                foreach (var folder in ignoreFolders)
                {
                    if (assetPath.ToLower().Contains("/" + folder.ToLower() + "/"))
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip)
                    continue;

                bool fileModified = false;

                if (guidPairs.Count > 0)
                {
                    string content = File.ReadAllText(filePath);
                    foreach (var pair in guidPairs)
                    {
                        if (!string.IsNullOrEmpty(pair.findGuid) && !string.IsNullOrEmpty(pair.replaceGuid))
                        {
                            if (content.Contains(pair.findGuid))
                            {
                                content = content.Replace(pair.findGuid, pair.replaceGuid);
                                fileModified = true;
                            }
                        }
                    }
                    if (fileModified)
                        File.WriteAllText(filePath, content, Encoding.UTF8);
                }

                if (Path.GetExtension(filePath).ToLower() == ".asset")
                {
                    string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                    bool isPlayMakerFSM = false;
                    foreach (var line in lines)
                    {
                        if (line.Contains("PlayMakerFSM"))
                        {
                            isPlayMakerFSM = true;
                            break;
                        }
                    }
                    if (isPlayMakerFSM)
                    {
                        string content = File.ReadAllText(filePath, Encoding.UTF8);
                        bool fsmModified = false;
                        foreach (var pair in guidPairs)
                        {
                            if (!string.IsNullOrEmpty(pair.findGuid) && !string.IsNullOrEmpty(pair.replaceGuid))
                            {
                                if (content.Contains(pair.findGuid))
                                {
                                    content = content.Replace(pair.findGuid, pair.replaceGuid);
                                    fsmModified = true;
                                }
                            }
                        }
                        void ReplaceAssetGuid(UnityEngine.Object find, UnityEngine.Object replace)
                        {
                            if (find == null || replace == null) return;
                            string findGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(find));
                            string replaceGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(replace));
                            if (!string.IsNullOrEmpty(findGuid) && !string.IsNullOrEmpty(replaceGuid) && findGuid != replaceGuid)
                            {
                                if (content.Contains(findGuid))
                                {
                                    content = content.Replace(findGuid, replaceGuid);
                                    fsmModified = true;
                                }
                            }
                        }
                        for (int tabIdx = 0; tabIdx < tabs.Length - 1; tabIdx++)
                        {
                            foreach (var assetPair in assetPairs[tabIdx])
                                ReplaceAssetGuid(assetPair.findAsset, assetPair.replaceAsset);
                        }
                        foreach (var pair in animationClipPairs) ReplaceAssetGuid(pair.findAsset, pair.replaceAsset);
                        foreach (var pair in animatorControllerPairs) ReplaceAssetGuid(pair.findAsset, pair.replaceAsset);
                        foreach (var pair in animatorOverrideControllerPairs) ReplaceAssetGuid(pair.findAsset, pair.replaceAsset);
                        if (fsmModified)
                        {
                            File.WriteAllText(filePath, content, Encoding.UTF8);
                        }
                    }
                }

                if (Path.GetExtension(filePath).ToLower() == ".unity")
                {
                    if (!File.Exists(assetPath))
                    {
                        EditorUtility.DisplayDialog("Scene Replacement Error", $"Scene file does not exist: {assetPath}", "OK");
                        continue;
                    }
                    try
                    {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(assetPath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        bool sceneModified = false;

                        foreach (var rootObj in scene.GetRootGameObjects())
                        {
                            foreach (var comp in rootObj.GetComponentsInChildren<Component>(true))
                            {
                                if (comp == null) continue;
                                SerializedObject so = new SerializedObject(comp);
                                SerializedProperty prop = so.GetIterator();

                                bool objectModified = false;

                                while (prop.NextVisible(true))
                                {
                                    if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;

                                    UnityEngine.Object val = prop.objectReferenceValue;

                                    for (int tabIdx = 0; tabIdx < tabs.Length - 1; tabIdx++)
                                    {
                                        if (tabIdx != 4)
                                        {
                                            foreach (var assetPair in assetPairs[tabIdx])
                                            {
                                                if (assetPair.findAsset != null && val == assetPair.findAsset)
                                                {
                                                    prop.objectReferenceValue = assetPair.replaceAsset;
                                                    objectModified = true;
                                                }
                                            }
                                        }
                                    }

                                    foreach (var pair in animationClipPairs)
                                    {
                                        if (pair.findAsset != null && val == pair.findAsset)
                                        {
                                            prop.objectReferenceValue = pair.replaceAsset;
                                            objectModified = true;
                                        }
                                    }
                                    foreach (var pair in animatorControllerPairs)
                                    {
                                        if (pair.findAsset != null && val == pair.findAsset)
                                        {
                                            prop.objectReferenceValue = pair.replaceAsset;
                                            objectModified = true;
                                        }
                                    }
                                    foreach (var pair in animatorOverrideControllerPairs)
                                    {
                                        if (pair.findAsset != null && val == pair.findAsset)
                                        {
                                            prop.objectReferenceValue = pair.replaceAsset;
                                            objectModified = true;
                                        }
                                    }
                                }

                                if (objectModified)
                                {
                                    so.ApplyModifiedProperties();
                                    sceneModified = true;
                                }
                            }
                        }

                        if (sceneModified)
                        {
                            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                            fileModified = true;
                        }

                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                    }
                    catch (System.Exception ex)
                    {
                        EditorUtility.DisplayDialog("Scene Replacement Error", $"Failed to process scene {assetPath}: {ex.Message}", "OK");
                    }
                    continue;
                }

                UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);

                if (mainAsset is AnimationClip animationClip)
                {
                    bool clipModified = false;

                    EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);
                    foreach (EditorCurveBinding binding in bindings)
                    {
                        if (binding.type == typeof(SpriteRenderer) || binding.propertyName.Contains("m_Sprite"))
                        {
                            ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);
                            bool curveModified = false;

                            for (int k = 0; k < keyframes.Length; k++)
                            {
                                if (keyframes[k].value is Sprite sprite)
                                {
                                    foreach (var pair in assetPairs[0])
                                    {
                                        if (pair.findAsset is Sprite findSprite &&
                                            pair.replaceAsset is Sprite replaceSprite)
                                        {
                                            if (sprite == findSprite)
                                            {
                                                keyframes[k].value = replaceSprite;
                                                curveModified = true;
                                                continue;
                                            }

                                            if (sprite.texture == findSprite.texture &&
                                                sprite.name == findSprite.name)
                                            {
                                                keyframes[k].value = replaceSprite;
                                                curveModified = true;
                                            }
                                        }
                                    }

                                    foreach (var restoredPair in newSprites)
                                    {
                                        Sprite restoredSprite = restoredPair.Value as Sprite;
                                        if (restoredSprite != null &&
                                            sprite.texture == restoredSprite.texture &&
                                            sprite.name == restoredSprite.name)
                                        {
                                            keyframes[k].value = restoredSprite;
                                            curveModified = true;
                                        }
                                    }
                                }
                            }

                            if (curveModified)
                            {
                                AnimationUtility.SetObjectReferenceCurve(animationClip, binding, keyframes);
                                clipModified = true;
                            }
                        }
                    }

                    SerializedObject clipSO = new SerializedObject(animationClip);
                    SerializedProperty spriteProperty = clipSO.FindProperty("m_Sprite");
                    if (spriteProperty != null && spriteProperty.objectReferenceValue != null)
                    {
                        Sprite sprite = spriteProperty.objectReferenceValue as Sprite;
                        if (sprite != null)
                        {
                            foreach (var pair in assetPairs[0])
                            {
                                if (pair.findAsset is Sprite findSprite &&
                                    pair.replaceAsset is Sprite replaceSprite)
                                {
                                    if (sprite == findSprite)
                                    {
                                        spriteProperty.objectReferenceValue = replaceSprite;
                                        clipModified = true;
                                        continue;
                                    }

                                    if (sprite.texture == findSprite.texture &&
                                        sprite.name == findSprite.name)
                                    {
                                        spriteProperty.objectReferenceValue = replaceSprite;
                                        clipModified = true;
                                    }
                                }
                            }
                        }
                    }

                    if (clipModified)
                    {
                        EditorUtility.SetDirty(animationClip);
                        fileModified = true;
                        AssetDatabase.SaveAssets();
                    }
                }
                else if (mainAsset is AnimatorController controller)
                {
                    bool controllerModified = false;

                    foreach (var layer in controller.layers)
                    {
                        foreach (var state in layer.stateMachine.states)
                        {
                            if (state.state.motion != null)
                            {
                                foreach (var pair in animationClipPairs)
                                {
                                    if (pair.findAsset != null && state.state.motion == pair.findAsset)
                                    {
                                        state.state.motion = (Motion)pair.replaceAsset;
                                        controllerModified = true;
                                    }
                                }
                            }
                        }
                    }

                    if (controllerModified)
                    {
                        EditorUtility.SetDirty(controller);
                        fileModified = true;
                    }
                }
                else if (mainAsset is AnimatorOverrideController overrideController)
                {
                    bool overrideModified = false;

                    List<KeyValuePair<AnimationClip, AnimationClip>> overrides =
                        new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
                    overrideController.GetOverrides(overrides);

                    for (int j = 0; j < overrides.Count; j++)
                    {
                        var currentOverride = overrides[j];
                        bool clipModified = false;
                        AnimationClip newOriginalClip = currentOverride.Key;
                        AnimationClip newOverrideClip = currentOverride.Value;

                        foreach (var pair in animationClipPairs)
                        {
                            if (pair.findAsset != null && currentOverride.Key == pair.findAsset)
                            {
                                newOriginalClip = (AnimationClip)pair.replaceAsset;
                                clipModified = true;
                            }

                            if (pair.findAsset != null && currentOverride.Value == pair.findAsset)
                            {
                                newOverrideClip = (AnimationClip)pair.replaceAsset;
                                clipModified = true;
                            }
                        }

                        if (clipModified)
                        {
                            overrides[j] = new KeyValuePair<AnimationClip, AnimationClip>(newOriginalClip, newOverrideClip);
                            overrideModified = true;
                        }
                    }

                    if (overrideModified)
                    {
                        overrideController.ApplyOverrides(overrides);
                        EditorUtility.SetDirty(overrideController);
                        fileModified = true;
                    }
                }
                else if (mainAsset is Material mat)
                {
                    bool matModified = false;
                    var shader = mat.shader;
#if UNITY_2021_1_OR_NEWER
                    int propertyCount = shader.GetPropertyCount();
                    for (int j = 0; j < propertyCount; j++)
                    {
                        if (shader.GetPropertyType(j) == UnityEngine.Rendering.ShaderPropertyType.Texture)
                        {
                            string propertyName = shader.GetPropertyName(j);
                            Texture currentTex = mat.GetTexture(propertyName);
                            foreach (var pair in assetPairs[1])
                            {
                                if (pair.findAsset != null && currentTex == pair.findAsset)
                                {
                                    mat.SetTexture(propertyName, (Texture)pair.replaceAsset);
                                    matModified = true;
                                }
                            }
                        }
                    }
#else
                    var so = new SerializedObject(mat);
                    var prop = so.GetIterator();
                    while (prop.NextVisible(true))
                    {
                        if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue is Texture currentTex)
                        {
                            foreach (var pair in assetPairs[1])
                            {
                                if (pair.findAsset != null && currentTex == pair.findAsset)
                                {
                                    prop.objectReferenceValue = pair.replaceAsset;
                                    matModified = true;
                                }
                            }
                        }
                    }
                    if (matModified)
                        so.ApplyModifiedProperties();
#endif
                    if (matModified)
                    {
                        EditorUtility.SetDirty(mat);
                        fileModified = true;
                    }
                }

                if (mainAsset != null)
                {
                    bool objectModified = false;
                    var allObjects = new List<UnityEngine.Object>();
                    if (mainAsset is GameObject go)
                        allObjects.AddRange(go.GetComponentsInChildren<Component>(true));
                    allObjects.Add(mainAsset);

                    foreach (var obj in allObjects)
                    {
                        if (obj == null) continue;

                        SerializedObject so = new SerializedObject(obj);
                        SerializedProperty prop = so.GetIterator();

                        while (prop.NextVisible(true))
                        {
                            if (prop.propertyType != SerializedPropertyType.ObjectReference) continue;

                            UnityEngine.Object val = prop.objectReferenceValue;

                            for (int tabIdx = 0; tabIdx < tabs.Length - 1; tabIdx++)
                            {
                                if (tabIdx != 4)
                                {
                                    foreach (var assetPair in assetPairs[tabIdx])
                                    {
                                        if (assetPair.findAsset != null && val == assetPair.findAsset)
                                        {
                                            prop.objectReferenceValue = assetPair.replaceAsset;
                                            objectModified = true;
                                        }
                                    }
                                }
                            }

                            foreach (var pair in animationClipPairs)
                            {
                                if (pair.findAsset != null && val == pair.findAsset)
                                {
                                    prop.objectReferenceValue = pair.replaceAsset;
                                    objectModified = true;
                                }
                            }
                            foreach (var pair in animatorControllerPairs)
                            {
                                if (pair.findAsset != null && val == pair.findAsset)
                                {
                                    prop.objectReferenceValue = pair.replaceAsset;
                                    objectModified = true;
                                }
                            }
                            foreach (var pair in animatorOverrideControllerPairs)
                            {
                                if (pair.findAsset != null && val == pair.findAsset)
                                {
                                    prop.objectReferenceValue = pair.replaceAsset;
                                    objectModified = true;
                                }
                            }
                        }

                        if (objectModified)
                            so.ApplyModifiedProperties();
                    }

                    if (objectModified)
                    {
                        fileModified = true;
                    }

                    if (Path.GetExtension(filePath).ToLower() == ".anim")
                    {
                        Dictionary<string, string> guidMap = new Dictionary<string, string>();
                        Dictionary<string, string> fileIdMap = new Dictionary<string, string>();
                        if (assetPairs != null && assetPairs.Length > 0)
                        {
                            foreach (var pair in assetPairs[0])
                            {
                                if (pair.findAsset != null && pair.replaceAsset != null)
                                {
                                    string findGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.findAsset));
                                    string replaceGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(pair.replaceAsset));
                                    string findFileId = GetFileID(pair.findAsset);
                                    string replaceFileId = GetFileID(pair.replaceAsset);

                                    if (!string.IsNullOrEmpty(findGuid) && !string.IsNullOrEmpty(replaceGuid))
                                        guidMap[findGuid] = replaceGuid;
                                    if (!string.IsNullOrEmpty(findFileId) && !string.IsNullOrEmpty(replaceFileId))
                                        fileIdMap[findFileId] = replaceFileId;
                                }
                            }
                        }
                        ReplaceAnimClipSpriteReferencesYAML_SingleFile(filePath, guidMap, fileIdMap);
                    }
                }

            }

        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (selectedTab == 9)
        {
            ReplaceAudioMixerReferences();
        }

        if (selectedTab == 10)
        {
            ReplaceShaderReferencesInMaterials();
        }
    }

    private void ReplaceShaderReferencesInMaterials()
    {
        List<AssetPair> shaderPairs = assetPairs[10]
            .Where(pair => pair.findAsset is Shader &&
                           pair.replaceAsset is Shader &&
                           pair.findAsset != pair.replaceAsset)
            .ToList();

        if (shaderPairs.Count == 0)
        {
            EditorUtility.DisplayDialog("Shader Replacement", "No valid Shader replacement pairs were configured.", "OK");
            return;
        }

        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        HashSet<int> processedMaterials = new HashSet<int>();
        foreach (string materialGuid in materialGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(materialGuid);
            foreach (Material material in AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Material>())
            {
                if (!processedMaterials.Add(material.GetInstanceID()))
                    continue;

                bool materialModified = false;
                foreach (AssetPair pair in shaderPairs)
                {
                    if (ShadersMatch(material.shader, (Shader)pair.findAsset))
                    {
                        material.shader = (Shader)pair.replaceAsset;
                        materialModified = true;
                    }
                }

                if (materialModified)
                    EditorUtility.SetDirty(material);
            }
        }

        AssetDatabase.SaveAssets();
    }

    private static bool ShadersMatch(Shader materialShader, Shader findShader)
    {
        if (materialShader == null || findShader == null)
            return false;
        if (materialShader == findShader)
            return true;

        bool hasMaterialIdentity = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
            materialShader, out string materialGuid, out long materialFileId);
        bool hasFindIdentity = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(
            findShader, out string findGuid, out long findFileId);
        if (hasMaterialIdentity && hasFindIdentity)
            return materialGuid == findGuid && materialFileId == findFileId;

        return string.Equals(materialShader.name, findShader.name, System.StringComparison.Ordinal);
    }

    private class GuidPair
    {
        public string findGuid = "";
        public string replaceGuid = "";
    }

    private class AssetPair
    {
        public UnityEngine.Object findAsset;
        public UnityEngine.Object replaceAsset;
    }

    private class AudioMixerGroupMapping
    {
        public AudioMixerGroup findGroup;
        public AudioMixerGroup replaceGroup;
        public string groupPath;
        public long findFileId;
        public long replaceFileId;
        public bool isMixerSelf;
    }
}
