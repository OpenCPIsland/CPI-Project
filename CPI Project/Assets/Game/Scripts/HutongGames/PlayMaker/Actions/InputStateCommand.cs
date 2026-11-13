using ClubPenguin;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Input")]
	public class InputStateCommand : FsmStateAction
	{
		[Tooltip("Interact Button has been clicked by the player.")]
		public FsmEvent InteractEvent;

		[Tooltip("Action 1 Button has been clicked by the player.")]
		public FsmEvent Action1Event;

		[Tooltip("Action 2 Button has been clicked by the player.")]
		public FsmEvent Action2Event;

		[Tooltip("Action 3 Button has been clicked by the player.")]
		public FsmEvent Action3Event;

		[Tooltip("Jump Button has been clicked by the player.")]
		public FsmEvent JumpEvent;

		[Tooltip("The player has steered the joystick.")]
		public FsmEvent SteerEvent;

		public FsmBool FindComponentsOnParent = false;

		[Tooltip("Cancel input event from the local player")]
		public FsmEvent LocalPlayerCancelEvent;

		private AvatarLocomotionStateSetter locomotionSetter;

		private RunController runController;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private AvatarLocomotionStateSetter LocomotionSetter
		{
			get
			{
				if (locomotionSetter == null)
				{
					locomotionSetter = GetComponent<AvatarLocomotionStateSetter>();
				}
				return locomotionSetter;
			}
		}

		private RunController RunController
		{
			get
			{
				if (runController == null)
				{
					runController = GetComponent<RunController>();
				}
				return runController;
			}
		}

		private LocomotionEventBroadcaster LocomotionEventBroadcaster
		{
			get
			{
				if (locomotionEventBroadcaster == null)
				{
					locomotionEventBroadcaster = GetComponent<LocomotionEventBroadcaster>();
				}
				return locomotionEventBroadcaster;
			}
		}

		private T GetComponent<T>()
		{
			if (FindComponentsOnParent.Value)
			{
				return base.Owner.GetComponentInParent<T>();
			}
			return base.Owner.GetComponent<T>();
		}

		public override void OnEnter()
		{
			if (RunController != null)
			{
				runController.OnSteer += OnPlayerMove;
			}
			if (LocomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnDoActionEvent += OnDoAction;
				locomotionEventBroadcaster.OnLocomotionStateChangedEvent += OnLocomotionStateChanged;
				locomotionEventBroadcaster.OnStickDirectionEvent += OnSteerStick;
				Service.Get<EventDispatcher>().AddListener<InputEvents.ActionEvent>(onInputEventActionEvent);
			}
			if (LocomotionSetter != null)
			{
				locomotionSetter.ActionButtonInvoked += OnActionButtonClicked;
				locomotionSetter.GenericStateButtonInvoked += OnGenericStateChanged;
			}
		}

		public override void OnExit()
		{
			if (RunController != null)
			{
				runController.OnSteer -= OnPlayerMove;
			}
			if (LocomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnDoActionEvent -= OnDoAction;
				locomotionEventBroadcaster.OnLocomotionStateChangedEvent -= OnLocomotionStateChanged;
				locomotionEventBroadcaster.OnStickDirectionEvent -= OnSteerStick;
				Service.Get<EventDispatcher>().RemoveListener<InputEvents.ActionEvent>(onInputEventActionEvent);
			}
			if (LocomotionSetter != null)
			{
				locomotionSetter.ActionButtonInvoked -= OnActionButtonClicked;
				locomotionSetter.GenericStateButtonInvoked -= OnGenericStateChanged;
			}
		}

		private void OnPlayerMove(Vector3 steer)
		{
			if (steer != Vector3.zero)
			{
				base.Fsm.Event(SteerEvent);
			}
		}

		private void OnSteerStick(Vector2 steer)
		{
			if (steer != Vector2.zero)
			{
				base.Fsm.Event(SteerEvent);
			}
		}

		private bool onInputEventActionEvent(InputEvents.ActionEvent evt)
		{
			if (evt.Action == InputEvents.Actions.Cancel)
			{
				base.Fsm.Event(LocalPlayerCancelEvent);
			}
			return false;
		}

		private void OnActionButtonClicked(LocomotionAction action)
		{
			switch (action)
			{
			case LocomotionAction.ChargeThrow:
			case LocomotionAction.LaunchThrow:
			case LocomotionAction.Torpedo:
			case LocomotionAction.SlideTrick:
				break;
			case LocomotionAction.Interact:
				base.Fsm.Event(InteractEvent);
				break;
			case LocomotionAction.Action1:
				base.Fsm.Event(Action1Event);
				break;
			case LocomotionAction.Action2:
				base.Fsm.Event(Action2Event);
				break;
			case LocomotionAction.Action3:
				base.Fsm.Event(Action3Event);
				break;
			case LocomotionAction.Jump:
				base.Fsm.Event(JumpEvent);
				break;
			}
		}

		private void OnDoAction(LocomotionController.LocomotionAction action, object userData)
		{
			switch (action)
			{
			case LocomotionController.LocomotionAction.Torpedo:
			case LocomotionController.LocomotionAction.SlideTrick:
			case LocomotionController.LocomotionAction.ChargeThrow:
			case LocomotionController.LocomotionAction.LaunchThrow:
				break;
			case LocomotionController.LocomotionAction.Interact:
				base.Fsm.Event(InteractEvent);
				break;
			case LocomotionController.LocomotionAction.Action1:
				base.Fsm.Event(Action1Event);
				break;
			case LocomotionController.LocomotionAction.Action2:
				base.Fsm.Event(Action2Event);
				break;
			case LocomotionController.LocomotionAction.Action3:
				base.Fsm.Event(Action3Event);
				break;
			case LocomotionController.LocomotionAction.Jump:
				base.Fsm.Event(JumpEvent);
				break;
			}
		}

		private void OnGenericStateChanged(LocomotionState state)
		{
			switch (state)
			{
			case LocomotionState.Slide:
				break;
			case LocomotionState.Default:
				base.Fsm.Event(InteractEvent);
				break;
			case LocomotionState.GenericState1:
				base.Fsm.Event(Action1Event);
				break;
			case LocomotionState.GenericState2:
				base.Fsm.Event(Action2Event);
				break;
			case LocomotionState.GenericState3:
				base.Fsm.Event(Action3Event);
				break;
			}
		}

		private void OnLocomotionStateChanged(LocomotionState state)
		{
			switch (state)
			{
			case LocomotionState.Slide:
				break;
			case LocomotionState.Default:
				base.Fsm.Event(InteractEvent);
				break;
			case LocomotionState.GenericState1:
				base.Fsm.Event(Action1Event);
				break;
			case LocomotionState.GenericState2:
				base.Fsm.Event(Action2Event);
				break;
			case LocomotionState.GenericState3:
				base.Fsm.Event(Action3Event);
				break;
			}
		}
	}
}
