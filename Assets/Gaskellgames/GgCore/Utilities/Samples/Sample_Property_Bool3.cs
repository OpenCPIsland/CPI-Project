using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class Sample_Property_Bool3 : MonoBehaviour
    {
        // ---------- Bool3 ----------

        [SerializeField]
        private Bool3 bool3;

        [SerializeField, Space]
        private GgMaths.LogicGate logicType = GgMaths.LogicGate.AND;

        [SerializeField]
        private bool output;

        private void OnValidate()
        {
            bool3 ??= new Bool3();
            output = bool3.LogicOutput(logicType);
        }

    } // class end
}