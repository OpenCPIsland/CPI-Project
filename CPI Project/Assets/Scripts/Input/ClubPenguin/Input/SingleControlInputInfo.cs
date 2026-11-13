using System;

namespace ClubPenguin.Input
{
	public class SingleControlInputInfo : InputInfo
	{
		public enum Actions
		{
			Jump,
			Action1,
			Action2,
			Action3,
			Cancel,
			Profile
		}

		public Actions ControlAction;

		public string PrimaryKey = string.Empty;

		public override void Populate(ControlScheme controlScheme)
		{
			if (ActiveInputDevice.CurrentKind == ActiveInputDevice.Kind.Gamepad)
			{
				switch (ControlAction)
				{
					case Actions.Jump:
						PrimaryKey = ActiveInputDevice.GetLabel(ActiveInputDevice.GamepadControl.ButtonSouth);
						return;
					case Actions.Action1:
						PrimaryKey = ActiveInputDevice.GetLabel(ActiveInputDevice.GamepadControl.ButtonWest);
						return;
					case Actions.Action2:
						PrimaryKey = ActiveInputDevice.GetLabel(ActiveInputDevice.GamepadControl.ButtonNorth);
						return;
					case Actions.Action3:
						PrimaryKey = ActiveInputDevice.GetLabel(ActiveInputDevice.GamepadControl.ButtonEast);
						return;
					case Actions.Cancel:
						PrimaryKey = ActiveInputDevice.GetLabel(ActiveInputDevice.GamepadControl.Select);
						return;
					case Actions.Profile:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			switch (ControlAction)
			{
				case Actions.Jump:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Jump.PrimaryKey);
					break;
				case Actions.Action1:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Action1.PrimaryKey);
					break;
				case Actions.Action2:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Action2.PrimaryKey);
					break;
				case Actions.Action3:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Action3.PrimaryKey);
					break;
				case Actions.Cancel:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Cancel.PrimaryKey);
					break;
				case Actions.Profile:
					PrimaryKey = getKeyCodeTranslation(controlScheme.Profile.PrimaryKey);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
