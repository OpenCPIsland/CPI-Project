using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class ConsumableInventoryData : ScopedData
	{
		private ConsumableInventory consumableInventory = default(ConsumableInventory);

		public ConsumableInventory ConsumableInventory
		{
			get
			{
				return consumableInventory;
			}
			internal set
			{
				if (this.OnConsumableInventoryChanged != null)
				{
					this.OnConsumableInventoryChanged(value);
				}
				consumableInventory = value;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ConsumableInventoryDataMonoBehaviour);
			}
		}

		public event Action<ConsumableInventory> OnConsumableInventoryChanged;

		public void UseConsumable(string consumableType)
		{
			ConsumableInventory consumableInventory = default(ConsumableInventory);
			consumableInventory.inventoryMap = new Dictionary<string, InventoryItemStock>(this.consumableInventory.inventoryMap);
			InventoryItemStock inventoryItemStock = consumableInventory.inventoryMap[consumableType];
			inventoryItemStock.DecrementCount();
			ConsumableInventory = consumableInventory;
		}

		public void AddConsumable(string consumableType, int consumableCount)
		{
			if (!consumableInventory.inventoryMap.ContainsKey(consumableType))
			{
				consumableInventory.inventoryMap[consumableType] = new InventoryItemStock();
			}
			InventoryItemStock inventoryItemStock = consumableInventory.inventoryMap[consumableType];
			inventoryItemStock.itemCount += consumableCount;
		}

		protected override void notifyWillBeDestroyed()
		{
			this.OnConsumableInventoryChanged = null;
		}
	}
}
