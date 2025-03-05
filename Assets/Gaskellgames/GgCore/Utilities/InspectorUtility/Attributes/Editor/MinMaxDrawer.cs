#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        #region Variables

        private MinMaxAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as MinMaxAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            if (property.propertyType == SerializedPropertyType.Float)
            {
                float limitedValue = property.floatValue;
                if (attributeAsType.max < property.floatValue)
                {
                    limitedValue = attributeAsType.max;
                }
                else if (property.floatValue < attributeAsType.min)
                {
                    limitedValue = attributeAsType.min;
                }

                property.floatValue = EditorGUI.FloatField(position, label, limitedValue);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                int limitedValue = property.intValue;
                if ((int)attributeAsType.max < property.intValue)
                {
                    limitedValue = (int)attributeAsType.max;
                }
                else if (property.intValue < (int)attributeAsType.min)
                {
                    limitedValue = (int)attributeAsType.min;
                }

                property.intValue = EditorGUI.IntField(position, label, limitedValue);
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