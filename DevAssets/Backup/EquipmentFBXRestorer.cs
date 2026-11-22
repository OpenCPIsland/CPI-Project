using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using ClubPenguin;
using ClubPenguin.Avatar;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.Formats.Fbx.Exporter;
#endif

public class EquipmentFBXRestorer : EditorWindow
{
    private const string EQUIPMENT_ASSET_PATH = "Assets/Game/ItemCustomizer/Resources/Definitions/Equipment";
    private const string TEMPLATE_DEFINITIONS_PATH = "Assets/Game/ItemCustomizer/Resources/Definitions/Equipment/TemplateDefinitions";
    private const string REFERENCE_SKELETON_PATH = "Assets/Game/ArtAssets/Avatar/Penguin/Penguin.fbx";

    private string costumeName = "mikecostume";

    [MenuItem("Project/Tools/Editor/Equipment FBX Restorer")]
    public static void ShowWindow()
    {
        GetWindow<EquipmentFBXRestorer>("Equipment FBX Restorer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Equipment FBX Restorer", EditorStyles.boldLabel);
        costumeName = EditorGUILayout.TextField("Equipment Name:", costumeName);

        if (GUILayout.Button("Export FBX Assets", GUILayout.Height(30)))
        {
            if (!string.IsNullOrEmpty(costumeName))
                ExportCostumeFBX(costumeName);
            else
                EditorUtility.DisplayDialog("Error", "Please enter an equipment name", "OK");
        }

        if (GUILayout.Button("Restore All Equipment", GUILayout.Height(30)))
        {
            Debug.Log("Restore All Equipment button pressed");
            RestoreAllEquipment();
        }
    }

