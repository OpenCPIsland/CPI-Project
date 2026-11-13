using UnityEngine;

public class C_EasyButtonTemplate : MonoBehaviour
{
	private void OnEnable()
	{
		EasyButton.On_ButtonDown += On_ButtonDown;
		EasyButton.On_ButtonPress += On_ButtonPress;
		EasyButton.On_ButtonUp += On_ButtonUp;
	}

	private void OnDisable()
	{
		EasyButton.On_ButtonDown -= On_ButtonDown;
		EasyButton.On_ButtonPress -= On_ButtonPress;
		EasyButton.On_ButtonUp -= On_ButtonUp;
	}

	private void OnDestroy()
	{
		EasyButton.On_ButtonDown -= On_ButtonDown;
		EasyButton.On_ButtonPress -= On_ButtonPress;
		EasyButton.On_ButtonUp -= On_ButtonUp;
	}

	private void On_ButtonDown(string buttonName)
	{
	}

	private void On_ButtonPress(string buttonName)
	{
	}

	private void On_ButtonUp(string buttonName)
	{
	}
}
