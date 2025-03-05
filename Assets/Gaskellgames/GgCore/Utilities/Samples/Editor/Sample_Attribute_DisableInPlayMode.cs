#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_DisableInPlayMode", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_DisableInPlayMode : ScriptableObject
    {
        // ---------- DisableInPlayMode ----------

        [SerializeField, DisableInPlayMode]
        private byte disableInPlayMode;

        [field: SerializeField, DisableInPlayMode]
        private byte DisableInPlayMode { get; set; }

    } // class end
}

#endif