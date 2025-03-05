#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(HighlightAttribute))]
    public class HighlightDrawer : PropertyDrawer
    {
        #region Variables

        private HighlightAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as HighlightAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.PropertyField(position, property, label);
            
            Color32 outlineColor = new Color32(attributeAsType.R, attributeAsType.G, attributeAsType.B, attributeAsType.A);
            Rect topBorder = new Rect(position.xMin - 1, position.yMin - 1, position.width + 2, 1);
            EditorGUI.DrawRect(topBorder, outlineColor);
            Rect bottomBorder = new Rect(position.xMin - 1, position.yMax, position.width + 2, 1);
            EditorGUI.DrawRect(bottomBorder, outlineColor);
            Rect leftBorder = new Rect(position.xMin - 1, position.yMin - 1, 1, position.height + 2);
            EditorGUI.DrawRect(leftBorder, outlineColor);
            Rect rightBorder = new Rect(position.xMax, position.yMin - 1, 1, position.height + 2);
            EditorGUI.DrawRect(rightBorder, outlineColor);
            
            EditorGUI.EndProperty();
        }

        #endregion
        
    } // class end
}

#endif