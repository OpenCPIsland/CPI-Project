using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DiscordController))]
public class DiscordControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the target object (DiscordController)
        DiscordController discordController = (DiscordController)target;

        // Begin custom editor layout
        EditorGUILayout.LabelField("Scene Name Mappings", EditorStyles.boldLabel);

        // Display a list of scene name mappings that can be modified
        for (int i = 0; i < discordController.customSceneNames.Length; i++)
        {
            // Start a horizontal layout for each scene name mapping
            EditorGUILayout.BeginHorizontal();

            // Custom fields for each scene name and display name
            discordController.customSceneNames[i].sceneName = EditorGUILayout.TextField("Scene Name", discordController.customSceneNames[i].sceneName);
            discordController.customSceneNames[i].displayName = EditorGUILayout.TextField("Display Name", discordController.customSceneNames[i].displayName);

            // Allow the user to remove an entry
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                // Remove the entry from the list
                var list = new System.Collections.Generic.List<DiscordController.SceneNameMapping>(discordController.customSceneNames);
                list.RemoveAt(i);
                discordController.customSceneNames = list.ToArray();
            }

            EditorGUILayout.EndHorizontal();
        }

        // Button to add a new mapping entry
        if (GUILayout.Button("Add Scene Mapping"))
        {
            var list = new System.Collections.Generic.List<DiscordController.SceneNameMapping>(discordController.customSceneNames);
            list.Add(new DiscordController.SceneNameMapping());
            discordController.customSceneNames = list.ToArray();
        }

        // Ensure changes are saved in the editor
        if (GUI.changed)
        {
            EditorUtility.SetDirty(discordController);
        }

        // Default inspector UI (if you still want to show the base inspector values)
        DrawDefaultInspector();
    }
}
