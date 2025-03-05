#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [CustomPropertyDrawer(typeof(CustomCurveAttribute))]
    public class CustomCurveDrawer : PropertyDrawer
    {
        #region Variables

        private CustomCurveAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as CustomCurveAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Color32 lineColor = new Color32(attributeAsType.R, attributeAsType.G, attributeAsType.B, attributeAsType.A);

            if (property.propertyType == SerializedPropertyType.AnimationCurve)
            {
                // draw curve
                EditorGUI.CurveField(position, property, lineColor, default, new GUIContent(label));
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