using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class MtBlizzardLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Mt. Blizzard")]
    static void OpenScene()
    {
        BakeMtBlizzard();
    }

    static double bakeStartTime;

    static void BakeMtBlizzard()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/MtBlizzard.unity");
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
                string FolderPath = "Assets/Game/World/Scenes/MtBlizzard";
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

                Gol.Animated3.isStatic = true;
                SetStaticRecursively(Gol.Animated3, true);

                Gol.Animated4.isStatic = true;
                SetStaticRecursively(Gol.Animated4, true);

                Gol.Animated5.isStatic = true;
                SetStaticRecursively(Gol.Animated5, true);

                Gol.Animated6.isStatic = true;
                SetStaticRecursively(Gol.Animated6, true);

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

                Gol.StaticObject6.isStatic = false;
                SetStaticRecursively(Gol.StaticObject6, false);

                Gol.StaticObject7.isStatic = false;
                SetStaticRecursively(Gol.StaticObject7, false);

                Gol.StaticObject8.isStatic = false;
                SetStaticRecursively(Gol.StaticObject8, false);

                Gol.StaticObject9.isStatic = false;
                SetStaticRecursively(Gol.StaticObject9, false);

                Gol.StaticObject10.isStatic = false;
                SetStaticRecursively(Gol.StaticObject10, false);

                Gol.StaticObject11.isStatic = false;
                SetStaticRecursively(Gol.StaticObject11, false);

                Gol.StaticObject12.isStatic = false;
                SetStaticRecursively(Gol.StaticObject12, false);

                Gol.StaticObject13.isStatic = false;
                SetStaticRecursively(Gol.StaticObject13, false);

                Gol.StaticObject14.isStatic = true;
                SetStaticRecursively(Gol.StaticObject14, true);

                Gol.StaticObject15.isStatic = true;
                SetStaticRecursively(Gol.StaticObject15, true);

                Gol.ChangeSource(AmbientMode.Skybox);
                _postBakeAction = () =>
                {
                    Gol.ChangeSkybox(Gol.MtBlizzardCubemap);

                    Gol.Animated.isStatic = false;
                    SetStaticRecursively(Gol.Animated, false);

                    Gol.Animated2.isStatic = false;
                    SetStaticRecursively(Gol.Animated2, false);

                    Gol.Animated3.isStatic = false;
                    SetStaticRecursively(Gol.Animated3, false);

                    Gol.Animated4.isStatic = false;
                    SetStaticRecursively(Gol.Animated4, false);

                    Gol.Animated5.isStatic = false;
                    SetStaticRecursively(Gol.Animated5, false);

                    Gol.Animated6.isStatic = false;
                    SetStaticRecursively(Gol.Animated6, false);

                    Gol.StaticObject1.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject1, true);

                    Gol.StaticObject2.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject2, true);

                    Gol.StaticObject3.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject3, true);

                    Gol.StaticObject4.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject4, true);

                    Gol.StaticObject5.isStatic = true;
                    SetStaticRecursively(Gol.StaticObject5, true);

                    Gol.StaticObject6.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject6, false);

                    Gol.StaticObject7.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject7, false);

                    Gol.StaticObject8.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject8, false);

                    Gol.StaticObject9.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject9, false);

                    Gol.StaticObject10.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject10, false);

                    Gol.StaticObject11.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject11, false);

                    Gol.StaticObject12.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject12, false);

                    Gol.StaticObject13.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject13, false);

                    Gol.StaticObject14.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject14, false);

                    Gol.StaticObject15.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject15, false);

                    Gol.ChangeSource(AmbientMode.Flat);
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