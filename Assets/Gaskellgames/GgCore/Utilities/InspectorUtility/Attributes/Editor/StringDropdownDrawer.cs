#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(StringDropdownAttribute))]
    public class StringDropdownDrawer : PropertyDrawer
    {
        #region Variables

        private StringDropdownAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as StringDropdownAttribute;
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            string[] list = attributeAsType.list;
            
            if (property.propertyType == SerializedPropertyType.String)
            {
                // get selected index
                int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
                index = EditorGUI.Popup(position, property.displayName, index, list);

                // convert index to string value
                property.stringValue = list[index];
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
            
            EditorGUI.EndProperty();
        }

        #endregion
        
    } // class end
}

#endif