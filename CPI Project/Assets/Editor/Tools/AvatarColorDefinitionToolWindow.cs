using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ClubPenguin;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;

public class AvatarColorDefinitionToolWindow : EditorWindow
{
    private const string MenuPath = "Project/Tools/Avatar Color Manager";
    private const string DefinitionsFolder = "Assets/Game/Avatar/Resources/Definitions";
    private const string GeneratedAvatarColorsAssetPath = "Assets/Generated/Resources/Avatar/AvatarColors.asset";
    private const string PenguinMannequinPath = "Assets/Game/Avatar/Penguin_Mannequin.prefab";
    private const string PenguinAnimatorControllerPath = "Assets/AnimatorController/UIPenguinAnimatorController.controller";
    private const string AvatarMaterialsFolder = "Assets/Game/Avatar/Materials";
    private const string IdleAnimationsFolder = "Assets/Game/ArtAssets/Avatar/Penguin/StandardAnimation/Idle";

    private static readonly string[] IdleClipNames = new string[]
    {
        "idle_default_breathe",
        "idle_default_lookAroundTwo",
        "idle_default_armSwing",
        "idle_default_lookAround",
        "idle_default_shuffle"
    };

    private static readonly int ShaderBodyRedChannelColor = Shader.PropertyToID("_BodyRedChannelColor");
    private static readonly int ShaderBodyGreenChannelColor = Shader.PropertyToID("_BodyGreenChannelColor");
    private static readonly int ShaderBodyBlueChannelColor = Shader.PropertyToID("_BodyBlueChannelColor");

