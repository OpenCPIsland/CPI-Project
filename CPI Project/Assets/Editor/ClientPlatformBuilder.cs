using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum ClientBuildPlatform
{
    Windows,
    Linux,
    Android,
    iOS,
    macOS,
    WebGL
}

[InitializeOnLoad]
public static class ClientPlatformBuilder
{
    private enum RunStep
    {
        Switch,
        WaitAfterSwitch,
        Generate,
        WaitAfterGenerate,
        Build
    }

    private const string PrefPrefix = "CPI.ClientBuild.";
    private const string RunPrefix = "CPI.ClientBuild.Run.";
    private const string RunActiveKey = RunPrefix + "Active";
    private const string RunQueueKey = RunPrefix + "Queue";
    private const string RunIndexKey = RunPrefix + "Index";
    private const string RunStepKey = RunPrefix + "Step";
    private const string RunBuiltKey = RunPrefix + "Built";
    private const string RunProgressKey = RunPrefix + "ProgressId";
    private const string RunWaitStartKey = RunPrefix + "WaitStart";
    private const string RunIdleSinceKey = RunPrefix + "IdleSince";
    private const string RunCancelKey = RunPrefix + "Cancel";
    private const float IdleSeconds = 2f;
    private const int StepsPerPlatform = 5;

    private static bool ticking;
    private static bool reloading;

    public static readonly ClientBuildPlatform[] AllPlatforms =
    {
        ClientBuildPlatform.Windows,
        ClientBuildPlatform.Linux,
        ClientBuildPlatform.Android
    };

    public static readonly ClientBuildPlatform[] OtherPlatforms =
    {
        ClientBuildPlatform.iOS,
        ClientBuildPlatform.macOS,
        ClientBuildPlatform.WebGL
    };

    static ClientPlatformBuilder()
    {
        AssemblyReloadEvents.beforeAssemblyReload += () => reloading = true;
        if (IsRunning)
        {
            EditorApplication.delayCall += Resume;
        }
    }

    public static bool IsRunning
    {
        get { return SessionState.GetBool(RunActiveKey, false); }
    }

    public static string RunStatus
    {
        get
        {
            if (!IsRunning)
            {
                return "";
            }
            int id = SessionState.GetInt(RunProgressKey, -1);
            if (id >= 0 && Progress.Exists(id))
            {
                return Progress.GetDescription(id);
            }
            return "Running...";
        }
    }

    public static string GetFolder(ClientBuildPlatform platform)
    {
        return EditorPrefs.GetString(PrefPrefix + platform + ".Folder", "");
    }

    public static void SetFolder(ClientBuildPlatform platform, string folder)
    {
        EditorPrefs.SetString(PrefPrefix + platform + ".Folder", folder ?? "");
    }

    public static string GetFileName(ClientBuildPlatform platform)
    {
        return EditorPrefs.GetString(PrefPrefix + platform + ".FileName", PlayerSettings.productName);
    }

    public static void SetFileName(ClientBuildPlatform platform, string fileName)
    {
        EditorPrefs.SetString(PrefPrefix + platform + ".FileName", fileName ?? "");
    }

    public static bool AndroidAppBundle
    {
        get { return EditorPrefs.GetBool(PrefPrefix + "Android.AppBundle", false); }
        set { EditorPrefs.SetBool(PrefPrefix + "Android.AppBundle", value); }
    }

    public static string AndroidKeystorePass
    {
        get { return EditorPrefs.GetString(PrefPrefix + "Android.KeystorePass", ""); }
        set { EditorPrefs.SetString(PrefPrefix + "Android.KeystorePass", value ?? ""); }
    }

    public static string AndroidKeyaliasPass
    {
        get { return EditorPrefs.GetString(PrefPrefix + "Android.KeyaliasPass", ""); }
        set { EditorPrefs.SetString(PrefPrefix + "Android.KeyaliasPass", value ?? ""); }
    }

    public static bool WaitForBackgroundTasks
    {
        get { return EditorPrefs.GetBool(PrefPrefix + "WaitForBackgroundTasks", true); }
        set { EditorPrefs.SetBool(PrefPrefix + "WaitForBackgroundTasks", value); }
    }