    private static void ExportCostumeFBX(string targetCostume)
    {
        try
        {
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Searching for assets...", 0f);

            Debug.Log($"Starting {targetCostume} FBX export...");

            var allAssets = AssetDatabase.FindAssets($"{targetCostume}", new[] { EQUIPMENT_ASSET_PATH });

            if (allAssets.Length == 0)
            {
                Debug.LogError($"No assets found matching '{targetCostume}' in {EQUIPMENT_ASSET_PATH}");
                return;
            }

            var lod0Assets = new List<EquipmentViewDefinition>();
            var lod1Assets = new List<EquipmentViewDefinition>();

            for (int i = 0; i < allAssets.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Loading asset {i + 1} of {allAssets.Length}...", (float)i / allAssets.Length * 0.1f);

                var assetPath = AssetDatabase.GUIDToAssetPath(allAssets[i]);
                var assetName = Path.GetFileNameWithoutExtension(assetPath);
                string equipmentName = ExtractEquipmentName(assetName);

                if (string.IsNullOrEmpty(equipmentName) || !equipmentName.Equals(targetCostume, System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<EquipmentViewDefinition>(assetPath);
                if (asset == null)
                    continue;

                int lodType = -1;
                if (assetName.EndsWith("0LOD", System.StringComparison.OrdinalIgnoreCase))
                    lodType = 0;
                else if (assetName.EndsWith("1LOD", System.StringComparison.OrdinalIgnoreCase))
                    lodType = 1;
                else if (assetName.EndsWith("_0LOD", System.StringComparison.OrdinalIgnoreCase))
                    lodType = 0;
                else if (assetName.EndsWith("_1LOD", System.StringComparison.OrdinalIgnoreCase))
                    lodType = 1;

                if (lodType == 0)
                    lod0Assets.Add(asset);
                else if (lodType == 1)
                    lod1Assets.Add(asset);

                Debug.Log($"Found asset: {assetName}");
            }

            Debug.Log($"Found {lod0Assets.Count} 0LOD assets and {lod1Assets.Count} 1LOD assets");

            if (lod0Assets.Count > 0)
            {
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Exporting {targetCostume}_0LOD...", 0.1f);
                ExportLODFBX(lod0Assets, $"{targetCostume}_0LOD");
            }

            if (lod1Assets.Count > 0)
            {
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Exporting {targetCostume}_1LOD...", 0.55f);
                ExportLODFBX(lod1Assets, $"{targetCostume}_1LOD");
            }

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Done!", 1f);
            Debug.Log($"{targetCostume} FBX export completed!");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void ExportLODFBX(List<EquipmentViewDefinition> assets, string filename)
    {
        if (assets.Count == 0)
        {
            Debug.LogError("No assets provided for export");
            return;
        }

        string meshAssetPath = AssetDatabase.GetAssetPath(assets[0].SkinnedMesh.Mesh);
        string meshDirectory = Path.GetDirectoryName(meshAssetPath);
        string exportPath = Path.Combine(meshDirectory, filename + ".fbx");
        string temporaryExportPath = Path.Combine(meshDirectory, filename + "_replacement.fbx");
        string existingGuid = GetMetaGuid(exportPath + ".meta");

        if (File.Exists(temporaryExportPath))
            AssetDatabase.DeleteAsset(temporaryExportPath);

        GameObject tempRoot = new GameObject(filename);

        try
        {
            int meshCount = 0;
            HashSet<Mesh> exportedMeshes = new HashSet<Mesh>();
            HashSet<string> exportedPieces = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Transform> skeleton = BuildSkeleton(tempRoot.transform);

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Building mesh {i + 1} of {assets.Count}: {asset.SkinnedMesh.Name}...", 0.1f + (float)i / assets.Count * 0.2f);

                if (asset.SkinnedMesh.Mesh == null)
                {
                    Debug.LogWarning($"Asset {asset.name} has no mesh data");
                    continue;
                }

                if (exportedMeshes.Contains(asset.SkinnedMesh.Mesh) || exportedPieces.Contains(asset.SkinnedMesh.Name))
                {
                    Debug.Log($"Skipping duplicate piece '{asset.SkinnedMesh.Name}' in {asset.name} - already added to FBX.");
                    continue;
                }

                exportedMeshes.Add(asset.SkinnedMesh.Mesh);
                exportedPieces.Add(asset.SkinnedMesh.Name);

                GameObject meshObj = new GameObject(asset.SkinnedMesh.Name);
                meshObj.transform.SetParent(tempRoot.transform);

                SkinnedMeshRenderer renderer = meshObj.AddComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = asset.SkinnedMesh.Mesh;

                if (asset.SkinnedMesh.BoneNames != null && asset.SkinnedMesh.BoneNames.Length > 0)
                    SetupBones(meshObj, asset, tempRoot.transform, skeleton);

                meshCount++;
                Debug.Log($"Added mesh: {asset.SkinnedMesh.Name}");
            }

            if (meshCount == 0)
            {
                Debug.LogError("No meshes were successfully added to the export");
                return;
            }

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Exporting FBX...", 0.3f);

#if UNITY_2020_2_OR_NEWER
            var options = new ExportModelOptions { ExportFormat = ExportFormat.Binary };
            ModelExporter.ExportObject(temporaryExportPath, tempRoot, options);

            if (!File.Exists(temporaryExportPath))
                throw new IOException($"FBX exporter did not create the temporary file: {temporaryExportPath}");

            AssetDatabase.Refresh();

            if (File.Exists(exportPath))
            {
                if (!AssetDatabase.DeleteAsset(exportPath))
                    throw new IOException($"Could not remove the existing FBX: {exportPath}");
            }

            string moveError = AssetDatabase.MoveAsset(temporaryExportPath, exportPath);
            if (!string.IsNullOrEmpty(moveError))
                throw new IOException($"Could not move replacement FBX into place: {moveError}");

            if (!string.IsNullOrEmpty(existingGuid))
                SetMetaGuid(exportPath + ".meta", existingGuid);

            Debug.Log($"Exported FBX to: {exportPath}");
#else
            Debug.LogError("FBX Exporter SDK not available. Requires Unity 2020.2 or newer.");
            return;
#endif

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Refreshing asset database...", 0.35f);
            AssetDatabase.Refresh();

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Applying import settings...", 0.4f);

            ModelImporter importer = AssetImporter.GetAtPath(exportPath) as ModelImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.generateSecondaryUV = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;

                SerializedObject serializedImporter = new SerializedObject(importer);
                SerializedProperty prop = serializedImporter.GetIterator();
                bool foundLegacyNormals = false;
                while (prop.NextVisible(true))
                {
                    if (prop.name.ToLower().Contains("legacycomputeallnormals") ||
                        prop.name.ToLower().Contains("legacyblendshape"))
                    {
                        prop.boolValue = true;
                        foundLegacyNormals = true;
                        Debug.Log($"Set legacy blend shape normals via property: {prop.name}");
                        break;
                    }
                }

                if (!foundLegacyNormals)
                    Debug.LogWarning("Could not find legacy blend shape normals property on ModelImporter.");

                serializedImporter.ApplyModifiedProperties();
                importer.SaveAndReimport();
                Debug.Log($"Import settings applied to: {exportPath}");
            }
            else
            {
                Debug.LogWarning($"Could not find ModelImporter for: {exportPath}");
            }

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Remapping definition meshes...", 0.5f);
            RemapDefinitionMeshes(assets, exportPath);
        }
        finally
        {
            DestroyImmediate(tempRoot);
        }
    }

    private static string GetMetaGuid(string metaPath)
    {
        if (!File.Exists(metaPath))
            return null;

        string[] lines = File.ReadAllLines(metaPath);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.StartsWith("guid:", System.StringComparison.Ordinal))
                return line.Substring("guid:".Length).Trim();
        }

        return null;
    }

