using ClubPenguin.Core;

namespace ClubPenguin.UI
{
	public class JoystickDisabler : UIElementDisabler
	{
		private VirtualJoystick joystick;

		private void Awake()
		{
			joystick = GetComponentInChildren<VirtualJoystick>();
		}

		public override void DisableElement(bool hide)
		{
			joystick.DisableJoystick();
			changeVisibility(!hide);
			isEnabled = false;
		}

		public override void EnableElement()
		{
			changeVisibility(true);
			joystick.EnableJoystick();
			isEnabled = true;
		}
	}
}
