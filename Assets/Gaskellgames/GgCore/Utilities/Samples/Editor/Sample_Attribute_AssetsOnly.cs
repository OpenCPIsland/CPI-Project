#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_AssetsOnly", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_AssetsOnly : ScriptableObject
    {
        // ---------- AssetsOnly ----------

        [SerializeField, AssetsOnly]
        private GameObject prefab;

        [field: SerializeField, AssetsOnly]
        private GameObject Prefab { get; set; }

    } // class end
}

#endif