    private static void SetMetaGuid(string metaPath, string guid)
    {
        if (!File.Exists(metaPath))
            return;

        string[] lines = File.ReadAllLines(metaPath);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart().StartsWith("guid:", System.StringComparison.Ordinal))
            {
                lines[i] = "guid: " + guid;
                File.WriteAllLines(metaPath, lines);
                AssetDatabase.Refresh();
                return;
            }
        }
    }

    private static void RemapDefinitionMeshes(List<EquipmentViewDefinition> assets, string exportPath)
    {
        EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Loading FBX sub-assets...", 0.52f);

        Object[] fbxSubAssets = AssetDatabase.LoadAllAssetsAtPath(exportPath);

        Dictionary<string, Mesh> fbxMeshByName = new Dictionary<string, Mesh>();
        foreach (var obj in fbxSubAssets)
        {
            if (obj is Mesh mesh)
            {
                fbxMeshByName[mesh.name] = mesh;
                Debug.Log($"FBX sub asset mesh found: {mesh.name}");
            }
        }

        List<string> meshPathsToDelete = new List<string>();
        Dictionary<Mesh, Mesh> oldToNewMesh = new Dictionary<Mesh, Mesh>();

        for (int i = 0; i < assets.Count; i++)
        {
            var asset = assets[i];
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Remapping definition {i + 1} of {assets.Count}: {asset.name}...", 0.52f + (float)i / assets.Count * 0.08f);

            if (asset.SkinnedMesh.Mesh == null)
                continue;

            string targetName = asset.SkinnedMesh.Name;
            Mesh oldMesh = asset.SkinnedMesh.Mesh;
            Mesh newMesh = null;

            if (oldToNewMesh.TryGetValue(oldMesh, out Mesh cached))
            {
                newMesh = cached;
                Debug.Log($"Reusing already resolved FBX mesh '{newMesh.name}' for {asset.name}");
            }
            else
            {
                if (!fbxMeshByName.TryGetValue(targetName, out newMesh))
                {
                    foreach (var kvp in fbxMeshByName)
                    {
                        if (kvp.Key.Contains(targetName) || targetName.Contains(kvp.Key))
                        {
                            newMesh = kvp.Value;
                            Debug.Log($"Fuzzy matched FBX mesh '{kvp.Key}' for definition piece '{targetName}'");
                            break;
                        }
                    }
                }
            }

            if (newMesh == null)
            {
                Debug.LogWarning($"No matching FBX mesh found for definition piece: {targetName}");
                continue;
            }

            string oldMeshPath = AssetDatabase.GetAssetPath(oldMesh);

            SerializedObject so = new SerializedObject(asset);
            SerializedProperty iterator = so.GetIterator();
            bool replaced = false;

            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType == SerializedPropertyType.ObjectReference &&
                    iterator.objectReferenceValue == oldMesh)
                {
                    iterator.objectReferenceValue = newMesh;
                    replaced = true;
                    Debug.Log($"Replaced mesh reference in {asset.name}: '{oldMesh.name}' -> '{newMesh.name}'");
                    break;
                }
            }

            if (replaced)
            {
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);

                if (!oldToNewMesh.ContainsKey(oldMesh))
                {
                    oldToNewMesh[oldMesh] = newMesh;

                    if (!string.IsNullOrEmpty(oldMeshPath) && oldMeshPath != exportPath)
                    {
                        bool isMainAsset = AssetDatabase.IsMainAsset(oldMesh);
                        bool isSubAssetOfStandaloneFile = AssetDatabase.IsSubAsset(oldMesh) &&
                            (oldMeshPath.EndsWith(".asset") || oldMeshPath.EndsWith(".mesh"));

                        if (isMainAsset || isSubAssetOfStandaloneFile)
                        {
                            if (!meshPathsToDelete.Contains(oldMeshPath))
                                meshPathsToDelete.Add(oldMeshPath);
                        }
                        else
                        {
                            Debug.LogWarning($"Old mesh '{oldMesh.name}' is a sub asset of '{oldMeshPath}' - skipping deletion to avoid removing the parent asset.");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Could not find mesh property referencing '{oldMesh.name}' in {asset.name}");
            }
        }

        if (oldToNewMesh.Count > 0)
        {
            ReplaceInPrefabsAndScriptableObjects(oldToNewMesh);
            ReplaceInScenes(oldToNewMesh);
        }

        EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Saving assets...", 0.97f);
        AssetDatabase.SaveAssets();

        for (int i = 0; i < meshPathsToDelete.Count; i++)
        {
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Deleting old mesh {i + 1} of {meshPathsToDelete.Count}...", 0.97f + (float)i / meshPathsToDelete.Count * 0.02f);
            bool deleted = AssetDatabase.DeleteAsset(meshPathsToDelete[i]);
            Debug.Log(deleted ? $"Deleted old mesh asset: {meshPathsToDelete[i]}" : $"Failed to delete: {meshPathsToDelete[i]}");
        }

        EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Refreshing asset database...", 0.99f);
        AssetDatabase.Refresh();
        Debug.Log("Definition mesh remapping complete.");
    }

    private static void ReplaceInPrefabsAndScriptableObjects(Dictionary<Mesh, Mesh> oldToNewMesh)
    {
        int totalReplaced = 0;

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Scanning prefabs {i + 1} of {prefabGuids.Length}...", 0.6f + (float)i / prefabGuids.Length * 0.1f);

            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                continue;

            bool dirty = false;

            foreach (var smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (smr.sharedMesh != null && oldToNewMesh.TryGetValue(smr.sharedMesh, out Mesh replacement))
                {
                    SerializedObject so = new SerializedObject(smr);
                    SerializedProperty meshProp = so.FindProperty("m_Mesh");
                    if (meshProp != null)
                    {
                        meshProp.objectReferenceValue = replacement;
                        so.ApplyModifiedPropertiesWithoutUndo();
                        dirty = true;
                        totalReplaced++;
                    }
                }
            }

            foreach (var mf in prefab.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.sharedMesh != null && oldToNewMesh.TryGetValue(mf.sharedMesh, out Mesh replacement))
                {
                    SerializedObject so = new SerializedObject(mf);
                    SerializedProperty meshProp = so.FindProperty("m_Mesh");
                    if (meshProp != null)
                    {
                        meshProp.objectReferenceValue = replacement;
                        so.ApplyModifiedPropertiesWithoutUndo();
                        dirty = true;
                        totalReplaced++;
                    }
                }
            }

            if (dirty)
                EditorUtility.SetDirty(prefab);
        }

        string[] soGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" });
        for (int i = 0; i < soGuids.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Scanning ScriptableObjects {i + 1} of {soGuids.Length}...", 0.7f + (float)i / soGuids.Length * 0.1f);

            string path = AssetDatabase.GUIDToAssetPath(soGuids[i]);
            Object[] allObjs = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var obj in allObjs)
            {
                if (obj == null)
                    continue;

                SerializedObject so = new SerializedObject(obj);
                SerializedProperty prop = so.GetIterator();
                bool dirty = false;

                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference &&
                        prop.objectReferenceValue is Mesh existingMesh &&
                        oldToNewMesh.TryGetValue(existingMesh, out Mesh replacement))
                    {
                        prop.objectReferenceValue = replacement;
                        dirty = true;
                        totalReplaced++;
                    }
                }

                if (dirty)
                {
                    so.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }

    private static void ReplaceInScenes(Dictionary<Mesh, Mesh> oldToNewMesh)
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int totalReplaced = 0;
        int scenesProcessed = 0;

        HashSet<string> oldMeshPaths = new HashSet<string>();
        foreach (var oldMesh in oldToNewMesh.Keys)
        {
            string p = AssetDatabase.GetAssetPath(oldMesh);
            if (!string.IsNullOrEmpty(p))
                oldMeshPaths.Add(p);
        }

        for (int i = 0; i < sceneGuids.Length; i++)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Checking scene {i + 1} of {sceneGuids.Length}: {Path.GetFileName(scenePath)}...", 0.8f + (float)i / sceneGuids.Length * 0.15f);

            string[] deps = AssetDatabase.GetDependencies(scenePath, true);
            bool hasReference = false;
            foreach (var dep in deps)
            {
                if (oldMeshPaths.Contains(dep))
                {
                    hasReference = true;
                    break;
                }
            }

            if (!hasReference)
                continue;

            EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Processing scene: {Path.GetFileName(scenePath)}...", 0.8f + (float)i / sceneGuids.Length * 0.15f);

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            bool sceneDirty = false;
            scenesProcessed++;

            foreach (var go in scene.GetRootGameObjects())
            {
                foreach (var smr in go.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (smr.sharedMesh != null && oldToNewMesh.TryGetValue(smr.sharedMesh, out Mesh replacement))
                    {
                        SerializedObject so = new SerializedObject(smr);
                        SerializedProperty meshProp = so.FindProperty("m_Mesh");
                        if (meshProp != null)
                        {
                            meshProp.objectReferenceValue = replacement;
                            so.ApplyModifiedPropertiesWithoutUndo();
                            EditorUtility.SetDirty(smr);
                            sceneDirty = true;
                            totalReplaced++;
                        }
                    }
                }

                foreach (var mf in go.GetComponentsInChildren<MeshFilter>(true))
                {
                    if (mf.sharedMesh != null && oldToNewMesh.TryGetValue(mf.sharedMesh, out Mesh replacement))
                    {
                        SerializedObject so = new SerializedObject(mf);
                        SerializedProperty meshProp = so.FindProperty("m_Mesh");
                        if (meshProp != null)
                        {
                            meshProp.objectReferenceValue = replacement;
                            so.ApplyModifiedPropertiesWithoutUndo();
                            EditorUtility.SetDirty(mf);
                            sceneDirty = true;
                            totalReplaced++;
                        }
                    }
                }
            }

            if (sceneDirty)
                EditorSceneManager.SaveScene(scene);

            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static void RestoreAllEquipment()
    {
        try
        {
            string logPath = Path.Combine(Application.persistentDataPath, "EquipmentFBXRestorer_Log.txt");
            using (StreamWriter log = new StreamWriter(logPath, false))
            {
                log.WriteLine(" Equipment FBX Restorer Log: ");
                log.WriteLine($"Time: {System.DateTime.Now}");
                log.WriteLine();

                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Scanning TemplateDefinitions...", 0f);

                var templateGuids = AssetDatabase.FindAssets("t:TemplateDefinition", new[] { TEMPLATE_DEFINITIONS_PATH });

                if (templateGuids.Length == 0)
                {
                    log.WriteLine("ERROR: No TemplateDefinitions found.");
                    EditorUtility.DisplayDialog("Info", "No TemplateDefinitions found.", "OK");
                    return;
                }

                log.WriteLine($"Found {templateGuids.Length} TemplateDefinitions");
                log.WriteLine();

                HashSet<string> assetNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < templateGuids.Length; i++)
                {
                    EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Reading template {i + 1} of {templateGuids.Length}...", (float)i / templateGuids.Length * 0.2f);

                    string assetPath = AssetDatabase.GUIDToAssetPath(templateGuids[i]);
                    TemplateDefinition template = AssetDatabase.LoadAssetAtPath<TemplateDefinition>(assetPath);

                    if (template == null)
                    {
                        log.WriteLine($"WARNING: Could not load TemplateDefinition at {assetPath}");
                        continue;
                    }

                    if (string.IsNullOrEmpty(template.AssetName))
                    {
                        log.WriteLine($"WARNING: TemplateDefinition '{template.name}' has empty AssetName, skipping.");
                        continue;
                    }

                    log.WriteLine($"Found template: {template.name} -> AssetName: {template.AssetName}");
                    assetNames.Add(template.AssetName);
                }

                log.WriteLine();
                log.WriteLine($"Collected {assetNames.Count} unique AssetNames to process");
                log.WriteLine();

                List<string> equipmentsToProcess = new List<string>();

                foreach (string assetName in assetNames)
                {
                    string fbxPath0LOD = FindExpectedFBXPath(assetName, "0LOD");
                    string fbxPath1LOD = FindExpectedFBXPath(assetName, "1LOD");

                    bool needs0LOD = string.IsNullOrEmpty(fbxPath0LOD) || !File.Exists(fbxPath0LOD);
                    bool needs1LOD = string.IsNullOrEmpty(fbxPath1LOD) || !File.Exists(fbxPath1LOD);

                    log.WriteLine($"  {assetName}: needs0LOD={needs0LOD}, needs1LOD={needs1LOD}");

                    if (needs0LOD || needs1LOD)
                        equipmentsToProcess.Add(assetName);
                    else
                        log.WriteLine($"Skipping {assetName}: all FBX files already exist");
                }

                log.WriteLine();
                log.WriteLine($"Found {equipmentsToProcess.Count} equipment to process");
                log.WriteLine();

                for (int i = 0; i < equipmentsToProcess.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"Processing {i + 1} of {equipmentsToProcess.Count}: {equipmentsToProcess[i]}...", 0.2f + (float)i / equipmentsToProcess.Count * 0.8f);
                    log.WriteLine($"Processing: {equipmentsToProcess[i]}");
                    ExportCostumeFBX(equipmentsToProcess[i]);
                }

                log.WriteLine();
                log.WriteLine("All equipment restoration completed!");
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", "Done!", 1f);
                Debug.Log("All equipment restoration completed!");
                log.WriteLine($"Log saved to: {logPath}");
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static string FindExpectedFBXPath(string assetName, string lodSuffix)
    {
        var guids = AssetDatabase.FindAssets($"{assetName}", new[] { EQUIPMENT_ASSET_PATH });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string dir = Path.GetDirectoryName(path);
            string candidate = Path.Combine(dir, $"{assetName}_{lodSuffix}.fbx");
            if (!string.IsNullOrEmpty(dir))
                return candidate;
        }
        return null;
    }

    private static string ExtractEquipmentName(string definitionName)
    {
        string name = definitionName;

        if (name.Contains("__"))
            name = name.Substring(0, name.IndexOf("__"));

        if (name.EndsWith("0LOD", System.StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - 4).TrimEnd('_');
        else if (name.EndsWith("1LOD", System.StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - 4).TrimEnd('_');
        else if (name.EndsWith("_0LOD", System.StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - 5).TrimEnd('_');
        else if (name.EndsWith("_1LOD", System.StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - 5).TrimEnd('_');

        if (name.Contains("_"))
        {
            string[] parts = name.Split('_');
            return parts[0];
        }

        return name;
    }

    private static Dictionary<string, Transform> BuildSkeleton(Transform root)
    {
        Dictionary<string, Transform> map = new Dictionary<string, Transform>();
        GameObject reference = AssetDatabase.LoadAssetAtPath<GameObject>(REFERENCE_SKELETON_PATH);
        if (reference == null)
        {
            Debug.LogWarning($"Reference skeleton not found at {REFERENCE_SKELETON_PATH}; bones will be exported without a hierarchy.");
            return map;
        }

        Transform hips = FindDeep(reference.transform, "hips_jnt");
        Transform top = hips;
        if (top != null)
        {
            while (top.parent != null && top.parent != reference.transform)
                top = top.parent;
        }

        if (top == null)
        {
            Debug.LogWarning($"No hips_jnt in {REFERENCE_SKELETON_PATH}; bones will be exported without a hierarchy.");
            return map;
        }

        CopyBranch(top, root, map);
        return map;
    }

    private static void CopyBranch(Transform source, Transform parent, Dictionary<string, Transform> map)
    {
        if (source.GetComponent<Renderer>() != null)
            return;

        GameObject copy = new GameObject(source.name);
        copy.transform.SetParent(parent, false);
        copy.transform.localPosition = source.localPosition;
        copy.transform.localRotation = source.localRotation;
        copy.transform.localScale = source.localScale;

        if (!map.ContainsKey(source.name))
            map[source.name] = copy.transform;

        for (int i = 0; i < source.childCount; i++)
            CopyBranch(source.GetChild(i), copy.transform, map);
    }

    private static Transform FindDeep(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindDeep(parent.GetChild(i), name);
            if (found != null)
                return found;
        }

        return null;
    }

    private static int Depth(Transform t)
    {
        int depth = 0;
        while (t.parent != null)
        {
            depth++;
            t = t.parent;
        }
        return depth;
    }

    private static void SetupBones(GameObject meshObj, EquipmentViewDefinition asset, Transform root, Dictionary<string, Transform> skeleton)
    {
        SkinnedMeshRenderer renderer = meshObj.GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        string[] boneNames = asset.SkinnedMesh.BoneNames;
        string rootBoneName = asset.SkinnedMesh.RootBoneName;
        Mesh mesh = asset.SkinnedMesh.Mesh;
        Matrix4x4[] bindPoses = mesh.bindposes ?? new Matrix4x4[0];

        Transform[] bones;
        bool useBindPoses;

        if (bindPoses.Length == boneNames.Length)
        {
            bones = new Transform[boneNames.Length];
            for (int i = 0; i < boneNames.Length; i++)
                bones[i] = GetOrCreateBone(boneNames[i], asset, root, skeleton);
            useBindPoses = true;
        }
        else if (bindPoses.Length > 0 && skeleton.Count > 0)
        {
            bones = MatchBonesByBindPose(meshObj.transform, bindPoses, boneNames, skeleton, asset);
            useBindPoses = true;
        }
        else
        {
            Debug.LogWarning($"{asset.name}: mesh has no bind poses; using the reference skeleton pose for {boneNames.Length} bones.");
            bones = new Transform[boneNames.Length];
            for (int i = 0; i < boneNames.Length; i++)
                bones[i] = GetOrCreateBone(boneNames[i], asset, root, skeleton);
            useBindPoses = false;
        }

        if (useBindPoses)
        {
            int[] order = new int[bones.Length];
            for (int i = 0; i < order.Length; i++)
                order[i] = i;
            System.Array.Sort(order, (a, b) => Depth(bones[a]).CompareTo(Depth(bones[b])));

            Matrix4x4 meshToWorld = meshObj.transform.localToWorldMatrix;
            foreach (int i in order)
            {
                Matrix4x4 world = meshToWorld * bindPoses[i].inverse;
                Vector3 scale = new Vector3(
                    new Vector3(world.m00, world.m10, world.m20).magnitude,
                    new Vector3(world.m01, world.m11, world.m21).magnitude,
                    new Vector3(world.m02, world.m12, world.m22).magnitude);
                bones[i].position = new Vector3(world.m03, world.m13, world.m23);
                bones[i].rotation = world.rotation;
                Vector3 parentScale = bones[i].parent != null ? bones[i].parent.lossyScale : Vector3.one;
                bones[i].localScale = new Vector3(
                    parentScale.x != 0f ? scale.x / parentScale.x : scale.x,
                    parentScale.y != 0f ? scale.y / parentScale.y : scale.y,
                    parentScale.z != 0f ? scale.z / parentScale.z : scale.z);
            }
        }
        else
        {
            Matrix4x4[] poses = new Matrix4x4[bones.Length];
            for (int i = 0; i < bones.Length; i++)
                poses[i] = bones[i].worldToLocalMatrix * meshObj.transform.localToWorldMatrix;
            Mesh copy = Object.Instantiate(mesh);
            copy.name = mesh.name;
            copy.bindposes = poses;
            renderer.sharedMesh = copy;
        }

        renderer.bones = bones;

        Transform rootBone = null;
        if (!string.IsNullOrEmpty(rootBoneName))
        {
            for (int i = 0; i < bones.Length && rootBone == null; i++)
            {
                if (bones[i].name == rootBoneName)
                    rootBone = bones[i];
            }
            if (rootBone == null)
                skeleton.TryGetValue(rootBoneName, out rootBone);
        }
        if (rootBone == null && bones.Length > 0)
            rootBone = bones[0];
        renderer.rootBone = rootBone;
    }

    private static Transform GetOrCreateBone(string boneName, EquipmentViewDefinition asset, Transform root, Dictionary<string, Transform> skeleton)
    {
        if (skeleton.TryGetValue(boneName, out Transform bone))
            return bone;

        GameObject boneObj = new GameObject(boneName);
        boneObj.transform.SetParent(root, false);
        skeleton[boneName] = boneObj.transform;
        Debug.LogWarning($"{asset.name}: bone '{boneName}' is not in the reference skeleton; added it under the root.");
        return boneObj.transform;
    }

    private static Transform[] MatchBonesByBindPose(Transform meshTransform, Matrix4x4[] bindPoses, string[] boneNames, Dictionary<string, Transform> skeleton, EquipmentViewDefinition asset)
    {
        List<Transform> candidates = new List<Transform>();
        HashSet<string> wanted = new HashSet<string>(boneNames);
        foreach (var kvp in skeleton)
        {
            if (wanted.Contains(kvp.Key))
                candidates.Add(kvp.Value);
        }
        if (candidates.Count < bindPoses.Length)
        {
            candidates.Clear();
            candidates.AddRange(skeleton.Values);
        }

        Vector3[] bindPositions = new Vector3[bindPoses.Length];
        for (int i = 0; i < bindPoses.Length; i++)
            bindPositions[i] = meshTransform.TransformPoint(bindPoses[i].inverse.MultiplyPoint3x4(Vector3.zero));

        List<KeyValuePair<float, int>> pairs = new List<KeyValuePair<float, int>>();
        for (int i = 0; i < bindPositions.Length; i++)
        {
            for (int c = 0; c < candidates.Count; c++)
                pairs.Add(new KeyValuePair<float, int>((bindPositions[i] - candidates[c].position).sqrMagnitude, i * candidates.Count + c));
        }
        pairs.Sort((a, b) => a.Key.CompareTo(b.Key));

        Transform[] bones = new Transform[bindPoses.Length];
        HashSet<int> usedCandidates = new HashSet<int>();
        float worst = 0f;
        int assigned = 0;
        foreach (var pair in pairs)
        {
            int i = pair.Value / candidates.Count;
            int c = pair.Value % candidates.Count;
            if (bones[i] != null || usedCandidates.Contains(c))
                continue;
            bones[i] = candidates[c];
            usedCandidates.Add(c);
            worst = Mathf.Max(worst, Mathf.Sqrt(pair.Key));
            if (++assigned == bones.Length)
                break;
        }

        string[] matched = new string[bones.Length];
        for (int i = 0; i < bones.Length; i++)
            matched[i] = bones[i] != null ? bones[i].name : "<none>";
        Debug.Log($"{asset.name}: mesh is skinned to {bindPoses.Length} bones but the definition lists {boneNames.Length}; matched by bind pose position (worst offset {worst:F4}): {string.Join(", ", matched)}");
        if (worst > 0.05f)
            Debug.LogWarning($"{asset.name}: some bind poses are {worst:F4} units from the nearest reference bone; check the bone match above.");

        return bones;
    }
}