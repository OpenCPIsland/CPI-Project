using UnityEngine;

namespace SwrveUnity.Input
{
	public class NativeInputManager : IInputManager
	{
		private static NativeInputManager instance;

		public static NativeInputManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new NativeInputManager();
				}
				return instance;
			}
		}

		private NativeInputManager()
		{
		}

		bool IInputManager.GetMouseButtonUp(int buttonId)
		{
			return !UnityEngine.Input.GetMouseButton(buttonId);
		}

		bool IInputManager.GetMouseButtonDown(int buttonId)
		{
			return UnityEngine.Input.GetMouseButton(buttonId);
		}

		Vector3 IInputManager.GetMousePosition()
		{
			Vector3 mousePosition = UnityEngine.Input.mousePosition;
			mousePosition.y = (float)Screen.height - mousePosition.y;
			return mousePosition;
		}
	}
}
