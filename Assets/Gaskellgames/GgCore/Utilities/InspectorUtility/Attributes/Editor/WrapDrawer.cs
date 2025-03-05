#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(WrapAttribute))]
    public class WrapDrawer : PropertyDrawer
    {
        #region Variables

        private WrapAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as WrapAttribute;
            
            return EditorGUI.GetPropertyHeight(property, label, true);
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
                    limitedValue = attributeAsType.min;
                }
                else if (property.floatValue < attributeAsType.min)
                {
                    limitedValue = attributeAsType.max;
                }

                property.floatValue = EditorGUI.FloatField(position, label, limitedValue);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                int limitedValue = property.intValue;
                if ((int)attributeAsType.max < property.intValue)
                {
                    limitedValue = (int)attributeAsType.min;
                }
                else if (property.intValue < (int)attributeAsType.min)
                {
                    limitedValue = (int)attributeAsType.max;
                }

                property.intValue = EditorGUI.IntField(position, label, limitedValue);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2)
            {
                float limitedValueX = property.vector2Value.x;
                if (attributeAsType.max < property.vector2Value.x)
                {
                    limitedValueX = attributeAsType.min;
                }
                else if (property.vector2Value.x < attributeAsType.min)
                {
                    limitedValueX = attributeAsType.max;
                }
                
                float limitedValueY = property.vector2Value.y;
                if (attributeAsType.max < property.vector2Value.y)
                {
                    limitedValueY = attributeAsType.min;
                }
                else if (property.vector2Value.y < attributeAsType.min)
                {
                    limitedValueY = attributeAsType.max;
                }

                property.vector2Value = EditorGUI.Vector2Field(position, label, new Vector2(limitedValueX, limitedValueY));
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                int limitedValueX = property.vector2IntValue.x;
                if ((int)attributeAsType.max < property.vector2IntValue.x)
                {
                    limitedValueX = (int)attributeAsType.min;
                }
                else if (property.vector2IntValue.x < (int)attributeAsType.min)
                {
                    limitedValueX = (int)attributeAsType.max;
                }
                
                int limitedValueY = property.vector2IntValue.y;
                if ((int)attributeAsType.max < property.vector2IntValue.y)
                {
                    limitedValueY = (int)attributeAsType.min;
                }
                else if (property.vector2IntValue.y < (int)attributeAsType.min)
                {
                    limitedValueY = (int)attributeAsType.max;
                }

                property.vector2IntValue = EditorGUI.Vector2IntField(position, label, new Vector2Int(limitedValueX, limitedValueY));
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                float limitedValueX = property.vector3Value.x;
                if (attributeAsType.max < property.vector3Value.x)
                {
                    limitedValueX = attributeAsType.min;
                }
                else if (property.vector3Value.x < attributeAsType.min)
                {
                    limitedValueX = attributeAsType.max;
                }
                
                float limitedValueY = property.vector3Value.y;
                if (attributeAsType.max < property.vector3Value.y)
                {
                    limitedValueY = attributeAsType.min;
                }
                else if (property.vector3Value.y < attributeAsType.min)
                {
                    limitedValueY = attributeAsType.max;
                }
                
                float limitedValueZ = property.vector3Value.z;
                if (attributeAsType.max < property.vector3Value.z)
                {
                    limitedValueZ = attributeAsType.min;
                }
                else if (property.vector3Value.z < attributeAsType.min)
                {
                    limitedValueZ = attributeAsType.max;
                }

                property.vector3Value = EditorGUI.Vector3Field(position, label, new Vector3(limitedValueX, limitedValueY, limitedValueZ));
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                int limitedValueX = property.vector3IntValue.x;
                if ((int)attributeAsType.max < property.vector3IntValue.x)
                {
                    limitedValueX = (int)attributeAsType.min;
                }
                else if (property.vector3IntValue.x < (int)attributeAsType.min)
                {
                    limitedValueX = (int)attributeAsType.max;
                }
                
                int limitedValueY = property.vector3IntValue.y;
                if ((int)attributeAsType.max < property.vector3IntValue.y)
                {
                    limitedValueY = (int)attributeAsType.min;
                }
                else if (property.vector3IntValue.y < (int)attributeAsType.min)
                {
                    limitedValueY = (int)attributeAsType.max;
                }
                
                int limitedValueZ = property.vector3IntValue.z;
                if (attributeAsType.max < property.vector3IntValue.z)
                {
                    limitedValueZ = (int)attributeAsType.min;
                }
                else if (property.vector3IntValue.z < attributeAsType.min)
                {
                    limitedValueZ = (int)attributeAsType.max;
                }

                property.vector3IntValue = EditorGUI.Vector3IntField(position, label, new Vector3Int(limitedValueX, limitedValueY, limitedValueZ));
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