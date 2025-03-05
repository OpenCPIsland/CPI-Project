#if UNITY_EDITOR
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomEditor(typeof(Comment)), CanEditMultipleObjects]
    public class CommentEditor : GGEditor
    {
        #region Serialized Properties / OnEnable
        
        SerializedProperty lines;
        SerializedProperty comment;
        
        private Texture iconTexture;
        private GUIContent label;
        private Rect[] repaintPositions;
        private float singleLine = EditorGUIUtility.singleLineHeight;
        
        private const string pathRefName = "GgCore";
        private const string bannerRelativePath = "/Utilities/ProjectUtility/Editor/Icons/InspectorBanner_GgCore.png";
        private Texture banner;
        
        private void OnEnable()
        {
            lines = serializedObject.FindProperty("lines");
            comment = serializedObject.FindProperty("comment");
            
            banner = EditorWindowUtility.LoadInspectorBanner(pathRefName, bannerRelativePath);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            Comment commentTarget = (Comment)target;
            serializedObject.Update();

            // draw banner if turned on in Gaskellgames settings
            EditorWindowUtility.TryDrawBanner(banner);
            
            // cache values
            repaintPositions = new Rect[2];
            bool defaultGui = GUI.enabled;
            bool defaultWrap = EditorStyles.textField.wordWrap;
            
            // draw inspector
            if (1 < targets.Length)
            {
                EditorGUILayout.Space(-EditorGUIUtility.singleLineHeight);
                EditorGUILayout.PropertyField(comment, new GUIContent("", comment.tooltip));
                GUI.enabled = false;
            }
            else
            {
                EditorStyles.textField.wordWrap = true;
                comment.stringValue = EditorGUILayout.TextArea(comment.stringValue, GUILayout.Height(singleLine * lines.intValue));
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            iconTexture = EditorGUIUtility.IconContent("d_Toolbar Plus").image;
            label = new GUIContent(iconTexture, "Increase text area size");
            if (GUILayout.Button(label, InspectorExtensions.Style_Button(), GUILayout.Width(25), GUILayout.Height(20)))
            {
                lines.intValue++;
            }
            repaintPositions[0] = GUILayoutUtility.GetLastRect();
            iconTexture = EditorGUIUtility.IconContent("d_Toolbar Minus").image;
            label = new GUIContent(iconTexture, "Reduce text area size");
            if (GUILayout.Button(label, InspectorExtensions.Style_Button(), GUILayout.Width(25), GUILayout.Height(20)))
            {
                lines.intValue--;
            }
            repaintPositions[1] = GUILayoutUtility.GetLastRect();
            EditorGUILayout.EndHorizontal();
            
            // force update window (to have snappy hover on buttons) if mouse over buttons
            foreach (var repaintPosition in repaintPositions)
            {
                if (repaintPosition.Contains(Event.current.mousePosition)) { Repaint(); }
            }
            
            // reset values
            GUI.enabled = defaultGui;
            EditorStyles.textField.wordWrap = defaultWrap;

            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
        
    } // class end
}
#endif