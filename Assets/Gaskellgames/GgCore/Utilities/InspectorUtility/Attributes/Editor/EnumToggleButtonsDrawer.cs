#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(EnumToggleButtonsAttribute))]
    public class EnumToggleButtonsDrawer : PropertyDrawer
    {
        #region Variables

        private EnumToggleButtonsAttribute attributeAsType;

        private readonly float singleLine = EditorGUIUtility.singleLineHeight;
        private readonly float gap = EditorGUIUtility.standardVerticalSpacing;
        
        private int rows;
        private int columns;
        private int enumCount;
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as EnumToggleButtonsAttribute;

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                InitialiseVariables(property);
                return (singleLine + gap) * rows;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                if (attributeAsType.showLabel)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    float offset = labelWidth + gap;
                    
                    Rect labelPosition = new Rect(position.xMin, position.yMin, labelWidth, position.height);
                    Rect enumPosition = new Rect(position.xMin + offset, position.yMin, position.width - offset, position.height);

                    EditorGUI.PrefixLabel(labelPosition, label);
                    if (attributeAsType.enumFlags) { DrawEnumButtonFlags(enumPosition, property); }
                    else { DrawEnumButtons(enumPosition, property); }
                }
                else
                {
                    if (attributeAsType.enumFlags) { DrawEnumButtonFlags(position, property); }
                    else { DrawEnumButtons(position, property); }
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions
        
        private void InitialiseVariables(SerializedProperty property)
        {
            float viewWidth = EditorGUIUtility.currentViewWidth - singleLine;
            float buttonWidth = attributeAsType.showLabel ? viewWidth - EditorGUIUtility.labelWidth : viewWidth;
            enumCount = property.enumNames.Length;
            columns = Mathf.FloorToInt(buttonWidth / attributeAsType.minWidth);
            rows = Mathf.CeilToInt((float)enumCount / (float)columns);
        }

        private int AllSelectedValue()
        {
            int selected = 0;
            int minInclusive = attributeAsType.enumIncludesNothing ? 1 : 0;
            int maxExclusive = attributeAsType.enumIncludesEverything ? enumCount - 1 : enumCount;
            for (int i = minInclusive; i < maxExclusive; i++)
            {
                int bitShiftValue = attributeAsType.enumIncludesNothing ? i - 1 : i;
                selected += 1 << bitShiftValue;
            }
            return selected;
        }

        private void DrawEnumButtons(Rect position, SerializedProperty property)
        {
            // cache local values
            float buttonWidth = position.width / Mathf.Min(columns, enumCount);

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                { 
                    // enum index target
                    int enumIndex = column + (row * columns);
                    if (enumCount <= enumIndex) { break; }
                    
                    // set initial toggle value: Check if the button is/was pressed
                    bool buttonPressed = property.intValue == enumIndex;
                    
                    // draw button
                    float positionX = position.x + (buttonWidth * column);
                    float positionY = position.y + (row * (singleLine + gap));
                    Rect buttonPos = new Rect(positionX, positionY, buttonWidth, singleLine);
                    buttonPressed = GUI.Toggle(buttonPos, buttonPressed, property.enumDisplayNames[enumIndex], EditorStyles.miniButton);
                    
                    if (buttonPressed)
                    {
                        property.intValue = enumIndex;
                    }
                }
            }
        }
        
        private void DrawEnumButtonFlags(Rect position, SerializedProperty property)
        {
            // cache local values
            int buttonsIntValue = 0;
            bool[] buttonPressed = new bool[enumCount];
            float buttonWidth = position.width / Mathf.Min(columns, enumCount);
            
            // draw buttons
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    // enum index target
                    int enumIndex = column + (row * columns);
                    if (enumCount <= enumIndex) { break; }

                    // set bit shift value
                    int bitShiftValue = attributeAsType.enumIncludesNothing ? enumIndex - 1 : enumIndex;
                    
                    // set initial toggle value
                    if (enumIndex == 0 && attributeAsType.enumIncludesNothing)
                    {
                        // set none
                        buttonPressed[0] = property.intValue == 0;
                    }
                    else if (enumIndex == enumCount - 1 && attributeAsType.enumIncludesEverything)
                    {
                        // set all
                        buttonPressed[enumCount - 1] = property.intValue == AllSelectedValue();
                    }
                    else
                    {
                        // Check if the button is/was pressed
                        if ((property.intValue & (1 << bitShiftValue)) == 1 << bitShiftValue)
                        {
                            buttonPressed[enumIndex] = true;
                        }
                    }

                    // draw button
                    float positionX = position.x + (buttonWidth * column);
                    float positionY = position.y + (row * (singleLine + gap));
                    Rect buttonPos = new Rect(positionX, positionY, buttonWidth, singleLine);
                    buttonPressed[enumIndex] = GUI.Toggle(buttonPos, buttonPressed[enumIndex], property.enumDisplayNames[enumIndex], EditorStyles.miniButton);

                    // update buttons if pressed
                    if (!buttonPressed[enumIndex]) { continue; }
                    if (enumIndex == 0 && attributeAsType.enumIncludesNothing)
                    {
                        property.intValue = 0;
                        buttonPressed[enumCount - 1] = property.intValue == AllSelectedValue();
                    }
                    else if (enumIndex == enumCount - 1 && attributeAsType.enumIncludesEverything && property.intValue != AllSelectedValue())
                    {
                        buttonsIntValue = AllSelectedValue();
                    }
                    else
                    {
                        buttonsIntValue += 1 << bitShiftValue;
                    }
                }
            }

            // set property value
            property.intValue = buttonsIntValue;
        }

        #endregion

    } // class end
}

#endif