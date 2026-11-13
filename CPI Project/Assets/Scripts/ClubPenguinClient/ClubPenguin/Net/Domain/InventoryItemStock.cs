namespace ClubPenguin.Net.Domain
{
	public class InventoryItemStock
	{
		public int itemCount;

		public int partialCount;

		public long lastPurchaseTimestamp;

		public int GetItemCount()
		{
			if (partialCount > 0)
			{
				return itemCount + 1;
			}
			return itemCount;
		}

		public void DecrementCount()
		{
			if (partialCount > 0)
			{
				partialCount = 0;
			}
			else
			{
				itemCount--;
			}
		}
	}
}
