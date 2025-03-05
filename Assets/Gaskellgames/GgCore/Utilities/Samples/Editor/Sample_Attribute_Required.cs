#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Required", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Required : ScriptableObject
    {
        // ---------- RequiredField ----------

        [SerializeField, Required]
        private GameObject required;

        [field: SerializeField, Required(000, 179, 223, 255)]
        private GameObject Required { get; set; }

    } // class end
}
#endif