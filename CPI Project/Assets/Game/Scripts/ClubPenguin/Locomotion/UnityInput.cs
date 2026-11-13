using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.Locomotion
{
    public class UnityInput : MonoBehaviour
    {
        private EventDispatcher dispatcher;

        private Vector2 prevAxis;

        private bool isWalking;

        public void Awake()
        {
            dispatcher = Service.Get<EventDispatcher>();
        }

        public void Update()
        {
            // Replace legacy Input axes with new Input System equivalents
            Vector2 lhs = Vector2.zero;

            // Horizontal/Vertical axes (WASD/Arrow keys or gamepad left stick)
            if (Keyboard.current != null)
            {
                float x = 0f, y = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
                lhs = new Vector2(x, y);
            }
            // Gamepad left stick
            if (Gamepad.current != null)
            {
                var stick = Gamepad.current.leftStick.ReadValue();
                if (stick.magnitude > lhs.magnitude) // Prefer gamepad if more input
                    lhs = stick;
            }

            if (lhs != prevAxis)
            {
                dispatcher.DispatchEvent(new InputEvents.MoveEvent(lhs.normalized));
                prevAxis = lhs;
            }

            // Jump (Space or gamepad A)
            bool jumpPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                               (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

            if (jumpPressed)
            {
                dispatcher.DispatchEvent(new InputEvents.ActionEvent(InputEvents.Actions.Jump));
            }

            // "Fire3" (default: Left Shift or gamepad X)
            bool fire3Pressed = (Keyboard.current != null && Keyboard.current.leftShiftKey.wasPressedThisFrame) ||
                                (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);

            if (fire3Pressed)
            {
                isWalking = !isWalking;
                dispatcher.DispatchEvent(new InputEvents.SwitchChangeEvent(InputEvents.Switches.Tube, isWalking));
            }
        }
    }
}