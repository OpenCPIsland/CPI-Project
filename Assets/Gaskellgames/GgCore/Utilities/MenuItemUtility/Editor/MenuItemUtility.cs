#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class MenuItemUtility
    {
        #region Menu Items
        
        private const string GgToolsMenu = "Tools/Gaskellgames";
        private const string GgWindowMenu = "Window/Gaskellgames";
        private const string GgGameObjectMenu = "GameObject/Gaskellgames";
        
        // Extensions
        
        public const string GameObjectMenu_Path = "GameObject";
        public const int GameObjectMenu_Priority = 0;
        
        // Gaskellgames Hub
        private const string Hub = "/Gaskellgames Hub";
        
        public const string Hub_ToolsMenu_Path = GgToolsMenu + Hub;
        public const int Hub_ToolsMenu_Priority = 100;
        
        public const string Hub_WindowMenu_Path = GgWindowMenu + Hub;
        public const int Hub_WindowMenu_Priority = 100;
        
        // GgCore
        public const string GgCoreSamples_ToolsMenu_Path = GgToolsMenu + "/GgCore Samples";
        public const int GgCoreSamples_ToolsMenu_Priority = 101;
        
        public const string GgCoreSamples_WindowMenu_Path = GgWindowMenu + "/GgCore Samples";
        public const int GgCoreSamples_WindowMenu_Priority = 101;
        
        private const string GgCore_GameObjectMenu_Path = GgGameObjectMenu + "/GgCore";
        private const int GgCore_GameObjectMenu_Priority = 10;
        
        // Audio Controller
        public const string AudioController_ToolsMenu_Path = GgToolsMenu + "/Audio Manager";
        public const int AudioController_ToolsMenu_Priority = 120;
        
        public const string AudioController_WindowMenu_Path = GgWindowMenu + "/Audio Manager";
        public const int AudioController_WindowMenu_Priority = 120;
        
        public const string AudioController_GameObjectMenu_Path = GgGameObjectMenu + "/Audio Controller";
        public const int AudioController_GameObjectMenu_Priority = 10;
        
        // Camera System
        public const string CameraSystem_GameObjectMenu_Path = GgGameObjectMenu + "/Camera System";
        public const int CameraSystem_GameObjectMenu_Priority = 10;
        
        // Character Controller
        public const string CharacterController_GameObjectMenu_Path = GgGameObjectMenu + "/Character Controller";
        public const int CharacterController_GameObjectMenu_Priority = 10;
        
        // Folder System
        public const string FolderSystem_ToolsMenu_Path = GgToolsMenu + "/Folder Manager";
        public const int FolderSystem_ToolsMenu_Priority = 121;
        
        public const string FolderSystem_WindowMenu_Path = GgWindowMenu + "/Folder Manager";
        public const int FolderSystem_WindowMenu_Priority = 121;
        
        public const string FolderSystem_GameObjectMenu_Path = GameObjectMenu_Path;
        public const int FolderSystem_GameObjectMenu_Priority = GameObjectMenu_Priority;
        
        // Input Event System
        public const string InputEventSystem_GameObjectMenu_Path = GgGameObjectMenu + "/Input Event System";
        public const int InputEventSystem_GameObjectMenu_Priority = 10;
        
        // Logic System
        public const string LogicSystem_GameObjectMenu_Path = GgGameObjectMenu + "/Logic System";
        public const int LogicSystem_GameObjectMenu_Priority = 10;
        
        // Platform Controller
        public const string PlatformController_GameObjectMenu_Path = GgGameObjectMenu + "/Platform Controller";
        public const int PlatformController_GameObjectMenu_Priority = 10;
        
        // Spline System
        public const string SplineSystem_GameObjectMenu_Path = GgGameObjectMenu + "/Spline System";
        public const int SplineSystem_GameObjectMenu_Priority = 10;
        
        // Scene Controller
        public const string SceneController_ToolsMenu_Path = GgToolsMenu + "/Scene Manager";
        public const int SceneController_ToolsMenu_Priority = 122;
        
        public const string SceneController_WindowMenu_Path = GgWindowMenu + "/Scene Manager";
        public const int SceneController_WindowMenu_Priority = 122;
        
        public const string SceneController_GameObjectMenu_Path = GgGameObjectMenu + "/Scene Controller";
        public const int SceneController_GameObjectMenu_Priority = 10;
        
        // Icon Finder
        public const string IconFinder_ToolsMenu_Path = GgToolsMenu + "/Icon Finder";
        public const int IconFinder_ToolsMenu_Priority = 123;
        
        public const string IconFinder_WindowMenu_Path = GgWindowMenu + "/Icon Finder";
        public const int IconFinder_WindowMenu_Priority = 123;
        
        // Pooling System
        public const string PoolingSystem_GameObjectMenu_Path = GgGameObjectMenu + "/Pooling System";
        public const int PoolingSystem_GameObjectMenu_Priority = 10;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Helper Functions

        public static GameObject SetupMenuItemInContext(MenuCommand menuCommand, string gameObjectName)
        {
            // create a custom gameObject, register in the undo system, parent and set position relative to context
            GameObject go = new GameObject(gameObjectName);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            GameObject context = (GameObject)menuCommand.context;
            if (context != null) { go.transform.SetParent(context.transform); }
            EditorExtensions.GetSceneViewLookAt(out Vector3 point);
            go.transform.position = point;

            VerboseLogs.Log($"'{gameObjectName}' created.");
            
            return go;
        }

        public static GameObject AddChildItemInContext(Transform parent, string gameObjectName)
        {
            GameObject go = new GameObject(gameObjectName);
            go.transform.parent = parent;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            
            return go;
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Gameobject Menu
        
        [MenuItem(GgCore_GameObjectMenu_Path + "/Comment", false, GgCore_GameObjectMenu_Priority)]
        private static void Gaskellgames_GameObjectMenu_Comment(MenuCommand menuCommand)
        {
            // create a custom gameObject, register in the undo system, parent and set position relative to context
            GameObject go = SetupMenuItemInContext(menuCommand, "Comment");
            
            // add scripts & components
            go.AddComponent<Comment>();
            
            // select newly created gameObject
            Selection.activeObject = go;
        }

        #endregion
        
    } // class end
}
#endif