using UnityEditor;
using UnityEngine;

public class ClaimableRewardIdUpdaterTo32 : MonoBehaviour
{
    [MenuItem("Project/Rewards/Update CPIFThrowback ID to 32")]
    public static void UpdateClaimableRewardId()
    {
        string assetPath = "Assets/Game/Rewards/Resources/Definitions/ClaimableReward/ClaimableReward_39_CPIFThrowback.asset";

        var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
        if (asset != null)
        {
            SerializedObject serializedObject = new SerializedObject(asset);
            SerializedProperty idProperty = serializedObject.FindProperty("DateDefinitionKey.Id");

            if (idProperty != null)
            {
                if (idProperty.intValue == 7)
                {
                    idProperty.intValue = 32;
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Updated ID from 7 to 32 in asset: {assetPath}");
                }
                else
                {
                    Debug.Log($"Asset already has ID set to {idProperty.intValue}, no change needed.");
                }
            }
            else
            {
                Debug.LogError("Could not find property 'DateDefinitionKey.Id' in the asset.");
            }
        }
        else
        {
            Debug.LogError($"Failed to load asset at path: {assetPath}");
        }
    }
}
