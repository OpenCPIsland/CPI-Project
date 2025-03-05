#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(LabelWidthAttribute))]
    public class LabelWidthDrawer : PropertyDrawer
    {
        #region Variables

        private LabelWidthAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as LabelWidthAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // cache default
            float defaultWidth = EditorGUIUtility.labelWidth;
            
            // draw property
            EditorGUIUtility.labelWidth = attributeAsType.labelWidth;
            EditorGUI.PropertyField(position, property, label);
            
            // reset values
            EditorGUIUtility.labelWidth = defaultWidth;

            EditorGUI.EndProperty();
        }

        #endregion
        
    } // class end
}

#endif