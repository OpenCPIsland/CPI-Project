using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneAudioPathReverter : MonoBehaviour
{
    [MenuItem("Project/Events/Default/Revert to the Default Audio")]
    public static void RevertSceneAudioPaths()
    {
        // Define the list of scene asset paths
        string[] scenePaths = new string[]
        {
            "Assets/Game/World/Resources/definitions/scene/Scene_Boardwalk.asset",
            "Assets/Game/World/Resources/definitions/scene/Scene_Diving.asset",
            "Assets/Game/World/Resources/definitions/scene/Scene_MtBlizzard.asset",
            "Assets/Game/World/Resources/definitions/scene/Scene_Town.asset"
        };

        // Loop through each scene path and revert the audio path
        foreach (var scenePath in scenePaths)
        {
            RevertSceneAudioPath(scenePath);
        }

        // Save all assets after modification
        AssetDatabase.SaveAssets();
    }

    private static void RevertSceneAudioPath(string scenePath)
    {
        // Load the Scene asset (replace with the correct type if necessary)
        var scene = AssetDatabase.LoadAssetAtPath<ScriptableObject>(scenePath);

        if (scene != null)
        {
            // Get the scene name from the path (e.g., "Boardwalk", "Diving", etc.)
            string sceneName = Path.GetFileNameWithoutExtension(scenePath).Split('_')[1];

            // Construct the default audio path
            string defaultAudioPath = $"Audio/{sceneName}/Audio.{sceneName}";

            // Create a SerializedObject for the scene asset
            SerializedObject serializedObject = new SerializedObject(scene);

            // Find the SceneAudioContentKey property (replace with the correct property name if necessary)
            SerializedProperty audioContentKeyProperty = serializedObject.FindProperty("SceneAudioContentKey.Key");

            if (audioContentKeyProperty != null)
            {
                // Revert the Key value to the default audio path
                audioContentKeyProperty.stringValue = defaultAudioPath;

                // Apply the changes to the serialized object
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"Audio path for {sceneName} reverted to: {defaultAudioPath}");
            }
            else
            {
                Debug.LogError($"SceneAudioContentKey.Key property not found in {sceneName}.");
            }
        }
        else
        {
            Debug.LogError($"Scene asset not found at path: {scenePath}");
        }
    }
}
