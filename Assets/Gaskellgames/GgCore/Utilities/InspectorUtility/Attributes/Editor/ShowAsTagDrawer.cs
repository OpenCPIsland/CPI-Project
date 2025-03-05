#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    // <summary>
    // Code created by Gaskellgames: https://github.com/Gaskellgames?tab=repositories
    // </summary>

    [CustomPropertyDrawer(typeof(ShowAsTagAttribute))]
    public class ShowAsTagDrawer : PropertyDrawer
    {
        #region Variables

        private ShowAsTagAttribute attributeAsType;
        
        private float singleLine = EditorGUIUtility.singleLineHeight;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as ShowAsTagAttribute;

            return attributeAsType != null
                ? EditorGUI.GetPropertyHeight(property, label) - (singleLine * 1.1f)
                : EditorGUI.GetPropertyHeight(property, label);
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
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                GUILayout.Space(2);
                EditorExtensions.DrawAsTag(property.stringValue);
                EditorGUILayout.EndHorizontal();
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