using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [AddComponentMenu("Gaskellgames/GgCore/Comment")]
    public class Comment : GGMonoBehaviour
    {
        [SerializeField, Min(0)]
        private int lines = 3;
        
        [SerializeField, TextArea]
        private string comment = "Add comment here...";

        private void RemoveWarnings()
        {
            if (string.IsNullOrEmpty(comment) && lines < 3) {  };
        }
        
    } // class end
}