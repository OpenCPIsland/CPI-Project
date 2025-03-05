#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomPropertyDrawer(typeof(HiddenAttribute))]
    public class HiddenDrawer : PropertyDrawer
    {
        private float removeSpacing = -EditorGUIUtility.standardVerticalSpacing;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return removeSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // blank
        }

    } // class end
}

#endif