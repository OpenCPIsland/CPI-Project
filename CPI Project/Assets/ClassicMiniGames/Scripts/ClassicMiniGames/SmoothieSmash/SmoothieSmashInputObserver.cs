#if UNITY_ANDROID || UNITY_IOS
using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using MinigameFramework;

namespace SmoothieSmash
{
    public class SmoothieSmashInputObserver : MonoBehaviour
    {
        public Vector2 CurrentSteering;

        public event Action<Vector2, Vector2> SteeringChangedEvent;

        private const float TouchSteeringMultiplier = 1.5f; 

        private Vector2? swipeStartPosition = null;
        private bool swipeDownTriggered = false;

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
            Vector2 vector = Vector2.zero;


            if (Touch.activeTouches.Count > 0)
            {
                var touch = Touch.activeTouches[0];
                Vector2 touchPosition = touch.screenPosition;

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    swipeStartPosition = touchPosition;
                    swipeDownTriggered = false;
                }
                else if (swipeStartPosition.HasValue && !swipeDownTriggered && touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    float swipeDistance = touchPosition.y - swipeStartPosition.Value.y;
                    float requiredDistance = Screen.height * 0.5f;
                    if (swipeDistance <= -requiredDistance) 
                    {
                        swipeDownTriggered = true;
                        OnSwipeDown();
                    }
                }
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    swipeStartPosition = null;
                    swipeDownTriggered = false;
                }

                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                vector = (touchPosition - screenCenter) / (Mathf.Min(Screen.width, Screen.height) / 2f);
                vector = Vector2.ClampMagnitude(vector * TouchSteeringMultiplier, 1f);
            }
            else if (Gamepad.current != null)
            {
                vector = Gamepad.current.leftStick.ReadValue();
            }
            else if (Keyboard.current != null)
            {
                float x = 0f, y = 0f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    x -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    x += 1f;
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    y -= 1f;
                vector = new Vector2(x, y);
                vector = vector.normalized;
            }

            if (vector != CurrentSteering && this.SteeringChangedEvent != null)
            {
                this.SteeringChangedEvent(CurrentSteering, vector);
            }
            CurrentSteering = vector;
        }

        private void OnSwipeDown()
        {
            Debug.Log("Swipe Down Detected!");
            var minigame = MinigameManager.GetActive<mg_SmoothieSmash>();
            if (minigame != null && minigame.GameLogic != null)
            {
                var playerLogicField = typeof(mg_ss_GameLogic).GetField("m_player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var playerLogic = playerLogicField?.GetValue(minigame.GameLogic) as mg_ss_PlayerLogic;
                playerLogic?.StartSmashing();
            }
        }

        private void OnDestroy()
        {
            this.SteeringChangedEvent = null;
        }
    }
}
#else
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SmoothieSmash
{
	public class SmoothieSmashInputObserver : MonoBehaviour
	{
		public Vector2 CurrentSteering;

		public event Action<Vector2, Vector2> SteeringChangedEvent;

		private const float StickDeadZoneSqr = 0.04f;

		private void Update()
		{
			Vector2 keyboardVec = Vector2.zero;
			Vector2 gamepadVec = Vector2.zero;

			Keyboard kb = Keyboard.current;
			if (kb != null)
			{
				float x = 0f;
				float y = 0f;

				if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
				{
					x -= 1f;
				}
				if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
				{
					x += 1f;
				}
				if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
				{
					y += 1f;
				}
				if (kb.sKey.isPressed || kb.downArrowKey.isPressed)
				{
					y -= 1f;
				}

				if (x != 0f || y != 0f)
				{
					keyboardVec = new Vector2(x, y).normalized;
				}
			}

			Gamepad gp = Gamepad.current;
			if (gp != null)
			{
				gamepadVec = gp.leftStick.ReadValue();
				if (gamepadVec.sqrMagnitude < StickDeadZoneSqr)
				{
					gamepadVec = Vector2.zero;
				}
			}

			
			Vector2 vector = (keyboardVec != Vector2.zero) ? keyboardVec : gamepadVec;

			if (vector != CurrentSteering && SteeringChangedEvent != null)
			{
				SteeringChangedEvent(CurrentSteering, vector);
			}
			CurrentSteering = vector;
		}

		private void OnDestroy()
		{
			SteeringChangedEvent = null;
		}
	}
}
#endif
