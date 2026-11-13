// (c) Copyright HutongGames, LLC 2021. All rights reserved.

#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("PlayerInput")]
    [Note("Make sure the Button is setup with the Press and Release Interaction to trigger Pressed and Released events.")]
	[Tooltip("Sends Events based InputAction buttons in a PlayerInput component.")]
	public class PlayerInputButtonEvents: PlayerInputUpdateActionBase
	{
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
            isPressed = null;
            isPressedEvent = null;
            wasPressedThisFrame = null;
            wasReleasedThisFrame = null;
        }

        protected override void Execute()
        {
            /* buttonControl is null when we need to query wasReleasedThisFrame!
            var buttonControl = m_inputAction.activeControl as ButtonControl;
            if (buttonControl == null)
            {
                isPressed.Value = false;
                return;
            }

            // Not sure if this is the correct way to read button values from input actions:
            // https://forum.unity.com/threads/getbuttondown-getbuttonup-with-the-new-system.876451/#post-5764510
            // Seems better to get the ButtonControl and use IsPressed etc. but can't figure out how!

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
            */

            if (action == null) return;

            // Would be better to use button pressed threshold here:
            var pressed = action.ReadValue<float>() > 0;

            isPressed.Value = pressed;

            if (isPressedEvent != null && pressed)
            {
                Fsm.Event(isPressedEvent);
            }

            if (wasPressedThisFrame != null && action.triggered && pressed)
            {
                Fsm.Event(wasPressedThisFrame);
            }

            if (wasReleasedThisFrame != null && action.triggered && !pressed)
            {
                Fsm.Event(wasReleasedThisFrame);
            }
        }

        //ErrorCheck if interaction is setup properly for press and release.
    }
}

#endif