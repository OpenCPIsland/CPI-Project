#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    // <summary>
    // Code created by Gaskellgames: https://github.com/Gaskellgames?tab=repositories
    // </summary>

    [CustomPropertyDrawer(typeof(ShowAsStringAttribute))]
    public class ShowAsStringDrawer : PropertyDrawer
    {
        #region Variables

        private ShowAsStringAttribute attributeAsType;
        
        private float singleLine = EditorGUIUtility.singleLineHeight;
        private float labelWidth = EditorGUIUtility.labelWidth;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as ShowAsStringAttribute;
            
            object propertyValue = GetSerializedPropertyValue(property);

            return propertyValue != null && attributeAsType != null
                ? EditorGUI.GetPropertyHeight(property, label) - (singleLine * 1.1f)
                : EditorGUI.GetPropertyHeight(property, label);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            object propertyValue = GetSerializedPropertyValue(property);
            
            // cache default values
            bool guiDefault = GUI.enabled;
            
            if (propertyValue != null && attributeAsType != null)
            {
                GUI.enabled = !attributeAsType.readOnly;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(label);
                GUILayout.Space(2);
                EditorGUILayout.LabelField(propertyValue.ToString());
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
            
            // reset default values
            GUI.enabled = guiDefault;
            
            // close property
            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        /// <summary>
        /// Get the value for a SerializedProperty
        /// </summary>
        /// <param name="property"></param>
        /// <returns> Null if not able to get value for SerializedProperty's type </returns>
        private object GetSerializedPropertyValue(SerializedProperty property)
        {
            if (property == null) { return null; }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return null; // cannot handle Generic
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return (Color32)property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return (LayerMask)property.intValue;
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.ArraySize:
                    return property.arraySize;
                case SerializedPropertyType.Character:
                    return (char)property.intValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Gradient:
                    return null; // cannot handle Gradient
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return property.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return property.fixedBufferSize;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
                case SerializedPropertyType.ManagedReference:
                    return property.managedReferenceValue;
                case SerializedPropertyType.Hash128:
                    return property.hash128Value;
                default:
                    return null; // other?
            }
        }

        #endregion
        
    } // class end
}
#endif