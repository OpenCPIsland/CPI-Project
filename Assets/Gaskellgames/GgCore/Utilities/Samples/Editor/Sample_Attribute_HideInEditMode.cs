#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_HideInEditMode", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_HideInEditMode : ScriptableObject
    {
        // ---------- HideInEditMode ----------

        [SerializeField, HideInEditMode]
        private byte hideInEditMode;

        [field: SerializeField, HideInEditMode]
        private byte HideInEditMode { get; set; }

    } // class end
}

#endif