using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class PirateExpeditionBeachLightingBake : MonoBehaviour
{
    // Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
    [MenuItem("Project/Generate lighting/Lightmap baking/Pirate Expedition Beach")]
    static void OpenScene()
    {
        // Open the Scene in the Editor (do not enter Play Mode)
        BakePirateExpeditionBeach();
    }

    static void BakePirateExpeditionBeach()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Events/PirateParty2018/Resources/AdditiveScenes/EventPirateParty2018_Beach_Decorations.unity");
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

                // Get the folder where the current scene is located
                string sceneFolderPath = Path.GetDirectoryName(activeScene.path);

                // Get all files in the scene's directory and subdirectories
                string[] files = Directory.GetFiles(sceneFolderPath, "*", SearchOption.AllDirectories);

                // Delete .exr and LightingData.asset files
                foreach (string file in files)
                {
                    if (file.EndsWith(".exr") || file.EndsWith("LightingData.asset"))
                    {
                        Debug.Log("Deleting: " + file);
                        AssetDatabase.DeleteAsset(file.Replace(Application.dataPath, "Assets"));
                    }
                }

                // Set for baking
                Gol.ChangeSkybox(Gol.PiratePartySkyboxForBakingLightmaps);

                Gol.EventPirateParty2018_Beach_Prefab.isStatic = true;
                SetStaticRecursively(Gol.EventPirateParty2018_Beach_Prefab, true);

                Gol.GatewayFX.isStatic = false;
                SetStaticRecursively(Gol.GatewayFX, false);

                Gol.ChangeSource(AmbientMode.Skybox);

                // Bake the lightmap
                Lightmapping.Bake();

                // Reset skybox and static flags
                Gol.ChangeSkybox(Gol.PiratePartySkyboxForBakingLightmaps);

                Gol.EventPirateParty2018_Beach_Prefab.isStatic = false;
                SetStaticRecursively(Gol.EventPirateParty2018_Beach_Prefab, false);

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
