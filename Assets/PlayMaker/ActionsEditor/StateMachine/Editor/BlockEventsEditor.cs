// (c) Copyright HutongGames, LLC. All rights reserved.

using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    [CustomActionEditor(typeof (HutongGames.PlayMaker.Actions.BlockEvents))]
	public class BlockEventsEditor : CustomActionEditor
    {
        private const string noFinishHelpText = "\nNote, the action will not finish with this setting.";

        private BlockEvents action;

        private object[] timeoutAttributes;
        private object[] realTimeAttributes;
        private object[] boolVariableAttributes;
        private object[] useEventAttributes;

        public override void OnEnable()
        {
            base.OnEnable();

            action = target as BlockEvents;

            timeoutAttributes = new object[] {new TooltipAttribute("Block events for the specified amount of time.")};
            realTimeAttributes = new object[] {new TooltipAttribute("Use real time, ignoring time scale.")};
            useEventAttributes = new object[] {new TooltipAttribute("Respond to the unblock event itself.")};
            boolVariableAttributes = new object[]
            {
                new TooltipAttribute("Bool variable to test."),
                new UIHintAttribute(UIHint.Variable)
            };
        }

        public override bool OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();

            EditField("condition");

            if (EditorGUI.EndChangeCheck())
            {
                SetParameterDefaults();
            }

            switch (action.condition)
            {
                case BlockEvents.Options.Timeout:

                    EditField("floatParam", "Timeout", timeoutAttributes);
                    EditField("boolParam", "Real Time", realTimeAttributes);
                    
                    break;

                case BlockEvents.Options.WhileTrue:
              
                    EditField("boolParam", "Bool Variable", boolVariableAttributes);
                    EditorGUILayout.HelpBox("Block events while this variable is true. " + noFinishHelpText, 
                        MessageType.None);
                    break;
                
                case BlockEvents.Options.WhileFalse:

                    EditField("boolParam", "Bool Variable", boolVariableAttributes);
                    EditorGUILayout.HelpBox("Block events while this variable is false. " + noFinishHelpText, 
                        MessageType.None);
                    break;
                
                case BlockEvents.Options.UntilTrue:
                    
                    EditField("boolParam", "Bool Variable", boolVariableAttributes);
                    EditorGUILayout.HelpBox("Block events until this variable becomes true.", 
                        MessageType.None);
                    break;

                case BlockEvents.Options.UntilFalse:

                    EditField("boolParam", "Bool Variable", boolVariableAttributes);
                    EditorGUILayout.HelpBox("Block events until this variable becomes false.", 
                        MessageType.None);
                    break;

                case BlockEvents.Options.UntilEvent:
                
                    EditField("eventParam", "Event");
                    EditorGUILayout.HelpBox("Block events until this event is received.", 
                        MessageType.None);
                    EditField("boolParam", "Use Event", useEventAttributes);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditField("logBlockedEvents");

            return EditorGUI.EndChangeCheck();
        }

        private void SetParameterDefaults()
        {
            switch (action.condition)
            {
                case BlockEvents.Options.Timeout:
                    action.floatParam = null;
                    action.boolParam = null;
                    break;
                case BlockEvents.Options.WhileTrue:
                case BlockEvents.Options.WhileFalse:
                case BlockEvents.Options.UntilTrue:
                case BlockEvents.Options.UntilFalse:
                    action.boolParam = null;
                    break;
                case BlockEvents.Options.UntilEvent:
                    action.eventParam = null;
                    action.boolParam = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}