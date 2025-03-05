#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_GUIColor", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_GUIColor : ScriptableObject
    {
        // ---------- GUIColor ----------

        [SerializeField, GUIColor(223, 050, 050, 255, GUIColorAttribute.Target.All)]
        private GameObject objectField1;

        [SerializeField, GUIColor(223, 050, 050, 255, GUIColorAttribute.Target.Background)]
        private GameObject objectField2;

        [field: SerializeField, GUIColor(223, 050, 050, 255, GUIColorAttribute.Target.Content)]
        private GameObject ObjectProperty { get; set; }


        [SerializeField, GUIColor(050, 179, 050, 255, GUIColorAttribute.Target.All), Space]
        private LayerMask dropdownField1;

        [SerializeField, GUIColor(050, 179, 050, 255, GUIColorAttribute.Target.Background)]
        private LayerMask dropdownField2;

        [field: SerializeField, GUIColor(050, 179, 050, 255, GUIColorAttribute.Target.Content)]
        private LayerMask DropdownProperty { get; set; }


        [SerializeField, GUIColor(000, 179, 223, 255, GUIColorAttribute.Target.All), Space]
        private string stringField1;

        [SerializeField, GUIColor(000, 179, 223, 255, GUIColorAttribute.Target.Background)]
        private string stringField2;

        [field: SerializeField, GUIColor(000, 179, 223, 255, GUIColorAttribute.Target.Content)]
        private string StringProperty { get; set; }

    } // class end
}
#endif