using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class Sample_Property_GGEvent : MonoBehaviour
    {
        // ---------- GGEvent ----------

        [SerializeField]
        public GgEvent zeroArgsEvent;

        [SerializeField]
        public GgEvent<int> singleArgsEvent;

        [SerializeField]
        public GgEvent<int, int> doubleArgsEvent;

        [SerializeField]
        public GgEvent<int, int, int> tripleArgsEvent;

    } // class end
}
