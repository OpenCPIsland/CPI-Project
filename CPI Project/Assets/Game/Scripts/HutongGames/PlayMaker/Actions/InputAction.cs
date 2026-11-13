using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sends an Event when specified input is received.")]
	[ActionCategory("Locomotion")]
	public class InputAction : FsmStateAction
	{
		public FsmEvent OnJumpEvent;

		public FsmEvent OnInteractEvent;

		public FsmEvent OnThrowEvent;

		public FsmEvent OnSlideEvent;

		public FsmEvent OnBoostEvent;

		public FsmEvent OnAction1Event;

		public FsmEvent OnAction2Event;

		public FsmEvent OnAction3Event;

		public FsmEvent OnCancel;

		public FsmEvent OnVirtualJoystickMoveEvent;

		private EventDispatcher dispatcher;

		public override void Reset()
		{
			OnJumpEvent = null;
			OnInteractEvent = null;
			OnThrowEvent = null;
			OnSlideEvent = null;
			OnBoostEvent = null;
			OnAction1Event = null;
			OnAction2Event = null;
			OnAction3Event = null;
			OnCancel = null;
			OnVirtualJoystickMoveEvent = null;
		}

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<InputEvents.ActionEvent>(OnAction);
			dispatcher.AddListener<InputEvents.SwitchChangeEvent>(OnSwitchChange);
			dispatcher.AddListener<InputEvents.MoveEvent>(OnInputMoveEvent);
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<InputEvents.ActionEvent>(OnAction);
			dispatcher.RemoveListener<InputEvents.SwitchChangeEvent>(OnSwitchChange);
			dispatcher.RemoveListener<InputEvents.MoveEvent>(OnInputMoveEvent);
		}

		private bool OnAction(InputEvents.ActionEvent evt)
		{
			switch (evt.Action)
			{
			case InputEvents.Actions.Jump:
				if (OnJumpEvent != null)
				{
					base.Fsm.Event(OnJumpEvent);
				}
				break;
			case InputEvents.Actions.Interact:
				if ((!Service.Get<PropService>().IsLocalUserUsingProp() || Service.Get<PropService>().CanActionsBeUsedWithProp()) && OnInteractEvent != null)
				{
					base.Fsm.Event(OnInteractEvent);
				}
				break;
			case InputEvents.Actions.Snowball:
				if (OnThrowEvent != null)
				{
					base.Fsm.Event(OnThrowEvent);
				}
				break;
			case InputEvents.Actions.Action1:
				if (OnAction1Event != null)
				{
					base.Fsm.Event(OnAction1Event);
				}
				break;
			case InputEvents.Actions.Action2:
				if (OnAction2Event != null)
				{
					base.Fsm.Event(OnAction2Event);
				}
				break;
			case InputEvents.Actions.Action3:
				if (OnAction3Event != null)
				{
					base.Fsm.Event(OnAction3Event);
				}
				break;
			case InputEvents.Actions.Cancel:
				if (OnCancel != null)
				{
					base.Fsm.Event(OnCancel);
				}
				break;
			case InputEvents.Actions.Torpedo:
				if (OnBoostEvent != null)
				{
					base.Fsm.Event(OnBoostEvent);
				}
				break;
			}
			return false;
		}

		private bool OnSwitchChange(InputEvents.SwitchChangeEvent evt)
		{
			if (OnSlideEvent != null)
			{
				base.Fsm.Event(OnSlideEvent);
			}
			return false;
		}

		private bool OnInputMoveEvent(InputEvents.MoveEvent evt)
		{
			if (OnVirtualJoystickMoveEvent != null && evt.Direction.magnitude > 0f)
			{
				base.Fsm.Event(OnVirtualJoystickMoveEvent);
			}
			return false;
		}
	}
}
