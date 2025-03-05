#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [CustomEditor(typeof(GgPathRef)), CanEditMultipleObjects]
    public class GgPathRefEditor : GGEditor
    {
        #region Serialized Properties / Variables

        private GgPathRef targetAsType;
        
        private SerializedProperty pathRef;
        
        private const string pathRefName = "GgCore";
        private const string bannerRelativePath = "/Utilities/ProjectUtility/Editor/Icons/InspectorBanner_GgCore.png";
        private Texture banner;
        
        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region OnEnable
        
        private void OnEnable()
        {
            if (!targetAsType) { targetAsType = target as GgPathRef; }
            banner = EditorWindowUtility.LoadInspectorBanner(pathRefName, bannerRelativePath);
            
            pathRef = serializedObject.FindProperty("pathRef");
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            serializedObject.Update();
            
            // draw banner if turned on in Gaskellgames settings
            EditorWindowUtility.TryDrawBanner(banner);
            
            // draw custom inspector:
            EditorGUILayout.PropertyField(pathRef);
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    } // class end
}

#endif