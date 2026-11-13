using UnityEngine;

namespace SwrveUnity.Input
{
	public interface IInputManager
	{
		bool GetMouseButtonUp(int buttonId);

		bool GetMouseButtonDown(int buttonId);

		Vector3 GetMousePosition();
	}
}
