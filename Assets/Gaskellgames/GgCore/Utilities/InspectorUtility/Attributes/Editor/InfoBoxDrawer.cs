#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawer : PropertyDrawer
    {
        #region Variables

        private InfoBoxAttribute attributeAsType;

        private static readonly int boxHeight = 20;
        private static readonly int boxHeightIcon = boxHeight + 5;
        private readonly float spacing = EditorGUIUtility.standardVerticalSpacing;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as InfoBoxAttribute;

            float offset = 0;
            
            if (GetConditionalResult(attributeAsType, property))
            {
                offset += GetBoxHeight(Screen.width);
            }
            
            PropertyDrawer customProperty = PropertyDrawerExtensions.Find(property);
            if (customProperty != null)
            {
                // custom property
                return customProperty.GetPropertyHeight(property, label) + offset;
            }
                
            // 'built in' property
            return EditorGUI.GetPropertyHeight(property, label) + offset;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = GetBoxHeight(position.width);
            Rect infoBoxPosition = new Rect(position.xMin, position.yMin, position.width, height);
            Rect propertyPosition = new Rect(position.xMin, position.yMin, position.width, position.height);

            if (GetConditionalResult(attributeAsType, property))
            {
                propertyPosition.yMin += height + spacing;
                switch (attributeAsType.messageType)
                {
                    case InfoMessageType.None:
                        EditorGUI.HelpBox(infoBoxPosition, attributeAsType.message, MessageType.None);
                        break;
                    case InfoMessageType.Info:
                        EditorGUI.HelpBox(infoBoxPosition, attributeAsType.message, MessageType.Info);
                        break;
                    case InfoMessageType.Warning:
                        EditorGUI.HelpBox(infoBoxPosition, attributeAsType.message, MessageType.Warning);
                        break;
                    case InfoMessageType.Error:
                        EditorGUI.HelpBox(infoBoxPosition, attributeAsType.message, MessageType.Error);
                        break;
                }
            }
            
            // draw property
            PropertyDrawer customDrawer = PropertyDrawerExtensions.Find(property);
            if (customDrawer == null)
            {
                EditorGUI.PropertyField(propertyPosition, property, label, true);
            }
            else
            {
                customDrawer.OnGUI(propertyPosition, property, label);
            }
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private float GetBoxHeight(float positionWidth)
        {
            bool iconHidden = attributeAsType.messageType == InfoMessageType.None;
            float minHeight = iconHidden ? boxHeight : boxHeightIcon;
            float wrapHeight = StringExtensions.GetWrappedStringHeight(attributeAsType.message, iconHidden ? positionWidth : positionWidth + boxHeightIcon);
            return Mathf.Max(minHeight, wrapHeight);
        }
        
        /// <summary>
        /// Calculates the logic gate result of all comparisons
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool GetConditionalResult(InfoBoxAttribute condition, SerializedProperty property)
        {
            // skip checks
            if (!attributeAsType.isConditional) { return true; }
            
            // get the logic result from the first condition
            bool[] results = new bool[condition.conditions.Length];
            
            // logic gate logic on the 'previous' condition along with the current condition
            for (var i = 0; i < condition.conditions.Length; i++)
            {
                results[i] = SerializedPropertyExtensions.Equals(property.GetField(condition.conditions[i].field), condition.conditions[i].comparison);
            }
            return GgMaths.LogicGateOutputValue(results, condition.LogicGate);
        }

        #endregion

    } // class end
}

#endif