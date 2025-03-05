#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class GaskellgamesHub : GGEditorWindow
    {
        #region Variables
        
        public string[] PopUp_ShowAtStartUp = new string[] {"Always", "Never"}; // TBC: "On New Version"
        public int index_ShowAtStartUp;

        public string[] PopUp_PackageBanners = new string[] {"Always", "Never"};
        public int index_PackageBanners;

        public string[] PopUp_TransformExtension = new string[] {"Transform Utilities", "Reset Buttons", "Default Unity"};
        public int index_TransformExtension;
        
        private static readonly int windowWidth = 725;
        private static readonly int windowHeight = 525;
        private static readonly int logoSize = 75;

        // package paths - downloadable
        private const string pathRefName_GgCore = "GgCore";
        private const string relativePath_GgCore = "/Utilities/ProjectUtility/Editor/Icons/";
        
        private const string pathRefName_AudioSystem = "Audio System";
        private const string pathRefName_CameraSystem = "Camera System";
        private const string pathRefName_CharacterController = "Character Controller";
        private const string pathRefName_FolderSystem = "Folder System";
        private const string pathRefName_InputEventSystem = "Input Event System";
        private const string pathRefName_LogicSystem = "Logic System";
        private const string pathRefName_PlatformController = "Platform Controller";
        private const string pathRefName_PoolingSystem = "Pooling System";
        private const string pathRefName_SceneController = "Scene Controller";
        private const string pathRefName_SplineSystem = "Spline System";
        
        private const string relativePath = "/Editor/Icons/";
        
        // package icons - helper
        private Texture icon_PlatformController;
        private Texture icon_CameraSystem;
        private Texture icon_CharacterController;
        private Texture icon_AudioSystem;
        private Texture icon_SceneController;
        private Texture icon_FolderSystem;
        private Texture icon_SplineSystem;
        
        // package icons - helper
        private Texture icon_InputEventSystem;
        private Texture icon_PoolingSystem;
        private Texture icon_LogicSystem;
        
        // package icons - powered by
        private Texture icon_GgCore;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Menu Item
        
        [MenuItem(MenuItemUtility.Hub_ToolsMenu_Path, false, MenuItemUtility.Hub_ToolsMenu_Priority)]
        private static void OpenWindow_ToolsMenu()
        {
            OpenWindow_WindowMenu();
        }

        [MenuItem(MenuItemUtility.Hub_WindowMenu_Path, false, MenuItemUtility.Hub_WindowMenu_Priority)]
        public static void OpenWindow_WindowMenu()
        {
            OpenWindow<GaskellgamesHub>("Gaskellgames Hub", windowWidth, windowHeight, true);
        }
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Overriding Functions

        protected override void OnInitialise()
        {
            InitialiseSettings();
        }

        protected override void OnFocusChange()
        {
            InitialiseSettings();
        }

        protected override List<ToolbarItem> LeftToolbar()
        {
            string copyright = isWindowWide ? "Copyright \u00a9 2024 Gaskellgames. All rights reserved." : "\u00a9 2024 Gaskellgames.";
            List<ToolbarItem> leftToolbar = new List<ToolbarItem>
            {
                new (null, new GUIContent(copyright)),
            };

            return leftToolbar;
        }

        protected override List<ToolbarItem> RightToolbar()
        {
            string version = isWindowWide ? "Version 1.1.0" : "v1.1.0";
            List<ToolbarItem> leftToolbar = new List<ToolbarItem>
            {
                new (null, new GUIContent(version)),
            };

            return leftToolbar;
        }

        protected override GenericMenu OptionsToolbar()
        {
            GenericMenu toolsMenu = new GenericMenu();
            toolsMenu.AddItem(new GUIContent("Gaskellgames Unity Page"), false, OnSupport_AssetStoreLink);
            toolsMenu.AddItem(new GUIContent("Gaskellgames Discord"), false, OnSupport_DiscordLink);
            toolsMenu.AddItem(new GUIContent("Gaskellgames Website"), false, OnSupport_WebsiteLink);
            return toolsMenu;
        }
        
        protected override void OnPageGUI()
        {
            // draw window content
            EditorWindowUtility.TryDrawBanner(banner, true, true);
            DrawWelcomeMessage();
            EditorExtensions.DrawInspectorLine(InspectorExtensions.backgroundSeperatorColor, 4, 0);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            HandleShowAtStartUp();
            HandleShowPackageBanners();
            HandleTransformExtension();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Powered By:", EditorStyles.boldLabel, GUILayout.Width(logoSize + 10));
            DrawPackageLogo(icon_GgCore, "Gg Core");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorExtensions.DrawInspectorLine(InspectorExtensions.backgroundSeperatorColor, 4, 0);
            EditorGUILayout.LabelField("Downloaded Packages:", EditorStyles.boldLabel);
            HandleDownloadedPackages();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions
        
        private void InitialiseSettings()
        {
            banner = EditorWindowUtility.LoadInspectorBanner(pathRefName_GgCore, relativePath_GgCore + "InspectorBanner_SettingsHub.png");
            LoadPackageIcons();
            
            switch (EditorWindowUtility.settings.showPackageBanners)
            {
                case true: // always
                    index_PackageBanners = 0;
                    break;
                case false: // never
                    index_PackageBanners = 1;
                    break;
            }
            
            switch (EditorWindowUtility.settings.showHubOnStartup)
            {
                case true: // always
                    index_ShowAtStartUp = 0;
                    break;
                case false: // never
                    index_ShowAtStartUp = 1;
                    break;
            }
        }

        private void LoadPackageIcons()
        {
            string filePath = string.Empty;
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_GgCore, relativePath_GgCore, out filePath)) { return; }
            icon_GgCore = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_GgCore.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_AudioSystem, relativePath, out filePath)) { return; }
            icon_AudioSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_AudioSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_CameraSystem, relativePath, out filePath)) { return; }
            icon_CameraSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_CameraSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_CharacterController, relativePath, out filePath)) { return; }
            icon_CharacterController = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_CharacterController.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_FolderSystem, relativePath, out filePath)) { return; }
            icon_FolderSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_HierarchyFolderSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_InputEventSystem, relativePath, out filePath)) { return; }
            icon_InputEventSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_InputEventSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_LogicSystem, relativePath, out filePath)) { return; }
            icon_LogicSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_LogicSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_PlatformController, relativePath, out filePath)) { return; }
            icon_PlatformController = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_PlatformController.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_PoolingSystem, relativePath, out filePath)) { return; }
            icon_PoolingSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_PoolingSystem.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_SceneController, relativePath, out filePath)) { return; }
            icon_SceneController = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_SceneController.png", typeof(Texture));
            
            if (!GgPathRef.TryGetFullFilePath(pathRefName_SplineSystem, relativePath, out filePath)) { return; }
            icon_SplineSystem = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "Logo_SplineSystem.png", typeof(Texture));
        
        }

        private void DrawWelcomeMessage()
        {
            GUI.enabled = false;
            float defaultHeight = EditorStyles.textField.fixedHeight;
            EditorStyles.textField.fixedHeight = 100;
            EditorGUILayout.TextArea("Thank you for installing a Gaskellgames asset, and welcome to the settings hub!\n\n" +
                                     "Any settings options you choose here will be applied to all relevant Gaskellgames packages.\n\n" +
                                     "Links to the Unity Asset Store page, Gaskellgames Discord and Gaskellgames Website are available via the 'options' dropdown\n" +
                                     "menu above. (Note: Please read through each packages documentation pdf before contacting Gaskellgames with any queries.)");
            EditorStyles.textField.fixedHeight = defaultHeight;
            GUI.enabled = true;
        }

        private void HandleShowAtStartUp()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Show Hub On Startup", "Option to show/hide this window when loading into a project.");
            int changeCheck = index_ShowAtStartUp;
            index_ShowAtStartUp = EditorGUILayout.Popup(label, index_ShowAtStartUp, PopUp_ShowAtStartUp, GUILayout.Width(Screen.width * 0.4f));
            switch (index_ShowAtStartUp)
            {
                case 0: // always
                    EditorWindowUtility.settings.showHubOnStartup = true;
                    break;
                case 1: // never
                    EditorWindowUtility.settings.showHubOnStartup = false;
                    break;
                /*
                case 2: // on new version
                    break;
                */
            }
            if (changeCheck != index_ShowAtStartUp) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleShowPackageBanners()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Show Package Banners", "Show or hide the Gaskellgames package header for component scripts.");
            int changeCheck = index_PackageBanners;
            index_PackageBanners = EditorGUILayout.Popup(label, index_PackageBanners, PopUp_PackageBanners, GUILayout.Width(Screen.width * 0.4f));
            switch (index_PackageBanners)
            {
                case 0: // always
                    EditorWindowUtility.settings.showPackageBanners = true;
                    break;
                case 1: // never
                    EditorWindowUtility.settings.showPackageBanners = false;
                    break;
            }
            if (changeCheck != index_PackageBanners) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleTransformExtension()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Transform Inspector", "Enable or Disable the Gaskellgames transform utilities extension.");
            int changeCheck = index_TransformExtension;
            index_TransformExtension = EditorGUILayout.Popup(label, index_TransformExtension, PopUp_TransformExtension, GUILayout.Width(Screen.width * 0.4f));
            switch (index_TransformExtension)
            {
                case 0:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.TransformUtilities;
                    break;
                case 1:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.ResetButtons;
                    break;
                case 2:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.DefaultUnity;
                    break;
            }
            if (changeCheck != index_TransformExtension) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleDownloadedPackages()
        {
            EditorGUILayout.BeginHorizontal();
            
            float xMin = spacing;
            xMin = DrawPackageLogo(icon_AudioSystem, "Audio Controller", true, xMin);
            xMin = DrawPackageLogo(icon_CameraSystem, "Camera System", true, xMin);
            xMin = DrawPackageLogo(icon_CharacterController, "Character Controller", true, xMin);
            xMin = DrawPackageLogo(icon_FolderSystem, "Folder System", true, xMin);
            xMin = DrawPackageLogo(icon_InputEventSystem, "Input Event System", true, xMin);
            xMin = DrawPackageLogo(icon_LogicSystem, "Logic System", true, xMin);
            xMin = DrawPackageLogo(icon_PlatformController, "Platform Controller", true, xMin);
            xMin = DrawPackageLogo(icon_PoolingSystem, "Pooling System", true, xMin);
            xMin = DrawPackageLogo(icon_SceneController, "Scene Controller", true, xMin);
            xMin = DrawPackageLogo(icon_SplineSystem, "Spline System", true, xMin);
            
            EditorGUILayout.EndHorizontal();
        }

        private float DrawPackageLogo(Texture packageLogo, string toolTip, bool autoWrap = false, float xMin = 0)
        {
            if (!packageLogo) { return xMin; }
            
            // handle auto wrap
            if (autoWrap && pageWidth < (xMin + logoSize + spacing))
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                xMin = spacing;
            }
            
            // draw package logo
            GUIContent label = new GUIContent(packageLogo, toolTip);
            GUILayout.Box(label, GUILayout.Width(logoSize), GUILayout.Height(logoSize));
            
            return xMin + logoSize + spacing;
        }

        #endregion
        
    } // class end
}

#endif