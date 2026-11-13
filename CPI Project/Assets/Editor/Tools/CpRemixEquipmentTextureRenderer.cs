using System;
using System.IO;
using System.Collections.Generic;
using ClubPenguin;
using ClubPenguin.Avatar;
using CpRemixShaders;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CpRemixEquipmentTextureRenderer : EditorWindow
{
    private const string MenuPath = "Project/Tools/CpRemix Equipment Texture Renderer";
    private const int DefaultOutputSize = 1024;
    private const float PreviewDisplaySize = 512f;
    private const float LivePreviewHeight = 320f;
    private const float LibraryScrollHeight = 440f;
    private const float LibraryItemWidth = 72f;
    private const float LibraryThumbnailSize = 56f;
    private const float PreviewOrbitSensitivity = 0.5f;
    private const float PreviewLayerMoveSensitivity = 0.35f;
    private const float PreviewLayerRotationSensitivity = 0.75f;
    private const float PreviewLayerScaleStep = 1f;
    private const float DesignerDefaultScale = 6.5f;
    private const float MinDecalScale = 0.1f;
    private const float MaxDecalScale = 30f;
    private const float MinUvOffset = -0.5f;
    private const float MaxUvOffset = 0.5f;
    private const float UvKeyPrecision = 1000000f;
    private const string DecalTextureFolder = "Assets/AssetPipeline/BundleAssets/AssetBundles/Decals";
    private const string MickeyEarsAssetPathSegment = "/MickeyEars/";
    private const float MickeyEarsBandHeightFraction = 0.61f;
    private const int MickeyEarsMaxRasterSize = 2048;
    private const int MickeyEarsSupersamplingScale = 8;
    private const string LastSaveDirectoryPreference = "CpRemixEquipmentTextureRenderer.LastSaveDirectory";

    private static readonly int[] OutputSizeValues = { 0, 128, 256, 512, 1024, 2048, 4096 };
    private static readonly string[] OutputSizeLabels = { "Auto", "128", "256", "512", "1024", "2048", "4096" };
    private static readonly string[] LibraryTabs = { "Fabrics", "Decals" };
    private static readonly string[] ChannelLabels = { "Red", "Green", "Blue" };
    private static readonly string[] FabricDefinitionFolders = { "Assets/Game/ItemCustomizer/Resources/Definitions/Equipment/FabricDefinitions" };
    private static readonly string[] DecalDefinitionFolders = { "Assets/Game/ItemCustomizer/Resources/Definitions/Equipment/DecalDefinitions" };
    private static readonly int[] FabricTexturePropertyIds = { EquipmentShaderParams.DECAL_RED_1_TEX, EquipmentShaderParams.DECAL_GREEN_2_TEX, EquipmentShaderParams.DECAL_BLUE_3_TEX };
    private static readonly int[] FabricScalePropertyIds = { EquipmentShaderParams.DECAL_RED_1_SCALE, EquipmentShaderParams.DECAL_GREEN_2_SCALE, EquipmentShaderParams.DECAL_BLUE_3_SCALE };
    private static readonly int[] FabricUOffsetPropertyIds = { EquipmentShaderParams.DECAL_RED_1_U_OFFSET, EquipmentShaderParams.DECAL_GREEN_2_U_OFFSET, EquipmentShaderParams.DECAL_BLUE_3_U_OFFSET };
    private static readonly int[] FabricVOffsetPropertyIds = { EquipmentShaderParams.DECAL_RED_1_V_OFFSET, EquipmentShaderParams.DECAL_GREEN_2_V_OFFSET, EquipmentShaderParams.DECAL_BLUE_3_V_OFFSET };
    private static readonly int[] FabricRepeatPropertyIds = { EquipmentShaderParams.DECAL_RED_1_REPEAT, EquipmentShaderParams.DECAL_GREEN_2_REPEAT, EquipmentShaderParams.DECAL_BLUE_3_REPEAT };
    private static readonly int[] FabricRotationPropertyIds = { EquipmentShaderParams.DECAL_RED_1_ROTATION_RADS, EquipmentShaderParams.DECAL_GREEN_2_ROTATION_RADS, EquipmentShaderParams.DECAL_BLUE_3_ROTATION_RADS };
    private static readonly int[] DecalTexturePropertyIds = { EquipmentShaderParams.DECAL_RED_4_TEX, EquipmentShaderParams.DECAL_GREEN_5_TEX, EquipmentShaderParams.DECAL_BLUE_6_TEX };
    private static readonly int[] DecalScalePropertyIds = { EquipmentShaderParams.DECAL_RED_4_SCALE, EquipmentShaderParams.DECAL_GREEN_5_SCALE, EquipmentShaderParams.DECAL_BLUE_6_SCALE };
    private static readonly int[] DecalUOffsetPropertyIds = { EquipmentShaderParams.DECAL_RED_4_U_OFFSET, EquipmentShaderParams.DECAL_GREEN_5_U_OFFSET, EquipmentShaderParams.DECAL_BLUE_6_U_OFFSET };
    private static readonly int[] DecalVOffsetPropertyIds = { EquipmentShaderParams.DECAL_RED_4_V_OFFSET, EquipmentShaderParams.DECAL_GREEN_5_V_OFFSET, EquipmentShaderParams.DECAL_BLUE_6_V_OFFSET };
    private static readonly int[] DecalRepeatPropertyIds = { EquipmentShaderParams.DECAL_RED_4_REPEAT, EquipmentShaderParams.DECAL_GREEN_5_REPEAT, EquipmentShaderParams.DECAL_BLUE_6_REPEAT };
    private static readonly int[] DecalRotationPropertyIds = { EquipmentShaderParams.DECAL_RED_4_ROTATION_RADS, EquipmentShaderParams.DECAL_GREEN_5_ROTATION_RADS, EquipmentShaderParams.DECAL_BLUE_6_ROTATION_RADS };
    private static readonly Color32[] Decal123ChannelColors =
    {
        new Color32(255, 0, 0, 255),
        new Color32(0, 255, 0, 255),
        new Color32(0, 0, 255, 255)
    };

    private Material sourceMaterial;
    private Mesh sourceMesh;
    private BaseViewDefinition sourceDefinition;
    private Texture2D previewTexture;
    private Texture livePreviewFrameTexture;
    private PreviewRenderUtility previewRenderUtility;
    private GameObject previewInstance;
    private MeshRenderer previewMeshRenderer;
    private MeshCollider previewMeshCollider;
    private Vector2 scrollPosition;
    private Vector2 previewPaneScrollPosition;
    private int outputSizeIndex = 2;
    private float previewRotationX = 12f;
    private float previewRotationY = 150f;
    private float previewDistance = 4f;
    private float previewBaseDistance = 4f;
    private bool previewDragging;
    private bool previewMoveLayerDragging;
    private bool previewRotateLayerDragging;
    private float lightIntensity = 1.5f;
    private float lightXRotation = 45f;
    private float lightYRotation = 160f;
    private Vector2 libraryScrollPosition;
    private int libraryTabIndex;
    private int libraryChannelIndex;
    private string librarySearch = string.Empty;
    private bool libraryLoaded;
    private DesignerLibraryEntry[] fabricLibraryEntries = new DesignerLibraryEntry[0];
    private DesignerLibraryEntry[] decalLibraryEntries = new DesignerLibraryEntry[0];
    private string lastAppliedLibraryAssetName;
    private bool previewMouseEditMode;

    private sealed class DesignerLibraryEntry
    {
        public string AssetName;
        public string DisplayName;
        public Texture2D Texture;
        public bool AllowRotationAndScale = true;
    }

    private struct UvPointKey : IEquatable<UvPointKey>
    {
        public readonly int X;
        public readonly int Y;

        public UvPointKey(Vector2 uv)
        {
            X = Mathf.RoundToInt(uv.x * UvKeyPrecision);
            Y = Mathf.RoundToInt(uv.y * UvKeyPrecision);
        }

        public bool Equals(UvPointKey other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is UvPointKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }

    private struct UvEdgeKey : IEquatable<UvEdgeKey>
    {
        public readonly UvPointKey A;
        public readonly UvPointKey B;

        public UvEdgeKey(Vector2 first, Vector2 second)
        {
            UvPointKey firstKey = new UvPointKey(first);
            UvPointKey secondKey = new UvPointKey(second);
            if (firstKey.X < secondKey.X || (firstKey.X == secondKey.X && firstKey.Y <= secondKey.Y))
            {
                A = firstKey;
                B = secondKey;
            }
            else
            {
                A = secondKey;
                B = firstKey;
            }
        }

        public bool Equals(UvEdgeKey other)
        {
            return A.Equals(other.A) && B.Equals(other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is UvEdgeKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A.GetHashCode() * 397) ^ B.GetHashCode();
            }
        }
    }

    [MenuItem(MenuPath)]
    private static void OpenWindow()
    {
        CpRemixEquipmentTextureRenderer window = GetWindow<CpRemixEquipmentTextureRenderer>("CPRemix Equipment Texture Renderer");
        window.minSize = new Vector2(960f, 620f);
        window.maximized = true;
        window.Focus();
    }

    private void OnEnable()
    {
        if (outputSizeIndex <= 0 || outputSizeIndex >= OutputSizeValues.Length)
        {
            outputSizeIndex = 2;
        }

        if (Mathf.Approximately(lightYRotation, 35f))
        {
            lightYRotation = 160f;
        }

        previewMouseEditMode = false;
        previewDragging = false;
        previewMoveLayerDragging = false;
        previewRotateLayerDragging = false;

        InitLivePreview();
    }

    private void OnDisable()
    {
        DestroyPreviewTexture();
        CleanupLivePreview();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (sourceMesh == null || mouse == null)
        {
            return;
        }

        if (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed)
        {
            Repaint();
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.activeObject is Material material && IsSupportedShader(material.shader))
        {
            sourceMaterial = material;
            RefreshLivePreview();
            Repaint();
            return;
        }

        if (Selection.activeObject is Mesh mesh)
        {
            sourceMesh = mesh;
            TryAutoAssignDefinitionFromMesh();
            RefreshLivePreview();
            Repaint();
            return;
        }

        if (Selection.activeObject is BaseViewDefinition definition)
        {
            sourceDefinition = definition;
            sourceMesh = GetDefinitionMesh(definition);
            RefreshLivePreview();
            Repaint();
        }
    }

    private void OnGUI()
    {
        const float paneSpacing = 12f;
        float leftPaneWidth = Mathf.Clamp(position.width * 0.28f, 300f, 420f);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(leftPaneWidth), GUILayout.ExpandHeight(true));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
        DrawFields();
        DrawActions();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        GUILayout.Space(paneSpacing);

        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        previewPaneScrollPosition = EditorGUILayout.BeginScrollView(previewPaneScrollPosition, GUILayout.ExpandHeight(true));
        DrawLivePreview();
        DrawGameLibrary();
        DrawBakedPreview();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawFields()
    {
        EditorGUI.BeginChangeCheck();
        sourceMaterial = (Material)EditorGUILayout.ObjectField("Material", sourceMaterial, typeof(Material), false);
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", sourceMesh, typeof(Mesh), false);
        sourceDefinition = (BaseViewDefinition)EditorGUILayout.ObjectField("Definition", sourceDefinition, typeof(BaseViewDefinition), false);
        outputSizeIndex = EditorGUILayout.Popup("Texture Size", outputSizeIndex, OutputSizeLabels);
        lightIntensity = EditorGUILayout.Slider("Light Intensity", lightIntensity, 0f, 4f);
        lightXRotation = EditorGUILayout.Slider("Light X Rotation", lightXRotation, -180f, 180f);
        lightYRotation = EditorGUILayout.Slider("Light Y Rotation", lightYRotation, -180f, 180f);
        if (EditorGUI.EndChangeCheck())
        {
            TryAutoAssignDefinitionFromMesh();
            RefreshLivePreview();
            DestroyPreviewTexture();
        }
    }

    private void DrawActions()
    {
        GUILayout.Space(8f);

        if (GUILayout.Button("Create Material From Definition...", GUILayout.Height(30f)))
        {
            CreateMaterialFromDefinition();
        }

        if (GUILayout.Button("Generate Decal 123 Texture...", GUILayout.Height(30f)))
        {
            GenerateDecal123Texture();
        }

        if (GUILayout.Button("Bake Preview", GUILayout.Height(30f)))
        {
            BakePreviewTexture();
        }

        if (GUILayout.Button("Save PNG To Assets...", GUILayout.Height(30f)))
        {
            SaveBakedTextureToAssets();
        }
    }

    private void DrawGameLibrary()
    {
        GUILayout.Space(12f);
        EditorGUILayout.LabelField("Game Library", EditorStyles.boldLabel);

        if (sourceMaterial == null || !IsEquipmentShader(sourceMaterial.shader))
        {
            EditorGUILayout.LabelField("Assign a CPRemix equipment material to browse game fabrics and decals.", EditorStyles.miniLabel);
            return;
        }

        EnsureLibraryLoaded();

        libraryTabIndex = GUILayout.Toolbar(libraryTabIndex, LibraryTabs);
        libraryChannelIndex = GUILayout.Toolbar(Mathf.Clamp(libraryChannelIndex, 0, ChannelLabels.Length - 1), ChannelLabels);
        librarySearch = EditorGUILayout.TextField("Search", librarySearch);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Library"))
        {
            ReloadLibrary();
        }

        if (GUILayout.Button("Clear Channel"))
        {
            ClearCurrentLibraryChannel();
        }

        EditorGUILayout.EndHorizontal();

        string currentSelectionLabel = string.IsNullOrEmpty(lastAppliedLibraryAssetName) ? "None" : lastAppliedLibraryAssetName;
        EditorGUILayout.LabelField("Current: " + currentSelectionLabel, EditorStyles.miniBoldLabel);

        bool currentIsFabric = (libraryTabIndex == 0);
        int currentChannelIndex = Mathf.Clamp(libraryChannelIndex, 0, ChannelLabels.Length - 1);
        Texture currentTexture = GetCurrentLibraryChannelTexture(currentChannelIndex, currentIsFabric);
        string currentAssetName = GetCurrentLibraryChannelAssetName(currentChannelIndex, currentIsFabric);

        GUILayout.Space(6f);
        EditorGUILayout.LabelField((currentIsFabric ? "Fabric" : "Decal") + " Controls", EditorStyles.boldLabel);

        bool hasCurrentTexture = currentTexture != null;
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (hasCurrentTexture)
        {
            GUILayout.Label(currentTexture, GUILayout.Width(42f), GUILayout.Height(42f));
        }
        else
        {
            GUILayout.Box(GUIContent.none, GUILayout.Width(42f), GUILayout.Height(42f));
        }

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(hasCurrentTexture ? currentAssetName : "None", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(ChannelLabels[currentChannelIndex] + " channel", EditorStyles.miniLabel);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        bool allowFabricTransform = !currentIsFabric || CurrentFabricAllowsRotationAndScale(currentAssetName);
        string transformNote = currentIsFabric && hasCurrentTexture && !allowFabricTransform
            ? "This fabric disables scale and rotation in the game definition."
            : string.Empty;
        EditorGUILayout.LabelField(transformNote, EditorStyles.miniLabel);

        bool previousPreviewMouseEditMode = previewMouseEditMode;
        using (new EditorGUI.DisabledScope(!hasCurrentTexture))
        {
            previewMouseEditMode = EditorGUILayout.Toggle("Edit On Model Preview", previewMouseEditMode);
        }

        if (!hasCurrentTexture)
        {
            previewMouseEditMode = false;
        }

        if (previousPreviewMouseEditMode != previewMouseEditMode)
        {
            previewDragging = false;
            previewMoveLayerDragging = false;
            previewRotateLayerDragging = false;
        }

        EditorGUILayout.LabelField(
            previewMouseEditMode
                ? "Left drag moves. Right drag rotates. Mouse wheel scales. Middle drag rotates the object."
                : (hasCurrentTexture ? string.Empty : "This channel is empty. Pick one from the library below."),
            EditorStyles.miniLabel);

        float scale = hasCurrentTexture ? GetDisplayedChannelScale(currentChannelIndex, currentIsFabric) : (currentIsFabric ? DesignerDefaultScale : 1f);
        float rotationDegrees = hasCurrentTexture ? GetDisplayedChannelRotationDegrees(currentChannelIndex, currentIsFabric) : 0f;
        float positionX = hasCurrentTexture ? GetDisplayedChannelPositionX(currentChannelIndex, currentIsFabric) : 0f;
        float positionY = hasCurrentTexture ? GetDisplayedChannelPositionY(currentChannelIndex, currentIsFabric) : 0f;
        bool repeat = hasCurrentTexture && GetChannelFloat(currentChannelIndex, currentIsFabric ? FabricRepeatPropertyIds : DecalRepeatPropertyIds) > 0.5f;

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup(!hasCurrentTexture || (currentIsFabric && !allowFabricTransform));
        scale = EditorGUILayout.Slider("Scale", scale, MinDecalScale, MaxDecalScale);
        rotationDegrees = EditorGUILayout.Slider("Rotation", rotationDegrees, -180f, 180f);
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(!hasCurrentTexture);
        positionX = EditorGUILayout.Slider("Position X", positionX, MinUvOffset, MaxUvOffset);
        positionY = EditorGUILayout.Slider("Position Y", positionY, MinUvOffset, MaxUvOffset);
        repeat = EditorGUILayout.Toggle("Repeat", repeat);
        EditorGUI.EndDisabledGroup();
        bool channelControlsChanged = EditorGUI.EndChangeCheck();
        if (hasCurrentTexture && channelControlsChanged)
        {
            Undo.RecordObject(sourceMaterial, "Adjust CPRemix Channel");
            if (!currentIsFabric || allowFabricTransform)
            {
                SetDisplayedChannelScale(currentChannelIndex, currentIsFabric, scale);
                SetDisplayedChannelRotationDegrees(currentChannelIndex, currentIsFabric, rotationDegrees);
            }

            SetDisplayedChannelPosition(currentChannelIndex, currentIsFabric, positionX, positionY);
            SetChannelFloat(currentChannelIndex, currentIsFabric ? FabricRepeatPropertyIds : DecalRepeatPropertyIds, repeat ? 1f : 0f);
            EditorUtility.SetDirty(sourceMaterial);
            Repaint();
        }

        List<DesignerLibraryEntry> entries = GetFilteredLibraryEntries();
        if (entries.Count == 0)
        {
            EditorGUILayout.LabelField("No library items matched the current search.", EditorStyles.miniLabel);
            return;
        }

        libraryScrollPosition = EditorGUILayout.BeginScrollView(libraryScrollPosition, GUILayout.Height(LibraryScrollHeight));
        int columnCount = Mathf.Max(3, Mathf.FloorToInt((position.width - 32f) / LibraryItemWidth));
        GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.wordWrap = true;

        for (int i = 0; i < entries.Count; i += columnCount)
        {
            EditorGUILayout.BeginHorizontal();
            for (int column = 0; column < columnCount; column++)
            {
                int index = i + column;
                if (index >= entries.Count)
                {
                    break;
                }

                DesignerLibraryEntry entry = entries[index];
                bool isSelected = string.Equals(lastAppliedLibraryAssetName, entry.AssetName, StringComparison.OrdinalIgnoreCase);
                Color previousBackground = GUI.backgroundColor;
                if (isSelected)
                {
                    GUI.backgroundColor = new Color(0.44f, 0.67f, 0.95f, 1f);
                }

                GUILayout.BeginVertical(GUILayout.Width(LibraryItemWidth));
                GUIContent content = entry.Texture != null ? new GUIContent(entry.Texture, entry.DisplayName) : new GUIContent(entry.DisplayName);
                if (GUILayout.Button(content, GUILayout.Width(LibraryThumbnailSize), GUILayout.Height(LibraryThumbnailSize)))
                {
                    ApplyLibraryEntry(entry);
                }

                GUI.backgroundColor = previousBackground;
                GUILayout.Label(entry.DisplayName, labelStyle, GUILayout.Width(LibraryThumbnailSize));
                GUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void EnsureLibraryLoaded()
    {
        if (libraryLoaded)
        {
            return;
        }

        ReloadLibrary();
    }

    private void ReloadLibrary()
    {
        fabricLibraryEntries = LoadFabricLibraryEntries();
        decalLibraryEntries = LoadDecalLibraryEntries();
        libraryLoaded = true;
    }

    private DesignerLibraryEntry[] LoadFabricLibraryEntries()
    {
        return LoadLibraryEntries<FabricDefinition>(FabricDefinitionFolders, true);
    }

    private DesignerLibraryEntry[] LoadDecalLibraryEntries()
    {
        return LoadLibraryEntries<DecalDefinition>(DecalDefinitionFolders, false);
    }

    private DesignerLibraryEntry[] LoadLibraryEntries<TDefinition>(string[] folders, bool isFabric) where TDefinition : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(TDefinition).Name, folders);
        List<DesignerLibraryEntry> entries = new List<DesignerLibraryEntry>(guids.Length);
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            TDefinition definition = AssetDatabase.LoadAssetAtPath<TDefinition>(path);
            if (definition == null)
            {
                continue;
            }

            string assetName = GetLibraryAssetName(definition);
            if (string.IsNullOrEmpty(assetName))
            {
                continue;
            }

            DesignerLibraryEntry entry = new DesignerLibraryEntry();
            entry.AssetName = assetName;
            entry.DisplayName = definition.name;
            entry.Texture = LoadLibraryTexture(assetName);
            entry.AllowRotationAndScale = !isFabric || GetFabricAllowsRotation(definition as FabricDefinition);
            entries.Add(entry);
        }

        entries.Sort((left, right) => string.Compare(left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase));
        return entries.ToArray();
    }

    private static string GetLibraryAssetName(UnityEngine.Object definition)
    {
        FabricDefinition fabricDefinition = definition as FabricDefinition;
        if (fabricDefinition != null)
        {
            return fabricDefinition.AssetName;
        }

        DecalDefinition decalDefinition = definition as DecalDefinition;
        if (decalDefinition != null)
        {
            return decalDefinition.AssetName;
        }

        return null;
    }

    private static bool GetFabricAllowsRotation(FabricDefinition definition)
    {
        return definition == null || definition.allowRotationAndScale;
    }

    private static Texture2D LoadLibraryTexture(string assetName)
    {
        string directPath = DecalTextureFolder + "/" + assetName + ".png";
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(directPath);
        if (texture != null)
        {
            return texture;
        }

        string[] guids = AssetDatabase.FindAssets(assetName + " t:Texture2D", new[] { DecalTextureFolder });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (string.Equals(Path.GetFileNameWithoutExtension(path), assetName, StringComparison.OrdinalIgnoreCase))
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }

        return null;
    }

    private List<DesignerLibraryEntry> GetFilteredLibraryEntries()
    {
        DesignerLibraryEntry[] sourceEntries = libraryTabIndex == 0 ? fabricLibraryEntries : decalLibraryEntries;
        List<DesignerLibraryEntry> filteredEntries = new List<DesignerLibraryEntry>();
        for (int i = 0; i < sourceEntries.Length; i++)
        {
            DesignerLibraryEntry entry = sourceEntries[i];
            if (entry == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(librarySearch) ||
                entry.DisplayName.IndexOf(librarySearch, StringComparison.OrdinalIgnoreCase) >= 0 ||
                entry.AssetName.IndexOf(librarySearch, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filteredEntries.Add(entry);
            }
        }

        return filteredEntries;
    }

    private Texture GetCurrentLibraryChannelTexture(int channelIndex, bool isFabric)
    {
        int[] textureIds = isFabric ? FabricTexturePropertyIds : DecalTexturePropertyIds;
        return sourceMaterial != null ? sourceMaterial.GetTexture(textureIds[channelIndex]) : null;
    }

    private string GetCurrentLibraryChannelAssetName(int channelIndex, bool isFabric)
    {
        Texture texture = GetCurrentLibraryChannelTexture(channelIndex, isFabric);
        if (texture == null)
        {
            return "None";
        }

        return texture.name;
    }

    private bool CurrentFabricAllowsRotationAndScale(string assetName)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            return true;
        }

        for (int i = 0; i < fabricLibraryEntries.Length; i++)
        {
            DesignerLibraryEntry entry = fabricLibraryEntries[i];
            if (entry != null && string.Equals(entry.AssetName, assetName, StringComparison.OrdinalIgnoreCase))
            {
                return entry.AllowRotationAndScale;
            }
        }

        return true;
    }

    private float GetChannelFloat(int channelIndex, int[] propertyIds)
    {
        if (sourceMaterial == null)
        {
            return 0f;
        }

        int propertyId = propertyIds[channelIndex];
        return sourceMaterial.HasProperty(propertyId) ? sourceMaterial.GetFloat(propertyId) : 0f;
    }

    private float GetDisplayedChannelScale(int channelIndex, bool isFabric)
    {
        int[] scalePropertyIds = isFabric ? FabricScalePropertyIds : DecalScalePropertyIds;
        return Mathf.Clamp(GetChannelFloat(channelIndex, scalePropertyIds), MinDecalScale, MaxDecalScale);
    }

    private void SetDisplayedChannelScale(int channelIndex, bool isFabric, float displayedScale)
    {
        int[] scalePropertyIds = isFabric ? FabricScalePropertyIds : DecalScalePropertyIds;
        SetChannelFloat(channelIndex, scalePropertyIds, Mathf.Clamp(displayedScale, MinDecalScale, MaxDecalScale));
    }

    private float GetDisplayedChannelRotationDegrees(int channelIndex, bool isFabric)
    {
        int[] rotationPropertyIds = isFabric ? FabricRotationPropertyIds : DecalRotationPropertyIds;
        return Mathf.Clamp(GetChannelFloat(channelIndex, rotationPropertyIds) * Mathf.Rad2Deg, -180f, 180f);
    }

    private void SetDisplayedChannelRotationDegrees(int channelIndex, bool isFabric, float displayedRotationDegrees)
    {
        int[] rotationPropertyIds = isFabric ? FabricRotationPropertyIds : DecalRotationPropertyIds;
        SetChannelFloat(channelIndex, rotationPropertyIds, Mathf.Clamp(displayedRotationDegrees, -180f, 180f) * Mathf.Deg2Rad);
    }

    private float GetDisplayedChannelPositionX(int channelIndex, bool isFabric)
    {
        int[] uOffsetPropertyIds = isFabric ? FabricUOffsetPropertyIds : DecalUOffsetPropertyIds;
        return Mathf.Clamp(GetChannelFloat(channelIndex, uOffsetPropertyIds), MinUvOffset, MaxUvOffset);
    }

    private float GetDisplayedChannelPositionY(int channelIndex, bool isFabric)
    {
        int[] vOffsetPropertyIds = isFabric ? FabricVOffsetPropertyIds : DecalVOffsetPropertyIds;
        return Mathf.Clamp(GetChannelFloat(channelIndex, vOffsetPropertyIds), MinUvOffset, MaxUvOffset);
    }

    private void SetDisplayedChannelPosition(int channelIndex, bool isFabric, float displayedX, float displayedY)
    {
        int[] uOffsetPropertyIds = isFabric ? FabricUOffsetPropertyIds : DecalUOffsetPropertyIds;
        int[] vOffsetPropertyIds = isFabric ? FabricVOffsetPropertyIds : DecalVOffsetPropertyIds;
        SetChannelFloat(channelIndex, uOffsetPropertyIds, Mathf.Clamp(displayedX, MinUvOffset, MaxUvOffset));
        SetChannelFloat(channelIndex, vOffsetPropertyIds, Mathf.Clamp(displayedY, MinUvOffset, MaxUvOffset));
    }

    private void SetChannelFloat(int channelIndex, int[] propertyIds, float value)
    {
        if (sourceMaterial == null)
        {
            return;
        }

        int propertyId = propertyIds[channelIndex];
        if (sourceMaterial.HasProperty(propertyId))
        {
            sourceMaterial.SetFloat(propertyId, value);
        }
    }

    private void ApplyLibraryEntry(DesignerLibraryEntry entry)
    {
        if (sourceMaterial == null || entry == null)
        {
            return;
        }

        int channelIndex = Mathf.Clamp(libraryChannelIndex, 0, ChannelLabels.Length - 1);
        bool isFabric = (libraryTabIndex == 0);
        int[] textureIds = isFabric ? FabricTexturePropertyIds : DecalTexturePropertyIds;
        int[] scaleIds = isFabric ? FabricScalePropertyIds : DecalScalePropertyIds;
        int[] uOffsetIds = isFabric ? FabricUOffsetPropertyIds : DecalUOffsetPropertyIds;
        int[] vOffsetIds = isFabric ? FabricVOffsetPropertyIds : DecalVOffsetPropertyIds;
        int[] repeatIds = isFabric ? FabricRepeatPropertyIds : DecalRepeatPropertyIds;
        int[] rotationIds = isFabric ? FabricRotationPropertyIds : DecalRotationPropertyIds;

        Undo.RecordObject(sourceMaterial, "Apply CPRemix Library Item");
        bool hadTexture = sourceMaterial.GetTexture(textureIds[channelIndex]) != null;

        sourceMaterial.SetTexture(textureIds[channelIndex], entry.Texture);
        sourceMaterial.SetFloat(repeatIds[channelIndex], isFabric ? 1f : 0f);

        if (!hadTexture)
        {
            sourceMaterial.SetFloat(scaleIds[channelIndex], DesignerDefaultScale);
            sourceMaterial.SetFloat(uOffsetIds[channelIndex], 0f);
            sourceMaterial.SetFloat(vOffsetIds[channelIndex], 0f);
            sourceMaterial.SetFloat(rotationIds[channelIndex], 0f);
        }

        lastAppliedLibraryAssetName = entry.AssetName;
        EditorUtility.SetDirty(sourceMaterial);
        Repaint();
    }

    private void ClearCurrentLibraryChannel()
    {
        if (sourceMaterial == null)
        {
            return;
        }

        int channelIndex = Mathf.Clamp(libraryChannelIndex, 0, ChannelLabels.Length - 1);
        bool isFabric = (libraryTabIndex == 0);
        int[] textureIds = isFabric ? FabricTexturePropertyIds : DecalTexturePropertyIds;
        int[] scaleIds = isFabric ? FabricScalePropertyIds : DecalScalePropertyIds;
        int[] uOffsetIds = isFabric ? FabricUOffsetPropertyIds : DecalUOffsetPropertyIds;
        int[] vOffsetIds = isFabric ? FabricVOffsetPropertyIds : DecalVOffsetPropertyIds;
        int[] repeatIds = isFabric ? FabricRepeatPropertyIds : DecalRepeatPropertyIds;
        int[] rotationIds = isFabric ? FabricRotationPropertyIds : DecalRotationPropertyIds;

        Undo.RecordObject(sourceMaterial, "Clear CPRemix Channel");
        sourceMaterial.SetTexture(textureIds[channelIndex], null);
        sourceMaterial.SetFloat(scaleIds[channelIndex], isFabric ? DesignerDefaultScale : 1f);
        sourceMaterial.SetFloat(uOffsetIds[channelIndex], 0f);
        sourceMaterial.SetFloat(vOffsetIds[channelIndex], 0f);
        sourceMaterial.SetFloat(repeatIds[channelIndex], isFabric ? 1f : 0f);
        sourceMaterial.SetFloat(rotationIds[channelIndex], 0f);
        lastAppliedLibraryAssetName = null;
        EditorUtility.SetDirty(sourceMaterial);
        Repaint();
    }

    private void DrawLivePreview()
    {
        GUILayout.Space(12f);
        Rect previewRect = GUILayoutUtility.GetRect(10f, LivePreviewHeight, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(previewRect, new Color(0.14f, 0.14f, 0.14f, 1f));

        if (sourceMesh == null)
        {
            GUI.Label(previewRect, "Assign a mesh to show the live model preview.", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        InitLivePreview();
        EnsureLivePreviewInstance();
        UpdateLivePreviewLight();
        HandleLivePreviewInput(previewRect);
        RenderLivePreview(previewRect);
    }

    private void DrawBakedPreview()
    {
        if (previewTexture == null)
        {
            return;
        }

        GUILayout.Space(12f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        Rect previewRect = GUILayoutUtility.GetAspectRect(1f, GUILayout.MaxWidth(PreviewDisplaySize));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUI.DrawRect(previewRect, new Color(0.18f, 0.18f, 0.18f, 1f));
        EditorGUI.DrawPreviewTexture(previewRect, previewTexture, null, ScaleMode.ScaleToFit);
    }

    private void BakePreviewTexture()
    {
        if (!TryValidateInputs(out string error))
        {
            ShowError(error);
            return;
        }

        DestroyPreviewTexture();

        if (!TryBakeMaterial(sourceMaterial, GetSelectedOutputSize(), out Texture2D bakedTexture, out error))
        {
            ShowError(error);
            return;
        }

        previewTexture = bakedTexture;
    }

    private void SaveBakedTextureToAssets()
    {
        if (!TryValidateInputs(out string error))
        {
            ShowError(error);
            return;
        }

        if (!TryBakeMaterial(sourceMaterial, GetSelectedOutputSize(), out Texture2D bakedTexture, out error))
        {
            ShowError(error);
            return;
        }

        string defaultFileName = BuildDefaultFileName(sourceMesh, sourceMaterial);
        if (!TryShowSavePanelInProject("Save Baked Texture", defaultFileName, "png", "Choose where to save the baked texture.", out string savePath))
        {
            DestroyImmediate(bakedTexture);
            return;
        }

        try
        {
            string absoluteSavePath = GetAbsoluteAssetPath(savePath);
            File.WriteAllBytes(absoluteSavePath, bakedTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);
            ConfigureImportedTexture(savePath);
            RememberSaveDirectory(savePath);

            Texture2D importedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
            if (importedTexture != null)
            {
                DestroyPreviewTexture();
                previewTexture = importedTexture;
                EditorGUIUtility.PingObject(importedTexture);
            }

            Debug.Log("Saved baked texture to " + savePath);
        }
        catch (Exception ex)
        {
            ShowError("Failed to save baked texture: " + ex.Message);
        }
        finally
        {
            if (previewTexture != bakedTexture)
            {
                DestroyImmediate(bakedTexture);
            }
        }
    }

    private void GenerateDecal123Texture()
    {
        if (!TryValidateSourceMesh(sourceMesh, out string error))
        {
            ShowError(error);
            return;
        }

        if (!TryGenerateDecal123Texture(sourceMesh, GetSelectedOutputSize(), out Texture2D decalTexture, out error))
        {
            ShowError(error);
            return;
        }

        string defaultFileName = BuildDecal123FileName(sourceMesh);
        if (!TryShowSavePanelInProject("Save Decal 123 Texture", defaultFileName, "png", "Choose where to save the generated decal 123 texture.", out string savePath))
        {
            DestroyImmediate(decalTexture);
            return;
        }

        try
        {
            string absoluteSavePath = GetAbsoluteAssetPath(savePath);
            File.WriteAllBytes(absoluteSavePath, decalTexture.EncodeToPNG());
            AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);
            ConfigureImportedDecal123Texture(savePath);
            RememberSaveDirectory(savePath);

            Texture2D importedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
            if (importedTexture != null)
            {
                DestroyPreviewTexture();
                previewTexture = importedTexture;
                EditorGUIUtility.PingObject(importedTexture);

                if (sourceMaterial != null && IsEquipmentShader(sourceMaterial.shader))
                {
                    Undo.RecordObject(sourceMaterial, "Assign CPRemix Decal 123 Texture");
                    sourceMaterial.SetTexture(EquipmentShaderParams.DECALS_123_OPACITY_TEX, importedTexture);
                    EditorUtility.SetDirty(sourceMaterial);
                    RefreshLivePreview();
                }
            }

            Debug.Log("Saved decal 123 texture to " + savePath);
        }
        catch (Exception ex)
        {
            ShowError("Failed to save decal 123 texture: " + ex.Message);
        }
        finally
        {
            if (previewTexture != decalTexture)
            {
                DestroyImmediate(decalTexture);
            }
        }
    }

    private void CreateMaterialFromDefinition()
    {
        if (!TryResolveDefinition(out BaseViewDefinition resolvedDefinition, out string error))
        {
            ShowError(error);
            return;
        }

        Material material = CreatePreviewMaterialFromDefinition(resolvedDefinition, sourceMaterial, out error);
        if (material == null)
        {
            ShowError(error);
            return;
        }

        string defaultFileName = BuildMaterialFileName(resolvedDefinition, sourceMesh);
        if (!TryShowSavePanelInProject("Create Material From Definition", defaultFileName, "mat", "Choose where to save the generated material.", out string savePath))
        {
            DestroyImmediate(material);
            return;
        }

        AssetDatabase.CreateAsset(material, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        RememberSaveDirectory(savePath);

        sourceMaterial = AssetDatabase.LoadAssetAtPath<Material>(savePath);
        if (sourceMaterial == null)
        {
            sourceMaterial = material;
        }

        EditorGUIUtility.PingObject(sourceMaterial);
        RefreshLivePreview();
        DestroyPreviewTexture();
    }

    private bool TryValidateInputs(out string error)
    {
        error = null;

        if (sourceMaterial == null)
        {
            error = "Assign a material.";
            return false;
        }

        if (!TryReadMaterialParams(sourceMaterial, out _, out error))
        {
            return false;
        }

        if (sourceMesh == null)
        {
            error = "Assign a mesh.";
            return false;
        }

        return TryValidateSourceMesh(sourceMesh, out error);
    }

    private bool TryResolveDefinition(out BaseViewDefinition resolvedDefinition, out string error)
    {
        resolvedDefinition = sourceDefinition;
        error = null;

        if (resolvedDefinition != null && sourceMesh == null)
        {
            sourceMesh = GetDefinitionMesh(resolvedDefinition);
        }

        if (resolvedDefinition == null && sourceMesh != null)
        {
            resolvedDefinition = FindUniqueDefinitionForMesh(sourceMesh, out error);
            if (resolvedDefinition != null)
            {
                sourceDefinition = resolvedDefinition;
            }
        }

        if (resolvedDefinition == null)
        {
            if (string.IsNullOrEmpty(error))
            {
                error = "Assign a definition or pick a mesh that matches exactly one definition.";
            }

            return false;
        }

        if (!DefinitionUsesMesh(resolvedDefinition, sourceMesh))
        {
            error = "The selected definition does not use the selected mesh.";
            return false;
        }

        return true;
    }

    private void InitLivePreview()
    {
        if (previewRenderUtility != null)
        {
            return;
        }

        previewRenderUtility = new PreviewRenderUtility();
        previewRenderUtility.camera.fieldOfView = 30f;
        previewRenderUtility.camera.nearClipPlane = 0.01f;
        previewRenderUtility.camera.farClipPlane = 100f;
        previewRenderUtility.camera.clearFlags = CameraClearFlags.SolidColor;
        previewRenderUtility.camera.backgroundColor = new Color(0.14f, 0.14f, 0.14f, 1f);
        UpdateLivePreviewLight();
    }

    private void CleanupLivePreview()
    {
        CleanupLivePreviewInstance();
        livePreviewFrameTexture = null;

        if (previewRenderUtility != null)
        {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }
    }

    private void CleanupLivePreviewInstance()
    {
        previewMeshCollider = null;
        previewMeshRenderer = null;

        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
    }

    private void RefreshLivePreview()
    {
        CleanupLivePreviewInstance();
        livePreviewFrameTexture = null;
        Repaint();
    }

    private void EnsureLivePreviewInstance()
    {
        if (previewRenderUtility == null || sourceMesh == null)
        {
            return;
        }

        if (previewInstance == null)
        {
            previewInstance = new GameObject("CPRemixMaterialPreviewMesh");
            previewInstance.hideFlags = HideFlags.HideAndDontSave;

            MeshFilter meshFilter = previewInstance.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = sourceMesh;

            previewMeshRenderer = previewInstance.AddComponent<MeshRenderer>();
            previewMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            previewMeshRenderer.receiveShadows = false;
            previewMeshCollider = previewInstance.AddComponent<MeshCollider>();
            previewMeshCollider.sharedMesh = sourceMesh;

            previewRenderUtility.AddSingleGO(previewInstance);
            AutoFrameLivePreview();
        }

        MeshFilter previewMeshFilter = previewInstance.GetComponent<MeshFilter>();
        if (previewMeshFilter != null && previewMeshFilter.sharedMesh != sourceMesh)
        {
            previewMeshFilter.sharedMesh = sourceMesh;
            AutoFrameLivePreview();
        }

        if (previewMeshRenderer == null)
        {
            previewMeshRenderer = previewInstance.GetComponent<MeshRenderer>();
        }

        if (previewMeshCollider == null)
        {
            previewMeshCollider = previewInstance.GetComponent<MeshCollider>();
        }

        if (previewMeshCollider != null && previewMeshCollider.sharedMesh != sourceMesh)
        {
            previewMeshCollider.sharedMesh = sourceMesh;
        }

        if (previewMeshRenderer != null)
        {
            previewMeshRenderer.sharedMaterial = sourceMaterial;
        }
    }

    private void AutoFrameLivePreview()
    {
        if (sourceMesh == null)
        {
            previewBaseDistance = 4f;
            previewDistance = 4f;
            return;
        }

        Bounds bounds = sourceMesh.bounds;
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        if (size < 0.001f)
        {
            size = 1f;
        }

        float distance = size / (2f * Mathf.Tan(previewRenderUtility.camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        previewBaseDistance = distance * 1.8f;
        previewDistance = previewBaseDistance;
        UpdateLivePreviewCamera();
    }

    private void UpdateLivePreviewCamera()
    {
        if (previewRenderUtility == null || sourceMesh == null)
        {
            return;
        }

        Bounds bounds = sourceMesh.bounds;
        Quaternion rotation = Quaternion.Euler(previewRotationX, previewRotationY, 0f);
        Vector3 target = bounds.center;
        Vector3 offset = rotation * new Vector3(0f, 0f, -previewDistance);

        previewRenderUtility.camera.transform.position = target + offset;
        previewRenderUtility.camera.transform.rotation = rotation;
        previewRenderUtility.camera.transform.LookAt(target);
    }

    private void UpdateLivePreviewLight()
    {
        if (previewRenderUtility == null)
        {
            return;
        }

        Light light = previewRenderUtility.lights[0];
        if (light != null)
        {
            light.type = LightType.Directional;
            light.enabled = true;
            light.intensity = lightIntensity;
            light.color = Color.white;
            light.transform.rotation = Quaternion.Euler(lightXRotation, lightYRotation, 0f);
        }

        if (previewRenderUtility.lights.Length > 1 && previewRenderUtility.lights[1] != null)
        {
            previewRenderUtility.lights[1].enabled = false;
            previewRenderUtility.lights[1].intensity = 0f;
        }
    }

    private void HandleLivePreviewInput(Rect previewRect)
    {
        Mouse mouse = Mouse.current;
        Event currentEvent = Event.current;
        if (mouse == null || currentEvent == null)
        {
            return;
        }

        Vector2 guiMousePosition = currentEvent.mousePosition;
        bool mouseInsidePreview = previewRect.Contains(guiMousePosition);
        bool hasActivePreviewInteraction = previewDragging || previewMoveLayerDragging || previewRotateLayerDragging;
        if (!IsPreviewInputEvent(currentEvent))
        {
            return;
        }

        if (!mouseInsidePreview && !hasActivePreviewInteraction)
        {
            return;
        }

        if (HandlePreviewChannelEditing(previewRect, guiMousePosition, mouse, mouseInsidePreview, currentEvent))
        {
            currentEvent.Use();
            return;
        }

        if (HandlePreviewCameraInput(mouseInsidePreview, mouse, currentEvent))
        {
            currentEvent.Use();
        }
    }

    private bool HandlePreviewChannelEditing(Rect previewRect, Vector2 guiMousePosition, Mouse mouse, bool mouseInsidePreview, Event currentEvent)
    {
        if (currentEvent.type == EventType.MouseUp || !mouse.leftButton.isPressed)
        {
            previewMoveLayerDragging = false;
        }

        if (currentEvent.type == EventType.MouseUp || !mouse.rightButton.isPressed)
        {
            previewRotateLayerDragging = false;
        }

        if (!previewMouseEditMode)
        {
            return false;
        }

        if (!mouseInsidePreview && !previewMoveLayerDragging && !previewRotateLayerDragging)
        {
            return false;
        }

        if (!TryGetCurrentEditableChannel(out bool isFabric, out int channelIndex, out bool allowRotationAndScale))
        {
            return false;
        }

        bool changed = false;

        if (mouseInsidePreview && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && !previewMoveLayerDragging)
        {
            Undo.RecordObject(sourceMaterial, "Move CPRemix Channel");
            previewMoveLayerDragging = true;
            previewRotateLayerDragging = false;
            previewDragging = false;
        }

        if (mouseInsidePreview && currentEvent.type == EventType.MouseDown && currentEvent.button == 1 && !previewRotateLayerDragging && allowRotationAndScale)
        {
            Undo.RecordObject(sourceMaterial, "Rotate CPRemix Channel");
            previewRotateLayerDragging = true;
            previewMoveLayerDragging = false;
            previewDragging = false;
        }

        if (previewMoveLayerDragging &&
            mouse.leftButton.isPressed &&
            currentEvent.type == EventType.MouseDrag)
        {
            Vector2 delta = currentEvent.delta;
            if (delta.sqrMagnitude > 0f)
            {
                float displayedX = GetDisplayedChannelPositionX(channelIndex, isFabric);
                float displayedY = GetDisplayedChannelPositionY(channelIndex, isFabric);
                float nextX = displayedX + (delta.x / Mathf.Max(1f, previewRect.width)) * PreviewLayerMoveSensitivity;
                float nextY = displayedY - (delta.y / Mathf.Max(1f, previewRect.height)) * PreviewLayerMoveSensitivity;
                SetDisplayedChannelPosition(channelIndex, isFabric, nextX, nextY);
                changed = true;
            }
        }

        if (previewRotateLayerDragging && mouse.rightButton.isPressed && allowRotationAndScale && currentEvent.type == EventType.MouseDrag)
        {
            Vector2 delta = currentEvent.delta;
            if (delta.sqrMagnitude > 0f)
            {
                float displayedRotationDegrees = GetDisplayedChannelRotationDegrees(channelIndex, isFabric);
                SetDisplayedChannelRotationDegrees(channelIndex, isFabric, displayedRotationDegrees + delta.x * PreviewLayerRotationSensitivity);
                changed = true;
            }
        }

        if (mouseInsidePreview && allowRotationAndScale && currentEvent.type == EventType.ScrollWheel)
        {
            float scrollDelta = currentEvent.delta.y;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                Undo.RecordObject(sourceMaterial, "Scale CPRemix Channel");
                float displayedScale = GetDisplayedChannelScale(channelIndex, isFabric);
                SetDisplayedChannelScale(channelIndex, isFabric, displayedScale + Mathf.Sign(scrollDelta) * PreviewLayerScaleStep);
                changed = true;
            }
        }

        if (!changed)
        {
            return previewMoveLayerDragging || previewRotateLayerDragging;
        }

        EditorUtility.SetDirty(sourceMaterial);
        Repaint();
        return true;
    }

    private bool HandlePreviewCameraInput(bool mouseInsidePreview, Mouse mouse, Event currentEvent)
    {
        int orbitButton = previewMouseEditMode ? 2 : 0;
        bool orbitButtonPressed = previewMouseEditMode ? previewDragging : mouse.leftButton.isPressed;
        bool cameraEnabled = !previewMouseEditMode || previewDragging || (mouseInsidePreview && currentEvent.type == EventType.MouseDown && currentEvent.button == orbitButton);

        if ((currentEvent.type == EventType.MouseUp || currentEvent.rawType == EventType.MouseUp) && (currentEvent.button == orbitButton || previewDragging))
        {
            previewDragging = false;
            return currentEvent.button == orbitButton;
        }

        bool cameraInputEnabled = mouseInsidePreview && cameraEnabled;

        if (!cameraInputEnabled)
        {
            return false;
        }

        if (currentEvent.type == EventType.MouseDown && currentEvent.button == orbitButton)
        {
            previewDragging = true;
            return true;
        }

        if (previewDragging && currentEvent.type == EventType.MouseDrag)
        {
            Vector2 delta = currentEvent.delta;
            if (delta.sqrMagnitude > 0f)
            {
                previewRotationY += delta.x * PreviewOrbitSensitivity;
                previewRotationX = Mathf.Clamp(previewRotationX + delta.y * PreviewOrbitSensitivity, -80f, 80f);
                UpdateLivePreviewCamera();
                Repaint();
                return true;
            }
        }

        if (!previewMouseEditMode && mouseInsidePreview && currentEvent.type == EventType.ScrollWheel)
        {
            float scrollDelta = currentEvent.delta.y;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                previewDistance += scrollDelta * previewDistance * 0.08f;
                previewDistance = Mathf.Clamp(previewDistance, previewBaseDistance * 0.35f, previewBaseDistance * 4f);
                UpdateLivePreviewCamera();
                Repaint();
                return true;
            }
        }

        return false;
    }

    private bool TryGetCurrentEditableChannel(out bool isFabric, out int channelIndex, out bool allowRotationAndScale)
    {
        isFabric = (libraryTabIndex == 0);
        channelIndex = Mathf.Clamp(libraryChannelIndex, 0, ChannelLabels.Length - 1);
        allowRotationAndScale = true;

        if (sourceMaterial == null || !IsEquipmentShader(sourceMaterial.shader))
        {
            return false;
        }

        Texture currentTexture = GetCurrentLibraryChannelTexture(channelIndex, isFabric);
        if (currentTexture == null)
        {
            return false;
        }

        string currentAssetName = GetCurrentLibraryChannelAssetName(channelIndex, isFabric);
        allowRotationAndScale = !isFabric || CurrentFabricAllowsRotationAndScale(currentAssetName);
        return true;
    }

    private bool TryRaycastPreviewUV(Rect previewRect, Vector2 guiMousePosition, out Vector2 uv)
    {
        uv = Vector2.zero;
        if (previewRenderUtility == null || previewMeshCollider == null)
        {
            return false;
        }

        Vector2 localMousePosition = guiMousePosition - previewRect.position;
        Vector3 viewportPoint = new Vector3(
            Mathf.Clamp01(localMousePosition.x / previewRect.width),
            Mathf.Clamp01(1f - (localMousePosition.y / previewRect.height)),
            0f);

        Ray ray = previewRenderUtility.camera.ViewportPointToRay(viewportPoint);
        if (!previewMeshCollider.Raycast(ray, out RaycastHit hit, previewRenderUtility.camera.farClipPlane))
        {
            return false;
        }

        uv = hit.textureCoord;
        return true;
    }

    private void SetChannelOffsetFromPreviewHit(int channelIndex, bool isFabric, Vector2 hitUv)
    {
        Vector2 centeredOffset = new Vector2(0.5f - hitUv.x, 0.5f - hitUv.y);
        SetDisplayedChannelPosition(channelIndex, isFabric, centeredOffset.x, centeredOffset.y);
    }

    private static bool IsPreviewInputEvent(Event currentEvent)
    {
        return currentEvent.type == EventType.MouseDown ||
               currentEvent.type == EventType.MouseDrag ||
               currentEvent.type == EventType.MouseUp ||
               currentEvent.type == EventType.ScrollWheel;
    }

    private void RenderLivePreview(Rect previewRect)
    {
        if (previewRenderUtility == null || Event.current == null)
        {
            return;
        }

        if (Event.current.type == EventType.Repaint)
        {
            UpdateLivePreviewCamera();
            previewRenderUtility.BeginPreview(previewRect, GUIStyle.none);
            previewRenderUtility.camera.Render();
            livePreviewFrameTexture = previewRenderUtility.EndPreview();
        }

        if (livePreviewFrameTexture != null)
        {
            GUI.DrawTexture(previewRect, livePreviewFrameTexture, ScaleMode.StretchToFill, false);
        }
    }

    private static void ShowError(string error)
    {
        EditorUtility.DisplayDialog("CPRemix Equipment Texture Renderer", error, "OK");
    }

    private static bool TryShowSavePanelInProject(string title, string defaultFileName, string extension, string message, out string assetPath)
    {
        assetPath = null;
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string initialDirectory = projectRoot;
        string lastSaveDirectory = EditorPrefs.GetString(LastSaveDirectoryPreference, string.Empty).Replace('\\', '/').Trim('/');
        if (!string.IsNullOrEmpty(lastSaveDirectory) &&
            (lastSaveDirectory.Equals("Assets", StringComparison.OrdinalIgnoreCase) ||
             lastSaveDirectory.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)))
        {
            initialDirectory = Path.GetFullPath(Path.Combine(projectRoot, lastSaveDirectory));
        }

        if (!Directory.Exists(initialDirectory))
        {
            initialDirectory = projectRoot;
        }

        string selectedPath = EditorUtility.SaveFilePanel(title, initialDirectory, defaultFileName, extension);
        if (string.IsNullOrEmpty(selectedPath))
        {
            return false;
        }

        string assetsRoot = Path.GetFullPath(Application.dataPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string fullSelectedPath = Path.GetFullPath(selectedPath);
        string assetsPrefix = assetsRoot + Path.DirectorySeparatorChar;
        if (!fullSelectedPath.StartsWith(assetsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            ShowError("Choose a save location inside the project's Assets folder.");
            return false;
        }

        assetPath = "Assets" + fullSelectedPath.Substring(assetsRoot.Length).Replace('\\', '/');
        return true;
    }

    private static void RememberSaveDirectory(string assetPath)
    {
        string directory = Path.GetDirectoryName(assetPath);
        if (!string.IsNullOrEmpty(directory))
        {
            EditorPrefs.SetString(LastSaveDirectoryPreference, directory.Replace('\\', '/'));
        }
    }

    private static void ConfigureImportedTexture(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Trilinear;
        importer.anisoLevel = 16;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static void ConfigureImportedDecal123Texture(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Trilinear;
        importer.anisoLevel = 16;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport();
    }

    private static string BuildMaterialFileName(BaseViewDefinition definition, Mesh mesh)
    {
        string definitionName = definition != null ? definition.name : null;
        if (!string.IsNullOrEmpty(definitionName))
        {
            return definitionName + "_PreviewMaterial";
        }

        if (mesh != null)
        {
            return mesh.name + "_PreviewMaterial";
        }

        return "GeneratedPreviewMaterial";
    }

    private static string BuildDefaultFileName(Mesh mesh, Material material)
    {
        if (mesh != null && material != null)
        {
            return mesh.name + "_" + material.name + "_Baked";
        }

        if (material != null)
        {
            return material.name + "_Baked";
        }

        if (mesh != null)
        {
            return mesh.name + "_Baked";
        }

        return "BakedTexture";
    }

    private static string BuildDecal123FileName(Mesh mesh)
    {
        if (mesh != null)
        {
            return mesh.name + "_DecalOpacity123";
        }

        return "Generated_DecalOpacity123";
    }

    private static string GetAbsoluteAssetPath(string assetPath)
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        return Path.GetFullPath(Path.Combine(projectRoot, assetPath));
    }

    private void TryAutoAssignDefinitionFromMesh()
    {
        if (sourceMesh == null)
        {
            return;
        }

        if (sourceDefinition != null && DefinitionUsesMesh(sourceDefinition, sourceMesh))
        {
            return;
        }

        sourceDefinition = FindUniqueDefinitionForMesh(sourceMesh, out _);
    }

    private static bool TryBakeMaterial(Material material, int requestedSize, out Texture2D bakedTexture, out string error)
    {
        bakedTexture = null;
        error = null;

        if (!TryReadMaterialParams(material, out EquipmentShaderParams shaderParams, out error))
        {
            return false;
        }

        Shader bakeShader = GetBakeShader(material.shader);
        if (bakeShader == null)
        {
            error = "Could not find the bake shader for " + material.shader.name + ".";
            return false;
        }

        int outputSize = requestedSize > 0 ? requestedSize : GetSuggestedOutputSize(shaderParams);
        outputSize = Mathf.Max(16, outputSize);

        Material bakeMaterial = null;
        RenderTexture renderTexture = null;
        RenderTexture previousActive = RenderTexture.active;

        try
        {
            bakeMaterial = new Material(bakeShader);
            shaderParams.AtlasOffsetU = 0f;
            shaderParams.AtlasOffsetV = 0f;
            shaderParams.AtlasScaleU = 1f;
            shaderParams.AtlasScaleV = 1f;
            shaderParams.ApplyToMaterial(bakeMaterial);

            renderTexture = RenderTexture.GetTemporary(outputSize, outputSize, 0, RenderTextureFormat.ARGB32);
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.anisoLevel = 16;

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));

            Texture blitSource = GetBlitSourceTexture(material, shaderParams);
            Graphics.Blit(blitSource, renderTexture, bakeMaterial);

            bakedTexture = new Texture2D(outputSize, outputSize, TextureFormat.RGBA32, false, false);
            bakedTexture.name = material.name + "_Baked";
            bakedTexture.ReadPixels(new Rect(0f, 0f, outputSize, outputSize), 0, 0);
            Color32[] bakedPixels = bakedTexture.GetPixels32();
            DilateRasterizedColors(bakedPixels, outputSize, 10);
            for (int i = 0; i < bakedPixels.Length; i++)
            {
                bakedPixels[i].a = 255;
            }

            bakedTexture.SetPixels32(bakedPixels);
            bakedTexture.Apply(false, false);
            return true;
        }
        catch (Exception ex)
        {
            error = "Bake failed: " + ex.Message;
            if (bakedTexture != null)
            {
                DestroyImmediate(bakedTexture);
                bakedTexture = null;
            }

            return false;
        }
        finally
        {
            RenderTexture.active = previousActive;

            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }

            if (bakeMaterial != null)
            {
                DestroyImmediate(bakeMaterial);
            }
        }
    }

    private static bool TryReadMaterialParams(Material material, out EquipmentShaderParams shaderParams, out string error)
    {
        shaderParams = null;
        error = null;

        if (material == null)
        {
            error = "Assign a material.";
            return false;
        }

        if (!IsSupportedShader(material.shader))
        {
            string shaderName = material.shader != null ? material.shader.name : "<Missing Shader>";
            error = "Unsupported shader: " + shaderName + ".";
            return false;
        }

        try
        {
            shaderParams = EquipmentShaderParams.FromMaterial(material);
            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    private static Shader GetBakeShader(Shader sourceShader)
    {
        if (sourceShader == null)
        {
            return null;
        }

        if (sourceShader.name == EquipmentShaderParams.EQUIPMENT_PREVIEW_SHADER_NAME ||
            sourceShader.name == EquipmentShaderParams.EQUIPMENT_BAKE_SHADER_NAME)
        {
            return Shader.Find(EquipmentShaderParams.EQUIPMENT_BAKE_SHADER_NAME);
        }

        if (sourceShader.name == EquipmentShaderParams.BODY_PREVIEW_SHADER_NAME ||
            sourceShader.name == EquipmentShaderParams.BODY_BAKE_SHADER_NAME)
        {
            return Shader.Find(EquipmentShaderParams.BODY_BAKE_SHADER_NAME);
        }

        return null;
    }

    private static bool IsSupportedShader(Shader shader)
    {
        if (shader == null)
        {
            return false;
        }

        return shader.name == EquipmentShaderParams.EQUIPMENT_PREVIEW_SHADER_NAME ||
               shader.name == EquipmentShaderParams.EQUIPMENT_BAKE_SHADER_NAME ||
               shader.name == EquipmentShaderParams.BODY_PREVIEW_SHADER_NAME ||
               shader.name == EquipmentShaderParams.BODY_BAKE_SHADER_NAME;
    }

    private static bool IsEquipmentShader(Shader shader)
    {
        if (shader == null)
        {
            return false;
        }

        return shader.name == EquipmentShaderParams.EQUIPMENT_PREVIEW_SHADER_NAME ||
               shader.name == EquipmentShaderParams.EQUIPMENT_BAKE_SHADER_NAME;
    }

    private static Material CreatePreviewMaterialFromDefinition(BaseViewDefinition definition, Material existingMaterial, out string error)
    {
        error = null;

        BodyViewDefinition bodyDefinition = definition as BodyViewDefinition;
        if (bodyDefinition == null)
        {
            error = "Unsupported definition type.";
            return null;
        }

        bool isEquipment = definition is EquipmentViewDefinition;
        string shaderName = isEquipment ? EquipmentShaderParams.EQUIPMENT_PREVIEW_SHADER_NAME : EquipmentShaderParams.BODY_PREVIEW_SHADER_NAME;
        Shader shader = Shader.Find(shaderName);
        if (shader == null)
        {
            error = "Could not find shader " + shaderName + ".";
            return null;
        }

        Material material = new Material(shader);
        if (bodyDefinition.BodyMaterial != null)
        {
            bodyDefinition.BodyMaterial.Apply(material);
        }
        ApplyDefaultBodyChannelColors(material, existingMaterial);

        EquipmentViewDefinition equipmentDefinition = definition as EquipmentViewDefinition;
        if (equipmentDefinition != null)
        {
            if (equipmentDefinition.EquipmentMaterial != null)
            {
                equipmentDefinition.EquipmentMaterial.Apply(material);
            }

            ApplyDefinitionDecalValues(material, equipmentDefinition);
        }

        return material;
    }

    private static void ApplyDefaultBodyChannelColors(Material material, Material existingMaterial)
    {
        if (existingMaterial != null && IsSupportedShader(existingMaterial.shader))
        {
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.BODY_RED_CHANNEL_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.BODY_GREEN_CHANNEL_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.BODY_BLUE_CHANNEL_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_RED_1_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_GREEN_2_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_BLUE_3_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_RED_4_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_GREEN_5_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.DECAL_BLUE_6_COLOR);
            CopyColorIfPresent(existingMaterial, material, EquipmentShaderParams.EMISSIVE_COLOR_TINT);
            return;
        }

        BodyColorMaterialProperties defaultColors = new BodyColorMaterialProperties();
        defaultColors.Apply(material);
    }

    private static void ApplyDefinitionDecalValues(Material material, EquipmentViewDefinition equipmentDefinition)
    {
        for (int channel = 0; channel < ShaderParams.DECAL_TEX.Length; channel++)
        {
            DecalMaterialProperties decalMaterial = null;
            if (equipmentDefinition.DecalMaterials != null && channel < equipmentDefinition.DecalMaterials.Length)
            {
                decalMaterial = equipmentDefinition.DecalMaterials[channel];
            }

            if (decalMaterial == null)
            {
                material.SetTexture(ShaderParams.DECAL_TEX[channel], null);
                material.SetFloat(ShaderParams.DECAL_SCALE[channel], 1f);
                material.SetFloat(ShaderParams.DECAL_U_OFFSET[channel], 0f);
                material.SetFloat(ShaderParams.DECAL_V_OFFSET[channel], 0f);
                material.SetFloat(ShaderParams.DECAL_REPEAT[channel], 0f);
                material.SetFloat(ShaderParams.DECAL_ROTATION_RADS[channel], 0f);
                continue;
            }

            material.SetTexture(ShaderParams.DECAL_TEX[channel], decalMaterial.Texture);
            material.SetFloat(ShaderParams.DECAL_SCALE[channel], decalMaterial.Scale);
            material.SetFloat(ShaderParams.DECAL_U_OFFSET[channel], decalMaterial.UOffset);
            material.SetFloat(ShaderParams.DECAL_V_OFFSET[channel], decalMaterial.VOffset);
            material.SetFloat(ShaderParams.DECAL_REPEAT[channel], decalMaterial.Repeat ? 1f : 0f);
            material.SetFloat(ShaderParams.DECAL_ROTATION_RADS[channel], decalMaterial.RotationRads);
        }
    }

    private static void CopyColorIfPresent(Material source, Material target, int propertyId)
    {
        if (source.HasProperty(propertyId) && target.HasProperty(propertyId))
        {
            target.SetColor(propertyId, source.GetColor(propertyId));
        }
    }

    private static bool TryValidateSourceMesh(Mesh mesh, out string error)
    {
        error = null;

        if (mesh == null)
        {
            error = "Assign a mesh.";
            return false;
        }

        if (mesh.vertexCount == 0)
        {
            error = "The selected mesh has no vertices.";
            return false;
        }

        Vector2[] uv = mesh.uv;
        if (uv == null || uv.Length == 0)
        {
            error = "The selected mesh has no UVs.";
            return false;
        }

        return true;
    }

    private static bool DefinitionUsesMesh(BaseViewDefinition definition, Mesh mesh)
    {
        if (definition == null || mesh == null)
        {
            return false;
        }

        return GetDefinitionMesh(definition) == mesh;
    }

    private static Mesh GetDefinitionMesh(BaseViewDefinition definition)
    {
        BodyViewDefinition bodyDefinition = definition as BodyViewDefinition;
        if (bodyDefinition == null || bodyDefinition.SkinnedMesh == null)
        {
            return null;
        }

        return bodyDefinition.SkinnedMesh.Mesh;
    }

    private static BaseViewDefinition FindUniqueDefinitionForMesh(Mesh mesh, out string error)
    {
        error = null;
        if (mesh == null)
        {
            error = "Assign a mesh.";
            return null;
        }

        string[] equipmentGuids = AssetDatabase.FindAssets("t:EquipmentViewDefinition");
        BaseViewDefinition match = null;
        int matchCount = FindMatchingDefinition(mesh, equipmentGuids, ref match);

        if (matchCount == 0)
        {
            string[] bodyGuids = AssetDatabase.FindAssets("t:BodyViewDefinition");
            matchCount = FindMatchingDefinition(mesh, bodyGuids, ref match);
        }

        if (matchCount == 1)
        {
            return match;
        }

        if (matchCount > 1)
        {
            error = "More than one definition uses this mesh. Assign the definition manually.";
            return null;
        }

        error = "No definition was found for the selected mesh.";
        return null;
    }

    private static int FindMatchingDefinition(Mesh mesh, string[] guids, ref BaseViewDefinition firstMatch)
    {
        int matches = 0;
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            BodyViewDefinition definition = AssetDatabase.LoadAssetAtPath<BodyViewDefinition>(path);
            if (definition == null || definition.SkinnedMesh == null || definition.SkinnedMesh.Mesh != mesh)
            {
                continue;
            }

            matches++;
            if (firstMatch == null)
            {
                firstMatch = definition;
            }
        }

        return matches;
    }

    private static Texture GetBlitSourceTexture(Material material, EquipmentShaderParams shaderParams)
    {
        Texture sourceTexture = GetUvDomainTexture(material, shaderParams);
        return sourceTexture != null ? sourceTexture : Texture2D.whiteTexture;
    }

    private static bool TryGenerateDecal123Texture(Mesh mesh, int requestedSize, out Texture2D decalTexture, out string error)
    {
        decalTexture = null;
        error = null;

        if (!TryValidateSourceMesh(mesh, out error))
        {
            return false;
        }

        int[] triangles = mesh.triangles;
        if (triangles == null || triangles.Length < 3)
        {
            error = "The selected mesh has no triangles.";
            return false;
        }

        int triangleCount = triangles.Length / 3;
        Vector2[] uv = mesh.uv;
        List<int>[] triangleNeighbors = BuildUvTriangleNeighbors(uv, triangles, triangleCount);
        int[] triangleIslandIndices = new int[triangleCount];
        for (int i = 0; i < triangleIslandIndices.Length; i++)
        {
            triangleIslandIndices[i] = -1;
        }

        List<float> islandAreas = new List<float>();
        Queue<int> pendingTriangles = new Queue<int>();
        int islandCount = 0;

        for (int triangleIndex = 0; triangleIndex < triangleCount; triangleIndex++)
        {
            if (triangleIslandIndices[triangleIndex] >= 0)
            {
                continue;
            }

            float islandArea = 0f;
            triangleIslandIndices[triangleIndex] = islandCount;
            pendingTriangles.Enqueue(triangleIndex);

            while (pendingTriangles.Count > 0)
            {
                int currentTriangleIndex = pendingTriangles.Dequeue();
                int triangleBaseIndex = currentTriangleIndex * 3;
                islandArea += ComputeUvTriangleArea(
                    uv[triangles[triangleBaseIndex]],
                    uv[triangles[triangleBaseIndex + 1]],
                    uv[triangles[triangleBaseIndex + 2]]);

                List<int> neighbors = triangleNeighbors[currentTriangleIndex];
                for (int neighborIndex = 0; neighborIndex < neighbors.Count; neighborIndex++)
                {
                    int adjacentTriangleIndex = neighbors[neighborIndex];
                    if (triangleIslandIndices[adjacentTriangleIndex] >= 0)
                    {
                        continue;
                    }

                    triangleIslandIndices[adjacentTriangleIndex] = islandCount;
                    pendingTriangles.Enqueue(adjacentTriangleIndex);
                }
            }

            islandAreas.Add(islandArea);
            islandCount++;
        }

        if (islandAreas.Count == 0)
        {
            error = "Unable to derive any UV islands from the selected mesh.";
            return false;
        }

        int outputSize = Mathf.Clamp(Mathf.ClosestPowerOfTwo(Mathf.Max(16, requestedSize)), 16, 4096);
        Color32[] islandColors = BuildDecal123IslandColors(islandAreas);
        Vector3[] meshVertices = IsMickeyEarsMesh(mesh) ? mesh.vertices : null;
        float bandHeight = meshVertices != null
            ? mesh.bounds.min.y + (mesh.bounds.size.y * MickeyEarsBandHeightFraction)
            : 0f;
        decalTexture = RasterizeDecal123Texture(uv, triangles, triangleIslandIndices, islandColors, outputSize, meshVertices, bandHeight);
        decalTexture.name = BuildDecal123FileName(mesh);
        return true;
    }

    private static bool IsMickeyEarsMesh(Mesh mesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        return !string.IsNullOrEmpty(assetPath) &&
            assetPath.IndexOf(MickeyEarsAssetPathSegment, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static List<int>[] BuildUvTriangleNeighbors(Vector2[] uv, int[] triangles, int triangleCount)
    {
        List<int>[] triangleNeighbors = new List<int>[triangleCount];
        Dictionary<UvEdgeKey, List<int>> edgeTriangles = new Dictionary<UvEdgeKey, List<int>>();

        for (int triangleIndex = 0; triangleIndex < triangleCount; triangleIndex++)
        {
            triangleNeighbors[triangleIndex] = new List<int>();
            int triangleBaseIndex = triangleIndex * 3;
            AddTriangleEdge(edgeTriangles, new UvEdgeKey(uv[triangles[triangleBaseIndex]], uv[triangles[triangleBaseIndex + 1]]), triangleIndex);
            AddTriangleEdge(edgeTriangles, new UvEdgeKey(uv[triangles[triangleBaseIndex + 1]], uv[triangles[triangleBaseIndex + 2]]), triangleIndex);
            AddTriangleEdge(edgeTriangles, new UvEdgeKey(uv[triangles[triangleBaseIndex + 2]], uv[triangles[triangleBaseIndex]]), triangleIndex);
        }

        foreach (List<int> connectedTriangles in edgeTriangles.Values)
        {
            for (int i = 0; i < connectedTriangles.Count; i++)
            {
                int currentTriangle = connectedTriangles[i];
                for (int j = i + 1; j < connectedTriangles.Count; j++)
                {
                    int otherTriangle = connectedTriangles[j];
                    AddUniqueTriangleNeighbor(triangleNeighbors[currentTriangle], otherTriangle);
                    AddUniqueTriangleNeighbor(triangleNeighbors[otherTriangle], currentTriangle);
                }
            }
        }

        return triangleNeighbors;
    }

    private static void AddTriangleEdge(Dictionary<UvEdgeKey, List<int>> edgeTriangles, UvEdgeKey edgeKey, int triangleIndex)
    {
        if (!edgeTriangles.TryGetValue(edgeKey, out List<int> connectedTriangles))
        {
            connectedTriangles = new List<int>();
            edgeTriangles.Add(edgeKey, connectedTriangles);
        }

        connectedTriangles.Add(triangleIndex);
    }

    private static void AddUniqueTriangleNeighbor(List<int> neighbors, int triangleIndex)
    {
        if (!neighbors.Contains(triangleIndex))
        {
            neighbors.Add(triangleIndex);
        }
    }

    private static float ComputeUvTriangleArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return Mathf.Abs(((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x))) * 0.5f;
    }

    private static Color32[] BuildDecal123IslandColors(List<float> islandAreas)
    {
        List<int> islandOrder = new List<int>(islandAreas.Count);
        for (int islandIndex = 0; islandIndex < islandAreas.Count; islandIndex++)
        {
            islandOrder.Add(islandIndex);
        }

        islandOrder.Sort((left, right) => islandAreas[right].CompareTo(islandAreas[left]));

        Color32[] islandColors = new Color32[islandAreas.Count];
        for (int orderIndex = 0; orderIndex < islandOrder.Count; orderIndex++)
        {
            islandColors[islandOrder[orderIndex]] = Decal123ChannelColors[orderIndex % Decal123ChannelColors.Length];
        }

        return islandColors;
    }

    private static Texture2D RasterizeDecal123Texture(Vector2[] uv, int[] triangles, int[] triangleIslandIndices, Color32[] islandColors, int size, Vector3[] meshVertices, float bandHeight)
    {
        int supersamplingScale = meshVertices == null
            ? 1
            : Mathf.Min(MickeyEarsSupersamplingScale, Mathf.Max(1, MickeyEarsMaxRasterSize / size));
        int rasterSize = size * supersamplingScale;
        Color32[] pixels = new Color32[rasterSize * rasterSize];
        Color32 backgroundColor = new Color32(0, 0, 0, 0);
        for (int pixelIndex = 0; pixelIndex < pixels.Length; pixelIndex++)
        {
            pixels[pixelIndex] = backgroundColor;
        }

        for (int triangleIndex = 0; triangleIndex < triangleIslandIndices.Length; triangleIndex++)
        {
            int triangleBaseIndex = triangleIndex * 3;
            Vector2 a = UvToPixelSpace(uv[triangles[triangleBaseIndex]], rasterSize);
            Vector2 b = UvToPixelSpace(uv[triangles[triangleBaseIndex + 1]], rasterSize);
            Vector2 c = UvToPixelSpace(uv[triangles[triangleBaseIndex + 2]], rasterSize);
            if (meshVertices != null)
            {
                RasterizeTriangleByHeight(
                    pixels,
                    rasterSize,
                    a,
                    b,
                    c,
                    meshVertices[triangles[triangleBaseIndex]].y,
                    meshVertices[triangles[triangleBaseIndex + 1]].y,
                    meshVertices[triangles[triangleBaseIndex + 2]].y,
                    bandHeight,
                    Decal123ChannelColors[0],
                    Decal123ChannelColors[2]);
            }
            else
            {
                RasterizeTriangle(pixels, rasterSize, a, b, c, islandColors[triangleIslandIndices[triangleIndex]]);
            }
        }

        FillSmallRasterizedGaps(pixels, rasterSize, 4 * supersamplingScale);
        if (meshVertices == null)
        {
            RemoveStrayRasterizedColors(pixels, size);
        }

        if (supersamplingScale > 1)
        {
            pixels = DownsampleRasterizedPixels(pixels, size, supersamplingScale);
        }

        DilateRasterizedColors(pixels, size, 10);

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.SetPixels32(pixels);
        texture.Apply(false, false);
        return texture;
    }

    private static Color32[] DownsampleRasterizedPixels(Color32[] sourcePixels, int size, int scale)
    {
        int sourceSize = size * scale;
        int samplesPerPixel = scale * scale;
        Color32[] downsampledPixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int red = 0;
                int green = 0;
                int blue = 0;
                int alpha = 0;
                for (int sampleY = 0; sampleY < scale; sampleY++)
                {
                    int rowOffset = ((y * scale + sampleY) * sourceSize) + (x * scale);
                    for (int sampleX = 0; sampleX < scale; sampleX++)
                    {
                        Color32 sample = sourcePixels[rowOffset + sampleX];
                        red += sample.r;
                        green += sample.g;
                        blue += sample.b;
                        alpha += sample.a;
                    }
                }

                int pixelIndex = (y * size) + x;
                downsampledPixels[pixelIndex] = new Color32(
                    (byte)((red + samplesPerPixel / 2) / samplesPerPixel),
                    (byte)((green + samplesPerPixel / 2) / samplesPerPixel),
                    (byte)((blue + samplesPerPixel / 2) / samplesPerPixel),
                    (byte)((alpha + samplesPerPixel / 2) / samplesPerPixel));
            }
        }

        return downsampledPixels;
    }

    private static void RasterizeTriangleByHeight(Color32[] pixels, int size, Vector2 a, Vector2 b, Vector2 c, float heightA, float heightB, float heightC, float splitHeight, Color32 lowerColor, Color32 upperColor)
    {
        RasterizeTriangle(pixels, size, a, b, c, lowerColor, upperColor, heightA, heightB, heightC, splitHeight);
    }

    private static void DilateRasterizedColors(Color32[] pixels, int size, int radius)
    {
        Color32[] sourcePixels = new Color32[pixels.Length];
        Array.Copy(pixels, sourcePixels, pixels.Length);
        int radiusSquared = radius * radius;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int pixelIndex = (y * size) + x;
                if (sourcePixels[pixelIndex].a != 0)
                {
                    continue;
                }

                Color32 nearestColor = sourcePixels[pixelIndex];
                int nearestDistanceSquared = radiusSquared + 1;
                int minY = Mathf.Max(0, y - radius);
                int maxY = Mathf.Min(size - 1, y + radius);
                int minX = Mathf.Max(0, x - radius);
                int maxX = Mathf.Min(size - 1, x + radius);
                for (int neighborY = minY; neighborY <= maxY; neighborY++)
                {
                    for (int neighborX = minX; neighborX <= maxX; neighborX++)
                    {
                        int offsetX = neighborX - x;
                        int offsetY = neighborY - y;
                        int distanceSquared = (offsetX * offsetX) + (offsetY * offsetY);
                        if (distanceSquared > radiusSquared || distanceSquared >= nearestDistanceSquared)
                        {
                            continue;
                        }

                        Color32 neighborColor = sourcePixels[(neighborY * size) + neighborX];
                        if (neighborColor.a == 0)
                        {
                            continue;
                        }

                        nearestColor = neighborColor;
                        nearestColor.a = 0;
                        nearestDistanceSquared = distanceSquared;
                    }
                }

                if (nearestDistanceSquared <= radiusSquared)
                {
                    pixels[pixelIndex] = nearestColor;
                }
            }
        }
    }

    private static void RemoveStrayRasterizedColors(Color32[] pixels, int size)
    {
        Color32[] sourcePixels = new Color32[pixels.Length];
        for (int pass = 0; pass < 2; pass++)
        {
            Array.Copy(pixels, sourcePixels, pixels.Length);
            for (int y = 1; y < size - 1; y++)
            {
                for (int x = 1; x < size - 1; x++)
                {
                    int pixelIndex = (y * size) + x;
                    Color32 currentColor = sourcePixels[pixelIndex];
                    if (currentColor.a == 0)
                    {
                        continue;
                    }

                    Color32 majorityColor = currentColor;
                    int majorityCount = 0;
                    for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
                    {
                        for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
                        {
                            if (neighborX == x && neighborY == y)
                            {
                                continue;
                            }

                            Color32 neighborColor = sourcePixels[(neighborY * size) + neighborX];
                            if (neighborColor.a == 0)
                            {
                                continue;
                            }

                            int neighborCount = CountNeighborColor(sourcePixels, size, neighborX, neighborY, neighborColor);
                            if (neighborCount > majorityCount)
                            {
                                majorityColor = neighborColor;
                                majorityCount = neighborCount;
                            }
                        }
                    }

                    if (majorityCount >= 5 && !AreSameColor(currentColor, majorityColor))
                    {
                        pixels[pixelIndex] = majorityColor;
                    }
                }
            }
        }
    }

    private static void FillSmallRasterizedGaps(Color32[] pixels, int size, int maxGapSize)
    {
        Color32[] sourcePixels = new Color32[pixels.Length];
        Array.Copy(pixels, sourcePixels, pixels.Length);
        FillHorizontalRasterizedGaps(sourcePixels, pixels, size, maxGapSize);
        Array.Copy(pixels, sourcePixels, pixels.Length);
        FillVerticalRasterizedGaps(sourcePixels, pixels, size, maxGapSize);
    }

    private static void FillHorizontalRasterizedGaps(Color32[] sourcePixels, Color32[] pixels, int size, int maxGapSize)
    {
        for (int y = 0; y < size; y++)
        {
            int x = 0;
            while (x < size)
            {
                if (sourcePixels[(y * size) + x].a != 0)
                {
                    x++;
                    continue;
                }

                int gapStart = x;
                while (x < size && sourcePixels[(y * size) + x].a == 0)
                {
                    x++;
                }

                int gapSize = x - gapStart;
                if (gapStart > 0 && x < size && gapSize <= maxGapSize)
                {
                    Color32 leftColor = sourcePixels[(y * size) + gapStart - 1];
                    Color32 rightColor = sourcePixels[(y * size) + x];
                    if (AreSameColor(leftColor, rightColor))
                    {
                        for (int gapX = gapStart; gapX < x; gapX++)
                        {
                            pixels[(y * size) + gapX] = leftColor;
                        }
                    }
                }
            }
        }
    }

    private static void FillVerticalRasterizedGaps(Color32[] sourcePixels, Color32[] pixels, int size, int maxGapSize)
    {
        for (int x = 0; x < size; x++)
        {
            int y = 0;
            while (y < size)
            {
                if (sourcePixels[(y * size) + x].a != 0)
                {
                    y++;
                    continue;
                }

                int gapStart = y;
                while (y < size && sourcePixels[(y * size) + x].a == 0)
                {
                    y++;
                }

                int gapSize = y - gapStart;
                if (gapStart > 0 && y < size && gapSize <= maxGapSize)
                {
                    Color32 topColor = sourcePixels[((gapStart - 1) * size) + x];
                    Color32 bottomColor = sourcePixels[(y * size) + x];
                    if (AreSameColor(topColor, bottomColor))
                    {
                        for (int gapY = gapStart; gapY < y; gapY++)
                        {
                            pixels[(gapY * size) + x] = topColor;
                        }
                    }
                }
            }
        }
    }

    private static int CountNeighborColor(Color32[] pixels, int size, int centerX, int centerY, Color32 color)
    {
        int count = 0;
        for (int y = centerY - 1; y <= centerY + 1; y++)
        {
            if (y < 0 || y >= size)
            {
                continue;
            }

            for (int x = centerX - 1; x <= centerX + 1; x++)
            {
                if (x < 0 || x >= size)
                {
                    continue;
                }

                if (AreSameColor(pixels[(y * size) + x], color))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool AreSameColor(Color32 left, Color32 right)
    {
        return left.a != 0 && right.a != 0 && left.r == right.r && left.g == right.g && left.b == right.b;
    }

    private static Vector2 UvToPixelSpace(Vector2 uv, int size)
    {
        float maxCoordinate = size - 1f;
        return new Vector2(uv.x * maxCoordinate, uv.y * maxCoordinate);
    }

    private static void RasterizeTriangle(Color32[] pixels, int size, Vector2 a, Vector2 b, Vector2 c, Color32 color)
    {
        RasterizeTriangle(pixels, size, a, b, c, color, color, 0f, 0f, 0f, 0f);
    }

    private static void RasterizeTriangle(Color32[] pixels, int size, Vector2 a, Vector2 b, Vector2 c, Color32 lowerColor, Color32 upperColor, float heightA, float heightB, float heightC, float splitHeight)
    {
        float minX = Mathf.Min(a.x, Mathf.Min(b.x, c.x));
        float maxX = Mathf.Max(a.x, Mathf.Max(b.x, c.x));
        float minY = Mathf.Min(a.y, Mathf.Min(b.y, c.y));
        float maxY = Mathf.Max(a.y, Mathf.Max(b.y, c.y));

        int clampedMinX = Mathf.Clamp(Mathf.FloorToInt(minX), 0, size - 1);
        int clampedMaxX = Mathf.Clamp(Mathf.CeilToInt(maxX), 0, size - 1);
        int clampedMinY = Mathf.Clamp(Mathf.FloorToInt(minY), 0, size - 1);
        int clampedMaxY = Mathf.Clamp(Mathf.CeilToInt(maxY), 0, size - 1);

        float signedArea = ComputeEdgeValue(a, b, c);
        if (Mathf.Approximately(signedArea, 0f))
        {
            return;
        }

        bool splitByHeight = !lowerColor.Equals(upperColor);
        for (int y = clampedMinY; y <= clampedMaxY; y++)
        {
            for (int x = clampedMinX; x <= clampedMaxX; x++)
            {
                Vector2 samplePoint = new Vector2(x + 0.5f, y + 0.5f);
                float edge0 = ComputeEdgeValue(b, c, samplePoint);
                float edge1 = ComputeEdgeValue(c, a, samplePoint);
                float edge2 = ComputeEdgeValue(a, b, samplePoint);

                bool isInside = signedArea > 0f
                    ? edge0 >= 0f && edge1 >= 0f && edge2 >= 0f
                    : edge0 <= 0f && edge1 <= 0f && edge2 <= 0f;
                int pixelIndex = (y * size) + x;
                if (isInside)
                {
                    if (splitByHeight)
                    {
                        float interpolatedHeight = ((edge0 * heightA) + (edge1 * heightB) + (edge2 * heightC)) / signedArea;
                        pixels[pixelIndex] = interpolatedHeight < splitHeight ? lowerColor : upperColor;
                    }
                    else
                    {
                        pixels[pixelIndex] = lowerColor;
                    }
                }
            }
        }
    }

    private static float ComputeEdgeValue(Vector2 a, Vector2 b, Vector2 point)
    {
        return ((point.x - a.x) * (b.y - a.y)) - ((point.y - a.y) * (b.x - a.x));
    }

    private static Texture GetUvDomainTexture(Material material, EquipmentShaderParams shaderParams)
    {
        if (material.shader.name == EquipmentShaderParams.EQUIPMENT_PREVIEW_SHADER_NAME ||
            material.shader.name == EquipmentShaderParams.EQUIPMENT_BAKE_SHADER_NAME)
        {
            if (shaderParams.Decals123OpacityTexture != null)
            {
                return shaderParams.Decals123OpacityTexture;
            }
        }
        else
        {
            if (shaderParams.BodyColorsMaskTexture != null)
            {
                return shaderParams.BodyColorsMaskTexture;
            }
        }

        return GetFirstAvailableTexture(shaderParams);
    }

    private static Texture GetFirstAvailableTexture(EquipmentShaderParams shaderParams)
    {
        Texture[] textures =
        {
            shaderParams.DiffuseTexture,
            shaderParams.Decals123OpacityTexture,
            shaderParams.BodyColorsMaskTexture,
            shaderParams.DetailMatcapmaslEmissiveTex,
            shaderParams.DecalRed1Texture,
            shaderParams.DecalGreen2Texture,
            shaderParams.DecalBlue3Texture,
            shaderParams.DecalRed4Texture,
            shaderParams.DecalGreen5Texture,
            shaderParams.DecalBlue6Texture
        };

        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
            {
                return textures[i];
            }
        }

        return null;
    }

    private static int GetSuggestedOutputSize(EquipmentShaderParams shaderParams)
    {
        int maxSize = 0;

        Texture[] textures =
        {
            shaderParams.DiffuseTexture,
            shaderParams.Decals123OpacityTexture,
            shaderParams.BodyColorsMaskTexture,
            shaderParams.DetailMatcapmaslEmissiveTex,
            shaderParams.DecalRed1Texture,
            shaderParams.DecalGreen2Texture,
            shaderParams.DecalBlue3Texture,
            shaderParams.DecalRed4Texture,
            shaderParams.DecalGreen5Texture,
            shaderParams.DecalBlue6Texture
        };

        for (int i = 0; i < textures.Length; i++)
        {
            Texture texture = textures[i];
            if (texture == null)
            {
                continue;
            }

            maxSize = Mathf.Max(maxSize, texture.width, texture.height);
        }

        if (maxSize <= 0)
        {
            maxSize = DefaultOutputSize;
        }

        return Mathf.ClosestPowerOfTwo(Mathf.Clamp(maxSize, 16, 4096));
    }

    private int GetSelectedOutputSize()
    {
        int selectedValue = OutputSizeValues[Mathf.Clamp(outputSizeIndex, 0, OutputSizeValues.Length - 1)];
        if (selectedValue > 0)
        {
            return selectedValue;
        }

        if (TryReadMaterialParams(sourceMaterial, out EquipmentShaderParams shaderParams, out _))
        {
            return GetSuggestedOutputSize(shaderParams);
        }

        return DefaultOutputSize;
    }

    private void DestroyPreviewTexture()
    {
        if (previewTexture == null)
        {
            return;
        }

        if (!AssetDatabase.Contains(previewTexture))
        {
            DestroyImmediate(previewTexture);
        }

        previewTexture = null;
    }
}
