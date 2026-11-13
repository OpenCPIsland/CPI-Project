using ClubPenguin.Cinematography;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin.UI
{
    [RequireComponent(typeof(AvatarRenderTextureComponent))]
    public class AvatarRenderTextureRotator : MonoBehaviour
    {
        public RectTransform TouchArea;

        public float RotationSpeed = 15f;

        private AvatarRenderTextureComponent renderTextureComponent;

        private float previousTouchX;

        private bool isRotating;

        private void Awake()
        {
            renderTextureComponent = GetComponent<AvatarRenderTextureComponent>();
        }

        private void Start()
        {
            Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.DisableElasticGlancer));
        }

        private void Update()
        {
            // Only proceed if we have a mouse (desktop) or touch (mobile)
            Vector2 pointerPosition = Vector2.zero;
            bool pointerDown = false, pointerUp = false, pointerHeld = false;

            // Mouse events
            if (Mouse.current != null)
            {
                pointerPosition = Mouse.current.position.ReadValue();
                pointerDown = Mouse.current.leftButton.wasPressedThisFrame;
                pointerUp = Mouse.current.leftButton.wasReleasedThisFrame;
                pointerHeld = Mouse.current.leftButton.isPressed;
            }
            // Touch events (single touch only)
            else if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                var touch = Touchscreen.current.touches[0];
                pointerPosition = touch.position.ReadValue();
                pointerDown = touch.press.wasPressedThisFrame;
                pointerUp = touch.press.wasReleasedThisFrame;
                pointerHeld = touch.press.isPressed;
            }

            if (pointerDown)
            {
                if (TouchArea == null || RectTransformUtility.RectangleContainsScreenPoint(TouchArea, pointerPosition))
                {
                    isRotating = true;
                    previousTouchX = pointerPosition.x;
                }
            }
            else if (pointerUp)
            {
                isRotating = false;
            }
            else if (pointerHeld && isRotating)
            {
                float num = pointerPosition.x - previousTouchX;
                renderTextureComponent.RotateModel((0f - num) * RotationSpeed);
                previousTouchX = pointerPosition.x;
            }
        }

        private void OnDestroy()
        {
            Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.EnableElasticGlancer));
        }
    }
}