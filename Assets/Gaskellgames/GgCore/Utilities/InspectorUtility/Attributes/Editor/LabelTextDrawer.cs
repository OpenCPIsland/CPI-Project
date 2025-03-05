#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public class LabelTextDrawer : PropertyDrawer
    {
        #region Variables

        private LabelTextAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as LabelTextAttribute;
            
            return base.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // cache default
            float defaultWidth = EditorGUIUtility.labelWidth;
            
            // draw property
            if (ForceArray(property) == false) { label.text = attributeAsType.label; }
            EditorGUI.PropertyField(position, property, label);
            
            // reset values
            EditorGUIUtility.labelWidth = defaultWidth;

            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        // ForceArray code from Jamora & vexe : https://answers.unity.com/questions/603882/serializedproperty-isnt-being-detected-as-an-array.html
        // Updated by Gaskellgames
        bool ForceArray(SerializedProperty property)
        {
            string path = property.propertyPath;
            int indexOfDot = path.IndexOf('.');
            if (indexOfDot == -1)
            {
                return false;
            }
            else
            {
                string propName = path.Substring(0, indexOfDot);
                SerializedProperty serializedProperty = property.serializedObject.FindProperty(propName);
                return serializedProperty.isArray;
            }
        }

        #endregion
        
    } // class end
}

#endif