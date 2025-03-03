// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Get values from a Gamepad button. " + XboxGamepad)]
    public class GamepadGetButtonValues : GamepadActionBase
    {
        [ObjectType(typeof(GamepadButton))]
        [Tooltip("The Gamepad button to test.")]
        public FsmEnum button;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store button's current value.")]
        public FsmFloat currentValue;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store if the button is pressed. This is true if its current value is greater than a predetermined threshold.")]
        public FsmBool isPressed;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores how long the button has been pressed. Resets to zero when released.")]
        public FsmFloat heldTime;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores how many times the button has been pressed while this State was active. Useful for 'double-click' buttons.")]
        public FsmInt pressedCount;

        private float pressedStartTime;

        public override void Reset()
        {
            base.Reset();

            button = null;
            currentValue = null;
        }

        public override void OnEnter()
        {
            pressedCount.Value = 0;
            heldTime.Value = 0;

            base.OnEnter();
        }

        protected override void Execute()
        {
            var readButton = GetButtonControl((GamepadButton) button.Value);
            if (readButton == null) return;

            currentValue.Value = readButton.ReadValue();

            if (readButton.wasPressedThisFrame)
            {
                pressedStartTime = Time.time;
                pressedCount.Value += 1;
            }

            var pressed = readButton.isPressed;
            isPressed.Value = pressed;
            if (pressed)
            {
                heldTime.Value += Time.time - pressedStartTime;
            }
            else
            {
                heldTime.Value = 0;
            }
        
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("GetButtonValue", button, currentValue);
        }
#endif
    }
}

#endif
