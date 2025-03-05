#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Indent", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Indent : ScriptableObject
    {
        // ---------- Indent ----------

        [SerializeField, Indent(0)]
        private int indent0;

        [SerializeField, Indent(1)]
        private int indent1;

        [field: SerializeField, Indent(2)]
        private int Indent2 { get; set; }

    } // class end
}

#endif