// (c) Copyright HutongGames, LLC. All rights reserved.

//#define DEBUG_LOG

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;
using UnityEditor.Build.Reporting;

namespace HutongGames.PlayMakerEditor
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class PlayMakerBuildCallbacks
    {

#if UNITY_2019_3_OR_NEWER

        // For fast play mode in editor

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitInEditor()
        {
            PlayMakerGlobals.InitInEditor();
            PlayMakerFSM.InitInEditor();
            FsmEvent.InitInEditor();
            FsmLog.InitInEditor();
        }

#endif

        /* See PlayMakerProjectTools 
        public class PlayMakerPreProcessBuild : IPreprocessBuildWithReport
        {
            public int callbackOrder { get { return 0; } }
            
            public void OnPreprocessBuild(BuildReport report)
            {
                Debug.Log("PlayMakerPreProcessBuild...");
                ProjectTools.PreprocessPrefabFSMs();
            }
        }*/
    
        [PostProcessScene(2)]
        public static void OnPostProcessScene()
        {
            // No need to post process if playing in editor,
            // sine we're not really making a build
            if (Application.isPlaying) return;

            DebugLog("OnPostProcessScene", LogColor.Yellow);

            PlayMakerGlobals.IsBuilding = true;
            PlayMakerGlobals.InitApplicationFlags();

            var fsmList = Resources.FindObjectsOfTypeAll<PlayMakerFSM>();
            foreach (var playMakerFSM in fsmList)
            {
                // not sure when this happens, but need to catch it...
                if (playMakerFSM == null) continue; 
                    
                // PlayMakerPreProcessBuild has already processed prefabs
                if (FsmPrefabs.IsPrefab(playMakerFSM)) continue;

                playMakerFSM.Preprocess();

                StripEditorData(playMakerFSM);
            }

            PlayMakerGlobals.IsBuilding = false;

            //Debug.Log("EndPostProcessScene");
        }

        /// <summary>
        /// Try to minimize size of data
        /// </summary>
        /// <param name="fsmComponent"></param>
        private static void StripEditorData(PlayMakerFSM fsmComponent)
        {
            if (fsmComponent == null) return;
            
            var fsm = fsmComponent.Fsm;
            if (fsm == null) return;

            DebugLog("StripEditorData: " + Labels.GetFullFsmLabelWithInstanceID(fsm), LogColor.Yellow);

#if PM2
            fsm.EditorData = "";
#endif

            fsm.Description = "";
            fsm.Watermark = "";

            foreach (var state in fsm.States)
            {
                state.Description = "";
            }

            fsm.Variables.DeleteEmptyVariables();
            fsm.Variables.StripEditorOnlyData();
        }

        #region Debug

        [Conditional("DEBUG_LOG")]
        private static void DebugLog(object message, LogColor logColor = LogColor.None)
        {
            LogHelper.Log("PlayMakerBuildCallbacks", message, logColor);
        }

        #endregion

    }
}
