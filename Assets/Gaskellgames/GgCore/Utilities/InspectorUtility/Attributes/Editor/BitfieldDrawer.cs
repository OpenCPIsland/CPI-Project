#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(BitfieldAttribute))]
    public class BitfieldDrawer : PropertyDrawer
    {
        #region Variables

        private BitfieldAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as BitfieldAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                bool defaultGui = GUI.enabled;
                GUI.enabled = false;
                
                string bitfieldString = GgMaths.BitfieldAsString(property.intValue, attributeAsType.length);
                float labelWidth = StringExtensions.GetStringWidth(label.text);
                float contentWidth = StringExtensions.GetStringWidth(bitfieldString);
                float defaultLabelWidth = EditorGUIUtility.labelWidth;
                
                if (defaultLabelWidth + contentWidth < position.width)
                {
                    // draw with label using defaultLabelWidth
                    Rect labelRect = new Rect(position.x, position.y, defaultLabelWidth, position.height);
                    Rect valueRect = new Rect(position.x + defaultLabelWidth, position.y, position.width - defaultLabelWidth, position.height);
                    
                    EditorGUI.PrefixLabel(labelRect, label);
                    GUIContent tooltipContent = new GUIContent(bitfieldString, label.tooltip);
                    EditorGUI.LabelField(valueRect, tooltipContent);
                }
                else if (labelWidth + contentWidth < position.width)
                {
                    // draw with label added to tooltip
                    Rect labelRect = new Rect(position.x, position.y, Mathf.Max(0, position.width - contentWidth), position.height);
                    Rect valueRect = new Rect(position.x + labelRect.width, position.y, contentWidth, position.height);
                    
                    EditorGUI.PrefixLabel(labelRect, label);
                    GUIContent tooltipContent = new GUIContent(bitfieldString, label.tooltip);
                    EditorGUI.LabelField(valueRect, tooltipContent);
                }
                else
                {
                    GUIContent hiddenLabel = new GUIContent(bitfieldString, $"{label.text}\n\n{label.tooltip}");
                    EditorGUI.LabelField(position, hiddenLabel);
                }
                
                GUI.enabled = defaultGui;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            // close property
            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}
#endif