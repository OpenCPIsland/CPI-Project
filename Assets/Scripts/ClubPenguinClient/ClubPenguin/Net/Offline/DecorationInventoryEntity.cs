using ClubPenguin.Net.Domain.Decoration;
using LitJson;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct DecorationInventoryEntity : IOfflineData
	{
		public class InventoryWrapper
		{
			private Dictionary<string, int> inventory;

			public int this[DecorationId id]
			{
				get
				{
					return inventory[id.ToString()];
				}
				set
				{
					inventory[id.ToString()] = value;
				}
			}

			public InventoryWrapper(Dictionary<string, int> inventory)
			{
				this.inventory = inventory;
			}

			public bool ContainsKey(DecorationId key)
			{
				return inventory.ContainsKey(key.ToString());
			}
		}

		public Dictionary<string, int> inventory;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		private InventoryWrapper inventoryWrapper;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public InventoryWrapper Inventory
		{
			get
			{
				if (inventoryWrapper == null)
				{
					if (inventory == null)
					{
						Init();
					}
					inventoryWrapper = new InventoryWrapper(inventory);
				}
				return inventoryWrapper;
			}
		}

		public void Init()
		{
			inventory = new Dictionary<string, int>();
		}
	}
}
