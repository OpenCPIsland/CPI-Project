using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class MedievalBoardwalkLightingBake : MonoBehaviour
{
    [MenuItem("Project/Generate lighting/Lightmap baking/Events/Medieval Boardwalk")]
    static void OpenScene()
    {
        BakeMedievalBoardwalk();
    }

    static double bakeStartTime;

    static void BakeMedievalBoardwalk()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/MedievalParty2018/Resources/AdditiveScenes/MedievalParty2018_Boardwalk_Decorations.unity");
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
                string FolderPath = "Assets/Game/World/Scenes/Events/MedievalParty2018/Resources/AdditiveScenes/MedievalParty2018_Boardwalk_Decorations";
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

                Gol.ChangeSource(AmbientMode.Skybox);
                _postBakeAction = () =>
                {
                    Gol.ChangeSkybox(Gol.LightmappingSkybox);

                    Gol.Animated.isStatic = false;
                    SetStaticRecursively(Gol.Animated, false);

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