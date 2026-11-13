using System.IO;
using UnityEditor;
using UnityEngine;

public class SwitchToMedievalParty2026Audio : MonoBehaviour
{
    [MenuItem("Project/Events/Medieval Party 2026/Switch to the Medieval Party 2026 Audio")]
    public static void UpdateSceneAudioPaths()
    {
        // Define the list of scene asset paths
        string[] scenePaths = new string[]
        {
           // "Assets/Game/World/Resources/Definitions/Scene/Scene_Boardwalk.asset",
           // "Assets/Game/World/Resources/Definitions/Scene/Scene_Diving.asset",
           // "Assets/Game/World/Resources/Definitions/Scene/Scene_MtBlizzard.asset",
            "Assets/Game/World/Resources/Definitions/Scene/Scene_Town.asset"
        };

        // Loop through each scene path and modify the audio path
        foreach (var scenePath in scenePaths)
        {
            ModifySceneAudioPath(scenePath);
        }

        // Save all assets after modification
        AssetDatabase.SaveAssets();
    }

    private static void ModifySceneAudioPath(string scenePath)
    {
        // Load the Scene asset (replace with the correct type if necessary)
        var scene = AssetDatabase.LoadAssetAtPath<ScriptableObject>(scenePath);

        if (scene != null)
        {
            // Get the scene name from the path (e.g., "Boardwalk", "Diving", etc.)
            string sceneName = Path.GetFileNameWithoutExtension(scenePath).Split('_')[1];

            // Construct the new audio path
            string newAudioPath = $"Audio/TemporaryContent/MedievalParty_2026/{sceneName}/Audio.{sceneName}";

            // Create a SerializedObject for the scene asset
            SerializedObject serializedObject = new SerializedObject(scene);

            // Find the SceneAudioContentKey property (replace with the correct property name if necessary)
            SerializedProperty audioContentKeyProperty = serializedObject.FindProperty("SceneAudioContentKey.Key");

            if (audioContentKeyProperty != null)
            {
                // Update the Key value with the new audio path
                audioContentKeyProperty.stringValue = newAudioPath;

                // Apply the changes to the serialized object
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"Audio path for {sceneName} updated to: {newAudioPath}");
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
