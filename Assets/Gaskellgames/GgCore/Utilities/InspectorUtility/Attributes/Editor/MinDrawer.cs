#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(MinAttribute))]
    public class MinDrawer : PropertyDrawer
    {
        #region Variables

        private MinAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as MinAttribute;
            
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
                float limitedValue = Mathf.Max(property.floatValue, attributeAsType.min);
                property.floatValue = EditorGUI.FloatField(position, label, limitedValue);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                int limitedValue = Mathf.Max(property.intValue, (int)attributeAsType.min);
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