    private static readonly HashSet<string> ScanExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".prefab",
        ".asset",
        ".unity"
    };

    private readonly List<DefinitionEntry> definitions = new List<DefinitionEntry>();
    private readonly Dictionary<string, List<UsageHit>> usageMap = new Dictionary<string, List<UsageHit>>(StringComparer.OrdinalIgnoreCase);

    private ReorderableList definitionsList;

    private Vector2 listScroll;
    private Vector2 detailsScroll;
    private Vector2 usageScroll;

    private int selectedIndex = -1;
    private string statusMessage = string.Empty;

    private bool includePrefabs = true;
    private bool includeAssets = true;
    private bool includeScenes = true;
    private bool hasScannedUsageCache = false;
    private bool showCreateDefinitionPanel = false;

    private int newDefinitionId = 0;
    private string newDefinitionColorName = string.Empty;
    private string newDefinitionColorString = "#FFFFFF";

    private PreviewRenderUtility previewRenderUtility;
    private GameObject previewInstance;
    private Animator previewAnimator;
    private List<PreviewBodyPart> previewBodyParts;
    private string previewAppliedGuid;
    private Color previewAppliedColor;
    private double lastPreviewTime;
    private float previewRotationY;
    private bool previewDragging;

    private AnimationClip[] idleClips;
    private int currentIdleClipIndex;
    private float idleClipTime;
    private float nextIdleSwapTime;
    private System.Random idleRandom;

    [MenuItem(MenuPath)]
    private static void OpenWindow()
    {
        AvatarColorDefinitionToolWindow window = GetWindow<AvatarColorDefinitionToolWindow>("Avatar Color Manager");
        window.minSize = new Vector2(1280f, 720f);
        window.LoadDefinitionsOnly();
    }

    private void OnEnable()
    {
        LoadDefinitionsOnly();
        InitPreview();
    }

    private void OnDisable()
    {
        CleanupPreview();
    }

    private void OnProjectChange()
    {
        LoadDefinitionsOnly(false);
    }

    private void Update()
    {
        if (previewAnimator != null && previewInstance != null && idleClips != null && idleClips.Length > 0)
        {
            double currentTime = EditorApplication.timeSinceStartup;
            float deltaTime = (float)(currentTime - lastPreviewTime);
            lastPreviewTime = currentTime;

            if (deltaTime > 0f && deltaTime < 0.5f)
            {
                idleClipTime += deltaTime;

                if (idleClipTime >= nextIdleSwapTime)
                {
                    PickNextIdleClip();
                }

                AnimationClip clip = idleClips[currentIdleClipIndex];
                float clipLength = clip.length;
                if (clipLength > 0f)
                {
                    float sampleTime = clip.isLooping
                        ? idleClipTime % clipLength
                        : Mathf.Min(idleClipTime, clipLength);
                    clip.SampleAnimation(previewInstance, sampleTime);
                }
            }

            Repaint();
        }
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (EditorSettings.serializationMode != SerializationMode.ForceText)
        {
            EditorGUILayout.HelpBox("Reference scanning and clearing works best when Asset Serialization Mode is Force Text.", MessageType.Warning);
        }

        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            DrawDefinitionsPanel();
            DrawDetailsPanel();
            DrawUsagePanel();
        }
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            if (GUILayout.Button("Refresh Definitions", EditorStyles.toolbarButton, GUILayout.Width(130f)))
            {
                LoadDefinitionsOnly();
            }

            if (GUILayout.Button("Scan Usages", EditorStyles.toolbarButton, GUILayout.Width(90f)))
            {
                ScanUsages();
            }

            GUI.enabled = definitions.Count > 0;
            if (GUILayout.Button("Save All", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            {
                SaveAllDefinitions();
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            GUILayout.Label("Definitions: " + definitions.Count, EditorStyles.miniLabel);
        }
    }

    private void DrawDefinitionsPanel()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(420f)))
        {
            EditorGUILayout.LabelField("Avatar Colors", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(showCreateDefinitionPanel ? "Hide New Definition" : "Create New Definition", GUILayout.Height(28f)))
                {
                    showCreateDefinitionPanel = !showCreateDefinitionPanel;
                }
            }

            if (showCreateDefinitionPanel)
            {
                GUILayout.Space(6f);
                DrawCreateDefinitionPanelInListArea();
                GUILayout.Space(8f);
            }

            if (definitions.Count == 0)
            {
                EditorGUILayout.HelpBox("No AvatarColorDefinition assets were found.", MessageType.Info);
                return;
            }

            listScroll = EditorGUILayout.BeginScrollView(listScroll);
            Rect listRect = GUILayoutUtility.GetRect(0f, GetDefinitionsListHeight(), GUILayout.ExpandWidth(true));
            definitionsList.DoList(listRect);
            EditorGUILayout.EndScrollView();
        }
    }

    private float GetDefinitionsListHeight()
    {
        if (definitionsList == null)
        {
            return 0f;
        }

        return definitionsList.GetHeight();
    }

    private void DrawCreateDefinitionPanelInListArea()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("New Definition", EditorStyles.boldLabel);

            newDefinitionId = EditorGUILayout.IntField("Definition ID", newDefinitionId);
            newDefinitionColorName = EditorGUILayout.TextField("Definition Name", newDefinitionColorName);
            newDefinitionColorString = EditorGUILayout.TextField("Color String", newDefinitionColorString ?? string.Empty);

            Color parsedColor;
            bool validColor = TryParseColorString(newDefinitionColorString, out parsedColor);
            if (!validColor)
            {
                parsedColor = Color.white;
            }

            EditorGUI.BeginChangeCheck();
            Color pickedColor = EditorGUILayout.ColorField("Color Picker", parsedColor);
            if (EditorGUI.EndChangeCheck())
            {
                newDefinitionColorString = ToHtmlColorString(pickedColor);
                validColor = true;
                parsedColor = pickedColor;
            }

            Rect previewRect = GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(previewRect, validColor ? parsedColor : new Color(1f, 0f, 1f, 1f));

            if (!validColor)
            {
                EditorGUILayout.HelpBox("Color String could not be parsed. Use a value like #FFA500.", MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = validColor;
                if (GUILayout.Button("Create", GUILayout.Height(28f)))
                {
                    CreateNewDefinition();
                }
                GUI.enabled = true;

                if (GUILayout.Button("Cancel", GUILayout.Height(28f)))
                {
                    showCreateDefinitionPanel = false;
                }
            }
        }
    }

    private void DrawDefinitionElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index < 0 || index >= definitions.Count)
        {
            return;
        }

        DefinitionEntry entry = definitions[index];

        rect.y += 2f;
        rect.height -= 4f;

        Rect swatchRect = new Rect(rect.x + 4f, rect.y + 6f, 22f, 22f);
        EditorGUI.DrawRect(swatchRect, entry.HasValidColor ? entry.PreviewColor : new Color(1f, 0f, 1f, 1f));

        Rect line1Rect = new Rect(rect.x + 32f, rect.y + 2f, rect.width - 36f, 18f);
        Rect line2Rect = new Rect(rect.x + 32f, rect.y + 20f, rect.width - 36f, 18f);

        string usageText = hasScannedUsageCache ? GetUsageCount(entry.Guid).ToString() : "?";
        string line1 = "ID " + entry.Definition.ColorId + "    " + (string.IsNullOrEmpty(entry.Definition.ColorName) ? "(No Name)" : entry.Definition.ColorName) + "    Uses: " + usageText;
        string line2 = Path.GetFileName(entry.Path);

        EditorGUI.LabelField(line1Rect, line1);
        EditorGUI.LabelField(line2Rect, line2, EditorStyles.miniLabel);
    }

    private void OnDefinitionsListSelected(ReorderableList list)
    {
        selectedIndex = list.index;
        Repaint();
    }

    private void OnDefinitionsListReordered(ReorderableList list, int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || newIndex < 0 || oldIndex >= definitions.Count || newIndex >= definitions.Count || oldIndex == newIndex)
        {
            RebuildDefinitionsList();
            return;
        }

        DefinitionEntry movedEntry = definitions[newIndex];
        string selectedGuid = movedEntry != null ? movedEntry.Guid : null;

        RenumberDefinitionsFromCurrentListOrder();

        SortDefinitionsPreserveSelection(selectedGuid);
        statusMessage = "Reordered definitions. IDs, ViewOrder, and names were updated to match the new list order. Save All to rename files.";
    }

    private void DrawDetailsPanel()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(500f)))
        {
            EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);

            detailsScroll = EditorGUILayout.BeginScrollView(detailsScroll);

            DefinitionEntry entry = SelectedEntry;
            if (entry == null)
            {
                EditorGUILayout.HelpBox("Select an AvatarColor definition from the left.", MessageType.Info);
                EditorGUILayout.EndScrollView();
                return;
            }

            AvatarColorDefinition definition = entry.Definition;

            EditorGUILayout.ObjectField("Definition", definition, typeof(AvatarColorDefinition), false);
            EditorGUILayout.LabelField("Path", entry.Path);
            EditorGUILayout.SelectableLabel(entry.Guid, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.LabelField("Cached Usage Count", hasScannedUsageCache ? GetUsageCount(entry.Guid).ToString() : "?");
            EditorGUILayout.LabelField("In AvatarColors.asset", IsInGeneratedAvatarColors(definition) ? "Yes" : "No");

            int requestedColorId = EditorGUILayout.IntField("Color ID", definition.ColorId);
            string newColorName = EditorGUILayout.TextField("Color Name", definition.ColorName ?? string.Empty);
            string currentColorString = definition.Color ?? string.Empty;
            string editedColorString = EditorGUILayout.TextField("Color String", currentColorString);

            Color parsedColor;
            bool validColor = TryParseColorString(editedColorString, out parsedColor);
            if (!validColor)
            {
                parsedColor = Color.white;
            }

            EditorGUI.BeginChangeCheck();
            Color pickedColor = EditorGUILayout.ColorField("Color Picker", parsedColor);
            if (EditorGUI.EndChangeCheck())
            {
                editedColorString = ToHtmlColorString(pickedColor);
                validColor = true;
                parsedColor = pickedColor;
            }

            Rect previewRect = GUILayoutUtility.GetRect(0f, 42f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(previewRect, validColor ? parsedColor : new Color(1f, 0f, 1f, 1f));

            if (!validColor)
            {
                EditorGUILayout.HelpBox("Color String could not be parsed. Use a value like #FFA500.", MessageType.Warning);
            }

            if (requestedColorId != definition.ColorId)
            {
                ApplyColorIdChange(entry, requestedColorId);
                entry = SelectedEntry;
                if (entry == null)
                {
                    EditorGUILayout.EndScrollView();
                    return;
                }

                definition = entry.Definition;
            }

            bool otherChanged =
                !string.Equals(newColorName, definition.ColorName ?? string.Empty, StringComparison.Ordinal) ||
                !string.Equals(editedColorString, definition.Color ?? string.Empty, StringComparison.Ordinal);

            if (otherChanged)
            {
                Undo.RecordObject(definition, "Edit Avatar Color Definition");
                definition.ColorName = newColorName;
                definition.Color = editedColorString;
                EditorUtility.SetDirty(definition);
                entry.RefreshPreview();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Ping Asset", GUILayout.Height(28f)))
            {
                EditorGUIUtility.PingObject(definition);
            }

            GUILayout.Space(10f);

            Color oldBackground = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.7f, 0.7f, 1f);

            if (GUILayout.Button("Delete Selected Definition And Clear References", GUILayout.Height(34f)))
            {
                DeleteSelectedDefinitionAndClearReferences();
            }

            GUI.backgroundColor = oldBackground;

            GUILayout.Space(10f);
            DrawPenguinPreview(entry);

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawPenguinPreview(DefinitionEntry entry)
    {
        if (previewRenderUtility == null || previewInstance == null)
        {
            if (GUILayout.Button("Initialize Penguin Preview", GUILayout.Height(28f)))
            {
                InitPreview();
            }

            return;
        }

        EditorGUILayout.LabelField("Penguin Preview", EditorStyles.boldLabel);

        bool needsColorUpdate = entry != null &&
            (!string.Equals(entry.Guid, previewAppliedGuid, StringComparison.OrdinalIgnoreCase) ||
             entry.PreviewColor != previewAppliedColor);

        if (needsColorUpdate)
        {
            ApplyColorToPreview(entry);
        }

        Rect previewRect = GUILayoutUtility.GetRect(256f, 320f, GUILayout.ExpandWidth(true));
        if (previewRect.width > 1f && previewRect.height > 1f)
        {
            HandlePreviewInput(previewRect);
            RenderPreview(previewRect);
        }
    }

    private void InitPreview()
    {
        CleanupPreview();

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PenguinMannequinPath);
        if (prefab == null)
        {
            return;
        }

        previewRenderUtility = new PreviewRenderUtility();
        previewRenderUtility.camera.fieldOfView = 30f;
        previewRenderUtility.camera.nearClipPlane = 0.01f;
        previewRenderUtility.camera.farClipPlane = 100f;
        previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
        previewRenderUtility.camera.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        previewRenderUtility.camera.transform.position = new Vector3(0f, 0.5f, -2.2f);
        previewRenderUtility.camera.transform.LookAt(new Vector3(0f, 0.4f, 0f));

        previewRenderUtility.lights[0].transform.localPosition = new Vector3(0f, 3f, 0f);
        previewRenderUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        previewRenderUtility.lights[0].color = new Color(1f, 0.957f, 0.839f, 1f);
        previewRenderUtility.lights[0].intensity = 1.9f;
        previewRenderUtility.lights[0].shadows = LightShadows.Soft;

        previewInstance = previewRenderUtility.InstantiatePrefabInScene(prefab);
        previewInstance.transform.position = Vector3.zero;
        previewInstance.transform.rotation = Quaternion.identity;
        previewRotationY = 160f;
        previewInstance.transform.rotation = Quaternion.Euler(0f, previewRotationY, 0f);

        SetupPreviewAnimator();
        CollectPreviewBodyParts();

        previewAppliedGuid = null;
        previewAppliedColor = Color.clear;
        lastPreviewTime = EditorApplication.timeSinceStartup;
    }

    private void SetupPreviewAnimator()
    {
        if (previewInstance == null)
        {
            return;
        }

        previewAnimator = previewInstance.GetComponent<Animator>();
        if (previewAnimator == null)
        {
            previewAnimator = previewInstance.AddComponent<Animator>();
        }

        RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PenguinAnimatorControllerPath);
        if (controller != null)
        {
            previewAnimator.runtimeAnimatorController = controller;
        }

        previewAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        previewAnimator.enabled = false;

        LoadIdleClips();
    }

    private void LoadIdleClips()
    {
        List<AnimationClip> clips = new List<AnimationClip>(IdleClipNames.Length);

        for (int i = 0; i < IdleClipNames.Length; i++)
        {
            string path = IdleAnimationsFolder + "/" + IdleClipNames[i] + ".anim";
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        idleClips = clips.Count > 0 ? clips.ToArray() : null;
        idleRandom = new System.Random();
        currentIdleClipIndex = 0;
        idleClipTime = 0f;
        nextIdleSwapTime = idleClips != null && idleClips.Length > 0 ? idleClips[0].length : 3f;
    }

    private void PickNextIdleClip()
    {
        if (idleClips == null || idleClips.Length == 0)
        {
            return;
        }

        int nextIndex;
        if (idleClips.Length == 1)
        {
            nextIndex = 0;
        }
        else
        {
            do
            {
                nextIndex = idleRandom.Next(0, idleClips.Length);
            }
            while (nextIndex == currentIdleClipIndex);
        }

        currentIdleClipIndex = nextIndex;
        idleClipTime = 0f;
        nextIdleSwapTime = idleClips[currentIdleClipIndex].length;
    }

    private void CollectPreviewBodyParts()
    {
        if (previewInstance == null)
        {
            previewBodyParts = null;
            return;
        }

        Renderer[] renderers = previewInstance.GetComponentsInChildren<Renderer>(true);
        previewBodyParts = new List<PreviewBodyPart>(renderers.Length);

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer rend = renderers[i];
            string partName = rend.gameObject.name;

            Material correctMaterial = AssetDatabase.LoadAssetAtPath<Material>(AvatarMaterialsFolder + "/" + partName + ".mat");
            if (correctMaterial == null || !correctMaterial.HasProperty(ShaderBodyBlueChannelColor))
            {
                continue;
            }

            rend.sharedMaterial = correctMaterial;

            bool isPupil = partName.Equals("pupil", StringComparison.OrdinalIgnoreCase);

            previewBodyParts.Add(new PreviewBodyPart(rend, isPupil));
        }
    }

    private void ApplyColorToPreview(DefinitionEntry entry)
    {
        if (previewBodyParts == null || entry == null)
        {
            return;
        }

        Color bodyColor = entry.HasValidColor ? entry.PreviewColor : Color.blue;
        Color beakColor = new Color(1f, 0.85f, 0f, 1f);
        Color bellyColor = Color.white;

        for (int i = 0; i < previewBodyParts.Count; i++)
        {
            PreviewBodyPart part = previewBodyParts[i];
            if (part.Renderer == null)
            {
                continue;
            }

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            part.Renderer.GetPropertyBlock(block);

            if (part.IsPupil)
            {
                block.SetColor(ShaderBodyRedChannelColor, Color.black);
                block.SetColor(ShaderBodyGreenChannelColor, Color.black);
                block.SetColor(ShaderBodyBlueChannelColor, Color.black);
            }
            else
            {
                block.SetColor(ShaderBodyRedChannelColor, beakColor);
                block.SetColor(ShaderBodyGreenChannelColor, bellyColor);
                block.SetColor(ShaderBodyBlueChannelColor, bodyColor);
            }

            part.Renderer.SetPropertyBlock(block);
        }

        previewAppliedGuid = entry.Guid;
        previewAppliedColor = entry.PreviewColor;
    }

    private void HandlePreviewInput(Rect previewRect)
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        Event evt = Event.current;
        if (evt.type == EventType.Repaint)
        {
            bool leftPressed = mouse.leftButton.isPressed;
            Vector2 screenPos = mouse.position.ReadValue();
            Vector2 guiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);

            if (leftPressed && !previewDragging && previewRect.Contains(evt.mousePosition))
            {
                previewDragging = true;
            }
            else if (!leftPressed && previewDragging)
            {
                previewDragging = false;
            }

            if (previewDragging)
            {
                Vector2 delta = mouse.delta.ReadValue();
                if (delta.x != 0f)
                {
                    previewRotationY -= delta.x * 0.5f;
                    if (previewInstance != null)
                    {
                        previewInstance.transform.rotation = Quaternion.Euler(0f, previewRotationY, 0f);
                    }
                }
            }
        }
    }

    private void RenderPreview(Rect previewRect)
    {
        previewRenderUtility.BeginPreview(previewRect, GUIStyle.none);
        previewRenderUtility.camera.Render();
        Texture resultTexture = previewRenderUtility.EndPreview();
        GUI.DrawTexture(previewRect, resultTexture, ScaleMode.StretchToFill, false);
    }

    private void CleanupPreview()
    {
        previewBodyParts = null;
        previewInstance = null;
        previewAnimator = null;
        previewAppliedGuid = null;

        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }
    }

    private void DrawUsagePanel()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField("Cached Usages", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                includePrefabs = EditorGUILayout.ToggleLeft("Scan Prefabs", includePrefabs);
                includeAssets = EditorGUILayout.ToggleLeft("Scan Definitions", includeAssets);
                includeScenes = EditorGUILayout.ToggleLeft("Scan Scenes", includeScenes);
                EditorGUILayout.HelpBox("Use the single Scan Usages button in the toolbar.", MessageType.Info);
                EditorGUILayout.HelpBox("Drag a definition in the left list to reorder. The list order becomes the new ID order.", MessageType.Info);
            }

            usageScroll = EditorGUILayout.BeginScrollView(usageScroll);

            DefinitionEntry entry = SelectedEntry;
            if (entry == null)
            {
                EditorGUILayout.HelpBox("Select a definition to view cached usages.", MessageType.Info);
            }
            else if (!hasScannedUsageCache)
            {
                EditorGUILayout.HelpBox("No usage scan has been run yet. Use the Scan Usages button in the toolbar.", MessageType.Info);
            }
            else
            {
                List<UsageHit> hits = GetUsages(entry.Guid);
                if (hits.Count == 0)
                {
                    EditorGUILayout.HelpBox("No usages found for the selected definition.", MessageType.Info);
                }
                else
                {
                    for (int i = 0; i < hits.Count; i++)
                    {
                        DrawUsageRow(hits[i]);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawUsageRow(UsageHit hit)
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label(hit.Category, GUILayout.Width(90f));
            GUILayout.Label(hit.StructuredReference ? "Structured" : "Text", GUILayout.Width(75f));
            EditorGUILayout.SelectableLabel(hit.Path, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            if (GUILayout.Button("Ping", GUILayout.Width(50f)))
            {
                PingAssetAtPath(hit.Path);
            }

            if (GUILayout.Button("Open", GUILayout.Width(50f)))
            {
                OpenAssetAtPath(hit.Path);
            }
        }
    }

    private DefinitionEntry SelectedEntry
    {
        get
        {
            if (selectedIndex < 0 || selectedIndex >= definitions.Count)
            {
                return null;
            }

            return definitions[selectedIndex];
        }
    }

    private void SelectIndex(int index)
    {
        selectedIndex = index;
        if (definitionsList != null)
        {
            definitionsList.index = index;
        }

        Repaint();
    }

    private void LoadDefinitionsOnly(bool preserveSelection = true)
    {
        string selectedGuid = preserveSelection && SelectedEntry != null ? SelectedEntry.Guid : null;
        LoadDefinitionsInternal(preserveSelection, selectedGuid, true);
    }

    private void ReloadDefinitionsKeepUsageCache(string selectedGuid)
    {
        LoadDefinitionsInternal(true, selectedGuid, false);
    }

    private void LoadDefinitionsInternal(bool preserveSelection, string selectedGuid, bool clearUsageCache)
    {
        try
        {
            definitions.Clear();
            selectedIndex = -1;

            if (clearUsageCache)
            {
                usageMap.Clear();
                hasScannedUsageCache = false;
            }

            string[] guids = AssetDatabase.FindAssets("t:AvatarColorDefinition", new[] { DefinitionsFolder });
            int total = guids.Length;

            for (int i = 0; i < total; i++)
            {
                float progress = total > 0 ? (float)i / total : 1f;
                EditorUtility.DisplayProgressBar("Avatar Color Manager", "Loading definitions...", progress);

                string guid = guids[i];
                string path = NormalizeAssetPath(AssetDatabase.GUIDToAssetPath(guid));

                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(path);
                if (!fileName.StartsWith("AvatarColor_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                AvatarColorDefinition definition = AssetDatabase.LoadAssetAtPath<AvatarColorDefinition>(path);
                if (definition == null)
                {
                    continue;
                }

                definitions.Add(new DefinitionEntry(definition, path, guid));
            }

            SortDefinitionsInternal();
            RebuildDefinitionsList();

            if (clearUsageCache)
            {
                TryRestoreUsageCache();
            }

            if (preserveSelection && !string.IsNullOrEmpty(selectedGuid))
            {
                ReselectByGuid(selectedGuid);
            }
            else if (definitions.Count > 0)
            {
                SelectIndex(0);
            }

            if (!hasScannedUsageCache)
            {
                statusMessage = "Loaded " + definitions.Count + " definitions. Usage scan not run yet.";
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Repaint();
    }

    private void RebuildDefinitionsList()
    {
        definitionsList = new ReorderableList(definitions, typeof(DefinitionEntry), true, false, false, false);
        definitionsList.elementHeight = 42f;
        definitionsList.drawElementCallback = DrawDefinitionElement;
        definitionsList.onSelectCallback = OnDefinitionsListSelected;
        definitionsList.onReorderCallbackWithDetails = OnDefinitionsListReordered;

        if (selectedIndex >= 0 && selectedIndex < definitions.Count)
        {
            definitionsList.index = selectedIndex;
        }
    }

    private void ScanUsages()
    {
        if (definitions.Count == 0)
        {
            usageMap.Clear();
            hasScannedUsageCache = true;
            PersistUsageCache();
            statusMessage = "No definitions to scan.";
            Repaint();
            return;
        }

        try
        {
            usageMap.Clear();

            List<SearchFile> files = new List<SearchFile>(EnumerateSearchFiles());
            int totalFiles = files.Count;

            for (int fileIndex = 0; fileIndex < totalFiles; fileIndex++)
            {
                float progress = totalFiles > 0 ? (float)fileIndex / totalFiles : 1f;
                EditorUtility.DisplayProgressBar("Avatar Color Manager", "Scanning usages...", progress);

                SearchFile file = files[fileIndex];

                string text;
                try
                {
                    text = File.ReadAllText(file.FullPath);
                }
                catch
                {
                    continue;
                }

                for (int i = 0; i < definitions.Count; i++)
                {
                    DefinitionEntry entry = definitions[i];
                    if (text.IndexOf(entry.Guid, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    bool structuredReference = ContainsStructuredGuidReference(text, entry.Guid);
                    string category = GetUsageCategory(file.RelativePath, text);

                    List<UsageHit> hits;
                    if (!usageMap.TryGetValue(entry.Guid, out hits))
                    {
                        hits = new List<UsageHit>();
                        usageMap.Add(entry.Guid, hits);
                    }

                    hits.Add(new UsageHit(file.RelativePath, category, structuredReference));
                }
            }

            foreach (KeyValuePair<string, List<UsageHit>> pair in usageMap)
            {
                pair.Value.Sort((a, b) =>
                {
                    int categoryCompare = string.Compare(a.Category, b.Category, StringComparison.OrdinalIgnoreCase);
                    if (categoryCompare != 0)
                    {
                        return categoryCompare;
                    }

                    return string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
                });
            }

            hasScannedUsageCache = true;
            PersistUsageCache();
            statusMessage = "Usage scan complete.";
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Repaint();
    }

    private void SaveAllDefinitions()
    {
        string selectedGuid = SelectedEntry != null ? SelectedEntry.Guid : null;
        bool preserveUsageCache = hasScannedUsageCache;
        List<string> renameErrors = new List<string>();
        Dictionary<string, string> renamedPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            int total = definitions.Count;

            for (int i = 0; i < total; i++)
            {
                float progress = total > 0 ? (float)i / total : 1f;
                EditorUtility.DisplayProgressBar("Avatar Color Manager", "Saving definitions...", progress);

                AvatarColorDefinition definition = definitions[i].Definition;
                if (definition == null)
                {
                    continue;
                }

                SyncDefinitionIdentityFields(definition);
                EditorUtility.SetDirty(definition);
            }

            RenameAllDefinitionFilesToMatchIds(renameErrors, renamedPaths);

            if (preserveUsageCache)
            {
                UpdateUsageCachePaths(renamedPaths);
                PersistUsageCache();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        if (preserveUsageCache)
        {
            ReloadDefinitionsKeepUsageCache(selectedGuid);
        }
        else
        {
            LoadDefinitionsOnly(false);
            if (!string.IsNullOrEmpty(selectedGuid))
            {
                ReselectByGuid(selectedGuid);
            }
        }

        if (renameErrors.Count == 0)
        {
            statusMessage = preserveUsageCache
                ? "Saved all AvatarColor definitions. Cached usages preserved."
                : "Saved all AvatarColor definitions. Usage scan not run yet.";
        }
        else
        {
            statusMessage = preserveUsageCache
                ? "Saved all AvatarColor definitions. Cached usages preserved. Rename issues: " + string.Join(" | ", renameErrors)
                : "Saved all AvatarColor definitions. Rename issues: " + string.Join(" | ", renameErrors);
        }
    }

    private void RenameAllDefinitionFilesToMatchIds(List<string> renameErrors, Dictionary<string, string> renamedPaths)
    {
        List<RenameOperation> operations = new List<RenameOperation>();
        HashSet<string> desiredFullPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            if (entry == null || entry.Definition == null)
            {
                continue;
            }

            string currentAssetPath = NormalizeAssetPath(AssetDatabase.GetAssetPath(entry.Definition));
            if (string.IsNullOrEmpty(currentAssetPath))
            {
                renameErrors.Add("Could not find current asset path for definition GUID " + entry.Guid);
                continue;
            }

            string desiredAssetPath = DefinitionsFolder + "/" + GetDesiredDefinitionAssetName(entry.Definition.ColorId) + ".asset";
            string currentFullPath = AssetPathToFullPath(currentAssetPath);
            string desiredFullPath = AssetPathToFullPath(desiredAssetPath);

            if (!desiredFullPaths.Add(desiredFullPath))
            {
                renameErrors.Add("Duplicate desired definition path: " + desiredAssetPath);
            }

            if (string.Equals(currentAssetPath, desiredAssetPath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            operations.Add(new RenameOperation(entry.Guid, currentAssetPath, desiredAssetPath, currentFullPath, desiredFullPath));
        }

        if (renameErrors.Count > 0 || operations.Count == 0)
        {
            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.ReleaseCachedFileHandles();

        Dictionary<string, RenameFileContents> contentsByCurrentFullPath = new Dictionary<string, RenameFileContents>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < operations.Count; i++)
        {
            float progress = operations.Count > 0 ? (float)i / operations.Count : 1f;
            EditorUtility.DisplayProgressBar("Avatar Color Manager", "Reading definition files...", progress);

            RenameOperation op = operations[i];

            if (!File.Exists(op.CurrentFullPath))
            {
                renameErrors.Add("Definition file does not exist: " + op.CurrentAssetPath);
                continue;
            }

            string currentMetaFullPath = op.CurrentFullPath + ".meta";
            if (!File.Exists(currentMetaFullPath))
            {
                renameErrors.Add("Definition meta file does not exist: " + op.CurrentAssetPath + ".meta");
                continue;
            }

            contentsByCurrentFullPath[op.CurrentFullPath] = new RenameFileContents(
                File.ReadAllBytes(op.CurrentFullPath),
                File.ReadAllBytes(currentMetaFullPath));
        }

        if (renameErrors.Count > 0)
        {
            return;
        }

        for (int i = 0; i < operations.Count; i++)
        {
            float progress = operations.Count > 0 ? (float)i / operations.Count : 1f;
            EditorUtility.DisplayProgressBar("Avatar Color Manager", "Writing definition files...", progress);

            RenameOperation op = operations[i];
            RenameFileContents contents = contentsByCurrentFullPath[op.CurrentFullPath];

            string desiredDirectory = Path.GetDirectoryName(op.DesiredFullPath);
            if (!string.IsNullOrEmpty(desiredDirectory))
            {
                Directory.CreateDirectory(desiredDirectory);
            }

            File.WriteAllBytes(op.DesiredFullPath, contents.AssetBytes);
            File.WriteAllBytes(op.DesiredFullPath + ".meta", contents.MetaBytes);

            renamedPaths[NormalizeAssetPath(op.CurrentAssetPath)] = NormalizeAssetPath(op.DesiredAssetPath);
        }

        HashSet<string> desiredPathsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < operations.Count; i++)
        {
            desiredPathsSet.Add(operations[i].DesiredFullPath);
        }

        for (int i = 0; i < operations.Count; i++)
        {
            float progress = operations.Count > 0 ? (float)i / operations.Count : 1f;
            EditorUtility.DisplayProgressBar("Avatar Color Manager", "Cleaning old definition files...", progress);

            RenameOperation op = operations[i];

            if (!desiredPathsSet.Contains(op.CurrentFullPath))
            {
                SafeDeleteFile(op.CurrentFullPath);
                SafeDeleteFile(op.CurrentFullPath + ".meta");
            }
        }
    }

    private void UpdateUsageCachePaths(Dictionary<string, string> renamedPaths)
    {
        if (!hasScannedUsageCache || renamedPaths == null || renamedPaths.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, List<UsageHit>> pair in usageMap)
        {
            List<UsageHit> hits = pair.Value;
            for (int i = 0; i < hits.Count; i++)
            {
                string normalizedPath = NormalizeAssetPath(hits[i].Path);
                string renamedPath;
                if (renamedPaths.TryGetValue(normalizedPath, out renamedPath))
                {
                    hits[i].Path = renamedPath;
                }
            }
        }
    }

    private static void SafeDeleteFile(string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        catch
        {
        }
    }

    private void RenumberDefinitionsFromCurrentListOrder()
    {
        if (definitions.Count == 0)
        {
            return;
        }

        List<int> sortedIds = new List<int>(definitions.Count);
        for (int i = 0; i < definitions.Count; i++)
        {
            sortedIds.Add(definitions[i].Definition.ColorId);
        }

        sortedIds.Sort();

        UnityEngine.Object[] undoObjects = new UnityEngine.Object[definitions.Count];
        for (int i = 0; i < definitions.Count; i++)
        {
            undoObjects[i] = definitions[i].Definition;
        }

        Undo.RecordObjects(undoObjects, "Reorder Avatar Color Definitions");

        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            entry.Definition.ColorId = sortedIds[i];
            SyncDefinitionIdentityFields(entry.Definition);
            entry.RefreshPreview();
        }
    }

    private void SyncDefinitionIdentityFields(AvatarColorDefinition definition)
    {
        if (definition == null)
        {
            return;
        }

        definition.ViewOrder = definition.ColorId;
        definition.name = GetDesiredDefinitionAssetName(definition.ColorId);
        EditorUtility.SetDirty(definition);
    }

    private static string GetDesiredDefinitionAssetName(int colorId)
    {
        return "AvatarColor_" + colorId;
    }

    private void ApplyColorIdChange(DefinitionEntry entry, int requestedColorId)
    {
        if (entry == null || entry.Definition == null)
        {
            return;
        }

        if (requestedColorId == entry.Definition.ColorId)
        {
            return;
        }

        DefinitionEntry other = FindEntryByColorId(requestedColorId, entry);
        string selectedGuid = entry.Guid;
        int oldId = entry.Definition.ColorId;

        if (other != null)
        {
            SwapDefinitionIds(entry, other, "Swap Avatar Color Definition IDs");
            statusMessage = "Swapped definition IDs " + oldId + " and " + requestedColorId + ".";
        }
        else
        {
            Undo.RecordObject(entry.Definition, "Edit Avatar Color Definition ID");
            entry.Definition.ColorId = requestedColorId;
            SyncDefinitionIdentityFields(entry.Definition);
            statusMessage = "Set definition ID from " + oldId + " to " + requestedColorId + ".";
        }

        SortDefinitionsPreserveSelection(selectedGuid);
    }

    private DefinitionEntry FindEntryByColorId(int colorId, DefinitionEntry ignoredEntry)
    {
        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            if (entry == ignoredEntry)
            {
                continue;
            }

            if (entry.Definition != null && entry.Definition.ColorId == colorId)
            {
                return entry;
            }
        }

        return null;
    }

    private void SwapDefinitionIds(DefinitionEntry first, DefinitionEntry second, string undoLabel)
    {
        if (first == null || second == null || first.Definition == null || second.Definition == null)
        {
            return;
        }

        string selectedGuid = first.Guid;

        Undo.RecordObjects(new UnityEngine.Object[] { first.Definition, second.Definition }, undoLabel);

        int firstId = first.Definition.ColorId;
        first.Definition.ColorId = second.Definition.ColorId;
        second.Definition.ColorId = firstId;

        SyncDefinitionIdentityFields(first.Definition);
        SyncDefinitionIdentityFields(second.Definition);

        SortDefinitionsPreserveSelection(selectedGuid);
    }

    private void CreateNewDefinition()
    {
        if (string.IsNullOrWhiteSpace(newDefinitionColorString))
        {
            statusMessage = "New Color String is empty.";
            return;
        }

        Color parsedColor;
        if (!TryParseColorString(newDefinitionColorString, out parsedColor))
        {
            statusMessage = "New Color String is invalid.";
            return;
        }

        for (int i = 0; i < definitions.Count; i++)
        {
            if (definitions[i].Definition.ColorId == newDefinitionId)
            {
                statusMessage = "A definition with ID " + newDefinitionId + " already exists.";
                return;
            }
        }

        string assetPath = DefinitionsFolder + "/" + GetDesiredDefinitionAssetName(newDefinitionId) + ".asset";
        string fullPath = AssetPathToFullPath(assetPath);

        if (File.Exists(fullPath))
        {
            statusMessage = "That asset file already exists: " + assetPath;
            return;
        }

        AvatarColorDefinition asset = CreateInstance<AvatarColorDefinition>();
        asset.ColorId = newDefinitionId;
        asset.ColorName = newDefinitionColorName ?? string.Empty;
        asset.Color = newDefinitionColorString;
        SyncDefinitionIdentityFields(asset);

        AssetDatabase.CreateAsset(asset, assetPath);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string addError;
        if (!TryAddDefinitionToGeneratedAvatarColors(asset, out addError))
        {
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            statusMessage = "Created definition was rolled back because AvatarColors.asset could not be updated: " + addError;
            LoadDefinitionsOnly(false);
            return;
        }

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        bool preserveUsageCache = hasScannedUsageCache;

        if (preserveUsageCache)
        {
            usageMap[guid] = new List<UsageHit>();
            PersistUsageCache();
            ReloadDefinitionsKeepUsageCache(guid);
        }
        else
        {
            LoadDefinitionsOnly(false);
            ReselectByGuid(guid);
        }

        showCreateDefinitionPanel = false;
        statusMessage = preserveUsageCache
            ? "Created " + assetPath + ", added it to AvatarColors.asset, and preserved cached usages."
            : "Created " + assetPath + " and added it to AvatarColors.asset. Usage scan not run yet.";

        newDefinitionId++;
        newDefinitionColorName = string.Empty;
        newDefinitionColorString = "#FFFFFF";
    }

    private void DeleteSelectedDefinitionAndClearReferences()
    {
        DefinitionEntry entry = SelectedEntry;
        if (entry == null)
        {
            statusMessage = "Nothing selected.";
            return;
        }

        string deletedGuid = entry.Guid;
        int deletedColorId = entry.Definition != null ? entry.Definition.ColorId : 0;
        bool preserveUsageCache = hasScannedUsageCache;
        string selectionAfterDeleteGuid = FindSelectionGuidAfterDelete(deletedGuid, deletedColorId);
        string usageCountText = hasScannedUsageCache ? GetUsageCount(entry.Guid).ToString() : "?";

        string dialogMessage =
            "Delete " + Path.GetFileName(entry.Path) + " and clear structured references?\n\n" +
            "Cached usages: " + usageCountText;

        if (!EditorUtility.DisplayDialog("Delete AvatarColor Definition", dialogMessage, "Delete", "Cancel"))
        {
            return;
        }

        string removeError;
        if (!TryRemoveDefinitionFromGeneratedAvatarColors(entry.Definition, out removeError))
        {
            statusMessage = "Could not remove definition from AvatarColors.asset: " + removeError;
            return;
        }

        int changedFiles;
        try
        {
            changedFiles = ClearStructuredGuidReferences(entry.Guid);
            EditorUtility.DisplayProgressBar("Avatar Color Manager", "Deleting definition...", 1f);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        bool deleted = AssetDatabase.DeleteAsset(entry.Path);

        if (deleted)
        {
            definitions.RemoveAll(d => string.Equals(d.Guid, deletedGuid, StringComparison.OrdinalIgnoreCase));
            CollapseRemainingDefinitionIdsAfterDelete(deletedColorId);

            List<string> renameErrors = new List<string>();
            Dictionary<string, string> renamedPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                RenameAllDefinitionFilesToMatchIds(renameErrors, renamedPaths);

                if (preserveUsageCache)
                {
                    usageMap.Remove(deletedGuid);
                    UpdateUsageCachePaths(renamedPaths);
                    PersistUsageCache();
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (preserveUsageCache)
            {
                ReloadDefinitionsKeepUsageCache(selectionAfterDeleteGuid);
            }
            else
            {
                LoadDefinitionsOnly(false);
                if (!string.IsNullOrEmpty(selectionAfterDeleteGuid))
                {
                    ReselectByGuid(selectionAfterDeleteGuid);
                }
            }

            if (renameErrors.Count == 0)
            {
                statusMessage = preserveUsageCache
                    ? "Deleted definition, compacted IDs, renamed files, cleared references in " + changedFiles + " file(s), and preserved cached usages."
                    : "Deleted definition, compacted IDs, renamed files, and cleared references in " + changedFiles + " file(s).";
            }
            else
            {
                statusMessage = preserveUsageCache
                    ? "Deleted definition, compacted IDs, cleared references in " + changedFiles + " file(s), preserved cached usages. Rename issues: " + string.Join(" | ", renameErrors)
                    : "Deleted definition, compacted IDs, cleared references in " + changedFiles + " file(s). Rename issues: " + string.Join(" | ", renameErrors);
            }
        }
        else
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (preserveUsageCache)
            {
                ReloadDefinitionsKeepUsageCache(deletedGuid);
            }
            else
            {
                LoadDefinitionsOnly(false);
                ReselectByGuid(deletedGuid);
            }

            statusMessage = preserveUsageCache
                ? "Failed to delete definition. It was removed from AvatarColors.asset, references were cleared in " + changedFiles + " file(s), and cached usages were preserved."
                : "Failed to delete definition. It was removed from AvatarColors.asset and references were cleared in " + changedFiles + " file(s).";
        }
    }

    private void CollapseRemainingDefinitionIdsAfterDelete(int deletedColorId)
    {
        List<UnityEngine.Object> changedObjects = new List<UnityEngine.Object>();

        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            if (entry == null || entry.Definition == null)
            {
                continue;
            }

            if (entry.Definition.ColorId > deletedColorId)
            {
                changedObjects.Add(entry.Definition);
            }
        }

        if (changedObjects.Count > 0)
        {
            Undo.RecordObjects(changedObjects.ToArray(), "Collapse Avatar Color Definition IDs");
        }

        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            if (entry == null || entry.Definition == null)
            {
                continue;
            }

            if (entry.Definition.ColorId > deletedColorId)
            {
                entry.Definition.ColorId -= 1;
                SyncDefinitionIdentityFields(entry.Definition);
            }
        }
    }

    private string FindSelectionGuidAfterDelete(string deletedGuid, int deletedColorId)
    {
        DefinitionEntry nextHigher = null;
        DefinitionEntry previousLower = null;

        for (int i = 0; i < definitions.Count; i++)
        {
            DefinitionEntry entry = definitions[i];
            if (entry == null || string.Equals(entry.Guid, deletedGuid, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            int id = entry.Definition != null ? entry.Definition.ColorId : int.MinValue;

            if (id > deletedColorId)
            {
                if (nextHigher == null || id < nextHigher.Definition.ColorId)
                {
                    nextHigher = entry;
                }
            }
            else if (id < deletedColorId)
            {
                if (previousLower == null || id > previousLower.Definition.ColorId)
                {
                    previousLower = entry;
                }
            }
        }

        if (nextHigher != null)
        {
            return nextHigher.Guid;
        }

        if (previousLower != null)
        {
            return previousLower.Guid;
        }

        return null;
    }

    private void PersistUsageCache()
    {
        EditorUsageCacheState state = new EditorUsageCacheState();
        state.HasScannedUsageCache = hasScannedUsageCache;
        state.IncludePrefabs = includePrefabs;
        state.IncludeAssets = includeAssets;
        state.IncludeScenes = includeScenes;
        state.Entries = new List<EditorUsageCacheEntry>();

        foreach (KeyValuePair<string, List<UsageHit>> pair in usageMap)
        {
            EditorUsageCacheEntry entry = new EditorUsageCacheEntry();
            entry.Guid = pair.Key;
            entry.Hits = new List<EditorUsageCacheHit>();

            List<UsageHit> hits = pair.Value;
            for (int i = 0; i < hits.Count; i++)
            {
                UsageHit hit = hits[i];
                EditorUsageCacheHit savedHit = new EditorUsageCacheHit();
                savedHit.Path = hit.Path;
                savedHit.Category = hit.Category;
                savedHit.StructuredReference = hit.StructuredReference;
                entry.Hits.Add(savedHit);
            }

            state.Entries.Add(entry);
        }

        EditorPrefs.SetString(GetUsageCacheKey(), JsonUtility.ToJson(state));
    }

    private void TryRestoreUsageCache()
    {
        string json = EditorPrefs.GetString(GetUsageCacheKey(), string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        EditorUsageCacheState state = JsonUtility.FromJson<EditorUsageCacheState>(json);
        if (state == null)
        {
            return;
        }

        if (state.IncludePrefabs != includePrefabs || state.IncludeAssets != includeAssets || state.IncludeScenes != includeScenes)
        {
            return;
        }

        usageMap.Clear();

        if (state.Entries != null)
        {
            for (int i = 0; i < state.Entries.Count; i++)
            {
                EditorUsageCacheEntry savedEntry = state.Entries[i];
                if (savedEntry == null || string.IsNullOrEmpty(savedEntry.Guid))
                {
                    continue;
                }

                List<UsageHit> restoredHits = new List<UsageHit>();
                if (savedEntry.Hits != null)
                {
                    for (int j = 0; j < savedEntry.Hits.Count; j++)
                    {
                        EditorUsageCacheHit savedHit = savedEntry.Hits[j];
                        if (savedHit == null)
                        {
                            continue;
                        }

                        restoredHits.Add(new UsageHit(savedHit.Path, savedHit.Category, savedHit.StructuredReference));
                    }
                }

                usageMap[savedEntry.Guid] = restoredHits;
            }
        }

        hasScannedUsageCache = state.HasScannedUsageCache;
    }

    private string GetUsageCacheKey()
    {
        return "AvatarColorDefinitionToolWindow.UsageCache." + GetProjectRootFullPath();
    }

    private bool TryAddDefinitionToGeneratedAvatarColors(AvatarColorDefinition definition, out string error)
    {
        error = string.Empty;

        UnityEngine.Object container = AssetDatabase.LoadMainAssetAtPath(GeneratedAvatarColorsAssetPath);
        if (container == null)
        {
            error = "Could not load " + GeneratedAvatarColorsAssetPath;
            return false;
        }

        SerializedObject serializedObject = new SerializedObject(container);
        SerializedProperty arrayProperty;
        if (!TryFindAvatarColorArrayProperty(serializedObject, out arrayProperty, out error))
        {
            return false;
        }

        for (int i = 0; i < arrayProperty.arraySize; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue == definition)
            {
                return true;
            }
        }

        int oldSize = arrayProperty.arraySize;
        arrayProperty.arraySize = oldSize + 1;
        SerializedProperty newElement = arrayProperty.GetArrayElementAtIndex(oldSize);

        if (newElement.propertyType != SerializedPropertyType.ObjectReference)
        {
            error = "Found a candidate array, but its elements are not object references.";
            return false;
        }

        newElement.objectReferenceValue = definition;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return true;
    }

    private bool TryRemoveDefinitionFromGeneratedAvatarColors(AvatarColorDefinition definition, out string error)
    {
        error = string.Empty;

        UnityEngine.Object container = AssetDatabase.LoadMainAssetAtPath(GeneratedAvatarColorsAssetPath);
        if (container == null)
        {
            error = "Could not load " + GeneratedAvatarColorsAssetPath;
            return false;
        }

        SerializedObject serializedObject = new SerializedObject(container);
        SerializedProperty arrayProperty;
        if (!TryFindAvatarColorArrayProperty(serializedObject, out arrayProperty, out error))
        {
            return false;
        }

        bool removedAny = false;

        for (int i = arrayProperty.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            if (element.propertyType != SerializedPropertyType.ObjectReference)
            {
                continue;
            }

            if (element.objectReferenceValue == definition)
            {
                arrayProperty.DeleteArrayElementAtIndex(i);
                if (i < arrayProperty.arraySize)
                {
                    SerializedProperty checkElement = arrayProperty.GetArrayElementAtIndex(i);
                    if (checkElement.propertyType == SerializedPropertyType.ObjectReference && checkElement.objectReferenceValue == null)
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                removedAny = true;
            }
        }

        if (removedAny)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(container);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return true;
    }

    private bool IsInGeneratedAvatarColors(AvatarColorDefinition definition)
    {
        if (definition == null)
        {
            return false;
        }

        UnityEngine.Object container = AssetDatabase.LoadMainAssetAtPath(GeneratedAvatarColorsAssetPath);
        if (container == null)
        {
            return false;
        }

        SerializedObject serializedObject = new SerializedObject(container);
        SerializedProperty arrayProperty;
        string error;
        if (!TryFindAvatarColorArrayProperty(serializedObject, out arrayProperty, out error))
        {
            return false;
        }

        for (int i = 0; i < arrayProperty.arraySize; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue == definition)
            {
                return true;
            }
        }

        return false;
    }

    private bool TryFindAvatarColorArrayProperty(SerializedObject serializedObject, out SerializedProperty bestProperty, out string error)
    {
        bestProperty = null;
        error = string.Empty;

        SerializedProperty iterator = serializedObject.GetIterator();
        int bestScore = int.MinValue;
        string bestPath = null;
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            int score;
            if (!IsAvatarColorReferenceArray(iterator, out score))
            {
                continue;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestPath = iterator.propertyPath;
            }
        }

        if (string.IsNullOrEmpty(bestPath))
        {
            error = "Could not find an AvatarColorDefinition reference array in " + GeneratedAvatarColorsAssetPath;
            return false;
        }

        bestProperty = serializedObject.FindProperty(bestPath);
        if (bestProperty == null)
        {
            error = "Found a candidate array path, but could not reopen it: " + bestPath;
            return false;
        }

        return true;
    }

    private bool IsAvatarColorReferenceArray(SerializedProperty arrayProperty, out int score)
    {
        score = int.MinValue;

        if (!arrayProperty.isArray)
        {
            return false;
        }

        if (arrayProperty.propertyType == SerializedPropertyType.String)
        {
            return false;
        }

        string path = arrayProperty.propertyPath.ToLowerInvariant();
        int nameBonus = 0;

        if (path.Contains("avatarcolors"))
        {
            nameBonus = 100;
        }
        else if (path.Contains("avatarcolor"))
        {
            nameBonus = 80;
        }
        else if (path.Contains("colors"))
        {
            nameBonus = 20;
        }

        if (arrayProperty.arraySize == 0)
        {
            if (nameBonus > 0)
            {
                score = 100 + nameBonus;
                return true;
            }

            return false;
        }

        int avatarColorRefs = 0;
        int nullRefs = 0;

        for (int i = 0; i < arrayProperty.arraySize; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            if (element.propertyType != SerializedPropertyType.ObjectReference)
            {
                return false;
            }

            UnityEngine.Object elementObject = element.objectReferenceValue;
            if (elementObject == null)
            {
                nullRefs++;
                continue;
            }

            if (elementObject is AvatarColorDefinition)
            {
                avatarColorRefs++;
                continue;
            }

            return false;
        }

        if (avatarColorRefs > 0)
        {
            score = 1000 + avatarColorRefs + nameBonus;
            return true;
        }

        if (nullRefs == arrayProperty.arraySize && nameBonus > 0)
        {
            score = 100 + nameBonus;
            return true;
        }

        return false;
    }

    private int ClearStructuredGuidReferences(string sourceGuid)
    {
        AssetDatabase.ReleaseCachedFileHandles();

        int changedFiles = 0;
        List<SearchFile> files = new List<SearchFile>(EnumerateSearchFiles());
        int totalFiles = files.Count;

        for (int fileIndex = 0; fileIndex < totalFiles; fileIndex++)
        {
            float progress = totalFiles > 0 ? (float)fileIndex / totalFiles : 1f;
            EditorUtility.DisplayProgressBar("Avatar Color Manager", "Clearing references...", progress);

            SearchFile file = files[fileIndex];

            string text;
            try
            {
                text = File.ReadAllText(file.FullPath);
            }
            catch
            {
                continue;
            }

            int replacements;
            string updatedText = NullStructuredGuidReferences(text, sourceGuid, out replacements);
            if (replacements <= 0)
            {
                continue;
            }

            try
            {
                File.WriteAllText(file.FullPath, updatedText);
                changedFiles++;
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not write " + file.RelativePath + ": " + ex.Message);
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        return changedFiles;
    }

    private List<UsageHit> GetUsages(string guid)
    {
        List<UsageHit> hits;
        if (guid != null && usageMap.TryGetValue(guid, out hits))
        {
            return hits;
        }

        return new List<UsageHit>();
    }

    private int GetUsageCount(string guid)
    {
        List<UsageHit> hits;
        if (guid != null && usageMap.TryGetValue(guid, out hits))
        {
            return hits.Count;
        }

        return 0;
    }

    private IEnumerable<SearchFile> EnumerateSearchFiles()
    {
        string assetsRoot = Application.dataPath.Replace('\\', '/');

        foreach (string fullPathRaw in Directory.EnumerateFiles(assetsRoot, "*.*", SearchOption.AllDirectories))
        {
            string fullPath = fullPathRaw.Replace('\\', '/');
            string relativePath = "Assets" + fullPath.Substring(assetsRoot.Length);
            string extension = Path.GetExtension(relativePath).ToLowerInvariant();

            if (!ScanExtensions.Contains(extension))
            {
                continue;
            }

            if (!ShouldScanExtension(extension))
            {
                continue;
            }

            yield return new SearchFile(relativePath, fullPath);
        }
    }

    private bool ShouldScanExtension(string extension)
    {
        switch (extension)
        {
            case ".prefab":
                return includePrefabs;

            case ".asset":
                return includeAssets;

            case ".unity":
                return includeScenes;

            default:
                return false;
        }
    }

    private static string GetUsageCategory(string relativePath, string text)
    {
        string path = NormalizeAssetPath(relativePath);

        bool looksLikePlayMaker =
            path.IndexOf("PlayMaker", StringComparison.OrdinalIgnoreCase) >= 0 ||
            text.IndexOf("PlayMaker", StringComparison.OrdinalIgnoreCase) >= 0 ||
            text.IndexOf("PlayMakerFSM", StringComparison.OrdinalIgnoreCase) >= 0 ||
            text.IndexOf("HutongGames.PlayMaker", StringComparison.OrdinalIgnoreCase) >= 0;

        if (looksLikePlayMaker)
        {
            return "PlayMaker";
        }

        if (path.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
        {
            return "Scene";
        }

        if (path.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
        {
            return "Prefab";
        }

        if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
        {
            if (path.StartsWith(DefinitionsFolder + "/", StringComparison.OrdinalIgnoreCase))
            {
                return "Definition";
            }

            return "Asset";
        }

        return "Asset";
    }

    private static bool ContainsStructuredGuidReference(string text, string guid)
    {
        string pattern = @"\{fileID:\s*-?\d+\s*,\s*guid:\s*" + Regex.Escape(guid) + @"\s*,\s*type:\s*\d+\s*\}";
        return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

    private static string NullStructuredGuidReferences(string input, string sourceGuid, out int replacements)
    {
        int replacementCount = 0;

        string pattern = @"\{fileID:\s*-?\d+\s*,\s*guid:\s*" + Regex.Escape(sourceGuid) + @"\s*,\s*type:\s*\d+\s*\}";

        string result = Regex.Replace(
            input,
            pattern,
            match =>
            {
                replacementCount++;
                return "{fileID: 0}";
            },
            RegexOptions.IgnoreCase);

        replacements = replacementCount;
        return result;
    }

    private static bool TryParseColorString(string value, out Color color)
    {
        color = Color.white;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string trimmed = value.Trim();

        if (!trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            trimmed = "#" + trimmed;
        }

        return ColorUtility.TryParseHtmlString(trimmed, out color);
    }

    private static string ToHtmlColorString(Color color)
    {
        Color32 color32 = color;

        if (color32.a == 255)
        {
            return ColorUtility.ToHtmlStringRGB(color).ToUpperInvariant();
        }

        return ColorUtility.ToHtmlStringRGBA(color).ToUpperInvariant();
    }

    private static string NormalizeAssetPath(string path)
    {
        return string.IsNullOrEmpty(path) ? string.Empty : path.Replace('\\', '/');
    }

    private static string AssetPathToFullPath(string assetPath)
    {
        string projectRoot = GetProjectRootFullPath();
        return (projectRoot + "/" + assetPath).Replace('\\', '/');
    }

    private static string GetProjectRootFullPath()
    {
        return Directory.GetParent(Application.dataPath).FullName.Replace('\\', '/');
    }

    private void ReselectByGuid(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            return;
        }

        for (int i = 0; i < definitions.Count; i++)
        {
            if (string.Equals(definitions[i].Guid, guid, StringComparison.OrdinalIgnoreCase))
            {
                SelectIndex(i);
                return;
            }
        }
    }

    private void SortDefinitionsPreserveSelection(string guid)
    {
        SortDefinitionsInternal();
        RebuildDefinitionsList();
        ReselectByGuid(guid);
        Repaint();
    }

    private void SortDefinitionsInternal()
    {
        definitions.Sort((a, b) =>
        {
            int idCompare = a.Definition.ColorId.CompareTo(b.Definition.ColorId);
            if (idCompare != 0)
            {
                return idCompare;
            }

            return string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
        });
    }

    private static void PingAssetAtPath(string path)
    {
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        if (obj != null)
        {
            EditorGUIUtility.PingObject(obj);
        }
    }

    private static void OpenAssetAtPath(string path)
    {
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);
        if (obj != null)
        {
            AssetDatabase.OpenAsset(obj);
        }
    }

    [Serializable]
    private sealed class EditorUsageCacheState
    {
        public bool HasScannedUsageCache;
        public bool IncludePrefabs;
        public bool IncludeAssets;
        public bool IncludeScenes;
        public List<EditorUsageCacheEntry> Entries;
    }

    [Serializable]
    private sealed class EditorUsageCacheEntry
    {
        public string Guid;
        public List<EditorUsageCacheHit> Hits;
    }

    [Serializable]
    private sealed class EditorUsageCacheHit
    {
        public string Path;
        public string Category;
        public bool StructuredReference;
    }

    private sealed class DefinitionEntry
    {
        public readonly AvatarColorDefinition Definition;
        public readonly string Path;
        public readonly string Guid;

        public Color PreviewColor;
        public bool HasValidColor;

        public DefinitionEntry(AvatarColorDefinition definition, string path, string guid)
        {
            Definition = definition;
            Path = path;
            Guid = guid;
            RefreshPreview();
        }

        public void RefreshPreview()
        {
            HasValidColor = TryParseColorString(Definition != null ? Definition.Color : null, out PreviewColor);
            if (!HasValidColor)
            {
                PreviewColor = new Color(1f, 0f, 1f, 1f);
            }
        }
    }

    private sealed class UsageHit
    {
        public string Path;
        public readonly string Category;
        public readonly bool StructuredReference;

        public UsageHit(string path, string category, bool structuredReference)
        {
            Path = path;
            Category = category;
            StructuredReference = structuredReference;
        }
    }

    private readonly struct PreviewBodyPart
    {
        public readonly Renderer Renderer;
        public readonly bool IsPupil;

        public PreviewBodyPart(Renderer renderer, bool isPupil)
        {
            Renderer = renderer;
            IsPupil = isPupil;
        }
    }

    private readonly struct SearchFile
    {
        public readonly string RelativePath;
        public readonly string FullPath;

        public SearchFile(string relativePath, string fullPath)
        {
            RelativePath = relativePath;
            FullPath = fullPath;
        }
    }

    private sealed class RenameOperation
    {
        public readonly string Guid;
        public readonly string CurrentAssetPath;
        public readonly string DesiredAssetPath;
        public readonly string CurrentFullPath;
        public readonly string DesiredFullPath;

        public RenameOperation(string guid, string currentAssetPath, string desiredAssetPath, string currentFullPath, string desiredFullPath)
        {
            Guid = guid;
            CurrentAssetPath = currentAssetPath;
            DesiredAssetPath = desiredAssetPath;
            CurrentFullPath = currentFullPath;
            DesiredFullPath = desiredFullPath;
        }
    }

    private sealed class RenameFileContents
    {
        public readonly byte[] AssetBytes;
        public readonly byte[] MetaBytes;

        public RenameFileContents(byte[] assetBytes, byte[] metaBytes)
        {
            AssetBytes = assetBytes;
            MetaBytes = metaBytes;
        }
    }
}
