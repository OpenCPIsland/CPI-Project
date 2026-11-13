#if UNITY_ANDROID || UNITY_IOS
using Disney.MobileNetwork;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Disney.Kelowna.Common
{
    public class InputWrapper : MonoBehaviour
    {
        private TouchEquivalent? fakeTouch;
        private bool? fakeLeftMouseButtonDown;
        private bool fakeMouseButtonChangedStateThisFrame;
        private Vector3? fakeMousePosition;
        private float[] touchPrevTimes;
        private TouchEquivalent[] lastTouchStates;

        private static InputWrapper instance
        {
            get
            {
                var inst = Service.Get<InputWrapper>();
                if (inst == null)
                {
                    Debug.LogError("InputWrapper: Service.Get<InputWrapper> returned null. Ensure InputWrapper is registered.");
                }
                return inst;
            }
        }

        private void Awake()
        {
            TouchEquivalent.Initialize();
            touchPrevTimes = new float[10];
            lastTouchStates = new TouchEquivalent[10];
            EnhancedTouch.Touch.onFingerUp += HandleFingerUp;
            Debug.Log("InputWrapper: Initialized with EnhancedTouchSupport");
        }

        private void OnDestroy()
        {
            EnhancedTouch.Touch.onFingerUp -= HandleFingerUp;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < lastTouchStates.Length; i++)
            {
                if (lastTouchStates[i].Phase == UnityEngine.TouchPhase.Ended)
                {
                    lastTouchStates[i] = default;
                }
            }
        }

        private void HandleFingerUp(EnhancedTouch.Finger finger)
        {
            int index = finger.index % 10;
            var currentTouch = finger.currentTouch;
            if (currentTouch.finger != null)
            {
                float prevTime = touchPrevTimes[index];
                lastTouchStates[index] = TouchEquivalent.FromEnhancedTouch(currentTouch, prevTime);
                Debug.Log($"InputWrapper.HandleFingerUp: fingerId={finger.index}, phase={lastTouchStates[index].Phase}");
            }
        }

        public static int touchCount
        {
            get
            {
                if (instance == null) return 0;
                if (instance.fakeTouch.HasValue) return 1;
                int activeCount = EnhancedTouch.EnhancedTouchSupport.enabled ? EnhancedTouch.Touch.activeTouches.Count : 0;
                int endedCount = instance.lastTouchStates.Any(state => state.Phase == UnityEngine.TouchPhase.Ended && state.DeltaTime < Time.deltaTime * 2) ? 1 : 0;
                return Mathf.Max(activeCount, endedCount);
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                if (instance == null) return Vector3.zero;
                if (instance.fakeMousePosition.HasValue)
                    return instance.fakeMousePosition.Value;
                return Mouse.current != null ? (Vector3)Mouse.current.position.ReadValue() : Vector3.zero;
            }
        }

        public static TouchEquivalent GetTouch(int index)
        {
            if (instance == null)
            {
                Debug.LogError("InputWrapper.GetTouch: instance is null");
                return default;
            }

            if (instance.fakeTouch.HasValue)
            {
                Debug.Log("InputWrapper.GetTouch: Returning fake touch");
                return instance.fakeTouch.Value;
            }

            if (!EnhancedTouch.EnhancedTouchSupport.enabled)
            {
                Debug.LogError("InputWrapper.GetTouch: EnhancedTouch is not enabled");
                return default;
            }

            if (index == 0 && instance.lastTouchStates.Any(state => state.Phase == UnityEngine.TouchPhase.Ended))
            {
                TouchEquivalent endedTouch = default;
                int endedIndex = -1;
                for (int i = 0; i < instance.lastTouchStates.Length; i++)
                {
                    if (instance.lastTouchStates[i].Phase == UnityEngine.TouchPhase.Ended)
                    {
                        endedTouch = instance.lastTouchStates[i];
                        endedIndex = i;
                        break;
                    }
                }
                if (endedIndex >= 0)
                {
                    instance.lastTouchStates[endedIndex] = default;
                    Debug.Log($"InputWrapper.GetTouch: Returning ended touch, fingerId={endedTouch.FingerId}, phase={endedTouch.Phase}");
                    return endedTouch;
                }
            }

            if (index < 0 || index >= EnhancedTouch.Touch.activeTouches.Count)
            {
                Debug.LogWarning($"InputWrapper.GetTouch: Invalid index {index}, activeCount={EnhancedTouch.Touch.activeTouches.Count}");
                return default;
            }

            var activeTouch = EnhancedTouch.Touch.activeTouches[index];
            float prevTime = instance.touchPrevTimes[activeTouch.finger.index % 10];
            instance.touchPrevTimes[activeTouch.finger.index % 10] = (float)activeTouch.time;
            var touchEq = TouchEquivalent.FromEnhancedTouch(activeTouch, prevTime);
            Debug.Log($"InputWrapper.GetTouch: index={index}, fingerId={touchEq.FingerId}, phase={touchEq.Phase}, position={touchEq.Position}");
            return touchEq;
        }

        public static void SetTouch(int index, TouchEquivalent? touch)
        {
            if (instance == null)
            {
                Debug.LogError("InputWrapper.SetTouch: instance is null");
                return;
            }
            instance.fakeTouch = touch;
            Debug.Log($"InputWrapper.SetTouch: index={index}, touch={(touch.HasValue ? touch.Value.Phase.ToString() : "null")}");
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (instance == null) return false;
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;

            if (Mouse.current == null) return false;
            bool result = button switch
            {
                0 => Mouse.current.leftButton.wasPressedThisFrame,
                1 => Mouse.current.rightButton.wasPressedThisFrame,
                2 => Mouse.current.middleButton.wasPressedThisFrame,
                _ => false
            };
            Debug.Log($"InputWrapper.GetMouseButtonDown: button={button}, result={result}");
            return result;
        }

        public static bool GetMouseButtonUp(int button)
        {
            if (instance == null) return false;
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return !instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;

            if (Mouse.current == null) return false;
            bool result = button switch
            {
                0 => Mouse.current.leftButton.wasReleasedThisFrame,
                1 => Mouse.current.rightButton.wasReleasedThisFrame,
                2 => Mouse.current.middleButton.wasReleasedThisFrame,
                _ => false
            };
            Debug.Log($"InputWrapper.GetMouseButtonUp: button={button}, result={result}");
            return result;
        }

        public static bool GetMouseButton(int index)
        {
            if (instance == null) return false;
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return instance.fakeLeftMouseButtonDown.Value;

            if (Mouse.current == null) return false;
            bool result = index switch
            {
                0 => Mouse.current.leftButton.isPressed,
                1 => Mouse.current.rightButton.isPressed,
                2 => Mouse.current.middleButton.isPressed,
                _ => false
            };
            Debug.Log($"InputWrapper.GetMouseButton: index={index}, result={result}");
            return result;
        }

        public static void SetMouseButton(int index, bool? isPressed, Vector3? position)
        {
            if (instance == null)
            {
                Debug.LogError("InputWrapper.SetMouseButton: instance is null");
                return;
            }
            if (isPressed.HasValue && isPressed != instance.fakeLeftMouseButtonDown)
                CoroutineRunner.StartPersistent(fakeMouseButtonStateChange(), instance, "mouseButtonChange");
            instance.fakeLeftMouseButtonDown = isPressed;
            instance.fakeMousePosition = position;
            Debug.Log($"InputWrapper.SetMouseButton: index={index}, isPressed={isPressed}, position={position}");
        }

        private static IEnumerator fakeMouseButtonStateChange()
        {
            if (instance == null) yield break;
            instance.fakeMouseButtonChangedStateThisFrame = true;
            yield return null;
            instance.fakeMouseButtonChangedStateThisFrame = false;
        }
    }
}
#else
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Disney.Kelowna.Common
{
    public class InputWrapper : MonoBehaviour
    {
        private TouchEquivalent? fakeTouch;
        private bool? fakeLeftMouseButtonDown;
        private bool fakeMouseButtonChangedStateThisFrame;
        private Vector3? fakeMousePosition;

        private static InputWrapper instance
        {
            get { return Service.Get<InputWrapper>(); }
        }

        public static int touchCount
        {
            get
            {
                if (instance.fakeTouch.HasValue) return 1;
                
                return EnhancedTouch.EnhancedTouchSupport.enabled ? EnhancedTouch.Touch.activeTouches.Count : 0;
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                if (instance.fakeMousePosition.HasValue)
                    return instance.fakeMousePosition.Value;

                
                return Mouse.current != null ? (Vector3)Mouse.current.position.ReadValue() : Vector3.zero;
            }
        }

        public static TouchEquivalent GetTouch(int index)
        {
            if (instance.fakeTouch.HasValue)
                return instance.fakeTouch.Value;

            if (!EnhancedTouch.EnhancedTouchSupport.enabled)
                throw new System.InvalidOperationException("EnhancedTouch is not enabled!");

            if (index < 0 || index >= EnhancedTouch.Touch.activeTouches.Count)
                throw new System.IndexOutOfRangeException("No touch at this index.");

            var touches = EnhancedTouch.Touch.activeTouches;
            var touch = touches[index];

            
            return TouchEquivalent.FromEnhancedTouch(touch, 0);
        }

        public static void SetTouch(int index, TouchEquivalent? touch)
        {
            instance.fakeTouch = touch;
        }

        public static bool GetMouseButtonDown(int button)
        {
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;

            if (Mouse.current == null) return false;
            switch (button)
            {
                case 0: return Mouse.current.leftButton.wasPressedThisFrame;
                case 1: return Mouse.current.rightButton.wasPressedThisFrame;
                case 2: return Mouse.current.middleButton.wasPressedThisFrame;
                default: return false;
            }
        }

        public static bool GetMouseButtonUp(int button)
        {
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return !instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;

            if (Mouse.current == null) return false;
            switch (button)
            {
                case 0: return Mouse.current.leftButton.wasReleasedThisFrame;
                case 1: return Mouse.current.rightButton.wasReleasedThisFrame;
                case 2: return Mouse.current.middleButton.wasReleasedThisFrame;
                default: return false;
            }
        }

        public static bool GetMouseButton(int index)
        {
            if (instance.fakeLeftMouseButtonDown.HasValue)
                return instance.fakeLeftMouseButtonDown.Value;

            if (Mouse.current == null) return false;
            switch (index)
            {
                case 0: return Mouse.current.leftButton.isPressed;
                case 1: return Mouse.current.rightButton.isPressed;
                case 2: return Mouse.current.middleButton.isPressed;
                default: return false;
            }
        }

        public static void SetMouseButton(int index, bool? isPressed, Vector3? position)
        {
            if (isPressed.HasValue && isPressed != instance.fakeLeftMouseButtonDown)
                CoroutineRunner.StartPersistent(fakeMouseButtonStateChange(), instance, "mouseButtonChange");
            instance.fakeLeftMouseButtonDown = isPressed;
            instance.fakeMousePosition = position;
        }

        private static IEnumerator fakeMouseButtonStateChange()
        {
            instance.fakeMouseButtonChangedStateThisFrame = true;
            yield return null;
            instance.fakeMouseButtonChangedStateThisFrame = false;
        }
    }
}
#endif
