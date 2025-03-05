#if UNITY_EDITOR

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    //[CreateAssetMenu(fileName = "GaskellgamesHubSettings", menuName = "Gaskellgames/GaskellgamesHubSettings")]
    public class GaskellgamesHubSettings_SO : GGScriptableObject
    {
        #region Variables
        
        public enum TransformInspector
        {
            TransformUtilities,
            ResetButtons,
            DefaultUnity
        }
        
        public bool showHubOnStartup = true;
        public bool showPackageBanners = true;
        public TransformInspector enableTransformInspector = TransformInspector.TransformUtilities;

        #endregion
        
    } // class end
}
#endif