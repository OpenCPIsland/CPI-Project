// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Checks Gamepad buttons. " + XboxGamepad)]
    public class GamepadButtonEvents : GamepadActionBase
    {
        [ObjectType(typeof(GamepadButton))]
        [Tooltip("The Gamepad button to test.")]
        public FsmEnum button;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store if the button is pressed.")]
        public FsmBool isPressed;

        [Tooltip("Event to send if the button is pressed.")]
        public FsmEvent isPressedEvent;

        [Tooltip("Event to send if the button was pressed this frame.")]
        public FsmEvent wasPressedThisFrame;

        [Tooltip("Event to send if the button was released this frame.")]
        public FsmEvent wasReleasedThisFrame;

        public override void Reset()
        {
            base.Reset();

            button = GamepadButton.ButtonEast;
            isPressed = null;
            isPressedEvent = null;
            wasPressedThisFrame = null;
            wasReleasedThisFrame = null;
        }

        protected override void Execute()
        {
            var buttonControl = GetButtonControl((GamepadButton) button.Value);

            isPressed.Value = buttonControl.isPressed;

            if (isPressedEvent != null && buttonControl.isPressed)
            {
                Fsm.Event(isPressedEvent);
            }

            if (wasPressedThisFrame != null && buttonControl.wasPressedThisFrame)
            {
                Fsm.Event(wasPressedThisFrame);
            }

            if (wasReleasedThisFrame != null && buttonControl.wasReleasedThisFrame)
            {
                Fsm.Event(wasReleasedThisFrame);
            }
        }


#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("ButtonEvent", button);
        }
#endif

    }
}

#endif
