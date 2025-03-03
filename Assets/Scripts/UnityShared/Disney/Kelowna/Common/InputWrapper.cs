using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

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
			get
			{
				return Service.Get<InputWrapper>();
			}
		}

		public static int touchCount
		{
			get
			{
				return instance.fakeTouch.HasValue ? 1 : Input.touchCount;
			}
		}

		public static Vector3 mousePosition
		{
			get
			{
				return instance.fakeMousePosition.HasValue ? instance.fakeMousePosition.Value : Input.mousePosition;
			}
		}

		public static TouchEquivalent GetTouch(int index)
		{
			return instance.fakeTouch.HasValue ? instance.fakeTouch.Value : TouchEquivalent.FromTouch(Input.GetTouch(index));
		}

		public static void SetTouch(int index, TouchEquivalent? touch)
		{
			instance.fakeTouch = touch;
		}

		public static bool GetMouseButtonDown(int button)
		{
			if (instance.fakeLeftMouseButtonDown.HasValue)
			{
				return instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;
			}
			return Input.GetMouseButtonDown(button);
		}

		public static bool GetMouseButtonUp(int button)
		{
			if (instance.fakeLeftMouseButtonDown.HasValue)
			{
				return !instance.fakeLeftMouseButtonDown.Value && instance.fakeMouseButtonChangedStateThisFrame;
			}
			return Input.GetMouseButtonUp(button);
		}

		public static bool GetMouseButton(int index)
		{
			return instance.fakeLeftMouseButtonDown.HasValue ? instance.fakeLeftMouseButtonDown.Value : Input.GetMouseButton(index);
		}

		public static void SetMouseButton(int index, bool? isPressed, Vector3? position)
		{
			if (isPressed.HasValue && isPressed != instance.fakeLeftMouseButtonDown)
			{
				CoroutineRunner.StartPersistent(fakeMouseButtonStateChange(), instance, "mouseButtonChange");
			}
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
