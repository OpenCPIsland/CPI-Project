#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(HideInEditModeAttribute))]
    public class HideInEditModeDrawer : PropertyDrawer
    {
        #region Variables

        private HideInEditModeAttribute attributeAsType;
        
        private float removeSpacing = -EditorGUIUtility.standardVerticalSpacing;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as HideInEditModeAttribute;

            return Application.isPlaying ? base.GetPropertyHeight(property, label) : removeSpacing;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            // draw property
            if (Application.isPlaying)
            {
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}

#endif