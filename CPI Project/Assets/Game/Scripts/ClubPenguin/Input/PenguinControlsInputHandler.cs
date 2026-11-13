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

		[SerializeField]
		private InputMappedButton btnSprint = null;

		private bool walkModifier;

		private bool sprintModifier;

		private bool runActionSprintModifier;

		private bool isTubeEnabled;

		private RunController runController;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		protected override void identifiedLocalPlayer(GameObject localPlayer)
		{
			locomotionEventBroadcaster = localPlayer.GetComponent<LocomotionEventBroadcaster>();
			runController = localPlayer.GetComponent<RunController>();
			isTubeEnabled = LocomotionHelper.IsCurrentControllerOfType<SlideController>(localPlayer);
			runActionSprintModifier = runController != null && runController.IsRunActionSprintModifier;
			base.identifiedLocalPlayer(localPlayer);
		}

		protected override void OnEnable()
		{
			locomotionEventBroadcaster.OnControllerChangedEvent += onControllerChanged;
			eventDispatcher.AddListener<InputEvents.ActionEvent>(onAction);
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			locomotionEventBroadcaster.OnControllerChangedEvent -= onControllerChanged;
			eventDispatcher.RemoveListener<InputEvents.ActionEvent>(onAction);
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
			handleButtonInput(btnSprint, inputResult.SprintModifier);
			walkModifier = inputResult.WalkModifier.IsHeld;
			sprintModifier = inputResult.SprintModifier.IsHeld;
			setSprintButtonState(sprintModifier || runActionSprintModifier);
			if (runController != null && runController.enabled)
			{
				runController.SetSprintModifier(sprintModifier);
			}
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
			handleButtonInput(btnSprint);
			walkModifier = false;
			sprintModifier = false;
			runActionSprintModifier = runController != null && runController.IsRunActionSprintModifier;
			setSprintButtonState(runActionSprintModifier);
			if (runController != null && runController.enabled)
			{
				runController.Behaviour.SetStyle(PlayerLocoStyle.Style.Run);
				runController.SetSprintModifier(false);
			}
		}

		private void setSprintButtonState(bool isSprinting)
		{
			if (btnSprint != null)
			{
				TrayInputButton trayInputButton = btnSprint.GetComponentInParent<TrayInputButton>();
				if (trayInputButton != null && !trayInputButton.IsLocked)
				{
					TrayInputButton.ButtonState buttonState = isTubeEnabled
						? TrayInputButton.ButtonState.Disabled
						: (isSprinting ? TrayInputButton.ButtonState.Highlighted : TrayInputButton.ButtonState.Default);
					trayInputButton.SetState(buttonState);
				}
			}
		}

		private bool onAction(InputEvents.ActionEvent evt)
		{
#if UNITY_IOS || UNITY_ANDROID
			if (evt.Action == InputEvents.Actions.Sprint)
			{
				runActionSprintModifier = !runActionSprintModifier;
				setSprintButtonState(sprintModifier || runActionSprintModifier);
			}
#endif
			return false;
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
			isTubeEnabled = newController is SlideController;
			if (newController == runController)
			{
				runActionSprintModifier = runController.IsRunActionSprintModifier;
			}
			setSprintButtonState(sprintModifier || runActionSprintModifier);
			if (runController != null && runController.enabled)
			{
				runController.Behaviour.SetStyle(walkModifier ? PlayerLocoStyle.Style.Walk : PlayerLocoStyle.Style.Run);
				runController.SetSprintModifier(sprintModifier);
			}
		}
	}
}
