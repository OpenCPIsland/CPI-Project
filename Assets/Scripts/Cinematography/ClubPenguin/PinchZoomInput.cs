using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

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
			if (Input.touchSupported)
			{
				if (Input.touchCount == 2)
				{
					Touch touch = Input.GetTouch(0);
					Touch touch2 = Input.GetTouch(1);
					if (touch.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
					{
						Vector2 position = touch.position;
						Vector2 deltaPosition = touch.deltaPosition;
						Vector2 position2 = touch2.position;
						Vector2 deltaPosition2 = touch2.deltaPosition;
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
				num -= Input.GetAxis("Mouse ScrollWheel") * MouseSensitivity;
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
