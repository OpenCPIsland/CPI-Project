#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Unit", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Unit : ScriptableObject
    {
        // ---------- Unit ----------

        [SerializeField, Unit(GgMaths.Units.Seconds)]
        private int unit;

        [field: SerializeField, Unit(GgMaths.Units.Percentage)]
        private int Unit { get; set; }

    } // class end
}

#endif