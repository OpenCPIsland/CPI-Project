using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class TownLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Town")]
    static void OpenScene()
    {
        BakeTown();
    }

    static double bakeStartTime;

    static void BakeTown()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Town.unity");
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

                string townFolderPath = "Assets/Game/World/Scenes/Town";
                string[] files = Directory.GetFiles(townFolderPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
                    {
                        AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
                    }
                }

                Gol.ChangeSkybox(Gol.LightmappingSkybox);

                SetScaleForBake(Gol.FrontTrainDoorLeft, new Vector3(1, 1, 1));
                SetScaleForBake(Gol.FrontTrainDoorRight, new Vector3(1, 1, 1));

                Gol.FrontTrainDoorLeft.isStatic = true;
                SetStaticRecursively(Gol.FrontTrainDoorLeft, true);

                Gol.FrontTrainDoorRight.isStatic = true;
                SetStaticRecursively(Gol.FrontTrainDoorRight, true);

                Gol.StudioDoorLeft.isStatic = true;
                SetStaticRecursively(Gol.StudioDoorLeft, true);

                Gol.StudioDoorRight.isStatic = true;
                SetStaticRecursively(Gol.StudioDoorRight, true);

                Gol.ClothingDoorLeft.isStatic = true;
                SetStaticRecursively(Gol.ClothingDoorLeft, true);

                Gol.ClothingDoorRight.isStatic = true;
                SetStaticRecursively(Gol.ClothingDoorRight, true);

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

                Gol.StaticObject7.isStatic = true;
                SetStaticRecursively(Gol.StaticObject7, true);

                Gol.StaticObject8.isStatic = true;
                SetStaticRecursively(Gol.StaticObject8, true);

                Gol.StaticObject9.isStatic = true;
                SetStaticRecursively(Gol.StaticObject9, true);

                Gol.StaticObject10.isStatic = true;
                SetStaticRecursively(Gol.StaticObject10, true);

                if (Gol.Animated2 != null)
                {
                    Gol.Animated2.isStatic = true;
                    SetStaticRecursively(Gol.Animated2, true);
                }

                Gol.ChangeSource(AmbientMode.Skybox);

                _postBakeAction = () =>
                {
                    Gol.ChangeSkybox(Gol.DayCubemap);

                    ResetScaleAfterBake(Gol.FrontTrainDoorLeft, new Vector3(0.002207483f, 1f, 1));
                    ResetScaleAfterBake(Gol.FrontTrainDoorRight, new Vector3(-0.03327911f, 1f, 1));

                    Gol.FrontTrainDoorLeft.isStatic = false;
                    SetStaticRecursively(Gol.FrontTrainDoorLeft, false);

                    Gol.FrontTrainDoorRight.isStatic = false;
                    SetStaticRecursively(Gol.FrontTrainDoorRight, false);

                    Gol.StudioDoorLeft.isStatic = false;
                    SetStaticRecursively(Gol.StudioDoorLeft, false);

                    Gol.StudioDoorRight.isStatic = false;
                    SetStaticRecursively(Gol.StudioDoorRight, false);

                    Gol.ClothingDoorLeft.isStatic = false;
                    SetStaticRecursively(Gol.ClothingDoorLeft, false);

                    Gol.ClothingDoorRight.isStatic = false;
                    SetStaticRecursively(Gol.ClothingDoorRight, false);

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

                    Gol.StaticObject7.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject7, false);

                    Gol.StaticObject8.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject8, false);

                    Gol.StaticObject9.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject9, false);

                    Gol.StaticObject10.isStatic = false;
                    SetStaticRecursively(Gol.StaticObject10, false);

                    if (Gol.Animated2 != null)
                    {
                        Gol.Animated2.isStatic = false;
                        SetStaticRecursively(Gol.Animated2, false);
                    }

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