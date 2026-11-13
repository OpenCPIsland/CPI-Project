using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Disney.Kelowna.Common
{
    public class TouchMonitor : MonoBehaviour
    {
        [Tooltip("The magnitude of movement that constitutes a swipe gesture")]
        [Range(1f, 100f)]
        public float SwipeMagnitude = 10f;

        public bool EnableSampling = true;

        private bool isTouching = false;

        public Vector2 DeltaPosition { get; private set; }

        public float Magnitude => DeltaPosition.magnitude;

        public bool IsSwiping => Magnitude > SwipeMagnitude;

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            if (!EnableSampling)
                return;

            // Use EnhancedTouch API for improved touch support
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            if (activeTouches.Count > 0)
            {
                var touch = activeTouches[0];
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    DeltaPosition = touch.delta;
                    isTouching = true;
                    return;
                }
            }

            if (isTouching)
            {
                DeltaPosition = Vector2.zero;
                isTouching = false;
            }
        }
    }
}