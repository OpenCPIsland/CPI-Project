#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_CustomCurve", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_CustomCurve : ScriptableObject
    {
        // ---------- Custom Curve ----------

        [SerializeField, CustomCurve]
        private AnimationCurve customCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [field: SerializeField, CustomCurve(223, 179, 000, 255)]
        private AnimationCurve CustomCurve { get; set; } = AnimationCurve.Linear(0, 0, 1, 1);

    } // class end
}
#endif
