//-----------------------------------------------------------------------
// <copyright file="FsmComponentInspector.cs" company="Hutong Games LLC">
// Copyright (c) Hutong Games LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using HutongGames.Editor;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Custom inspector for PlayMakerFSM
    /// </summary>
    [CustomEditor(typeof(PlayMakerFSM))]
    public class FsmComponentInspector : UnityEditor.Editor
    {
        private PlayMakerFSM fsmComponent;
        private FsmInspector fsmInspector;
        private EditorWindow inspectorWindow;

        public void OnEnable()
        {
            fsmComponent = (PlayMakerFSM) target;

            Init();
        }

        private void OnDisable()
        {
            PlayMakerFSM.OnSettingChanged -= DoSettingsChanged;
        }

        private void Init()
        {
            //Debug.Log("FsmComponentInspector.Init()");

            fsmInspector = new FsmInspector(fsmComponent.Fsm, true);
            fsmInspector.OnEditButtonPressed += () => { FsmEditorWindow.OpenInEditor(fsmComponent); };
            FsmEditor.OnFsmControlsChanged += (fsm) => Repaint();

            FsmEditorSettings.LoadSettings();
            PlayMakerFSM.OnSettingChanged = DoSettingsChanged;
        }

        private void DoSettingsChanged(string setting)
        {
            switch (setting)
            {
                case "ShowFullFsmInspector":
                    FsmEditorSettings.ShowFullFsmInspector = !FsmEditorSettings.ShowFullFsmInspector;
                    break;
            }

            FsmEditorSettings.SaveSettings();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            try
            {
                fsmInspector.OnGUI();
            }
            catch(Exception e)
            {
                if (e is ExitGUIException) throw;

                //TODO: detect if stuck in loop here
                // E.g., set a "triedToRecover" flag that is cleared when OnGUI succeeds.
                // If triedToRecover is true here it means we failed twice!

                fsmInspector.Reset();
                Repaint();

                GUIUtility.ExitGUI();
            }

            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                if (inspectorWindow == null)
                    inspectorWindow = EditorHacks.GetUnityInspectorWindow();
                HighlighterHelper.Init(inspectorWindow);
                HighlighterHelper.FromGUILayout("PlayMakerFSM_" + fsmComponent.GetInstanceID());

                HighlighterHelper.OnGUI();
            }
        }

        #region Obsolete

        // These should be in FsmEditor,
        // but keeping here since they were public API

        /// <summary>
        /// Open the specified FSM in the Playmaker Editor
        /// </summary>
        [Obsolete("Use FsmEditorWindow.OpenInEditor instead")]
        public static void OpenInEditor(PlayMakerFSM fsmComponent)
        {
            FsmEditorWindow.OpenInEditor(fsmComponent);
        }

        /// <summary>
        /// Open the specified FSM in the Playmaker Editor
        /// </summary>
        [Obsolete("Use FsmEditorWindow.OpenInEditor instead")]
        public static void OpenInEditor(Fsm fsm)
        {
            FsmEditorWindow.OpenInEditor(fsm.Owner as PlayMakerFSM);
        }

        /// <summary>
        /// Open the first PlayMakerFSM on a GameObject in the Playmaker Editor
        /// </summary>
        [Obsolete("Use FsmEditorWindow.OpenInEditor instead")]
        public static void OpenInEditor(GameObject go)
        {
            FsmEditorWindow.OpenInEditor(FsmSelection.FindFsmOnGameObject(go));
        }

        #endregion

    }
}


