using ClubPenguin.Interactables.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class ReceivedInteractablesEventCommand : FsmStateAction
	{
		public FsmOwnerDefault InteractableTarget;

		public FsmEvent OnDoneInteractingEvent;

		public FsmEvent OnAction1Event;

		public FsmEvent OnAction2Event;

		public FsmEvent OnAction3Event;

		private GameObject interactableTarget;

		public override void Reset()
		{
			OnDoneInteractingEvent = null;
			OnAction1Event = null;
			OnAction2Event = null;
			OnAction3Event = null;
		}

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<InteractablesEvents.ActionEvent>(OnAction);
			interactableTarget = ((InteractableTarget.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : InteractableTarget.GameObject.Value);
		}

		public override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<InteractablesEvents.ActionEvent>(OnAction);
		}

		private bool OnAction(InteractablesEvents.ActionEvent evt)
		{
			if (interactableTarget != null && evt.InteractableTarget.Equals(interactableTarget))
			{
				switch (evt.Action)
				{
				case InteractablesEvents.Actions.DoneInteracting:
					if (OnDoneInteractingEvent != null)
					{
						base.Fsm.Event(OnDoneInteractingEvent);
					}
					break;
				case InteractablesEvents.Actions.Action1:
					if (OnAction1Event != null)
					{
						base.Fsm.Event(OnAction1Event);
					}
					break;
				case InteractablesEvents.Actions.Action2:
					if (OnAction2Event != null)
					{
						base.Fsm.Event(OnAction2Event);
					}
					break;
				case InteractablesEvents.Actions.Action3:
					if (OnAction3Event != null)
					{
						base.Fsm.Event(OnAction3Event);
					}
					break;
				}
			}
			return false;
		}
	}
}
