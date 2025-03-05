#if UNITY_EDITOR
using Gaskellgames.EditorOnly;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CustomEditor(typeof(LockToLayer)), CanEditMultipleObjects]
    public class LockToLayerEditor : GGEditor
    {
        #region Serialized Properties / OnEnable
        
        SerializedProperty layerLock;
        
        private const string pathRefName = "GgCore";
        private const string bannerRelativePath = "/Utilities/ProjectUtility/Editor/Icons/InspectorBanner_GgCore.png";
        private Texture banner;
        
        private void OnEnable()
        {
            layerLock = serializedObject.FindProperty("layerLock");
            banner = EditorWindowUtility.LoadInspectorBanner(pathRefName, bannerRelativePath);
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnInspectorGUI

        public override void OnInspectorGUI()
        {
            // get & update references
            LockToLayer lockToLayer = (LockToLayer)target;
            serializedObject.Update();

            // draw banner if turned on in Gaskellgames settings
            EditorWindowUtility.TryDrawBanner(banner);
            
            // draw inspector
            EditorGUILayout.PropertyField(layerLock);
            
            // apply reference changes
            serializedObject.ApplyModifiedProperties();
        }

        #endregion
        
    } // class end
}
#endif