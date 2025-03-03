using ClubPenguin.Net.Domain;

namespace ClubPenguin.UI
{
	public static class DisneyStoreEvents
	{
		public struct PurchaseComplete
		{
			public readonly Reward Reward;

			public PurchaseComplete(Reward reward)
			{
				Reward = reward;
			}
		}
	}
}
