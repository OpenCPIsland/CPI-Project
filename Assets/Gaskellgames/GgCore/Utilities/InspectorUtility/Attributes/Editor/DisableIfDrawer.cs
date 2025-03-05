#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames: https://github.com/Gaskellgames
    /// Original code created by ghysc: https://github.com/ghysc/SwitchAttribute
    /// </summary>
	
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public class DisableIfDrawer : PropertyDrawer
    {
        #region Variables

        private DisableIfAttribute attributeAsType;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as DisableIfAttribute;
            
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
            EditorGUI.BeginProperty(position, label, property);

            // cache default values
            bool defaultGui = GUI.enabled;
			
            // draw property
            GUI.enabled = !GetConditionalResult(attributeAsType, property);
            PropertyDrawer customDrawer = PropertyDrawerExtensions.Find(property);
            if (customDrawer == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                customDrawer.OnGUI(position, property, label);
            }

            // reset default values
            GUI.enabled = defaultGui;
            
            // close property
            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        /// <summary>
        /// Calculates the logic gate result of all comparisons
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool GetConditionalResult(DisableIfAttribute condition, SerializedProperty property)
        {
            // get the logic result from the first condition
            bool[] results = new bool[condition.conditions.Length];
            
            // logic gate logic on the 'previous' condition along with the current condition
            for (var i = 0; i < condition.conditions.Length; i++)
            {
                results[i] = SerializedPropertyExtensions.Equals(property.GetField(condition.conditions[i].field), condition.conditions[i].comparison);
            }
            return GgMaths.LogicGateOutputValue(results, condition.LogicGate);
        }

        #endregion
		
    } // class end
}

#endif