#if UNITY_EDITOR
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [CustomEditor(typeof(SelectionTarget)), CanEditMultipleObjects]
    public class SelectionTargetEditor : GGEditor
    {
        #region Serialized Properties / OnEnable
        
        private readonly string note = "When selecting any child GameObject in the scene view,\nthis GameObject will be selected instead.";
        private readonly float singleLine = EditorGUIUtility.singleLineHeight;

        private const string pathRefName = "GgCore";
        private const string bannerRelativePath = "/Utilities/ProjectUtility/Editor/Icons/InspectorBanner_GgCore.png";
        private Texture banner;
        
        private void OnEnable()
        {
            banner = EditorWindowUtility.LoadInspectorBanner(pathRefName, bannerRelativePath);
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            SelectionTarget selectionTarget = (SelectionTarget)target;
            serializedObject.Update();

            // draw banner if turned on in Gaskellgames settings
            EditorWindowUtility.TryDrawBanner(banner);
            
            // draw inspector
            bool defaultGui = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.TextArea(note);
            GUI.enabled = defaultGui;
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
        
    } // class end
}
#endif