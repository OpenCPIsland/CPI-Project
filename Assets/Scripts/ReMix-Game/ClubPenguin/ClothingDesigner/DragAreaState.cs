using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public abstract class DragAreaState
	{
		public struct ITouch
		{
			public TouchPhase phase;

			public Vector2 position;

			public Vector2 deltaPosition;

			public int tapCount;

			private static Vector3 lastPosition;

			private static float lastClick;

			public static ITouch fromTouch(Touch touch)
			{
				ITouch result = default(ITouch);
				result.phase = touch.phase;
				result.position = touch.position;
				result.deltaPosition = touch.deltaPosition;
				result.tapCount = touch.tapCount;
				return result;
			}

			public static ITouch fromMouse()
			{
				ITouch result = default(ITouch);
				result.tapCount = 0;
				if (UnityEngine.Input.GetMouseButtonDown(0))
				{
					result.phase = TouchPhase.Began;
					lastClick = Time.time;
				}
				else if (UnityEngine.Input.GetMouseButtonUp(0))
				{
					result.phase = TouchPhase.Ended;
					if (lastClick - Time.time < 0.1f)
					{
						result.tapCount = 1;
					}
				}
				else if (UnityEngine.Input.GetMouseButton(0))
				{
					if (lastPosition != UnityEngine.Input.mousePosition)
					{
						result.phase = TouchPhase.Moved;
					}
					else
					{
						result.phase = TouchPhase.Stationary;
					}
				}
				else
				{
					result.phase = TouchPhase.Canceled;
				}
				result.position = UnityEngine.Input.mousePosition;
				result.deltaPosition = UnityEngine.Input.mousePosition - lastPosition;
				lastPosition = UnityEngine.Input.mousePosition;
				return result;
			}
		}

		public const float MAX_DRAG_X = 5f;

		public const float MAX_DRAG_Y = 20f;

		public const float MIN_DRAG_Y = 3f;

		public float DragDeltaDampenX = 3f;

		public abstract void EnterState(CustomizerGestureModel currentGesture);

		public virtual void UpdateState()
		{
			if (UnityEngine.Input.touchCount == 1 && PlatformUtils.GetPlatformType() != PlatformType.Standalone)
			{
				ProcessOneTouch(ITouch.fromTouch(UnityEngine.Input.GetTouch(0)));
				return;
			}
			if (UnityEngine.Input.touchCount == 2)
			{
				ProcessTwoTouchPinchAndZoom();
				return;
			}
			ITouch touch = ITouch.fromMouse();
			if (touch.phase != TouchPhase.Canceled)
			{
				ProcessOneTouch(touch);
			}
		}

		public abstract void ExitState();

		protected virtual void ProcessOneTouch(ITouch touch)
		{
		}

		protected void ProcessTwoTouchPinchAndZoom()
		{
		}

		protected bool checkButtonDrag(Vector2 dragDelta)
		{
			float num = (PlatformUtils.GetPlatformType() == PlatformType.Standalone) ? (dragDelta.x / DragDeltaDampenX) : dragDelta.x;
			return dragDelta.y > 20f || (dragDelta.y > 3f && dragDelta.y >= num && num > -5f && num < 5f);
		}
	}
}
