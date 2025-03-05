#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        #region Variables

        private MinMaxSliderAttribute attributeAsType;
        
        private readonly float singleLine = EditorGUIUtility.singleLineHeight;
        private readonly float labelWidth = EditorGUIUtility.labelWidth;
        private readonly int floatWidth = 50;
        private readonly int gap = 4;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as MinMaxSliderAttribute;
            
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

            // draw label, min max slider and float fields
            if (fieldInfo.FieldType == typeof(Vector2))
            {
                // get value
                Vector2 minMax = property.vector2Value;
                
                // label
                EditorGUI.PrefixLabel(position, label);
                    
                // input field: min
                float positionX = position.x + labelWidth + 2;
                Rect minRect = new Rect(positionX, position.y, floatWidth, singleLine);
                GUIContent minRectLabel = new GUIContent("", "Min Value");
                minMax.x = GgMaths.RoundFloat(EditorGUI.FloatField(minRect, minRectLabel, minMax.x), 2);
                    
                // min max slider
                positionX = minRect.xMax + gap;
                float sliderWidth = position.width - (labelWidth + 2 + ((gap + floatWidth) * 2));
                Rect sliderRect = new Rect(positionX, position.y, sliderWidth, singleLine);
                EditorGUI.MinMaxSlider(sliderRect, ref minMax.x, ref minMax.y, attributeAsType.min, attributeAsType.max);

                // input field: max
                positionX = sliderRect.xMax + gap;
                Rect maxRect = new Rect(positionX, position.y, floatWidth, singleLine);
                GUIContent maxRectLabel = new GUIContent("", "Max Value");
                minMax.y = GgMaths.RoundFloat(EditorGUI.FloatField(maxRect, maxRectLabel, minMax.y), 2);
                    
                // apply values
                property.vector2Value = minMax;
                    
                // draw sub labels
                Rect subLabelRect = new Rect(sliderRect.x, sliderRect.y + (singleLine * 0.75f), sliderRect.width, sliderRect.height);
                EditorExtensions.DrawSubLabels(subLabelRect, attributeAsType.minLabel, attributeAsType.maxLabel, attributeAsType.subLabels);
            }
            else if (fieldInfo.FieldType == typeof(Vector2Int))
            {
                // get value
                Vector2 minMax = property.vector2IntValue;
                
                // label
                EditorGUI.PrefixLabel(position, label);
                    
                // input field: min
                float positionX = position.x + labelWidth + 2;
                Rect minRect = new Rect(positionX, position.y, floatWidth, singleLine);
                GUIContent minRectLabel = new GUIContent("", "Min Value");
                minMax.x = GgMaths.RoundFloat(EditorGUI.FloatField(minRect, minRectLabel, minMax.x), 0);
                    
                // min max slider
                positionX = minRect.xMax + gap;
                float sliderWidth = position.width - (labelWidth + 2 + ((gap + floatWidth) * 2));
                Rect sliderRect = new Rect(positionX, position.y, sliderWidth, singleLine);
                EditorGUI.MinMaxSlider(sliderRect, ref minMax.x, ref minMax.y, attributeAsType.min, attributeAsType.max);

                // input field: max
                positionX = sliderRect.xMax + gap;
                Rect maxRect = new Rect(positionX, position.y, floatWidth, singleLine);
                GUIContent maxRectLabel = new GUIContent("", "Max Value");
                minMax.y = GgMaths.RoundFloat(EditorGUI.FloatField(maxRect, maxRectLabel, minMax.y), 0);
                    
                // apply values
                property.vector2IntValue = new Vector2Int((int)minMax.x, (int)minMax.y);
                    
                // draw sub labels
                Rect subLabelRect = new Rect(sliderRect.x, sliderRect.y + (singleLine * 0.75f), sliderRect.width, sliderRect.height);
                EditorExtensions.DrawSubLabels(subLabelRect, attributeAsType.minLabel, attributeAsType.maxLabel, attributeAsType.subLabels);
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