#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(RangeAttribute))]
    public class RangeDrawer : PropertyDrawer
    {
        #region Variables

        private RangeAttribute attributeAsType;
        
        private readonly float singleLine = EditorGUIUtility.singleLineHeight;
        private readonly float labelWidth = EditorGUIUtility.labelWidth;
        private readonly int floatWidth = 50;
        private readonly int gap = 4;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as RangeAttribute;
            
            if (attributeAsType.subLabels)
            {
                return EditorGUI.GetPropertyHeight(property, label) + (singleLine * 0.4f);
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // draw label, slider and float field
            // draw slider
            if (fieldInfo.FieldType == typeof(int))
            {
                Rect sliderRect = new Rect(position.x, position.y, position.width, singleLine);
                property.intValue = (int)EditorGUI.Slider(sliderRect, label, property.intValue, attributeAsType.min, attributeAsType.max);
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                Rect sliderRect = new Rect(position.x, position.y, position.width, singleLine);
                property.floatValue = EditorGUI.Slider(sliderRect, label, property.floatValue, attributeAsType.min, attributeAsType.max);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            // draw sub labels
            Rect subLabelRect = new Rect(position.x + labelWidth + 2, position.y + (singleLine * 0.75f), position.width - (labelWidth + 2 + floatWidth + gap), singleLine);
            EditorExtensions.DrawSubLabels(subLabelRect, attributeAsType.minLabel, attributeAsType.maxLabel, attributeAsType.subLabels);

            EditorGUI.EndProperty();
        }

        #endregion
        
    } // class end
}
#endif