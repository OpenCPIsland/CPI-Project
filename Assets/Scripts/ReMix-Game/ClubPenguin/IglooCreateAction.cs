using ClubPenguin.Igloo;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class IglooCreateAction : FsmStateAction
	{
		public FsmEvent OnSuccess;

		public FsmEvent OnFail;

		private EventDispatcher eventDispatcher;

		public override void OnEnter()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<IglooEvents.CreateIgloo>(onCreateIgloo);
		}

		public override void OnExit()
		{
			eventDispatcher.RemoveListener<IglooEvents.CreateIgloo>(onCreateIgloo);
			eventDispatcher.RemoveListener<IglooUIEvents.ManageIglooPopupDisplayed>(onManageIglooPopupDisplayed);
		}

		private bool onCreateIgloo(IglooEvents.CreateIgloo evt)
		{
			eventDispatcher.RemoveListener<IglooEvents.CreateIgloo>(onCreateIgloo);
			if (evt.Success)
			{
				base.Fsm.Event(OnSuccess);
			}
			else
			{
				eventDispatcher.AddListener<IglooUIEvents.ManageIglooPopupDisplayed>(onManageIglooPopupDisplayed);
			}
			return false;
		}

		private bool onManageIglooPopupDisplayed(IglooUIEvents.ManageIglooPopupDisplayed evt)
		{
			eventDispatcher.RemoveListener<IglooUIEvents.ManageIglooPopupDisplayed>(onManageIglooPopupDisplayed);
			base.Fsm.Event(OnFail);
			return false;
		}
	}
}
