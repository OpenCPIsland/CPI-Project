#if UNITY_EDITOR
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    // <summary>
    // Code created by Gaskellgames
    // </summary>
    
    [CustomPropertyDrawer(typeof(UnitAttribute))]
    internal class UnitDrawer : PropertyDrawer
    {
        #region Variables

        private UnitAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as UnitAttribute;
            
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
            EditorGUI.BeginProperty(position, label, property);
            
            // draw property
            PropertyDrawer customDrawer = PropertyDrawerExtensions.Find(property);
            if (customDrawer == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                customDrawer.OnGUI(position, property, label);
            }
            
            // draw units
            DrawUnitOverlay(position, GetUnitLabel(attributeAsType.unit), property.propertyType, property.isExpanded);

            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private GUIContent GetUnitLabel(GgMaths.Units units)
        {
            GUIContent label = new GUIContent("", units.ToString());
            
            switch (units)
            {
                case GgMaths.Units.None:
                    label.text = "";
                    break;
                case GgMaths.Units.Milliseconds:
                    label.text = "ms";
                    break;
                case GgMaths.Units.Seconds:
                    label.text = "s";
                    break;
                case GgMaths.Units.Minutes:
                    label.text = "m";
                    break;
                case GgMaths.Units.Hours:
                    label.text = "h";
                    break;
                case GgMaths.Units.Days:
                    label.text = "d";
                    break;
                case GgMaths.Units.Bytes:
                    label.text = "B";
                    break;
                case GgMaths.Units.Kilobytes:
                    label.text = "kB";
                    break;
                case GgMaths.Units.Megabytes:
                    label.text = "MB";
                    break;
                case GgMaths.Units.Nutons:
                    label.text = "N";
                    break;
                case GgMaths.Units.Units:
                    label.text = "u";
                    break;
                case GgMaths.Units.Multiplier:
                    label.text = "x";
                    break;
                case GgMaths.Units.Percentage:
                    label.text = "%";
                    break;
                case GgMaths.Units.Degrees:
                    label.text = "\u00B0";
                    break;
                case GgMaths.Units.Radians:
                    label.text = "rad";
                    break;
                case GgMaths.Units.Frames:
                    label.text = "frames";
                    break;
                case GgMaths.Units.PerSecond:
                    label.text = "/s";
                    break;
                case GgMaths.Units.UnitsPerSecond:
                    label.text = "u/s";
                    break;
                case GgMaths.Units.DegreesPerSecond:
                    label.text = "\u00B0/s";
                    break;
                case GgMaths.Units.RadiansPerSecond:
                    label.text = "rad/s";
                    break;
                case GgMaths.Units.FramesPerSecond:
                    label.text = "fps";
                    break;
                
                default:
                    return null;
            }

            return label;
        }

        private void DrawUnitOverlay(Rect position, GUIContent label, SerializedPropertyType propertyType, bool isExpanded)
        {
            Rect pos = position;
            if (propertyType == SerializedPropertyType.Vector2 || propertyType == SerializedPropertyType.Vector3)
            {
                // vector properties get broken down into two lines when there's not enough space
                if (EditorGUIUtility.wideMode)
                {
                    pos.xMin += EditorGUIUtility.labelWidth;
                    pos.width /= 3;
                }
                else
                {
                    pos.xMin += 12;
                    pos.yMin = pos.yMax - EditorGUIUtility.singleLineHeight;
                    pos.width /= (propertyType == SerializedPropertyType.Vector2) ? 2 : 3;
                }

                pos.height = EditorGUIUtility.singleLineHeight;
                DrawUnits(pos, label);
                pos.x += pos.width;
                DrawUnits(pos, label);
                if (propertyType == SerializedPropertyType.Vector3)
                {
                    pos.x += pos.width;
                    DrawUnits(pos, label);
                }
            }
            else if (propertyType == SerializedPropertyType.Vector4)
            {
                if (isExpanded)
                {
                    pos.yMin = pos.yMax - 4 * EditorGUIUtility.singleLineHeight -
                               3 * EditorGUIUtility.standardVerticalSpacing;
                    pos.height = EditorGUIUtility.singleLineHeight;
                    for (int i = 0; i < 4; ++i)
                    {
                        DrawUnits(pos, label);
                        pos.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            else
            {
                pos.height = EditorGUIUtility.singleLineHeight;
                DrawUnits(pos, label);
            }
        }

        private void DrawUnits(Rect position, GUIContent label)
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight,
                contentOffset = new Vector2(-2, 0),
                normal = { textColor = new Color32(000, 179, 223, 255) },
                hover = { textColor = new Color32(223, 179, 000, 255) },
            };
            
            GUI.Label(position, label, style);
        }

        #endregion
        
    } // class end
}
#endif