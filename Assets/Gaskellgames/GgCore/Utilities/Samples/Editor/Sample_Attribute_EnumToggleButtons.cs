#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    //[CreateAssetMenu(fileName = "Sample_Attribute_EnumToggleButtons", menuName = "Gaskellgames/GGSamplePage")]
    public class Sample_Attribute_EnumToggleButtons : ScriptableObject
    {
        // ---------- EnumToggleButtons ----------

        private enum ExampleEnum
        {
            One,
            Two,
            Three,
            Four,
        }

        [Flags]
        private enum ExampleEnumFlags
        {
            None = 0,
            All = P1 | P2 | P3 | P4 | P5,

            P1 = 1 << 0,
            P2 = 1 << 1,
            P3 = 1 << 2,
            P4 = 1 << 3,
            P5 = 1 << 4,
        }

        [Title("Enum Buttons")]
        [SerializeField, EnumToggleButtons]
        private ExampleEnum enumButtons;

        [SerializeField, EnumToggleButtons(true)]
        private ExampleEnum enumButtonsShowLabel;

        [Title("Enum Buttons: Flags")]
        [SerializeField, EnumToggleButtons(true, true, true, true)]
        private ExampleEnumFlags enumFlags;

    } // class end
}

#endif