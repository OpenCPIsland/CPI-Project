using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Stores per project settings.
    /// EditorPrefs are universal so not well suited to per project settings.
    /// NOTE: This class is included as source and cannot be accessed from dll
    /// So it's intended for use by startup and install scripts.
    /// </summary>
    [Serializable]
    public class EditorStartupPrefs : ScriptableObject
    {
        /// <summary>
        /// Path to a separate VersionInfo.txt file.
        /// This file stores the version of Unity the installer was exported from.
        /// We can't use a property for this because ScriptableObjects saved in newer versions of Unity might not load in older versions,
        /// then the EditorStartupPrefs is reset and the unity version is lost (e.g. if importing a 2018.3 package in 5.3).
        /// So instead we store the info in a plain text file for maximum compatibility.
        /// </summary>
        private const string versionInfoPath = "Assets/PlayMaker/Editor/Resources/VersionInfo.txt";
        private const string versionInfoFile = "VersionInfo";

        private static EditorStartupPrefs instance;

        public static EditorStartupPrefs Instance
        {
            get
            {
                if (instance == null)
                {
                    //UnityEngine.Debug.Log("Load PlayMakerEditorPrefs");
                    instance = Resources.Load<EditorStartupPrefs>("EditorStartupPrefs");
                    if (instance == null)
                    {
                        instance = CreateInstance<EditorStartupPrefs>();

                        // Note: Save will be called after importing unitypackage
                        // when the WelcomeScreen opens and sets WelcomeScreenVersion
                    }

                    // Newer versions of Unity complain about this:
                    instance.name = "EditorStartupPrefs";
                }
                return instance;
            }
        }

        public static string UnityBuildVersion
        {
            /* We can't just do this, see note above
            get { return Instance.unityBuildVersion; }
            private set { Instance.unityBuildVersion = value; Save();}
            */

            get
            {
                var asset = Resources.Load<TextAsset>(versionInfoFile);
                if (asset == null) return String.Empty;
                return asset.text;
            }

            set
            {
                var writer = new StreamWriter(versionInfoPath, false);
                writer.WriteLine(value);
                writer.Close();
                AssetDatabase.ImportAsset(versionInfoPath);
            }
        }

        public static string PlaymakerVersion
        {
            get { return Instance.playmakerVersion; }
            private set { Instance.playmakerVersion = value; Save();}
        }

        public static string WelcomeScreenVersion
        {
            get { return Instance.welcomeScreenVersion; }
            set { Instance.welcomeScreenVersion = value; Save();}
        }

        public static bool ShowWelcomeScreen
        {
            get { return Instance.showWelcomeScreen; }
            set
            {
                if (value != Instance.showWelcomeScreen)
                {
                    Instance.showWelcomeScreen = value; 
                    Save();
                }
            }
        }

        public static bool ShowUpgradeGuide
        {
            get { return Instance.showUpgradeGuide; }
            set
            {
                if (value != Instance.showUpgradeGuide)
                {
                    Instance.showUpgradeGuide = value;
                    Save();
                }
            }
        }

        public static bool AutoUpdateProject
        {
            get { return Instance.lastAutoUpdateSignature != GetProjectSignature(); }
        }

        public static bool UseLegacyNetworking
        {
            get { return Instance.useLegacyNetworking; }
            set { instance.useLegacyNetworking = value; Save(); }
        }

        [Header("NOTE: Do not manually edit these parameters!")]

        // can't do this, see note above
        //[SerializeField] private string unityBuildVersion;

        [SerializeField] private string welcomeScreenVersion;
        [SerializeField] private string playmakerVersion;
        [SerializeField] private bool showWelcomeScreen = true;
        [SerializeField] private bool showUpgradeGuide;
        [SerializeField] private string lastAutoUpdateSignature;

        // Not the best place for this setting
        // since it gets reset when updating Playmaker
        [SerializeField] private bool useLegacyNetworking;

        public static void ResetForExport()
        {
            UnityBuildVersion = Application.unityVersion;
            ShowWelcomeScreen = true;
            PlaymakerVersion = string.Empty;
            WelcomeScreenVersion = string.Empty;
            UseLegacyNetworking = false;
        }

        /// <summary>
        /// Check if the Unity version is compatible with the version the PlayMaker package was built with.
        /// The Unity build version is stored in UnityBuildVersion by ResetForExport().
        /// ResetForExport is called by build scripts.
        /// </summary>
        public static bool IsUnityVersionCompatible()
        {
            if (string.IsNullOrEmpty(UnityBuildVersion)) return true; // no info

            var currentVersionInfo = Application.unityVersion.Split('.');
            if (currentVersionInfo.Length < 2) return true; // shouldn't happen

            var buildVersionInfo = UnityBuildVersion.Split('.');
            if (buildVersionInfo.Length < 2) return true; // shouldn't happen

            var currentVersionMajor = int.Parse(currentVersionInfo[0]);
            var currentVersionMinor = int.Parse(currentVersionInfo[1]);
            var buildVersionMajor = int.Parse(buildVersionInfo[0]);
            var buildVersionMinor = int.Parse(buildVersionInfo[1]);

            if (currentVersionMajor > buildVersionMajor) return true;
            if (currentVersionMajor == buildVersionMajor)
            {
                return currentVersionMinor >= buildVersionMinor;
            }
                
            // currentVersionMajor < buildVersionMajor
            // so not compatible

            return false;
        }

        public static void Save()
        {
            if (!AssetDatabase.Contains(Instance))
            {
                var copy = CreateInstance<EditorStartupPrefs>();
                EditorUtility.CopySerialized(Instance, copy);
                instance = Resources.Load<EditorStartupPrefs>("EditorStartupPrefs");
                if (instance == null)
                {
                    AssetDatabase.CreateAsset(copy, "Assets/PlayMaker/Editor/Resources/EditorStartupPrefs.asset");
                    AssetDatabase.Refresh();
                    instance = copy;

                    //Debug.Log("Missing EditorStartupPrefs asset!");
                    return;
                }
                EditorUtility.CopySerialized(copy, instance);
            }
            EditorUtility.SetDirty(Instance);
        }

        public static void ProjectUpdated(bool state)
        {
            Instance.lastAutoUpdateSignature = state ? GetProjectSignature() : string.Empty;
        }

        // Get a unique signature for this project to avoid repeatedly auto updating the same project
        // NOTE: might be a better way to do this. Currently doesn't catch project changes like imports...
        private static string GetProjectSignature()
        {
            return Application.unityVersion + "__" + Application.dataPath + "__" + GetInformationalVersion(Assembly.GetExecutingAssembly()); ;
        }

        public static string GetInformationalVersion(Assembly assembly)
        {
            if (assembly == null) return "PlayMaker_SourceCode_Version";
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }

    }
}

