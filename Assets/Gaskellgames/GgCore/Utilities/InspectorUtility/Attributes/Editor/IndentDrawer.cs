#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(IndentAttribute))]
    public class IndentDrawer : PropertyDrawer
    {
        #region Variables

        private IndentAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as IndentAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            
            if (attributeAsType.specificIndentLevel)
            {
                int defaultIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = attributeAsType.indentLevel;
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.indentLevel = defaultIndent;
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.indentLevel--;
            }

            // close property
            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}
#endif