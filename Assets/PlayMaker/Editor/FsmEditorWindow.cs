// (c) Copyright HutongGames, LLC. All rights reserved.

//#define PROFILE_PLAYMAKER_EDITOR

#if PROFILE_PLAYMAKER_EDITOR
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#endif

using System.Collections;
using HutongGames.Editor;
using HutongGames.PlayMaker;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if PLAYMAKER_SOURCE
using EditorCoroutines;
#endif

/* NOTE: Wrapper no longer needed in Unity 4.x
 * BUT changing it breaks saved layouts
 * SO wrap in namespace instead (which is also now supported in 4.x)
 */

// EditorWindow classes can't be called from a dll 
// so create a thin wrapper class as a workaround

namespace HutongGames.PlayMakerEditor
{
    [System.Serializable]
    internal class FsmEditorWindow : BaseEditorWindow
    {
        // Only one instance allowed
        // PM2 will allow multiple FSM editor windows         
        private static FsmEditorWindow instance;

        /// <summary>
        /// Open the main Fsm Editor
        /// </summary>
        public static void OpenWindow()
        {
            OpenWindow<FsmEditorWindow>();
        }

        /// <summary>
        /// Opens a PlayMaker Tool Window.
        /// Opens the main Fsm Editor window first if not open already.
        /// </summary>
        public static void OpenToolWindow<T>() where T : EditorWindow
        {
            if (instance == null)
            {
                OpenWindow();

                // Tool windows check if the main editor is open
                // so we wait a frame to open the tool window
                // otherwise it will close itself after opening

                EditorApplication.delayCall += DelayedOpenWindow<T>;
            }
            else
            {
                OpenWindow<T>();
            }
        }

        /// <summary>
        /// Add one more delay.
        /// Otherwise window would sometimes close immediately.
        /// </summary>
        private static void DelayedOpenWindow<T>() where T : EditorWindow
        {
            EditorApplication.delayCall += () => OpenWindow<T>();
        }

        /// <summary>
        /// Open a window and optionally "ping" it if it's already open.
        /// </summary>
        public static void OpenWindow<T>(string id = "Window") where T : EditorWindow
        {
            // Sometimes it's confusing in Unity when you open a window
            // that's already open but you get no feedback and can't find it.
            // We fix this by "pinging" the window if it's already open.
            
            if (FsmEditorSettings.PingOpenEditorWindows)
            {
                var window = Resources.FindObjectsOfTypeAll<T>();
                if (window.Length > 0)
                {
                    HighlighterHelper.PingHighlight(typeof(T), id);
                }
            }

            GetWindow<T>();
        }

        /// <summary>
        /// Open the main Fsm Editor and select an Fsm Component
        /// </summary>
        public static void OpenWindow(PlayMakerFSM fsmComponent)
        {
            OpenWindow();

            FsmEditor.SelectFsm(fsmComponent.Fsm);
        }

        /// <summary>
        /// Open the Fsm Editor and select an Fsm Component
        /// </summary>
        public static void OpenWindow(FsmTemplate fsmTemplate)
        {
            OpenWindow();

            FsmEditor.SelectFsm(fsmTemplate.fsm);
        }

        /// <summary>
        /// Is the Fsm Editor open?
        /// </summary>
        public static bool IsOpen()
        {
            return instance != null;
        }

        /// <summary>
        /// Open a PlayMakerFSM component in the main FSM Editor.
        /// If the component uses a template we select it to edit.
        /// </summary>
        public static void OpenInEditor(PlayMakerFSM fsmComponent)
        {
            if (!IsOpen())
            {
                OpenWindow(fsmComponent);
            }
            else
            {
                FocusWindowIfItsOpen<FsmEditorWindow>();
                FsmEditor.SelectFsm(fsmComponent.FsmTemplate == null ? fsmComponent.Fsm : fsmComponent.FsmTemplate.fsm);
            }
        }

        /// <summary>
        /// Open an Fsm in the main FSM Editor
        /// </summary>
        public static void OpenInEditor(Fsm fsm)
        {
            if (fsm != null && fsm.Owner != null)
            {
                OpenInEditor(fsm.Owner as PlayMakerFSM);
            }
        }

