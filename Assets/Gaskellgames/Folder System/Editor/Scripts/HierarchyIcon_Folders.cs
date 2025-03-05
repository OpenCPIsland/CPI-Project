#if UNITY_EDITOR
using System.Linq;
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.FolderSystem.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames
    /// </summary>
    
    [InitializeOnLoad]
    public class HierarchyIcon_Folders
    {
        #region Static Variables

        // icons
        private static Texture2D icon_FolderActiveEmpty;
        private static Texture2D icon_FolderActiveFull;
        private static Texture2D icon_FolderDisabledEmpty;
        private static Texture2D icon_FolderDisabledFull;
        private static Texture2D icon_HierarchyHighlight;

        private const string pathRefName = "Folder System";
        private const string relativePath = "/Editor/Icons/";
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Contructors

        static HierarchyIcon_Folders()
        {
            CreateHierarchyIcon_Folder();
            
            // subscribe to inspector updates
            EditorApplication.hierarchyWindowItemOnGUI += EditorApplication_hierarchyWindowItemOnGUI;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private static void CreateHierarchyIcon_Folder()
        {
            if (!GgPathRef.TryGetFullFilePath(pathRefName, relativePath, out string filePath)) { return; }
            
            // load base icons
            Texture2D icon_FolderFull = AssetDatabase.LoadAssetAtPath(filePath + "Icon_FolderFull.png", typeof(Texture2D)) as Texture2D;
            Texture2D icon_FolderEmpty = AssetDatabase.LoadAssetAtPath(filePath + "Icon_FolderEmpty.png", typeof(Texture2D)) as Texture2D;
            
            // create custom icons
            icon_FolderActiveFull = InspectorExtensions.TintTexture(icon_FolderFull, InspectorExtensions.textNormalColor);
            icon_FolderDisabledFull = InspectorExtensions.TintTexture(icon_FolderFull, InspectorExtensions.textDisabledColor);
            icon_FolderActiveEmpty = InspectorExtensions.TintTexture(icon_FolderEmpty, InspectorExtensions.textNormalColor);
            icon_FolderDisabledEmpty = InspectorExtensions.TintTexture(icon_FolderEmpty, InspectorExtensions.textDisabledColor);
        }

        private static void CreateHierarchyIcon_Highlight()
        {
            if (!GgPathRef.TryGetFullFilePath(pathRefName, relativePath, out string filePath)) { return; }
            
            icon_HierarchyHighlight = AssetDatabase.LoadAssetAtPath(filePath + "Icon_HierarchyHighlight.png", typeof(Texture2D)) as Texture2D;
        }
        
        private static void EditorApplication_hierarchyWindowItemOnGUI(int instanceID, Rect position)
        {
            // check for valid draw
            if (Event.current.type != EventType.Repaint) { return; }
            
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject != null)
            {
                HierarchyFolders component = gameObject.GetComponent<HierarchyFolders>();
                if (component != null)
                {
                    // cache values
                    int hierarchyPixelHeight = 16;
                    bool isSelected = Selection.instanceIDs.Contains(instanceID);
                    bool isActive = component.isActiveAndEnabled;
                    bool isEmpty = (0 == component.transform.childCount);
                    Color32 defaultContentColor = GUI.contentColor;
                    Color32 textColor;
                    Color32 backgroundColor;
                    Texture2D icon_Folder;
                    
                    // check icons exist
                    if (!icon_FolderActiveFull || !icon_FolderDisabledFull || !icon_FolderActiveEmpty || !icon_FolderDisabledEmpty)
                    {
                        CreateHierarchyIcon_Folder();
                    }
                    
                    if (isActive || isSelected)
                    {
                        // text
                        if (component.customText) { textColor = component.textColor; }
                        else { textColor = InspectorExtensions.textNormalColor; }
                        
                        // icon
                        if (isEmpty)
                        {
                            if (component.customIcon) { icon_Folder = InspectorExtensions.TintTexture(icon_FolderActiveEmpty, component.iconColor); }
                            else { icon_Folder = icon_FolderActiveEmpty; }
                        }
                        else
                        {
                            if (component.customIcon) { icon_Folder = InspectorExtensions.TintTexture(icon_FolderActiveFull, component.iconColor); }
                            else { icon_Folder = icon_FolderActiveFull; }
                        }
                    }
                    else
                    {
                        // text
                        if (component.customText) { textColor = (Color)component.textColor * 0.6f; }
                        else { textColor = InspectorExtensions.textDisabledColor; }
                        
                        // icon
                        if (isEmpty)
                        {
                            if (component.customIcon) { icon_Folder = InspectorExtensions.TintTexture(icon_FolderDisabledEmpty, component.iconColor); }
                            else { icon_Folder = icon_FolderDisabledEmpty; }
                        }
                        else
                        {
                            if (component.customIcon) { icon_Folder = InspectorExtensions.TintTexture(icon_FolderDisabledFull, component.iconColor); }
                            else { icon_Folder = icon_FolderDisabledFull; }
                        }
                    }
                    
                    // draw background
                    if (isSelected)
                    {
                        backgroundColor = InspectorExtensions.backgroundActiveColor;
                    }
                    else
                    {
                        backgroundColor = InspectorExtensions.backgroundNormalColorLight;
                    }
                    Rect backgroundPosition = new Rect(position.xMin, position.yMin, position.width + hierarchyPixelHeight, position.height);
                    EditorGUI.DrawRect(backgroundPosition, backgroundColor);
                    
                    // check icon exists
                    if (!icon_HierarchyHighlight)
                    {
                        CreateHierarchyIcon_Highlight();
                    }
                    
                    // draw highlight
                    if (component.customHighlight)
                    {
                        GUI.contentColor = component.highlightColor;
                        Rect iconPosition = new Rect(position.xMin, position.yMin, icon_HierarchyHighlight.width, icon_HierarchyHighlight.height);
                        GUIContent iconGUIContent = new GUIContent(icon_HierarchyHighlight);
                        EditorGUI.LabelField(iconPosition, iconGUIContent);
                        GUI.contentColor = defaultContentColor;
                    }
                    
                    // draw icon
                    if (icon_Folder != null)
                    {
                        EditorGUIUtility.SetIconSize(new Vector2(hierarchyPixelHeight, hierarchyPixelHeight));
                        Rect iconPosition = new Rect(position.xMin, position.yMin, hierarchyPixelHeight, hierarchyPixelHeight);
                        GUIContent iconGUIContent = new GUIContent(icon_Folder);
                        EditorGUI.LabelField(iconPosition, iconGUIContent);
                    }
                    
                    // draw text
                    GUIStyle hierarchyText = new GUIStyle() { };
                    hierarchyText.normal = new GUIStyleState() { textColor = textColor };
                    hierarchyText.fontStyle = component.textStyle;
                    int offsetX;
                    if (component.textAlignment == HierarchyFolders.TextAlignment.Center)
                    {
                        hierarchyText.alignment = TextAnchor.MiddleCenter;
                        offsetX = 0;
                    }
                    else
                    {
                        hierarchyText.alignment = TextAnchor.MiddleLeft;
                        offsetX = hierarchyPixelHeight + 2;
                    }
                    Rect textOffset = new Rect(position.xMin + offsetX, position.yMin, position.width, position.height);
                    EditorGUI.LabelField(textOffset, component.name, hierarchyText);
                }
            }
        }

        #endregion
        
    } // class end
}

#endif