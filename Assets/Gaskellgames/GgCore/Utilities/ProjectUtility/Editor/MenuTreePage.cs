#if UNITY_EDITOR

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [System.Serializable]
    public class MenuTreePage
    {
        public delegate void MethodDelegate(); // defines what type of method you're going to call.
        
        public string pageName;
        public MethodDelegate drawPageMethod;
        
        public MenuTreePage()
        {
            this.pageName = "";
            this.drawPageMethod = null;
        }
        
        public MenuTreePage(MethodDelegate drawPageMethod)
        {
            this.pageName = drawPageMethod.Method.Name.NicifyName();
            this.drawPageMethod = drawPageMethod;
        }
        
        public MenuTreePage(MethodDelegate drawPageMethod, string pageName)
        {
            this.pageName = pageName;
            this.drawPageMethod = drawPageMethod;
        }
        
    } // class end
}

#endif