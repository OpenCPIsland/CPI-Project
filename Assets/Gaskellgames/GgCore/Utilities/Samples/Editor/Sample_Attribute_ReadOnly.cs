#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_ReadOnly", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_ReadOnly : ScriptableObject
    {
        // ---------- ReadOnly ----------

        [SerializeField, ReadOnly]
        private float readOnly;

        [field: SerializeField, ReadOnly]
        private GameObject ReadOnly { get; set; }

    } // class end
}

#endif