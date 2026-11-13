using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct ConsumableInventory : IOfflineData
	{
		public Dictionary<string, InventoryItemStock> Inventory;

		public void Init()
		{
			Inventory = new Dictionary<string, InventoryItemStock>();
		}
	}
}
