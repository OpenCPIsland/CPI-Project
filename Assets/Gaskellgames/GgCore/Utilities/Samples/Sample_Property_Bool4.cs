using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class Sample_Property_Bool4 : MonoBehaviour
    {
        // ---------- Bool4 ----------

        [SerializeField]
        private Bool4 bool4;

        [SerializeField, Space]
        private GgMaths.LogicGate logicType = GgMaths.LogicGate.AND;

        [SerializeField]
        private bool output;

        private void OnValidate()
        {
            bool4 ??= new Bool4();
            output = bool4.LogicOutput(logicType);
        }

    } // class end
}