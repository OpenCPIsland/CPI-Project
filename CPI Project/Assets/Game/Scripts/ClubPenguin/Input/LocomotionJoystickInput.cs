using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class LocomotionJoystickInput : LocomotionInput
	{
		[SerializeField]
		private GameObject joystickPrefab = null;

		private VirtualJoystick joystick;

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			Service.Get<EventDispatcher>().AddListener<VirtualJoystickEvents.JoystickAdded>(onJoystickAdded);
			Service.Get<EventDispatcher>().DispatchEvent(new ControlsScreenEvents.SetDefaultLeftOption(joystickPrefab));
			base.Initialize(keyCodeRemapper);
		}

		private bool onJoystickAdded(VirtualJoystickEvents.JoystickAdded evt)
		{
			joystick = evt.Joystick;
			return false;
		}

		protected override bool process(int filter)
		{
			bool result = false;
			if (joystick != null)
			{
				inputEvent.Direction = joystick.ProcessInput();
				result = true;
			}
			else
			{
				inputEvent.Direction = Vector2.zero;
			}
			return result;
		}

		protected override void resetInput()
		{
			if (joystick != null)
			{
				joystick.ResetInput();
			}
			base.resetInput();
		}
	}
}
