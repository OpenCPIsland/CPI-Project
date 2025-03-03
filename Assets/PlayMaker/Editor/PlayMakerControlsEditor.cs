using System;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Easily edit multiple FSMs on a single GameObject
    /// Exposes name, input variables...
    /// </summary>
    [DisallowMultipleComponent]
    [CustomEditor(typeof(PlayMakerControls))]
    public class PlayMakerControlsEditor : UnityEditor.Editor
    {
        private FsmControlsPanel controlsPanel;

        [UsedImplicitly]
        private void OnEnable()
        {
            FsmEditor.OnFsmChanged -= OnFsmChanged; // just in case
            FsmEditor.OnFsmChanged += OnFsmChanged;

            InitControls();
        }

        private void InitControls()
        {
            controlsPanel = new FsmControlsPanel(((MonoBehaviour)target).gameObject, true);
        }

        private void OnFsmChanged(Fsm fsm)
        {
            Repaint();
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            FsmEditor.OnFsmChanged -= OnFsmChanged;
        }

        public override void OnInspectorGUI()
        {
            FsmEditorStyles.Init();

            EditorGUILayout.Space();

            try
            {
                controlsPanel.OnGUI();
            }
            catch (Exception e)
            {
                if (e is ExitGUIException) throw;

                // FSMs might have been edited invalidating our controls,
                // so we have to InitControls again
                InitControls();

                // ExitGUI to avoid layout errors
                GUIUtility.ExitGUI();
            }

            GUILayout.Space(4);
        }

    }


}
