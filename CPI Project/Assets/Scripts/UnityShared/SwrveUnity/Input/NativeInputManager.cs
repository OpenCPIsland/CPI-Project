using UnityEngine;
using UnityEngine.InputSystem;

namespace SwrveUnity.Input
{
    public class NativeInputManager : IInputManager
    {
        private static NativeInputManager instance;

        public static NativeInputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NativeInputManager();
                }
                return instance;
            }
        }

        private NativeInputManager()
        {
        }

        bool IInputManager.GetMouseButtonUp(int buttonId)
        {
            // InputSystem: Mouse.current.leftButton, rightButton, middleButton, etc.
            return GetButtonState(buttonId, ButtonState.Up);
        }

        bool IInputManager.GetMouseButtonDown(int buttonId)
        {
            return GetButtonState(buttonId, ButtonState.Down);
        }

        Vector3 IInputManager.GetMousePosition()
        {
            if (Mouse.current != null)
            {
                Vector2 pos = Mouse.current.position.ReadValue();
                // If you need to flip Y, do it here. Otherwise, use as is.
                pos.y = (float)Screen.height - pos.y;
                return new Vector3(pos.x, pos.y, 0f);
            }
            return Vector3.zero;
        }

        // Helper enum to distinguish button states
        private enum ButtonState
        {
            Down,
            Up
        }

        // Helper method to map buttonId to InputSystem mouse button
        private bool GetButtonState(int buttonId, ButtonState state)
        {
            if (Mouse.current == null)
                return false;

            // 0 = Left, 1 = Right, 2 = Middle
            switch (buttonId)
            {
                case 0:
                    return state == ButtonState.Down ? Mouse.current.leftButton.wasPressedThisFrame
                        : Mouse.current.leftButton.wasReleasedThisFrame;
                case 1:
                    return state == ButtonState.Down ? Mouse.current.rightButton.wasPressedThisFrame
                        : Mouse.current.rightButton.wasReleasedThisFrame;
                case 2:
                    return state == ButtonState.Down ? Mouse.current.middleButton.wasPressedThisFrame
                        : Mouse.current.middleButton.wasReleasedThisFrame;
                default:
                    // Optionally support extra buttons if needed
                    return false;
            }
        }
    }
}