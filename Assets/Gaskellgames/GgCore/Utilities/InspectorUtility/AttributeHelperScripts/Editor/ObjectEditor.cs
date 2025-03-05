#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames
    /// Original code from 'EditorCools': https://github.com/datsfain/EditorCools
    /// </summary>
    
    [CustomEditor(typeof(Object), true), CanEditMultipleObjects]
    internal class ObjectEditor : Editor
    {
        private ButtonDrawer buttonDrawer;

        private void OnEnable()
        {
            buttonDrawer = new ButtonDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            buttonDrawer.DrawButtons(targets);
        }
        
    } // class end
}
#endif