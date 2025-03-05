#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Wrap", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Wrap : ScriptableObject
    {
        // ---------- Wrap ----------

        [SerializeField, Wrap(0, 360)]
        private int wrap;

        [field: SerializeField, Wrap(0, 360)]
        private int Wrap { get; set; }

    } // class end
}

#endif