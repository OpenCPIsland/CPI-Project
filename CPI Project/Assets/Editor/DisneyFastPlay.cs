using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class DisneyFastPlay
{
    private const string BootScenePath = "Assets/Game/Core/Scenes/Boot.unity";
    private const string PreviousSceneKey = "DisneyFastPlay.PreviousScene";

    private const int TargetWidth = 1920;
    private const int TargetHeight = 1080;
    private const int MobileTargetWidth = 480;
    private const int MobileTargetHeight = 800;

    private static object previousPlayModeBehavior;

    static DisneyFastPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem("Project/Disney Fast Play")]
    public static void OpenBootSceneAndPlay()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        string currentScenePath =
            EditorSceneManager.GetActiveScene().path;

        if (!string.IsNullOrEmpty(currentScenePath) &&
            currentScenePath != BootScenePath)
        {
            EditorPrefs.SetString(
                PreviousSceneKey,
                currentScenePath
            );
        }

        if (!System.IO.File.Exists(BootScenePath))
        {
            Debug.LogError(
                $"Boot scene not found: {BootScenePath}"
            );

            return;
        }

        EditorWindow gameView = GetGameView();

        if (gameView != null)
        {
            SaveGameViewSettings(gameView);
            SetFastPlaySettings(gameView);
        }

        EditorSceneManager.OpenScene(
            BootScenePath,
            OpenSceneMode.Single
        );

        EditorApplication.isPlaying = true;
    }

    private static EditorWindow GetGameView()
    {
        Type gameViewType =
            typeof(Editor).Assembly.GetType(
                "UnityEditor.GameView"
            );

        if (gameViewType == null)
            return null;

        return EditorWindow.GetWindow(gameViewType);
    }

    private static void SaveGameViewSettings(
        EditorWindow gameView)
    {
        Type gameViewType =
            gameView.GetType();

        PropertyInfo enterPlayModeBehavior =
            gameViewType.GetProperty(
                "enterPlayModeBehavior",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (enterPlayModeBehavior != null)
        {
            previousPlayModeBehavior =
                enterPlayModeBehavior.GetValue(gameView);
        }
    }

    private static void SetFastPlaySettings(
        EditorWindow gameView)
    {
        Type gameViewType =
            gameView.GetType();

        bool isMobileBuildTarget =
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android ||
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS;

        SelectResolution(
            gameView,
            gameViewType,
            isMobileBuildTarget ? MobileTargetWidth : TargetWidth,
            isMobileBuildTarget ? MobileTargetHeight : TargetHeight
        );

        PropertyInfo vSyncEnabled =
            gameViewType.GetProperty(
                "vSyncEnabled",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (vSyncEnabled != null)
            vSyncEnabled.SetValue(
                gameView,
                true
            );

        PropertyInfo enterPlayModeBehavior =
            gameViewType.GetProperty(
                "enterPlayModeBehavior",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (enterPlayModeBehavior != null)
        {
            Type behaviorType =
                enterPlayModeBehavior.PropertyType;

            object playMaximized =
                Enum.Parse(
                    behaviorType,
                    "PlayMaximized"
                );

            enterPlayModeBehavior.SetValue(
                gameView,
                playMaximized
            );
        }

        gameView.Repaint();
    }

    private static void SelectResolution(
        EditorWindow gameView,
        Type gameViewType,
        int targetWidth,
        int targetHeight)
    {
        Type gameViewSizesType =
            typeof(Editor).Assembly.GetType(
                "UnityEditor.GameViewSizes"
            );

        if (gameViewSizesType == null)
        {
            Debug.LogWarning(
                "Disney Fast Play: UnityEditor.GameViewSizes type not found."
            );

            return;
        }

        Type scriptableSingletonType =
            typeof(ScriptableSingleton<>).MakeGenericType(
                gameViewSizesType
            );

        PropertyInfo instanceProperty =
            scriptableSingletonType.GetProperty(
                "instance",
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        object gameViewSizes =
            instanceProperty?.GetValue(null);

        if (gameViewSizes == null)
        {
            Debug.LogWarning(
                "Disney Fast Play: could not resolve GameViewSizes.instance."
            );

            return;
        }

        PropertyInfo currentGroupTypeProperty =
            gameViewSizesType.GetProperty(
                "currentGroupType",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        object currentGroupTypeValue =
            currentGroupTypeProperty?.GetValue(
                gameViewSizes
            );

        if (currentGroupTypeValue == null)
        {
            Debug.LogWarning(
                "Disney Fast Play: could not read GameViewSizes.currentGroupType."
            );

            return;
        }

        MethodInfo getGroupMethod =
            gameViewSizesType.GetMethod(
                "GetGroup",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        object currentGroup =
            getGroupMethod?.Invoke(
                gameViewSizes,
                new object[]
                {
                    currentGroupTypeValue
                }
            );

        if (currentGroup == null)
        {
            Debug.LogWarning(
                "Disney Fast Play: GetGroup returned null for " +
                currentGroupTypeValue
            );

            return;
        }

        Type groupType =
            currentGroup.GetType();

        MethodInfo getTotalCount =
            groupType.GetMethod(
                "GetTotalCount",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        MethodInfo getGameViewSize =
            groupType.GetMethod(
                "GetGameViewSize",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        PropertyInfo selectedSizeIndex =
            gameViewType.GetProperty(
                "selectedSizeIndex",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (getTotalCount == null ||
            getGameViewSize == null ||
            selectedSizeIndex == null)
        {
            Debug.LogWarning(
                "Disney Fast Play: required GameView/GameViewSizeGroup members not found."
            );

            return;
        }

        int totalCount =
            (int)getTotalCount.Invoke(
                currentGroup,
                null
            );

        for (int i = 0; i < totalCount; i++)
        {
            object size =
                getGameViewSize.Invoke(
                    currentGroup,
                    new object[]
                    {
                        i
                    }
                );

            if (size == null)
                continue;

            Type sizeType =
                size.GetType();

            PropertyInfo widthProperty =
                sizeType.GetProperty(
                    "width",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

            PropertyInfo heightProperty =
                sizeType.GetProperty(
                    "height",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

            if (widthProperty == null ||
                heightProperty == null)
            {
                continue;
            }

            int width =
                (int)widthProperty.GetValue(size);

            int height =
                (int)heightProperty.GetValue(size);

            if (width == targetWidth &&
                height == targetHeight)
            {
                selectedSizeIndex.SetValue(
                    gameView,
                    i
                );

                return;
            }
        }

        MethodInfo getDisplayTexts =
            groupType.GetMethod(
                "GetDisplayTexts",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        string[] displayTexts =
            getDisplayTexts?.Invoke(
                currentGroup,
                null
            ) as string[];

        Debug.LogWarning(
            $"Disney Fast Play could not find a {targetWidth}x{targetHeight} size in group {currentGroupTypeValue}. Available: " +
            (displayTexts != null
                ? string.Join(", ", displayTexts)
                : "unknown")
        );
    }

    private static void RestoreGameViewSettings()
    {
        EditorWindow gameView =
            GetGameView();

        if (gameView == null)
            return;

        Type gameViewType =
            gameView.GetType();

        if (previousPlayModeBehavior != null)
        {
            PropertyInfo enterPlayModeBehavior =
                gameViewType.GetProperty(
                    "enterPlayModeBehavior",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic
                );

            if (enterPlayModeBehavior != null)
            {
                enterPlayModeBehavior.SetValue(
                    gameView,
                    previousPlayModeBehavior
                );
            }
        }

        gameView.Repaint();
    }

    private static void RestorePreviousScene()
    {
        string previousScenePath =
            EditorPrefs.GetString(
                PreviousSceneKey,
                string.Empty
            );

        if (string.IsNullOrEmpty(previousScenePath))
            return;

        EditorPrefs.DeleteKey(
            PreviousSceneKey
        );

        if (!System.IO.File.Exists(previousScenePath))
        {
            Debug.LogWarning(
                $"Disney Fast Play: previous scene no longer exists: {previousScenePath}"
            );

            return;
        }

        EditorSceneManager.OpenScene(
            previousScenePath,
            OpenSceneMode.Single
        );
    }

    private static void OnPlayModeStateChanged(
        PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            RestoreGameViewSettings();
            RestorePreviousScene();
            RestoreGameViewSettings();

            EditorWindow.FocusWindowIfItsOpen<SceneView>();
        }
    }
}