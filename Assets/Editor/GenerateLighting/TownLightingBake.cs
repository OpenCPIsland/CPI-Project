using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class TownLightingBake : MonoBehaviour
{
    // Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
    [MenuItem("Project/Generate lighting/Lightmap baking/Town")]
    static void OpenScene()
    {
        // Open the Scene in the Editor (do not enter Play Mode)
        BakeTown();
    }

    static void BakeTown()
    {
        EditorSceneManager.OpenScene("Assets/Game/World/Scenes/Town.unity");
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

                // Delete only .exr files and LightingData.asset inside Town folder
                string townFolderPath = "Assets/Game/World/Scenes/Town";
                string[] files = Directory.GetFiles(townFolderPath, "*", SearchOption.AllDirectories);

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

                // Set objects to static
                Gol.WorldArt.isStatic = true;
                SetStaticRecursively(Gol.WorldArt, true);

                Gol.HerbertBaseV2.isStatic = true;
                SetStaticRecursively(Gol.HerbertBaseV2, true);

                // Adjust the scale of FrontTrainDoorLeft and FrontTrainDoorRight during baking
                SetScaleForBake(Gol.FrontTrainDoorLeft, new Vector3(1, 1, 1));
                SetScaleForBake(Gol.FrontTrainDoorRight, new Vector3(1, 1, 1));

                Gol.FrontTrainDoorLeft.isStatic = true;
                SetStaticRecursively(Gol.FrontTrainDoorLeft, true);

                Gol.FrontTrainDoorRight.isStatic = true;
                SetStaticRecursively(Gol.FrontTrainDoorRight, true);

                Gol.TrainBlue.isStatic = true;
                SetStaticRecursively(Gol.TrainBlue, true);

                Gol.Interactables.isStatic = true;
                SetStaticRecursively(Gol.Interactables, true);

                Gol.ChangeSource(AmbientMode.Skybox);

                // Bake the lightmap
                Lightmapping.Bake();

                // Reset the scale after baking
                ResetScaleAfterBake(Gol.FrontTrainDoorLeft, new Vector3(0.002207483f, 1f, 1));
                ResetScaleAfterBake(Gol.FrontTrainDoorRight, new Vector3(-0.03327911f, 1f, 1));

                // Reset static flags
                Gol.WorldArt.isStatic = false;
                SetStaticRecursively(Gol.WorldArt, false);

                Gol.HerbertBaseV2.isStatic = false;
                SetStaticRecursively(Gol.HerbertBaseV2, false);

                Gol.FrontTrainDoorLeft.isStatic = false;
                SetStaticRecursively(Gol.FrontTrainDoorLeft, false);

                Gol.FrontTrainDoorRight.isStatic = false;
                SetStaticRecursively(Gol.FrontTrainDoorRight, false);

                Gol.TrainBlue.isStatic = false;
                SetStaticRecursively(Gol.TrainBlue, false);

                Gol.Interactables.isStatic = false;
                SetStaticRecursively(Gol.Interactables, false);

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

    // Helper method to set scale during baking
    private static void SetScaleForBake(GameObject obj, Vector3 scale)
    {
        if (obj != null)
        {
            obj.transform.localScale = scale;
        }
    }

    // Helper method to reset scale after baking
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
}
