using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class BeachLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Beach")]
    static void OpenScene()
    {
        BakeBeach();
    }

    static double bakeStartTime;
    static List<GameObject> temporarilyStaticObjects = new List<GameObject>();
    static System.Action _postBakeAction = null;

    const string waterRockMaterialPath = "Assets/Game/World/Scenes/GlobalAssets/WorldObjects/WaterRocks/SourceAssets/WaterRock.mat";

    static void BakeBeach()
    {
        string scenePath = "Assets/Game/World/Scenes/Beach.unity";
        EditorSceneManager.OpenScene(scenePath);
        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.IsValid())
        {
            Debug.LogError("No active scene found.");
            return;
        }

        string gameObjectName = "GameObjectLocations";
        GameObject[] rootObjects = activeScene.GetRootGameObjects();
        GameObject targetObject = null;

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name == gameObjectName)
            {
                targetObject = obj;
                break;
            }
        }

        if (targetObject == null)
        {
            Debug.LogError("GameObject not found: " + gameObjectName);
            return;
        }

        GameObjectLocations Gol = targetObject.GetComponent<GameObjectLocations>();
        string FolderPath = "Assets/Game/World/Scenes/Beach";
        string[] files = Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
            {
                AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
            }
        }

        Gol.ChangeSkybox(Gol.LightmappingSkybox);

        Gol.Animated2.isStatic = true;
        SetStaticRecursively(Gol.Animated2, true);

        Gol.StaticObject2.isStatic = false;
        SetStaticRecursively(Gol.StaticObject2, false);

        temporarilyStaticObjects.Clear();

        GameObject[] allGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allGameObjects)
        {
            if (go.name.StartsWith("MARK4STATIC_") && !go.isStatic)
            {
                go.isStatic = true;
                temporarilyStaticObjects.Add(go);
            }
        }

        Gol.ChangeSource(AmbientMode.Skybox);

        Material waterRockMaterial = AssetDatabase.LoadAssetAtPath<Material>(waterRockMaterialPath);

        if (waterRockMaterial == null)
        {
            Debug.LogError("WaterRock material not found: " + waterRockMaterialPath);
            return;
        }

        waterRockMaterial.doubleSidedGI = true;
        EditorUtility.SetDirty(waterRockMaterial);
        AssetDatabase.SaveAssets();


        _postBakeAction = () =>
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(waterRockMaterialPath);

            if (material != null)
            {
                material.doubleSidedGI = false;
                EditorUtility.SetDirty(material);
                AssetDatabase.SaveAssets();

            }
            else
            {
                Debug.LogError("WaterRock material not found after bake: " + waterRockMaterialPath);
            }

            Gol.ChangeSkybox(Gol.DayCubemap);

            Gol.Animated2.isStatic = false;
            SetStaticRecursively(Gol.Animated2, false);

            Gol.StaticObject2.isStatic = true;
            SetStaticRecursively(Gol.StaticObject2, true);

            foreach (GameObject go in temporarilyStaticObjects)
            {
                if (go != null)
                {
                    go.isStatic = false;
                }
            }

            temporarilyStaticObjects.Clear();

            Gol.StaticObject2.isStatic = true;
            SetStaticRecursively(Gol.StaticObject2, true);

            Gol.ChangeSource(AmbientMode.Flat);
        };

        bakeStartTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += UpdateProgressBar;
        Lightmapping.bakeCompleted += OnBakeCompleted;
        Lightmapping.BakeAsync();
    }

    private static void SetStaticRecursively(GameObject parent, bool flag)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.isStatic = flag;
            SetStaticRecursively(child.gameObject, flag);
        }
    }

    static void OnBakeCompleted()
    {
        EditorUtility.ClearProgressBar();
        EditorApplication.update -= UpdateProgressBar;
        Lightmapping.bakeCompleted -= OnBakeCompleted;

        LightmapTexturePostProcessor.SetLightmapTextureSize();

        _postBakeAction?.Invoke();
        _postBakeAction = null;
    }

    static void UpdateProgressBar()
    {
        if (Lightmapping.isRunning)
        {
            double elapsed = EditorApplication.timeSinceStartup - bakeStartTime;

            EditorUtility.DisplayProgressBar(
                "Baking Lightmaps...",
                $"Busy for {elapsed:F1} seconds.\nPlease wait while Unity bakes the lightmaps.",
                0.5f
            );

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
        else
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= UpdateProgressBar;
        }
    }
}