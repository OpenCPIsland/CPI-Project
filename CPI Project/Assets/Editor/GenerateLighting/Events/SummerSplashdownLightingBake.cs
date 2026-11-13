using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class SummerSplashdownLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Events/Summer Splashdown")]
    static void OpenScene()
    {
        BakeSummerSplashdown();
    }

    static double bakeStartTime;

    static void BakeSummerSplashdown()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/SummerBeach2018/Resources/AdditiveScenes/SummerBeach2018_Town_Decorations.unity");
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.IsValid())
        {
        }
        else
        {
            Debug.LogError("No active scene found.");
        }

        if (activeScene.IsValid())
        {
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

            if (targetObject != null)
            {
                GameObjectLocations Gol = targetObject.GetComponent<GameObjectLocations>();

                string FolderPath = "Assets/Game/World/Scenes/Events/SummerBeach2018/Resources/AdditiveScenes/SummerBeach2018_Town_Decorations";
                string[] files = Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
                    {
                        AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
                    }
                }

                Gol.ChangeSkybox(Gol.LightmappingSkybox);

                Gol.Animated.isStatic = true;
                SetStaticRecursively(Gol.Animated, true);

                Gol.Animated2.isStatic = true;
                SetStaticRecursively(Gol.Animated2, true);

                Gol.StaticObject2.isStatic = false;
                SetStaticRecursively(Gol.StaticObject2, false);

                Gol.StaticObject9.isStatic = true;
                SetStaticRecursively(Gol.StaticObject9, true);

                Gol.ChangeSource(AmbientMode.Skybox);

                _postBakeAction = () =>
                {
                    Gol.ChangeSource(AmbientMode.Flat);

                    Gol.Animated.isStatic = false;
                    SetStaticRecursively(Gol.Animated, false);

                    Gol.Animated2.isStatic = false;
                    SetStaticRecursively(Gol.Animated2, false);

                    Gol.StaticObject2.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject2, true);

                    Gol.StaticObject9.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject9, false);

                };

                bakeStartTime = EditorApplication.timeSinceStartup;
                EditorApplication.update += UpdateProgressBar;
                Lightmapping.bakeCompleted += OnBakeCompleted;
                Lightmapping.BakeAsync();
                return;
            }
            else
            {
                Debug.LogError("GameObject not found: " + gameObjectName);
            }
        }
        else
        {
            Debug.LogError("No active scene found.");
        }
    }

    private static void SetStaticRecursively(GameObject parent, bool flag)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.isStatic = flag;
            SetStaticRecursively(child.gameObject, flag);
        }
    }

    private static System.Action _postBakeAction = null;

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