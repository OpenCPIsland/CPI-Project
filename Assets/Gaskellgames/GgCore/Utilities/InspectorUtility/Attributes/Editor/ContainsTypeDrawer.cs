#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gaskellgames.EditorOnly
{
    // <summary>
    // Code created by Gaskellgames: https://github.com/Gaskellgames?tab=repositories
    // </summary>
    
    [CustomPropertyDrawer(typeof(ContainsTypeAttribute))]
    public class ContainsTypeDrawer : PropertyDrawer
    {
        #region Variables

        private ContainsTypeAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as ContainsTypeAttribute;
            
            PropertyDrawer customProperty = PropertyDrawerExtensions.Find(property);
            if (customProperty != null)
            {
                // custom property
                return customProperty.GetPropertyHeight(property, label);
            }
                
            // 'built in' property
            return EditorGUI.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                Object objectReference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, fieldInfo.FieldType, true);
                GameObject gameObjectReference = objectReference as GameObject;
                
                if (ContainsComponentOfType(attributeAsType.type, gameObjectReference))
                {
                    property.objectReferenceValue = objectReference;
                }
                else if (gameObjectReference)
                {
                    VerboseLogs.Log($"Reference object [{gameObjectReference.name}] does not contain component of type [{attributeAsType.type}]", LogType.Warning);
                }
                else
                {
                    property.objectReferenceValue = null;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }

            // close property
            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private bool ContainsComponentOfType(Type type, GameObject gameObject)
        {
            if (type == null || gameObject == null) { return false; }
            
            var component = gameObject.GetComponent(type);
            return component != null;
        }

        #endregion

    } // class end
}
#endif