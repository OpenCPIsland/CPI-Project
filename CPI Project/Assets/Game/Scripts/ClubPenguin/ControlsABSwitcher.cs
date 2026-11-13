using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ControlsABSwitcher : MonoBehaviour
	{
		public enum ControlSlots
		{
			Left,
			Right
		}

		public enum LocomotionOptions
		{
			Joystick1,
			Joystick2,
			Touchpad
		}

		public enum ActionOptions
		{
			Circle,
			ThreeButtons,
			HalfCircle,
			Buttons
		}

		private const string path = "Prefabs/ControlsScreen/";

		private const string joystick1PrefabPath = "Prefabs/ControlsScreen/JoyStickPanel1";

		private const string joystick2PrefabPath = "Prefabs/ControlsScreen/JoyStickPanel2";

		private const string touchpadPrefabPath = "Prefabs/ControlsScreen/JoyStickPanel3";

		private const string circleQuartersPrefabPath = "Prefabs/ControlsScreen/ButtonsPanel1";

		private const string threeButtonsPrefabPath = "Prefabs/ControlsScreen/ButtonsPanel2";

		private const string halfCirclePrefabPath = "Prefabs/ControlsScreen/ButtonsPanel4";

		private const string buttonsPrefabPath = "Prefabs/ControlsScreen/ButtonsPanel5";

		public LocomotionOptions LocomotionOption;

		public ActionOptions ActionOption;

		public ControlSlots ControlSlot;

		public Text Label;

		private void Start()
		{
			switch (ControlSlot)
			{
			case ControlSlots.Left:
				Label.text = LocomotionOption.ToString();
				break;
			case ControlSlots.Right:
				Label.text = ActionOption.ToString();
				break;
			}
		}

		public void SetOption(bool selected)
		{
			if (selected)
			{
				switch (ControlSlot)
				{
				case ControlSlots.Left:
					setLeftOption();
					break;
				case ControlSlots.Right:
					setRightOption();
					break;
				}
			}
		}

		private void setLeftOption()
		{
			switch (LocomotionOption)
			{
			case LocomotionOptions.Joystick1:
				break;
			case LocomotionOptions.Joystick2:
				break;
			case LocomotionOptions.Touchpad:
				break;
			}
		}

		private void setRightOption()
		{
			switch (ActionOption)
			{
			case ActionOptions.Circle:
				break;
			case ActionOptions.ThreeButtons:
				break;
			case ActionOptions.HalfCircle:
				break;
			case ActionOptions.Buttons:
				break;
			}
		}
	}
}
