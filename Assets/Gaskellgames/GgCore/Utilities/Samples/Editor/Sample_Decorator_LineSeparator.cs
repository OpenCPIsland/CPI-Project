#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Decorator_LineSeparator", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Decorator_LineSeparator : ScriptableObject
    {
        // ---------- LineSeparator ----------

        [SerializeField, LineSeparator]
        private float lineSeparator;

        [LineSeparator(true, false)]
        [SerializeField, LineSeparator]
        private float lineSeparator2;

        [SerializeField, LineSeparator(000, 028, 045, 255)]
        private float lineSeparator4;

        [field: SerializeField, LineSeparator(5)]
        private float lineSeparator5 { get; set; }

    } // class end
}

#endif