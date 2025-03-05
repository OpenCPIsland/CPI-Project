#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Max", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Max : ScriptableObject
    {
        // ---------- Max ----------

        [SerializeField, Max(10)]
        private int max;

        [field: SerializeField, Max(10)]
        private int Max { get; set; }

    } // class end
}

#endif