    public static int BackgroundTaskTimeout
    {
        get { return Mathf.Max(10, EditorPrefs.GetInt(PrefPrefix + "BackgroundTaskTimeout", 600)); }
        set { EditorPrefs.SetInt(PrefPrefix + "BackgroundTaskTimeout", Mathf.Max(10, value)); }
    }

    [MenuItem("Build/Windows", false, 0)]
    private static void BuildWindowsMenu()
    {
        Build(ClientBuildPlatform.Windows);
    }

    [MenuItem("Build/Linux", false, 1)]
    private static void BuildLinuxMenu()
    {
        Build(ClientBuildPlatform.Linux);
    }

    [MenuItem("Build/Android", false, 2)]
    private static void BuildAndroidMenu()
    {
        Build(ClientBuildPlatform.Android);
    }

    [MenuItem("Build/All Platforms", false, 20)]
    private static void BuildAllMenu()
    {
        Build(AllPlatforms);
    }

    [MenuItem("Build/Other Platforms/iOS", false, 30)]
    private static void BuildIOSMenu()
    {
        Build(ClientBuildPlatform.iOS);
    }

    [MenuItem("Build/Other Platforms/macOS", false, 31)]
    private static void BuildMacOSMenu()
    {
        Build(ClientBuildPlatform.macOS);
    }

    [MenuItem("Build/Other Platforms/WebGL", false, 32)]
    private static void BuildWebGLMenu()
    {
        Build(ClientBuildPlatform.WebGL);
    }

    [MenuItem("Build/Cancel Running Build", false, 45)]
    private static void CancelMenu()
    {
        Cancel();
    }

    [MenuItem("Build/Cancel Running Build", true)]
    private static bool CancelMenuValidate()
    {
        return IsRunning;
    }

    [MenuItem("Build/Settings...", false, 50)]
    private static void OpenSettingsMenu()
    {
        ClientBuildSettingsWindow.Open();
    }

    public static BuildTarget GetTarget(ClientBuildPlatform platform)
    {
        switch (platform)
        {
            case ClientBuildPlatform.Windows:
                return BuildTarget.StandaloneWindows64;
            case ClientBuildPlatform.Linux:
                return BuildTarget.StandaloneLinux64;
            case ClientBuildPlatform.Android:
                return BuildTarget.Android;
            case ClientBuildPlatform.iOS:
                return BuildTarget.iOS;
            case ClientBuildPlatform.macOS:
                return BuildTarget.StandaloneOSX;
            case ClientBuildPlatform.WebGL:
                return BuildTarget.WebGL;
            default:
                throw new ArgumentOutOfRangeException(nameof(platform));
        }
    }

    public static bool IsModuleInstalled(ClientBuildPlatform platform)
    {
        BuildTarget target = GetTarget(platform);
        return BuildPipeline.IsBuildTargetSupported(BuildPipeline.GetBuildTargetGroup(target), target);
    }

    public static string GetOutputPath(ClientBuildPlatform platform)
    {
        string folder = GetFolder(platform);
        string fileName = GetFileName(platform);
        if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(fileName))
        {
            return "";
        }

