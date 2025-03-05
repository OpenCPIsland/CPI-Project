#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        #region Variables

        private FolderPathAttribute attributeAsType;
        
        private string selectedPath;
        private Texture iconTexture;
        private GUIStyle iconButtonStyle = new GUIStyle();

        private int warningHeight = 25;
        private int gap = 2;
        private float singleLine = EditorGUIUtility.singleLineHeight;
        private float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Property Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as FolderPathAttribute;
            CreateButton();
            
            if (FileExtensions.IsFolderPathValid(selectedPath))
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label) + warningHeight + verticalSpacing;
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                float iconWidth = singleLine;
                Rect fieldPosition = new Rect(position.xMin, position.yMin, position.width - (iconWidth + gap), position.height);
                Rect iconPosition = new Rect(fieldPosition.xMax + gap, position.yMin, iconWidth, position.height);

                if (!FileExtensions.IsFolderPathValid(selectedPath))
                {
                    Rect warningPosition = new Rect(position.xMin, position.yMin, position.width, warningHeight);
                    EditorGUI.HelpBox(warningPosition, "Folder path does not exist", MessageType.Warning);

                    fieldPosition.yMin += warningHeight + verticalSpacing;
                    iconPosition.yMin += warningHeight + verticalSpacing;
                }
                
                if (GUI.Button(iconPosition, iconTexture, iconButtonStyle))
                {
                    selectedPath = EditorUtility.OpenFolderPanel("Select a folder", "Assets", "");
                    if (selectedPath.Contains(Application.dataPath))
                    {
                        selectedPath = selectedPath.Substring(Application.dataPath.Length + 1);
                        property.stringValue = selectedPath;
                    }
                    else
                    {
                        Debug.LogError("The folder path must be in the Assets folder");
                    }
                    GUIUtility.ExitGUI();
                }
                
                // set value
                property.stringValue = EditorGUI.TextField(fieldPosition, label, selectedPath);
                selectedPath = property.stringValue;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            // close property
            EditorGUI.EndProperty();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private void CreateButton()
        {
            iconTexture = EditorGUIUtility.IconContent("d_FolderOpened Icon").image;
            
            iconButtonStyle.fontSize = 9;
            iconButtonStyle.alignment = TextAnchor.MiddleCenter;
            iconButtonStyle.normal.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.hover.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.active.textColor = InspectorExtensions.textNormalColor;
            iconButtonStyle.normal.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.blankColor, InspectorExtensions.blankColor);
            iconButtonStyle.hover.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonHoverColor, InspectorExtensions.blankColor);
            iconButtonStyle.active.background = InspectorExtensions.CreateTexture(20, 20, 1, true, InspectorExtensions.buttonActiveColor, InspectorExtensions.buttonActiveBorderColor);
        }

        #endregion

    } // class end
}
#endif