using UnityEditor;
using UnityEngine;

namespace Kamgam.LEF
{
    public enum LineEndingBehaviour { Majority, OSDefault, Windows, Unix }

    public enum TextEncoding {
        Default, Unicode, BigEndianUnicode, UTF7, UTF8, UTF32, ASCII
    }

    public class LineEndingFixerSettings : ScriptableObject
    {
        public const string Version = "1.1.0";
        public const string SettingsFilePath = "Assets/LineEndingFixerSettings.asset";
        protected static LineEndingFixerSettings cachedSettings;

        [SerializeField]
        public LogLevel LogLevel = LogLevel.Warning;

        [SerializeField, Tooltip(_LineEndingTooltip)]
        public LineEndingBehaviour LineEnding;
        public const string _LineEndingTooltip = "How is the new line ending sequence chosen?" +
            "\n\nMajority:\nWill convert all line endings to the same as the majority of lines." +
            "\n\nOSDefault:\nWill convert all line endings to the OS default (most likely \\r\\n on Windows \\n on Mac and Unix)." +
            "\n\nWindows:\nWill convert all line endings to \\r\\n." +
            "\n\nUnix:\nWill convert all line endings to \\n.";

        [SerializeField, Tooltip(_AutoFixWarnings)]
        public bool AutoFixWarnings = true;
        public const string _AutoFixWarnings = "Should the tool offer to auto-fix after the 'inconsitent line endings' warning is shown?";

        [SerializeField, Tooltip(_DefaultEncoding)]
        protected TextEncoding defaultEncoding;
        public const string _DefaultEncoding = "The tool will assume your files are using this encoding.";
        public System.Text.Encoding DefaultEncoding
        {
            get
            {
                switch (defaultEncoding)
                {
                    case TextEncoding.Default:
                        return System.Text.Encoding.Default;
                    case TextEncoding.Unicode:
                        return System.Text.Encoding.Unicode;
                    case TextEncoding.BigEndianUnicode:
                        return System.Text.Encoding.BigEndianUnicode;
                    case TextEncoding.UTF7:
                        return System.Text.Encoding.UTF7;
                    case TextEncoding.UTF8:
                        return System.Text.Encoding.UTF8;
                    case TextEncoding.UTF32:
                        return System.Text.Encoding.UTF32;
                    case TextEncoding.ASCII:
                        return System.Text.Encoding.ASCII;
                    default:
                        return System.Text.Encoding.Default;
                }
            }
        }

        public static LineEndingFixerSettings GetOrCreateSettings()
        {
            if (cachedSettings == null)
            {
                cachedSettings = AssetDatabase.LoadAssetAtPath<LineEndingFixerSettings>(SettingsFilePath);

                // Not found? Then search for it.
                if (cachedSettings == null)
                {
                    string[] results = AssetDatabase.FindAssets("t:LineEndingFixerSettings");
                    if (results.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(results[0]);
                        cachedSettings = AssetDatabase.LoadAssetAtPath<LineEndingFixerSettings>(path);
                    }
                }

                // Still not found? Then create settings.
                if (cachedSettings == null)
                {
                    cachedSettings = ScriptableObject.CreateInstance<LineEndingFixerSettings>();
                    cachedSettings.LogLevel = LogLevel.Warning;
                    cachedSettings.LineEnding = LineEndingBehaviour.Majority;
                    cachedSettings.defaultEncoding = TextEncoding.Default;

                    AssetDatabase.CreateAsset(cachedSettings, SettingsFilePath);
                    AssetDatabase.SaveAssets();
                }

                if (cachedSettings == null)
                {
                    EditorUtility.DisplayDialog("Error", "LineBreakFixer settings could not be found or created.", "Ok");
                }
            }
            return cachedSettings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        // settings
        public static void SelectSettings()
        {
            var settings = LineEndingFixerSettings.GetOrCreateSettings();
            if (settings != null)
            {
                Selection.activeObject = settings;
                EditorGUIUtility.PingObject(settings);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "LineBreakFixer settings could not be found or created.", "Ok");
            }
        }
    }

    static class LineEndingFixerSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateLineEndingFixerSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Line Break Fixer", SettingsScope.Project)
            {
                label = "Line Break Fixer",
                guiHandler = (searchContext) =>
                {
                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;

                    var settings = LineEndingFixerSettings.GetSerializedSettings();

                    EditorGUILayout.LabelField("Version: " + LineEndingFixerSettings.Version);

                    EditorGUILayout.PropertyField(settings.FindProperty("LogLevel"), new GUIContent("Log level:"));

                    EditorGUILayout.PropertyField(settings.FindProperty("LineEnding"), new GUIContent("Line Ending:"));
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(LineEndingFixerSettings._LineEndingTooltip, style);
                    GUILayout.EndVertical();

                    EditorGUILayout.PropertyField(settings.FindProperty("AutoFixWarnings"), new GUIContent("Auto Fix Warnings:"));
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(LineEndingFixerSettings._AutoFixWarnings, style);
                    GUILayout.EndVertical();

                    EditorGUILayout.PropertyField(settings.FindProperty("defaultEncoding"), new GUIContent("Default Encoding:"));
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(LineEndingFixerSettings._DefaultEncoding, style);
                    GUILayout.EndVertical();

                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting.
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "line break", "new line", "line ending", "line", "newline", "fix", "fixer", "script" })
            };

            return provider;
        }
    }
}