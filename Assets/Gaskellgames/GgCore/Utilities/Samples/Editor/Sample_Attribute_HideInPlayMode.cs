#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_HideInPlayMode", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_HideInPlayMode : ScriptableObject
    {
        // ---------- HideInPlayMode ----------

        [SerializeField, HideInPlayMode]
        private byte hideInPlayMode;

        [field: SerializeField, HideInPlayMode]
        private byte HideInPlayMode;

    } // class end
}

#endif