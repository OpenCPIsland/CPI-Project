using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class SeaCavesLightingBake : MonoBehaviour
{
    // Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
    [MenuItem("Project/Generate lighting/Lightmap baking/Sea Caves")]
    static void OpenScene()
    {
        // Open the Scene in the Editor (do not enter Play Mode)
        BakeSeaCaves();
    }

    static void BakeSeaCaves()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Diving.unity");
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.IsValid())
        {
            Debug.Log("Current Scene Name: " + activeScene.name);
            Debug.Log("Current Scene Path: " + activeScene.path);
        }
        else
        {
            Debug.LogError("No active scene found.");
        }

        if (activeScene.IsValid())
        {
            // Replace "YourGameObjectName" with the name of the GameObject you want to find
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
                Debug.Log("Found GameObject: " + targetObject.name);
                GameObjectLocations Gol = targetObject.GetComponent<GameObjectLocations>();

                // Delete only .exr files and LightingData.asset inside the Diving folder
                string folderPath = "Assets/Game/World/Scenes/Diving";
                string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
                    {
                        Debug.Log("Deleting: " + file);
                        AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
                    }
                }

                // Set for baking
                Gol.ChangeSkybox(Gol.LightmappingSkybox);

                Gol.WorldArt.isStatic = true;
                SetStaticRecursively(Gol.WorldArt, true);
                Gol.LevelDesign.isStatic = true;
                SetStaticRecursively(Gol.LevelDesign, true);

                Gol.ChangeSource(AmbientMode.Skybox);

                // Bake the lightmap
                Lightmapping.Bake();

                // Reset skybox and static flags
                Gol.ChangeSkybox(Gol.DivingCubemap);

                Gol.WorldArt.isStatic = false;
                SetStaticRecursively(Gol.WorldArt, false);
                Gol.LevelDesign.isStatic = false;
                SetStaticRecursively(Gol.LevelDesign, false);

                Gol.ChangeSource(AmbientMode.Flat);
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
}
