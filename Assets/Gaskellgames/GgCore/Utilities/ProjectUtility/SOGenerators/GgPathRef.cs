#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [CreateAssetMenu(fileName = "GgPathRef_", menuName = "Gaskellgames/GgCore/GgPathRef")]
    public class GgPathRef : GGScriptableObject
    {
        #region Variables

        [SerializeField, ReadOnly]
        [Tooltip("The local file path of this PathRef.")]
        private string pathRef;
        
        private string PathRef => AssetDatabase.GetAssetPath(this).Replace($"/{this.name}.asset", "");

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region OnValidate

        private void OnValidate()
        {
            pathRef = PathRef;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Static Functions
        
        public static bool TryGetFullFilePath(string pathRefName, string relativeFilepath, out string filePath)
        {
            List<GgPathRef> allPathRefs = EditorExtensions.GetAllAssetsByType<GgPathRef>();
            foreach (GgPathRef pathRefInstance in allPathRefs)
            {
                if (pathRefInstance.name != pathRefName) { continue; }
                filePath = pathRefInstance.PathRef + relativeFilepath;
                return true;
            }

            filePath = string.Empty;
            return false;
        }

        #endregion

    } // class end
}
#endif