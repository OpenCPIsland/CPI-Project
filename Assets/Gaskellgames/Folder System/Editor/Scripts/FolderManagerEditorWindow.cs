#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.FolderSystem.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    public class FolderManagerEditorWindow : GGEditorWindow
    {
        #region Variables
        
        private const string pathRefName = "Folder System";
        private const string iconsRelativePath = "/Editor/Icons/";
        private const string textureRelativePath = "/Editor/Textures/";
        private bool doSearch => !string.IsNullOrWhiteSpace(searchBarText) && searchBarText != "";
        private string searchBarText = "";
        
        private int index_DictionarySelector = 0;
        private string[] popUp_DictionarySelector = new []{ "Auto Generated", "User Generated", "Off" };
        
        private int iconSize = 50;
        private float leftPageWidth = Screen.width * 0.75f;
        private float rightPageWidth = Screen.width * 0.25f;
        private Vector2 leftPageScrollPos;
        private Vector2 rightPageScrollPos;
        
        private static int selectedIconIndex;
        private static List<GUIContent> folderIcons = new List<GUIContent>();
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Menu Item
        
        [MenuItem(MenuItemUtility.FolderSystem_ToolsMenu_Path, false, MenuItemUtility.FolderSystem_ToolsMenu_Priority)]
        private static void OpenWindow_ToolsMenu()
        {
            OpenWindow_WindowMenu();
        }

        [MenuItem(MenuItemUtility.FolderSystem_WindowMenu_Path, false, MenuItemUtility.FolderSystem_WindowMenu_Priority)]
        public static void OpenWindow_WindowMenu()
        {
            OpenWindow<FolderManagerEditorWindow>("Folder Manager", 500, 750, false, true, Placement.GameView);
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Overrides

        protected override void OnInitialise()
        {
            if (!GgPathRef.TryGetFullFilePath(pathRefName, iconsRelativePath, out string filePath)) { return; }
            banner = (Texture)AssetDatabase.LoadAssetAtPath(filePath + "InspectorBanner_FolderSystem.png", typeof(Texture));
            GetAssetReferences();
        }

        protected override void OnFocusChange() { }

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
            Texture refreshTexture = EditorGUIUtility.IconContent("d_Refresh").image;
            string version = isWindowWide ? "Version 1.2.0" : "v1.2.0";
            List<ToolbarItem> leftToolbar = new List<ToolbarItem>
            {
                new (GetAssetReferences, new GUIContent(refreshTexture, "Refresh Asset references")),
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
            if (!ProjectFolderIcons.settings)
            {
                // error message
                string errorMessage = "No folder settings found! Please re-import package.";
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                return;
            }
            
            // page top (options)
            bool drawBanner = EditorWindowUtility.TryDrawBanner(banner, true);
            EditorGUILayout.BeginHorizontal();
            OnPageGUI_DictionarySelection();
            OnPageGUI_SearchBar();
            EditorGUILayout.EndHorizontal();
            float optionsHeight = (singleLine + spacing) * 2;
            float remainingPageHeight = (drawBanner ? pageHeight - bannerHeight : pageHeight) - optionsHeight;
            
            EditorGUILayout.BeginHorizontal();
            
            // page left (icons)
            leftPageWidth = (pageWidth * 0.7f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(leftPageWidth), GUILayout.MaxHeight(remainingPageHeight));
            OnLeftPageGUI();
            EditorGUILayout.EndVertical();
            
            // page right (names)
            rightPageWidth = (pageWidth * 0.2f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.MaxWidth(rightPageWidth), GUILayout.MaxHeight(remainingPageHeight));
            OnRightPageGUI();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region private functions
        
        private void GetAssetReferences()
        {
            // update auto-generated dictionary
            ProjectFolderIconAssetPostprocessor.AutoGenerateIconDictionary();
            
            // update user-generated dictionary
            folderIcons = new List<GUIContent>();
            if (!GgPathRef.TryGetFullFilePath(pathRefName, textureRelativePath, out string filePath)) { return; }
            List<Texture> folderTextures = EditorExtensions.GetAllAssetsByType<Texture>(new []{ filePath }, true);
            foreach (Texture texture in folderTextures)
            {
                GUIContent folderIcon = new GUIContent(texture, $"{texture.name}");
                folderIcons.AddWithoutDuplicating(folderIcon);
            }
            RebuildUserGeneratedIconDictionary();
            
            // update in-use dictionary
            ProjectFolderIcons.settings.CreateFolderIconDictionary();
        }

        private void RebuildUserGeneratedIconDictionary()
        {
            // initialise list if null
            ProjectFolderIcons.settings.TryInitialiseUserGeneratedLinks();
            
            // update list with new entries (missing texture icons)
            foreach (GUIContent guiContent in folderIcons)
            {
                ProjectFolderIconLinks newLink = new ProjectFolderIconLinks()
                {
                    name = guiContent.tooltip,
                    texture = guiContent.image,
                    additionalLinks = new List<string>() {}
                };
                ProjectFolderIcons.settings.TryAddToUserGeneratedLinks(newLink);
            }
        }
        
        private void OnPageGUI_DictionarySelection()
        {
            GUIContent label = new GUIContent("", "Select whether to use auto generated or user generated folder icon links.");
            int changeCheck = index_DictionarySelector;
            index_DictionarySelector = EditorGUILayout.Popup(label, index_DictionarySelector, popUp_DictionarySelector, GUILayout.Width(125));
            if (changeCheck != index_DictionarySelector)
            {
                switch (index_DictionarySelector)
                {
                    case 0: // auto-generated
                        ProjectFolderIcons.settings.SetShowIcons(true);
                        ProjectFolderIcons.settings.SetUseCustomDictionary(false);
                        break;
                    case 1: // user-generated
                        RebuildUserGeneratedIconDictionary();
                        ProjectFolderIcons.settings.SetShowIcons(true);
                        ProjectFolderIcons.settings.SetUseCustomDictionary(true);
                        break;
                    case 2: // off
                        ProjectFolderIcons.settings.SetShowIcons(false);
                        ProjectFolderIcons.settings.SetUseCustomDictionary(false);
                        break;
                }
                EditorUtility.SetDirty(ProjectFolderIcons.settings);
            }
        }
        
        private void OnPageGUI_SearchBar()
        {
            GUILayout.BeginHorizontal();
            searchBarText = EditorGUILayout.TextField(searchBarText, EditorStyles.toolbarSearchField);
            Texture iconTexture = EditorGUIUtility.IconContent("d_clear").image;
            if (GUILayout.Button(new GUIContent("", iconTexture, "Clear Search"), EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                searchBarText = string.Empty;
            }
            GUILayout.EndHorizontal();
        }

        private void OnLeftPageGUI()
        {
            // cache defaults
            Color32 defaultBackground = GUI.backgroundColor;
            bool defaultEnabled = GUI.enabled;
            
            List<GUIContent> iconList = doSearch ? folderIcons.Where(x => x.tooltip.ToLower().Contains(searchBarText.ToLower())).ToList() : folderIcons;
            int totalItems = iconList.Count;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Folder Icons:", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUI.enabled = false;
            string iconTotal = $"Showing {totalItems} of {folderIcons.Count} Icons";
            EditorGUILayout.LabelField(iconTotal, GUILayout.Width(StringExtensions.GetStringWidth(iconTotal)));
            GUI.enabled = defaultEnabled;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (totalItems <= 0)
            {
                // error message
                string errorMessage = "No icons found.";
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                return;
            }
            
            leftPageScrollPos = EditorGUILayout.BeginScrollView(leftPageScrollPos);
            
            // cache content variables
            int maxColumns = Mathf.FloorToInt((leftPageWidth - (scrollBarOffset + (spacing * 4))) / (iconSize + (spacing * 2)));
            int totalColumns = Mathf.Max(1, totalItems < maxColumns ? totalItems : maxColumns);
            int totalRows = (int)(totalItems / (float)totalColumns);
            totalRows += ((totalItems % totalColumns) != 0) ? 1 : 0;
            
            // iterate through items
            GUI.backgroundColor = InspectorExtensions.buttonNormalColorDark;
            int index = 0;
            for (int row = 0; row < totalRows; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int column = 0; column < totalColumns; column++)
                {
                    if (totalItems <= index) { break; }
                    GUIContent currentIcon = iconList[index];
                    if (!currentIcon.image) { continue; }
                    
                    if (GUILayout.Button(currentIcon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                    {
                        selectedIconIndex = index;
                    }
                    index++;
                }

                if (totalItems == index)
                {
                    Texture addTexture = EditorGUIUtility.IconContent("d_CreateAddNew").image;
                    if (GUILayout.Button(new GUIContent(addTexture, "Add new image"), GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                    {
                        if (!GgPathRef.TryGetFullFilePath(pathRefName, textureRelativePath, out string filePath)) { return; }
                        Object obj = EditorExtensions.GetAssetByType<Texture>(new []{ filePath });
                        EditorGUIUtility.PingObject(obj);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = defaultBackground;
            
            EditorGUILayout.EndScrollView();
        }

        private void OnRightPageGUI()
        {
            // cache defaults
            Color32 defaultBackground = GUI.backgroundColor;
            bool defaultEnabled = GUI.enabled;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Icon:", EditorStyles.boldLabel);
            GUI.backgroundColor = InspectorExtensions.buttonNormalColorDark;
            Texture iconTexture = EditorGUIUtility.IconContent("d_P4_DeletedLocal").image;
            if (GUILayout.Button(new GUIContent(iconTexture, "Close selected"), GUILayout.Width(25), GUILayout.Height(25)))
            {
                selectedIconIndex = -1;
            }
            GUI.backgroundColor = defaultBackground;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            if (0 <= selectedIconIndex && selectedIconIndex < folderIcons.Count)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = InspectorExtensions.buttonNormalColorDark;
                GUILayout.Button(folderIcons[selectedIconIndex], GUILayout.Width(iconSize), GUILayout.Height(iconSize));
                GUI.backgroundColor = defaultBackground;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                string stringValue = "No icon selected...";
                EditorGUILayout.LabelField(stringValue, GUILayout.Width(StringExtensions.GetStringWidth(stringValue)));
                GUI.enabled = defaultEnabled;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(iconSize - singleLine);
            }
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Folder Names:", EditorStyles.boldLabel);
            if (selectedIconIndex < 0 || folderIcons.Count <= selectedIconIndex) { return; }
            EditorGUILayout.Space();
            
            // show / edit links
            rightPageScrollPos = EditorGUILayout.BeginScrollView(rightPageScrollPos);
            
            GUI.enabled = false;
            EditorGUILayout.TextField(folderIcons[selectedIconIndex].tooltip);
            GUI.enabled = defaultEnabled;

            List<int> toRemove = new List<int>();
            for (int i = 0; i < ProjectFolderIcons.settings.userGeneratedLinks[selectedIconIndex].additionalLinks.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Undo.RecordObject (ProjectFolderIcons.settings, "Additional Links Updated");
                ProjectFolderIcons.settings.userGeneratedLinks[selectedIconIndex].additionalLinks[i] = EditorGUILayout.TextField(ProjectFolderIcons.settings.userGeneratedLinks[selectedIconIndex].additionalLinks[i]);
                if (GUILayout.Button(new GUIContent("-", "Remove folder name"), GUILayout.Width(20))) { toRemove.Add(i); }
                EditorGUILayout.EndHorizontal();
            }
            foreach (int entry in toRemove)
            {
                Undo.RecordObject (ProjectFolderIcons.settings, "Additional Links Updated");
                ProjectFolderIcons.settings.userGeneratedLinks[selectedIconIndex].additionalLinks.RemoveAt(entry);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Add", "Add folder name"), GUILayout.Width(35)))
            {
                Undo.RecordObject (ProjectFolderIcons.settings, "Additional Links Updated");
                ProjectFolderIcons.settings.userGeneratedLinks[selectedIconIndex].additionalLinks.Add("");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        #endregion
        
    } // class end
}
#endif