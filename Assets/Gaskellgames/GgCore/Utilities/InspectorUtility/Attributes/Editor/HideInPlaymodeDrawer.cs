#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(HideInPlayModeAttribute))]
    public class HideInPlayModeDrawer : PropertyDrawer
    {
        #region Variables

        private HideInPlayModeAttribute attributeAsType;
        
        private float removeSpacing = -EditorGUIUtility.standardVerticalSpacing;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as HideInPlayModeAttribute;

            return !Application.isPlaying ? base.GetPropertyHeight(property, label) : removeSpacing;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // draw property
            if (!Application.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}

#endif