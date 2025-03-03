using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kamgam.LEF
{
    public enum LogLevel
    {
        Log = 0,
        Warning = 1,
        Error = 2,
        Message = 3,
        NoLogs = 99
    }

    public delegate void LogCallback(string message, LogLevel logLevel = LogLevel.Log);

    public static class LineEndingFixer
    {
        public static void LogMessage(string message, LogLevel logLevel = LogLevel.Log)
        {
            var settingsLogLevel = LineEndingFixerSettings.GetOrCreateSettings().LogLevel;

            if (logLevel < settingsLogLevel)
                return;

            switch (logLevel)
            {
                case LogLevel.Log:
                    Debug.Log("LineEndingFixer: " + message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning("LineEndingFixer: " + message);
                    break;
                case LogLevel.Error:
                    Debug.LogError("LineEndingFixer: " + message);
                    break;
                case LogLevel.Message:
                    Debug.Log("LineEndingFixer: " + message);
                    break;
                default:
                    break;
            }
        }

        [InitializeOnLoadMethod]
        public static void InitializeOnLoad()
        {
            Application.logMessageReceived -= onLogMessageReceived;
            Application.logMessageReceived += onLogMessageReceived;
        }

        static string filePathToAutoFix;
        static int updateCountDown;

        static void onLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Warning)
                return;

            var settings = LineEndingFixerSettings.GetOrCreateSettings();
            if (!settings.AutoFixWarnings)
                return;

            if (!condition.Contains("line endings"))
                return;

            var pathParts = condition.Split('\'');
            if (pathParts.Length >= 3)
            {
                // We do these delay shenanigans to make the LineFixer messages appear AFTER the warning log in the console.
                // Otherwise it would appear before the warning and the user would wonder which came first.
                EditorApplication.update -= onAutoFixSingleFile;
                EditorApplication.update += onAutoFixSingleFile;
                updateCountDown = 10;
                filePathToAutoFix = pathParts[1];
            }
        }

        static void onAutoFixSingleFile()
        {
            if (updateCountDown-- > 0)
                return;

            EditorApplication.update -= onAutoFixSingleFile;

            bool fixNow = EditorUtility.DisplayDialog("Fix line endings now?", "LineEndingFixer detected a file (" + filePathToAutoFix + ") with mixed line endings.\n\nShould it be fixed for you?", "Yes (fix it now)", "Cancel");
            if (fixNow)
            {
                if (string.IsNullOrEmpty(filePathToAutoFix))
                    return;

                var settings = LineEndingFixerSettings.GetOrCreateSettings();
                SingleFileFixer.FixLineEndings(filePathToAutoFix, settings.LineEnding, settings.DefaultEncoding);
                filePathToAutoFix = null;
            }
        }

        [MenuItem("Assets/Fix Line Endings", priority = 1200)]
        public static void FixLineEndingsMenu()
        {
            var settings = LineEndingFixerSettings.GetOrCreateSettings();

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                SingleFileFixer.FixLineEndings(path, settings.LineEnding, settings.DefaultEncoding);
            }
        }

        [MenuItem("Assets/Fix Line Endings", validate = true)]
        public static bool FixLineEndingsMenuValidate()
        {
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.EndsWith(".cs"))
                    return true;
            }

            return false;
        }

        public static async Task<List<ScriptFile>> ScanForBrokenScripts(CancellationToken ct, List<string> scanLogs)
        {
            var results = new List<ScriptFile>();

            var scriptGUIDs = AssetDatabase.FindAssets("t:script", new string[] {"Assets"});
            if (scriptGUIDs.Length == 0)
            {
                scanLogs.Add("No Sript Assets found");
                return results;
            }

            for (int i = 0; i < scriptGUIDs.Length; i++)
            {
                if (ct.IsCancellationRequested)
                    break;

                var path = AssetDatabase.GUIDToAssetPath(scriptGUIDs[i]);
                try
                {
                    scanLogs.Add("Scanning '" + path + "'.");

                    // analyze
                    var text = File.ReadAllText(SingleFileFixer.GetFullAssetPath(path));
                    int rCount, nCount, rnCount;
                    bool hasMixedLineEndings;
                    SingleFileFixer.AnalyzeLineEndings(text, out rCount, out nCount, out rnCount, out hasMixedLineEndings);

                    // add if it has mixed line endings
                    if (hasMixedLineEndings)
                    {
                        scanLogs.Add(
                            string.Format("  Mixed line endings found. It contains {0} \\r, {1} \\n and {2} \\r\\n lines.", rCount, nCount, rnCount)
                            );
                        results.Add(new ScriptFile(scriptGUIDs[i], path));
                    }

                    // async
                    await Task.Delay(1);
                    LineEndingFixerWindow.GetOrOpen().Repaint();

                    // cancel
                    if (ct.IsCancellationRequested)
                        break;
                }
                catch (System.Exception e)
                {
                    scanLogs.Add("  ERROR: Could access not file. " + e.Message + " Path: '" + path + "'");

                    bool isAccessDenied = e.Message.Contains("ccess") && e.Message.Contains("denied");
                    if (isAccessDenied)
                    {
                        string msg = "    Maybe your IDE is locking the file?. Please try to close it or make it unlock the file.";
                        scanLogs.Add(msg);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return results;
        }
    }
}
