using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace ClubPenguin.UI
{
    public class ScreenPenguinRotation : MonoBehaviour
    {
        private Transform rotationTransform;
        private float rotationMultiplier = 0.5f;
        private Vector2 previousTouchPosition = Vector2.zero;
        private Quaternion initialRotation;
        private float maxTouchPositionY;
        private float minTouchPositionY;
        private float worldContainerHeight;
        private RectTransform worldContainerTransform;
        private Canvas canvas;

        public void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
            Transform transform = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).transform;
            worldContainerTransform = (transform.Find("WorldContainer") as RectTransform);
            if (worldContainerTransform == null)
            {
                Transform transform2 = transform.Find("VertLayout");
                if (transform2 != null)
                {
                    worldContainerTransform = (transform2.Find("WorldContainer") as RectTransform);
                }
            }
            if (worldContainerTransform == null)
            {
                Log.LogError(this, "Could not find the world container rect transform");
            }
            if (localPlayerGameObject != null)
            {
                rotationTransform = localPlayerGameObject.GetComponent<Transform>();
                initialRotation = rotationTransform.rotation;
            }
            EnhancedTouchSupport.Enable(); // Enable EnhancedTouch for InputSystem
        }

        private void Update()
        {
            checkInput();
        }

        public void OnDestroy()
        {
            if (rotationTransform != null)
            {
                rotationTransform.rotation = initialRotation;
            }
            EnhancedTouchSupport.Disable();
        }

        private void checkInput()
        {
            Vector2 lhs = Vector2.zero;
            int touchCount = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
            if (touchCount > 0)
            {
                var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began && isInputWithinRect(touch.screenPosition))
                {
                    previousTouchPosition = touch.screenPosition;
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved && isInputWithinRect(touch.screenPosition))
                {
                    lhs = touch.screenPosition;
                }
            }
            // Also support mouse drag on desktop
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (previousTouchPosition == Vector2.zero && isInputWithinRect(mousePos))
                {
                    previousTouchPosition = mousePos;
                }
                else if (isInputWithinRect(mousePos))
                {
                    lhs = mousePos;
                }
            }

            if (lhs != Vector2.zero)
            {
                float num = lhs.x - previousTouchPosition.x;
                float num2 = rotationMultiplier * num;
                if (rotationTransform != null)
                {
                    rotationTransform.Rotate(new Vector3(0f, num2 * -1f, 0f));
                }
                previousTouchPosition = lhs;
            }

            // Reset previousTouchPosition when input ends
            if ((UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 0 && Mouse.current != null && !Mouse.current.leftButton.isPressed))
            {
                previousTouchPosition = Vector2.zero;
            }
        }

        private bool isInputWithinRect(Vector2 inputPosition)
        {
            if (worldContainerTransform != null && Math.Abs(worldContainerHeight - worldContainerTransform.rect.height) > float.Epsilon)
            {
                worldContainerHeight = worldContainerTransform.rect.height;
                float num = (1f - worldContainerTransform.pivot.y) * worldContainerHeight + worldContainerTransform.anchoredPosition.y;
                maxTouchPositionY = canvas.pixelRect.height + num;
                minTouchPositionY = maxTouchPositionY - worldContainerHeight;
            }
            return inputPosition.y > minTouchPositionY && inputPosition.y < maxTouchPositionY;
        }
    }
}