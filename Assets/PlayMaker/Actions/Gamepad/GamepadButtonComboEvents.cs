// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Checks Gamepad buttons for combos. " +
             "Combos are either buttons pressed at the same time or pressed in a specific sequence. " + XboxGamepad)]
    public class GamepadButtonComboEvents : GamepadActionBase
    {
        public enum Combo
        {
            SameTime,
            Sequence
        }

        [ObjectType(typeof(Combo))]
        [Tooltip("The type of combo to detect.")]
        public FsmEnum combo;

        [ArrayEditor(typeof(GamepadButton), "Button")]
        [Tooltip("The Gamepad button to test.")]
        public FsmArray buttons;

        [Tooltip("Time allowed for the next button press. Generally shorter for Same Time combos and longer for Sequences.")]
        public FsmFloat timeWindow;

        [Tooltip("Use unscaled time for time window.")]
        public FsmBool realTime;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store if the combo was detected.")]
        public FsmBool didSucceed;

        [Tooltip("Event to send if the combo was successfully executed.")]
        public FsmEvent successEvent;

        [Tooltip("Event to send if the combo failed (started but timed-out).")]
        public FsmEvent failedEvent;

        [Tooltip("Log Debug info to the Unity Console.")]
        public FsmBool debug;

        public override void Reset()
        {
            base.Reset();

            buttons = null;
            timeWindow = 0.2f;
            realTime = null;
            didSucceed = null;
            successEvent = null;
            failedEvent = null;
            debug = null;
        }

        private bool comboStarted;
        private int stepInSequence;
        private float timer;

        private readonly List<ButtonControl> comboButtons = new List<ButtonControl>();
        private List<ButtonControl> buttonsPressed = new List<ButtonControl>();
        private List<ButtonControl> validNextButtons;

        public override void OnEnter()
        {
            gamepad = Gamepad.current;
            if (gamepad == null) return;
            if (buttons.Length == 0) return;

            foreach (var button in buttons.Values)
            {
                comboButtons.Add( GetButtonControl((GamepadButton) button));
            }

            ResetCombo();
        }

        private void ResetCombo()
        {
            var comboType = (Combo) combo.Value;
            if (comboType == Combo.SameTime)
            {
                validNextButtons = new List<ButtonControl>(comboButtons);
            }
            else
            {
                validNextButtons = new List<ButtonControl> { comboButtons[0] };
            }

            buttonsPressed.Clear();
            didSucceed.Value = false;
            comboStarted = false;
            stepInSequence = 0;
            timer = 0;
        }

        protected override void Execute()
        {
            if (buttons.Length == 0) return;

            if (comboStarted)
            {
                timer += Time.deltaTime;
                if (timer > timeWindow.Value)
                {
                    if (debug.Value) Log("Combo Failed: Timeout");
                    Fsm.Event(failedEvent);
                    ResetCombo();
                }
            }

            // copy list so we can modify it in this loop
            var nextButtons = new List<ButtonControl>(validNextButtons);
            foreach (var button in nextButtons)
            {
                if (button.wasPressedThisFrame)
                {
                   DoComboStep(button);
                }
            }
        }

        private void DoComboStep(ButtonControl lastPressedButton)
        {
            if (!comboStarted) comboStarted = true;

            if (debug.Value) Log("Combo Button: " + lastPressedButton.name);

            timer = 0;

            var comboType = (Combo)combo.Value;
            if (comboType == Combo.SameTime)
            {
                validNextButtons.Remove(lastPressedButton);
            }
            else
            {
                validNextButtons.Clear();
                stepInSequence++;
                if (stepInSequence < comboButtons.Count)
                {
                    validNextButtons.Add(comboButtons[stepInSequence]);
                }
            }

            if (validNextButtons.Count == 0)
            {
                if (debug.Value) Log("Combo Succeeded!");
                didSucceed.Value = true;
                Fsm.Event(successEvent);
                ResetCombo();
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("ButtonComboEvent", buttons);
        }
#endif

    }
}
#endif