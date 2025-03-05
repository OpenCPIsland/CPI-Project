#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_StringDropdown", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_StringDropdown : ScriptableObject
    {
        // ---------- StringDropdown ----------

        [SerializeField, StringDropdown("Option1", "Option2", "Option3")]
        private string stringDropdown;

        [field: SerializeField, StringDropdown("Option1", "Option2", "Option3")]
        private string StringDropdown { get; set; }

    } // class end
}

#endif