using UnityEngine;

namespace Gaskellgames
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [AddComponentMenu("Gaskellgames/GgCore/Lock To Layer")]
    public class LockToLayer : GGMonoBehaviour, IEditorUpdate
    {
        [SerializeField]
        private LayerDropdown layerLock;

        private void LateUpdate()
        {
            HandleLayerLock();
        }

        public void EditorUpdate()
        {
            HandleLayerLock();
        }

        private void HandleLayerLock()
        {
            if (gameObject.layer != layerLock.value)
            {
                gameObject.layer = layerLock.value;
            }
        }
        
    } // class end 
}
