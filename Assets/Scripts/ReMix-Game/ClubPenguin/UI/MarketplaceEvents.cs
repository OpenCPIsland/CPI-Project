using ClubPenguin.Props;

namespace ClubPenguin.UI
{
	public class MarketplaceEvents
	{
		public struct ItemPurchased
		{
			public PropDefinition ItemDefinition;

			public int Quantity;

			public ItemPurchased(PropDefinition itemDefinition, int quantity)
			{
				ItemDefinition = itemDefinition;
				Quantity = quantity;
			}
		}

		public struct MarketplaceOpened
		{
			public string MarketplaceName;

			public MarketplaceOpened(string marketplaceName)
			{
				MarketplaceName = marketplaceName;
			}
		}

		public struct MarketplaceClosed
		{
			public string MarketplaceName;

			public MarketplaceClosed(string marketplaceName)
			{
				MarketplaceName = marketplaceName;
			}
		}
	}
}
