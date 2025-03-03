// (c) Copyright HutongGames, LLC 2010-2020. All rights reserved.

// TODO: Only do this when necessary.
// TODO: Manual option.

// Unity 5.1 introduced a new networking library. 
// Unless we define PLAYMAKER_LEGACY_NETWORK old network actions are disabled
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || PLAYMAKER_LEGACY_NETWORK)
#define UNITY_NEW_NETWORK
#endif

// Some platforms do not support networking (at least the old network library)
#if (UNITY_FLASH || UNITY_NACL || UNITY_METRO || UNITY_WP8 || UNITY_WIIU || UNITY_PSM || UNITY_WEBGL || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE)
#define PLATFORM_NOT_SUPPORTED
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace HutongGames.PlayMakerEditor
{
    /*
     * These tools did 2 main things:
     * 
     * 1. Updated old Unity/PlayMaker projects when major changes were made.
     * 2. Preprocessed FSMs to optimize runtime performance.
     *
     * Problems:
     *
     * With Case 1, the Versions fixed are now very old, so it's not clear that
     * we still need those tools. We will probably retire these soon.
     * 
     * With Case 2, SaveAsPrefabAsset in Unity 2022.3 breaks prefab Ids!
     * Additionally the logic used by SaveAsPrefabAsset (name based matching of GameObjects
     * and Components) seems fragile and could cause problems in more complex scenarios.
     * Unity has also optimized component operations since this preprocessing was implemented.
     *
     * For these reasons (especially the breaking bug in 2022.3!)
     * we're sunsetting the prefab tools as we investigate alternatives.
     * We're not removing the public APIs in case any third party
     * tools call them, but they won't do anything!
     */
    public class ProjectTools
    {
        // Change MenuRoot to move the Playmaker Menu
        // E.g., MenuRoot = "Plugins/PlayMaker/"
        private const string MenuRoot = "PlayMaker/";

        [MenuItem(MenuRoot + "Tools/Update All Loaded FSMs", false, 25)]
        public static void UpdateAllLoadedFSMs()
        {
            ReSaveAllLoadedFSMs();
        }

        [MenuItem(MenuRoot + "Tools/Update All FSMs in Build", false, 26)]
        public static void UpdateAllFSMsInBuild()
        {
            UpdateScenesInBuild();
        }

        //[MenuItem(MenuRoot + "Tools/Preprocess Prefab FSMs in Build", false, 27)]
        public static void PreprocessPrefabFSMs()
        {   
            Debug.LogWarning("PreprocessPrefabFSMs is Obsolete!");
            
            /*
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var timer = Stopwatch.StartNew();

            var logOutput = DoPreprocessPrefabFSMsInBuild();

            logOutput += "\nElapsed Time: " + timer.Elapsed.Seconds + "s";

            Debug.Log(logOutput);
            */
        }

        //[MenuItem(MenuRoot + "Tools/Preprocess All Prefab FSMs", false, 27)]
        public static void PreprocessAllPrefabFSMs()
        {
            Debug.LogWarning("PreprocessAllPrefabFSMs is Obsolete!");

            /*
            var timer = Stopwatch.StartNew();

            var logOutput = DoPreprocessAllPrefabFSMs();

            logOutput += "\nElapsed Time: " + timer.Elapsed.Seconds + "s";

            Debug.Log(logOutput);
            */
        }

        /*WIP
        [MenuItem(MenuRoot + "Tools/Scan Scenes", false, 33)]
        public static void ScanScenesInProject()
        {
            FindAllScenes();
        }
        */

        /// <summary>
        /// Collects all prefabs with FSMs referenced by scenes in the build.
        /// Then preprocess all FSMs on those prefabs.
        /// TODO: check if this handles:
        /// - PlayMakerFSMs referenced in actions
        /// - PlayMakerFSMs in Resources folders
        /// </summary>
        private static string DoPreprocessPrefabFSMsInBuild()
        {
            LogHelper.Log("DoPreprocessPrefabFSMsInBuild", LogColor.Yellow);

            var loadedScenes = GetLoadedScenes();

            var report = "Preprocess Prefab FSMs in Build:";

            // We open enabled scenes in the build to find prefabs with PlayMakerFSMs
            // We do this first to avoid an AssetDatabase reload for each scene
            // Processing fsms in the scene load loop is much slower

            var prefabs = GetAllPrefabFSMsInBuild();
            report += "\nPrefab FSMs found: " + prefabs.Count;

            if (prefabs.Count == 0) return report;

            /*
            // Unity Bug: https://github.com/TeamSirenix/odin-serializer/issues/10
            // Note: StartAssetEditing/StopAssetEditing doesn't seem to work across scene loading.
            // another reason to collect all the prefabs first.

            try
            {
                AssetDatabase.StartAssetEditing();

                report += DoPreprocessPrefabFSMs(prefabs);
            }
            finally
            {
                StopAssetEditing();
            }
            */
            
            try
            {
                report += DoPreprocessPrefabFSMs(prefabs);
            }
            finally
            {
                AssetDatabase.Refresh();
            }
            
            // Restore previously loaded scenes
            LoadScenes(loadedScenes);

            return report;
        }

        private static void StopAssetEditing()
        {
            if (EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += StopAssetEditing;
                return;
            }
            
            AssetDatabase.StopAssetEditing();
        }

        /// <summary>
        /// Get the main GameObject for a prefab file.
        /// In 2018.3+ loads the prefab so it can be edited.
        /// </summary>
        public static GameObject GetPrefabGameObject(string prefabPath)
        {
            GameObject go = null;

            // We need to put this in a try block otherwise it can hang Unity editor

            try
            {
#if UNITY_2018_3_OR_NEWER

                go = PrefabUtility.LoadPrefabContents(prefabPath);
#else
                go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return go;
        }

        /// <summary>
        /// Get all scenes currently open in the editor
        /// </summary>
        public static List<string> GetLoadedScenes()
        {
            var openScenes = new List<string>();
#if UNITY_2022_2_OR_NEWER
            for (var i = 0; i < SceneManager.loadedSceneCount; i++)
#else
            for (var i = 0; i < EditorSceneManager.loadedSceneCount; i++)
#endif                
            {
                openScenes.Add(SceneManager.GetSceneAt(i).path);
            }

            return openScenes;    
        }

        /// <summary>
        /// Get all enabled scenes in Build Settings.
        /// </summary>
        public static List<string> GetScenesInBuild()
        {
            return (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToList();
        }

        /// <summary>
        /// Load multiple scenes in the editor.
        /// </summary>
        /// <param name="scenes"></param>
        public static void LoadScenes(List<string> scenes)
        {
            if (scenes.Count == 0 || string.IsNullOrEmpty(scenes[0])) return;
            
            var loadedScenes = GetLoadedScenes();
            if (loadedScenes.SequenceEqual(scenes)) return;

            EditorSceneManager.OpenScene(scenes[0], OpenSceneMode.Single);
            for (var i = 1; i < scenes.Count; i++)
            {
                EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Additive);
            }
        }

        /// <summary>
        /// Get all prefab files in build that have PlayMakerFSMs
        /// </summary>
        public static List<string> GetAllPrefabFSMsInBuild()
        {
            var scenes = GetScenesInBuild();
            var loadedScenes = GetLoadedScenes();
            var prefabs = new List<string>();

            // first get prefab FSMs that are already loaded

            GetLoadedPrefabFSMs(prefabs);

            foreach (var scene in loadedScenes)
            {
                scenes.Remove(scene);
            }

            // now iterate through other scenes in build

            float sceneCount = scenes.Count;
            if (sceneCount > 0)
            {
                for (var i = 0; i < scenes.Count; i++)
                {
                    var scene = scenes[i];
                    EditorSceneManager.OpenScene(scene, OpenSceneMode.Single);

                    var cancel = EditorUtility.DisplayCancelableProgressBar("PlayMaker",
                        "Finding prefab FSMs in scene: " + scene, i / sceneCount);
                    if (cancel) return new List<string>();

                    GetLoadedPrefabFSMs(prefabs);
                }

                EditorUtility.ClearProgressBar();
            }
            
            return prefabs;
        }

        /// <summary>
        /// Get loaded prefab files that have PlayMakerFSMs
        /// </summary>
        public static void GetLoadedPrefabFSMs(List<string> prefabs)
        {
            var fsmList = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
            for (var i = 0; i < fsmList.Length; i++)
            {
                var playMakerFSM = fsmList[i];
                if (playMakerFSM == null || !FsmPrefabs.IsPrefab(playMakerFSM.Fsm)) continue;

                var assetPath = AssetDatabase.GetAssetPath(playMakerFSM);
                if (prefabs.Contains(assetPath)) continue;

                prefabs.Add(assetPath);
            }
        }

        private static string DoPreprocessPrefabFSMs(List<string> prefabs)
        {
            var report = "";
            if (prefabs.Count == 0) return report;

            float prefabCount = prefabs.Count;           
            for (int i = 0; i < prefabCount; i++)
            {
                var prefab = prefabs[i];
                if (prefab == null) continue;

                var cancel = EditorUtility.DisplayCancelableProgressBar("PlayMaker", prefab.Substring(7), i/prefabCount);
                if (cancel) return report + "\nCancelled";
                
                var go = GetPrefabGameObject(prefab);
                if (go == null) continue;

                var prefabFSMs = go.GetComponentsInChildren<PlayMakerFSM>();
                if (prefabFSMs.Length > 0)
                {
                    foreach (var prefabFSM in prefabFSMs)
                    {                

#if UNITY_2018_3_OR_NEWER
                    
                        // nested prefab will be processes as a prefab, so skip here
                        if (PrefabUtility.IsPartOfPrefabInstance(prefabFSM))
                        {
                            //report += "\nSkipping Nested Prefab: " + FsmUtility.GetFullFsmLabel(prefabFSM);
                            continue;
                        }   
#endif

                        //report += "\nPreprocess: " + FsmUtility.GetFullFsmLabel(prefabFSM);

                        prefabFSM.Preprocess();
                    }

                    EditorUtility.SetDirty(go);

#if UNITY_2018_3_OR_NEWER                

                    PrefabUtility.SaveAsPrefabAsset(go, prefab);
                }

                PrefabUtility.UnloadPrefabContents(go);
#else
                }
#endif
            }

            EditorUtility.ClearProgressBar();

            return report;
        }


        /// <summary>
        /// 
        /// </summary>
        private static string DoPreprocessAllPrefabFSMs()
        {
            var report = "Preprocess All Prefab FSMs:";

            AssetDatabase.StartAssetEditing();

            var prefabs = Files.GetPrefabsWithFsmComponent();
            report += "\nPrefabs found: " + prefabs.Count;

            if (prefabs.Count == 0) return report;

            float prefabCount = prefabs.Count;
            
            for (int i = 0; i < prefabCount; i++)
            {
                var prefab = prefabs[i];
                if (prefab == null) continue;

                var cancel = EditorUtility.DisplayCancelableProgressBar("PlayMaker", prefab, i/prefabCount);
                if (cancel) return report + "\nCancelled";
                
                var go = GetPrefabGameObject(prefab);
                if (go == null) continue;

                var prefabFSMs = go.GetComponentsInChildren<PlayMakerFSM>();
                if (prefabFSMs.Length > 0)
                {
                    foreach (var prefabFSM in prefabFSMs)
                    {                

#if UNITY_2018_3_OR_NEWER
                    
                        // nested prefab will be processes as a prefab, so skip here
                        if (PrefabUtility.IsPartOfPrefabInstance(prefabFSM))
                        {
                            //report += "\nSkipping Nested Prefab: " + FsmUtility.GetFullFsmLabel(prefabFSM);
                            continue;
                        }   
#endif

                        //report += "\nPreprocess: " + FsmUtility.GetFullFsmLabel(prefabFSM);

                        prefabFSM.Preprocess();
                    }

                    EditorUtility.SetDirty(go);

#if UNITY_2018_3_OR_NEWER                

                    PrefabUtility.SaveAsPrefabAsset(go, prefab);
                }
                PrefabUtility.UnloadPrefabContents(go);
#else
                }
#endif
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.StopAssetEditing();

            return report;
        }

        private static List<string> GetAllPrefabFiles()
        {
            var searchDirectory = new DirectoryInfo(Application.dataPath);
            var prefabFiles = searchDirectory.GetFiles("*.prefab", SearchOption.AllDirectories);

            var paths = new List<string>();
            float totalFiles = prefabFiles.Length;

            for (int i = 0; i < prefabFiles.Length; i++)
            {
                var file = prefabFiles[i];
                EditorUtility.DisplayProgressBar("PlayMaker", "Finding prefabs...",
                    i / totalFiles);

                var filePath = file.FullName.Replace('\\', '/').Replace(Application.dataPath, "Assets");
                //Debug.Log(filePath + "\n" + Application.dataPath);

                paths.Add(filePath);
            }

            EditorUtility.ClearProgressBar();

            return paths;
        }

        private static void ReSaveAllLoadedFSMs()
        {
            Debug.Log("Checking loaded FSMs...");
            FsmEditor.RebuildFsmList();
            foreach (var fsm in FsmEditor.FsmList)
            {
                // Re-initialize loads data and forces a dirty check
                // so we can just call this and let it handle dirty etc.

                fsm.Reload();

                if (fsm.DataVersion == 1)
                {
                    fsm.DataVersion = Fsm.CurrentDataVersion;
                    fsm.SaveActions();
                }
            }
        }

        private static void UpdateScenesInBuild()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            LoadPrefabsWithPlayMakerFSMComponents();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                Debug.Log("Open Scene: " + scene.path);

                EditorSceneManager.OpenScene(scene.path);

                UpdateGetSetPropertyActions();

                ReSaveAllLoadedFSMs();

                if (!EditorApplication.isUpdating && !EditorSceneManager.SaveOpenScenes())
                {
                    Debug.LogError("Could not save scene!");
                }
            }
        }

        private static void LoadPrefabsWithPlayMakerFSMComponents()
        {
            Debug.Log("Finding Prefabs with PlayMakerFSMs");

            var searchDirectory = new DirectoryInfo(Application.dataPath);
            var prefabFiles = searchDirectory.GetFiles("*.prefab", SearchOption.AllDirectories);

            foreach (var file in prefabFiles)
            {
                var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                //Debug.Log(filePath + "\n" + Application.dataPath);

                var dependencies = AssetDatabase.GetDependencies(new[] { filePath });
                foreach (var dependency in dependencies)
                {
                    if (dependency.Contains("/PlayMaker.dll"))
                    {
                        Debug.Log("Found Prefab with FSM: " + filePath);
                        AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
                    }
                }
            }

            FsmEditor.RebuildFsmList();
        }

        /// <summary>
        /// Older versions of Unity had built it properties to access common components.
        /// These properties need to be fixed in GetProperty/SetProperty.
        /// </summary>
        private static void UpdateGetSetPropertyActions()
        {
            var getPropertyActionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.GetProperty");
            var setPropertyActionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.SetProperty");

            FsmEditor.RebuildFsmList();

            foreach (var fsm in FsmEditor.FsmList)
            {
                if (fsm.GameObject == null) continue; // can't update property paths without GameObject

                var upgraded = false;
                foreach (var state in fsm.States)
                {
                    foreach (var action in state.Actions)
                    {
                        var actionType = action.GetType();
                        if (actionType == getPropertyActionType)
                        {
                            var targetPropertyField = getPropertyActionType.GetField("targetProperty", BindingFlags.Public | BindingFlags.Instance);
                            if (targetPropertyField != null)
                            {
                                upgraded |= TryUpgradeFsmProperty(fsm.GameObject, targetPropertyField.GetValue(action) as FsmProperty);
                            }
                        }
                        else if (actionType == setPropertyActionType)
                        {
                            var targetPropertyField = setPropertyActionType.GetField("targetProperty", BindingFlags.Public | BindingFlags.Instance);
                            if (targetPropertyField != null)
                            {
                                upgraded |= TryUpgradeFsmProperty(fsm.GameObject, targetPropertyField.GetValue(action) as FsmProperty);
                            }
                        }
                    }
                }
                if (upgraded)
                {
                    //Undo called in batch operation seems to crash Unity
                    //FsmEditor.SaveActions(fsm);

                    foreach (var state in fsm.States)
				    {
					    state.SaveActions();
				    }

				    FsmEditor.SetFsmDirty(fsm, true);

                    EditorSceneManager.MarkSceneDirty(fsm.Owner.gameObject.scene);
                }
            }
        }

        private static bool TryUpgradeFsmProperty(GameObject gameObject, FsmProperty fsmProperty)
        {
            if (gameObject == null || fsmProperty == null) return false;
            var propertyPath = fsmProperty.PropertyName;
            if (string.IsNullOrEmpty(propertyPath)) return false;

            var parts = propertyPath.Split('.');
            if (TryFindComponent(gameObject, fsmProperty, parts[0]))
            {
                var oldPath = fsmProperty.PropertyName;
                fsmProperty.PropertyName = string.Join(".", parts, 1, parts.Length-1);
                Debug.Log("Fixed: " + oldPath + "->" + fsmProperty.PropertyName);
                return true;
            }

            return false;
        }

        private static bool TryFindComponent(GameObject gameObject, FsmProperty fsmProperty, string component)
        {
            if (component == "rigidbody") return FixFsmProperty(gameObject, fsmProperty, typeof(Rigidbody));
            if (component == "rigidbody2D") return FixFsmProperty(gameObject, fsmProperty, typeof(Rigidbody2D));
            if (component == "camera") return FixFsmProperty(gameObject, fsmProperty, typeof(Camera));
            if (component == "light") return FixFsmProperty(gameObject, fsmProperty, typeof(Light));
            if (component == "animation") return FixFsmProperty(gameObject, fsmProperty, typeof(Animation));
            if (component == "constantForce") return FixFsmProperty(gameObject, fsmProperty, typeof(ConstantForce));
            if (component == "renderer") return FixFsmProperty(gameObject, fsmProperty, typeof(Renderer));
            if (component == "audio") return FixFsmProperty(gameObject, fsmProperty, typeof(AudioSource));
#if !UNITY_2017_2_OR_NEWER
            if (component == "guiText") return FixFsmProperty(gameObject, fsmProperty, typeof(GUIText));
            if (component == "guiTexture") return FixFsmProperty(gameObject, fsmProperty, typeof(GUITexture));
#endif
            if (component == "collider") return FixFsmProperty(gameObject, fsmProperty, typeof(Collider));
            if (component == "collider2D") return FixFsmProperty(gameObject, fsmProperty, typeof(Collider2D));
            if (component == "hingeJoint") return FixFsmProperty(gameObject, fsmProperty, typeof(HingeJoint));
            if (component == "particleSystem") return FixFsmProperty(gameObject, fsmProperty, typeof(ParticleSystem));
            
#if !(PLATFORM_NOT_SUPPORTED || UNITY_NEW_NETWORK || PLAYMAKER_NO_NETWORK)
            if (component == "networkView") return FixFsmProperty(gameObject, fsmProperty, typeof(NetworkView));
#endif
            return false;
        }

        private static bool FixFsmProperty(GameObject gameObject, FsmProperty fsmProperty, Type componentType)
        {
            fsmProperty.TargetObject.Value = gameObject.GetComponent(componentType);
            fsmProperty.TargetObject.ObjectType = componentType;
            return true;
        }

        /* WIP
        [Localizable(false)]
        private static void FindAllScenes()
        {
            Debug.Log("Finding all scenes...");

            var searchDirectory = new DirectoryInfo(Application.dataPath);
            var assetFiles = searchDirectory.GetFiles("*.unity", SearchOption.AllDirectories);

            foreach (var file in assetFiles)
            {
                var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                if (obj == null)
                {
                    //Debug.Log(filePath + ": null!");
                }
                else if (obj.GetType() == typeof(Object))
                {
                    Debug.Log(filePath);// + ": " + obj.GetType().FullName);
                }
                //var obj = AssetDatabase.
            }
        }
        */
    }
}

