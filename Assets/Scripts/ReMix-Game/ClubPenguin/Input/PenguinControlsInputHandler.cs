using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client.Event;
using ClubPenguin.UI;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class PenguinControlsInputHandler : PenguinControlsInputHandlerLib<PenguinControlsInputMap.Result>
	{
		[Header("Buttons")]
		[SerializeField]
		private InputMappedButton btnJump = null;

		[SerializeField]
		private InputMappedButton btnAction1 = null;

		[SerializeField]
		private InputMappedButton btnAction2 = null;

		[SerializeField]
		private InputMappedButton btnAction3 = null;

		[SerializeField]
		private InputMappedButton btnCancel = null;

		private bool walkModifier;

		private RunController runController;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		protected override void identifiedLocalPlayer(GameObject localPlayer)
		{
			locomotionEventBroadcaster = localPlayer.GetComponent<LocomotionEventBroadcaster>();
			runController = localPlayer.GetComponent<RunController>();
			base.identifiedLocalPlayer(localPlayer);
		}

		protected override void OnEnable()
		{
			locomotionEventBroadcaster.OnControllerChangedEvent += onControllerChanged;
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			locomotionEventBroadcaster.OnControllerChangedEvent -= onControllerChanged;
			base.OnDisable();
		}

		protected override void onHandle(PenguinControlsInputMap.Result inputResult)
		{
			handleLocomotionInput(inputResult.Locomotion);
			handleButtonInput(btnJump, inputResult.Jump);
			handleButtonInput(btnAction1, inputResult.Action1);
			handleButtonInput(btnAction2, inputResult.Action2);
			handleButtonInput(btnAction3, inputResult.Action3);
			handleButtonInput(btnCancel, inputResult.Cancel);
			walkModifier = inputResult.WalkModifier.IsHeld;
			if ((inputResult.WalkModifier.WasJustPressed || inputResult.WalkModifier.WasJustReleased) && runController != null && runController.enabled)
			{
				runController.Behaviour.SetStyle(walkModifier ? PlayerLocoStyle.Style.Walk : PlayerLocoStyle.Style.Run);
			}
		}

		protected override void onReset()
		{
			eventDispatcher.DispatchEvent(new InputEvents.MoveEvent(Vector2.zero));
			handleButtonInput(btnJump);
			handleButtonInput(btnAction1);
			handleButtonInput(btnAction2);
			handleButtonInput(btnAction3);
			handleButtonInput(btnCancel);
			walkModifier = false;
			if (runController != null && runController.enabled)
			{
				runController.Behaviour.SetStyle(PlayerLocoStyle.Style.Run);
			}
		}

		private void handleButtonInput(InputMappedButton button, ButtonInputResult buttonInput = null)
		{
			if (button != null)
			{
				MemberLockedTrayInputButton componentInChildren = button.GetComponentInChildren<MemberLockedTrayInputButton>();
				InputMappedButton inputMappedButton = button;
				if (componentInChildren != null && componentInChildren.IsLocked && componentInChildren.MemberLock != null)
				{
					inputMappedButton = componentInChildren.MemberLock.GetComponent<InputMappedButton>();
				}
				if (inputMappedButton != null)
				{
					inputMappedButton.HandleMappedInput(buttonInput);
				}
			}
		}

		private void onControllerChanged(LocomotionController newController)
		{
			if (runController != null && runController.enabled)
			{
				runController.Behaviour.SetStyle(walkModifier ? PlayerLocoStyle.Style.Walk : PlayerLocoStyle.Style.Run);
			}
		}
	}
}
