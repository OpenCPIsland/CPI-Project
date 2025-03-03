using System.IO;
using UnityEditor;
using UnityEngine;
using Disney.Kelowna.Common;  // Include the correct namespace

public class PlatformSwitcher : MonoBehaviour
{
    [MenuItem("Project/AssetBundles/Generated/Client-side AssetBundles platform switch/Switch Platform")]
    public static void SwitchPlatform()
    {
        string platform = "";

#if UNITY_STANDALONE_WIN
        platform = "standalonewindows";
#elif UNITY_STANDALONE_OSX
            platform = "standaloneosx";
#elif UNITY_STANDALONE_LINUX
            platform = "standalonelinux";
#elif UNITY_ANDROID
            platform = "android";
#else
            platform = "unknown";
#endif

        // Modify the client_info.asset file
        ModifyClientInfoAsset(platform);

        // Modify the embedded_content_manifest.txt file
        ModifyTextFile(platform);

        // Save assets (this will save the client_info.asset and any other modified assets)
        AssetDatabase.SaveAssets();
    }

    private static void ModifyClientInfoAsset(string platform)
    {
        // Load the ClientInfo scriptable object
        var clientInfo = AssetDatabase.LoadAssetAtPath<ClientInfo>("Assets/Generated/Resources/Configuration/client_info.asset");

        if (clientInfo != null)
        {
            SerializedObject serializedObject = new SerializedObject(clientInfo);
            SerializedProperty platformProperty = serializedObject.FindProperty("Platform");
            platformProperty.stringValue = platform;

            // Save the updated asset
            serializedObject.ApplyModifiedProperties();
            Debug.Log("Platform set to: " + platform + " in client_info.asset");
        }
        else
        {
            Debug.LogError("client_info.asset not found.");
        }
    }

    private static void ModifyTextFile(string platform)
    {
        string txtFilePath = "Assets/Generated/Resources/Configuration/embedded_content_manifest.txt"; // Update with actual path

        if (File.Exists(txtFilePath))
        {
            string content = File.ReadAllText(txtFilePath);

            // Replace any existing platform string with the new one
            if (content.Contains("standalonewindows") || content.Contains("standalonelinux") || content.Contains("standaloneosx") || content.Contains("android"))
            {
                content = content.Replace("standalonewindows", platform);
                content = content.Replace("standalonelinux", platform);
                content = content.Replace("standaloneosx", platform);
                content = content.Replace("android", platform);

                File.WriteAllText(txtFilePath, content); // Save the modified content
                Debug.Log("Text file updated with platform: " + platform);
            }
            else
            {
                Debug.Log("Platform string not found in the file.");
            }
        }
        else
        {
            Debug.LogError(".txt file not found.");
        }
    }
}
