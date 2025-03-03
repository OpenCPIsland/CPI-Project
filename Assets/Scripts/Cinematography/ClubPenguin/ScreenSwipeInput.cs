using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

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
			if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Vector2 position = Input.GetTouch(0).position;
				Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
				if (Camera.main.pixelRect.Contains(position))
				{
					float delta = deltaPosition.x * TouchSensitivity / (float)Screen.width;
					dispatcher.DispatchEvent(new InputEvents.SwipeEvent(delta));
				}
			}
		}
	}
}
