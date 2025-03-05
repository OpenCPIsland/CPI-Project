#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Min", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Min : ScriptableObject
    {
        // ---------- Min ----------

        [SerializeField, Min(0)]
        private int min;

        [field: SerializeField, Min(0)]
        private int Min { get; set; }

    } // class end
}

#endif