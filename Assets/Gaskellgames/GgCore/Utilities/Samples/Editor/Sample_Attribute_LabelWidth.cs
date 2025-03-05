#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_LabelWidth", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_LabelWidth : ScriptableObject
    {
        // ---------- LabelWidth ----------

        [SerializeField, LabelWidth(250)]
        private float overrideWidth;

        [field: SerializeField, LabelWidth(250)]
        private float OverrideWidth { get; set; }

    } // class end
}
#endif