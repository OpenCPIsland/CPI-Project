#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_ShowAsTag", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_ShowAsTag : ScriptableObject
    {
        // ---------- StringDropdown ----------

        [SerializeField]
        private string stringValue = "TagValue";

        [SerializeField, ShowAsTag, Space]
        private string showAsTag;

        private void OnValidate()
        {
            showAsTag = stringValue;
        }
        
    } // class end
}

#endif