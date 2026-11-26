using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class ClientBuildSettingsWindow : EditorWindow
{
    private Vector2 scroll;

    public static void Open()
    {
        ClientBuildSettingsWindow window = GetWindow<ClientBuildSettingsWindow>("Client Build");
        window.minSize = new Vector2(460f, 540f);
        window.Show();
    }

    private void OnInspectorUpdate()
    {
        if (ClientPlatformBuilder.IsRunning)
        {
            Repaint();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Active platform: " + EditorUserBuildSettings.activeBuildTarget, MessageType.None);
        DrawRunStatus();

        scroll = EditorGUILayout.BeginScrollView(scroll);
        DrawBackgroundSettings();
        EditorGUILayout.Space();

        foreach (ClientBuildPlatform platform in ClientPlatformBuilder.AllPlatforms)
        {
            DrawPlatform(platform);
            EditorGUILayout.Space();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Platforms", EditorStyles.largeLabel);
        foreach (ClientBuildPlatform platform in ClientPlatformBuilder.OtherPlatforms)
        {
            DrawPlatform(platform);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

        DrawBuildButtons();
    }

    private void DrawRunStatus()
    {
        if (!ClientPlatformBuilder.IsRunning)
        {
            return;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(ClientPlatformBuilder.RunStatus, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Cancel", GUILayout.Width(70f)))
            {
                ClientPlatformBuilder.Cancel();
            }
        }
    }

    private void DrawBackgroundSettings()
    {
        EditorGUILayout.LabelField("Background Tasks", EditorStyles.boldLabel);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUI.BeginChangeCheck();
            bool wait = EditorGUILayout.Toggle("Wait Before Each Step", ClientPlatformBuilder.WaitForBackgroundTasks);
            if (EditorGUI.EndChangeCheck())
            {
                ClientPlatformBuilder.WaitForBackgroundTasks = wait;
            }

            using (new EditorGUI.DisabledScope(!ClientPlatformBuilder.WaitForBackgroundTasks))
            {
                EditorGUI.BeginChangeCheck();
                int timeout = EditorGUILayout.IntField("Timeout (seconds)", ClientPlatformBuilder.BackgroundTaskTimeout);
                if (EditorGUI.EndChangeCheck())
                {
                    ClientPlatformBuilder.BackgroundTaskTimeout = timeout;
                }
            }
        }
    }

    private void DrawPlatform(ClientBuildPlatform platform)
    {
        EditorGUILayout.LabelField(platform.ToString(), EditorStyles.boldLabel);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (!ClientPlatformBuilder.IsModuleInstalled(platform))
            {
                EditorGUILayout.HelpBox(platform + " build support is not installed for this Unity version.", MessageType.Warning);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                string folder = EditorGUILayout.TextField("Output Folder", ClientPlatformBuilder.GetFolder(platform));
                if (EditorGUI.EndChangeCheck())
                {
                    ClientPlatformBuilder.SetFolder(platform, folder);
                }

                if (GUILayout.Button("Browse", GUILayout.Width(70f)))
                {
                    string current = ClientPlatformBuilder.GetFolder(platform);
                    string picked = EditorUtility.OpenFolderPanel(platform + " Output Folder", Directory.Exists(current) ? current : "", "");
                    if (!string.IsNullOrEmpty(picked))
                    {
                        ClientPlatformBuilder.SetFolder(platform, picked);
                        GUI.FocusControl(null);
                    }
                    GUIUtility.ExitGUI();
                }
            }

            EditorGUI.BeginChangeCheck();
            string fileName = EditorGUILayout.TextField(GetFileNameLabel(platform), ClientPlatformBuilder.GetFileName(platform));
            if (EditorGUI.EndChangeCheck())
            {
                ClientPlatformBuilder.SetFileName(platform, fileName);
            }

            if (platform == ClientBuildPlatform.Android)
            {
                DrawAndroid();
            }

            string outputPath = ClientPlatformBuilder.GetOutputPath(platform);
            EditorGUILayout.LabelField("Output", string.IsNullOrEmpty(outputPath) ? "-" : outputPath, EditorStyles.miniLabel);
        }
    }

    private static string GetFileNameLabel(ClientBuildPlatform platform)
    {
        switch (platform)
        {
            case ClientBuildPlatform.iOS:
                return "Xcode Project Name";
            case ClientBuildPlatform.WebGL:
                return "Build Folder Name";
            default:
                return "File Name";
        }
    }

    private void DrawAndroid()
    {
        EditorGUI.BeginChangeCheck();
        bool appBundle = EditorGUILayout.Toggle("Build App Bundle (.aab)", ClientPlatformBuilder.AndroidAppBundle);
        if (EditorGUI.EndChangeCheck())
        {
            ClientPlatformBuilder.AndroidAppBundle = appBundle;
        }

        if (!PlayerSettings.Android.useCustomKeystore)
        {
            EditorGUILayout.LabelField("Signing", "Debug keystore", EditorStyles.miniLabel);
            return;
        }

        EditorGUILayout.LabelField("Keystore", PlayerSettings.Android.keystoreName, EditorStyles.miniLabel);
        EditorGUILayout.LabelField("Alias", PlayerSettings.Android.keyaliasName, EditorStyles.miniLabel);

        EditorGUI.BeginChangeCheck();
        string keystorePass = EditorGUILayout.PasswordField("Keystore Password", ClientPlatformBuilder.AndroidKeystorePass);
        if (EditorGUI.EndChangeCheck())
        {
            ClientPlatformBuilder.AndroidKeystorePass = keystorePass;
        }

        EditorGUI.BeginChangeCheck();
        string keyaliasPass = EditorGUILayout.PasswordField("Alias Password", ClientPlatformBuilder.AndroidKeyaliasPass);
        if (EditorGUI.EndChangeCheck())
        {
            ClientPlatformBuilder.AndroidKeyaliasPass = keyaliasPass;
        }

        EditorGUILayout.LabelField("Passwords are stored in EditorPrefs as plain text.", EditorStyles.miniLabel);
    }

    private void DrawBuildButtons()
    {
        using (new EditorGUI.DisabledScope(ClientPlatformBuilder.IsRunning || BuildPipeline.isBuildingPlayer))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (ClientBuildPlatform platform in ClientPlatformBuilder.AllPlatforms)
                {
                    if (GUILayout.Button("Build " + platform, GUILayout.Height(28f)))
                    {
                        Queue(platform);
                    }
                }

                if (GUILayout.Button("Build All", GUILayout.Height(28f)))
                {
                    Queue(ClientPlatformBuilder.AllPlatforms);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (ClientBuildPlatform platform in ClientPlatformBuilder.OtherPlatforms)
                {
                    if (GUILayout.Button("Build " + platform, GUILayout.Height(24f)))
                    {
                        Queue(platform);
                    }
                }
            }
        }
    }

    private static void Queue(params ClientBuildPlatform[] platforms)
    {
        EditorApplication.delayCall += () => ClientPlatformBuilder.Build(platforms);
    }
}
