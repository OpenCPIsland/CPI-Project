#if UNITY_EDITOR
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [System.Serializable]
    public class ToolbarItem
    {
        public delegate void MethodDelegate(); // defines what type of method you're going to call.
        
        public MethodDelegate toolbarMethod;
        public GUIContent guiContent;
        public Color32 guiColor;
        
        public ToolbarItem(MethodDelegate toolbarMethod, GUIContent guiContent)
        {
            this.toolbarMethod = toolbarMethod;
            this.guiContent = guiContent;
            this.guiColor = GUI.backgroundColor;
        }
        
        public ToolbarItem(MethodDelegate toolbarMethod, GUIContent guiContent, Color32 guiColor)
        {
            this.toolbarMethod = toolbarMethod;
            this.guiContent = guiContent;
            this.guiColor = guiColor;
        }
        
    } // class end
}

#endif