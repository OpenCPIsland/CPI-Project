// (c) Copyright HutongGames, LLC 2020. All rights reserved.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace HutongGames.PlayMakerEditor
{
    [CustomEditor(typeof(PlayMakerGlobals))]
    internal class PlayMakerGlobalsInspector : UnityEditor.Editor
    {
	    private PlayMakerGlobals globals;

        private List<FsmVariable> variables;

        public void OnEnable()
	    {
		    globals = target as PlayMakerGlobals;

            Init();
	    }

        private void Init()
        {
            //Debug.Log("PlayMakerGlobalsInspector.Init");

            FsmEditorSettings.LoadSettings();

            if (globals != null)
            {
                variables = FsmVariable.GetFsmVariableList(globals);

                foreach (var fsmVariable in variables)
                {
                    fsmVariable.NamedVar.Init();
                }
            }

            Repaint();
        }

	    public override void OnInspectorGUI()
	    {
	        EditorGUIUtility.hierarchyMode = false;
	        FsmVariableEditor.UnityInspectorMode = true;

            FsmEditorStyles.Init();

            DoGlobalVariablesGUI();

            GUILayout.Space(20);

	        DoGlobalEventsGUI();

            GUILayout.Space(10);

            //DoImportExportGUI();
	    }

        private const float EditButtonWidth = 160;

        private void DoGlobalVariablesGUI()
        {
            DoSectionTitle(Strings.Command_Global_Variables);

            //EditorGUILayout.HelpBox(Strings.Hint_GlobalsInspector_Shows_DEFAULT_Values, MessageType.None);
            EditorGUILayout.HelpBox("NOTE: This inspector shows the default values of variables. " +
                                    "\nTo see current values while playing use the PlayMaker Editor: " +
                                    "\nGlobals Variables Window, State Inspector Debug etc.", MessageType.None);

            if (variables.Count > 0)
            {
                var listSerializedObject = variables[0].SerializedObject;
                listSerializedObject.Update();

                EditorGUI.BeginChangeCheck();

                FsmVariable.DoVariableListGUI(variables);

                if (EditorGUI.EndChangeCheck())
                {
                    listSerializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh"))
            {
                Init();
            }

            if (GUILayout.Button("Open Globals Window", GUILayout.Width(EditButtonWidth)))
            {
                if (FsmEditor.Instance == null) FsmEditor.Open();
                FsmEditor.OpenGlobalVariablesWindow();
            }

            GUILayout.EndHorizontal();
        }

        private static Rect sendIcon = new Rect();

        private void DoGlobalEventsGUI()
        {
            // TODO:
            // Context menu to find usages
            // Usages count

            DoSectionTitle(Strings.Label_Global_Events);

            foreach (var eventName in globals.Events)
            {
                var rect = GUILayoutUtility.GetRect(GUIContent.none, FsmEditorStyles.EventGlobalButton);
                sendIcon.Set(rect.x + 4, rect.y + 3, 12, rect.height - 4);
 
                FsmEditorContent.EventSendGlobalButton.text = eventName;
                if (GUI.Button(rect, FsmEditorContent.EventSendGlobalButton, FsmEditorStyles.EventGlobalButton))
                {
                    PlayMakerFSM.BroadcastEvent(eventName);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle.none.Draw(sendIcon, FsmEditorStyles.BroadcastIcon);
                }
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh"))
            {
                Init();
            }

            if (GUILayout.Button("Open Events Browser", GUILayout.Width(EditButtonWidth)))
            {
                if (FsmEditor.Instance == null) FsmEditor.Open();
                FsmEditor.OpenGlobalEventsWindow();
            }

            GUILayout.EndHorizontal();
        }

        private void DoSectionTitle(string title)
        {
            GUILayout.Label(title, FsmEditorStyles.LargeLabel);
            FsmEditorGUILayout.Divider();
            GUILayout.Space(10);
        }

        /*
        private static void DoImportExportGUI()
        {
            if (GUILayout.Button(Strings.Command_Export_Globals))
            {
                GlobalsAsset.Export();
            }

            if (GUILayout.Button(Strings.Command_Import_Globals))
            {
                GlobalsAsset.Import();
            }

            EditorGUILayout.HelpBox(Strings.Hint_Export_Globals_Notes, MessageType.None);
        }*/

    }
}

