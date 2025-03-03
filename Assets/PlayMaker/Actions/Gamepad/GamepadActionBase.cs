// (c) Copyright HutongGames, LLC 2021. All rights reserved.

using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HutongGames.PlayMaker.Actions
{
    [NoActionTargets]
    [ActionCategory("Gamepad")]
    [SeeAlso("New Unity Input Manager")]
    public abstract class GamepadActionBase : FsmStateAction
    {
        public const string XboxGamepad = "Assumes an Xbox-style gamepad with four face buttons, " +
                                          "two triggers, two shoulder buttons, and two menu buttons.";

        public enum UpdateMode
        {
            Once,
            Update,
            FixedUpdate,
        }

        [Tooltip("When to read the Input.")]
        public UpdateMode updateMode;

        protected Gamepad gamepad;

        public override void Reset()
        {
            updateMode = UpdateMode.Update;
        }

        public override void OnPreprocess()
        {
            Fsm.HandleFixedUpdate = updateMode == UpdateMode.FixedUpdate;
        }

        public override void OnEnter()
        {
            DoAction();

            if (updateMode == UpdateMode.Once)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (updateMode != UpdateMode.Update) return;

            DoAction();
        }

        public override void OnFixedUpdate()
        {
            if (updateMode != UpdateMode.FixedUpdate) return;

            DoAction();
        }

        private void DoAction()
        {
            gamepad = Gamepad.current;
            if (gamepad == null) return;

            Execute();
        }

        protected virtual void Execute() { }

        protected ButtonControl GetButtonControl(GamepadButton button)
        {
            switch (button)
            {
                case GamepadButton.ButtonNorth:
                    return gamepad.buttonNorth;
                case GamepadButton.ButtonEast:
                    return gamepad.buttonEast;
                case GamepadButton.ButtonWest:
                    return gamepad.buttonWest;
                case GamepadButton.ButtonSouth:
                    return gamepad.buttonSouth;
                case GamepadButton.LeftTrigger:
                    return gamepad.leftTrigger;
                case GamepadButton.RightTrigger:
                    return gamepad.rightTrigger;
                case GamepadButton.LeftShoulder:
                    return gamepad.leftShoulder;
                case GamepadButton.RightShoulder:
                    return gamepad.rightShoulder;
                case GamepadButton.SelectButton:
                    return gamepad.selectButton;
                case GamepadButton.StartButton:
                    return gamepad.startButton;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif