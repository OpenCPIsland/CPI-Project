using ClubPenguin.Core;
using ClubPenguin.Interactables.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class WaitForUseInvitationalItemAction : FsmStateAction
	{
		public FsmString ItemName;

		public FsmEvent UseEvent;

		public FsmEvent CompleteEvent;

		public FsmEvent CancelEvent;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<InteractablesEvents.InvitationalItemUsed>(onItemUsed);
			Service.Get<EventDispatcher>().AddListener<InputEvents.ActionEvent>(onAction);
		}

		public override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<InteractablesEvents.InvitationalItemUsed>(onItemUsed);
			Service.Get<EventDispatcher>().RemoveListener<InputEvents.ActionEvent>(onAction);
		}

		private bool onItemUsed(InteractablesEvents.InvitationalItemUsed evt)
		{
			string a = evt.Item.name.Replace("(Clone)", "");
			if (a == ItemName.Value)
			{
				if (evt.QuantityLeft <= 0)
				{
					base.Fsm.Event(CompleteEvent);
				}
				else
				{
					base.Fsm.Event(UseEvent);
				}
			}
			return false;
		}

		private bool onAction(InputEvents.ActionEvent evt)
		{
			if (evt.Action == InputEvents.Actions.Cancel)
			{
				base.Fsm.Event(CancelEvent);
			}
			return false;
		}
	}
}
