namespace ClubPenguin.Collectibles
{
	public static class CollectibleEvents
	{
		public struct CollectibleAdd
		{
			public readonly string Type;

			public readonly int Amount;

			public CollectibleAdd(string type, int amount)
			{
				Type = type;
				Amount = amount;
			}
		}
	}
}
