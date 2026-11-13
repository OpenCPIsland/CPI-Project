using ClubPenguin.Consumable;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForPartySupplyPurchaseAction : FsmStateAction
	{
		public PropDefinition purchaseItem;

		public FsmEvent PurchasedEvent;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<MarketplaceEvents.ItemPurchased>(onItemPurchased);
			Service.Get<EventDispatcher>().AddListener<DisneyStoreEvents.PurchaseComplete>(onDisneyStoreItemPurchased);
		}

		public override void OnExit()
		{
			Service.Get<EventDispatcher>().RemoveListener<MarketplaceEvents.ItemPurchased>(onItemPurchased);
			Service.Get<EventDispatcher>().RemoveListener<DisneyStoreEvents.PurchaseComplete>(onDisneyStoreItemPurchased);
		}

		private bool onItemPurchased(MarketplaceEvents.ItemPurchased evt)
		{
			if (purchaseItem != null && evt.ItemDefinition.GetNameOnServer() == purchaseItem.GetNameOnServer())
			{
				base.Fsm.Event(PurchasedEvent);
			}
			return false;
		}

		private bool onDisneyStoreItemPurchased(DisneyStoreEvents.PurchaseComplete evt)
		{
			ConsumableInstanceReward rewardable;
			if (evt.Reward.TryGetValue(out rewardable) && !rewardable.IsEmpty() && rewardable.Consumables.ContainsKey(purchaseItem.GetNameOnServer()))
			{
				base.Fsm.Event(PurchasedEvent);
			}
			return false;
		}
	}
}
