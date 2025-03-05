#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    [CustomPropertyDrawer(typeof(GgEventBase), true)]
    public class GGEventDrawer : PropertyDrawer
    {
        #region variables

        private SerializedProperty instanceName;
        private SerializedProperty delay;
        private SerializedProperty verboseLogs;
        private SerializedProperty logColor;
        private SerializedProperty unityEvent;
        
        private float singleLine = EditorGUIUtility.singleLineHeight;
        private float spacing = EditorGUIUtility.standardVerticalSpacing;
        
        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region Property Height
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) - singleLine;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to instance
            EditorGUI.BeginProperty(position, label, property);
            GgEventBase ggEvent = property.GetValue<GgEventBase>();

            // get reference to SerializeFields
            instanceName = property.FindPropertyRelative("instanceName");
            delay = property.FindPropertyRelative("delay");
            verboseLogs = property.FindPropertyRelative("verboseLogs");
            logColor = property.FindPropertyRelative("logColor");
            unityEvent = property.FindPropertyRelative("unityEvent");
            
            // cache defaults
            instanceName.stringValue = label.text;
            float defaultLabel = EditorGUIUtility.labelWidth;
            
            // draw property
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.BeginHorizontal();
            GUIContent toggleLabel = new GUIContent("Debug", verboseLogs.tooltip);
            EditorGUIUtility.labelWidth = StringExtensions.GetStringWidth("Debug");
            verboseLogs.boolValue = EditorGUILayout.ToggleLeft(toggleLabel, verboseLogs.boolValue, GUILayout.Width(singleLine + EditorGUIUtility.labelWidth));
            EditorGUIUtility.labelWidth = defaultLabel;
            
            if (verboseLogs.boolValue)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Space(2);
                logColor.colorValue = EditorExtensions.ColorFieldSquare(new GUIContent("", logColor.tooltip), logColor.colorValue);
                GUILayout.Space(-2);
                EditorGUILayout.EndVertical();
                EditorGUILayout.LabelField("Color", GUILayout.Width(StringExtensions.GetStringWidth("Color")));
            }
            else
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.FlexibleSpace();
            
            EditorGUIUtility.labelWidth = StringExtensions.GetStringWidth("Delay");
            EditorGUILayout.PropertyField(delay, GUILayout.Width(50 + EditorGUIUtility.labelWidth));
            EditorGUIUtility.labelWidth = defaultLabel;
            GUIContent buttonLabel = new GUIContent("Invoke", $"Invoke event after {delay.floatValue} seconds using default args.\n\n[Note: the inspector invoke button will pass default argument values to listeners.]");
            float buttonWidth = StringExtensions.GetStringWidth("Invoke") + singleLine;
            if (GUILayout.Button(buttonLabel, GUILayout.Width(buttonWidth), GUILayout.Height(singleLine)))
            {
                if (Application.isPlaying)
                {
                    ggEvent.InvokeEventWithDefaultArgs();
                }
                else
                {
                    VerboseLogs.Log($"Cannot call [{instanceName.stringValue}] event for [{property.serializedObject.targetObject.name}] while Application is not playing.", LogType.Warning);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(singleLine);
            EditorGUILayout.EndVertical();
            GUILayout.Space(-singleLine);
            
            EditorGUILayout.PropertyField(unityEvent, new GUIContent(instanceName.stringValue));

            // close property
            EditorGUI.EndProperty();
        }

        #endregion

    } // class end
}
#endif
