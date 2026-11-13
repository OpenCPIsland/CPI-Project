// (c) Copyright HutongGames, LLC 2021. All rights reserved.

using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Reads the value of a Gamepad button. " + XboxGamepad)]
    public class GamepadReadStickValue : GamepadActionBase
    {
        [ObjectType(typeof(GamepadStick))]
        [Tooltip("The Gamepad stick to test.")]
        public FsmEnum stick;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the stick's current value.")]
        public FsmVector2 storeVector2Value;
        
        public override void Reset()
        {
            base.Reset();

            stick = null;
            storeVector2Value = null;
        }

        protected override void Execute()
        {
            var control = GetControl();
            if (control == null) return;

            var stickControl = control as StickControl;
            if (stickControl != null)
            {
                storeVector2Value.Value = stickControl.ReadValue();
                return;
            }

            var dPadControl = control as DpadControl;
            if (dPadControl != null)
            {
                storeVector2Value.Value = dPadControl.ReadValue();

            }
        }

        private InputControl GetControl()
        {
            switch ((GamepadStick) stick.Value)
            {
                case GamepadStick.LeftStick:
                    return gamepad.leftStick;
                case GamepadStick.RightStick:
                    return gamepad.rightStick;
                case GamepadStick.DPad:
                    return gamepad.dpad;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("ReadStickValue", stick, storeVector2Value);
        }
#endif
    }
}

#endif