        /// <summary>
        /// Open an Fsm in the main FSM Editor
        /// </summary>
        public static void OpenInEditor(GameObject go)
        {
            if (go != null)
            {
                OpenInEditor(FsmSelection.FindFsmOnGameObject(go));
            }
        }


        [SerializeField]
        private FsmEditor fsmEditor;

        // ReSharper disable UnusedMember.Local

        /// <summary>
        /// Delay initialization until first OnGUI to avoid interfering with runtime system initialization.
        /// </summary>
        public override void Initialize()
        {
            // Unmaximize fix : when unmaximizing, a new window is enabled and disabled.
            // Prevent it from overriding the instance pointer.
            if (instance == null)
            {
                instance = this;
            }

#if PROFILE_PLAYMAKER_EDITOR
            var stopwatch = Stopwatch.StartNew();
#endif

            if (fsmEditor == null)
            {
                fsmEditor = new FsmEditor();
            }

            fsmEditor.InitWindow(this);
            fsmEditor.OnEnable();

#if PROFILE_PLAYMAKER_EDITOR            
            if (FsmEditor.debugStartupTime) 
                Debug.Log("Stopwatch: PlayMaker Editor Startup Time: " + stopwatch.ElapsedMilliseconds);
#endif
        }

        public override void InitWindowTitle()
        {
            SetTitle(Strings.ProductName);
        }

        protected override void DoUpdateHighlightIdentifiers()
        {
            // Not called? Need to investigate further...
            //fsmEditor.DoUpdateHighlightIdentifiers();
        }

        public override void DoGUI()
        {
            fsmEditor.OnGUI();

            switch (eventType)
            {
                case EventType.ValidateCommand:
                    switch (Event.current.commandName)
                    {
                        case "Cut":
                        case "Copy":
                        case "Paste":
                        case "SelectAll":
                            Event.current.Use();
                            break;
                    }

                    break;

                case EventType.ExecuteCommand:
                    switch (Event.current.commandName)
                    {
                        // NOTE: OSX 2018.3 needs Event.current.Use();
                        // otherwise e.g., it pastes twice #1814

                        case "Cut":
                            FsmEditor.Cut();
                            Event.current.Use();
                            break;

                        case "Copy":
                            FsmEditor.Copy();
                            Event.current.Use();
                            break;

                        case "Paste":
                            FsmEditor.Paste();
                            Event.current.Use();
                            break;

                        case "SelectAll":
                            FsmEditor.SelectAll();
                            Event.current.Use();
                            break;

                        case "OpenWelcomeWindow":
                            OpenWindow<PlayMakerWelcomeWindow>();
                            break;

                        case "OpenToolWindow":
                            OpenWindow<ContextToolWindow>();
                            break;

                        case "OpenFsmSelectorWindow":
                            OpenWindow<FsmSelectorWindow>();
                            break;

                        case "OpenFsmTemplateWindow":
                            OpenWindow<FsmTemplateWindow>();
                            break;

                        case "OpenStateSelectorWindow":
                            OpenWindow<FsmStateWindow>();
                            break;

                        case "OpenActionWindow":
                            OpenWindow<FsmActionWindow>();
                            break;

                        case "OpenGlobalEventsWindow":
                            OpenWindow<FsmEventsWindow>();
                            break;

                        case "OpenGlobalVariablesWindow":
                            OpenWindow<FsmGlobalsWindow>();
                            break;

                        case "OpenErrorWindow":
                            OpenWindow<FsmErrorWindow>();
                            break;

                        case "OpenTimelineWindow":
                            OpenWindow<FsmTimelineWindow>();
                            break;

                        case "OpenFsmLogWindow":
                            OpenWindow<FsmLogWindow>();
                            break;

                        case "OpenAboutWindow":
                            OpenWindow<AboutWindow>();
                            break;

                        case "OpenReportWindow":
                            OpenWindow<ReportWindow>();
                            break;

                        case "AddFsmComponent":
                            PlayMakerMainMenu.AddFsmToSelected();
                            Event.current.Use();
                            break;

                        case "ChangeLanguage":
                            ResetWindowTitles();
                            Event.current.Use();
                            break;

                        case "OpenFsmControlsWindow":
                            OpenWindow<FsmControlsWindow>();
                            break;
                    }

                    GUIUtility.ExitGUI();
                    break;
            }
        }

        /// <summary>
        /// Called when you change editor language
        /// </summary>
        public void ResetWindowTitles()
        {
            var windows = Resources.FindObjectsOfTypeAll<BaseEditorWindow>();
            foreach (var window in windows)
            {
                window.InitWindowTitle();
            }
        }

        public void RepaintAllWindows()
        {
            if (fsmEditor != null)
            {
                fsmEditor.RepaintAllWindows();
            }
        }

        private void Update()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.Update();
            }
        }

        private void OnInspectorUpdate()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnInspectorUpdate();
            }
        }

        private void OnFocus()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnFocus();
            }
        }

        private void OnBecameVisible()
        {
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnSelectionChange();
            }
        }

        private void OnHierarchyChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnHierarchyChange();
            }
        }

        /// <summary>
        /// Handler for message that is sent whenever the state of the project changes.
        /// Actions that trigger this message include creating, renaming, or re-parenting assets,
        /// as well as moving or renaming folders in the project.
        /// Note that the message is not sent immediately in response to these actions,
        /// but rather during the next update of the editor application.
        /// https://docs.unity3d.com/ScriptReference/EditorWindow.OnProjectChange.html
        /// </summary>
        private void OnProjectChange()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnProjectChange();
            }
        }

        private void OnDisable()
        {
            if (Initialized && fsmEditor != null)
            {
                fsmEditor.OnDisable();
            }

            HighlighterHelper.Reset(GetType());

            if (instance == this)
            {
                instance = null;
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                CloseAllWindowsThatNeedMainEditor();
            }
        }

#if PLAYMAKER_SOURCE

        public override IEnumerator CaptureDocScreenshots()
        {
            position = new Rect(100,100,695,305);

            FsmEditorSettings.GraphViewShowMinimap = false;
            FsmEditorSettings.ShowScrollBars = false;
            FsmEditorSettings.ShowFsmDescriptionInGraphView = false;

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();            
            EditorSceneManager.OpenScene("assets/docs/fake-scene.unity");
            
            Selection.activeGameObject = GameObject.FindWithTag("MainCamera");
            FsmEditor.SelectFsm(Selection.activeGameObject.GetComponent<PlayMakerFSM>());
            FsmEditor.Inspector.SetMode(InspectorMode.StateInspector);
            FsmEditor.InspectorPanelWidth = 350;
            FsmEditor.GraphView.SetScrollPosition(new Vector2(0, -30));
            FsmEditor.Selection.ActiveTransition = null;
            Repaint();
            
            yield return this.StartCoroutine(Capture("main-editor"));

            //FsmEditor.Inspector.SetMode(InspectorMode.FsmInspector);
            var captureWidth = position.width;
            var inspectorModeArea = new Rect(position.xMax - captureWidth, position.y, captureWidth, 87);
            yield return this.StartCoroutine(Capture("inspector-modes", inspectorModeArea));

            var selectionToolbarArea = new Rect(position.x, position.y, position.width - 345, 100);
            yield return this.StartCoroutine(Capture("selection-toolbar", selectionToolbarArea));


            Selection.activeGameObject = GameObject.Find("Camera");
            FsmEditor.SelectFsm(Selection.activeGameObject.GetComponent<PlayMakerFSM>());
            FsmEditor.Inspector.SetMode(InspectorMode.Watermarks);
            yield return this.StartCoroutine(Capture("inspector-watermarks"));

            var inspectorArea = new Rect(position.xMax - 350, position.y, 350, position.height);
            FsmEditor.Inspector.SetMode(InspectorMode.StateInspector);
            yield return this.StartCoroutine(Capture("state-inspector", inspectorArea));
            FsmEditor.Inspector.SetMode(InspectorMode.EventManager);
            yield return this.StartCoroutine(Capture("event-manager", inspectorArea));
            FsmEditor.Inspector.SetMode(InspectorMode.VariableManager);
            yield return this.StartCoroutine(Capture("variable-manager", inspectorArea));

            /* Doesn't work. Dropdown is modal?
            MainToolbar.ScreenshotMessageId = 2; Repaint();
            yield return this.StartCoroutine(Capture("fsm-selection-dropdown", selectionToolbarArea));
            MainToolbar.ScreenshotMessageId = 0;*/
        }

#endif

        // ReSharper restore UnusedMember.Local
    }
}
