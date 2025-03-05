#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_ShowAsString", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_ShowAsString : ScriptableObject
    {
        // ---------- ShowAsString ----------

        [SerializeField, ShowAsString]
        private Color32 colour = new Color32(000, 179, 223, 255);

        [field: SerializeField, ShowAsString]
        private Vector2 Vector2 { get; set; } = new Vector2(0, 0);

    } // class end
}

#endif