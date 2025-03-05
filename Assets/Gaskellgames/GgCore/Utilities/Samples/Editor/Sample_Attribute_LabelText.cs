#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_LabelText", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_LabelText : ScriptableObject
    {
        // ---------- LabelText ----------

        [SerializeField, LabelText("Label Text")]
        private float label;

        [field: SerializeField, LabelText("Label Text")]
        private float Label { get; set; }

    } // class end
}
#endif