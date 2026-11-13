using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForMarketplaceEventAction : FsmStateAction
	{
		public string MarketplaceName;

		public FsmEvent OpenedEvent;

		public FsmEvent ClosedEvent;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<MarketplaceEvents.MarketplaceOpened>(onMarketplaceOpened);
			dispatcher.AddListener<MarketplaceEvents.MarketplaceClosed>(onMarketplaceClosed);
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<MarketplaceEvents.MarketplaceOpened>(onMarketplaceOpened);
			dispatcher.RemoveListener<MarketplaceEvents.MarketplaceClosed>(onMarketplaceClosed);
		}

		private bool onMarketplaceOpened(MarketplaceEvents.MarketplaceOpened evt)
		{
			if (MarketplaceName == "" || evt.MarketplaceName == MarketplaceName)
			{
				base.Fsm.Event(OpenedEvent);
			}
			return false;
		}

		private bool onMarketplaceClosed(MarketplaceEvents.MarketplaceClosed evt)
		{
			if (MarketplaceName == "" || evt.MarketplaceName == MarketplaceName)
			{
				base.Fsm.Event(ClosedEvent);
			}
			return false;
		}
	}
}
