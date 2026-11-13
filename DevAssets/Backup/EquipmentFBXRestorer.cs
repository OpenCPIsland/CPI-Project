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

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                EditorUtility.DisplayProgressBar("Equipment FBX Restorer", $"[{filename}] Building mesh {i + 1} of {assets.Count}: {asset.SkinnedMesh.Name}...", 0.1f + (float)i / assets.Count * 0.2f);

                if (asset.SkinnedMesh.Mesh == null)
                {
                    Debug.LogWarning($"Asset {asset.name} has no mesh data");
                    continue;
                }

                if (exportedMeshes.Contains(asset.SkinnedMesh.Mesh))
                {
                    Debug.Log($"Skipping duplicate mesh '{asset.SkinnedMesh.Name}' in {asset.name} - already added to FBX.");
                    continue;
                }

                exportedMeshes.Add(asset.SkinnedMesh.Mesh);

                GameObject meshObj = new GameObject(asset.SkinnedMesh.Name);
                meshObj.transform.SetParent(tempRoot.transform);

                SkinnedMeshRenderer renderer = meshObj.AddComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = asset.SkinnedMesh.Mesh;

                if (asset.SkinnedMesh.BoneNames != null && asset.SkinnedMesh.BoneNames.Length > 0)
                    SetupBones(meshObj, asset);

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

    private static void SetupBones(GameObject meshObj, EquipmentViewDefinition asset)
    {
        SkinnedMeshRenderer renderer = meshObj.GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        string[] boneNames = asset.SkinnedMesh.BoneNames;
        string rootBoneName = asset.SkinnedMesh.RootBoneName;
        Mesh mesh = asset.SkinnedMesh.Mesh;
        Matrix4x4[] bindPoses = mesh.bindposes;

        Transform[] bones = new Transform[boneNames.Length];
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

        for (int i = 0; i < boneNames.Length; i++)
        {
            GameObject boneObj = new GameObject(boneNames[i]);
            boneObj.transform.SetParent(meshObj.transform, false);
            bones[i] = boneObj.transform;
            boneMap[boneNames[i]] = boneObj.transform;
        }

        for (int i = 0; i < bones.Length && i < bindPoses.Length; i++)
        {
            Matrix4x4 worldMatrix = bindPoses[i].inverse;

            bones[i].localPosition = new Vector3(
                worldMatrix.m03,
                worldMatrix.m13,
                worldMatrix.m23
            );

            bones[i].localRotation = worldMatrix.rotation;

            bones[i].localScale = new Vector3(
                new Vector3(worldMatrix.m00, worldMatrix.m10, worldMatrix.m20).magnitude,
                new Vector3(worldMatrix.m01, worldMatrix.m11, worldMatrix.m21).magnitude,
                new Vector3(worldMatrix.m02, worldMatrix.m12, worldMatrix.m22).magnitude
            );
        }

        renderer.bones = bones;

        if (!string.IsNullOrEmpty(rootBoneName) && boneMap.TryGetValue(rootBoneName, out Transform rootBone))
            renderer.rootBone = rootBone;
        else if (bones.Length > 0)
            renderer.rootBone = bones[0];
    }
}