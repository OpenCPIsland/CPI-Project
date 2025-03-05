#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_Graph", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Graph : ScriptableObject
    {
        // ---------- Graph ----------

        [SerializeField, Graph]
        private Vector2 graph;

        [field:SerializeField, Graph(-10, 10, "Custom x label", "Custom y label")]
        private Vector2 Graph2 { get; set; }

    } // class end
}
#endif