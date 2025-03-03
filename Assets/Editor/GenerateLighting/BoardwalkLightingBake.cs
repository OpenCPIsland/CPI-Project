using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

public class BoardwalkLightingBake : MonoBehaviour
{
    // Create a new drop-down menu in Editor named "Examples" and a new option called "Open Scene"
    [MenuItem("Project/Generate lighting/Lightmap baking/Boardwalk")]
    static void OpenScene()
    {
        // Open the Scene in the Editor (do not enter Play Mode)
        BakeBoardwalk();
    }

    static void BakeBoardwalk()
    {
        string scenePath = "Assets/Game/World/Scenes/Boardwalk.unity";
        EditorSceneManager.OpenScene(scenePath);
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
            // Replace "GameObjectLocations" with the name of the GameObject you want to find
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

                // Get the folder path where the scene is located
                string sceneFolderPath = Path.GetDirectoryName(scenePath);

                // Delete only .exr files and LightingData.asset inside the folder containing the scene
                string[] files = Directory.GetFiles(sceneFolderPath, "*", SearchOption.AllDirectories);

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

                Gol.LighthouseIce.transform.localPosition = new Vector3(Gol.LighthouseIce.transform.localPosition.x, 100, Gol.LighthouseIce.transform.localPosition.z);
                Gol.LighthouseUpperDoorLightOff.transform.localPosition = new Vector3(Gol.LighthouseUpperDoorLightOff.transform.localPosition.x, 100, Gol.LighthouseUpperDoorLightOff.transform.localPosition.z);
                Gol.LighthouseGlassWallsLightOff.transform.localPosition = new Vector3(Gol.LighthouseGlassWallsLightOff.transform.localPosition.x, 100, Gol.LighthouseGlassWallsLightOff.transform.localPosition.z);
                Gol.funHutSpeakerTowerA.transform.localPosition = new Vector3(Gol.funHutSpeakerTowerA.transform.localPosition.x, 100, Gol.funHutSpeakerTowerA.transform.localPosition.z);
                Gol.funHutSpeakerTowerB.transform.localPosition = new Vector3(Gol.funHutSpeakerTowerB.transform.localPosition.x, 100, Gol.funHutSpeakerTowerB.transform.localPosition.z);

                Gol.WorldArt.isStatic = true;
                SetStaticRecursively(Gol.WorldArt, true);

                Gol.funHutSpeakerTowerA.isStatic = true;
                SetStaticRecursively(Gol.funHutSpeakerTowerA, true);
                Gol.funHutSpeakerTowerB.isStatic = true;
                SetStaticRecursively(Gol.funHutSpeakerTowerB, true);

                Gol.ChangeSource(AmbientMode.Skybox);

                // Bake the lightmap
                Lightmapping.Bake();

                // Reset settings after baking
                Gol.ChangeSkybox(Gol.DayCubemap);

                Gol.LighthouseIce.transform.localPosition = new Vector3(Gol.LighthouseIce.transform.localPosition.x, 0.006f, Gol.LighthouseIce.transform.localPosition.z);
                Gol.LighthouseUpperDoorLightOff.transform.localPosition = new Vector3(Gol.LighthouseUpperDoorLightOff.transform.localPosition.x, 6.638884f, Gol.LighthouseUpperDoorLightOff.transform.localPosition.z);
                Gol.LighthouseGlassWallsLightOff.transform.localPosition = new Vector3(Gol.LighthouseGlassWallsLightOff.transform.localPosition.x, -0.00000023841858f, Gol.LighthouseGlassWallsLightOff.transform.localPosition.z);
                Gol.funHutSpeakerTowerA.transform.localPosition = new Vector3(Gol.funHutSpeakerTowerA.transform.localPosition.x, 0, Gol.funHutSpeakerTowerA.transform.localPosition.z);
                Gol.funHutSpeakerTowerB.transform.localPosition = new Vector3(Gol.funHutSpeakerTowerB.transform.localPosition.x, 0, Gol.funHutSpeakerTowerB.transform.localPosition.z);

                Gol.WorldArt.isStatic = false;
                SetStaticRecursively(Gol.WorldArt, false);

                Gol.funHutSpeakerTowerA.isStatic = false;
                SetStaticRecursively(Gol.funHutSpeakerTowerA, false);
                Gol.funHutSpeakerTowerB.isStatic = false;
                SetStaticRecursively(Gol.funHutSpeakerTowerB, false);

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
