using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class TheUnderTheSeaDinerLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Events/The Under The Sea Diner")]
    static void OpenScene()
    {
        BakeTheUnderTheSeaDiner();
    }

    static double bakeStartTime;

    static void BakeTheUnderTheSeaDiner()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/TheUnderTheSeaDiner/AdditiveScenes/TheUnderTheSeaDiner_Town_Decorations.unity");
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

                string townFolderPath = "Assets/Game/World/Scenes/Events/TheUnderTheSeaDiner/AdditiveScenes/TheUnderTheSeaDiner_Town_Decorations";
                string[] files = Directory.GetFiles(townFolderPath, "*", SearchOption.AllDirectories);

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

                Gol.StaticObject1.isStatic = false;
                SetStaticRecursively(Gol.StaticObject1, false);

                Gol.StaticObject2.isStatic = false;
                SetStaticRecursively(Gol.StaticObject2, false);

                Gol.StaticObject3.isStatic = false;
                SetStaticRecursively(Gol.StaticObject3, false);

                Gol.StaticObject4.isStatic = false;
                SetStaticRecursively(Gol.StaticObject4, false);

                Gol.StaticObject5.isStatic = false;
                SetStaticRecursively(Gol.StaticObject5, false);

                Gol.ChangeSource(AmbientMode.Skybox);
                _postBakeAction = () =>
                {
                    Gol.ChangeSource(AmbientMode.Flat);

                    Gol.Animated.isStatic = false;
                    SetStaticRecursively(Gol.Animated, false);

                    Gol.StaticObject1.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject1, true);

                    Gol.StaticObject2.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject2, true);

                    Gol.StaticObject3.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject3, true);

                    Gol.StaticObject4.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject4, true);
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
    private static void SetScaleForBake(GameObject obj, Vector3 scale)
    {
        if (obj != null)
        {
            obj.transform.localScale = scale;
        }
    }
    private static void ResetScaleAfterBake(GameObject obj, Vector3 defaultScale)
    {
        if (obj != null)
        {
            obj.transform.localScale = defaultScale;
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