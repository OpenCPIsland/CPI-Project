// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM

namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Reads the value of a Gamepad button. " + XboxGamepad)]
    public class GamepadReadButtonValue : GamepadActionBase
    {
        [ObjectType(typeof(GamepadButton))]
        [Tooltip("The Gamepad button to test.")]
        public FsmEnum button;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store button's current value.")]
        public FsmFloat storeFloatValue;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store if the button is pressed. This is true if its current value is greater than a predetermined threshold.")]
        public FsmBool isPressed;

        public override void Reset()
        {
            base.Reset();

            button = null;
            storeFloatValue = null;
        }

        protected override void Execute()
        {
            var readButton = GetButtonControl((GamepadButton) button.Value);
            if (readButton == null) return;

            storeFloatValue.Value = readButton.ReadValue();
            isPressed.Value = readButton.isPressed;

        }

        public override void OnEnter()
        {
            LogWarning("Action requires new Input System!");
            Finish();
        }

#if UNITY_EDITOR
        public override string AutoName()
        {
            return ActionHelpers.AutoName("ReadButtonValue", button, storeFloatValue);
        }
#endif
    }
}


#endif
