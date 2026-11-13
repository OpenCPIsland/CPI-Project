using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin
{
	internal class ScreenSwipeInput : MonoBehaviour
	{
		public float TouchSensitivity = 2f;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void LateUpdate()
		{
			// Use Unity Input System for single touch detection
			if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
			{
				var touch = Touchscreen.current.touches[0];
				if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
				{
					Vector2 position = touch.position.ReadValue();
					Vector2 deltaPosition = touch.delta.ReadValue();
					if (Camera.main.pixelRect.Contains(position))
					{
						float delta = deltaPosition.x * TouchSensitivity / (float)Screen.width;
						dispatcher.DispatchEvent(new InputEvents.SwipeEvent(delta));
					}
				}
			}
		}
	}
}