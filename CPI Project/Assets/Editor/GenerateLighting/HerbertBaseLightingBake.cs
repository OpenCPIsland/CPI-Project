using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class HerbertBaseLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Herbert's Base")]
    static void OpenScene()
    {
        BakeHerbertBase();
    }

    static double bakeStartTime;

    static void BakeHerbertBase()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/HerbertBase.unity");
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
                string FolderPath = "Assets/Game/World/Scenes/HerbertBase";
                string[] files = Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
                    {
                        AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
                    }
                }
                Gol.ChangeSkybox(Gol.HerbertBaseCubemap);

                Gol.Door1.isStatic = true;
                SetStaticRecursively(Gol.Door1, true);

                Gol.Door2.isStatic = true;
                SetStaticRecursively(Gol.Door2, true);

                Gol.Door3.isStatic = true;
                SetStaticRecursively(Gol.Door3, true);

                Gol.Animated.isStatic = true;
                SetStaticRecursively(Gol.Animated, true);

                Gol.StaticObject1.isStatic = false;
                SetStaticRecursively(Gol.StaticObject1, false);

                Gol.StaticObject2.isStatic = true;
                SetStaticRecursively(Gol.StaticObject2, true);

                Gol.ChangeSource(AmbientMode.Skybox);
                _postBakeAction = () =>
                {
                    Gol.ChangeSkybox(Gol.HerbertBaseCubemap);

                    Gol.Door1.isStatic = false;
                    SetStaticRecursively(Gol.Door1, false);

                    Gol.Door2.isStatic = false;
                    SetStaticRecursively(Gol.Door2, false);

                    Gol.Door3.isStatic = false;
                    SetStaticRecursively(Gol.Door3, false);

                    Gol.Animated.isStatic = false;
                    SetStaticRecursively(Gol.Animated, false);

                    Gol.StaticObject1.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject1, true);

                    Gol.StaticObject2.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject2, false);

                    Gol.ChangeSource(AmbientMode.Skybox);
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