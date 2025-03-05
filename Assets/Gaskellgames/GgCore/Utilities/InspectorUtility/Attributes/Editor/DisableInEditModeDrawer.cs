#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(DisableInEditModeAttribute))]
    public class DisableInEditModeDrawer : PropertyDrawer
    {
        #region Variables

        private DisableInEditModeAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as DisableInEditModeAttribute;
            
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool defaultGUI = GUI.enabled;
            GUI.enabled = Application.isPlaying;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = defaultGUI;

            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}

#endif