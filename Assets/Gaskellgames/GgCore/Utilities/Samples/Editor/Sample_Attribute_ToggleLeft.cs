#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_ToggleLeft", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_ToggleLeft : ScriptableObject
    {
        // ---------- ToggleLeft ----------

        [SerializeField, ToggleLeft]
        private bool toggleLeft;

        [field: SerializeField, ToggleLeft]
        private bool ToggleLeft { get; set; }

    } // class end
}

#endif