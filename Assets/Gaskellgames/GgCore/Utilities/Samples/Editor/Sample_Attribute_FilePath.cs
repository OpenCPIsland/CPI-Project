#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_FilePath", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_FilePath : ScriptableObject
    {
        // ---------- FilePath ----------

        [SerializeField, FilePath]
        private string filePath;

        [field: SerializeField, FilePath]
        private string FilePath { get; set; }

    } // class end
}

#endif