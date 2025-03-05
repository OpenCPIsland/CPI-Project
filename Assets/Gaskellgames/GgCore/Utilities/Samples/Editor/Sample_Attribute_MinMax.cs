#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_MinMax", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_MinMax : ScriptableObject
    {
        // ---------- MinMax ----------

        [SerializeField, MinMax(0, 10)]
        private int minMax;

        [field: SerializeField, MinMax(0, 10)]
        private int MinMax { get; set; }

    } // class end
}

#endif