using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class Sample_Property_Password : MonoBehaviour
    {
        // ---------- Password ----------

        [SerializeField]
        private Password password = new Password("StrongPassword");

        [SerializeField, ReadOnly, Space]
        private string passwordDebug;

        private void OnDrawGizmos()
        {
            RemoveConsoleWarning();
        }

        private void RemoveConsoleWarning()
        {
            passwordDebug = password.value;
        }

    } // class end
}