#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        #region Variables

        private ProgressBarAttribute attributeAsType;
        private int controlID = -1;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as ProgressBarAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // set label
            string barLabel = attributeAsType.label;
            if (barLabel == "")
            {
                barLabel = ObjectNames.NicifyVariableName(label.ToString());
            }
            
            // cache defaults
            bool defaultGui = GUI.enabled;
            int floatWidth = 50;
            int gap = 4;
            float width = position.width - (floatWidth + gap);
            Rect barBackground = new Rect(position.x, position.yMin, width, position.height);
            Rect floatRect = new Rect(position.xMax - floatWidth, position.yMin, floatWidth, position.height);
            Color32 barColor = new Color32(attributeAsType.R, attributeAsType.G, attributeAsType.B, attributeAsType.A);
            
            // draw bar
            if (fieldInfo.FieldType == typeof(int))
            {
                if (!attributeAsType.readOnly)
                {
                    property.intValue = (int)(attributeAsType.maxValue * CalculateSliderPercent(barBackground, property, attributeAsType));
                }
                int value = property.intValue;
                float barValue = value / attributeAsType.maxValue;
                if (barValue < 0) { barValue = 0; }
                else if (1 < barValue) { barValue = 1; }
                ProgressBar(barBackground, barValue, barLabel, barColor);
                if (attributeAsType.readOnly) { GUI.enabled = false; }
                property.intValue = (int)EditorGUI.FloatField(floatRect, GUIContent.none, value);
                GUI.enabled = defaultGui;
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                if (!attributeAsType.readOnly)
                {
                    property.floatValue = GgMaths.RoundFloat(attributeAsType.maxValue * CalculateSliderPercent(barBackground, property, attributeAsType), 3);
                }
                float value = property.floatValue;
                float barValue = value / attributeAsType.maxValue;
                if (barValue < 0) { barValue = 0; }
                else if (1 < barValue) { barValue = 1; }
                ProgressBar(barBackground, barValue, barLabel, barColor);
                if (attributeAsType.readOnly) { GUI.enabled = false; }
                property.floatValue = EditorGUI.FloatField(floatRect, GUIContent.none, value);
                GUI.enabled = defaultGui;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
            
            EditorGUI.EndProperty();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private float CalculateSliderPercent(Rect barPosition, SerializedProperty property, ProgressBarAttribute progressBar)
        {
            // update mouse style while inside barbackground
            EditorGUIUtility.AddCursorRect(barPosition, MouseCursor.ResizeHorizontal);
            Event currentEvent = Event.current;
            
            // check if mouse clicked (inside the graph)
            if (currentEvent.rawType == EventType.MouseDown && currentEvent.button == 0 && barPosition.Contains(currentEvent.mousePosition))
            {
                int hash = property.name.GetHashCode();
                controlID = GUIUtility.GetControlID(hash, FocusType.Passive, barPosition);
                GUIUtility.hotControl = controlID;
                currentEvent.Use();
            }

            // check if mouse un-clicked
            if (currentEvent.rawType == EventType.MouseUp && currentEvent.button == 0)
            {
                controlID = -1;
                GUIUtility.hotControl = 0;
                currentEvent.Use();
            }
            
            // set values
            if (GUIUtility.hotControl == controlID)
            {
                if (currentEvent.isMouse && currentEvent.button == 0 && GUI.enabled)
                {
                    // calculate x value
                    float value;
                    if (currentEvent.mousePosition.x < barPosition.xMin) { value = 0; }
                    else if (barPosition.xMax < currentEvent.mousePosition.x) { value = 1; }
                    else { value = (currentEvent.mousePosition.x - barPosition.xMin) / barPosition.width; }
					
                    currentEvent.Use();
                    return value;
                }
            }
            
            if (fieldInfo.FieldType == typeof(int))
            {
                return property.intValue / progressBar.maxValue;
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                return property.floatValue / progressBar.maxValue;
            }
            else
            {
                return 0;
            }
        }
        
        private void ProgressBar(Rect barPosition, float fillPercent, string label, Color barColor)
        {
            // create inner barPosition
            int border = 1;
            Rect barPositionInner = new Rect(barPosition.x + border, barPosition.y + border, barPosition.width - (border * 2), barPosition.height - (border * 2));
            
            // draw background
            EditorGUI.DrawRect(barPosition, new Color32(028, 028, 028, 255));
            EditorGUI.DrawRect(barPositionInner, new Color32(045, 045, 045, 255));
            
            // draw bar
            Rect fillRect = new Rect(barPositionInner.xMin, barPositionInner.yMin, barPositionInner.width * fillPercent, barPositionInner.height);
            EditorGUI.DrawRect(fillRect, barColor);
            fillRect = new Rect(barPositionInner.xMin, barPositionInner.yMin, barPositionInner.width * fillPercent, 2);
            EditorGUI.DrawRect(fillRect, barColor * 1.1f);
            fillRect = new Rect(barPositionInner.xMin, barPositionInner.yMax - 2 , barPositionInner.width * fillPercent, 2);
            EditorGUI.DrawRect(fillRect, barColor * 0.9f);

            // set label alignment
            TextAnchor defaultAlignment = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            // draw label
            bool defaultGui = GUI.enabled;
            Rect labelRect = new Rect(barPositionInner.xMin, barPositionInner.yMin, barPositionInner.width, barPositionInner.height);
            GUI.enabled = !attributeAsType.readOnly;
            GUI.Label(labelRect, label);
            GUI.enabled = defaultGui;

            // reset label alignment
            GUI.skin.label.alignment = defaultAlignment;
        }

        #endregion
        
    } // class end
}

#endif