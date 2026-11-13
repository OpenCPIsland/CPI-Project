using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(GenerateLightingEventBatch))]
public sealed class GenerateLightingEventBatchEditor : Editor
{
    const string MenuFile = "Assets/Editor/GenerateLighting/GeneratedEventMenus.cs";
    const string GeneratedHeader = "using UnityEditor;\npublic static class GeneratedEventMenus\n{\n";
    const string GeneratedFooter = "}\n";
    static readonly string[] ScenePaths =
    {
        "Assets/Game/World/Scenes/Town.unity",
        "Assets/Game/World/Scenes/Boardwalk.unity",
        "Assets/Game/World/Scenes/MtBlizzard.unity"
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh Event Menu Items"))
            RefreshGeneratedMenus();
    }

    [MenuItem("Project/Events Lightmap Baking/Refresh Event Menu Items")]
    static void RefreshGeneratedMenus()
    {
        var entries = new List<Tuple<string, string, string>>();
        var openedScenes = new List<Scene>();
        try
        {
            foreach (var path in ScenePaths)
            {
                if (!File.Exists(path))
                    continue;
                var scene = SceneManager.GetSceneByPath(path);
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    openedScenes.Add(scene);
                }
                foreach (var batch in Resources.FindObjectsOfTypeAll<GenerateLightingEventBatch>())
                {
                    if (batch.gameObject.scene != scene)
                        continue;
                    foreach (var group in batch.groups)
                    {
                        if (!string.IsNullOrWhiteSpace(group.name))
                            entries.Add(Tuple.Create(path, scene.name, group.name.Trim()));
                    }
                }
            }
        }
        finally
        {
            foreach (var scene in openedScenes)
                EditorSceneManager.CloseScene(scene, true);
        }

        var source = new StringBuilder(GeneratedHeader);
        var used = new HashSet<string>();
        foreach (var entry in entries)
        {
            var method = "Run_" + Sanitize(entry.Item2 + "_" + entry.Item3);
            if (!used.Add(method))
                continue;
            var menuScene = entry.Item2 == "MtBlizzard" ? "Mt. Blizzard" : entry.Item2;
            source.Append("    [MenuItem(\"Project/Events Lightmap Baking/").Append(menuScene).Append("/").Append(Escape(entry.Item3)).Append("\")]\n");
            source.Append("    static void ").Append(method).Append("() => GenerateLightingEventBatchRunner.Run(\"").Append(Escape(entry.Item1)).Append("\", \"").Append(Escape(entry.Item3)).Append("\");\n");
        }
        source.Append(GeneratedFooter);
        File.WriteAllText(MenuFile, source.ToString());
        AssetDatabase.Refresh();
    }

    static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    static string Sanitize(string value)
    {
        var result = new string(value.Where(char.IsLetterOrDigit).ToArray());
        return string.IsNullOrEmpty(result) ? "Unnamed" : result;
    }
}

public static class GenerateLightingEventBatchRunner
{
    const string WorldObjectShaderName = "CpRemix/World/WorldObject";
    const StaticEditorFlags AllStaticFlags = StaticEditorFlags.ContributeGI
        | StaticEditorFlags.OccluderStatic
        | StaticEditorFlags.OccludeeStatic
        | StaticEditorFlags.BatchingStatic
        | StaticEditorFlags.ReflectionProbeStatic;

    static readonly Dictionary<string, string> BakeMenus = new Dictionary<string, string>
    {
        { "Town", "Project/Generate lighting/Lightmap baking/Town" },
        { "Boardwalk", "Project/Generate lighting/Lightmap baking/Boardwalk" },
        { "MtBlizzard", "Project/Generate lighting/Lightmap baking/Mt. Blizzard" }
    };

