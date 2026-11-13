using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class ControlScheme : ScriptableObject
	{
		[Header("Penguin Controls")]
		public LocomotionInput Locomotion;

		public KeyCodeInput Jump;

		public KeyCodeInput Action1;

		public KeyCodeInput Action2;

		public KeyCodeInput Action3;

		public KeyCodeInput Cancel;

		public KeyCodeInput WalkModifier;

		public KeyCodeInput SprintModifier;

		[Header("Nav Bar")]
		public KeyCodeInput Profile;

		public KeyCodeInput Consumables;

		public KeyCodeInput Quest;

		public KeyCodeInput Map;

		[Header("Cellphone")]
		public KeyCodeInput Cellphone;

		[Header("UI")]
		public KeyCodeInput UI_Accept;

		public KeyCodeInput UI_Cancel;

		public KeyCodeInput UI_Navigation;

		public KeyCodeInput UI_NavigationBackwards;

		public KeyCodeInput UI_Submit;

		[Header("Chat")]
		public KeyCodeInput Chat;

		public KeyCodeInput QuickChat;

		public KeyCodeInput QuickEmote;

		public KeyCodeInput SendChat;

		[Header("Misc")]
		public KeyCodeInput Back;

		public KeyCodeInput Left;

		public KeyCodeInput Right;

		[NonSerialized]
		public KeyCodeInput MergedBackMap;

		[NonSerialized]
		public KeyCodeInput MergedBackCellphone;

		private List<InputLib> inputList;

		private void populateInputList()
		{
			inputList = new List<InputLib>
			{
				Locomotion,
				Jump,
				Action1,
				Action2,
				Action3,
				Cancel,
				WalkModifier,
				SprintModifier,
				Profile,
				Consumables,
				Quest,
				Map,
				Cellphone,
				UI_Accept,
				UI_Cancel,
				UI_Navigation,
				UI_NavigationBackwards,
				UI_Submit,
				Chat,
				QuickChat,
				QuickEmote,
				SendChat,
				Back,
				Left,
				Right,
				MergedBackMap,
				MergedBackCellphone
			};
		}

		public void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			createMergedBackMap();
			createMergedBackCellphone();
			if (SprintModifier == null)
			{
				SprintModifier = ScriptableObject.CreateInstance<KeyCodeInput>();
				SprintModifier.Keys = new KeyCode[0];
			}
			ApplyDefaultGamepadBindings();
			populateInputList();
			foreach (InputLib input in inputList)
			{
				input.Initialize(keyCodeRemapper);
			}
		}

		public void StartFrame()
		{
			foreach (InputLib input in inputList)
			{
				input.StartFrame();
			}
		}

		public void EndFrame()
		{
			foreach (InputLib input in inputList)
			{
				input.EndFrame();
			}
		}

		private void createMergedBackMap()
		{
			MergedBackMap = ScriptableObject.CreateInstance<KeyCodeInput>();
			MergedBackMap.Keys = new KeyCode[Back.Keys.Length + Map.Keys.Length];
			Back.Keys.CopyTo(MergedBackMap.Keys, 0);
			Map.Keys.CopyTo(MergedBackMap.Keys, Back.Keys.Length);
		}

		private void createMergedBackCellphone()
		{
			MergedBackCellphone = ScriptableObject.CreateInstance<KeyCodeInput>();
			MergedBackCellphone.Keys = new KeyCode[Back.Keys.Length + Cellphone.Keys.Length];
			Back.Keys.CopyTo(MergedBackCellphone.Keys, 0);
			Cellphone.Keys.CopyTo(MergedBackCellphone.Keys, Back.Keys.Length);
		}

		private void ApplyDefaultGamepadBindings()
		{
			if (Jump != null) Jump.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonSouth);

			if (Action1 != null) Action1.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonWest);
			if (Action2 != null) Action2.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonNorth);
			if (Action3 != null) Action3.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonEast);

			if (WalkModifier != null) WalkModifier.SetGamepadBindings(ActiveInputDevice.GamepadControl.LeftTrigger);
			if (SprintModifier != null) SprintModifier.SetGamepadBindings(ActiveInputDevice.GamepadControl.RightTrigger);

			if (Chat != null) Chat.SetGamepadBindings(ActiveInputDevice.GamepadControl.LeftShoulder);
			if (QuickEmote != null) QuickEmote.SetGamepadBindings(ActiveInputDevice.GamepadControl.RightShoulder);

			if (UI_Submit != null) UI_Submit.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonSouth);
			if (UI_Accept != null) UI_Accept.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonSouth);
			if (UI_Cancel != null) UI_Cancel.SetGamepadBindings(ActiveInputDevice.GamepadControl.ButtonEast);

			if (UI_Navigation != null) UI_Navigation.SetGamepadBindings(ActiveInputDevice.GamepadControl.DpadRight);
			if (UI_NavigationBackwards != null) UI_NavigationBackwards.SetGamepadBindings(ActiveInputDevice.GamepadControl.DpadLeft);

			if (Cancel != null) Cancel.SetGamepadBindings(ActiveInputDevice.GamepadControl.Select);
			if (Back != null) Back.SetGamepadBindings(ActiveInputDevice.GamepadControl.Select);
		}

	}
}