        fileName = fileName.Trim();
        switch (platform)
        {
            case ClientBuildPlatform.Windows:
                fileName = WithExtension(fileName, ".exe", ".exe");
                break;
            case ClientBuildPlatform.Linux:
                fileName = WithExtension(fileName, ".x86_64", ".x86_64");
                break;
            case ClientBuildPlatform.Android:
                fileName = WithExtension(fileName, AndroidAppBundle ? ".aab" : ".apk", ".apk", ".aab");
                break;
            case ClientBuildPlatform.macOS:
                fileName = WithExtension(fileName, ".app", ".app");
                break;
        }
        return Path.Combine(folder.Trim(), fileName);
    }

    private static string WithExtension(string fileName, string extension, params string[] replaceable)
    {
        string current = Path.GetExtension(fileName);
        if (replaceable.Any(e => string.Equals(e, current, StringComparison.OrdinalIgnoreCase)))
        {
            fileName = fileName.Substring(0, fileName.Length - current.Length);
        }
        return fileName + extension;
    }

    public static List<string> Validate(IEnumerable<ClientBuildPlatform> platforms)
    {
        List<string> errors = new List<string>();
        if (!EditorBuildSettings.scenes.Any(s => s.enabled))
        {
            errors.Add("No scenes are enabled in Build Settings.");
        }

        foreach (ClientBuildPlatform platform in platforms)
        {
            if (!IsModuleInstalled(platform))
            {
                errors.Add(platform + ": build support module is not installed.");
            }
            if (string.IsNullOrWhiteSpace(GetFolder(platform)))
            {
                errors.Add(platform + ": no output folder set.");
            }
            if (string.IsNullOrWhiteSpace(GetFileName(platform)))
            {
                errors.Add(platform + ": no file name set.");
            }
            if (platform == ClientBuildPlatform.Android &&
                PlayerSettings.Android.useCustomKeystore &&
                (string.IsNullOrEmpty(AndroidKeystorePass) || string.IsNullOrEmpty(AndroidKeyaliasPass)))
            {
                errors.Add("Android: custom keystore is enabled but the keystore or alias password is empty.");
            }
        }
        return errors;
    }

    public static void Build(params ClientBuildPlatform[] platforms)
    {
        if (IsRunning || BuildPipeline.isBuildingPlayer)
        {
            EditorUtility.DisplayDialog("Client Build", "A build is already running.", "OK");
            return;
        }

        List<string> errors = Validate(platforms);
        if (errors.Count > 0)
        {
            EditorUtility.DisplayDialog("Client Build", string.Join("\n", errors.ToArray()), "OK");
            ClientBuildSettingsWindow.Open();
            return;
        }

        SaveProject();

        SessionState.SetString(RunQueueKey, string.Join(",", platforms.Select(p => p.ToString()).ToArray()));
        SessionState.SetInt(RunIndexKey, 0);
        SessionState.SetInt(RunStepKey, (int)RunStep.Switch);
        SessionState.SetString(RunBuiltKey, "");
        SessionState.SetBool(RunCancelKey, false);
        SessionState.SetInt(RunProgressKey, Progress.Start("Client Build", "Starting..."));
        SessionState.SetBool(RunActiveKey, true);

        Debug.Log("[ClientBuild] Starting: " + string.Join(", ", platforms.Select(p => p.ToString()).ToArray()));
        Resume();
    }

    public static void Cancel()
    {
        if (!IsRunning)
        {
            return;
        }
        SessionState.SetBool(RunCancelKey, true);
        Resume();
    }

    private static void Resume()
    {
        if (!IsRunning)
        {
            return;
        }

        int id = SessionState.GetInt(RunProgressKey, -1);
        if (id < 0 || !Progress.Exists(id) || Progress.GetStatus(id) != Progress.Status.Running)
        {
            id = Progress.Start("Client Build", "Resuming...");
            SessionState.SetInt(RunProgressKey, id);
        }
        Progress.RegisterCancelCallback(id, OnCancelRequested);

        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
    }

    private static bool OnCancelRequested()
    {
        if (ticking || reloading)
        {
            Debug.Log("[ClientBuild] Ignored a cancel request during " + (reloading ? "a script reload" : "a build step") + ".");
            return false;
        }
        Debug.Log("[ClientBuild] Cancel requested from the progress bar.");
        SessionState.SetBool(RunCancelKey, true);
        return true;
    }

    private static void Tick()
    {
        if (ticking)
        {
            return;
        }
        if (!IsRunning)
        {
            EditorApplication.update -= Tick;
            return;
        }
        if (BuildPipeline.isBuildingPlayer)
        {
            return;
        }

        ClientBuildPlatform[] queue = GetQueue();
        int index = SessionState.GetInt(RunIndexKey, 0);
        RunStep step = (RunStep)SessionState.GetInt(RunStepKey, 0);

        if (SessionState.GetBool(RunCancelKey, false))
        {
            Finish(Progress.Status.Canceled, "Build canceled.");
            return;
        }
        if (index >= queue.Length)
        {
            Finish(Progress.Status.Succeeded, "Build finished.");
            return;
        }

        ClientBuildPlatform platform = queue[index];
        ticking = true;
        try
        {
            switch (step)
            {
                case RunStep.Switch:
                    Report(platform, index, queue.Length, step, "Switching platform...");
                    SwitchPlatform(platform);
                    BeginWait();
                    SetStep(RunStep.WaitAfterSwitch);
                    break;
                case RunStep.WaitAfterSwitch:
                    if (WaitForBackground(platform, index, queue.Length, step))
                    {
                        SetStep(RunStep.Generate);
                    }
                    break;
                case RunStep.Generate:
                    Report(platform, index, queue.Length, step, "Generating client asset bundles...");
                    GenerateClientBundles();
                    BeginWait();
                    SetStep(RunStep.WaitAfterGenerate);
                    break;
                case RunStep.WaitAfterGenerate:
                    if (WaitForBackground(platform, index, queue.Length, step))
                    {
                        SetStep(RunStep.Build);
                    }
                    break;
                case RunStep.Build:
                    Report(platform, index, queue.Length, step, "Saving project...");
                    SaveProject();
                    if (platform == ClientBuildPlatform.Android)
                    {
                        ApplyAndroidSettings();
                    }
                    Report(platform, index, queue.Length, step, "Building player...");
                    BuildPlayerFor(platform);
                    AddBuilt(platform);
                    SessionState.SetInt(RunIndexKey, index + 1);
                    SetStep(RunStep.Switch);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Finish(Progress.Status.Failed, platform + " failed during " + step + ". Check the Console.");
        }
        finally
        {
            ticking = false;
            EditorUtility.ClearProgressBar();
        }
    }

    private static ClientBuildPlatform[] GetQueue()
    {
        List<ClientBuildPlatform> queue = new List<ClientBuildPlatform>();
        foreach (string part in SessionState.GetString(RunQueueKey, "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            ClientBuildPlatform platform;
            if (Enum.TryParse(part.Trim(), out platform))
            {
                queue.Add(platform);
            }
        }
        return queue.ToArray();
    }

    private static void SetStep(RunStep step)
    {
        SessionState.SetInt(RunStepKey, (int)step);
    }

    private static void AddBuilt(ClientBuildPlatform platform)
    {
        string built = SessionState.GetString(RunBuiltKey, "");
        SessionState.SetString(RunBuiltKey, string.IsNullOrEmpty(built) ? platform.ToString() : built + ", " + platform);
    }

    private static void Report(ClientBuildPlatform platform, int index, int count, RunStep step, string description)
    {
        int id = SessionState.GetInt(RunProgressKey, -1);
        if (id < 0 || !Progress.Exists(id))
        {
            return;
        }
        float fraction = (index * StepsPerPlatform + (int)step) / (float)(count * StepsPerPlatform);
        Progress.Report(id, fraction, "[" + (index + 1) + "/" + count + "] " + platform + ": " + description);
    }

    private static void BeginWait()
    {
        SessionState.SetFloat(RunWaitStartKey, (float)EditorApplication.timeSinceStartup);
        SessionState.SetFloat(RunIdleSinceKey, -1f);
    }

    private static bool WaitForBackground(ClientBuildPlatform platform, int index, int count, RunStep step)
    {
        if (!WaitForBackgroundTasks)
        {
            return true;
        }

        float now = (float)EditorApplication.timeSinceStartup;
        float waitStart = SessionState.GetFloat(RunWaitStartKey, now);
        string busyWith = GetBusyReason();

        if (busyWith != null)
        {
            SessionState.SetFloat(RunIdleSinceKey, -1f);
            Report(platform, index, count, step, "Waiting for " + busyWith + "...");
        }
        else
        {
            float idleSince = SessionState.GetFloat(RunIdleSinceKey, -1f);
            if (idleSince < 0f)
            {
                SessionState.SetFloat(RunIdleSinceKey, now);
            }
            else if (now - idleSince >= IdleSeconds)
            {
                return true;
            }
            Report(platform, index, count, step, "Waiting for background tasks to settle...");
        }

        if (now - waitStart >= BackgroundTaskTimeout)
        {
            Debug.LogWarning("[ClientBuild] Gave up waiting for " + (busyWith ?? "background tasks") + " after " + BackgroundTaskTimeout + "s. Continuing.");
            return true;
        }
        return false;
    }

    private static string GetBusyReason()
    {
        if (EditorApplication.isCompiling)
        {
            return "script compilation";
        }
        if (EditorApplication.isUpdating)
        {
            return "asset import";
        }
        if (ShaderUtil.anythingCompiling)
        {
            return "shader compilation";
        }

        int ownId = SessionState.GetInt(RunProgressKey, -1);
        foreach (Progress.Item item in Progress.EnumerateItems())
        {
            if (item.id == ownId || item.parentId == ownId)
            {
                continue;
            }
            if (item.running)
            {
                return string.IsNullOrEmpty(item.name) ? "background task" : item.name;
            }
        }
        return null;
    }

    private static void Finish(Progress.Status status, string message)
    {
        int id = SessionState.GetInt(RunProgressKey, -1);
        string built = SessionState.GetString(RunBuiltKey, "");

        EditorApplication.update -= Tick;
        SessionState.EraseBool(RunActiveKey);
        SessionState.EraseString(RunQueueKey);
        SessionState.EraseInt(RunIndexKey);
        SessionState.EraseInt(RunStepKey);
        SessionState.EraseString(RunBuiltKey);
        SessionState.EraseInt(RunProgressKey);
        SessionState.EraseFloat(RunWaitStartKey);
        SessionState.EraseFloat(RunIdleSinceKey);
        SessionState.EraseBool(RunCancelKey);

        if (id >= 0 && Progress.Exists(id) && Progress.GetStatus(id) == Progress.Status.Running)
        {
            Progress.Finish(id, status);
        }
        EditorUtility.ClearProgressBar();

        if (!string.IsNullOrEmpty(built))
        {
            message += "\n\nBuilt: " + built;
        }

        if (status == Progress.Status.Failed)
        {
            Debug.LogError("[ClientBuild] " + message);
        }
        else
        {
            Debug.Log("[ClientBuild] " + message);
        }
        EditorUtility.DisplayDialog("Client Build", message, "OK");
    }

    private static void SwitchPlatform(ClientBuildPlatform platform)
    {
        BuildTarget target = GetTarget(platform);
        if (EditorUserBuildSettings.activeBuildTarget == target)
        {
            return;
        }
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target))
        {
            throw new InvalidOperationException("Failed to switch to " + platform + ".");
        }
    }

    private static void BuildPlayerFor(ClientBuildPlatform platform)
    {
        BuildTarget target = GetTarget(platform);
        string outputPath = GetOutputPath(platform);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = outputPath,
            target = target,
            targetGroup = BuildPipeline.GetBuildTargetGroup(target),
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(platform + " build " + report.summary.result + " with " + report.summary.totalErrors + " error(s).");
        }
        Debug.Log("[ClientBuild] " + platform + " built to " + outputPath + " (" + EditorUtility.FormatBytes((long)report.summary.totalSize) + ")");
    }

    private static void GenerateClientBundles()
    {
        MethodInfo method = typeof(GenerateAddressables).GetMethod("BuildAllAddressables", BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new MissingMethodException(nameof(GenerateAddressables), "BuildAllAddressables");
        }

        try
        {
            method.Invoke(null, null);
        }
        catch (TargetInvocationException e)
        {
            ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
        }
    }

    private static void ApplyAndroidSettings()
    {
        EditorUserBuildSettings.buildAppBundle = AndroidAppBundle;
        if (PlayerSettings.Android.useCustomKeystore)
        {
            PlayerSettings.Android.keystorePass = AndroidKeystorePass;
            PlayerSettings.Android.keyaliasPass = AndroidKeyaliasPass;
        }
    }

    private static void SaveProject()
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetSceneAt(i);
            if (scene.isDirty && !string.IsNullOrEmpty(scene.path))
            {
                EditorSceneManager.SaveScene(scene);
            }
        }
        AssetDatabase.SaveAssets();
    }
}
