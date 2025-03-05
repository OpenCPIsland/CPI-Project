using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class Sample_Property_Bool2 : MonoBehaviour
    {
        // ---------- Bool2 ----------

        [SerializeField]
        private Bool2 bool2;

        [SerializeField, Space]
        private GgMaths.LogicGate logicType = GgMaths.LogicGate.AND;

        [SerializeField]
        private bool output;

        private void OnValidate()
        {
            bool2 ??= new Bool2();
            output = bool2.LogicOutput(logicType);
        }

    } // class end
}