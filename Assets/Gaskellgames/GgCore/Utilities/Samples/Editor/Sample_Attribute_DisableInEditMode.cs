#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_DisableInEditMode", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_DisableInEditMode : ScriptableObject
    {
        // ---------- DisableInEditMode ----------

        [SerializeField, DisableInEditMode]
        private byte disableInEditMode;

        [field: SerializeField, DisableInEditMode]
        private byte DisableInEditMode { get; set; }

    } // class end
}

#endif