#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(GUIColorAttribute))]
    public class GUIColorDrawer : PropertyDrawer
    {
        #region Variables

        private GUIColorAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as GUIColorAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Color32 GUIColor = new Color32(attributeAsType.R, attributeAsType.G, attributeAsType.B, attributeAsType.A);
            
            // draw property with altered GUI color
            if (attributeAsType.target == GUIColorAttribute.Target.Background)
            {
                Color32 defaultBackgroundColour = GUI.backgroundColor;
                GUI.backgroundColor = GUIColor;
                EditorGUI.PropertyField(position, property, label);
                GUI.backgroundColor = defaultBackgroundColour;
            }
            else if (attributeAsType.target == GUIColorAttribute.Target.Content)
            {
                Color32 defaultContentColour = GUI.contentColor;
                GUI.contentColor = GUIColor;
                EditorGUI.PropertyField(position, property, label);
                GUI.contentColor = defaultContentColour;
            }
            else
            {
                Color32 defaultGUIColour = GUI.color;
                GUI.color = GUIColor;
                EditorGUI.PropertyField(position, property, label);
                GUI.color = defaultGUIColour;
            }

            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}

#endif