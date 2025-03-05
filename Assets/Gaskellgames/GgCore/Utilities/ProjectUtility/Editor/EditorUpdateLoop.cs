#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames: https://github.com/Gaskellgames/Unity_IEditorUpdate
    /// </summary>
    
    [InitializeOnLoad]
    public static class EditorUpdateLoop
    {
        internal static List<IEditorUpdate> iEditorUpdateList = new List<IEditorUpdate>();

        internal static Action OnIEditorUpdateListUpdated;
        
        /// <summary>
        /// Constructor will be called during initialisation
        /// </summary>
        static EditorUpdateLoop()
        {
            // clear any previous listeners
            AssemblyReloadEvents.afterAssemblyReload -= HandleIEditorUpdateCallbacks;
            EditorApplication.hierarchyChanged -= HandleIEditorUpdateCallbacks;
            
            // assign listeners
            AssemblyReloadEvents.afterAssemblyReload += HandleIEditorUpdateCallbacks;
            EditorApplication.hierarchyChanged += HandleIEditorUpdateCallbacks;
        }

        /// <summary>
        /// Force update the editorUpdateList for IEditorUpdate
        /// </summary>
        internal static void ForceUpdateComponentList()
        {
            HandleIEditorUpdateCallbacks();
        }
        
        /// <summary>
        /// Handle subscribing to or from the editor update loop
        /// </summary>
        private static void HandleIEditorUpdateCallbacks()
        {
            // playmode check
            if (Application.isPlaying)
            {
                // unsubscribe all existing IEditorUpdate components from the editor update loop
                if (0 < iEditorUpdateList.Count) { UnsubscribeFromEditorUpdateLoop(); }
                return;
            }
            
            if (ComponentListUpdated(out List<IEditorUpdate> addedComponents, out List<IEditorUpdate> removedComponents))
            {
                foreach (IEditorUpdate removedComponent in removedComponents)
                {
                    EditorApplication.update -= removedComponent.HandleEditorUpdate;
                }
                foreach (IEditorUpdate addedComponent in addedComponents)
                {
                    EditorApplication.update += addedComponent.HandleEditorUpdate;
                }
                
                OnIEditorUpdateListUpdated.Invoke();
            }
        }

        /// <summary>
        /// Checks to see if the list of all IEditorUpdate has changed
        /// </summary>
        /// <returns>True if changed, False if not</returns>
        private static bool ComponentListUpdated(out List<IEditorUpdate> addedComponents, out List<IEditorUpdate> removedComponents)
        {
            // get all 'component type' in open scenes
            List<IEditorUpdate> newEditorUpdateList = new List<IEditorUpdate>();
            List<GameObject> rootObjects = SceneExtensions.GetAllRootGameObjects();
            foreach(GameObject root in rootObjects)
            {
                IEditorUpdate[] componentsArray = root.GetComponentsInChildren<IEditorUpdate>(true);
                foreach (IEditorUpdate debugEvent in componentsArray)
                {
                    newEditorUpdateList.Add(debugEvent);
                }
            }
        
            // check to see if the list has been updated
            bool updated = false;
            addedComponents = new List<IEditorUpdate>();
            removedComponents = new List<IEditorUpdate>();
            foreach (IEditorUpdate editorUpdate in iEditorUpdateList)
            {
                if (newEditorUpdateList.Contains(editorUpdate)) { continue; }
                removedComponents.Add(editorUpdate);
                updated = true;
            }
            foreach (IEditorUpdate editorUpdate in newEditorUpdateList)
            {
                if (iEditorUpdateList.Contains(editorUpdate)) { continue; }
                addedComponents.Add(editorUpdate);
                updated = true;
            }

            if (updated) { iEditorUpdateList = newEditorUpdateList; }
            return updated;
        }

        /// <summary>
        /// Unsubscribe a list of components with IEditorUpdate from the editor update loop
        /// </summary>
        private static void UnsubscribeFromEditorUpdateLoop()
        {
            foreach (IEditorUpdate editorUpdate in iEditorUpdateList)
            {
                if (!(Object)editorUpdate) { continue; }
                EditorApplication.update -= editorUpdate.HandleEditorUpdate;
            }
            iEditorUpdateList.Clear();
        }

    } // class end
}
#endif
