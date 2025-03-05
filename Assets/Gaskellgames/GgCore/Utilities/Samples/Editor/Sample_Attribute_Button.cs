#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "Sample_Attribute_Button", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_Button : ScriptableObject
    {
        // ---------- Button ----------

        [Button]
        private void MethodRow1()
        {
            Debug.Log("Method 1: Button Pressed");
        }

        [Button("Row2")]
        private void MethodRow2a()
        {
            Debug.Log("Method 2a: Button Pressed");
        }

        [Button("Row2")]
        private void MethodRow2b()
        {
            Debug.Log("Method 2b: Button Pressed");
        }

    } // class end
}

#endif