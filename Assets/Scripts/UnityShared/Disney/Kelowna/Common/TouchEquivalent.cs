using UnityEngine;

namespace Disney.Kelowna.Common
{
	public struct TouchEquivalent
	{
		public const int MOUSE_POINTER_ID = 909;

		public Vector2 deltaPosition
		{
			get;
			set;
		}

		public float deltaTime
		{
			get;
			set;
		}

		public int fingerId
		{
			get;
			set;
		}

		public TouchPhase phase
		{
			get;
			set;
		}

		public Vector2 position
		{
			get;
			set;
		}

		public Vector2 rawPosition
		{
			get;
			set;
		}

		public int tapCount
		{
			get;
			set;
		}

		public static TouchEquivalent FromTouch(Touch touch)
		{
			TouchEquivalent result = default(TouchEquivalent);
			result.fingerId = touch.fingerId;
			result.position = touch.position;
			result.deltaTime = touch.deltaTime;
			result.deltaPosition = touch.deltaPosition;
			result.phase = touch.phase;
			result.tapCount = touch.tapCount;
			return result;
		}

		public static TouchEquivalent FromMouseButton(int buttonIndex, Vector3 lastMousePosition)
		{
			TouchEquivalent result = default(TouchEquivalent);
			if (lastMousePosition == Vector3.zero)
			{
				result.deltaPosition = Vector2.zero;
			}
			else
			{
				result.deltaPosition = Input.mousePosition - lastMousePosition;
			}
			if (Input.GetMouseButton(buttonIndex) || Input.GetMouseButtonUp(buttonIndex))
			{
				result.fingerId = 909;
				result.position = Input.mousePosition;
				result.deltaTime = Time.deltaTime;
				if (Input.GetMouseButtonUp(buttonIndex))
				{
					result.phase = TouchPhase.Ended;
				}
				else
				{
					result.phase = ((!Input.GetMouseButtonDown(buttonIndex)) ? ((result.deltaPosition.sqrMagnitude > 1f) ? TouchPhase.Moved : TouchPhase.Stationary) : TouchPhase.Began);
				}
				result.tapCount = 1;
			}
			return result;
		}

		public static TouchEquivalent FromLeftMouseButton(Vector3 lastMousePosition)
		{
			return FromMouseButton(0, lastMousePosition);
		}
	}
}
