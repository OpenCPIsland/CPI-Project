using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ClubPenguin
{
    [DisallowMultipleComponent]
    internal class PinchZoomInput : MonoBehaviour
    {
        public float MouseSensitivity = 1f;

        public float TouchSensitivity = 2f;

        public float PreviousZoom;

        private EventDispatcher dispatcher;

        private void Start()
        {
            dispatcher = Service.Get<EventDispatcher>();
        }

        private void Update()
        {
            float num = PreviousZoom;
            Touchscreen touchscreen = Touchscreen.current;

            if (touchscreen != null)
            {
                TouchControl touch = null;
                TouchControl touch2 = null;
                int touchCount = 0;

                for (int i = 0; i < touchscreen.touches.Count; i++)
                {
                    TouchControl currentTouch = touchscreen.touches[i];

                    if (currentTouch.press.isPressed)
                    {
                        if (touchCount == 0)
                        {
                            touch = currentTouch;
                        }
                        else if (touchCount == 1)
                        {
                            touch2 = currentTouch;
                        }

                        touchCount++;
                    }
                }

                if (touchCount == 2)
                {
                    if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved &&
                        touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        Vector2 position = touch.position.ReadValue();
                        Vector2 deltaPosition = touch.delta.ReadValue();
                        Vector2 position2 = touch2.position.ReadValue();
                        Vector2 deltaPosition2 = touch2.delta.ReadValue();

                        Rect pixelRect = Camera.main.pixelRect;

                        if (pixelRect.Contains(position) && pixelRect.Contains(position2))
                        {
                            float magnitude = (position - position2).magnitude;
                            float magnitude2 = (position - deltaPosition - (position2 - deltaPosition2)).magnitude;
                            float num2 = magnitude - magnitude2;

                            num -= num2 * TouchSensitivity / (float)Screen.width;
                        }
                    }
                }
            }
            else
            {
                Mouse mouse = Mouse.current;

                if (mouse != null)
                {
                    float scroll = mouse.scroll.ReadValue().y;

                    if (scroll != 0f)
                    {
                        num += Mathf.Sign(scroll) * 0.1f * MouseSensitivity;
                    }
                }
            }

            num = Mathf.Clamp(num, 0f, 1f);

            if (dispatcher != null && num != PreviousZoom)
            {
                dispatcher.DispatchEvent(new InputEvents.ZoomEvent(num));
                PreviousZoom = num;
            }
        }
    }
}