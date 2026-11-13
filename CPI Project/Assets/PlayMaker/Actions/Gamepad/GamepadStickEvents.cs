// (c) Copyright HutongGames, LLC 2021. All rights reserved.

using System;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Checks a Gamepad stick and translates its offset into events. " + XboxGamepad)]
    public class GamepadStickEvents : GamepadActionBase
    {
        public enum Stick
        {
            LeftStick,
            RightStick,
            DPad
        }

        [ObjectType(typeof(Stick))]
        [Tooltip("The Gamepad stick to test.")]
        public FsmEnum stick;

        [Tooltip("Event to send if input is to the left.")]
        public FsmEvent leftEvent;

        [Tooltip("Event to send if input is to the right.")]
        public FsmEvent rightEvent;

        [Tooltip("Event to send if input is to the up.")]
        public FsmEvent upEvent;

        [Tooltip("Event to send if input is to the down.")]
        public FsmEvent downEvent;

        [Tooltip("Event to send if input is in any direction.")]
        public FsmEvent anyDirection;

        [Tooltip("Event to send if no axis input (centered).")]
        public FsmEvent noDirection;

        public override void Reset()
        {
            base.Reset();

            stick = null;
            leftEvent = null;
            rightEvent = null;
            upEvent = null;
            downEvent = null;
            anyDirection = null;
            noDirection = null;
        }

        protected override void Execute()
        {
            var control = GetControl();
            if (control == null) return;

            var vector2 = Vector2.zero;

            var stickControl = control as StickControl;
            if (stickControl != null)
            {
                vector2 = stickControl.ReadValue();
            }

            var dPadControl = control as DpadControl;
            if (dPadControl != null)
            {
                vector2 = dPadControl.ReadValue();
            }

            var offset = vector2.sqrMagnitude;

            // no offset?

            if (offset.Equals(0))
            {
                if (noDirection != null)
                {
                    Fsm.Event(noDirection);
                }
                return;
            }

            // get integer direction sector (4 directions)
            // TODO: 8 directions? or new action?

            var angle = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg + 45f;
            if (angle < 0f)
            {
                angle += 360f;
            }

            var direction = (int)(angle / 90f);

            // send events bases on direction

            if (direction == 0 && rightEvent != null)
            {
                Fsm.Event(rightEvent);
                //Debug.Log("Right");
            }
            else if (direction == 1 && upEvent != null)
            {
                Fsm.Event(upEvent);
                //Debug.Log("Up");
            }
            else if (direction == 2 && leftEvent != null)
            {
                Fsm.Event(leftEvent);
                //Debug.Log("Left");
            }
            else if (direction == 3 && downEvent != null)
            {
                Fsm.Event(downEvent);
                //Debug.Log("Down");
            }
            else if (anyDirection != null)
            {
                // since we already no offset > 0

                Fsm.Event(anyDirection);
                //Debug.Log("AnyDirection");
            }

        }

        private InputControl GetControl()
        {
            switch ((Stick)stick.Value)
            {
                case Stick.LeftStick:
                    return gamepad.leftStick;
                case Stick.RightStick:
                    return gamepad.rightStick;
                case Stick.DPad:
                    return gamepad.dpad;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("StickEvent", stick);
        }
#endif

    }
}

#endif