    public static void Run(string scenePath, string groupName)
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.path != scenePath)
            scene = EditorSceneManager.OpenScene(scenePath);
        var batch = Resources.FindObjectsOfTypeAll<GenerateLightingEventBatch>().FirstOrDefault(x => x.gameObject.scene == scene);
        if (batch == null)
            return;
        var group = batch.groups.FirstOrDefault(x => x != null && x.name != null && x.name.Trim() == groupName);
        if (group == null)
            return;

        try
        {
            var total = group.spawn.Count + group.delete.Count + group.disable.Count + group.enable.Count + group.giStatic.Count + group.removeStatic.Count;
            var step = 0;
            foreach (var entry in group.spawn)
            {
                ShowProgress(groupName, "Spawning objects", step++, total);
                if (entry.prefab == null)
                    continue;
                var parent = FindByName(scene, entry.parentName);
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(entry.prefab, scene);
                if (parent != null)
                    instance.transform.SetParent(parent.transform, false);
                instance.transform.localPosition = entry.position;
                instance.transform.localEulerAngles = entry.rotation;
                instance.transform.localScale = entry.scale;
                Undo.RegisterCreatedObjectUndo(instance, "Spawn event object");
            }
            foreach (var entry in group.delete)
            {
                ShowProgress(groupName, "Deleting objects", step++, total);
                if (entry.target != null)
                    Undo.DestroyObjectImmediate(entry.target);
            }
            foreach (var entry in group.enable)
            {
                ShowProgress(groupName, "Enabling objects", step++, total);
                if (entry.target != null)
                    entry.target.SetActive(true);
            }
            foreach (var entry in group.disable)
            {
                ShowProgress(groupName, "Disabling objects", step++, total);
                if (entry.target != null)
                    entry.target.SetActive(false);
            }
            foreach (var entry in group.giStatic)
            {
                ShowProgress(groupName, "Setting GI Static", step++, total);
                if (entry.target != null)
                    ApplyStaticFlagsRecursively(entry.target, true);
            }
            foreach (var entry in group.removeStatic)
            {
                ShowProgress(groupName, "Removing static flags", step++, total);
                if (entry.target != null)
                    ApplyStaticFlagsRecursively(entry.target, false);
            }

            EditorUtility.ClearProgressBar();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            if (BakeMenus.TryGetValue(scene.name, out var bakeMenu))
            {
                Action onBakeCompleted = null;
                if (group.disableAfterBake.Count > 0)
                {
                    onBakeCompleted = () =>
                    {
                        Lightmapping.bakeCompleted -= onBakeCompleted;
                        EditorApplication.delayCall += () => DisableObjectsAfterBake(scenePath, groupName);
                    };
                    Lightmapping.bakeCompleted += onBakeCompleted;
                }

                if (!EditorApplication.ExecuteMenuItem(bakeMenu))
                {
                    if (onBakeCompleted != null)
                        Lightmapping.bakeCompleted -= onBakeCompleted;
                    Debug.LogError("Could not start the lightmap bake for event group: " + groupName);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static void DisableObjectsAfterBake(string scenePath, string groupName)
    {
        var scene = SceneManager.GetSceneByPath(scenePath);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogError("Could not find the loaded scene after lightmap bake: " + scenePath);
            return;
        }

        var batch = Resources.FindObjectsOfTypeAll<GenerateLightingEventBatch>()
            .FirstOrDefault(x => x.gameObject.scene == scene);
        var group = batch == null
            ? null
            : batch.groups.FirstOrDefault(x => x != null && x.name != null && x.name.Trim() == groupName);
        if (group == null)
        {
            Debug.LogError("Could not find event group after lightmap bake: " + groupName);
            return;
        }

        var disabledAnyObjects = false;
        foreach (var entry in group.disableAfterBake)
        {
            if (entry == null || entry.target == null)
                continue;
            Undo.RecordObject(entry.target, "Disable object after lightmap bake: " + groupName);
            entry.target.SetActive(false);
            disabledAnyObjects = true;
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                Debug.LogError("Could not save scene after disabling object '" + entry.target.name + "' following lightmap bake: " + scenePath);
                return;
            }
        }

        if (disabledAnyObjects)
        {
            var reloadedScene = EditorSceneManager.OpenScene(scenePath);
            if (!reloadedScene.IsValid() || !reloadedScene.isLoaded)
                Debug.LogError("Could not reload scene after disabling objects following lightmap bake: " + scenePath);
        }
    }

    static void ShowProgress(string group, string action, int step, int total)
    {
        EditorUtility.DisplayProgressBar("Event Lightmap Baking: " + group, action, total == 0 ? 1f : (float)step / total);
    }

    static void ApplyStaticFlagsRecursively(GameObject target, bool giStatic)
    {
        var meshRenderer = target.GetComponent<MeshRenderer>();
        if (!giStatic || meshRenderer != null)
        {
            Undo.RecordObject(target, giStatic ? "Set GI Static" : "Remove Static Flags");
            StaticEditorFlags flags = 0;
            if (giStatic)
            {
                flags = GameObjectUtility.GetStaticEditorFlags(target) | StaticEditorFlags.ContributeGI;
                if (UsesWorldObjectShader(meshRenderer))
                    flags |= AllStaticFlags;
            }
            GameObjectUtility.SetStaticEditorFlags(target, flags);
        }
        foreach (Transform child in target.transform)
            ApplyStaticFlagsRecursively(child.gameObject, giStatic);
    }

    static bool UsesWorldObjectShader(MeshRenderer renderer)
    {
        if (renderer == null)
            return false;
        foreach (var material in renderer.sharedMaterials)
        {
            if (material != null && material.shader != null && material.shader.name == WorldObjectShaderName)
                return true;
        }
        return false;
    }

    static GameObject FindByName(Scene scene, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == name)
                return root;
            var match = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(x => x.gameObject.name == name);
            if (match != null)
                return match.gameObject;
        }
        return null;
    }
}

public sealed class GenerateLightingEventBatchBuildStripper : IProcessSceneWithReport
{
    public int callbackOrder => int.MinValue;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        foreach (var batch in Resources.FindObjectsOfTypeAll<GenerateLightingEventBatch>())
        {
            if (batch.gameObject.scene == scene)
                UnityEngine.Object.DestroyImmediate(batch);
        }
    }
}
