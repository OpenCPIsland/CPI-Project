#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_FolderPath", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_FolderPath : ScriptableObject
    {
        // ---------- FolderPath ----------

        [SerializeField, FolderPath]
        private string folderPath;

        [field: SerializeField, FolderPath]
        private string FolderPath { get; set; }

    } // class end
}
#endif
